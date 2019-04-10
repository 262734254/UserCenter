using System;
using System.ComponentModel;
using System.Configuration;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using JNKJ.Core.Infrastructure;
using JNKJ.Services.UserCenter.Interface;

namespace JNKJ.Services.SMS
{
    public class SmsService : ISmsService
    {
        public const string Product = "Dysmsapi";//短信API产品名称
        public const string Domain = "dysmsapi.aliyuncs.com";//短信API产品域名
        public const string AccessKeyId = "LTAIVzACurls0m7H";
        public const string AccessKeySecret = "iwDV4vv3wIdjUn6Xw2TDRiAH8ilel3";
        public static readonly string IsSms = ConfigurationSettings.AppSettings["IsUsingSMS"];
        /// <summary>
        /// 短信发送记录查询接口
        /// 用于查询短信发送的状态，是否成功到达终端用户手机
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="bizId">发送流水号,从调用发送接口返回值中获取</param>
        /// <param name="date">短信发送日期格式yyyyMMdd,支持最近30天记录查询</param>
        /// <param name="currentPage">当前页码</param>
        /// <returns></returns>
        public QuerySendDetailsResponse QuerySendDetails(string phone, string bizId, DateTime date, int currentPage = 1)
        {
            //初始化acsClient,暂不支持region化
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", AccessKeyId, AccessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", Product, Domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            //组装请求对象
            var request = new QuerySendDetailsRequest();
            //必填-号码
            request.PhoneNumber = phone;
            //可选-流水号
            request.BizId = bizId;
            //必填-发送日期 支持30天内记录查询，格式yyyyMMdd       
            request.SendDate = date.ToString("yyyyMMdd");
            //必填-页大小
            request.PageSize = 50;
            //必填-当前页码从1开始计数
            request.CurrentPage = currentPage;
            QuerySendDetailsResponse querySendDetailsResponse = null;
            try
            {
                querySendDetailsResponse = acsClient.GetAcsResponse(request);
            }
            catch (ServerException)
            {
            }
            catch (ClientException)
            {
            }
            return querySendDetailsResponse;
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public SendSmsResponse SendSms(string phone, string content, SmsTemplateState template)
        {
            var templateParam = string.Empty;
            var verificationCode = string.Empty;
            switch (template)
            {
                case SmsTemplateState.Login:
                    verificationCode = new Random().Next(1000, 10000).ToString();
                    templateParam = "{\"code\":\"" + verificationCode + "\"}";
                    break;
                case SmsTemplateState.Inform:
                    templateParam = "{\"code\":\"" + content + "\"}";
                    //templateParam = "{\"code\":\"" + content + "\",\"address\":\"" + Address + "\"}";
                    break;
            }
            //IsSms = 1 表示启用短信验证
            SendSmsResponse result;
            if (IsSms == "1")
            {

                result = SendValMsg(phone, GetDescription(template), templateParam);
                result.Code = verificationCode;
            }
            else
            {
                result = new SendSmsResponse() { Code = "8888" };
            }
            return result;
        }
        /// <summary>
        /// 获取枚举的描述  
        /// </summary>  
        /// <param name="en">枚举</param>  
        /// <returns>返回枚举的描述</returns>  
        public static string GetDescription(Enum en)
        {
            var type = en.GetType();//获取类型  
            var memberInfos = type.GetMember(en.ToString());//获取成员  
            if (memberInfos.Length <= 0) return en.ToString();
            var attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];//获取描述特性  
            if (attrs != null && attrs.Length > 0)
            {
                return attrs[0].Description; //返回当前描述  
            }
            return en.ToString();
        }
        /// <summary>
        /// 通过阿里云短信发送API发送验证码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="templateCode"></param>
        /// <param name="templateParam"></param>
        /// <returns></returns>
        protected virtual SendSmsResponse SendValMsg(string phone, string templateCode, string templateParam)
        {
            //你的accessKeyId和accessKeySecret参考本文档步骤2
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", AccessKeyId, AccessKeySecret);
            //IAcsClient client = new DefaultAcsClient(profile);
            // SingleSendSmsRequest request = new SingleSendSmsRequest();
            //初始化ascClient,暂时不支持多region（请勿修改）
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", Product, Domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            var request = new SendSmsRequest();
            try
            {
                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = phone;
                //必填:短信签名
                request.SignName = "工程易管";
                //必填:短信模板
                request.TemplateCode = templateCode;
                request.TemplateParam = templateParam;
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                request.OutId = "";
                //请求失败这里会抛ClientException异常
                var sendSmsResponse = acsClient.GetAcsResponse(request);
                return sendSmsResponse;
            }
            catch (ServerException e)
            {
                return new SendSmsResponse() { Code = "isv.Error", Message = e.Message };
            }
            catch (ClientException e)
            {
                return new SendSmsResponse() { Code = "isv.Error", Message = e.Message };
            }
        }
    }
}
