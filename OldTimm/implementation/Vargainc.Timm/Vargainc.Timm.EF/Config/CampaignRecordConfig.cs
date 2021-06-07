using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class CampaignRecordConfig : EntityTypeConfiguration<CampaignRecord>
    {
        public CampaignRecordConfig()
        {
            HasKey(i => i.Id).ToTable("campaignrecords");
        }
    }
}