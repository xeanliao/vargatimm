using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class MetropolitanCoreAreaCoordinate : AbstractAreaCoordinate, ICoordinate
    {
        public virtual int MetropolitanCoreAreaId { get; set; }

        #region ICoordinate Members

        public virtual int ShapeId
        {
            get { return 0; }
            set { }
        }

        #endregion
    }
}
