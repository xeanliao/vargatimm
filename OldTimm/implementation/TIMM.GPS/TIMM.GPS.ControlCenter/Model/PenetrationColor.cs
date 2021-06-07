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

namespace TIMM.GPS.ControlCenter.Model
{
    public class PenetrationColor
    {
        public Color ColorName { get; set; }
        /// <summary>
        /// the color is >= Lower Value and < Upper Value
        /// value >= 0 and <= 100
        /// </summary>
        public double LowerValue { get; set; }

        /// <summary>
        /// the color is >= Lower Value and < Upper Value
        /// </summary>
        public double UpperValue { get; set; }

    }
}
