using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class FiveZipArea : AbstractArea, IBoxArea
    {
        public virtual string Description { get; set; }
        public virtual string LSAD { get; set; }
        public virtual string LSADTrans { get; set; }
        public virtual string Name { get; set; }
        public virtual int OTotal { get; set; }
        public virtual string StateCode { get; set; }

        public virtual int APT_COUNT { get; set; }
        public virtual int BUSINESS_COUNT { get; set; }
        public virtual int HOME_COUNT { get; set; }
        public virtual int TOTAL_COUNT { get; set; }

        #region children objects
        public virtual IList<FiveZipAreaCoordinate> FiveZipAreaCoordinates { get; set; }
        public virtual IList<FiveZipBoxMapping> FiveZipBoxMappings { get; set; }
        public virtual IList<BlockGroupSelectMapping> BlockGroupSelectMappings { get; set; }
        public virtual IList<PremiumCRouteSelectMapping> PremiumCRouteSelectMappings { get; set; }
        #endregion

        #region extended properties

        /// <summary>
        /// Test ----- population total
        /// </summary>
        public virtual long Total { get; set; }

        /// <summary>
        /// percentage of penetration
        /// </summary>
        public virtual double Penetration { get; set; }

        /// <summary>
        /// the parent id
        /// </summary>
        public virtual int ThreeZipId { get; set; }

        #endregion
    }
}
