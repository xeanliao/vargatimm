using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
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
