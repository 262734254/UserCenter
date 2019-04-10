using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Dto.UserCenter
{
    /// <summary>
    /// 身份验证信息 模拟JWT的payload
    /// </summary>
    public class AuthInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
        ///<summary>
        ///是否为APP
        ///</summary>
        public bool IsApp { set; get; }

        ///<summary>
        ///登陆时间
        ///</summary>
        public DateTime  LoginTime { set; get; }

        /// <summary>
        /// 口令过期时间
        /// </summary>
        public DateTime ExpiryDateTime { get; set; }
    }
}
