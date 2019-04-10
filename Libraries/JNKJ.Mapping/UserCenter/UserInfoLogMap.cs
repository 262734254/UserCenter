using JNKJ.Domain.UserCenter;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Mapping.UserCenter
{
    public class UserInfoLogMap : EntityTypeConfiguration<UserInfoLog>
    {
        public UserInfoLogMap()
        {
            ToTable("UserInfoLog");
            HasKey(cr => cr.Id);
        }
    }
}
