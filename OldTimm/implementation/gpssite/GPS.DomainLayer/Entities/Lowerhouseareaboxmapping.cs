using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class LowerHouseAreaBoxMapping : AbstractAreaBoxMapping
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
