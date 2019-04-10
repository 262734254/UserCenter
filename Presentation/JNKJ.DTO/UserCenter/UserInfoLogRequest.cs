using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Dto.UserCenter
{
    public class UserInfoLogRequest
    {
        public Guid? Id { get; set; }
        public Guid? UserInfo_EnterpriseInfoID { get; set; }
        public Guid? OldUserInfo_Id { get; set; }
        public Guid? OleRelationship_Id { get; set; }
        public int? AccountState { get; set; }
        public string Remark { get; set; }
        public int? Sort { get; set; }
        public string PostalAddress { get; set; }
        public string DetailAddress { get; set; }
        public string NickName { get; set; }
        public string ChsName { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public Guid? UserInfo_RolesID { get; set; }
        public Guid? Relationship_RolesID { get; set; }
        public Guid? Relationship_EnterpriseID { get; set; }
        public int? State { get; set; }
        public Guid? DeptInfoId { get; set; }

    }
}
