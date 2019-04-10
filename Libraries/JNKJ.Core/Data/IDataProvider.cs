using System.Data.Common;
namespace JNKJ.Core.Data
{
    /// <summary>
    /// 数据提供接口
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// 初始化连接工厂
        /// </summary>
        void InitConnectionFactory();
        /// <summary>
        /// 设置数据库初始化
        /// </summary>
        void SetDatabaseInitializer();
        /// <summary>
        ///初始化数据库
        /// </summary>
        void InitDatabase();
        /// <summary>
        /// 获取一个值，指示是否这个数据提供者支持选址程序
        /// </summary>
        bool sitedProceduredSupported { get; }
        /// <summary>
        /// 得到一个支持数据库参数对象(使用的选址过程)
        /// </summary>
        /// <returns>Parameter</returns>
        DbParameter GetParameter();
    }
}
