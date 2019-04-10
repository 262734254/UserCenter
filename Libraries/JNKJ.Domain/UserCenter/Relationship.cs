using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Domain.UserCenter
{
    ///<summary>
	///用户、企业、角色关系管理表
	///</summary>
	public class Relationship : BaseEntity
    {
        ///<summary>
        ///用户表Id
        ///</summary>
        public Guid? UserId { set; get; }
        ///<summary>
        ///企业表Id
        ///</summary>
        public Guid? EnterpriseID { set; get; }
        /// <summary>
        /// 用户所属的组织架构Id，关联组织架构表
        /// </summary>
        public Guid? DeptInfoId { set; get; }
        ///<summary>
        ///角色表Id
        ///</summary>
        public Guid? RoleId { set; get; }
        ///<summary>
        ///0为在职。1为离职
        ///</summary>
        public int? State { set; get; }
        ///<summary>
        ///创建时间
        ///</summary>
        public DateTime CreateTime { set; get; }

        /// <summary>
        /// 是否为企业创建者
        /// </summary>
        public bool? IsEnterprise { get; set; }
    }
}
