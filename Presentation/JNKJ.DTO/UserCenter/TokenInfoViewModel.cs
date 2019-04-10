using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Dto.UserCenter
{
    public class TokenInfoViewModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
    }
}
