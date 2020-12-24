using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection; 
using Nop.Data.Mapping;
using Nop.Domain;
namespace Nop.Data
{
    /// <summary>
    /// Object context
    /// 对象上下文
    /// </summary>
    public class NopObjectContext : DbContext, IDbContext
    {
        #region CtorCtor构造函数

        public NopObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        }
        
        #endregion

        #region Utilities

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //这句是重点，EF会默认使用"dbo"对象操作数据库，造成Mysql和Oracle报错
            //https://blog.csdn.net/u010293395/article/details/89164985
            modelBuilder.HasDefaultSchema(null);

            //dynamically load all configuration
            //动态加载所有配置
            //System.Type configType = typeof(LanguageMap);   //any of your configuration classes here
            //var typesToRegister = Assembly.GetAssembly(configType).GetTypes()

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(NopEntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                //将模型实体配置添加到DbModelBuilder中
                modelBuilder.Configurations.Add(configurationInstance);
            }
            //...or do it manually below. For example,或者在下面手动操作。例如,
            //modelBuilder.Configurations.Add(new LanguageMap());



            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Attach an entity to the context or return an already attached entity (if it was already attached)
        /// 将一个实体附加到上下文，或者返回一个已经附加的实体(如果已经附加了)
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Attached entity</returns>
        protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            //little hack here until Entity Framework really supports stored procedures
            //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            //除非实体框架真正支持存储过程，否则，加载实体的导航属性直到实体附加到上下文时才会加载
            var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
            if (alreadyAttached == null)
            {
                //attach new entity附上新实体
                Set<TEntity>().Attach(entity);
                return entity;
            }

            //entity is already loaded实体已经加载
            return alreadyAttached;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create database script创建数据库脚本
        /// </summary>
        /// <returns>SQL to generate database，SQL生成数据库</returns>
        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// Execute（执行）存储过程并在最后加载实体列表
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text命令文本</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities实体集</returns>
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            //add parameters to command向命令添加参数
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    commandText += i == 0 ? " " : ", ";

                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //output parameter输出参数
                        commandText += " output";
                    }
                }
            }

            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();

            //performance hack applied as described here性能hack应用在这里描述 - http://www.nopcommerce.com/boards/t/25483/fix-very-important-speed-improvement.aspx
            bool acd = this.Configuration.AutoDetectChangesEnabled;
            try
            {
                this.Configuration.AutoDetectChangesEnabled = false;

                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);
            }
            finally
            {
                this.Configuration.AutoDetectChangesEnabled = acd;
            }

            return result;
        }

        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// 创建一个原始SQL查询，返回给定泛型类型的元素。
        /// 该类型可以是具有与查询返回的列名称相匹配的属性的任何类型，
        /// 也可以是简单的基元类型。该类型不必是实体类型。即使返回的对象类型是实体类型，
        /// 该查询的结果也不会由上下文跟踪。
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.查询返回的对象的类型。</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.应用于SQL查询字符串的参数。</param>
        /// <returns>Result</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// 对数据库执行给定的DDL/DML命令。
        /// DDL数据库定义语言：创建和操作表结构
        /// DML数据操纵语言："选择"、"插入"、"更新" 和 "删除" 
        /// </summary>
        /// <param name="sql">The command string命令字符串</param>
        /// <param name="doNotEnsureTransaction">false - the transaction creation is not ensured; true - the transaction creation is ensured.
        /// false—不确保事务的创建;true—确保事务创建。</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used
        /// 超时值，以秒为单位。空值表示将使用基础提供程序的默认值</param>
        /// <param name="parameters">The parameters to apply to the command string.
        /// 应用于命令字符串的参数</param>
        /// <returns>The result returned by the database after executing the command.
        /// 数据库执行命令后返回的结果。</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                //储存之前超时
                previousTimeout = ((IObjectContextAdapter) this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = timeout;
            }

            var transactionalBehavior = doNotEnsureTransaction
                ? TransactionalBehavior.DoNotEnsureTransaction
                : TransactionalBehavior.EnsureTransaction;
            var result = this.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                //设置先前的超时
                ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = previousTimeout;
            }

            //return result
            return result;
        }

        /// <summary>
        /// Detach an entity分离一个实体
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether proxy creation setting is enabled (used in EF)
        /// 获取或设置一个值，该值指示是否启用代理创建设置(在EF中使用)
        /// </summary>
        public virtual bool ProxyCreationEnabled
        {
            get
            {
                return this.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Configuration.ProxyCreationEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// 获取或设置一个值，该值指示是否启用自动检测更改设置(在EF中使用)。
        /// </summary>
        public virtual bool AutoDetectChangesEnabled
        {
            get
            {
                return this.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                this.Configuration.AutoDetectChangesEnabled = value;
            }
        }

        #endregion
    }
}