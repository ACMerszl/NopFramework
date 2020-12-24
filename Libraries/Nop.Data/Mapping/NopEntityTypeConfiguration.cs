using System.Data.Entity.ModelConfiguration;

namespace Nop.Data.Mapping
{
    /// <summary>
    /// 所有实体与数据库的映射配置需要继承自该类。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NopEntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : class
    {
        protected NopEntityTypeConfiguration()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// 开发人员可以在自定义分部类中重写此方法
        ///添加一些自定义初始化代码到构造函数
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }
    }
}