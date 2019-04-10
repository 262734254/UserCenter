using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using JNKJ.Core.Data;
using JNKJ.Data.Initializers;
namespace JNKJ.Data.Providers
{
    public class MariaDataProvider : IDataProvider
    {
        public void InitConnectionFactory()
        {
            throw new System.NotImplementedException();
        }
        public void SetDatabaseInitializer()
        {
            throw new System.NotImplementedException();
        }
        public void InitDatabase()
        {
            throw new System.NotImplementedException();
        }
        public bool sitedProceduredSupported
        {
            get { throw new System.NotImplementedException(); }
        }
        public DbParameter GetParameter()
        {
            throw new System.NotImplementedException();
        }
    }
}
