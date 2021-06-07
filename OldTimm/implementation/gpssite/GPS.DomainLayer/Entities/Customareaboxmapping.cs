using System; 
using System.Collections.Generic; 
using System.Text; 

namespace GPS.DomainLayer.Entities 
{
    public class CustomAreaBoxMapping : AbstractAreaBoxMapping
    {
        #region parent

        public virtual CustomArea CustomArea
        {
            get;
            set;
        }

        #endregion
    }
}
