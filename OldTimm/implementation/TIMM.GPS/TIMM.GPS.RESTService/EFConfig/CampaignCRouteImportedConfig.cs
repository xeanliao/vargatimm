using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    class CampaignCRouteImportedConfig : EntityTypeConfiguration<CampaignCRouteImported>
    {
        public CampaignCRouteImportedConfig()
        {
            ToTable("campaigncrouteimported");
            HasKey(i => i.Id);
            HasRequired(i => i.PremiumCRoute)
            .WithMany()
            .HasForeignKey(i => i.PremiumCRouteId);
            Ignore(i => i.IsPartModified);
        }
    }
}
