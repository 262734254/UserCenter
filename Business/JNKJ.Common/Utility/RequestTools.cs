using System;
using System.Web;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Security;
namespace JNKJ.Common.Utility
{
    /// <summary>
    /// Request操作类
    /// </summary>
    public class RequestTools
    {
        #region  判断当前页面是否接收到了Post请求
        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        public static bool IsPost()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("POST");
        }
        #endregion
        #region  判断当前页面是否接收到了Get请求
        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        public static bool IsGet()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("GET");
        }
        #endregion
        #region 返回指定的服务器变量信息
        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="strName">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerString(string strName)
        {
            if (HttpContext.Current.Request.ServerVariables[strName] == null)
                return "";
            return HttpContext.Current.Request.ServerVariables[strName].ToString();
        }
        #endregion
        #region 返回上一个页面的地址
        /// <summary>
        /// 返回上一个页面的地址
        /// </summary>
        /// <returns>上一个页面的地址</returns>
        public static string GetUrlReferrer()
        {
            string retVal = null;
            try
            {
                retVal = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            catch { }
            if (retVal == null)
                return "";
            return retVal;
        }
        #endregion
        #region 得到当前完整主机头
        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
                return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());
            return request.Url.Host;
        }
        #endregion
        #region   得到主机头
        /// <summary>
        /// 得到主机头
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }
        #endregion
        #region  获取指定URL下的文件名
        /// <summary>
        /// 获取指定URL下的文件名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetUrlFileName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "";
            //如果不是合法的URL地址
            if (!Validator.IsURL(filePath)) return "";
            string[] urlArr = filePath.Split('/');
            string filename = urlArr[urlArr.Length - 1].ToLower();
            if (filename.Split('.').Length >= 2) return filename;
            return "";
        }
        #endregion
        #region 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// <summary>
        /// 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// </summary>
        /// <returns>原始 URL</returns>
        public static string GetRawUrl()
        {
            return HttpContext.Current.Request.RawUrl;
        }
        #endregion
        #region 判断当前访问是否来自浏览器软件
        /// <summary>
        /// 判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public static bool IsBrowserGet()
        {
            string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox" };
            string curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
            for (int i = 0; i < BrowserName.Length; i++)
            {
                if (curBrowser.IndexOf(BrowserName[i]) >= 0)
                    return true;
            }
            return false;
        }
        #endregion
        #region  判断是否来自搜索引擎链接
        /// <summary>
        /// 判断是否来自搜索引擎链接
        /// </summary>
        /// <returns>是否来自搜索引擎链接</returns>
        public static bool IsSearchEnginesGet()
        {
            if (HttpContext.Current.Request.UrlReferrer == null)
                return false;
            string[] SearchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
            string tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
            for (int i = 0; i < SearchEngine.Length; i++)
            {
                if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
                    return true;
            }
            return false;
        }
        #endregion
        #region  获得当前完整Url地址
        /// <summary>
        /// 获得当前完整Url地址
        /// </summary>
        /// <returns>当前完整Url地址</returns>
        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }
        #endregion
        #region  获得指定Url参数的值
        /// <summary>
        /// 获得指定Url参数的值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName)
        {
            return GetQueryString(strName, false);
        }
        #endregion
        #region   获得指定Url参数的值
        /// <summary>
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="strName">Url参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.QueryString[strName] == null)
                return "";
            if (sqlSafeCheck && !SqlTools.IsSafeSqlString(HttpContext.Current.Request.QueryString[strName]))
                return "unsafe string";
            return HttpContext.Current.Request.QueryString[strName];
        }
        /// <summary>
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="strName">Url参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName, string defaultVal)
        {
            string query = GetQueryString(strName);
            return string.IsNullOrEmpty(query) ? defaultVal : query;
        }
        #endregion
        #region 获取请求的内容
        /// <summary>
        /// 获取请求的内容
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns>返回内容</returns>
        public static string GetRequestContent(HttpRequest request)
        {
            string content = string.Empty;
            byte[] bytes = request.BinaryRead(request.ContentLength);
            if (bytes != null && bytes.Length > 0)
                content = request.ContentEncoding.GetString(bytes);
            return content;
        }
        #endregion
        #region   获得当前页面的名称
        /// <summary>
        /// 获得当前页面的名称
        /// </summary>
        /// <returns>当前页面的名称</returns>
        public static string GetPageName()
        {
            string[] urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
            return urlArr[urlArr.Length - 1].ToLower();
        }
        #endregion
        #region  返回表单或Url参数的总个数
        /// <summary>
        /// 返回表单或Url参数的总个数
        /// </summary>
        /// <returns></returns>
        public static int GetParamCount()
        {
            return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
        }
        #endregion
        #region  获得指定表单参数的值
        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName)
        {
            return GetFormString(strName, false);
        }
        #endregion
        #region  获得指定表单参数的值
        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.Form[strName] == null)
                return "";
            if (sqlSafeCheck && !SqlTools.IsSafeSqlString(HttpContext.Current.Request.Form[strName]))
                return "unsafe string";
            return HttpContext.Current.Request.Form[strName];
        }
        #endregion
        #region  获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName)
        {
            return GetString(strName, false);
        }
        #endregion
        #region  获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName, bool sqlSafeCheck)
        {
            if ("".Equals(GetQueryString(strName)))
                return GetFormString(strName, sqlSafeCheck);
            else
                return GetQueryString(strName, sqlSafeCheck);
        }
        #endregion
        #region  获得指定Url参数的int类型值
        /// <summary>
        /// 获得指定Url参数的int类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的int类型值</returns>
        public static int GetQueryInt(string strName)
        {
            return TypeConverter.StrToInt(HttpContext.Current.Request.QueryString[strName], 0);
        }
        #endregion
        #region  获得指定Url参数的int类型值
        /// <summary>
        /// 获得指定Url参数的int类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static int GetQueryInt(string strName, int defValue)
        {
            return TypeConverter.StrToInt(HttpContext.Current.Request.QueryString[strName], defValue);
        }
        #endregion
        #region  获得指定表单参数的int类型值
        /// <summary>
        /// 获得指定表单参数的int类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的int类型值</returns>
        public static int GetFormInt(string strName, int defValue)
        {
            return TypeConverter.StrToInt(HttpContext.Current.Request.Form[strName], defValue);
        }
        #endregion
        #region  获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// <summary>
        /// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static int GetInt(string strName, int defValue)
        {
            if (GetQueryInt(strName, defValue) == defValue)
                return GetFormInt(strName, defValue);
            else
                return GetQueryInt(strName, defValue);
        }
        #endregion
        #region  获得指定Url参数的float类型值
        /// <summary>
        /// 获得指定Url参数的float类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static float GetQueryFloat(string strName, float defValue)
        {
            return TypeConverter.StrToFloat(HttpContext.Current.Request.QueryString[strName], defValue);
        }
        #endregion
        #region  获得指定表单参数的float类型值
        /// <summary>
        /// 获得指定表单参数的float类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的float类型值</returns>
        public static float GetFormFloat(string strName, float defValue)
        {
            return TypeConverter.StrToFloat(HttpContext.Current.Request.Form[strName], defValue);
        }
        #endregion
        #region  获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// <summary>
        /// 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static float GetFloat(string strName, float defValue)
        {
            if (GetQueryFloat(strName, defValue) == defValue)
                return GetFormFloat(strName, defValue);
            else
                return GetQueryFloat(strName, defValue);
        }
        #endregion
        #region  获得当前页面客户端的IP
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;
            if (string.IsNullOrEmpty(result) || !Validator.IsIP(result))
                return "127.0.0.1";
            return result;
        }
        #endregion
        #region  保存用户上传的文件
        /// <summary>
        /// 保存用户上传的文件
        /// </summary>
        /// <param name="path">保存路径</param>
        public static void SaveRequestFile(string path)
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                HttpContext.Current.Request.Files[0].SaveAs(path);
            }
        }
        #endregion
        #region  根据网页的HTML内容提取网页的Encoding
        /// <summary>
        /// 根据网页的HTML内容提取网页的Encoding
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(string html)
        {
            string pattern = @"(?i)\bcharset=(?<charset>[-a-zA-Z_0-9]+)";
            string charset = Regex.Match(html, pattern).Groups["charset"].Value;
            try { return Encoding.GetEncoding(charset); }
            catch (ArgumentException) { return null; }
        }
        #endregion
        #region 根据网页的HTML内容提取网页的Title
        /// <summary>
        /// 根据网页的HTML内容提取网页的Title
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        static string GetTitle(string html)
        {
            string pattern = @"(?si)<title(?:\s+(?:""[^""]*""|'[^']*'|[^""'>])*)?>(?<title>.*?)</title>";
            return Regex.Match(html, pattern).Groups["title"].Value.Trim();
        }
        #endregion
        #region 得到真实IP以及所在地详细信息（Porschev）
        ///<summary>
        ///得到真实IP以及所在地详细信息（Porschev）
        ///</summary>
        ///<returns></returns>
        public static string GetIpDetails()
        {
            //IP地址
            string ip = UtilityHelper.GetRealIP();
            //设置获取IP地址和国家源码的网址
            //string ip = "222.182.90.214";
            //string url = "http://ip.taobao.com/service/getIpInfo.php?ip=" + ip;
            string url = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json&ip=" + ip;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Timeout = 19600;
            HttpWebResponse hwrs = (HttpWebResponse)request.GetResponse();
            Stream mystream = hwrs.GetResponseStream();
            StreamReader rd = new StreamReader(mystream);
            StringBuilder sb = new StringBuilder();
            while (-1 != rd.Peek())
            {
                sb.Append(rd.ReadLine() + "\r\n");
            }
            hwrs.Close();
            return sb.ToString();
        }
        #endregion
        #region MyRegion
        private static AspNetHostingPermissionLevel? _trustLevel = null;
        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum
                _trustLevel = AspNetHostingPermissionLevel.None;
                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in
                        new AspNetHostingPermissionLevel[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return _trustLevel.Value;
        }
        #endregion
        #region 判断img地址是否有效
        public static bool RemoteFileExists(string fileUrl)
        {
            bool result = false;//下载结果
            WebResponse response = null;
            try
            {
                WebRequest req = WebRequest.Create(fileUrl);
                response = req.GetResponse();
                result = response == null ? false : true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }
        #endregion
    }
}
