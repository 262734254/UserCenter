using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using JNKJ.Domain;
namespace JNKJ.Data
{
    /// <summary>
    /// 对象上下文
    /// </summary>
    public class ObjectContextExt : DbContext, IDbContext
    {
        #region Ctor
        public ObjectContextExt(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            ((IObjectContextAdapter)this).ObjectContext.ContextOptions.LazyLoadingEnabled = false;
            //拦截sql 实现读写分离
            this.Database.CommandTimeout = 0;
            //SQL语句拦截器
            System.Data.Entity.Infrastructure.Interception.DbInterception.Add(new CommandInterceptor());
            //CommandInterceptor.SuppressNoLock = true;
        }
        #endregion
        #region Utilities
        /// <summary>
        /// 创建模型时
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //读取映射关系，并生成映射
            //var typesitegister = Assembly.GetExecutingAssembly().GetTypes()
            //.Where(type => !String.IsNullOrEmpty(type.Namespace))
            //.Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            //foreach (var type in typesitegister)
            //{
            //    dynamic configurationInstance = Activator.CreateInstance(type);
            //    modelBuilder.Configurations.Add(configurationInstance);
            //}
            //base.OnModelCreating(modelBuilder);
            var typesitegister = Assembly.Load("JNKJ.Mapping").GetTypes().Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            foreach (var type in typesitegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);
        }
        /// <summary>
        /// 附加一个实体的上下文或返回一个已连接的实体(如果已经连接)
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>返回附加的实体</returns>
        protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            //在实体框架真正支持站点过程之前，这里很少有漏洞
            //否则，在实体附加到上下文之前，不会加载加载实体的导航属性。
            var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
            if (alreadyAttached == null)
            {
                //附上新的实体 
                Set<TEntity>().Attach(entity);
                return entity;
            }
            else
            {
                //实体已经加载。
                return alreadyAttached;
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// 创建数据库的脚本
        /// </summary>
        /// <returns>SQL生成数据库</returns>
        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }
        /// <summary>
        ///得到DbSet 
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>返回值</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }
        /// <summary>
        /// 执行站点过程并在结束时加载实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="commandText">执行的命令</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回值</returns>
        public IList<TEntity> ExecutesitedProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            //add parameters to command
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");
                    commandText += i == 0 ? " " : ", ";
                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //output parameter
                        commandText += " output";
                    }
                }
            }
            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
            bool acd = this.Configuration.AutoDetectChangesEnabled;
            try
            {
                this.Configuration.AutoDetectChangesEnabled = false;
                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);
            }
            finally
            {
                this.Configuration.AutoDetectChangesEnabled = acd;
            }
            return result;
        }
        /// <summary>
        /// 创建一个原始SQL查询，该查询将返回给定泛型类型的元素。该类型可以是具有与查询返回的列的名称匹配的属性的任何类型，
        /// 也可以是简单的原始类型。类型不一定是实体类型。即使返回的对象类型是一个实体类型，这个查询的结果也不会被上下文跟踪。
        /// </summary>
        /// <typeparam name="TElement">由查询返回的对象类型 .</typeparam>
        /// <param name="sql">查询的SQL语句</param>
        /// <param name="parameters">应用于SQL查询字符串的参数 .</param>
        /// <returns>返回值</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            var ssss = this.Database.SqlQuery<TElement>(sql, parameters);
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }
        /// <summary>
        /// 执行给定的DDL DML命令对数据库
        /// </summary>
        /// <param name="sql">命令字符串 </param>
        /// <param name="doNotEnsureTransaction">错误-事务创建没有保证；真的-确保事务创建.</param>
        /// <param name="timeout">超时值，以秒为单位。NULL值表示将使用底层提供程序的默认值。</param>
        /// <param name="parameters">应用于命令字符串的参数 .</param>
        /// <returns>执行命令后数据库返回的结果。.</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //site previous timeout
                previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
            }
            var transactionalBehavior = doNotEnsureTransaction
                ? TransactionalBehavior.DoNotEnsureTransaction
                : TransactionalBehavior.EnsureTransaction;
            var result = this.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);
            if (timeout.HasValue)
            {
                //设置以前的超时时间
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
            }
            //return result
            return result;
        }
        #endregion
        public IList<TEntity> ExecutesitedProcedureList<TEntity>(string commandText, params System.Data.SqlClient.SqlParameter[] parameters) where TEntity : BaseEntity, new()
        {
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("不支持该参数类型");
                    commandText += i == 0 ? " " : ", ";
                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //输出参数
                        commandText += " output";
                    }
                }
            }
            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
            bool acd = this.Configuration.AutoDetectChangesEnabled;
            try
            {
                this.Configuration.AutoDetectChangesEnabled = false;
                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);
            }
            finally
            {
                this.Configuration.AutoDetectChangesEnabled = acd;
            }
            return result;
        }
    }
}