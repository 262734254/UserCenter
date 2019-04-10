using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.UserCenter.Interface;
using JNKJ.WebAPI.App_Start;

namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{
    /// <summary>
    /// Token操作的控制器
    /// </summary>
    public class TokenController : BaseController
    {
        private readonly ITokenService _tokenService;
        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        // GET: UserCenter/Token
        #region 读取错误信息
        /// <summary>
        /// 读取错误信息
        /// </summary>
        /// <returns></returns>
        public string GetError()
        {
            var errors = ModelState.Values;
            foreach (var item in errors)
            {
                foreach (var item2 in item.Errors)
                {
                    if (!string.IsNullOrEmpty(item2.ErrorMessage))
                    {
                        return item2.ErrorMessage;
                    }
                }
            }
            return "";
        }
        #endregion

        /// <summary>
        /// 创建token
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        public object gen_token([FromUri]GenerateTokenRequest request)
        {
            if (ModelState.IsValid)//是否通过Model验证
            {
                return _tokenService.GenerateToken(request);
            }
            else
            {
                return GetError();
            }
        }

        /// <summary>
        /// token解密
        /// </summary>
        /// <param name="tokrn"></param>
        /// <returns></returns>
        [HttpGet]
        public object token_decryption(string token)
        {
            if (ModelState.IsValid)//是否通过Model验证
            {
                string errMsg = "当前令牌已过期，请重新登陆";
                AuthInfo json = new AuthInfo();
                var result = _tokenService.TokenDecryption(token, out errMsg, out json);
                return new { result, errMsg, json };
            }
            else
            {
                return GetError();
            }
        }

        /// <summary>
        /// 根据用户Id获取token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public object get_token([FromUri]GenerateTokenRequest request)
        {
            if (ModelState.IsValid)//是否通过Model验证
            {
                return _tokenService.GetToken(request);
            }
            else
            {
                return GetError();
            }
        }
        /// <summary>
        /// 更新用户认证信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public object update_token_updatetime(UserAuthentication request)
        {
            if (ModelState.IsValid)//是否通过Model验证
            {
                return _tokenService.Update(request);
            }
            else
            {
                return GetError();
            }
        }
    }
}