using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using JNKJ.Domain;
namespace JNKJ.Core.Data
{
    /// <summary>
    /// EF仓库
    /// </summary>
    public partial interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        ///根据标识符获取实体对象
        /// </summary>
        /// <param name="id">标识符</param>
        /// <returns>实体对象</returns>
        T GetById(object id);
        T GetSingle(string strSql, params System.Data.SqlClient.SqlParameter[] parameters);
        /// <summary>
        /// 执行自定义SQL，获取LIST
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pararm"></param>
        /// <returns></returns>
        IEnumerable<T> GetList(string strSql, params System.Data.SqlClient.SqlParameter[] parameters);
        /// <summary>
        ///  插入一个实体
        /// </summary>
        /// <param name="entity">Entity</param>
        bool Insert(T entity);
        /// <summary>
        /// 前置插入
        /// </summary>
        /// <param name="entity"></param>
        void PreInsert(T entity);
        /// <summary>
        /// 更新一个实体
        /// </summary>
        /// <param name="entity">Entity</param>
        bool Update(T entity);
        bool SingleUpdate(T entity);
        /// <summary>
        /// 更新一个实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="UpdateFields"></param>
        /// <returns></returns>
        bool SingleUpdate(T entity, string[] UpdateFields);
        bool AddRange(List<T> list);
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity">Entity</param>
        bool Delete(T entity);
        /// <summary>
        /// 删除一个队列
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool DeleteList(IList<T> list);
        bool SaveChanges();
        /// <summary>
        ///获取一个查询对象
        /// </summary>
        IQueryable<T> Table { get; }
        DbContext Context { get; }
        /// <summary>
        /// 得到一个启用了“不跟踪”(EF特性)的表查询对象，针对只读操作记录(s)
        /// </summary>
        IQueryable<T> TableNoTracking { get; }
    }
}
