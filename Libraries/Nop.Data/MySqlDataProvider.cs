using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using Nop.Core;
using Nop.Core.Data;
using Nop.Data.Initializers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data
{
    public class MySqlDataProvider : IDataProvider
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
                while ((statement = readNextStatementFromStream(reader)) != null)
                {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }

        protected virtual string readNextStatementFromStream(StreamReader reader)
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

                //MySql doesn't support GO, so just use a commented out GO as the separator
                if (lineOfText.TrimEnd().ToUpper() == "-- GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitDatabase()
        {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }

        /// <summary>
        /// Initialize connection factory
        /// </summary>
        public virtual void InitConnectionFactory()
        {
            var connectionFactory = new MySqlConnectionFactory();
            #pragma warning disable 0618
            Database.DefaultConnectionFactory = connectionFactory;
        }
        public virtual void SetDatabaseInitializer()
        {
            //pass some table names to ensure that we have nopCommerce 2.X installed
            var tablesToValidate = new[] { "Customer", "Discount", "Order", "Product", "ShoppingCartItem" };

            //custom commands (stored proedures, indexes)

            var customCommands = new List<string>();
            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            customCommands.AddRange(ParseCommands(CommonHelper.MapPath("~/App_Data/Install/MySql.Indexes.sql"), false));
            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            customCommands.AddRange(ParseCommands(CommonHelper.MapPath("~/App_Data/Install/MySql.StoredProcedures.sql"), false));

            var initializer = new CreateTablesIfNotExist<NopObjectContext>(tablesToValidate, customCommands.ToArray());
            Database.SetInitializer(initializer);
        }

        #endregion
        public bool StoredProceduredSupported
        {
            get
            {
                return true;
            }
        }

        public bool BackupSupported => false; // throw new NotImplementedException();

        //public int SupportedLengthOfBinaryHash => 2047; // throw new NotImplementedException();

        /// <summary>
        /// 返回参数
        /// </summary>
        /// <returns></returns>
        public DbParameter GetParameter()
        {
            return new MySqlParameter();
        }

        //public void SetDatabaseInitializer()
        //{
        //    var initializer = new MySqlDatabaseInitialize<NopObjectContext>(null);
        //    Database.SetInitializer(initializer);
        //}

        public int SupportedLengthOfBinaryHash()
        {
            return 2047; 
        }
    }
}
