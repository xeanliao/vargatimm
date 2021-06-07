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
using System.Windows.Data;

namespace TIMM.GPS.ControlCenter.Converters
{
    /// <summary>
    /// If parameter is true, reverse result
    /// </summary>
    public class BooleanToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool paramValue = parameter is bool ? System.Convert.ToBoolean(parameter) : false;

            if ((bool)value)
            {
                if (!paramValue)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                if (!paramValue)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool paramValue = parameter is bool ? (bool)parameter : false;

            if ((Visibility)value == Visibility.Visible)
            {
                if (!paramValue)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (!paramValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
