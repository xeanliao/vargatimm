using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vargainc.Timm.REST.ViewModel
{
    public class ViewGTUInfo
    {
        public string Code { get; set; }
        public bool? IsOnline { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}