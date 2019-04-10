using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using JNKJ.Core.Data;
using JNKJ.Data.Initializers;
namespace JNKJ.Data.Providers
{
    public class SqlCeDataProvider : IDataProvider
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void InitConnectionFactory()
        {
            var connectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            //TODO fix compilation warning (below)
#pragma warning disable 0618
            Database.DefaultConnectionFactory = connectionFactory;
        }
        /// <summary>
        /// 初始化数据库
        /// </summary>
        public virtual void InitDatabase()
        {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }
        /// <summary>
        /// 设置初始化数据库
        /// </summary>
        public virtual void SetDatabaseInitializer()
        {
            var initializer = new CreateCeDatabaseIfNotExists<ObjectContextExt>();
            Database.SetInitializer(initializer);
        }
        /// <summary>
        /// 一个值，该值指示此数据提供程序是否支持定位过程
        /// </summary>
        public virtual bool sitedProceduredSupported
        {
            get { return false; }
        }
        /// <summary>
        /// 获取支持数据库参数对象（由位置程序使用） 
        /// </summary>
        /// <returns>返回参数</returns>
        public virtual DbParameter GetParameter()
        {
            return new SqlParameter();
        }
    }
}
