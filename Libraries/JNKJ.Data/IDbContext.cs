using System.Collections.Generic;
using System.Data.Entity;
using JNKJ.Domain;
namespace JNKJ.Data
{
    public interface IDbContext
    {
        /// <summary>
        ///获取一个DbSet
        /// </summary>
        /// <typeparam name="TEntity">实体的类型</typeparam>
        /// <returns>DbSet</returns>
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;
        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        /// <summary>
        /// 执行网站程序和加载实体的列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="commandText">程序名</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体列表对象</returns>
        IList<TEntity> ExecutesitedProcedureList<TEntity>(string commandText, params System.Data.SqlClient.SqlParameter[] parameters)
            where TEntity : BaseEntity, new();
        /// <summary>
        /// 创建一个原始SQL查询，并将返回给定泛型类型的元素。属性的类型可以是任何类型相匹配的查询返回的列的名称,或者可以是一个简单的原语类型。
        /// </summary>
        /// <typeparam name="TElement">查询返回的对象的类型。</typeparam>
        /// <param name="sql"> SQL查询字符串.</param>
        /// <param name="parameters">查询请求的参数列表</param>
        /// <returns>查询列表</returns>
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params System.Data.SqlClient.SqlParameter[] parameters);
        /// <summary>
        /// 对数据库执行给定的DDL和DML命令
        /// </summary>
        /// <param name="sql">SQL 命令字符串</param>
        /// <param name="doNotEnsureTransaction">false -并不能确保事务创造;true -创建事务.</param>
        /// <param name="timeout">超时时间，为空引用默认值</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>数据库执行命令后返回的结果。</returns>
        int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters);
    }
}
