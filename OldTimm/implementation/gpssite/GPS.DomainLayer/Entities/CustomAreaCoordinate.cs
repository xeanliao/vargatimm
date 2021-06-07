using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class CustomAreaCoordinate : AbstractAreaCoordinate, ICoordinate
    {
        #region parent

        public virtual CustomArea CustomArea
        {
            get;
            set;
        }

        #endregion

        #region ICoordinate Members

        public virtual int ShapeId
        {
            get { return 0; }
        }

        #endregion
    }
}
