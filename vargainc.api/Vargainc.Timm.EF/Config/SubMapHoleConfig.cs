using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    public class SubMapHoleConfig : EntityTypeConfiguration<SubMapHole>
    {
        public SubMapHoleConfig()
        {
            HasKey(i => i.Id).ToTable("submapholes");
        }
    }
}
