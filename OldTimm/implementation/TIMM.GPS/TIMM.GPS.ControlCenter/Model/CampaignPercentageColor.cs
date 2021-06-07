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
using TIMM.GPS.Model;

namespace TIMM.GPS.ControlCenter.Model
{
    public class CampaignPercentageColorUI : CampaignPercentageColor
    {
        public SolidColorBrush FillColor
        {
            get
            {
                SolidColorBrush result;
                switch (this.ColorId)
                {
                    case 1:
                        result = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0x00, 0xFF));
                        break;
                    case 2:
                        result = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0xFF, 0x00));
                        break;
                    case 3:
                        result = new SolidColorBrush(Color.FromArgb(0x99, 0xFF, 0xFF, 0x00));
                        break;
                    case 4:
                        result = new SolidColorBrush(Color.FromArgb(0xB1, 0xFF, 0x96, 0x00));
                        break;
                    case 5:
                        result = new SolidColorBrush(Color.FromArgb(0x99, 0xBB, 0x00, 0x00));
                        break;
                    default:
                        result = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0x00, 0xFF));
                        break;
                }
                return result;
            }
        }

        private readonly string[] ColorNames = new string[] { "Blue", "Green", "Yellow", "Orange", "Red" };

        public string DisplayName
        {
            get
            {
                if (this.ColorId >= 1 && this.ColorId <=5)
                {
                    return ColorNames[this.ColorId - 1];
                }
                return string.Empty;
            }
        }
    }
}
