using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Interfaces
{
    public interface ISubMap
    {
        int Id { get; set; }
        string Name { get; set; }
        int Total { get; set; }
        int Penetration { get; set; }
        double Percentage { get; set; }
        int ColorR { get; set; }
        int ColorG { get; set; }
        int ColorB { get; set; }
        string ColorString { get; set; }
        int CampaignId { get; set; }
        List<IAreaRecord> Records { get; set; }
        List<ICoordinate> Coordinates { get; set; }
        List<IDistributionMap> DistributionMaps { get; set; }
    }
}
