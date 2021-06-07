using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class CountyAreaBoxMapping : AbstractAreaBoxMapping
    {
        #region parent

        public virtual CountyArea CountyArea
        {
            get;
            set;
        }

        #endregion
    }
}
