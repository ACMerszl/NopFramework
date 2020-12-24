using System.Web;

namespace Nop.Core
{
    /// <summary>
    /// Represents a common helper
    /// 表示公共帮助程序。
    /// </summary>
    public partial interface IWebHelper
    {
        /// <summary>
        /// Get URL referrer
        /// 获取URL引用
        /// </summary>
        /// <returns>URL referrerURL引用</returns>
        string GetUrlReferrer();

        /// <summary>
        /// Get context IP address
        /// 获取上下文IP地址
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// Gets this page name
        /// 获取此页面名称
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings
        /// 指示是否包括查询字符串的值</param>
        /// <returns>Page name页面名称</returns>
        string GetSiteUrl(bool includeQueryString);

        /// <summary>
        /// Gets this page name
        /// 获取此页面名称
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings
        /// 指示是否包括查询字符串的值</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected page
        /// 指示是否获取受SSL保护的页面的值</param>
        /// <returns>Page name页面名称</returns>
        string GetSiteUrl(bool includeQueryString, bool useSsl);

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// 获取一个值，该值指示当前连接是否安全
        /// </summary>
        /// <returns>true - secured, false - not secured真-安全，假-不安全</returns>
        bool IsCurrentConnectionSecured();

        /// <summary>
        /// Gets server variable by name
        /// 按名称获取服务器变量
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable服务器变量</returns>
        string ServerVariables(string name);

        /// <summary>
        /// Gets store host location
        /// 获取存储主机位置
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store host location存储主机位置</returns>
        string GetSiteHost(bool useSsl);

        /// <summary>
        /// Gets store location
        /// 获取存储位置
        /// </summary>
        /// <returns>Store location存储位置</returns>
        string GetSiteLocation();

        /// <summary>
        /// Gets store location
        /// 获取存储位置
        /// </summary>
        /// <param name="useSsl">Use SSL使用SSL</param>
        /// <returns>Store location存储位置</returns>
        string GetSiteLocation(bool useSsl);

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// 如果请求的资源是cms引擎不需要处理的典型资源之一，则返回true。
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.
        /// 如果请求的目标是静态资源文件，则为True。</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// 这些是被认为是静态资源的文件扩展名:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        bool IsStaticResource(HttpRequest request);

        /// <summary>
        /// Modifies query string修改查询字符串
        /// </summary>
        /// <param name="url">Url to modify修改网址</param>
        /// <param name="queryStringModification">Query string modification查询字符串修改</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New url</returns>
        string ModifyQueryString(string url, string queryStringModification, string anchor);

        /// <summary>
        /// Remove query string from url从网址中删除查询字符串
        /// </summary>
        /// <param name="url">Url to modify修改网址</param>
        /// <param name="queryString">Query string to remove查询字符串删除</param>
        /// <returns>New url</returns>
        string RemoveQueryString(string url, string queryString);

        /// <summary>
        /// Gets query string value by name
        /// 按名称获取查询字符串值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Parameter name参数名称</param>
        /// <returns>Query string value查询字符串值</returns>
        T QueryString<T>(string name);

        /// <summary>
        /// Restart application domain重新启动应用程序域
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether we should made redirection after restart
        /// 一个值，该值指示我们是否应在重新启动后进行重定向</param>
        /// <param name="redirectUrl">Redirect URL; empty string if you want to redirect to the current page URL
        /// 重定向网址； 如果要重定向到当前页面URL，则为空字符串</param>
        void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "");

        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// 获取一个值，该值指示客户端是否被重定向到新位置
        /// </summary>
        bool IsRequestBeingRedirected { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// 获取或设置一个值，该值指示是否使用POST将客户端重定向到新位置
        /// </summary>
        bool IsPostBeingDone { get; set; }
    }
}
