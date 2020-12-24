using System.Collections.Generic;
using System.ComponentModel;
using Nop.Core.ComponentModel; 
using Nop.Core.Infrastructure;

namespace Nop.Core
{
    /// <summary>
    /// Startup task for the registration custom type converters
    /// 注册自定义类型转换器的启动任务
    /// </summary>
    public class TypeConverterRegistrationStartUpTask : IStartupTask
    {
        /// <summary>
        /// Executes a task
        /// 执行一个任务
        /// </summary>
        public void Execute()
        {
            //lists列表
            TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            TypeDescriptor.AddAttributes(typeof(List<decimal>), new TypeConverterAttribute(typeof(GenericListTypeConverter<decimal>)));
            TypeDescriptor.AddAttributes(typeof(List<string>), new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));

            //dictionaries字典
            TypeDescriptor.AddAttributes(typeof(Dictionary<int, int>), new TypeConverterAttribute(typeof(GenericDictionaryTypeConverter<int, int>)));
  
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
