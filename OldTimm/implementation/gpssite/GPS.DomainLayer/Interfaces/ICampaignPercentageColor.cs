using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Interfaces
{
    public interface ICampaignPercentageColor
    {
        double Min { get; set; }
        double Max { get; set; }
        int ColorId { get; set; }
    }
}
