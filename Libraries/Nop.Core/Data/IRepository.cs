using System.Collections.Generic;
using System.Linq;
using Nop.Domain;

namespace Nop.Core.Data
{
    /// <summary>
    /// Repository仓库
    /// </summary>
    public partial interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get entity by identifier
        /// 按主键id获取实体
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        T GetById(object id);

        /// <summary>
        /// Insert entity
        /// 插入实体
        /// </summary>
        /// <param name="entity">Entity</param>
        void Insert(T entity);

        /// <summary>
        /// Insert entities
        /// 插入实体集
        /// </summary>
        /// <param name="entities">Entities</param>
        void Insert(IEnumerable<T> entities);

        /// <summary>
        /// Update entity
        /// 更新实体
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(T entity);

        /// <summary>
        /// Update entities
        /// 更新实体集
        /// </summary>
        /// <param name="entities">Entities</param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        /// Delete entity
        /// 删除实体
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(T entity);

        /// <summary>
        /// Delete entities
        /// 删除实体集
        /// </summary>
        /// <param name="entities">Entities</param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// Gets a table
        /// 获得表
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        ///获取一个启用了“无跟踪”的表(EF特性)，仅当你只在只读操作中加载记录时使用它
        /// </summary>
        IQueryable<T> TableNoTracking { get; }
    }
}
