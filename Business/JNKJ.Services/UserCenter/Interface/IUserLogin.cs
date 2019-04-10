using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Results;
using JNKJ.Dto.UserCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Services.UserCenter.Interface
{
    public interface IUserLogin
    {
        /// <summary>
        /// 数据进验证码表
        /// </summary>
        /// <param name="userInfoRequest"></param>
        /// <returns></returns>
        JsonResponse UserLogin(LoginRequest loginRequest);

        //bool IsBeOverdue(string phone);
    }
}
