using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;

namespace TIMM.GPS.Model
{
    public class PremiumCRoute : AbstractArea
    {
        public PremiumCRoute()
        {
            Locations = new List<PremiumCRouteCoordinate>();
        }
        [DefaultValue(0)]
        public int APT_COUNT { get; set; }
        [DefaultValue(0)]
        public int BUSINESS_COUNT { get; set; }
        public string COUNTY { get; set; }
        public string CROUTE { get; set; }
        public string Description { get; set; }
        public string FIPSSTCO { get; set; }
        [DefaultValue(0)]
        public int HOME_COUNT { get; set; }
        [DefaultValue(0)]
        public int OTotal { get; set; }
        public string STATE { get; set; }
        public string STATE_FIPS { get; set; }
        public int? TOTAL_COUNT { get; set; }
        public string ZIP { get; set; }
        public string ZIP_NAME { get; set; }
        public string GEOCODE { get; set; }

        public List<PremiumCRouteCoordinate> Locations { get; set; }
    }
}
