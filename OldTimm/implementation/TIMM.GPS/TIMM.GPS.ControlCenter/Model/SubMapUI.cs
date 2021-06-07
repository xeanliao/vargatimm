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
using System.Collections.Generic;

namespace TIMM.GPS.ControlCenter.Model
{
    public class SubMapUI : SubMap
    {
        public SubMapUI(SubMap baseEntity)
        {
            
        }

        public string PrintDisplayName
        {
            get
            {
                return string.Format("SUB MAP {0} ({1})", base.OrderId, base.Name);
            }
        }

        public string TargetMethod1 { get; set; }
        public string TargetMethod2 { get; set; }

        public int TotalHouseHold
        {
            get
            {
                return base.Total + base.TotalAdjustment;
            }
        }

        public int TargetHouseHold
        {
            get
            {
                return base.Penetration + base.CountAdjustment;
            }
        }

        public double DisplayPenetration
        {
            get
            {
                if (TotalHouseHold == 0)
                {
                    return 0;
                }
                return (double)TargetHouseHold / (double)TotalHouseHold;
            }
        }

        public new List<DistributionMapUI> DistributionMaps { get; set; }

        public new CampaignUI Campaign { get; set; }
    }
}
