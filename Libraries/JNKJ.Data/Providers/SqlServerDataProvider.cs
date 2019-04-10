using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.Hosting;
using JNKJ.Core.Data;
using JNKJ.Data.Initializers;
namespace JNKJ.Data.Providers
{
    public class SqlServerDataProvider : IDataProvider
    {
        #region Utilities
        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists)
        {
            if (!File.Exists(filePath))
            {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));
                else
                    return new string[0];
            }
            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream))
            {
                var statement = "";
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                {
                    statements.Add(statement);
                }
            }
            return statements.ToArray();
        }
        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();
            string lineOfText;
            while (true)
            {
                lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    else
                        return null;
                }
                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;
                sb.Append(lineOfText + Environment.NewLine);
            }
            return sb.ToString();
        }
        #endregion
        #region Methods
        /// <summary>
        /// 初始化数据库连接工厂
        /// </summary>
        public virtual void InitConnectionFactory()
        {
            var connectionFactory = new SqlConnectionFactory();
            //TODO修复编译警告 (如下)
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
        /// 设置一个数据库初始化
        /// </summary>
        public virtual void SetDatabaseInitializer()
        {
            //通过一些表名，来确保系统可以正确被安装
            //var tablesToValidate = new[] { "Customer", "Discount", "Order", "Product", "ShoppingCartItem" };
            var tablesToValidate = new[] { "Customer" };
            //用户命令
            var customCommands = new List<string>();
            //在单元测试中是不可用的webhelper.mappath 
            customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.Indexes.sql"), false));
            customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.sitedProcedures.sql"), false));
            var initializer = new CreateTablesIfNotExist<ObjectContextExt>(tablesToValidate, customCommands.ToArray());
            Database.SetInitializer(initializer);
        }
        /// <summary>
        /// 一个值，该值指示此数据提供程序是否支持定位过程。
        /// </summary>
        public virtual bool sitedProceduredSupported
        {
            get { return true; }
        }
        /// <summary>
        /// 获取支持数据库参数对象（由位置程序使用）
        /// </summary>
        /// <returns>Parameter</returns>
        public virtual DbParameter GetParameter()
        {
            return new SqlParameter();
        }
        #endregion
    }
}
