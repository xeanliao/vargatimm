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
using System.ComponentModel;

namespace TIMM.GPS.Model
{
    public class CampaignPercentageColor
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        [DefaultValue(0)]
        public int ColorId { get; set; }
        [DefaultValue(0.0)]
        public double Max { get; set; }
        [DefaultValue(0.0)]
        public double Min { get; set; }

        [DefaultValue(0)]
        public int CampaignId { get; set; }
    }
}
