using System;
using JNKJ.Core;
using JNKJ.Core.Data;
using JNKJ.Data.Providers;
namespace JNKJ.Data
{
    public partial class EfDataProviderManager : BaseDataProviderManager
    {
        public EfDataProviderManager(DataSettings settings)
            : base(settings)
        {
        }
        public override IDataProvider LoadDataProvider()
        {
            var providerName = Settings.DataProvider;
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ExceptionExt("Data Settings doesn't contain a providerName");
            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                case "sqlce":
                    return new SqlCeDataProvider();
                case "mariadb":
                    return new MariaDataProvider();
                default:
                    throw new ExceptionExt(string.Format("Not supported dataprovider name: {0}", providerName));
            }
        }
    }
}
