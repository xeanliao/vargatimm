using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class CampaignBlockGroupImportedConfig : EntityTypeConfiguration<CampaignBlockGroupImported>
    {
        public CampaignBlockGroupImportedConfig()
        {
            ToTable("campaignblockgroupimported");
            HasKey(i => i.Id);
            HasRequired(i => i.BlockGroup)
                .WithMany()
                .HasForeignKey(i => i.BlockGroupId);
        }
    }
}
