using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class DistributionMapRecordConfig : EntityTypeConfiguration<DistributionMapRecord>
    {
        public DistributionMapRecordConfig()
        {
            HasKey(i => i.Id).ToTable("distributionmaprecords");
        }
    }
}
