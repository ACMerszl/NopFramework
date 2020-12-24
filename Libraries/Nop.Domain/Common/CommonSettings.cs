using System.Collections.Generic;
using Nop.Core.Domain.Common;
using Nop.Domain.Configuration;

namespace Nop.Domain.Common
{
    /// <summary>
    ///  通用设置
    /// </summary>
    public class CommonSettings : ISettings
    {
        public CommonSettings()
        { 
            IgnoreLogWordlist = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether stored procedures are enabled (should be used if possible)
        /// 获取或设置一个值，该值指示是否启用了存储过程（如果可能，应使用）
        /// </summary>
        public bool UseStoredProceduresIfSupported { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use stored procedure (if supported) for loading categories (it's much faster in admin area with a large number of categories than the LINQ implementation)
        /// 获取或设置一个值，该值指示是否使用存储过程（如果支持）来加载类别
        /// （在具有大量类别的管理区域中，这比LINQ实现要快得多）
        /// </summary>
        public bool UseStoredProcedureForLoadingCategories { get; set; }
           
        /// <summary>
        /// Gets or sets a value indicating whether 404 errors (page or file not found) should be logged
        /// </summary>
        public bool Log404Errors { get; set; }
          
        /// <summary>
        /// Gets or sets ignore words (phrases) to be ignored when logging errors/messages
        /// </summary>
        public List<string> IgnoreLogWordlist { get; set; }
         
    }
}