using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;

namespace GPS.DomainLayer.Interfaces
{
    public interface IDistributionMap
    {
        int DMId { get; set; }
        String DMName { get; set; }
        int SubMapId { get; set; }

        List<IAreaRecord> Records { get; set; }
        List<ICoordinate> Coordinates { get; set; }
        
    }
}
