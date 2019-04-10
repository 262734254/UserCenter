namespace JNKJ.Services.JPush
{
    public class PushRequest
    {
        /// <summary>
        /// 推送目标
        /// 0=所有用户，1=目标用户
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
        public string value { get; set; }
    }
}
