using System;
using System.Data.Entity;
using System.Transactions;

namespace Nop.Data.Initializers
{
    /// <summary>
    /// An implementation of IDatabaseInitializer that will recreate and optionally re-seed the
    /// database only if the database does not exist.
    /// To seed the database, create a derived class and override the Seed method.
    /// IDatabaseInitializer的实现，它将重新创建并可选地重新种子
    ///仅当数据库不存在时才使用。
    ///要播种数据库，请创建一个派生类并覆盖Seed方法。
    /// </summary>
    /// <typeparam name="TContext">The type of the context.上下文的类型。</typeparam>
    public class CreateCeDatabaseIfNotExists<TContext> : SqlCeInitializer<TContext> where TContext : DbContext
    {
        #region Strategy implementation策略实施

        public override void InitializeDatabase(TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var replacedContext = ReplaceSqlCeConnection(context);

            bool databaseExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                databaseExists = replacedContext.Database.Exists();
            }

            if (databaseExists)
            {
                // If there is no metadata either in the model or in the databaase, then
                // we assume that the database matches the model because the common cases for
                // these scenarios are database/model first and/or an existing database.
                if (!context.Database.CompatibleWithModel(throwIfNoMetadata: false))
                {
                    throw new InvalidOperationException(string.Format("The model backing the '{0}' context has changed since the database was created. Either manually delete/update the database, or call Database.SetInitializer with an IDatabaseInitializer instance. For example, the DropCreateDatabaseIfModelChanges strategy will automatically delete and recreate the database, and optionally seed it with new data.", context.GetType().Name));
                }
            }
            else
            {
                context.Database.Create();
                Seed(context);
                context.SaveChanges();
            }
        }

        #endregion

        #region Seeding methods

        /// <summary>
        /// A that should be overridden to actually add data to the context for seeding. 
        /// The default implementation does nothing.
        /// </summary>
        /// <param name="context">The context to seed.</param>
        protected virtual void Seed(TContext context)
        {
        }

        #endregion
    }


}
