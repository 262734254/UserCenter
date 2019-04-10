using System.Data.Entity.ModelConfiguration;
using JNKJ.Domain.UserCenter;
namespace JNKJ.Mapping.UserCenter
{
   public class SubContractorMap : EntityTypeConfiguration<SubContractor>
    {
        public SubContractorMap()
        {
            ToTable("SubContractor");
            HasKey(cr => cr.Id);
        }
    }
}
