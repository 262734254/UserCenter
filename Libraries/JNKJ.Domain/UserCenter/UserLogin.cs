using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Domain.UserCenter
{
    public class UserLogin:BaseEntity
    {
        ///<summary>
        ///主键Id
        ///</summary>
        public Guid? ID { set; get; }
        ///<summary>
        ///手机号码
        ///</summary>
        public string PhoneNo { set; get; }
        ///<summary>
        ///验证码
        ///</summary>
        public string ValidationCode { set; get; }
        ///<summary>
        ///注册时间
        ///</summary>
        public DateTime RegisterTime { set; get; }
        ///<summary>
        ///验证码过期分钟数
        ///</summary>
        public int ValidationCodeExpiredMinutes { set; get; }
        ///<summary>
        ///验证码过期的截止时间
        ///</summary>
        public DateTime ValidationCodeExpiredEndTime { set; get; }
    }
}
