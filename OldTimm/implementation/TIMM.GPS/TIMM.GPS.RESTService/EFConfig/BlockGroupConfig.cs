using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFConfig
{
    class BlockGroupConfig : EntityTypeConfiguration<BlockGroup>
    {
        public BlockGroupConfig()
        {
            HasKey(i => i.Id).ToTable("blockgroups");
            Property(i => i.DbIsEnabled).HasColumnName("IsEnabled");
            Ignore(i => i.IsEnabled);
            Ignore(i => i.HasMultipleParts);

            HasMany(i => i.Locations)
                .WithRequired()
                .HasForeignKey(i => i.BlockGroupId);
        }
    }
}
