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
    public class DistributionMap
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        public string Name { get; set; }
        [DefaultValue(0)]
        public int ColorB { get; set; }
        [DefaultValue(0)]
        public int ColorG { get; set; }
        [DefaultValue(0)]
        public int ColorR { get; set; }
        public string ColorString { get; set; }
        [DefaultValue(0)]
        public int Penetration { get; set; }
        [DefaultValue(0.0)]
        public double Percentage { get; set; }
        [DefaultValue(0)]
        public int Total { get; set; }
        [DefaultValue(0)]
        public int TotalAdjustment { get; set; }
        [DefaultValue(0)]
        public int CountAdjustment { get; set; }

        public DistributionMap()
        {
            Tasks = new List<Task>();
            Locations = new List<Location>();
            GtuInfo = new List<GtuInfo>();
            NdAddress = new List<NdAddress>();
        }
        [DefaultValue(0)]
        public int SubMapId { get; set; }
        public virtual SubMap SubMap { get; set; }
        public virtual List<DistributionMapCoordinate> DistributionMapCoordinates { get; set; }
        public virtual List<Task> Tasks { get; set; }
        public virtual List<DistributionMapRecord> DistributionMapRecords { get; set; }

        public virtual List<Location> Locations { get; set; }
        public virtual List<GtuInfo> GtuInfo { get; set; }
        public virtual List<NdAddress> NdAddress { get; set; }
    }
}
