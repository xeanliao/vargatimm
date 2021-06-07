using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class BlockGroupCoordinate : AbstractAreaCoordinate, ICoordinate
    {
        public virtual BlockGroup BlockGroup { get; set; }

        #region ICoordinate Members

        public virtual int ShapeId
        {
            get { return 0; }
            set { }
        }

        #endregion
    }
}
