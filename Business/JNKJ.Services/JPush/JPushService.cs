using System.Collections.Generic;
using Beyova.JPush;
using Beyova.JPush.V3;
namespace JNKJ.Services.JPush
{
    public class JPushService : IJPushService
    {
        #region const
        private const string AppKey = "7ce73366169c8f89a8e35200";
        private const string MasterSecret = "fa3869911f29ebad0aaf65ba";
        #endregion
        /// <summary>
        /// 极光推送
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PushResponse SendPush(PushRequest request)
        {
            var client = new JPushClientV3(AppKey, MasterSecret);
            var audience = new Audience();
            if (request.type == 0)
            {
                audience.Add(PushTypeV3.Broadcast, null);
            }
            else
            {
                audience.Add(PushTypeV3.ByTagWithinAnd, new List<string>(new[] { request.userId, request.userId }));
            }
            var customizedValues = new Dictionary<string, object>
            {
                {"JPushValue", request.value}
            };
            var notification = new Notification
            {
                AndroidNotification = new AndroidNotificationParameters
                {
                    Title = request.title,
                    Alert = request.content,
                    CustomizedValues = customizedValues
                }
            };
            var response = client.SendPushMessage(new PushMessageRequestV3
            {
                Audience = audience,
                Platform = PushPlatform.AndroidAndiOS,
                Notification = notification,
                IsTestEnvironment = true,
                AppMessage = new AppMessage
                {
                    Content = request.value,
                    CustomizedValue = customizedValues
                },
            });
            return response;
        }
    }
}
