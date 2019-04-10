using System.Web;
namespace JNKJ.Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial interface IWebHelper
    {
        /// <summary>
        /// 获取URL链接
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();
        /// <summary>
        /// 获取当前的IP地址
        /// </summary>
        /// <returns>URL来源</returns>
        string GetCurrentIpAddress();
        /// <summary>
        /// 获取当前页面的URL
        /// </summary>
        /// <param name="includeQueryString">是否包含参数</param>
        /// <returns>返回页面URL</returns>
        string GetThisPageUrl(bool includeQueryString);
        /// <summary>
        /// 获取当前页面的URL
        /// </summary>
        /// <param name="includeQueryString">指示是否包含查询字符串</param>
        /// <param name="useSsl">是否使用ssl</param>
        /// <returns>返回页面URL</returns>
        string GetThisPageUrl(bool includeQueryString, bool useSsl);
        /// <summary>
        /// 获取一个值，该值指示当前连接是否已被保护。 
        /// </summary>
        /// <returns>true - 保护, false - 未保护</returns>
        bool IsCurrentConnectionSecured();
        /// <summary>
        /// 通过名称获取服务器变量
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>Server variable</returns>
        string ServerVariables(string name);
        /// <summary>
        ///获取网站本地位置
        /// </summary>
        /// <param name="useSsl">是否使用ssl</param>
        /// <returns>网站主机地址</returns>
        string GetSiteHost(bool useSsl);
        /// <summary>
        ///获取站点位置
        /// </summary>
        /// <returns>站点地址</returns>
        string GetSiteLocation();
        /// <summary>
        /// 获取站点位置
        /// </summary>
        /// <param name="useSsl">是否启用SSL</param>
        /// <returns>站点地址</returns>
        string GetSiteLocation(bool useSsl);
        /// <summary>
        /// 判断返回是否未不需要处理的资源
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>如果是静态资源，返回true</returns>
        /// <remarks>
        /// 静态资源扩展名
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
        /// 将虚拟路径映射到物理磁盘路径。 
        /// </summary>
        /// <param name="path">映射路径. 例如. "~/bin"</param>
        /// <returns>物理路径. 例如. "c:\inetpub\wwwroot\bin"</returns>
        string MapPath(string path);
        /// <summary>
        /// 修改请求
        /// </summary>
        /// <param name="url">要修改的URL</param>
        /// <param name="queryStringModification">修改查询字段</param>
        /// <param name="anchor">锚</param>
        /// <returns>处理后的URL</returns>
        string ModifyQueryString(string url, string queryStringModification, string anchor);
        /// <summary>
        /// 从url中删除查询字符串 
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="queryString">要移除的查询字段</param>
        /// <returns>处理后的URL</returns>
        string RemoveQueryString(string url, string queryString);
        /// <summary>
        /// 按名称获取查询字符串值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">参数名</param>
        /// <returns>返回值</returns>
        T QueryString<T>(string name);
        /// <summary>
        /// 重新启动应用程序域 
        /// </summary>
        /// <param name="makeRedirect">指定是否需要在重启后进行重定向</param>
        /// <param name="redirectUrl">重定向地址，如果为空则重定向到当前页</param>
        void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "");
        /// <summary>
        ///获取一个值，该值指示客户端是否正在重定向到新位置
        /// </summary>
        bool IsRequestBeingRedirected { get; }
        /// <summary>
        /// 获取或设置一个值，该值指示客户端是否使用POST重定向到新位置。 
        /// </summary>
        bool IsPostBeingDone { get; set; }
    }
}
