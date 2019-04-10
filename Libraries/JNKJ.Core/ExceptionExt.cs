using System;
using System.Runtime.Serialization;
namespace JNKJ.Core
{
    /// <summary>
    /// 程序运行期间发生的错误
    /// </summary>
    [Serializable]
    public class ExceptionExt : Exception
    {
        /// <summary>
        /// 初始化实例
        /// </summary>
        public ExceptionExt()
        {
        }
        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="message">错误消息描述</param>
        public ExceptionExt(string message)
            : base(message)
        {
        }
        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="messageFormat">错误消息格式</param>
        /// <param name="args">异常消息参数.</param>
        public ExceptionExt(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }
        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="info">序列化信息.</param>
        /// <param name="context">上下文.</param>
        protected ExceptionExt(SerializationInfo
            info, StreamingContext context)
            : base(info, context)
        {
        }
        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="message">错误消息描述.</param>
        /// <param name="innerException">当前异常对象.</param>
        public ExceptionExt(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
