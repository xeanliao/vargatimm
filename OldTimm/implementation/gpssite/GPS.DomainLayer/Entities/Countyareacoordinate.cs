using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class CountyAreaCoordinate : AbstractAreaCoordinate, ICoordinate
    {
        #region parent

        public virtual CountyArea CountyArea
        {
            get;
            set;
        }

        #endregion

        #region ICoordinate Members

        public virtual int ShapeId
        {
            get
            {
                return 0;
            }
        }

        #endregion
    }
}
