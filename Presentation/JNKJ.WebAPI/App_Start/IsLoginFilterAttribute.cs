using JNKJ.Core.Data;
using JNKJ.Core.Infrastructure;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.UserCenter.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace JNKJ.WebAPI.App_Start
{
    public class IsLoginAuthorize : AuthorizeAttribute
    {
     

        //此方法返回true则执行HandleUnauthorizedRequest，否则不执行，直接执行方法
        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            var UserName = httpContext.Request.Headers.Where(c => c.Key.ToLower() == "username").FirstOrDefault().Value.First();
            var IsApp = Convert.ToBoolean(httpContext.Request.Headers.Where(c => c.Key.ToLower() == "isapp").FirstOrDefault().Value.First());
            var userinfoService = EngineContext.Current.Resolve<IUserInfo>();
            var Id = userinfoService.GetUserInfo(new UserInfoGetRequest
            {
                Phone = UserName
            }).Select(t=>t.Id).FirstOrDefault();

            var tokenService = EngineContext.Current.Resolve<ITokenService>();
            var tokenInfo = tokenService.GenerateToken(new GenerateTokenRequest
            {
                UserId = Id,
                IsApp = IsApp
            });

            var token = httpContext.Request.Headers.Where(c => c.Key.ToLower() == "token").FirstOrDefault().Value;

            if (token != null && token.Count() > 0 )
            {
               
                var token1 = token.First<string>();
                //验证token，成功就不执行返回未登录信息
                if (token1 != tokenInfo.Token)
                {
                    return false;
                }

            }
            return true;
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            var response = filterContext.Response = filterContext.Response ?? new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.Forbidden;
            var content = new
            {
                success = false,
                errs = new[] { "未登录或token已过期" }
            };
            response.Content = new StringContent(JsonConvert.SerializeObject(content));

        }
    }
}