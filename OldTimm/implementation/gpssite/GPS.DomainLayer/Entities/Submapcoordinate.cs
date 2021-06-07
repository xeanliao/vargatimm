using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class SubMapCoordinate : AbstractAreaCoordinate
    {
        public virtual int SubMapId
        {
            get { return SubMap.Id; }
            set
            {
                SubMap.Id = value;
            }
        }
        public virtual SubMap SubMap { get; set; }
    }
}
