using System;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using JNKJ.Dto.Results;
using JNKJ.Dto.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace JNKJ.WebAPI.Areas
{
    public class BaseController : ApiController
    {
        /// <summary>
        /// 序列化成 JSON（默认的日期格式：yyyy-MM-dd HH:mm:ss）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="DateTimeFormats"></param>
        /// <returns></returns>
        public static string SerializeToJson(object data)
        {
            return SerializeToJson(data, "yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 序列化成 JSON，传指定的日期格式。（默认的日期格式：yyyy-MM-dd HH:mm:ss）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="DateTimeFormats"></param>
        /// <returns></returns>
        public static string SerializeToJson(object data, string dateTimeFormats)
        {
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = dateTimeFormats };
            return JsonConvert.SerializeObject(data, Formatting.None, timeConverter);
        }
        /// <summary>
        /// 转换成 JSON 作为响应的 Body 发送给客户端
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [NonAction]
        private HttpResponseMessage toJsonbase(Object obj)
        {
            string str;
            if (obj is string || obj is Char)
            {
                str = obj.ToString();
            }
            else
            {
                str = SerializeToJson(obj);
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        /// <summary>
        /// 转换成分页 JSON 作为响应的 Body 发送给客户端
        /// 拓展方法 -----  避免返回两次State
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NonAction]
        protected HttpResponseMessage toJson(JsonResponse response)
        {
            return toJsonbase(response);
        }

        /// <summary>
        /// 转换成分页 JSON 作为响应的 Body 发送给客户端
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NonAction]
        protected HttpResponseMessage toJson(object data)
        {
            var response = new JsonResponse(OperatingState.Success, "调用成功", data);
            return toJsonbase(response);
        }
        /// <summary>
        /// 转换成分页 JSON 作为响应的 Body 发送给客户端
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NonAction]
        protected HttpResponseMessage toJson(object data, string message)
        {
            var response = new JsonResponse(OperatingState.Success, message, data);
            return toJsonbase(response);
        }
        /// <summary>
        /// 转换成分页 JSON 作为响应的 Body 发送给客户端
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NonAction]
        protected HttpResponseMessage toJson(object data, OperatingState opstate, string message)
        {
            var response = new JsonResponse(opstate, message, data);
            return toJsonbase(response);
        }
        /// <summary>
        /// 转换成分页 JSON 作为响应的 Body 发送给客户端
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NonAction]
        protected HttpResponseMessage toListJson<T>(PageList<T> list)
        {
            JsonListResponse response = new JsonListResponse
            {
                Message = "",
                State = OperatingState.Success,
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalCount,
                TotalPages = list.TotalPages,
                DataModel = list.Data
            };
            return toJsonbase(response);
        }
        /// <summary>
        /// 转换成分页 JSON 作为响应的 Body 发送给客户端
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NonAction]
        protected HttpResponseMessage toListJson<T>(PageList<T> list, OperatingState opstate, string message)
        {
            JsonListResponse response = new JsonListResponse
            {
                Message = message,
                State = opstate,
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalCount,
                TotalPages = list.TotalPages,
                DataModel = list.Data
            };
            return toJsonbase(response);
        }
    }
}
