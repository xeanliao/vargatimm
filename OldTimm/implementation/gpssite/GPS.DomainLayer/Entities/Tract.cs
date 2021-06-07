using System; 
using System.Collections.Generic; 
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities {

    public class Tract : AbstractArea, IBoxArea
    {
        public virtual string CountyCode { get; set; }
        public virtual string Description { get; set; }
        public virtual string LSAD { get; set; }
        public virtual string LSADTrans { get; set; }
        public virtual string Name { get; set; }
        public virtual int OTotal { get; set; }
        public virtual string StateCode { get; set; }

        #region children objects
        public virtual IList<TractBoxMapping> TractBoxMappings { get; set; }
        public virtual IList<TractCoordinate> TractCoordinates { get; set; }
        public virtual IList<BlockGroupSelectMapping> BlockGroupSelectMappings { get; set; }
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
        public virtual List<int> FiveZipIds { get; set; }

        /// <summary>
        /// the parent id
        /// </summary>
        public virtual List<int> ThreeZipIds { get; set; }

        #endregion
    }
}
