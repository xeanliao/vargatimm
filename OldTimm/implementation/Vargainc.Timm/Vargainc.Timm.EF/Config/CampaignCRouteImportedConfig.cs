﻿using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
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
            //Ignore(i => i.IsPartModified);
        }
    }
}
