
using Nop.Domain;

namespace Nop.Core.Events
{
    /// <summary>
    /// A container for entities that have been inserted.
    /// 用于插入实体的容器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityInserted<T> where T : BaseEntity
    {
        public EntityInserted(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }
}
