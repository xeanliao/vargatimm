using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class BlockGroupSelectMapping
    {
        public BlockGroupSelectMapping()
        {
        }
        public virtual int Id
        {
            get;
            set;
        }

        #region parent

        public virtual FiveZipArea FiveZipArea { get; set; }

        public virtual BlockGroup BlockGroup { get; set; }

        public virtual Tract Tract { get; set; }

        public virtual ThreeZipArea ThreeZipArea { get; set; }

        #endregion
    }
}
