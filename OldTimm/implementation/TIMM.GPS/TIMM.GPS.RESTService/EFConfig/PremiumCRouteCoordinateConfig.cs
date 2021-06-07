using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using TIMM.GPS.Model;

namespace TIMM.GPS.RESTService.EFConfig
{
    class PremiumCRouteCoordinateConfig : EntityTypeConfiguration<PremiumCRouteCoordinate>
    {
        public PremiumCRouteCoordinateConfig()
        {
            HasKey(i => i.Id).ToTable("premiumcroutecoordinates");
        }
    }
}
