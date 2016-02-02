using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class CampaignClassificationConfig : EntityTypeConfiguration<CampaignClassification>
    {
        public CampaignClassificationConfig()
        {
            HasKey(i => i.Id).ToTable("campaignclassifications");
        }
    }
}