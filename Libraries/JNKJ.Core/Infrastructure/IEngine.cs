using System;
using JNKJ.Core.Infrastructure.DependencyManagement;
namespace JNKJ.Core.Infrastructure
{
    /// <summary>
    /// 实现此接口的类可以用作
    /// 构成ErNet引擎的各种服务。
    ///编辑功能、模块和实现通过该接口访问大多数JNKJ功能
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// 管理器
        /// </summary>
        ContainerManager ContainerManager { get; }
        /// <summary>
        ///在JNKJ环境中初始化组件和插件
        /// </summary>
        /// <param name="config">Config</param>
        void Initialize(JNKJConfig config);
        /// <summary>
        /// 解析依赖
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;
        /// <summary>
        ///  解析依赖
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        object Resolve(Type type);
        /// <summary>
        /// 解析依赖
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();
    }
}
