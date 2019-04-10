using JNKJ.Domain.UserCenter;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Mapping.UserCenter
{
    class RolesRightsMap : EntityTypeConfiguration<RolesRights>
    {
        public RolesRightsMap()
        {
            ToTable("RolesRights");
            HasKey(cr => cr.Id);
        }
    }
}
