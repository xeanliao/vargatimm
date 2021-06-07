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

namespace TIMM.GPS.ControlCenter.Extend
{
    public static class ColorHelper
    {
        public static Color FromRgbString(string rgbString)
        {
            return FromRgbString(0xFF, rgbString);
        }

        public static Color FromRgbString(byte alpha, string rgbString)
        {
            if (string.IsNullOrWhiteSpace(rgbString))
                return Colors.Transparent;

            if (rgbString.StartsWith("#"))
            {
                rgbString = rgbString.Substring(1);
            }

            if (rgbString.Length == 6)
            {
                rgbString = rgbString.ToUpper();
                const string haxString = "0123456789ABCDEF";
                var charArray = rgbString.ToCharArray();
                byte red = (byte)(haxString.IndexOf(charArray[0]) << 4 | haxString.IndexOf(charArray[1]));
                byte green = (byte)(haxString.IndexOf(charArray[2]) << 4 | haxString.IndexOf(charArray[3]));
                byte blue = (byte)(haxString.IndexOf(charArray[4]) << 4 | haxString.IndexOf(charArray[5]));
                return Color.FromArgb(alpha, red, green, blue);
            }
            return Colors.Transparent;
        }
    }
}
