using System;
using System.Collections.Generic;
using System.Reflection;
namespace JNKJ.Core.Infrastructure
{
    /// <summary>
    /// 实现程序引擎提供的各种服务的接口
    /// </summary>
    public interface ITypeFinder
    {
        IList<Assembly> GetAssemblies();
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
    }
}
