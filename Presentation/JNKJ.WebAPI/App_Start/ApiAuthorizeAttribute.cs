using JNKJ.Core.Data;
using JNKJ.Core.Infrastructure;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Enums;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.UserCenter.Interface;
using JWT;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace JNKJ.WebAPI.App_Start
{

    /// <summary>
    /// 身份认证拦截器
    /// </summary>
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public static readonly string secretKey = ConfigurationManager.AppSettings["TokenSecret"];//这个服务端加密秘钥 属于私钥\
        private string errMsg = "当前令牌已过期，请重新登陆";

        /// <summary>
        /// 指示指定的控件是否已获得授权
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool result = false;
            //前端请求api时会将token存放在名为"auth"的请求头中
            var authHeader = from t in actionContext.Request.Headers where t.Key == "auth" select t.Value.FirstOrDefault();
            if (authHeader != null)
            {
                string token = authHeader.FirstOrDefault();//获取token
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        AuthInfo json = new AuthInfo();
                        var tokenService = EngineContext.Current.Resolve<ITokenService>();
                        //TOKEN解密
                        result = tokenService.TokenDecryption(token, out errMsg, out json);
                        if (result)
                        {
                            actionContext.RequestContext.RouteData.Values.Add("auth", json);
                        }
                        return result;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    errMsg = "头部文件中未传入用户token";
                }
            }
            else
            {
                errMsg = "头部文件中未传入用户token";
            }
            return result;
        }

        /// <summary>
        /// 处理授权失败的请求
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var erModel = new
            {
                Success = "false",
                ErrorCode = errMsg
            };
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, erModel, "application/json");
        }
    }
}