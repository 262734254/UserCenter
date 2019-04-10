using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcyg.Domain.Dto.Customers.Enums
{
    /// <summary>
    /// 企业状态：0-未审核，1-审核通过，2-审核未通过
    /// </summary>
    public enum EnterpriseState
    {
        /// <summary>
        /// 新增待审核企业
        /// </summary>
        New = 0,
        /// <summary>
        /// 正常 ---- 审核通过
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 审核未通过
        /// </summary>
        Unapprove = 2,
        /// <summary>
        /// 审核中
        /// </summary>
        Auditing = 30,
        /// <summary>
        /// 被锁企业
        /// </summary>
        Lock = 40,
        /// <summary>
        /// 已经离场企业
        /// </summary>
        Invalid = 50,
        /// <summary>
        /// 进入黑名单企业
        /// </summary>
        Blacklist = 60,
        /// <summary>
        /// 修改待审核企业
        /// </summary>
        Modity = 70,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 90
    }
}
