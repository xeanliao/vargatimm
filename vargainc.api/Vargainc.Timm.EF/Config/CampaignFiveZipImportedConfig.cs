using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class CampaignFiveZipImportedConfig : EntityTypeConfiguration<CampaignFiveZipImported>
    {
        public CampaignFiveZipImportedConfig()
        {
            ToTable("campaignfivezipimported");
            HasKey(i => i.Id);
            HasRequired(i => i.FiveZipArea)
                .WithMany()
                .HasForeignKey(i => i.FiveZipAreaId);
            //Ignore(i => i.IsPartModified);
        }
    }
}
