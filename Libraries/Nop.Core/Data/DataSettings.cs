using System;
using System.Collections.Generic;

namespace Nop.Core.Data
{
    /// <summary>
    /// Data settings (connection string information)
    /// 数据设置(连接字符串信息)
    /// 链接配置
    /// </summary>
    public partial class DataSettings
    {
        /// <summary>
        /// Ctor构造函数
        /// </summary>
        public DataSettings()
        {
            RawDataSettings=new Dictionary<string, string>();
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DBType { get; set; }

        /// <summary>
        /// Data provider数据库提供者（同上）
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// Connection string连接字符串
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Raw settings file原始设置文件
        /// </summary>
        public IDictionary<string, string> RawDataSettings { get; private set; }

        /// <summary>
        /// A value indicating whether entered information is valid
        /// 指示输入信息是否有效的值
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !String.IsNullOrEmpty(this.DataProvider) && !String.IsNullOrEmpty(this.DataConnectionString);
        }
    }
}
