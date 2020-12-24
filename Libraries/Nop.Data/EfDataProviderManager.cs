using System;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Data
{
    /// <summary>
    /// EF数据库配置
    /// </summary>
    public partial class EfDataProviderManager : BaseDataProviderManager
    {
        public EfDataProviderManager(DataSettings settings):base(settings)
        {
        }
        /// <summary>
        /// 加载数据库
        /// </summary>
        /// <returns></returns>
        public override IDataProvider LoadDataProvider()
        {

            var providerName = Settings.DataProvider;
            if (String.IsNullOrWhiteSpace(providerName))
                throw new NopException("Data Settings doesn't contain a providerName");

            switch (providerName.ToLowerInvariant())
            {
                //case "mysql":
                //    return new MySqlDataProvider();
                //case "oracle":
                //    return new OracleDataProvider();
                case "sqlserver":
                    return new SqlServerDataProvider();
                case "sqlce":
                    return new SqlCeDataProvider();
                default:
                    throw new NopException(string.Format("Not supported dataprovider name: {0}", providerName));
            }
        }

    }
}
