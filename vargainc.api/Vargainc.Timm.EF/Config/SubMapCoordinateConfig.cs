using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class SubMapCoordinateConfig : EntityTypeConfiguration<SubMapCoordinate>
    {
        public SubMapCoordinateConfig()
        {
            HasKey(i => i.Id).ToTable("submapcoordinates");
        }
    }
}
