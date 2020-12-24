
using System.Data.Common;

namespace Nop.Core.Data
{
    /// <summary>
    /// Data provider interface
    /// 数据提供者接口（这个接口很重要，不同数据库实现该接口后，
    /// 可以自定义数据库初始化方式，连接工厂等，Oracle和Mysql才可以使用EF.）
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Initialize connection factory
        /// 初始化数据库连接
        /// </summary>
        void InitConnectionFactory();

        /// <summary>
        /// Set database initializer
        /// 数据库初始化配置,可以在该方法中为Database配置初始化方式
        /// </summary>
        void SetDatabaseInitializer();

        /// <summary>
        /// Initialize database
        /// 初始化数据库
        /// </summary>
        void InitDatabase();

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// 指示此数据提供程序是否支持存储过程的值
        /// </summary>
        bool StoredProceduredSupported { get; }

        /// <summary>
        /// A value indicating whether this data provider supports backup
        /// 指示此数据提供程序是否支持备份的值
        /// </summary>
        bool BackupSupported { get; }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// 获取支持数据库参数对象(由存储过程使用)。
        /// </summary>
        /// <returns>Parameter</returns>
        DbParameter GetParameter();

        /// <summary>
        /// Maximum length of the data for HASHBYTES functions
        /// returns 0 if HASHBYTES function is not supported
        /// 获取支持数据库参数对象(由存储过程使用)HASHBYTES函数的数据的最大长度
        ///如果HASHBYTES函数不支持，返回0
        /// </summary>
        /// <returns>Length of the data for HASHBYTES functions</returns>
        int SupportedLengthOfBinaryHash();
    }
}
