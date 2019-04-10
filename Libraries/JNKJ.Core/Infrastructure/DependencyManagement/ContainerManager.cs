using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;
namespace JNKJ.Core.Infrastructure.DependencyManagement
{
    public class ContainerManager
    {
        private readonly IContainer _container;
        public ContainerManager(IContainer container)
        {
            _container = container;
        }
        public IContainer Container
        {
            get { return _container; }
        }
        public T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<T>();
            }
            return scope.ResolveKeyed<T>(key);
        }
        public object Resolve(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.Resolve(type);
        }
        public T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<IEnumerable<T>>().ToArray();
            }
            return scope.ResolveKeyed<IEnumerable<T>>(key).ToArray();
        }
        public T ResolveUnregistered<T>(ILifetimeScope scope = null) where T : class
        {
            return ResolveUnregistered(typeof(T), scope) as T;
        }
        public object ResolveUnregistered(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = Resolve(parameter.ParameterType, scope);
                        if (service == null) throw new ExceptionExt("Unkown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (ExceptionExt)
                {
                }
            }
            throw new ExceptionExt("No contructor was found that had all the dependencies satisfied.");
        }
        public bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance)
        {
            if (scope == null)
            {
                //没有指定的范围 
                scope = Scope();
            }
            return scope.TryResolve(serviceType, out instance);
        }
        public bool IsRegistered(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //没有指定的范围 
                scope = Scope();
            }
            return scope.IsRegistered(serviceType);
        }
        public object ResolveOptional(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //没有指定的范围 
                scope = Scope();
            }
            return scope.ResolveOptional(serviceType);
        }
        public ILifetimeScope Scope()
        {
            try
            {
                if (HttpContext.Current != null)
                    return AutofacDependencyResolver.Current.RequestLifetimeScope;
                //当返回这样的生命周期范围时，应该确保它将被处理一次（例如在调度任务中)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
            catch (Exception exc)
            {
                //如果请求生命时间范围已经被处理，我们可以在这里得到一个异常。
                //例如, 在 "Application_EndRequest" 中处理
                //请注意,通常这不应该发生
                //当返回这样的生命周期范围时，应该确保它将被处理一次（例如在调度任务中)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
        }
    }
}
