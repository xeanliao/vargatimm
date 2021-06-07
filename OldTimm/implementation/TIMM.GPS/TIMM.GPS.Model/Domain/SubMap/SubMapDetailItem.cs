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
    public class SubMapDetailItem
    {
        public SubMapDetailItem()
        {
            Locations = new List<Location>();
        }
        [DefaultValue(0)]
        public int OrderId { get; set; }
        [DefaultValue(0)]
        public int AreaId { get; set; }
        [DefaultValue(0)]
        public int Classification { get; set; }
        public string Name { get; set; }        
        public int? TotalHouseHold { get; set; }
        public int? TargetHouseHold { get; set; }
        public double DisplayPenetration
        {
            get
            {
                if (TotalHouseHold.HasValue && TargetHouseHold.HasValue)
                {
                    if (TotalHouseHold == 0)//denominator is zero
                        return 0;
                    return (double)TargetHouseHold / (double)TotalHouseHold;
                }
                return 0d;
            }
        }

        public List<Location> Locations { get; set; }
    }
}
