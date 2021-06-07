using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    class CampaignRecordConfig : EntityTypeConfiguration<CampaignRecord>
    {
        public CampaignRecordConfig()
        {
            HasKey(i => i.Id).ToTable("campaignrecords");
        }
    }
}