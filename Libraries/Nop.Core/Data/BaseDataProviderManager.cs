using System;

namespace Nop.Core.Data
{
    /// <summary>
    /// Base data provider manager
    /// 基本数据提供程序管理器
    /// Provider管理类通过继承BaseDataProviderManager，并重写LoadDataProvider方法，返回不同数据库的Provider
    /// </summary>
    public abstract class BaseDataProviderManager
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="settings">Data settings-数据库配置</param>
        protected BaseDataProviderManager(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            this.Settings = settings;
        }

        /// <summary>
        /// Gets or sets settings
        /// </summary>
        protected DataSettings Settings { get; private set; }

        /// <summary>
        /// Load data provider
        /// 加载数据提供者
        /// </summary>
        /// <returns>Data provider</returns>
        public abstract IDataProvider LoadDataProvider();
    }
}
