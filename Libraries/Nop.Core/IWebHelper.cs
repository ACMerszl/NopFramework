using System.Web;

namespace Nop.Core
{
    /// <summary>
    /// Represents a common helper
    /// ��ʾ������������
    /// </summary>
    public partial interface IWebHelper
    {
        /// <summary>
        /// Get URL referrer
        /// ��ȡURL����
        /// </summary>
        /// <returns>URL referrerURL����</returns>
        string GetUrlReferrer();

        /// <summary>
        /// Get context IP address
        /// ��ȡ������IP��ַ
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// Gets this page name
        /// ��ȡ��ҳ������
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings
        /// ָʾ�Ƿ������ѯ�ַ�����ֵ</param>
        /// <returns>Page nameҳ������</returns>
        string GetSiteUrl(bool includeQueryString);

        /// <summary>
        /// Gets this page name
        /// ��ȡ��ҳ������
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings
        /// ָʾ�Ƿ������ѯ�ַ�����ֵ</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected page
        /// ָʾ�Ƿ��ȡ��SSL������ҳ���ֵ</param>
        /// <returns>Page nameҳ������</returns>
        string GetSiteUrl(bool includeQueryString, bool useSsl);

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ�ȫ
        /// </summary>
        /// <returns>true - secured, false - not secured��-��ȫ����-����ȫ</returns>
        bool IsCurrentConnectionSecured();

        /// <summary>
        /// Gets server variable by name
        /// �����ƻ�ȡ����������
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable����������</returns>
        string ServerVariables(string name);

        /// <summary>
        /// Gets store host location
        /// ��ȡ�洢����λ��
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store host location�洢����λ��</returns>
        string GetSiteHost(bool useSsl);

        /// <summary>
        /// Gets store location
        /// ��ȡ�洢λ��
        /// </summary>
        /// <returns>Store location�洢λ��</returns>
        string GetSiteLocation();

        /// <summary>
        /// Gets store location
        /// ��ȡ�洢λ��
        /// </summary>
        /// <param name="useSsl">Use SSLʹ��SSL</param>
        /// <returns>Store location�洢λ��</returns>
        string GetSiteLocation(bool useSsl);

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// ����������Դ��cms���治��Ҫ����ĵ�����Դ֮һ���򷵻�true��
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.
        /// ��������Ŀ���Ǿ�̬��Դ�ļ�����ΪTrue��</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// ��Щ�Ǳ���Ϊ�Ǿ�̬��Դ���ļ���չ��:
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
        /// Modifies query string�޸Ĳ�ѯ�ַ���
        /// </summary>
        /// <param name="url">Url to modify�޸���ַ</param>
        /// <param name="queryStringModification">Query string modification��ѯ�ַ����޸�</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New url</returns>
        string ModifyQueryString(string url, string queryStringModification, string anchor);

        /// <summary>
        /// Remove query string from url����ַ��ɾ����ѯ�ַ���
        /// </summary>
        /// <param name="url">Url to modify�޸���ַ</param>
        /// <param name="queryString">Query string to remove��ѯ�ַ���ɾ��</param>
        /// <returns>New url</returns>
        string RemoveQueryString(string url, string queryString);

        /// <summary>
        /// Gets query string value by name
        /// �����ƻ�ȡ��ѯ�ַ���ֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Parameter name��������</param>
        /// <returns>Query string value��ѯ�ַ���ֵ</returns>
        T QueryString<T>(string name);

        /// <summary>
        /// Restart application domain��������Ӧ�ó�����
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether we should made redirection after restart
        /// һ��ֵ����ֵָʾ�����Ƿ�Ӧ����������������ض���</param>
        /// <param name="redirectUrl">Redirect URL; empty string if you want to redirect to the current page URL
        /// �ض�����ַ�� ���Ҫ�ض��򵽵�ǰҳ��URL����Ϊ���ַ���</param>
        void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "");

        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// ��ȡһ��ֵ����ֵָʾ�ͻ����Ƿ��ض�����λ��
        /// </summary>
        bool IsRequestBeingRedirected { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// ��ȡ������һ��ֵ����ֵָʾ�Ƿ�ʹ��POST���ͻ����ض�����λ��
        /// </summary>
        bool IsPostBeingDone { get; set; }
    }
}
