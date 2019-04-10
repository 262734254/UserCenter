using Beyova.JPush;
namespace JNKJ.Services.JPush
{
    /// <summary>
    /// 极光推送接口
    /// </summary>
    public interface IJPushService
    {
        /// <summary>
        /// 极光推送
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PushResponse SendPush(PushRequest request);
    }
}
