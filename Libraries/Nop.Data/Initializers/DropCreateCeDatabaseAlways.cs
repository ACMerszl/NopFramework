using System;
using System.Data.Entity;

namespace Nop.Data.Initializers
{
    /// <summary>
    /// An implementation of IDatabaseInitializer that will always recreate and optionally re-seed the
    /// database the first time that a context is used in the app domain.
    /// To seed the database, create a derived class and override the Seed method.
    /// IDatabaseInitializer的实现，它将始终重新创建并可选地重新生成种子
    ///数据库第一次在应用程序域中使用上下文。
    ///创建一个派生类并覆盖该种子方法
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public class DropCreateCeDatabaseAlways<TContext> : SqlCeInitializer<TContext> where TContext : DbContext
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

            if (replacedContext.Database.Exists())
            {
                replacedContext.Database.Delete();
            }
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
