using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using JNKJ.Core;
using JNKJ.Core.Data;
using JNKJ.Core.Infrastructure;
using JNKJ.Web.Framework.Mvc;
using System.Web.Security;
using System.Security.Principal;
namespace JNKJ.WebAPI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //初始化引擎上下文
            EngineContext.Initialize(false);
            //实体绑定生成
            ModelBinders.Binders.Add(typeof(BaseJNKJModel), new JNKJModelBinder());
            //数据库生成和安装
            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();
            //添加一些功能默认实体元之上
            ModelMetadataProviders.Current = new JNKJMetadataProvider();
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RegisterRoutes(RouteTable.Routes);
            //数据验证
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ////开始安排任务
            //if (databaseInstalled)
            //{
            //    TaskManager.Instance.Initialize();
            //    TaskManager.Instance.Start();
            //}
            GlobalConfiguration.Configuration.EnsureInitialized();
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            EnsureDatabaseIsInstalled();
        }
        protected void EnsureDatabaseIsInstalled()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string installUrl = string.Format("{0}install", webHelper.GetSiteLocation());
            if (!webHelper.IsStaticResource(this.Request) &&
                !DataSettingsHelper.DatabaseIsInstalled() &&
                !webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Response.Redirect(installUrl);
            }
        }
        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            //错误日志
            LogException(exception);
            //404错误
            var httpException = exception as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404)
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                if (!webHelper.IsStaticResource(this.Request))
                {
                    Response.Clear();
                    Server.ClearError();
                    Response.TrySkipIisCustomErrors = true;
                }
            }
        }
        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;
            //忽略HTTP 404错误
            var httpException = exc as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404)
                return;
            try
            {
                //日志
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
       
    }
}
