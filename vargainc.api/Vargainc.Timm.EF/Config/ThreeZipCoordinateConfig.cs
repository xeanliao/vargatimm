using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class ThreeZipCoordinateConfig : EntityTypeConfiguration<ThreeZipCoordinate>
    {
        public ThreeZipCoordinateConfig()
        {
            HasKey(i => i.Id).ToTable("threezipareacoordinates");
        }
    }
}
