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
using System.ComponentModel;

namespace TIMM.GPS.Model
{
    public class CampaignFiveZipImported : CampaignImportedDataItem
    {
        [DefaultValue(0.0)]
        public float PartPercentage { get; set; }
        [DefaultValue(false)]
        public bool IsPartModified { get; set; }
        [DefaultValue(0)]
        public int FiveZipAreaId { get; set; }
        public virtual FiveZipArea FiveZipArea { get; set; }
    }
}