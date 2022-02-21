using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Spatial;

namespace Vargainc.Timm.Models
{
    public class PremiumCRoute : AbstractArea
    {
        public string COUNTY { get; set; }
        public string CROUTE { get; set; }
        public string Description { get; set; }
        public string FIPSSTCO { get; set; }
        [DefaultValue(0)]
        public int? OTotal { get; set; }
        public string STATE { get; set; }
        public string STATE_FIPS { get; set; }
        public string ZIP { get; set; }
        public string ZIP_NAME { get; set; }
        public string GEOCODE { get; set; }
        public DbGeometry Geom { get; set; }
        public virtual ICollection<PremiumCRouteCoordinate> Locations { get; set; }
    }
}
