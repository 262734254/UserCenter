using JNKJ.Domain.UserCenter;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Mapping.UserCenter
{
    class DeptInfoMap : EntityTypeConfiguration<DeptInfo>
    {
        public DeptInfoMap()
        {
            ToTable("DeptInfo");
            HasKey(cr => cr.Id);
        }
    }
}
