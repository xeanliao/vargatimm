using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class CampaignTractImportedConfig : EntityTypeConfiguration<CampaignTractImported>
    {
        public CampaignTractImportedConfig()
        {
            ToTable("campaigntractimported");
            HasKey(i => i.Id);
            HasRequired(i => i.Tract)
                .WithMany()
                .HasForeignKey(i => i.TractId);
        }
    }
}
