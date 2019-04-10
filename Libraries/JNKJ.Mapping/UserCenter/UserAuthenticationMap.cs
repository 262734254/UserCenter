using JNKJ.Domain.UserCenter;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Mapping.UserCenter
{
    class UserAuthenticationMap : EntityTypeConfiguration<UserAuthentication>
    {
        public UserAuthenticationMap()
        {
            ToTable("UserAuthentication");
            HasKey(cr => cr.Id);
        }
    }
}
