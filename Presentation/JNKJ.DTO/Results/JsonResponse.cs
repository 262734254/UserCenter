using JNKJ.Dto.Enums;
using Newtonsoft.Json;
namespace JNKJ.Dto.Results
{
    /// <summary>
    /// 响应的基类
    /// </summary>
    public class JsonResponse : IResponseBase<OperatingState>
    {
        public JsonResponse()
        { }
        public JsonResponse(OperatingState state, string message)
        {
            this.State = state;
            this.Message = message;
        }
        public JsonResponse(OperatingState state, string message, object data)
        {
            this.State = state;
            this.Message = message;
            this.DataModel = data;
        }
        /// <summary>
        /// 操作状态
        /// </summary>
        [JsonProperty("State")]
        public OperatingState State { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        [JsonProperty("Message")]
        public string Message { get; set; }
        /// <summary>
        /// 核心数据
        /// </summary>
        [JsonProperty("DataModel")]
        public object DataModel { get; set; }
    }
}
