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
    public class Radiuses
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        [DefaultValue(0)]
        public int AddressId { get; set; }
        public Address Address { get; set; }
        [DefaultValue(false)]
        public bool IsDisplay { get; set; }
        [DefaultValue(0.0)]
        public double Length { get; set; }
        [DefaultValue(0)]
        public int LengthMeasuresId { get; set; }
    }
}
