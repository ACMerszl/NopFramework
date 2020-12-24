using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Nop.Core;
using Nop.Core.Data;
using Nop.Data.Initializers;

namespace Nop.Data
{
    /// <summary>
    /// sqlServer数据提供者（sqlServer数据库）
    /// </summary>
    public class SqlServerDataProvider : IDataProvider
    {
        #region Utilities

        /// <summary>
        /// 解析命令，从文件中读取得到string数组
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="throwExceptionIfNonExists"></param>
        /// <returns></returns>
        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists)
        {
            //如果文件不存在
            if (!File.Exists(filePath))
            {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));
                
                return new string[0];
            }


            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream))
            {
                string statement;
                //从流中读取下一条语句
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }
        /// <summary>
        /// 从流中读取下一条语句
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();

            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    
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
        /// Initialize connection factory初始化连接工厂
        /// </summary>
        public virtual void InitConnectionFactory()
        {
            var connectionFactory = new SqlConnectionFactory();
            //TODO fix compilation warning (below)TODO修复编译警告(下面)
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
            //如果需要创建表可修改这里

            //pass some table names to ensure that we have nopCommerce 2.X installed
            //传递一些表名以确保我们有nopCommerce 2.X安装
            var tablesToValidate = new[] { "User"  };

            //custom commands (stored procedures, indexes)
            //自定义命令(存储过程、索引)
            var customCommands = new List<string>();
            customCommands.AddRange(ParseCommands(CommonHelper.MapPath("~/App_Data/Install/SqlServer.Indexes.sql"), false));
            customCommands.AddRange(ParseCommands(CommonHelper.MapPath("~/App_Data/Install/SqlServer.StoredProcedures.sql"), false));

            var initializer = new CreateTablesIfNotExist<NopObjectContext>(tablesToValidate, customCommands.ToArray());
            Database.SetInitializer(initializer);
        }

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// 指示此数据提供程序是否支持存储过程的值
        /// </summary>
        public virtual bool StoredProceduredSupported
        {
            get { return true; }
        }

        /// <summary>
        /// A value indicating whether this data provider supports backup
        /// 指示此数据提供程序是否支持备份的值
        /// </summary>
        public virtual bool BackupSupported
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// 获取支持数据库参数对象(由存储过程使用)。
        /// </summary>
        /// <returns>Parameter参数</returns>
        public virtual DbParameter GetParameter()
        {
            return new SqlParameter();
        }

        /// <summary>
        /// Maximum length of the data for HASHBYTES functions
        /// returns 0 if HASHBYTES function is not supported
        /// HASHBYTES函数的最大数据长度
        ///如果HASHBYTES函数不支持，返回0
        /// </summary>
        /// <returns>Length of the data for HASHBYTES functionsHASHBYTES函数的数据长度</returns>
        public int SupportedLengthOfBinaryHash()
        {
            return 8000; //for SQL Server 2008 and above HASHBYTES function has a limit of 8000 characters.
            //对于SQL Server 2008及以上版本，HASHBYTES函数的限制为8000个字符。
        }

        #endregion
    }
}
