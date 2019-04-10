using JNKJ.Core.Data;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.UserCenter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.IO;
using System.Linq.Expressions;
using JNKJ.Dto.Enums;
using JNKJ.Core;
using System.Configuration;
using JNKJ.Dto.Results;

namespace JNKJ.Services.UserCenter.Realize
{
    /// <summary>
    /// Token验证的服务
    /// </summary>
    public class TokenService : ITokenService
    {
        #region Fields
        private readonly IRepository<UserAuthentication> _userAuthenticationRepository;
        public static readonly string SecretKey = ConfigurationManager.AppSettings["TokenSecret"];//这个服务端加密秘钥 属于私钥
        public static readonly string key = ConfigurationManager.AppSettings["DesKey"];//DES密钥,为8位或16位
        private int CreatedOve = int.Parse(ConfigurationManager.AppSettings["CreatedOve"]); //Token生命周期，单位为小时

        #endregion
        #region Ctor
        public TokenService(IRepository<UserAuthentication> userAuthenticationRepository)
        {
            _userAuthenticationRepository = userAuthenticationRepository;
        }
        #endregion
        /// <summary>
        /// 创建token
        /// JWT实际上就是一个字符串，一般由三段构成，用.号分隔开：第一段是header(头部)，头部信息主要包括（参数的类型--JWT,签名的算法--HS256）；
        /// 第二段是payload(载荷)，负荷基本就是自己想要存放的信息(因为信息会暴露，不应该在载荷里面加入任何敏感的数据)；
        /// 第三段是signature(签名)，签名的作用就是为了防止恶意篡改数据
        /// 采用用户GUID+登录时间进行DES加密
        /// </summary>
        /// <param name="request">创建Token请求类</param>
        /// <returns></returns>
        public TokenInfo GenerateToken(GenerateTokenRequest request)
        {
            TokenInfo tokenInfo = new TokenInfo();//需要返回的口令信息
            //根据不同参数拼接筛选条件
            Expression<Func<UserAuthentication, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted && c.SecretKey == SecretKey;
            //判断参数是否合法
            if (request.UserId != Guid.Empty)
            {
                wheres = wheres.And(c => c.UserId == request.UserId);
            }
            var dbUserAuthentication = _userAuthenticationRepository.Table.Where(wheres).OrderByDescending(t => t.CreatTime).FirstOrDefault();
            //当前用户在用户认证表中无认证数据，则创建一个Token
            if (dbUserAuthentication == null)
            {
                try
                {
                    var token = CreateToken(request); //生成token
                    //认证信息入库
                    _userAuthenticationRepository.Insert(new UserAuthentication
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        SecretKey = SecretKey,
                        ExpiryDateTime = DateTime.Now.AddHours(CreatedOve),
                        Token = token,
                        IsApp = request.IsApp,
                        UpdateTime = DateTime.Now,
                        CreatTime = DateTime.Now,
                        DeletedState = 0,
                        DeletedTime = null,
                    });
                    //口令信息
                    tokenInfo.Success = true;
                    tokenInfo.Token = token;
                    tokenInfo.Message = "OK";
                }
                catch (Exception ex)
                {
                    tokenInfo.Success = false;
                    tokenInfo.Message = ex.Message.ToString();
                }
            }
            else //当前用户在用户认证表中有认证数据，返回数据库中的Token
            {
                //判断口令过期时间 --- 未过期，返回数据库中token
                if (dbUserAuthentication.ExpiryDateTime > DateTime.Now)
                {
                    dbUserAuthentication.UpdateTime = DateTime.Now; // 更新时间更新为当前时间                   
                }
                else  //口令已过期，更新过期时间和token后返回数据

                {
                    dbUserAuthentication.UpdateTime = DateTime.Now; // 更新时间更新为当前时间
                    dbUserAuthentication.ExpiryDateTime = DateTime.Now.AddHours(CreatedOve); //修改过期时间
                    dbUserAuthentication.Token = CreateToken(request);//生成token
                    //tokenInfo.Success = false;
                    //tokenInfo.Message = "当前用户口令已过期，请重新登陆";
                }
                _userAuthenticationRepository.Update(dbUserAuthentication);
                tokenInfo.Success = true;
                tokenInfo.Token = dbUserAuthentication.Token;
                tokenInfo.Message = "OK";
            }
            return tokenInfo;
        }

