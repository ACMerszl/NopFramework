using System;
using System.Data.Entity;
using System.Transactions;

namespace Nop.Data.Initializers
{

    /// <summary>
    /// An implementation of IDatabaseInitializer that will <b>DELETE</b>, recreate, and optionally re-seed the
    /// database only if the model has changed since the database was created.  This is achieved by writing a
    /// hash of the store model to the database when it is created and then comparing that hash with one
    /// generated from the current model.
    /// To seed the database, create a derived class and override the Seed method.
    /// IDatabaseInitializer的实现，它将<b>删除</b>，重新创建，并可选地重新种子
    ///只有当模型在创建数据库后发生了更改时才使用数据库。这是通过写a来实现的
    ///在创建存储模型时，将该存储模型散列到数据库，然后将该散列与一个散列进行比较
    ///从当前模型生成。
    ///创建一个派生类并覆盖该种子方法
    /// </summary>
    public class DropCreateCeDatabaseIfModelChanges<TContext> : SqlCeInitializer<TContext> where TContext : DbContext
    {
        #region Strategy implementation

        /// <summary>
        /// Executes the strategy to initialize the database for the given context.
        /// 执行策略以初始化给定上下文的数据库。
        /// </summary>
        /// <param name="context">The context.</param>
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
                if (context.Database.CompatibleWithModel(throwIfNoMetadata: true))
                {
                    return;
                }

                replacedContext.Database.Delete();
            }

            // Database didn't exist or we deleted it, so we now create it again.
            //数据库不存在或者我们删除了它，所以我们现在再次创建它。
            context.Database.Create();

            Seed(context);
            context.SaveChanges();
        }

        #endregion

        #region Seeding methods

        /// <summary>
        /// A that should be overridden to actually add data to the context for seeding. 
        /// The default implementation does nothing.
        /// 应该被重写以实际向播种上下文添加数据。
        ///默认实现不做任何事情。
        /// </summary>
        /// <param name="context">The context to seed.</param>
        protected virtual void Seed(TContext context)
        {
        }

        #endregion
    }

}
