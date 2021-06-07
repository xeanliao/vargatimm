using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class RadiusesConfig : EntityTypeConfiguration<Radiuses>
    {
        public RadiusesConfig()
        {
            HasKey(i => i.Id).ToTable("radiuses");
        }
    }
}
