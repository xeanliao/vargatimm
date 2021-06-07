using System;
using System.Collections.Generic;
using System.Text;


namespace GPS.DomainLayer.Entities
{
    public class PremiumCRoute : AbstractArea
    {
        public virtual int APT_COUNT { get; set; }
        public virtual int BUSINESS_COUNT { get; set; }
        public virtual string COUNTY { get; set; }
        public virtual string CROUTE { get; set; }
        public virtual string Description { get; set; }
        public virtual string FIPSSTCO { get; set; }
        public virtual int HOME_COUNT { get; set; }
        public virtual int OTotal { get; set; }
        public virtual string STATE { get; set; }
        public virtual string STATE_FIPS { get; set; }
        public virtual int TOTAL_COUNT { get; set; }
        public virtual string ZIP { get; set; }
        public virtual string ZIP_NAME { get; set; }

        #region children objects

        public virtual IList<PremiumCRouteBoxMapping> PremiumCRouteBoxMappings { get; set; }

        public virtual IList<PremiumCRouteCoordinate> PremiumCRouteCoordinates { get; set; }

        public virtual IList<PremiumCRouteSelectMapping> PremiumCRouteSelectMappings { get; set; }

        #endregion
    }
}
