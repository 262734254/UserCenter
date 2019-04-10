using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
namespace JNKJ.Data
{
    /// <summary>
    /// JSON 帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 序列化成 JSON（默认的日期格式：yyyy-MM-dd HH:mm:ss）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SerializeToJson(object data)
        {
            return SerializeToJson(data, "yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 序列化成 JSON，传指定的日期格式。（默认的日期格式：yyyy-MM-dd HH:mm:ss）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dateTimeFormats"></param>
        /// <returns></returns>
        public static string SerializeToJson(object data, string dateTimeFormats)
        {
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = dateTimeFormats };
            return JsonConvert.SerializeObject(data, Formatting.None, timeConverter);
        }
        /// <summary>
        /// 把json字符串转成实体对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="data">json字符串</param> 
        public static T DeserializeToModel<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        /// 将json字符串转换成list &lt;T&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<T> DeserializeToList<T>(string data)
        {
            return JsonConvert.DeserializeObject<List<T>>(data);
        }
    }
}
