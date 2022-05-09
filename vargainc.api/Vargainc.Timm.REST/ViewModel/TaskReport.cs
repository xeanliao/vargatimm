using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Vargainc.Timm.REST.ViewModel
{
    public class TaskReport
    {
        [JsonIgnore]
        public int? Count { get; set; }
        public double? SpeedAvg { get; set; }
        public double? SpeedHigh { get; set; }
        public double? SpeedLow { get; set; }
        public double? GroundAvg { get; set; }
        public double? GroundHigh { get; set; }
        public double? GroundLow { get; set; }
        [JsonIgnore]
        public int? Stop { get; set; }
        public double? StopAvg { get; set; }
        public int? StopHigh { get; set; }
        public int? StopLow { get; set; }
    }
}