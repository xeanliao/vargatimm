using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    class PremiumCRouteConfig : EntityTypeConfiguration<PremiumCRoute>
    {
        public PremiumCRouteConfig()
        {
            ToTable("premiumcroutes");
            HasKey(i => i.Id);
            Property(i => i.DbIsEnabled).HasColumnName("IsEnabled");
            Ignore(i => i.IsEnabled);
            Ignore(i => i.HasMultipleParts);

            HasMany(i => i.Locations)
                .WithRequired()
                .HasForeignKey(i => i.PreminumCRouteId);
        }
    }
}
