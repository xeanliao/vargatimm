﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    public class CampaignConfig : EntityTypeConfiguration<Campaign>
    {
        public CampaignConfig()
        {
            HasKey(i => i.Id).ToTable("campaigns");
            HasMany(i => i.CampaignBlockGroupImporteds)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(false);
            HasMany(i => i.CampaignCRouteImporteds)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(false);
            HasMany(i => i.CampaignFiveZipImporteds)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(false);
            HasMany(i => i.CampaignTractImporteds)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(false);
            HasMany(i => i.CampaignClassifications)
                .WithRequired()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(false);

            HasMany(i => i.Status)
                .WithRequired(i=>i.Campaign)
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(false);

            HasMany(i => i.SubMaps)
                .WithRequired(i => i.Campaign)
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(false);

            HasMany(i => i.Addresses)
                .WithRequired(i => i.Campaign)
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(false);

            HasMany(i => i.CampaignPercentageColors)
                .WithOptional()
                .HasForeignKey(i => i.CampaignId)
                .WillCascadeOnDelete(false);

            Ignore(i => i.LogoPath);
            Ignore(i => i.DistributionMap);
        }
    }

    
}
