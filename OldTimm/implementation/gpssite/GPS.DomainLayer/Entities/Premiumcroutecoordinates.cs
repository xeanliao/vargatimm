using System; 
using System.Collections.Generic; 
using System.Text;
using GPS.DomainLayer.Interfaces; 

namespace GPS.DomainLayer.Entities 
{
    public class PremiumCRouteCoordinate : AbstractAreaCoordinate, ICoordinate 
    {
        public virtual int PreminumCRouteId { get; set; }
        public virtual int ShapeId { get; set; }
    }
}
