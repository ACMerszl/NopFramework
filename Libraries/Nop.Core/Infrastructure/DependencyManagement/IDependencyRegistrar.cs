using Autofac;
using Nop.Core.Configuration;

namespace Nop.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// Dependency registrar interface
    /// 依赖注册器界面
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// 注册服务和接口
        /// </summary>
        /// <param name="builder">Container builder容器建造者</param>
        /// <param name="typeFinder">Type finder类型查找器</param>
        /// <param name="config">Config</param>
        void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config);

        /// <summary>
        /// Order of this dependency registrar implementation
        /// 依赖注册器实现的顺序
        /// 命令；顺序；规则；【贸易】订单
        /// </summary>
        int Order { get; }
    }
}
