using JNKJ.Domain.UserCenter;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Mapping.UserCenter
{
    class RolesRightsButtonsMap : EntityTypeConfiguration<RolesRightsButtons>
    {
        public RolesRightsButtonsMap()
        {
            ToTable("RolesRightsButtons");
            HasKey(cr => cr.Id);
        }
    }
}
