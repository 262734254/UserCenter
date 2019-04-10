using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Domain.UserCenter
{
    ///<summary>
    ///组织架构表
    ///</summary>
    public class DeptInfo : BaseEntity
    {
        ///<summary>
        ///企业表Id
        ///</summary>
        public Guid? EnterpriseID { set; get; }
        ///<summary>
        ///上级部门Id。null为无
        ///</summary>
        public Guid? ParentID { set; get; }
        ///<summary>
        ///部门名字
        ///</summary>
        public string Name { set; get; }

        /// <summary>
        /// 对应层级，最多5级，第一级值为0
        /// </summary>
        public int Sort { set; get; }

        ///<summary>
        ///创建人，对应用户表Id
        ///</summary>
        public Guid? CreateUserId { set; get; }

        ///<summary>
        ///创建时间
        ///</summary>
        public DateTime CreateTime { set; get; }
    }
}
