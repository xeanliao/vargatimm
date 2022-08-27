using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class DistributionMapHoleConfig : EntityTypeConfiguration<DistributionMapHole>
    {
        public DistributionMapHoleConfig()
        {
            HasKey(i => i.Id).ToTable("distributionmapholes");
        }
    }
}
