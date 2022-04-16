using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class ThreeZipAreaConfig : EntityTypeConfiguration<ThreeZipArea>
    {
        public ThreeZipAreaConfig()
        {
            HasKey(i => i.Id).ToTable("threezipareas_all");
        }
    }
}
