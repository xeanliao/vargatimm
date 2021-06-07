using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class NdAddressBoxMapping : AbstractAreaBoxMapping
    {
        #region parent

        public virtual NdAddress NdAddress
        {
            get;
            set;
        }

        #endregion
    }
}
