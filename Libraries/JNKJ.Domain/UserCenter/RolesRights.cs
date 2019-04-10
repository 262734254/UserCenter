using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Domain.UserCenter
{
    ///<summary>
    ///项目拥有的角色对应的权限表
    ///</summary>
    public class RolesRights : BaseEntity
    {
        ///<summary>
        ///项目拥有的角色ID，引用项目拥有的角色表
        ///</summary>
        public Guid? ProjectRolesID { set; get; }
        ///<summary>
        ///平台级的功能菜单ID。引用平台级的功能菜单表。
        ///</summary>
        public Guid? ProjectMenuID { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string FunctionKey { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string FunctionName { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string ImgUrl { set; get; }
        ///<summary>
        ///
        ///</summary>
        public Guid? ProjectMenuParendID { set; get; }
        ///<summary>
        ///序号
        ///</summary>
        public int DisplayNo { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string Icon { set; get; }
        ///<summary>
        ///
        ///</summary>
        public int? FunctionType { set; get; }
        ///<summary>
        ///创建时间
        ///</summary>
        public DateTime? CreatedTime { set; get; }
        ///<summary>
        ///是否为默认菜单（ 0=不是默认，1=默认）
        ///</summary>
        public int IsDefault { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string Remark { set; get; }
    }
}
