using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;

namespace Nop.Data
{
    /// <summary>
    /// EF任务启动执行
    /// </summary>
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            var settings = EngineContext.Current.Resolve<DataSettings>();
            if (settings != null && settings.IsValid())
            {
                var provider = EngineContext.Current.Resolve<IDataProvider>();
                if (provider == null)
                    throw new NopException("No IDataProvider found");
                provider.SetDatabaseInitializer();
            }
        }

        public int Order
        {
            //ensure that this task is run first 
            //确保该任务首先运行
            get { return -1000; }
        }
    }
}
