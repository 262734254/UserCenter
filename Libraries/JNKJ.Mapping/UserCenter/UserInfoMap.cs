using JNKJ.Domain.UserCenter;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Mapping.UserCenter
{
    public class UserInfoMap : EntityTypeConfiguration<UserInfo>
    {
        public UserInfoMap()
        {
            ToTable("UserInfo");
            HasKey(cr => cr.Id);
        }
    }
}

