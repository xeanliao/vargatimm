using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class LowerHouseAreaCoordinate : AbstractAreaCoordinate
    {
        #region parent

        public virtual LowerHouseArea LowerHouseArea
        {
            get;
            set;
        }

        #endregion
    }
}
