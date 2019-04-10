using System.Web.Http;
using System.Web.Http.Cors;
using System.Configuration;
namespace JNKJ.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //跨域配置
            var allowedMethods = ConfigurationManager.AppSettings["allowedMethods"];
            var allowedOrigin = ConfigurationManager.AppSettings["allowedOrigin"];
            var allowedHeaders = ConfigurationManager.AppSettings["allowedHeaders"];
            var jnkjCors = new EnableCorsAttribute(allowedOrigin, allowedHeaders, allowedMethods) { SupportsCredentials = true };
            config.EnableCors(jnkjCors);
            //var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);


            //路由配置
            config.MapHttpAttributeRoutes();
            //Home/Index
            config.Routes.MapHttpRoute("DefaultApi", "{controller}/{action}", new { controller = "Home", action = "Index", id = RouteParameter.Optional });
            //api/UserCenter/SubContractor/get_subcontractors
            config.Routes.MapHttpRoute("DefaultApi2", "api/{area}/{controller}/{action}", new { controller = "Home", action = "Index", id = RouteParameter.Optional });

        }
    }
}
