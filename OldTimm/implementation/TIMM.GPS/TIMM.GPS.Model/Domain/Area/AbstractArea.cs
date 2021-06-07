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
    public abstract class AbstractArea
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        [DefaultValue(0)]
        public int IsInnerShape { get; set; }
        [DefaultValue(0)]
        public int PartCount { get; set; }
        [DefaultValue(0)]
        public byte IsInnerRing { get; set; }
        [DefaultValue(0.0)]
        public double Latitude { get; set; }
        [DefaultValue(0.0)]
        public double Longitude { get; set; }
        [DefaultValue(0.0)]
        public double MaxLatitude { get; set; }
        [DefaultValue(0.0)]
        public double MaxLongitude { get; set; }
        [DefaultValue(0.0)]
        public double MinLatitude { get; set; }
        [DefaultValue(0.0)]
        public double MinLongitude { get; set; }
        [DefaultValue(0)]
        public int DbIsEnabled { get; set; }

        public bool IsEnabled
        {
            get
            {
                return DbIsEnabled == 1 ? true : false;
            }
            set
            {
                DbIsEnabled = value ? 1 : 0;
            }
        }

        public bool HasMultipleParts
        {
            get
            {
                return PartCount > 1;
            }
        }
    }
}
