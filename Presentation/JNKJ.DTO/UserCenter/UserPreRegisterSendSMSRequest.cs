using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Dto.UserCenter
{
    /// <summary>
    /// 登录的预登记表的发送短信的请求
    /// </summary>
    public class UserPreRegisterSendSMSRequest
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNo { get; set; }
    }
}
