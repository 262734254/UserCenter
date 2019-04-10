using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;
namespace JNKJ.Data.Initializers
{
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        private readonly string[] _tablesToValidate;
        private readonly string[] _customCommands;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tablesToValidate">验证列表中表名是否存在，0时不验证
        /// <param name="customCommands">执行的用户命令的列表</param>
        public CreateTablesIfNotExist(string[] tablesToValidate, string[] customCommands)
            : base()
        {
            this._tablesToValidate = tablesToValidate;
            this._customCommands = customCommands;
        }
        public void InitializeDatabase(TContext context)
        {
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }
            if (dbExists)
            {
                bool createTables = false;
                if (_tablesToValidate != null && _tablesToValidate.Length > 0)
                {
                    //验证表名
                    var existingTableNames = new List<string>(context.Database.SqlQuery<string>("SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'"));
                    createTables = !existingTableNames.Intersect(_tablesToValidate, StringComparer.InvariantCultureIgnoreCase).Any();
                }
                else
                {
                    //检查表是否已经创建
                    int numberOfTables = 0;
                    foreach (var t1 in context.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' "))
                        numberOfTables = t1;
                    createTables = numberOfTables == 0;
                }
                if (createTables)
                {
                    //创建所有的表
                    var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                    context.Database.ExecuteSqlCommand(dbCreationScript);
                    //种子(context);
                    context.SaveChanges();
                    if (_customCommands != null && _customCommands.Length > 0)
                    {
                        foreach (var command in _customCommands)
                            context.Database.ExecuteSqlCommand(command);
                    }
                }
            }
            else
            {
                throw new ApplicationException("No database instance");
            }
        }
    }
}
