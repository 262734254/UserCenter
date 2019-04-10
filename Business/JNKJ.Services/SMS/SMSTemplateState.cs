using System.ComponentModel;
namespace JNKJ.Services.SMS
{
    /// <summary>
    /// 短信模版
    /// </summary>
    public enum SmsTemplateState
    {
        /// <summary>
        /// 登录、注册模版
        /// </summary>
        [Description("SMS_135029621")]
        Login,
        /// <summary>
        /// 通知模版
        /// </summary>
        [Description("SMS_135044587")]
        Inform
    }
}
