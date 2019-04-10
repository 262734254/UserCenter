using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Domain.UserCenter
{
    ///<summary>
    ///用户认证表
    ///</summary>
    public class UserAuthentication : BaseEntity
    {
        ///<summary>
        ///用户表Id
        ///</summary>
        public Guid UserId { set; get; }

        ///<summary>
        ///到期时间
        ///</summary>
        public DateTime ExpiryDateTime { set; get; }
        ///<summary>
        ///密钥
        ///</summary>
        public string SecretKey { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string Token { set; get; }
        ///<summary>
        ///创建时间
        ///</summary>
        public DateTime CreatTime { set; get; }
        ///<summary>
        ///是否为APP
        ///</summary>
        public bool IsApp { set; get; }
        ///<summary>
        ///更新时间，每次调用接口时，判断当前时间和更新时间的时间间隔是否大于3小时并且小于过期时间一小时
        ///</summary>
        public DateTime UpdateTime { set; get; }
    }
}
