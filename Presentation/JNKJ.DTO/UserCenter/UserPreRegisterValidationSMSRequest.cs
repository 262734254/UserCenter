using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcyg.Domain.Dto.Customers
{
    /// <summary>
    /// 员工注册或登录的预登记表的验证短信的请求
    /// </summary>
    public class UserPreRegisterValidationSMSRequest
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNo { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string ValidationCode { get; set; }
    }
}
