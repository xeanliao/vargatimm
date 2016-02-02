using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class FiveZipCoordinateConfig : EntityTypeConfiguration<FiveZipCoordinate>
    {
        public FiveZipCoordinateConfig()
        {
            HasKey(i => i.Id).ToTable("fivezipareacoordinates");
        }
    }
}
