using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class Record
    {
        [JsonIgnore]
        public int? Id { get; set; }
        public string Name { get; set; }
        
        [JsonIgnore]
        public float? APT { get; set; }
        [JsonIgnore]
        public float? Home { get; set; }
        public double? TotalHouseHold { get; set; }
        public double? TargetHouseHold { get; set; }

        public double Penetration
        {
            get
            {
                if(!this.TotalHouseHold.HasValue || this.TotalHouseHold.Value == 0)
                {
                    return 0;
                }
                return Math.Round((this.TargetHouseHold ?? 0) / this.TotalHouseHold.Value, 4);
            }
        }

    }
}