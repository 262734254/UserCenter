using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using Autofac.Integration.Mvc;
using JNKJ.Core;
using JNKJ.Core.Data;
using JNKJ.Core.Fakes;
using JNKJ.Core.Infrastructure;
using JNKJ.Core.Infrastructure.DependencyManagement;
using JNKJ.Cache.Caching;
using JNKJ.Domain;
using JNKJ.Data;
using JNKJ.Services.Authority.Interface;
using JNKJ.Services.Authority.Realize;
using JNKJ.Services.Qiniu;
using JNKJ.Services.UserCenter.Interface;
using JNKJ.Services.UserCenter.Realize;
using JNKJ.Services.TencentIM;
using JNKJ.Services.SMS;

namespace JNKJ.Web.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();
            //WEB Helpers
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());
            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());//注册api容器的实现
            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>();
            builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IEfDataProvider>().InstancePerDependency();
            builder.Register(x => x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();
            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new EfDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();
                builder.Register<IDbContext>(c => new ObjectContextExt(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            }
            else
            {
                builder.Register<IDbContext>(c => new ObjectContextExt(dataSettingsManager.LoadSettings().DataConnectionString)).InstancePerLifetimeScope();
            }
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            //cache manager
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("JNKJ_cache_static").SingleInstance();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("JNKJ_cache_per_request").InstancePerLifetimeScope();
            builder.RegisterSource(new SettingsSource());
            #region 将自定义接口注入容器
            builder.RegisterType<QiniuService>().As<IQiniuService>().InstancePerLifetimeScope();            //七牛服务
            builder.RegisterType<SubContractorService>().As<ISubContractor>().InstancePerLifetimeScope();            //企业信息--企业信息
            builder.RegisterType<UserInfoService>().As<IUserInfo>().InstancePerLifetimeScope();//用户接口
            builder.RegisterType<EnterpriseInfoService>().As<IEnterpriseInfoService>().InstancePerLifetimeScope();//企业接口
            builder.RegisterType<TokenService>().As<ITokenService>().InstancePerLifetimeScope();//Token验证
            builder.RegisterType<TencentImService>().As<ITencentImService>().InstancePerLifetimeScope();//腾讯IM云通信
            builder.RegisterType<UserLoginService>().As<IUserLogin>().InstancePerLifetimeScope();//登陆
            builder.RegisterType<DeptInfoService>().As<IDeptInfoService>().InstancePerLifetimeScope();//组织架构
            builder.RegisterType<MenuOrButtonService>().As<IMenuOrButtonService>().InstancePerLifetimeScope();//菜单和按钮
            builder.RegisterType<RolesService>().As<IRolesService>().InstancePerLifetimeScope();//角色权限
            builder.RegisterType<RelationshipService>().As<IRelationshipService>().InstancePerLifetimeScope();
            builder.RegisterType<SmsService>().As<ISmsService>().InstancePerLifetimeScope();//角色权限
            builder.RegisterType<RelationshipService>().As<IRelationshipService>().InstancePerLifetimeScope();
            
            #endregion
            //Register event consumers
            #region service register
            #endregion
        }
        public int Order
        {
            get { return 0; }
        }
    }
    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);
        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }
        public bool IsAdapterForIndividualComponents { get { return false; } }
    }
}
