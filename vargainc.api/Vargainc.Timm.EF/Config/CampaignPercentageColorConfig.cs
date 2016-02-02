using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class CampaignPercentageColorConfig : EntityTypeConfiguration<CampaignPercentageColor>
    {
        public CampaignPercentageColorConfig()
        {
            HasKey(i => i.Id).ToTable("campaignpercentagecolors");
        }
    }
}
