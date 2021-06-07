using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class ThreeZipAreaCoordinate : AbstractAreaCoordinate, ICoordinate
    {
        public virtual int ThreeZipAreaId { get; set; }
        public virtual int ShapeId { get; set; }
    }
}
