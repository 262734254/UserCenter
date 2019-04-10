using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using System.Net;
using System.Configuration;
using System.IO.Compression;
namespace JNKJ.Common.Utility
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class UtilityHelper
    {
        #region Constants
        public const string ASSEMBLY_VERSION = "0.0.1";
        public const string ASSEMBLY_YEAR = "2011";
        public static Regex RegexFont = new Regex(@"<font color=" + "\".*?\"" + @">([\s\S]+?)</font>", UtilityHelper.GetRegexCompiledOptions());
        private static FileVersionInfo AssemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        private static string TemplateCookieName = string.Format("templateid_{0}_{1}_{2}", AssemblyFileVersion.FileMajorPart, AssemblyFileVersion.FileMinorPart, AssemblyFileVersion.FileBuildPart);
        #endregion
        #region 得到正则编译参数设置
        /// <summary>
        /// 得到正则编译参数设置
        /// </summary>
        /// <returns>参数设置</returns>
        public static RegexOptions GetRegexCompiledOptions()
        {
            return RegexOptions.None;
        }
        #endregion
        #region 获得当前绝对路径
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        #endregion
        #region 以指定的ContentType输出指定文件文件
        /// <summary>
        /// 以指定的ContentType输出指定文件文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="filename">输出的文件名</param>
        /// <param name="filetype">将文件输出时设置的ContentType</param>
        public static void ResponseFile(string filepath, string filename, string filetype)
        {
            Stream iStream = null;
            // 缓冲区为10k
            byte[] buffer = new byte[10000];
            // 文件长度
            int length;
            // 需要读的数据长度
            long dataToRead;
            try
            {
                // 打开文件
                iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                // 需要读的数据长度
                dataToRead = iStream.Length;
                HttpContext.Current.Response.ContentType = filetype;
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + UtilityHelper.UrlEncode(filename.Trim()).Replace("+", " "));
                while (dataToRead > 0)
                {
                    // 检查客户端是否还处于连接状态
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10000);
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        buffer = new byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        // 如果不再连接则跳出死循环
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    // 关闭文件
                    iStream.Close();
                }
            }
            HttpContext.Current.Response.End();
        }
        #endregion
        #region 判断文件名是否为浏览器可以直接显示的图片文件名
        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFilename(string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".") == -1)
                return false;
            string extname = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif");
        }
        #endregion
        #region MD5函数
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        /// <summary>
        /// MD5　32位加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5For32(string str)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2").ToUpper());
            }
            return sb.ToString();
        }
        #endregion
        #region SHA256函数
        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }
        #endregion
        #region DES加密/解密
        //设置加密机密的KEY
        private static byte[] rgbKey = { 0x01, 0x24, 0x37, 0x45, 0xAF, 0xAB, 0x1d, 0xe3 };
        private static byte[] rgbIV = { 0x37, 0x3A, 0x5D, 0x48, 0x95, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public static string TxtEnDes(string KeyValue)
        {
            byte[] clearTextArray = Encoding.UTF8.GetBytes(KeyValue);
            //申明一个DES对象 
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            //加密对象 
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(clearTextArray, 0, clearTextArray.Length);
            cStream.FlushFinalBlock();
            string revalue = Convert.ToBase64String(mStream.ToArray());
            cStream.Dispose();
            mStream.Dispose();
            return revalue;
        }
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public static string TxtDecDes(string KeyValue)
        {
            byte[] decryptArray = Convert.FromBase64String(KeyValue);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(decryptArray, 0, decryptArray.Length);
            cStream.FlushFinalBlock();
            string revalue = Encoding.UTF8.GetString(mStream.ToArray());
            cStream.Dispose();
            mStream.Dispose();
            return revalue;
        }
        #endregion
        #region 获取邮件地址的主机名
        /// <summary>
        /// 获取邮件地址的主机名
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public static string GetEmailHostName(string strEmail)
        {
            if (strEmail.IndexOf("@") < 0)
            {
                return "";
            }
            return strEmail.Substring(strEmail.LastIndexOf("@")).ToLower();
        }
        #endregion
        #region 判断是否为base64字符串
        /// <summary>
        /// 判断是否为base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            //A-Z, a-z, 0-9, +, /, =
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }
        #endregion
        #region 检测是否有危险的可能用于链接的字符串
        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }
        #endregion
        #region 清理字符串
        /// <summary>
        /// 清理字符串
        /// </summary>
        public static string CleanInput(string strIn)
        {
            return Regex.Replace(strIn.Trim(), @"[^\w\.@-]", "");
        }
        #endregion
        #region 返回URL中结尾的文件名
        /// <summary>
        /// 返回URL中结尾的文件名
        /// </summary>		
        public static string GetFilename(string url)
        {
            if (url == null)
            {
                return "";
            }
            string[] strs1 = url.Split(new char[] { '/' });
            return strs1[strs1.Length - 1].Split(new char[] { '?' })[0];
        }
        #endregion
        #region 根据阿拉伯数字返回月份的名称(可更改为某种语言)
        /// <summary>
        /// 根据阿拉伯数字返回月份的名称(可更改为某种语言)
        /// </summary>	
        public static string[] Monthes
        {
            get
            {
                return new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            }
        }
        #endregion
        #region 替换回车换行符为html换行符
        /// <summary>
        /// 替换回车换行符为html换行符
        /// </summary>
        public static string StrFormat(string str)
        {
            string str2;
            if (str == null)
            {
                str2 = "";
            }
            else
            {
                str = str.Replace("\r\n", "<br />");
                str = str.Replace("\n", "<br />");
                str2 = str;
            }
            return str2;
        }
        #endregion
        #region 返回标准日期格式string
        /// <summary>
        /// 返回标准日期格式string
        /// </summary>
        public static string GetDate()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd");
        }
        #endregion
        #region 返回指定日期格式
        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        public static string GetDate(string datetimestr, string replacestr)
        {
            if (datetimestr == null)
                return replacestr;
            if (datetimestr.Equals(""))
                return replacestr;
            try
            {
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr);
            }
            catch
            {
                return replacestr;
            }
            return datetimestr;
        }
        #endregion
        #region 返回标准时间格式string
        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetTime()
        {
            return DateTime.UtcNow.ToString("HH:mm:ss");
        }
        #endregion
        #region 返回标准时间格式string
        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion
        #region 返回相对于当前时间的相对天数
        /// <summary>
        /// 返回相对于当前时间的相对天数
        /// </summary>
        public static string GetDateTime(int relativeday)
        {
            return DateTime.UtcNow.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion
        #region 返回标准时间格式string
        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTimeF()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }
        #endregion
        #region 返回标准时间
        /// <summary>
        /// 返回标准时间 
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            if (fDateTime == "0000-0-0 0:00:00")
                return fDateTime;
            DateTime time = new DateTime(1900, 1, 1, 0, 0, 0, 0);
            if (DateTime.TryParse(fDateTime, out time))
                return time.ToString(formatStr);
            else
                return "N/A";
        }
        #endregion
        #region 返回标准时间 yyyy-MM-dd HH:mm:ss
        /// <summary>
        /// 返回标准时间 yyyy-MM-dd HH:mm:ss
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }
        #endregion
        #region 返回标准时间 yyyy-MM-dd
        /// <summary>
        /// 返回标准时间 yyyy-MM-dd
        /// </sumary>
        public static string GetStandardDate(string fDate)
        {
            return GetStandardDateTime(fDate, "yyyy-MM-dd");
        }
        #endregion
        #region 返回标准时间 mm-dd
        /// <summary>
        /// 返回标准时间 mm-dd
        /// </summary>
        /// <param name="fDate"></param>
        /// <returns></returns>
        public static string GetStandardDay(string fDate)
        {
            return GetStandardDateTime(fDate, "MM-dd");
        }
        #endregion
        #region 获取真实的IP地址
        /// <summary>
        /// 获取真实的IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetRealIP()
        {
            return RequestTools.GetIP();
        }
        #endregion
        #region 转换为静态html
        /// <summary>
        /// 转换为静态html
        /// </summary>
        public void transHtml(string path, string outpath)
        {
            Page page = new Page();
            StringWriter writer = new StringWriter();
            page.Server.Execute(path, writer);
            FileStream fs;
            if (File.Exists(page.Server.MapPath("") + "\\" + outpath))
            {
                File.Delete(page.Server.MapPath("") + "\\" + outpath);
                fs = File.Create(page.Server.MapPath("") + "\\" + outpath);
            }
            else
            {
                fs = File.Create(page.Server.MapPath("") + "\\" + outpath);
            }
            byte[] bt = Encoding.Default.GetBytes(writer.ToString());
            fs.Write(bt, 0, bt.Length);
            fs.Close();
        }
        #endregion
        #region 返回 HTML 字符串的编码结果
        /// <summary>
        /// 返回 HTML 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }
        #endregion
        #region 返回 HTML 字符串的解码结果
        /// <summary>
        /// 返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }
        #endregion
        #region 返回 URL 字符串的编码结果
        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }
        #endregion
        #region 返回 URL 字符串的编码结果
        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }
        #endregion
        #region 返回指定目录下的非 UTF8 字符集文件
        /// <summary>
        /// 返回指定目录下的非 UTF8 字符集文件
        /// </summary>
        /// <param name="Path">路径</param>
        /// <returns>文件名的字符串数组</returns>
        public static string[] FindNoUTF8File(string Path)
        {
            StringBuilder filelist = new StringBuilder();
            DirectoryInfo Folder = new DirectoryInfo(Path);
            FileInfo[] subFiles = Folder.GetFiles();
            for (int j = 0; j < subFiles.Length; j++)
            {
                if (subFiles[j].Extension.ToLower().Equals(".htm"))
                {
                    FileStream fs = new FileStream(subFiles[j].FullName, FileMode.Open, FileAccess.Read);
                    bool bUtf8 = IsUTF8(fs);
                    fs.Close();
                    if (!bUtf8)
                    {
                        filelist.Append(subFiles[j].FullName);
                        filelist.Append("\r\n");
                    }
                }
            }
            return StrTools.SplitString(filelist.ToString(), "\r\n");
        }
        #endregion
        #region 判断文件流是否为UTF8字符集
        /// <summary>
        /// 判断文件流是否为UTF8字符集
        /// </summary>
        /// <param name="sbInputStream">文件流</param>
        /// <returns>判断结果</returns>
        private static bool IsUTF8(FileStream sbInputStream)
        {
            int i;
            byte cOctets;  // octets to go in this UTF-8 encoded character 
            byte chr;
            bool bAllAscii = true;
            long iLen = sbInputStream.Length;
            cOctets = 0;
            for (i = 0; i < iLen; i++)
            {
                chr = (byte)sbInputStream.ReadByte();
                if ((chr & 0x80) != 0) bAllAscii = false;
                if (cOctets == 0)
                {
                    if (chr >= 0x80)
                    {
                        do
                        {
                            chr <<= 1;
                            cOctets++;
                        }
                        while ((chr & 0x80) != 0);
                        cOctets--;
                        if (cOctets == 0)
                            return false;
                    }
                }
                else
                {
                    if ((chr & 0xC0) != 0x80)
                        return false;
                    cOctets--;
                }
            }
            if (cOctets > 0)
                return false;
            if (bAllAscii)
                return false;
            return true;
        }
        #endregion
        #region 格式化字节数字符串
        /// <summary>
        /// 格式化字节数字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatBytesStr(int bytes)
        {
            if (bytes > 1073741824)
                return ((double)(bytes / 1073741824)).ToString("0") + "G";
            if (bytes > 1048576)
                return ((double)(bytes / 1048576)).ToString("0") + "M";
            if (bytes > 1024)
                return ((double)(bytes / 1024)).ToString("0") + "K";
            return bytes.ToString() + "Bytes";
        }
        #endregion
        #region 返回相差的秒数
        /// <summary>
        /// 返回相差的秒数
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="Sec"></param>
        /// <returns></returns>
        public static int StrDateDiffSeconds(string Time, int Sec)
        {
            TimeSpan ts = DateTime.UtcNow - DateTime.Parse(Time).AddSeconds(Sec);
            if (ts.TotalSeconds > int.MaxValue)
                return int.MaxValue;
            else if (ts.TotalSeconds < int.MinValue)
                return int.MinValue;
            return (int)ts.TotalSeconds;
        }
        #endregion
        #region 返回相差的分钟数
        /// <summary>
        /// 返回相差的分钟数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static int StrDateDiffMinutes(string time, int minutes)
        {
            if (UtilityHelper.StrIsNullOrEmpty(time))
                return 1;
            TimeSpan ts = DateTime.UtcNow - DateTime.Parse(time).AddMinutes(minutes);
            if (ts.TotalMinutes > int.MaxValue)
                return int.MaxValue;
            else if (ts.TotalMinutes < int.MinValue)
                return int.MinValue;
            return (int)ts.TotalMinutes;
        }
        #endregion
        #region 返回相差的小时数
        /// <summary>
        /// 返回相差的小时数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static int StrDateDiffHours(string time, int hours)
        {
            if (UtilityHelper.StrIsNullOrEmpty(time))
                return 1;
            TimeSpan ts = DateTime.UtcNow - DateTime.Parse(time).AddHours(hours);
            if (ts.TotalHours > int.MaxValue)
                return int.MaxValue;
            else if (ts.TotalHours < int.MinValue)
                return int.MinValue;
            return (int)ts.TotalHours;
        }
        #endregion
        #region 建立文件夹
        /// <summary>
        /// 建立文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateDir(string name)
        {
            return UtilityHelper.MakeSureDirectoryPathExists(name);
        }
        #endregion
        #region 创建目录
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>创建是否成功</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);
        #endregion
        #region 为脚本替换特殊字符串
        /// <summary>
        /// 为脚本替换特殊字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceStrToScript(string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
        }
        #endregion
        #region 获得Assembly版本号
        /// <summary>
        /// 获得Assembly版本号
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyVersion()
        {
            return string.Format("{0}.{1}.{2}", AssemblyFileVersion.FileMajorPart, AssemblyFileVersion.FileMinorPart, AssemblyFileVersion.FileBuildPart);
        }
        #endregion
        #region 获得Assembly产品名称
        /// <summary>
        /// 获得Assembly产品名称
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyProductName()
        {
            return AssemblyFileVersion.ProductName;
        }
        #endregion
        #region 获得Assembly产品版权
        /// <summary>
        /// 获得Assembly产品版权
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyCopyright()
        {
            return AssemblyFileVersion.LegalCopyright;
        }
        #endregion
        #region 移除Html标记
        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            return Regex.Replace(content, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        }
        #endregion
        #region 过滤HTML中的不安全标签
        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }
        #endregion
        #region 将用户组Title中的font标签去掉
        /// <summary>
        /// 将用户组Title中的font标签去掉
        /// </summary>
        /// <param name="title">用户组Title</param>
        /// <returns></returns>
        public static string RemoveFontTag(string title)
        {
            Match m = RegexFont.Match(title);
            if (m.Success)
                return m.Groups[1].Value;
            return title;
        }
        #endregion
        #region 从HTML中获取文本,保留br,p,img
        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return regEx.Replace(HTML, "");
        }
        #endregion
        #region 添加分钟
        /// <summary>
        /// 添加分钟
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string AdDeTime(int times)
        {
            return (DateTime.UtcNow).AddMinutes(times).ToString();
        }
        #endregion
        #region 备份文件
        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!System.IO.File.Exists(sourceFileName))
                throw new FileNotFoundException(sourceFileName + "文件不存在！");
            if (!overwrite && System.IO.File.Exists(destFileName))
                return false;
            try
            {
                System.IO.File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
        #region 备份文件,当目标文件存在时覆盖
        /// <summary>
        /// 备份文件,当目标文件存在时覆盖
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName)
        {
            return BackupFile(sourceFileName, destFileName, true);
        }
        #endregion
        #region 恢复文件
        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName">备份文件名</param>
        /// <param name="targetFileName">要恢复的文件名</param>
        /// <param name="backupTargetFileName">要恢复文件再次备份的名称,如果为null,则不再备份恢复文件</param>
        /// <returns>操作是否成功</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName, string backupTargetFileName)
        {
            try
            {
                if (!System.IO.File.Exists(backupFileName))
                    throw new FileNotFoundException(backupFileName + "文件不存在！");
                if (backupTargetFileName != null)
                {
                    if (!System.IO.File.Exists(targetFileName))
                        throw new FileNotFoundException(targetFileName + "文件不存在！无法备份此文件！");
                    else
                        System.IO.File.Copy(targetFileName, backupTargetFileName, true);
                }
                System.IO.File.Delete(targetFileName);
                System.IO.File.Copy(backupFileName, targetFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }
        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName"></param>
        /// <param name="targetFileName"></param>
        /// <returns></returns>
        public static bool RestoreFile(string backupFileName, string targetFileName)
        {
            return RestoreFile(backupFileName, targetFileName, null);
        }
        #endregion
        #region 获取记录模板id的cookie名称
        /// <summary>
        /// 获取记录模板id的cookie名称
        /// </summary>
        /// <returns></returns>
        public static string GetTemplateCookieName()
        {
            return TemplateCookieName;
        }
        #endregion
        #region 将全角数字转换为数字
        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="SBCCase"></param>
        /// <returns></returns>
        public static string SBCCaseToNumberic(string SBCCase)
        {
            char[] c = SBCCase.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }
        #endregion
        #region 将字符串转换为Color
        /// <summary>
        /// 将字符串转换为Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(string color)
        {
            int red, green, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1].ToString() + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2].ToString() + rgb[2].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2].ToString() + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4].ToString() + rgb[5].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                default:
                    return Color.FromName(color);
            }
        }
        #endregion
        #region 转换长文件名为短文件名
        /// <summary>
        /// 转换长文件名为短文件名
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="repstring"></param>
        /// <param name="leftnum"></param>
        /// <param name="rightnum"></param>
        /// <param name="charnum"></param>
        /// <returns></returns>
        public static string ConvertSimpleFileName(string fullname, string repstring, int leftnum, int rightnum, int charnum)
        {
            string simplefilename = "", leftstring = "", rightstring = "", filename = "";
            string extname = GetFileExtName(fullname);
            if (UtilityHelper.StrIsNullOrEmpty(extname))
                throw new Exception("字符串不含有扩展名信息");
            int filelength = 0, dotindex = 0;
            dotindex = fullname.LastIndexOf('.');
            filename = fullname.Substring(0, dotindex);
            filelength = filename.Length;
            if (dotindex > charnum)
            {
                leftstring = filename.Substring(0, leftnum);
                rightstring = filename.Substring(filelength - rightnum, rightnum);
                if (repstring == "" || repstring == null)
                    simplefilename = leftstring + rightstring + "." + extname;
                else
                    simplefilename = leftstring + repstring + rightstring + "." + extname;
            }
            else
                simplefilename = fullname;
            return simplefilename;
        }
        #endregion
        #region 获取ajax形式的分页链接
        /// <summary>
        /// 获取ajax形式的分页链接
        /// </summary>
        /// <param name="curPage">当前页数</param>
        /// <param name="countPage">总页数</param>
        /// <param name="callback">回调函数</param>
        /// <param name="extendPage">周边页码显示个数上限</param>
        /// <returns></returns>
        public static string GetAjaxPageNumbers(int curPage, int countPage, string callback, int extendPage)
        {
            string pagetag = "page";
            int startPage = 1;
            int endPage = 1;
            string t1 = "<a href=\"###\" onclick=\"" + string.Format(callback, pagetag + "=1");
            string t2 = "<a href=\"###\" onclick=\"" + string.Format(callback, pagetag + "=" + countPage);
            //上翻一页
            string t3 = "";
            //下翻一页
            string t4 = "";
            t1 += "\">&laquo;</a>";
            t2 += "\">&raquo;</a>";
            if (countPage < 1)
                countPage = 1;
            if (extendPage < 3)
                extendPage = 2;
            if (countPage <= 1)
            {
                t3 = "<a href=\"###\" onclick=\"" + string.Format(callback, pagetag + "=1");
            }
            if (countPage > extendPage)
            {
                if (curPage - (extendPage / 2) > 0)
                {
                    if (curPage + (extendPage / 2) < countPage)
                    {
                        startPage = curPage - (extendPage / 2);
                        endPage = startPage + extendPage - 1;
                    }
                    else
                    {
                        endPage = countPage;
                        startPage = endPage - extendPage + 1;
                        t2 = "";
                    }
                }
                else
                {
                    endPage = extendPage;
                    t1 = "";
                }
            }
            else
            {
                startPage = 1;
                endPage = countPage;
                t1 = "";
                t2 = "";
            }
            StringBuilder s = new StringBuilder("");
            s.Append(t1);
            for (int i = startPage; i <= endPage; i++)
            {
                if (i == curPage)
                {
                    s.Append("<span>");
                    s.Append(i);
                    s.Append("</span>");
                }
                else
                {
                    s.Append("<a href=\"###\" onclick=\"");
                    s.Append(string.Format(callback, pagetag + "=" + i));
                    s.Append("\">");
                    s.Append(i);
                    s.Append("</a>");
                }
            }
            s.Append(t2);
            return s.ToString();
        }
        #endregion
        #region 根据Url获得源文件内容
        /// <summary>
        /// 根据Url获得源文件内容
        /// </summary>
        /// <param name="url">合法的Url地址</param>
        /// <returns></returns>
        public static string GetSourceTextByUrl(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Timeout = 20000;//20秒超时
            WebResponse response = request.GetResponse();
            Stream resStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(resStream);
            return sr.ReadToEnd();
        }
        #endregion
        #region 根据URL 获取页面请求的 VIEWSTATE和EVENTVALIDATION
        public static void GetSourceMarkByUrl(string url, ref string _viewstate, ref string _eventvalidation)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.KeepAlive = false;
            HttpWebResponse response = null;
            try
            {
                //Get the response
                response = request.GetResponse() as HttpWebResponse;
                System.IO.Stream responseStream = response.GetResponseStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
                string srcString = reader.ReadToEnd();
                //Get the ViewState
                string viewStateFlag = "id=\"__VIEWSTATE\" value=\"";
                int i = srcString.IndexOf(viewStateFlag) + viewStateFlag.Length;
                int j = srcString.IndexOf("\"", i);
                string viewState = srcString.Substring(i, j - i);
                _viewstate = viewState;
                //Get the ViewState
                string EventValidationFlag = "id=\"__EVENTVALIDATION\" value=\"";
                i = srcString.IndexOf(EventValidationFlag) + EventValidationFlag.Length;
                j = srcString.IndexOf("\"", i);
                string eventValidation = srcString.Substring(i, j - i);
                _eventvalidation = eventValidation;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }
        #endregion
        #region 转换时间为unix时间戳
        /// <summary>
        /// 转换时间为unix时间戳
        /// </summary>
        /// <param name="date">需要传递UTC时间,避免时区误差,例:DataTime.UTCNow</param>
        /// <returns></returns>
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        #endregion
        #region Json特符字符过滤，参见http://www.json.org/
        /// <summary>
        /// Json特符字符过滤，参见http://www.json.org/
        /// </summary>
        /// <param name="sourceStr">要过滤的源字符串</param>
        /// <returns>返回过滤的字符串</returns>
        public static string JsonCharFilter(string sourceStr)
        {
            sourceStr = sourceStr.Replace("\\", "\\\\");
            sourceStr = sourceStr.Replace("\b", "\\\b");
            sourceStr = sourceStr.Replace("\t", "\\\t");
            sourceStr = sourceStr.Replace("\n", "\\\n");
            sourceStr = sourceStr.Replace("\n", "\\\n");
            sourceStr = sourceStr.Replace("\f", "\\\f");
            sourceStr = sourceStr.Replace("\r", "\\\r");
            return sourceStr.Replace("\"", "\\\"");
        }
        #endregion
        #region 获取站点根目录URL
        /// <summary>
        /// 获取站点根目录URL
        /// </summary>
        /// <returns></returns>
        public static string GetRootUrl(string forumPath)
        {
            int port = HttpContext.Current.Request.Url.Port;
            return string.Format("{0}://{1}{2}{3}",
                                 HttpContext.Current.Request.Url.Scheme,
                                 HttpContext.Current.Request.Url.Host.ToString(),
                                 (port == 80 || port == 0) ? "" : ":" + port,
                                 forumPath);
        }
        #endregion
        #region 获取指定文件的扩展名
        /// <summary>
        /// 获取指定文件的扩展名
        /// </summary>
        /// <param name="fileName">指定文件名</param>
        /// <returns>扩展名</returns>
        public static string GetFileExtName(string fileName)
        {
            if (UtilityHelper.StrIsNullOrEmpty(fileName) || fileName.IndexOf('.') <= 0)
                return "";
            fileName = fileName.ToLower().Trim();
            return fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
        }
        #endregion
        #region http POST请求url
        /// <summary>
        /// http POST请求url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetHttpWebResponse(string url)
        {
            return GetHttpWebResponse(url, string.Empty);
        }
        public static string GetHttpWebResponse(string url, Dictionary<string, string> postData)
        {
            string _postData = string.Empty;
            foreach (string key in postData.Keys)
            {
                _postData += UrlEncode(key) + "=" + UrlEncode(postData[key]) + "&";
            }
            //if (!string.IsNullOrEmpty(_postData)) _postData = _postData.Substring(0, _postData.Length - 1);
            return GetHttpWebResponse(url, _postData);
        }
        /// <summary>
        /// http POST请求url
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="method_name"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static string GetHttpWebResponse(string url, string postData)
        {
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(postData);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.Referer = url;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\n";
            request.Headers["Accept-Language"] = "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3\r\n";
            request.Headers["Accept-Encoding"] = " gzip, deflate\r\n";
            request.Headers.Add("content", "text/html; charset=UTF-8");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:34.0) Gecko/20100101 Firefox/34.0\r\n";
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.ContentLength = payload.Length;
            request.Timeout = 20000;
            HttpWebResponse response = null;
            try
            {
                StreamWriter swRequestWriter = new StreamWriter(request.GetRequestStream());
                swRequestWriter.Write(postData);
                if (swRequestWriter != null)
                    swRequestWriter.Close();
                response = (HttpWebResponse)request.GetResponse();
                string s = response.CharacterSet;
                using (StreamReader reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress), Encoding.GetEncoding("GB2312")))
                {
                    return reader.ReadToEnd();
                }
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }
        #endregion
        #region 字段串是否为Null或为""(空)
        /// <summary>
        /// 字段串是否为Null或为""(空)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool StrIsNullOrEmpty(string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return true;
            return false;
        }
        #endregion
        #region 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }
        #endregion
        #region base64编码加密
        public static string base64encode(string str)
        { //编码
            string base64EncodeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";//编码后的字符集
            string Out = "";
            int i = 0, len = str.Length;
            char c1, c2, c3;
            while (i < len)
            {
                c1 = Convert.ToChar(str[i++] & 0xff);
                if (i == len)
                {
                    Out += base64EncodeChars[c1 >> 2];
                    Out += base64EncodeChars[(c1 & 0x3) << 4];
                    Out += "==";
                    break;
                }
                c2 = str[i++];
                if (i == len)
                {
                    Out += base64EncodeChars[c1 >> 2];
                    Out += base64EncodeChars[((c1 & 0x3) << 4) | ((c2 & 0xF0) >> 4)];
                    Out += base64EncodeChars[(c2 & 0xF) << 2];
                    Out += "=";
                    break;
                }
                c3 = str[i++];
                Out += base64EncodeChars[c1 >> 2];
                Out += base64EncodeChars[((c1 & 0x3) << 4) | ((c2 & 0xF0) >> 4)];
                Out += base64EncodeChars[((c2 & 0xF) << 2) | ((c3 & 0xC0) >> 6)];
                Out += base64EncodeChars[c3 & 0x3F];
            }
            return Out;
        }
        //转换成utf—8编码
        public static string utf16to8(string str)
        {
            string Out = "";
            int i, len;
            char c;//char为16位Unicode字符,范围0~0xffff
            len = str.Length;
            for (i = 0; i < len; i++)
            {//根据字符的不同范围分别转化
                c = str[i];
                if ((c >= 0x0001) && (c <= 0x007F))
                {
                    Out += str[i];
                }
                else if (c > 0x07FF)
                {
                    Out += (char)(0xE0 | ((c >> 12) & 0x0F));
                    Out += (char)(0x80 | ((c >> 6) & 0x3F));
                    Out += (char)(0x80 | ((c >> 0) & 0x3F));
                }
                else
                {
                    Out += (char)(0xC0 | ((c >> 6) & 0x1F));
                    Out += (char)(0x80 | ((c >> 0) & 0x3F));
                }
            }
            return Out;
        }
        /// <summary>
        /// base64编码加密
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <returns></returns>
        public static string MyEncode(string str)
        {
            return base64encode(utf16to8(str));
        }
        #endregion
        #region 取指定长度的字符串
        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string myResult = p_SrcString;
            byte[] bComments = Encoding.UTF8.GetBytes(p_SrcString);
            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);
                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;
                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾
                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }
                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;
                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                                nFlag = 1;
                        }
                        else
                            nFlag = 0;
                        anResultFlag[i] = nFlag;
                    }
                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        nRealLength = p_Length + 1;
                    bsResult = new byte[nRealLength];
                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);
                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + p_TailString;
                }
            }
            return myResult;
        }
        #endregion
        #region 获取web.configs的appsettings的值
        /// <summary>
        /// 获取web.configs的appsettings的值
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string GetAppSettings(string keys)
        {
            if (string.IsNullOrEmpty(keys)) return string.Empty;
            return ConfigurationManager.AppSettings[keys];
        }
        #endregion
        #region 操作权限菜单
        /// <summary>
        /// 获取操作权限
        /// </summary>
        /// <returns>Dictionary</returns>
        public static Dictionary<string, string> ActionType()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Show", "显示");
            dic.Add("View", "查看");
            dic.Add("Add", "添加");
            dic.Add("Edit", "修改");
            dic.Add("Delete", "删除");
            dic.Add("Audit", "审核");
            dic.Add("Reply", "回复");
            dic.Add("Confirm", "确认");
            dic.Add("Cancel", "取消");
            dic.Add("Invalid", "作废");
            dic.Add("Build", "生成");
            dic.Add("Instal", "安装");
            dic.Add("Unload", "卸载");
            dic.Add("Back", "备份");
            dic.Add("Restore", "还原");
            dic.Add("Replace", "替换");
            return dic;
        }
        #endregion
        #region 获取UTC时间
        public static DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind)
        {
            dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
            return TimeZoneInfo.ConvertTimeToUtc(dt);
        }
        #endregion
        #region 时间函数
        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        public static DateTime ConvertToUserTime(DateTime dt)
        {
            return ConvertToUserTime(dt, dt.Kind);
        }
        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <param name="sourceDateTimeKind">The source datetimekind</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        public static DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind)
        {
            dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
            return TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Utc);
        }
        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        public static DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone)
        {
            var currentUserTimeZoneInfo = TimeZoneInfo.Utc;
            return ConvertToUserTime(dt, sourceTimeZone, currentUserTimeZoneInfo);
        }
        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <param name="destinationTimeZone">The time zone to convert dateTime to.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        public static DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
        {
            return TimeZoneInfo.ConvertTime(dt, sourceTimeZone, destinationTimeZone);
        }
        #endregion
        #region 用*号替换掉邮箱部分字符
        public static string ReplaceEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return "****";
            int leng = email.Length;
            if (leng <= 4) return "****";
            int start1 = email.IndexOf('@');
            string m1 = email.Substring(0, start1);
            string m2 = email.Substring(start1 + 1);
            m2 = m2.Substring(0, m2.IndexOf('.'));
            if (m1.Length > 4)
            { email = email.Replace(email.Substring(start1 - 3, 3), "***"); }
            else
            {
                email = email.Replace(email.Substring(start1 - 2, 2), "**");
            }
            if (m2.Length > 4)
            {
                email = email.Replace(email.Substring(start1 + 1, 3), "***");
            }
            else
            {
                email = email.Replace(email.Substring(start1 + 1, 2), "**");
            }
            return email;
        }
        #endregion
        #region 计算经纬度之间的距离
        private static double EARTH_RADIUS = 6371.0;//km 地球半径 平均值，千米
        /// <summary>
        /// 给定的经度1，纬度1；经度2，纬度2. 计算2个经纬度之间的距离。
        /// </summary>
        /// <param name="lat1">经度1</param>
        /// <param name="lon1">纬度1</param>
        /// <param name="lat2">经度2</param>
        /// <param name="lon2">纬度2</param>
        /// <returns>距离（公里、千米）</returns>
        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            //用haversine公式计算球面两点间的距离。
            //经纬度转换成弧度
            lat1 = ConvertDegreesToRadians(lat1);
            lon1 = ConvertDegreesToRadians(lon1);
            lat2 = ConvertDegreesToRadians(lat2);
            lon2 = ConvertDegreesToRadians(lon2);
            //差值
            var vLon = Math.Abs(lon1 - lon2);
            var vLat = Math.Abs(lat1 - lat2);
            //h is the great circle distance in radians, great circle就是一个球体上的切面，它的圆心即是球心的一个周长最大的圆。
            var h = HaverSin(vLat) + Math.Cos(lat1) * Math.Cos(lat2) * HaverSin(vLon);
            var distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));
            return distance;
        }
        /// <summary>
        /// 将角度换算为弧度。
        /// </summary>
        /// <param name="degrees">角度</param>
        /// <returns>弧度</returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
        /// <summary>
        /// 将弧度换算成角度
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double ConvertRadiansToDegrees(double radian)
        {
            return radian * 180.0 / Math.PI;
        }
        /// <summary>
        /// 半正矢换算
        /// </summary>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static double HaverSin(double theta)
        {
            var v = Math.Sin(theta / 2);
            return v * v;
        }
        #endregion
        #region 将颜色额转换成由16进制RGB分量表示的字符串。
        /// <summary>
        /// 将颜色额转换成由16进制RGB分量表示的字符串。
        /// 例如：红色-#FF0000
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns>返回颜色字符串</returns>
        public static string GetColorString(Color color)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
        }
        #endregion
    }
}
