using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Domain.UserCenter
{
    ///<summary>
	///项目拥有的角色表
	///</summary>
	public class Roles : BaseEntity
    {
        ///<summary>
        ///角色名称
        ///</summary>
        public string RoleName { set; get; }
        ///<summary>
        ///创建时间
        ///</summary>
        public DateTime CreatedTime { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string Remark { set; get; }
    }
}
