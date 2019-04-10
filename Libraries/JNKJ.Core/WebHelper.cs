using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using JNKJ.Core.Data;
namespace JNKJ.Core
{
    /// <summary>
    /// Represents 帮助类
    /// </summary>
    public partial class WebHelper : IWebHelper
    {
        #region Fields
        private readonly HttpContextBase _httpContext;
        #endregion
        #region Utilities
        protected virtual Boolean IsRequestAvailable(HttpContextBase httpContext)
        {
            if (httpContext == null)
                return false;
            try
            {
                if (httpContext.Request == null)
                    return false;
            }
            catch (HttpException ex)
            {
                return false;
            }
            return true;
        }
        protected virtual bool TryWriteWebConfig()
        {
            try
            {
                File.SetLastWriteTimeUtc(MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }
        protected virtual bool TryWriteGlobalAsax()
        {
            try
            {
                File.SetLastWriteTimeUtc(MapPath("~/global.asax"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext">HTTP上下文</param>
        public WebHelper(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
        }
        /// <summary>
        /// 获取 URL 来源
        /// </summary>
        /// <returns>URL来源</returns>
        public virtual string GetUrlReferrer()
        {
            string referrerUrl = string.Empty;
            //在一些实例中，来源会为空(例如, IE 8)
            if (IsRequestAvailable(_httpContext) && _httpContext.Request.UrlReferrer != null)
                referrerUrl = _httpContext.Request.UrlReferrer.PathAndQuery;
            return referrerUrl;
        }
        /// <summary>
        /// 获取当前的IP地址
        /// </summary>
        /// <returns>URL来源</returns>
        public virtual string GetCurrentIpAddress()
        {
            if (!IsRequestAvailable(_httpContext))
                return string.Empty;
            var result = "";
            if (_httpContext.Request.Headers != null)
            {
                //定义 X-Forwarded-For (XFF) HTTP header 标准
                //用于识别客户端的ip地址 
                //通过HTTP代理或负载均衡器连接到Web服务器。
                var forwardedHttpHeader = "X-FORWARDED-FOR";
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ForwardedHTTPheader"]))
                {
                    //但在某些情况下，服务器使用其他HTTP头 
                    //这种情况管理员可以指定一个定制转发的HTTP头
                    forwardedHttpHeader = ConfigurationManager.AppSettings["ForwardedHTTPheader"];
                }
                //用于标识连接到Web服务器的客户端的原始IP地址
                //通过HTTP代理或负载均衡器. 
                string xff = _httpContext.Request.Headers.AllKeys
                    .Where(x => forwardedHttpHeader.Equals(x, StringComparison.InvariantCultureIgnoreCase))
                    .Select(k => _httpContext.Request.Headers[k])
                    .FirstOrDefault();
                //排除私有地址
                if (!string.IsNullOrEmpty(xff))
                {
                    string lastIp = xff.Split(new char[] { ',' }).FirstOrDefault();
                    result = lastIp;
                }
            }
            if (string.IsNullOrEmpty(result) && _httpContext.Request.UserHostAddress != null)
            {
                result = _httpContext.Request.UserHostAddress;
            }
            //校验
            if (result == "::1")
                result = "127.0.0.1";
            //删除端口
            if (!string.IsNullOrEmpty(result))
            {
                int index = result.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                    result = result.Substring(0, index);
            }
            return result;
        }
        /// <summary>
        /// 获取当前页面的URL
        /// </summary>
        /// <param name="includeQueryString">指示是否包含查询字符串</param>
        /// <returns>返回页面URL</returns>
        public virtual string GetThisPageUrl(bool includeQueryString)
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetThisPageUrl(includeQueryString, useSsl);
        }
        /// <summary>
        /// 获取当前页面的URL
        /// </summary>
        /// <param name="includeQueryString">是否包含查询参数</param>
        /// <param name="useSsl">是否使用ssl</param>
        /// <returns>返回页面URL</returns>
        public virtual string GetThisPageUrl(bool includeQueryString, bool useSsl)
        {
            string url = string.Empty;
            if (!IsRequestAvailable(_httpContext))
                return url;
            if (includeQueryString)
            {
                string siteHost = GetSiteHost(useSsl);
                if (siteHost.EndsWith("/"))
                    siteHost = siteHost.Substring(0, siteHost.Length - 1);
                url = siteHost + _httpContext.Request.RawUrl;
            }
            else
            {
                if (_httpContext.Request.Url != null)
                {
                    url = _httpContext.Request.Url.GetLeftPart(UriPartial.Path);
                }
            }
            url = url.ToLowerInvariant();
            return url;
        }
        /// <summary>
        /// 获取一个值，该值指示当前连接是否已被保护。 
        /// </summary>
        /// <returns>true - 保护, false - 未保护</returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            bool useSSL = false;
            if (IsRequestAvailable(_httpContext))
            {
                useSSL = _httpContext.Request.IsSecureConnection;
                //如果你的服务器采用了负载，issecureconnection没有设置为true,那么用下面的操作
                //useSSL = _httpContext.Request.ServerVariables["HTTP_CLUSTER_HTTPS"] == "on" ? true : false;
            }
            return useSSL;
        }
        /// <summary>
        /// 通过名称获取服务器变量
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>Server variable</returns>
        public virtual string ServerVariables(string name)
        {
            string result = string.Empty;
            try
            {
                if (!IsRequestAvailable(_httpContext))
                    return result;
                if (_httpContext.Request.ServerVariables[name] != null)
                {
                    result = _httpContext.Request.ServerVariables[name];
                }
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }
        /// <summary>
        ///获取网站本地位置
        /// </summary>
        /// <param name="useSsl">是否使用ssl</param>
        /// <returns>网站主机地址</returns>
        public virtual string GetSiteHost(bool useSsl)
        {
            var result = "";
            var httpHost = ServerVariables("HTTP_HOST");
            if (!string.IsNullOrEmpty(httpHost))
            {
                result = "http://" + httpHost;
                if (!result.EndsWith("/"))
                    result += "/";
            }
            #region Database is not installed
            if (useSsl)
            {
                //安全url未指定
                //自动检测
                result = result.Replace("http:/", "https:/");
            }
            #endregion
            if (!result.EndsWith("/"))
                result += "/";
            return result.ToLowerInvariant();
        }
        /// <summary>
        ///获取站点位置
        /// </summary>
        /// <returns>站点地址</returns>
        public virtual string GetSiteLocation()
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetSiteLocation(useSsl);
        }
        /// <summary>
        /// 获取站点位置
        /// </summary>
        /// <param name="useSsl">是否启用SSL</param>
        /// <returns>站点地址</returns>
        public virtual string GetSiteLocation(bool useSsl)
        {
            //返回宿主环境应用程序虚拟路径;
            string result = GetSiteHost(useSsl);
            if (result.EndsWith("/"))
                result = result.Substring(0, result.Length - 1);
            if (IsRequestAvailable(_httpContext))
                result = result + _httpContext.Request.ApplicationPath;
            if (!result.EndsWith("/"))
                result += "/";
            return result.ToLowerInvariant();
        }
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
        public virtual bool IsStaticResource(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            string path = request.Path;
            string extension = VirtualPathUtility.GetExtension(path);
            if (extension == null) return false;
            switch (extension.ToLower())
            {
                case ".axd":
                case ".ashx":
                case ".bmp":
                case ".css":
                case ".gif":
                case ".htm":
                case ".html":
                case ".ico":
                case ".jpeg":
                case ".jpg":
                case ".js":
                case ".png":
                case ".rar":
                case ".zip":
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 将虚拟路径映射到物理磁盘路径。 
        /// </summary>
        /// <param name="path">映射路径. 例如. "~/bin"</param>
        /// <returns>物理路径. 例如. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //主机
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                //非主机. 例如, 运行在单元测试中
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }
        /// <summary>
        /// 修改请求
        /// </summary>
        /// <param name="url">要修改的URL</param>
        /// <param name="queryStringModification">修改查询字段</param>
        /// <param name="anchor">锚</param>
        /// <returns>处理后的URL</returns>
        public virtual string ModifyQueryString(string url, string queryStringModification, string anchor)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();
            if (queryStringModification == null)
                queryStringModification = string.Empty;
            queryStringModification = queryStringModification.ToLowerInvariant();
            if (anchor == null)
                anchor = string.Empty;
            anchor = anchor.ToLowerInvariant();
            string str = string.Empty;
            string str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                if (!dictionary.ContainsKey(strArray[0]))
                                {
                                    dictionary[strArray[0]] = strArray[1];
                                }
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (string str4 in queryStringModification.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            string[] strArray2 = str4.Split(new char[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrEmpty(anchor))
            {
                str2 = anchor;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2))).ToLowerInvariant();
        }
        /// <summary>
        /// 从url中删除查询字符串 
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="queryString">要移除的查询字段</param>
        /// <returns>处理后的URL</returns>
        public virtual string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();
            if (queryString == null)
                queryString = string.Empty;
            queryString = queryString.ToLowerInvariant();
            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);
                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }
        /// <summary>
        /// 按名称获取查询字符串值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">参数名</param>
        /// <returns>返回值</returns>
        public virtual T QueryString<T>(string name)
        {
            string queryParam = null;
            if (IsRequestAvailable(_httpContext) && _httpContext.Request.QueryString[name] != null)
                queryParam = _httpContext.Request.QueryString[name];
            if (!string.IsNullOrEmpty(queryParam))
                return CommonHelper.To<T>(queryParam);
            return default(T);
        }
        /// <summary>
        /// 重新启动应用程序域 
        /// </summary>
        /// <param name="makeRedirect">指定是否需要在重启后进行重定向</param>
        /// <param name="redirectUrl">重定向地址，如果为空则重定向到当前页</param>
        public virtual void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "")
        {
            if (CommonHelper.GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            {
                //full trust
                HttpRuntime.UnloadAppDomain();
                TryWriteGlobalAsax();
            }
            else
            {
                //信任度
                bool success = TryWriteWebConfig();
                if (!success)
                {
                    throw new ExceptionExt("JNKJ needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                        "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                        "- run the application in a full trust environment, or" + Environment.NewLine +
                        "- give the application write access to the 'web.config' file.");
                }
                success = TryWriteGlobalAsax();
                if (!success)
                {
                    throw new ExceptionExt("JNKJ needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                        "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                        "- run the application in a full trust environment, or" + Environment.NewLine +
                        "- give the application write access to the 'Global.asax' file.");
                }
            }
            // 如果设置了扩展/模块需要一个AppDomain重新启动，这种情况非常少
            if (_httpContext != null && makeRedirect)
            {
                if (string.IsNullOrEmpty(redirectUrl))
                    redirectUrl = GetThisPageUrl(true);
                _httpContext.Response.Redirect(redirectUrl, true /*endResponse*/);
            }
        }
        /// <summary>
        ///获取一个值，该值指示客户端是否正在重定向到新位置
        /// </summary>
        public virtual bool IsRequestBeingRedirected
        {
            get
            {
                var response = _httpContext.Response;
                return response.IsRequestBeingRedirected;
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示客户端是否使用POST重定向到新位置。 
        /// </summary>
        public virtual bool IsPostBeingDone
        {
            get
            {
                if (_httpContext.Items["JNKJ.IsPOSTBeingDone"] == null)
                    return false;
                return Convert.ToBoolean(_httpContext.Items["JNKJ.IsPOSTBeingDone"]);
            }
            set
            {
                _httpContext.Items["JNKJ.IsPOSTBeingDone"] = value;
            }
        }
        #endregion
    }
}
