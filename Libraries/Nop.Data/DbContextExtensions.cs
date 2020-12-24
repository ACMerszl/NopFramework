using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Nop.Domain;

namespace Nop.Data
{
    /// <summary>
    /// 数据库上下文扩展
    /// </summary>
    public static class DbContextExtensions
    {
        #region Utilities本类自实现方法

        private static T InnerGetCopy<T>(IDbContext context, T currentCopy, Func<DbEntityEntry<T>, DbPropertyValues> func) where T : BaseEntity
        {
            //Get the database context
            //获取数据库上下文
            DbContext dbContext = CastOrThrow(context);

            //Get the entity tracking object
            //获取实体跟踪对象
            DbEntityEntry<T> entry = GetEntityOrReturnNull(currentCopy, dbContext);

            //The output 
            //输出
            T output = null;

            //Try and get the values
            //尝试得到这些值
            if (entry != null)
            {
                DbPropertyValues dbPropertyValues = func(entry);
                if (dbPropertyValues != null)
                {
                    output = dbPropertyValues.ToObject() as T;
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the entity or return null.
        /// 获取实体或返回null。
        /// </summary>
        /// <typeparam name="T">继承于基本类型</typeparam>
        /// <param name="currentCopy">The current copy.当前的副本。</param>
        /// <param name="dbContext">The db context.db上下文。</param>
        /// <returns></returns>
        private static DbEntityEntry<T> GetEntityOrReturnNull<T>(T currentCopy, DbContext dbContext) where T : BaseEntity
        {
            return dbContext.ChangeTracker.Entries<T>().FirstOrDefault(e => e.Entity == currentCopy);
        }
        /// <summary>
        /// 得到结果或者抛出异常
        /// </summary>
        /// <param name="context">数据库上下文</param>
        /// <returns></returns>
        private static DbContext CastOrThrow(IDbContext context)
        {
            var output = context as DbContext;

            if (output == null)
            {
                throw new InvalidOperationException("Context does not support operation.");
            }

            return output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the original copy.
        /// 加载原始副本
        /// </summary>
        /// <typeparam name="T">继承继承实体的类</typeparam>
        /// <param name="context">The context.上下文</param>
        /// <param name="currentCopy">The current copy.当前的副本。</param>
        /// <returns></returns>
        public static T LoadOriginalCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity
        {
            return InnerGetCopy(context, currentCopy, e => e.OriginalValues);
        }

        /// <summary>
        /// Loads the database copy.
        /// 加载数据库副本。
        /// </summary>
        /// <typeparam name="T">继承继承实体的类</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="currentCopy">The current copy.</param>
        /// <returns></returns>
        public static T LoadDatabaseCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity
        {
            return InnerGetCopy(context, currentCopy, e => e.GetDatabaseValues());
        }

        /// <summary>
        /// Drop a plugin table
        /// 删除插件表
        /// </summary>
        /// <param name="context">Context，数据库上下文</param>
        /// <param name="tableName">Table name，表名</param>
        public static void DropPluginTable(this DbContext context, string tableName)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");

            //drop the table
            //删除表
            if (context.Database.SqlQuery<int>("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}", tableName).Any<int>())
            {
                var dbScript = "DROP TABLE [" + tableName + "]";
                context.Database.ExecuteSqlCommand(dbScript);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Get table name of entity
        /// 获取实体的表名
        /// </summary>
        /// <typeparam name="T">Entity type，实体类型</typeparam>
        /// <param name="context">Context，数据库上下文</param>
        /// <returns>Table name表名</returns>
        public static string GetTableName<T>(this IDbContext context) where T : BaseEntity
        {
            //var tableName = typeof(T).Name;
            //return tableName;

            //this code works only with Entity Framework.
            //If you want to support other database, then use the code above (commented)
            //此代码仅适用于EF实体框架。如果您想支持其他数据库，那么使用上面的代码(注释)
            var adapter = ((IObjectContextAdapter)context).ObjectContext;
            var storageModel = (StoreItemCollection)adapter.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
            var containers = storageModel.GetItems<EntityContainer>();
            var entitySetBase = containers.SelectMany(c => c.BaseEntitySets.Where(bes => bes.Name == typeof(T).Name)).First();

            // Here are variables that will hold table and schema name
            //下面是保存表和模式名的变量
            string tableName = entitySetBase.MetadataProperties.First(p => p.Name == "Table").Value.ToString();
            //string schemaName = productEntitySetBase.MetadataProperties.First(p => p.Name == "Schema").Value.ToString();
            return tableName;
        }

        /// <summary>
        /// Get column maximum length
        /// 获取列的最大长度
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="entityTypeName">Entity type name，实体类型名称</param>
        /// <param name="columnName">Column name列名</param>
        /// <returns>Maximum length. Null if such rule does not exist
        /// 最大长度。如果该规则不存在，则为空</returns>
        public static int? GetColumnMaxLength(this IDbContext context, string entityTypeName, string columnName)
        {
            var rez = GetColumnsMaxLength(context, entityTypeName, columnName);
            return rez.ContainsKey(columnName) ? rez[columnName] as int? : null;
        }

        /// <summary>
        /// Get columns maximum length获取列的最大长度
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="entityTypeName">Entity type name实体类型名称</param>
        /// <param name="columnNames">Column names，列名</param>
        /// <returns></returns>
        public static IDictionary<string, int> GetColumnsMaxLength(this IDbContext context, string entityTypeName, params string[] columnNames)
        {
            int temp;

            var fildFacets = GetFildFacets(context, entityTypeName, "String", columnNames);

            var queryResult = fildFacets
                .Select(f => new { Name = f.Key, MaxLength = f.Value["MaxLength"].Value })
                .Where(p => int.TryParse(p.MaxLength.ToString(), out temp))
                .ToDictionary(p => p.Name, p => Convert.ToInt32(p.MaxLength));

            return queryResult;
        }


        /// <summary>
        /// Get maximum decimal values获取最大十进制值
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="entityTypeName">Entity type name</param>
        /// <param name="columnNames">Column names</param>
        /// <returns></returns>
        public static IDictionary<string, decimal> GetDecimalMaxValue(this IDbContext context, string entityTypeName, params string[] columnNames)
        {
            var fildFacets = GetFildFacets(context, entityTypeName, "Decimal", columnNames);

            return fildFacets.ToDictionary(p => p.Key, p => int.Parse(p.Value["Precision"].Value.ToString()) - int.Parse(p.Value["Scale"].Value.ToString()))
                .ToDictionary(p => p.Key, p => new decimal(Math.Pow(10, p.Value)));
        }

        private static Dictionary<string, ReadOnlyMetadataCollection<Facet>> GetFildFacets(this IDbContext context,
            string entityTypeName, string edmTypeName, params string[] columnNames)
        {
            //original: http://stackoverflow.com/questions/5081109/entity-framework-4-0-automatically-truncate-trim-string-before-insert

            var entType = Type.GetType(entityTypeName);
            var adapter = ((IObjectContextAdapter)context).ObjectContext;
            var metadataWorkspace = adapter.MetadataWorkspace;
            var q = from meta in metadataWorkspace.GetItems(DataSpace.CSpace).Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                    from p in (meta as EntityType).Properties.Where(p => columnNames.Contains(p.Name) && p.TypeUsage.EdmType.Name == edmTypeName)
                    select p;

            var queryResult = q.Where(p =>
            {
                var match = p.DeclaringType.Name == entityTypeName;
                if (!match && entType != null)
                {
                    //Is a fully qualified name....
                    //一个名称是否完全限定
                    match = entType.Name == p.DeclaringType.Name;
                }

                return match;

            }).ToDictionary(p => p.Name, p => p.TypeUsage.Facets);

            return queryResult;
        }

        public static string DbName(this IDbContext context)
        {
            var connection = ((IObjectContextAdapter)context).ObjectContext.Connection as EntityConnection;
            if (connection == null)
                return string.Empty;

            return connection.StoreConnection.Database;
        }

        #endregion
    }
}