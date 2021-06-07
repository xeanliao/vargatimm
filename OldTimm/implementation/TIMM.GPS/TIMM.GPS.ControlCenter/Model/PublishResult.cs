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
    public class PublishResult
    {
        public string d { get; set; }

        public bool IsSuccess
        {
            get
            {
                if (string.IsNullOrWhiteSpace(d))
                {
                    return true;
                }
                return false;
            }
        }

        public string Message
        {
            get
            {
                return d.Replace("<div>", "").Replace("</div>", "").Replace("<br />", "\r\n");
            }
        }
    }
}
