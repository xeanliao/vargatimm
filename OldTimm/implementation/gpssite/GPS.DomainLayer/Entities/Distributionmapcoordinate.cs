using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class DistributionMapCoordinate : AbstractAreaCoordinate
    {
        public virtual int DistributionMapId { get; set; }
    }
}
