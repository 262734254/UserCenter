using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Timers;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure.Interception;
using JNKJ.Core.Data;
namespace JNKJ.Data
{
    public class CommandInterceptor : DbCommandInterceptor
    {
        static CommandInterceptor()
        {
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            //获取读的列表
            if (dataProviderSettings.RawDataSettings != null && dataProviderSettings.RawDataSettings.Count > 0)
            {
                readConnList = dataProviderSettings.RawDataSettings;
            }
            sysTimer.Enabled = true;
            sysTimer.Elapsed += sysTimer_Elapsed;
            sysTimer.Start();
        }
        /// <summary>
        /// 是否在一个事务中，如果是select,insert,update,delete都走主库
        /// ThreadStatic标识它只在当前线程有效
        /// </summary>
        [ThreadStatic]
        public static bool IsTransactionScope = false;
        /// <summary>
        /// 锁住它
        /// </summary>
        private static object lockObj = new object();
        /// <summary>
        /// 定期找没有在线的数据库服务器
        /// </summary>
        private static Timer sysTimer = new Timer(5000);
        /// <summary>
        ///　读库，从库集群，写库不用设置走默认的EF框架
        /// </summary>
        private static IDictionary<string, string> readConnList;
        #region Private Methods
        private static void sysTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (readConnList != null && readConnList.Any())
            {
                foreach (var item in readConnList)
                {
                    //心跳测试，将死掉的服务器IP从列表中移除
                    var client = new TcpClient();
                    try
                    {
                        client.Connect(new IPEndPoint(IPAddress.Parse(item.Key), int.Parse(item.Value)));
                    }
                    catch (SocketException)
                    {
                        //异常，没有连接上
                        readConnList.Remove(item);
                    }
                    if (!client.Connected)
                    {
                        readConnList.Remove(item);
                    }
                }
            }
        }
        /// <summary>
        /// 处理读库字符串
        /// </summary>
        /// <returns></returns>
        private string GetReadConn()
        {
            if (readConnList != null && readConnList.Any())
            {
                var t = readConnList.Values.ToArray();
                var resultConn = t[Convert.ToInt32(Math.Floor((double)new Random().Next(0, readConnList.Count)))];
                return resultConn;
            }
            return string.Empty;
        }
        /// <summary>
        /// 只读库的选择,加工command对象
        /// 说明：事务中,所有语句都走主库，事务外select走读库,insert,update,delete走主库
        /// 希望：一个ＷＥＢ请求中，读与写的仓储使用一个，不需要在程序中去重新定义
        /// </summary>
        /// <param name="command"></param>
        private void ReadDbSelect(DbCommand command)
        {
            if (!string.IsNullOrWhiteSpace(GetReadConn()))//如果配置了读写分离，就去实现
            {
                command.Connection.Close();
                if (!command.CommandText.StartsWith("insert", StringComparison.InvariantCultureIgnoreCase) && !IsTransactionScope)
                    command.Connection.ConnectionString = GetReadConn();
                command.Connection.Open();
            }
        }
        #endregion
        #region Override Methods
        /// <summary>
        /// Linq to Entity生成的update,delete
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            base.NonQueryExecuting(command, interceptionContext);//update,delete等写操作直接走主库
        }
        /// <summary>
        /// 执行sql语句，并返回第一行第一列，没有找到返回null,如果数据库中值为null，则返回 DBNull.Value
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            ReadDbSelect(command);
            base.ScalarExecuting(command, interceptionContext);
        }
        /// <summary>
        /// Linq to Entity生成的select,insert
        /// 发送到sqlserver之前触发
        /// warning:在select语句中DbCommand.Transaction为null，而ef会为每个insert添加一个DbCommand.Transaction进行包裹
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            ReadDbSelect(command);
            base.ReaderExecuted(command, interceptionContext);
        }
        /// <summary>
        /// 发送到sqlserver之后触发
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            base.ReaderExecuted(command, interceptionContext);
        }
        #endregion
    }
}
