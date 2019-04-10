using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Domain.UserCenter
{
    ///<summary>
	///项目拥有的角色对应的权限下面的操作按钮
	///</summary>
	public class RolesRightsButtons : BaseEntity
    {
        ///<summary>
        ///平台级的功能菜单ID。引用平台级的功能菜单表。
        ///</summary>
        public Guid ProjectRolesRightsID { set; get; }
        ///<summary>
        ///企业表ID。冗余字段，便于后期查询。
        ///</summary>
        public Guid ProjectButtonsID { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string ButtonName { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string ButtonKey { set; get; }
        ///<summary>
        ///项目表ID。冗余字段，便于后期查询。
        ///</summary>
        public Int32 DisplayNo { set; get; }
        ///<summary>
        ///创建时间
        ///</summary>
        public DateTime CreatedTime { set; get; }
    }
}
