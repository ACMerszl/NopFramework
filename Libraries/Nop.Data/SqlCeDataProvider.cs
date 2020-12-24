using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using Nop.Core.Data;
using Nop.Data.Initializers;

namespace Nop.Data
{
    /// <summary>
    /// sqlce数据库连接
    /// </summary>
    public class SqlCeDataProvider : IDataProvider
    {
        /// <summary>
        /// Initialize connection factory初始化连接工厂
        /// </summary>
        public virtual void InitConnectionFactory()
        {
            var connectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            //TODO fix compilation warning (below)TODO修复编译警告（如下）
            #pragma warning disable 0618
            Database.DefaultConnectionFactory = connectionFactory;
        }

        /// <summary>
        /// Initialize database初始化数据库
        /// </summary>
        public virtual void InitDatabase()
        {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }

        /// <summary>
        /// Set database initializer设置数据库初始化
        /// </summary>
        public virtual void SetDatabaseInitializer()
        {
            var initializer = new CreateCeDatabaseIfNotExists<NopObjectContext>();
            Database.SetInitializer(initializer);
        }

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// 指示此数据提供程序是否支持存储过程的值
        /// </summary>
        public virtual bool StoredProceduredSupported
        {
            get { return false; }
        }

        /// <summary>
        /// A value indicating whether this data provider supports backup
        /// 指示此数据提供程序是否支持备份的值
        /// </summary>
        public bool BackupSupported
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// 获取支持数据库参数对象(由存储过程使用)。
        /// </summary>
        /// <returns>Parameter</returns>
        public virtual DbParameter GetParameter()
        {
            return new SqlParameter();
        }

        /// <summary>
        /// Maximum length of the data for HASHBYTES functions
        /// returns 0 if HASHBYTES function is not supported
        /// HASHBYTES函数的最大数据长度
        ///如果不支持HASHBYTES函数，返回0
        /// </summary>
        /// <returns>Length of the data for HASHBYTES functions
        /// HASHBYTES函数的数据长度</returns>
        public int SupportedLengthOfBinaryHash()
        {
            return 0; //HASHBYTES functions is missing in SQL CESQL CE中缺少HASHBYTES函数
        }
    }
}
