using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class DistributionMapCoordinateConfig : EntityTypeConfiguration<DistributionMapCoordinate>
    {
        public DistributionMapCoordinateConfig()
        {
            HasKey(i => i.Id).ToTable("distributionmapcoordinates");
        }
    }
}
