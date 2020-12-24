
using Nop.Domain;

namespace Nop.Core.Events
{
    /// <summary>
    /// A container for passing entities that have been deleted. This is not used for entities that are deleted logicaly via a bit column.
    /// 用于传递已删除实体的容器。这不适用于通过位列逻辑删除的实体。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityDeleted<T> where T : BaseEntity
    {
        public EntityDeleted(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }
}
