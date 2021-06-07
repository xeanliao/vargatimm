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
    public class Location
    {
        [DefaultValue(0)]
        public long Id { get; set; }
        [DefaultValue(0.0)]
        public double Latitude { get; set; }
        [DefaultValue(0.0)]
        public double Longitude { get; set; }
    }
}
