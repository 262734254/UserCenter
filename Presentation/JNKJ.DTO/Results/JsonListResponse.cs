using System;
using JNKJ.Dto.Enums;
using Newtonsoft.Json;
namespace JNKJ.Dto.Results
{
    /// <summary>
    /// 响应的基类
    /// </summary>
    public class JsonListResponse : IResponseBase<OperatingState>
    {
        public JsonListResponse()
        { }
        public JsonListResponse(OperatingState state, string message)
        {
            this.State = state;
            this.Message = message;
        }
        public JsonListResponse(OperatingState state, string message, object data)
        {
            this.State = state;
            this.Message = message;
            this.DataModel = data;
        }
        /// <summary>
        /// 操作状态
        /// </summary>
        [JsonProperty("State")] //请不要更改这个 State，也不要改成小写的 state，目前安卓已经调试通过，大小写影响解析。
        public OperatingState State { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        [JsonProperty("Message")] //请不要更改这个 Message，也不要改成小写的 message，目前安卓已经调试通过，大小写影响解析。
        public string Message { get; set; }
        [JsonProperty("PageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("PageSize")]
        public int PageSize { get; set; }
        [JsonProperty("TotalCount")]
        public int TotalCount { get; set; }
        [JsonProperty("TotalPages")]
        public int TotalPages { get; set; }
        /// <summary>
        /// 核心数据
        /// </summary>
        [JsonProperty("DataModel")] //请不要更改这个 Data，也不要改成小写的 data，目前安卓已经调试通过，大小写影响解析。
        public object DataModel { get; set; }
    }
}
