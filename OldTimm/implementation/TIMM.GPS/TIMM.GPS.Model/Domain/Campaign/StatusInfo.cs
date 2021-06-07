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
    public class StatusInfo
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        [DefaultValue(0)]
        public int Status { get; set; }
        [DefaultValue(0)]
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        [DefaultValue(0)]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
