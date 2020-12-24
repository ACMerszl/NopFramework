using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Nop.Data.Initializers
{
    /// <summary>
    /// MySql数据库初始化
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class MySqlDatabaseInitialize<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        private readonly string[] _customCommands;

        public MySqlDatabaseInitialize(string[] customCommands)
        {
            this._customCommands = customCommands;
        }

        public void InitializeDatabase(TContext context)
        {
            //判断数据库是否存在.
            bool dbExist;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExist = context.Database.Exists();
            }

            if (dbExist)
            {
                int numberOfTables = 0;
                foreach (var t1 in context.Database.SqlQuery<int>("select count(1) from information_schema.`TABLES` where TABLE_SCHEMA='" + ((IObjectContextAdapter)context).ObjectContext.Connection.Database + "' "))
                    numberOfTables = t1;

                if (numberOfTables == 0)
                {
                    var dbCreatingScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                    context.Database.ExecuteSqlCommand(dbCreatingScript);

                    context.SaveChanges();

                    if (_customCommands != null && _customCommands.Length > 0)
                    {
                        foreach (var command in _customCommands)
                            context.Database.ExecuteSqlCommand(command);
                    }
                }
            }
            else
            {
                throw new ApplicationException("No database instance");
            }
        }
    }
}
