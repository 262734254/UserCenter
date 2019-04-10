using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Domain.UserCenter
{
    ///<summary>
    ///系统菜单下的操作按钮表
    ///</summary>
    public class Buttons : BaseEntity
    {
        ///<summary>
        ///菜单ID
        ///</summary>
        public Guid? ProjectMenuID { set; get; }
        ///<summary>
        ///按钮URL
        ///</summary>
        public string ButtonUrl { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string ButtonKey { set; get; }
        ///<summary>
        ///按钮文本
        ///</summary>
        public string ButtonName { set; get; }
        ///<summary>
        ///序号
        ///</summary>
        public int DisplayNo { set; get; }
        ///<summary>
        ///创建时间
        ///</summary>
        public DateTime CreatedTime { set; get; }
    }
}
