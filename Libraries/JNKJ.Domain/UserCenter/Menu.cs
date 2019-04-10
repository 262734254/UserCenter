using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Domain.UserCenter
{
    ///<summary>
    ///菜单表
    ///</summary>
    public class Menu : BaseEntity
    {
        ///<summary>
        ///菜单Key
        ///</summary>
        public string FunctionKey { set; get; }
        ///<summary>
        ///菜单名称
        ///</summary>
        public string FunctionName { set; get; }
        ///<summary>
        ///菜单URL
        ///</summary>
        public string FunctionUrl { set; get; }
        ///<summary>
        ///菜单类型。1=系统，2=PC，3=移动
        ///</summary>
        public int? FunctionType { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string Icon { set; get; }
        ///<summary>
        ///
        ///</summary>
        public int DisplayNo { set; get; }
        ///<summary>
        ///直接父级ID。没有父级则为 NULL。
        ///</summary>
        public Guid? ParentID { set; get; }
        ///<summary>
        ///创建时间
        ///</summary>
        public DateTime? CreatedTime { set; get; }
    }
}
