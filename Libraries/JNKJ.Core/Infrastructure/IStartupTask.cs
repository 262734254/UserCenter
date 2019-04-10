namespace JNKJ.Core.Infrastructure
{
    /// <summary>
    /// 启动时运行的应用的接口
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        void Execute();
        /// <summary>
        /// 订单号（顺序号）
        /// </summary>
        int Order { get; }
    }
}
