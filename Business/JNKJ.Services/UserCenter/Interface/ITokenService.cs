using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Results;
using JNKJ.Dto.UserCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Services.UserCenter.Interface
{
    /// <summary>
    /// Token验证的服务接口
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// 创建token
        /// JWT实际上就是一个字符串，一般由三段构成，用.号分隔开：第一段是header(头部)，头部信息主要包括（参数的类型--JWT,签名的算法--HS256）；
        /// 第二段是payload(载荷)，负荷基本就是自己想要存放的信息(因为信息会暴露，不应该在载荷里面加入任何敏感的数据)；
        /// 第三段是signature(签名)，签名的作用就是为了防止恶意篡改数据
        /// 采用用户GUID+登录时间进行DES加密
        /// </summary>
        /// <param name="request">创建Token请求类</param>
        /// <returns></returns>
        TokenInfo GenerateToken(GenerateTokenRequest request);

        /// <summary>
        /// token解密
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool TokenDecryption(string token, out string errMsg, out AuthInfo json);

        /// <summary>
        /// 根据用户Id获取token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        UserAuthentication GetToken(GenerateTokenRequest request);

        /// <summary>
        /// 更新用户认证信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse Update(UserAuthentication request);

        /// <summary>
        /// DES解密方法
        /// </summary>
        /// <param name="signStr">待解密的字符串</param>
        /// <param name="key">DES密钥,为8位或16位</param>
        /// <returns>解密后的字符串</returns>
        string DESDecrypt(string signStr);


    }
}
