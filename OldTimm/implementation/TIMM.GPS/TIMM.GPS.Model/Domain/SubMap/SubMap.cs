﻿using System;
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
    public class SubMap
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        [DefaultValue(0)]
        public int OrderId { get; set; }
        [DefaultValue(0)]
        public int ColorB { get; set; }
        [DefaultValue(0)]
        public int ColorG { get; set; }
        [DefaultValue(0)]
        public int ColorR { get; set; }
        public string ColorString { get; set; }
        public string Name { get; set; }
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

        public SubMap()
        {
            SubMapCoordinates = new List<SubMapCoordinate>();
            SubMapRecords = new List<SubMapRecord>();
            DistributionMaps = new List<DistributionMap>();

            BlockGroups = new List<SubMapDetailItem>();
            FiveZipAreas = new List<SubMapDetailItem>();
            Tracts = new List<SubMapDetailItem>();
            PremiumCRoutes = new List<SubMapDetailItem>();

            GtuInfos = new List<GtuInfo>();
        }
        [DefaultValue(0)]
        public int CampaignId { get; set; }
        public virtual Campaign Campaign { get; set; }

        public virtual List<SubMapCoordinate> SubMapCoordinates { get; set; }
        public virtual List<SubMapRecord> SubMapRecords { get; set; }
        public virtual List<DistributionMap> DistributionMaps { get; set; }

        public virtual List<SubMapDetailItem> BlockGroups { get; set; }
        public virtual List<SubMapDetailItem> FiveZipAreas { get; set; }
        public virtual List<SubMapDetailItem> Tracts { get; set; }
        public virtual List<SubMapDetailItem> PremiumCRoutes { get; set; }

        public virtual List<GtuInfo> GtuInfos { get; set; }
    }
}