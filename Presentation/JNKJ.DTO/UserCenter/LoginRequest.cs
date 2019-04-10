using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Dto.UserCenter
{
    /// <summary>
    /// 登录用户信息
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 用户名，即手机号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string VerificationCode { get; set; }

        ///<summary>
        ///是否为APP
        ///</summary>
        public bool IsApp { set; get; }
    }

    /// <summary>
    /// 创建Token请求类
    /// </summary>
    public class GenerateTokenRequest
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        ///<summary>
        ///是否为APP
        ///</summary>
        public bool IsApp { set; get; }
    }
}
