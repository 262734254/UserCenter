using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
namespace JNKJ.Core.Data
{
    public partial class DataSettingsManager
    {
        protected const char separator = ':';
        //默认的设置文件
        protected const string filename = "Settings.txt";
        /// <summary>
        /// 映射一个虚拟路径到物理路径
        /// </summary>
        /// <param name="path">一个路径的映射值. 比如. "~/bin"</param>
        /// <returns>一个物理路径值. 比如. "c:\inetpub\wwwroot\bin"</returns>
        protected virtual string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //宿主服务器
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                //不是服务器，比如，运行在单元测试下
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }
        protected virtual DataSettings ParseSettings(string text)
        {
            var shellSettings = new DataSettings();
            if (string.IsNullOrEmpty(text))
                return shellSettings;
            //第一个连接为主连接（写连接）
            //之后的连接为从连接（读连接）
            var settings = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                    settings.Add(str);
            }
            foreach (var setting in settings)
            {
                var separatorIndex = setting.IndexOf(separator);
                if (separatorIndex == -1)
                {
                    continue;
                }
                string key = setting.Substring(0, separatorIndex).Trim();
                string value = setting.Substring(separatorIndex + 1).Trim();
                switch (key)
                {
                    case "DataProvider":
                        shellSettings.DataProvider = value;
                        break;
                    case "DataConnectionString":
                        shellSettings.DataConnectionString = value;
                        break;
                    default:
                        shellSettings.RawDataSettings.Add(key, value);
                        break;
                }
            }
            return shellSettings;
        }
        protected virtual string ComposeSettings(DataSettings settings)
        {
            if (settings == null)
                return "";
            return string.Format("DataProvider: {0}{2}DataConnectionString: {1}{2}",
                                 settings.DataProvider,
                                 settings.DataConnectionString,
                                 Environment.NewLine
                );
        }
        /// <summary>
        /// 加载设置
        /// </summary>
        /// <param name="filePath">文件路径; 为空时使用默认设置文件路径</param>
        /// <returns></returns>
        public virtual DataSettings LoadSettings(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                //在单元测试中 使用 webHelper.MapPath 替代 HostingEnvironment.MapPath 
                filePath = Path.Combine(MapPath("~/App_Data/"), filename);
            }
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                return ParseSettings(text);
            }
            else
                return new DataSettings();
        }
        public virtual void SaveSettings(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            //在单元测试中 使用 webHelper.MapPath 替代 HostingEnvironment.MapPath 
            string filePath = Path.Combine(MapPath("~/App_Data/"), filename);
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath))
                {
                    //创建文件
                }
            }
            var text = ComposeSettings(settings);
            File.WriteAllText(filePath, text);
        }
    }
}
