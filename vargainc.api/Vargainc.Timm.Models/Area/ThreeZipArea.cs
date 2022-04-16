using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Spatial;

namespace Vargainc.Timm.Models
{
    public class ThreeZipArea
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Zip { get; set; }
        public int? APT_COUNT { get; set; }
        public int? BUSINESS_COUNT { get; set; }
        public int? HOME_COUNT { get; set; }
        public int? TOTAL_COUNT { get; set; }
        public DbGeometry Geom { get; set; }
    }
}
