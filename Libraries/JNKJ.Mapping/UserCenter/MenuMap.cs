using JNKJ.Domain.UserCenter;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Mapping.UserCenter
{
    class MenuMap : EntityTypeConfiguration<Menu>
    {
        public MenuMap()
        {
            ToTable("Menu");
            HasKey(cr => cr.Id);
        }
    }
}
