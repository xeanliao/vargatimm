using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class BlockGroup : AbstractArea, IBoxArea
    {
        public virtual string Name { get; set; }
        public virtual string StateCode { get; set; }
        public virtual string CountyCode { get; set; }
        public virtual string TractCode { get; set; }
        public virtual string LSAD { get; set; }
        public virtual string LSADTrans { get; set; }
        public virtual int OTotal { get; set; }
        public virtual string Description { get; set; }

        #region childrens

        public virtual IList<BlockGroupCoordinate> BlockGroupCoordinates { get; set; }

        public virtual IList<BlockGroupBoxMapping> BlockGroupBoxMappings { get; set; }

        public virtual IList<BlockGroupSelectMapping> BlockGroupSelectMappings { get; set; }

        #endregion

        #region IBoxArea Members

        /// <summary>
        /// population total
        /// </summary>
        public virtual long Total { get; set; }

        /// <summary>
        /// percentage of penetration
        /// </summary>
        public virtual double Penetration { get; set; }

        /// <summary>
        /// the parent id
        /// </summary>
        public virtual int tractId { get; set; }

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
