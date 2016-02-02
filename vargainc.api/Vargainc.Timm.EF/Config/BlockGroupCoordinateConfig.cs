using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class BlockGroupCoordinateConfig : EntityTypeConfiguration<BlockGroupCoordinate>
    {
        public BlockGroupCoordinateConfig()
        {
            HasKey(i => i.Id).ToTable("blockgroupcoordinates");
        }
    }
}
