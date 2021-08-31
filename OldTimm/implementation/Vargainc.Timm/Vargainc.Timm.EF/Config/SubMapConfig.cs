﻿using System.Data.Entity.ModelConfiguration;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.EF.Config
{
    class SubMapConfig : EntityTypeConfiguration<SubMap>
    {
        public SubMapConfig()
        {
            HasKey(i => i.Id).ToTable("submaps");

            HasMany(i => i.SubMapCoordinates)
                .WithRequired(i => i.SubMap)
                .HasForeignKey(i => i.SubMapId)
                .WillCascadeOnDelete(false);

            HasMany(i => i.DistributionMaps)
                .WithRequired(i => i.SubMap)
                .HasForeignKey(i => i.SubMapId);

            HasMany(i => i.SubMapRecords)
                .WithRequired(i => i.SubMap)
                .HasForeignKey(i => i.SubMapId);

            Ignore(i => i.FiveZipAreas);
            Ignore(i => i.BlockGroups);
            Ignore(i => i.Tracts);
            Ignore(i => i.PremiumCRoutes);
            Ignore(i => i.GtuInfos);
            
        }
    }
}