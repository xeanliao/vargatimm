using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class Record
    {
        [JsonIgnore]
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Zip { get; set; }
        
        [JsonIgnore]
        public float? APT { get; set; }
        [JsonIgnore]
        public float? Home { get; set; }
        [JsonIgnore]
        public float? Business { get; set; }
        public double? TotalHouseHold { get; set; }
        public double? TargetHouseHold { get; set; }

        public double Penetration
        {
            get
            {
                if (!this.TotalHouseHold.HasValue || this.TotalHouseHold == 0)
                {
                    return 0.0;
                }
                return Math.Round((this.TargetHouseHold ?? 0) / this.TotalHouseHold.Value, 4);
            }
        }
        [JsonIgnore]
        public DbGeometry Geom { get; set; }
    }
}