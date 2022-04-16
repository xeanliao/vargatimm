using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class TaskReport
    {
        public string GtuUniqueID { get; set; }
        public double? SpeedAvg { get; set; }
        public double? SpeedHigh { get; set; }
        public double? SpeedLow { get; set; }
        public double? GroundAvg { get; set; }
        public double? GroundHigh { get; set; }
        public double? GroundLow { get; set; }
        public int? Stop { get; set; }
    }
}