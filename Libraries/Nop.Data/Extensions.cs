using System;
using System.Data.Entity.Core.Objects; 
using Nop.Domain;

namespace Nop.Data
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get unproxied entity type
        /// 获取未经证实的实体类型
        /// </summary>
        /// <remarks> If your Entity Framework context is proxy-enabled, 
        /// the runtime will create a proxy instance of your entities, 
        /// i.e. a dynamically generated class which inherits from your entity class 
        /// and overrides its virtual properties by inserting specific code useful for example 
        /// for tracking changes and lazy loading.
        /// 如果你的实体框架上下文是启用代理的，运行时将创建你的实体的一个代理实例，
        /// 即一个动态生成的类，它继承自你的实体类，并通过插入特定的代码覆盖它的虚拟属性，
        /// 例如跟踪变化和延迟加载有用的代码。
        /// </remarks>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Type GetUnproxiedEntityType(this BaseEntity entity)
        {
            var userType = ObjectContext.GetObjectType(entity.GetType());
            return userType;
        }
    }
}
