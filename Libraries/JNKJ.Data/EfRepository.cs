using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using System.Linq;
using JNKJ.Domain;
using JNKJ.Core.Data;
namespace JNKJ.Data
{
    /// <summary>
    /// EF仓库类
    /// </summary>
    public partial class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">上下文环境</param>
        public EfRepository(IDbContext context)
        {
            this._context = context;
        }
        public virtual IEnumerable<T> GetList(string strSql, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            return _context.SqlQuery<T>(strSql, parameters);
        }
        /// <summary>
        /// 根据标识符获取实体
        /// </summary>
        /// <param name="id">标识符</param>
        /// <returns>实体对象</returns>
        public virtual T GetById(object id)
        {
            return this.Entities.Find(id);
        }
        public virtual T GetSingle(string strSql, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            return _context.SqlQuery<T>(strSql, parameters).FirstOrDefault();
        }
        public bool AddRange(List<T> list)
        {
            try
            {
                if (list == null)
                    throw new ArgumentNullException("list");
                foreach (T entity in list)
                {
                    this.Entities.Add(entity);
                }
                return this._context.SaveChanges() > 0 ? true : false;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        /// <summary>
        /// 插入一个实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual bool Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                this.Entities.Add(entity);
                return this._context.SaveChanges() > 0 ? true : false;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        /// <summary>
        /// 前置插入
        /// </summary>
        /// <param name="entity"></param>
        public void PreInsert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                this.Entities.Add(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual bool Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                ////如果上下文已经存在数据，先删除掉
                //RemoveHoldingEntityInContext(entity);
                //this.Entities.Attach(entity);
                //DbEntityEntry<T> entry = ((DbContext)this._context).Entry<T>(entity);
                //entry.State = System.Data.Entity.EntityState.Modified;
                return this._context.SaveChanges() > 0 ? true : false;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual bool SingleUpdate(T entity, string[] UpdateFields)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                //如果上下文已经存在数据，先删除掉
                RemoveHoldingEntityInContext(entity);
                DbEntityEntry<T> entry = ((DbContext)this._context).Entry<T>(entity);
                entry.State = System.Data.Entity.EntityState.Unchanged;
                foreach (var field in UpdateFields)
                {
                    entry.Property(field).IsModified = true;
                }
                return this._context.SaveChanges() > 0 ? true : false;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        public virtual bool SingleUpdate(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                //如果上下文已经存在数据，先删除掉
                RemoveHoldingEntityInContext(entity);
                this.Entities.Attach(entity);
                DbEntityEntry<T> entry = ((DbContext)this._context).Entry<T>(entity);
                entry.State = System.Data.Entity.EntityState.Modified;
                return this._context.SaveChanges() > 0 ? true : false;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual bool Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                this.Entities.Remove(entity);
                return this._context.SaveChanges() > 0 ? true : false;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        public virtual bool DeleteList(IList<T> list)
        {
            try
            {
                if (list == null || list.Count == 0)
                    throw new ArgumentNullException("entity");
                foreach (var m in list)
                {
                    this.Entities.Remove(m);
                }
                return this._context.SaveChanges() > 0 ? true : false;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        /// <summary>
        ///获取一个Table查询对象
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }
        /// <summary>
        /// 得到一个启用了“不跟踪”(EF特性)的表查询对象，针对只读操作记录(s)
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return this.Entities.AsNoTracking();
            }
        }
        public virtual bool SaveChanges()
        {
            try
            {
                return this._context.SaveChanges() > 0 ? true : false;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        public virtual DbContext Context
        {
            get { return (DbContext)_context; }
        }
        /// <summary>
        /// 实体
        /// </summary>
        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }
        /// <summary>
        /// 判断Context中的Entity是否存在，如果存在，将其Detach，防止出现问题。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Boolean RemoveHoldingEntityInContext(T entity)
        {
            var objContext = ((IObjectContextAdapter)this._context).ObjectContext;
            var objSet = objContext.CreateObjectSet<T>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);
            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);
            if (exists)
            {
                objContext.Detach(foundEntity);
            }
            return (exists);
        }
    }
}