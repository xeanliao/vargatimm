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
using System.Linq;
using TIMM.GPS.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TIMM.GPS.ControlCenter.Model
{
    public class CampaignUI : Campaign
    {
        public CampaignUI()
        {
            SubMaps = new List<SubMapUI>();
        }

        [JsonIgnore]
        public bool IsChecked
        {
            get;
            set;
        }
        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}-{4}",
                    Date.ToString("MMddyy"),
                    ClientCode,
                    CreatorName,
                    AreaDescription,
                    Sequence);
            }
        }
        [JsonIgnore]
        public string LogoFileName { get; set; }
        [JsonIgnore]
        public byte[] LogoFile { get; set; }
        [JsonIgnore]
        public int TotalHouseHolds
        {
            get
            {
                int total = 0;
                SubMaps.ForEach(i => 
                {
                    total += i.Total;
                });
                return total;
            }
        }

        [JsonIgnore]
        public int TargetHouseHolds
        {
            get
            {
                int total = 0;
                SubMaps.ForEach(i =>
                {
                    total += i.Penetration;
                });
                return total;
            }
        }

        [JsonIgnore]
        public double Penetration
        {
            get
            {
                double total = 0;
                double target = 0;
                SubMaps.ForEach(i =>
                {
                    total += i.Total;
                    target += i.Penetration;
                });
                return target / total;
            }
        }

        [JsonIgnore]
        public string TargetMethod { get; set; }

        public new List<SubMapUI> SubMaps { get; set; }

        public new List<CampaignPercentageColorUI> CampaignPercentageColors { get; set; }

        public new List<DistributionMapUI> DistributionMap { get; set; }
    }
}
