using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class TractCoordinate : AbstractAreaCoordinate, ICoordinate
    {
        public virtual int TractId { get; set; }

        #region ICoordinate Members

        public virtual int ShapeId
        {
            get { return 0; }
        }

        #endregion
    }
}
