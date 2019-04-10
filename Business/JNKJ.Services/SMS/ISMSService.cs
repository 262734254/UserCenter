using System;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
namespace JNKJ.Services.SMS
{
    /// <summary>
    /// 阿里云短信接口
    ///  author :刘少峰
    ///  date   :2018.5
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// 通过阿里云短信发送API发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        SendSmsResponse SendSms(string phone, string content, SmsTemplateState template);
        /// <summary>
        /// 短信发送记录查询接口
        /// 用于查询短信发送的状态，是否成功到达终端用户手机
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="bizId">发送流水号,从调用发送接口返回值中获取</param>
        /// <param name="date">短信发送日期格式yyyyMMdd,支持最近30天记录查询</param>
        /// <param name="currentPage">当前页码,默认为第1页</param>
        /// <returns></returns>
        QuerySendDetailsResponse QuerySendDetails(string phone, string bizId, DateTime date, int currentPage);
    }
}
