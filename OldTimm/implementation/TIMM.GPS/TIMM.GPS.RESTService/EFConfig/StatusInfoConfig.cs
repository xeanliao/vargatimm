using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    public class StatusInfoConfig : EntityTypeConfiguration<StatusInfo>
    {
        public StatusInfoConfig()
        {
            HasKey(i => i.Id);
            ToTable("campaignusermappings");

            HasRequired(i => i.Campaign).WithMany().HasForeignKey(i => i.CampaignId);
            HasRequired(i => i.User).WithMany().HasForeignKey(i => i.UserId);
        }
    }
}
