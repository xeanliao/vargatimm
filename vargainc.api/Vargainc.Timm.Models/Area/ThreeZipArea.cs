﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Spatial;

namespace Vargainc.Timm.Models
{
    public class ThreeZipArea : AbstractArea
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string LSAD { get; set; }
        public string LSADTrans { get; set; }
        public string Name { get; set; }
        [DefaultValue(0)]
        public int? OTotal { get; set; }
        public string StateCode { get; set; }
        [DefaultValue(0)]
        public int? APT_COUNT { get; set; }
        [DefaultValue(0)]
        public int? BUSINESS_COUNT { get; set; }
        [DefaultValue(0)]
        public int? HOME_COUNT { get; set; }
        [DefaultValue(0)]
        public int? TOTAL_COUNT { get; set; }

        public DbGeometry Geom { get; set; }
        public virtual ICollection<ThreeZipCoordinate> Locations { get; set; }
    }
}