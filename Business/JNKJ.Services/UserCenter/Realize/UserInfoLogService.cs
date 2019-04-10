using JNKJ.Core.Data;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Results;
using JNKJ.Services.UserCenter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Services.UserCenter.Realize
{
    public class UserInfoLogService : IUserInfoLog
    {
        private readonly IRepository<UserInfoLog> _userInfoLogRepository;
        public UserInfoLogService(IRepository<UserInfoLog> userInfoLogRepository)
        {
            _userInfoLogRepository = userInfoLogRepository;
        }
    }
}
