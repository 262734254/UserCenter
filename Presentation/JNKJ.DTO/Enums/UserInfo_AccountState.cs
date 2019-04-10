using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcyg.Domain.Enums
{
    /// <summary>
    /// 帐号状态
    /// </summary>
    public enum UserInfo_AccountState
    {
        None = -1,

        /// <summary>
        /// 待实名认证
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 已禁用
        /// </summary>
        Disabled = 1,

        /// <summary>
        /// 正常（已经实名认证）
        /// </summary>
        RealNameAuthenticationed = 2,

        /// <summary>
        /// 新增待审核
        /// </summary>
        New = 3,

        /// <summary>
        /// 已经离场
        /// </summary>
        Invalid = 4,

        /// <summary>
        /// 修改待审核企业
        /// </summary>
        Modity = 5,

        /// <summary>
        /// 实名认证信息审核中
        /// </summary>
        NameAuthenticationedApproving = 6,

        /// <summary>
        /// 删除
        /// </summary>
        Delete = 90
    }


    /// <summary>
    /// 用户类别
    /// </summary>
    public enum EnterpriseUserInfo_UserTypeEnum
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        Common = 0,

        /// <summary>
        /// 管理员用户
        /// </summary>
        Admin = 1,
    }
}
