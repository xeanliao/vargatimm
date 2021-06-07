using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public abstract class AbstractAreaCoordinate
    {
        public virtual long Id { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
    }
}