        /// <summary>
        /// token解密
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool TokenDecryption(string token, out string errMsg, out AuthInfo json)
        {
            errMsg = "当前令牌无效";
            byte[] key = Encoding.UTF8.GetBytes(SecretKey);
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
            //先进行DES解密，在进行base64解密
            json = decoder.DecodeToObject<AuthInfo>(DESDecrypt(token), key, verify: true);
            if (json != null)
            {
                //根据用户ID获取token
                var userToken = GetToken(new GenerateTokenRequest
                {
                    UserId = json.UserId,
                    IsApp = json.IsApp
                });
                //判断数据库中的token是否与传过来的相同
                if (userToken != null && userToken.Token == token)
                {
                    //判断口令过期时间
                    if (json.ExpiryDateTime < DateTime.Now)
                    {
                        errMsg = "当前令牌已过期，请重新登陆";
                        return false;
                    }
                    //更新时间，每次调用接口时，判断当前时间和更新时间的时间间隔是否大于3小时并且小于过期时间一小时
                    TimeSpan ts = DateTime.Now - userToken.UpdateTime; TimeSpan ts1 = userToken.ExpiryDateTime - userToken.UpdateTime;
                    //判断当前时间和更新时间的时间间隔是否大于3小时并且小于过期时间一小时
                    if (ts.TotalMinutes >= 180 && ts1.TotalMinutes >= 60)
                    {
                        errMsg = "您长期未登录，令牌已失效，请重新登陆";
                        return false;
                    }
                    else if (ts.Hours < 3 && ts1.TotalMinutes >= 60)
                    {
                        //更新时间更新
                        userToken.UpdateTime = DateTime.Now;
                        Update(userToken);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 更新用户认证信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse Update(UserAuthentication request)
        {
            if (request == null || request?.Id == Guid.Empty)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入必要参数");
            }
            var userAuthentication = _userAuthenticationRepository.GetById(request.Id);
            if (userAuthentication != null)
            {
                userAuthentication.UpdateTime = request.UpdateTime;
                userAuthentication.UserId = request.UserId;
                userAuthentication.IsApp = request.IsApp;
                userAuthentication.ExpiryDateTime = request.ExpiryDateTime;
                userAuthentication.Token = request.Token;
                userAuthentication.SecretKey = request.SecretKey;
            }

            try
            {
                _userAuthenticationRepository.Update(userAuthentication);
                return new JsonResponse(OperatingState.Success, "更新用户认证信息成功");
            }
            catch (Exception ex)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(ex.Message, ex);
                return new JsonResponse(OperatingState.Failure, "更新用户认证信息失败");
            }
        }


        /// <summary>
        /// 根据用户Id获取token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UserAuthentication GetToken(GenerateTokenRequest request)
        {
            var dbUserAuthentication = _userAuthenticationRepository.Table.Where(t => t.DeletedState != (int)DeletedStates.Deleted && t.UserId == request.UserId && t.IsApp == request.IsApp).OrderByDescending(t => t.CreatTime).FirstOrDefault();
            if (dbUserAuthentication != null)
            {
                return dbUserAuthentication;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// DES解密方法
        /// </summary>
        /// <param name="signStr">待解密的字符串</param>
        /// <param name="key">DES密钥,为8位或16位</param>
        /// <returns>解密后的字符串</returns>
        public string DESDecrypt(string signStr)
        {
            MemoryStream ms = new MemoryStream();

            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[signStr.Length / 2];
                for (int x = 0; x < signStr.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(signStr.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                des.Key = ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);

                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();

            }
            catch
            {

            }
            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }



        #region private
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string CreateToken(GenerateTokenRequest request)
        {
            var payload = new AuthInfo
            {
                UserId = request.UserId,
                IsApp = request.IsApp,
                LoginTime = DateTime.Now,
                ExpiryDateTime = DateTime.Now.AddHours(CreatedOve),
            };
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//加密方式
            IJsonSerializer serializer = new JsonNetSerializer();//序列化Json
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();//base64加解密
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);//JWT编码
            var token = DESEncrypt(encoder.Encode(payload, SecretKey));//生成令牌，DES加密
            return token;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="signStr"></param>
        /// <returns></returns>
        private string MD5Encrypt(string signStr)
        {
            var hash = MD5.Create();
            //将字符串中字符按升序排序
            var sortStr = string.Concat(signStr.OrderBy(c => c));
            var bytes = Encoding.UTF8.GetBytes(sortStr);
            //使用MD5加密
            var md5Val = hash.ComputeHash(bytes);
            //把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            foreach (var c in md5Val)
            {
                result.Append(c.ToString("X2"));
            }
            return result.ToString().ToUpper();
        }


        /// <summary>
        /// DES加密方法
        /// </summary>
        /// <param name="encryptedValue">要加密的字符串</param>
        /// <param name="key">DES加密密钥,为8位或16位</param>
        /// <returns>加密后的字符串</returns>
        private string DESEncrypt(string signStr)
        {
            StringBuilder ret = new StringBuilder();
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.Default.GetBytes(signStr);
                des.Key = Encoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
            }
            catch
            {

            }
            return ret.ToString();
        }


        #endregion
    }
}
