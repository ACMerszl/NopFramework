using System.Collections.Generic;
using System.Data.Entity; 
using Nop.Domain;

namespace Nop.Data
{
    /// <summary>
    /// 数据库上下文接口，实现该接口, 对EF原始DBContext进行封装
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// Get DbSet
        /// 获得DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Save changes保存更改
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// 执行存储过程,并返回列表
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text命令文本</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities实体列表</returns>
        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new();

        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// 执行SQL查询,返回TElement列表,TElement可以是任何类型,且不会对查询结果进行追踪。
        /// 创建一个原始SQL查询，返回给定泛型类型的元素。
        /// 该类型可以是具有与查询返回的列名称相匹配的属性的任何类型，也可以是简单的基元类型。
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.
        /// 查询返回的对象的类型。</typeparam>
        /// <param name="sql">The SQL query string.
        /// SQL查询字符串。</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.
        /// 应用于SQL查询字符串的参数。</param>
        /// <returns>Result结果</returns>
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// 对数据库执行给定的DDL/DML
        /// （数据库定义语言-》创建和操作表结构-----数据操纵语言-》"选择"、"插入"、"更新" 和 "删除" ）命令。
        /// </summary>
        /// <param name="sql">The command string命令字符串</param>
        /// <param name="doNotEnsureTransaction">false - the transaction creation is not ensured; true - the transaction creation is ensured.
        /// false—不确保事务的创建;true—确保事务创建。</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used
        /// 超时值，以秒为单位。空值表示将使用基础提供程序的默认值</param>
        /// <param name="parameters">The parameters to apply to the command string.
        /// 应用于命令字符串的参数。</param>
        /// <returns>The result returned by the database after executing the command.
        /// 数据库执行命令后返回的结果。</returns>
        int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters);

        /// <summary>
        /// Detach an entity
        /// 分离一个实体
        /// https://www.cnblogs.com/Jacky312/archive/2004/01/13/5446535.html
        /// </summary>
        /// <param name="entity">Entity</param>
        void Detach(object entity);

        /// <summary>
        /// Gets or sets a value indicating whether proxy creation setting is enabled (used in EF)
        /// 获取或设置一个值，该值指示是否启用代理创建设置(在EF中使用)
        /// </summary>
        bool ProxyCreationEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// 获取或设置一个值，该值指示是否启用自动检测更改设置(在EF中使用)。
        /// </summary>
        bool AutoDetectChangesEnabled { get; set; }
    }
}
