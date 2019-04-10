using System.Data.Entity.Infrastructure;
using JNKJ.Core.Data;
namespace JNKJ.Data
{
    public interface IEfDataProvider : IDataProvider
    {
        /// <summary>
        /// Get connection factory
        /// </summary>
        /// <returns>Connection factory</returns>
        IDbConnectionFactory GetConnectionFactory();
        /// <summary>
        /// Initialize connection factory
        /// </summary>
        void InitConnectionFactory();
        /// <summary>
        /// Set database initializer
        /// </summary>
        void SetDatabaseInitializer();
    }
}
