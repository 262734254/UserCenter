using JNKJ.Dto.Enums;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.UserCenter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{

    public class LoginController : BaseController
    {
        #region Fields
        private readonly IUserLogin _userLoginService;
        private readonly IUserInfo _userinfoService;
        #endregion

        #region Ctor
        public LoginController(IUserLogin userLoginService,IUserInfo userinfoService)
        {
            _userLoginService = userLoginService;
            _userinfoService = userinfoService;
        }
        #endregion


        /// <summary>
        /// 验证码登陆
        /// </summary>
        /// <param name="eugRequest"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("get_userLogin")]
        public HttpResponseMessage GetUserLogin([FromUri]LoginRequest loginRequest)
        {
            var result = _userLoginService.UserLogin(loginRequest);
          
            return toJson(result);
        }

        //密码登陆
        [HttpGet]
        [ActionName("get_userLoginByPasswork")]
        public HttpResponseMessage GetUserLoginByPasswork([FromUri]LoginRequest loginRequest)
        {
            var result = _userinfoService.GetUserLoginByPasswork(loginRequest);

            return toJson(result);
        }
    }
}
