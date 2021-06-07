using System;

namespace GPS.DomainLayer.Entities
{
    public class FiveZipBoxMapping : AbstractAreaBoxMapping
    {
        #region parent

        public virtual FiveZipArea FiveZipArea
        {
            get;
            set;
        }

        #endregion
    }
}
