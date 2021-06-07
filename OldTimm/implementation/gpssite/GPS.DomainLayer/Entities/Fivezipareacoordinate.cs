using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class FiveZipAreaCoordinate : AbstractAreaCoordinate, ICoordinate
    {
        public virtual int ShapeId { get; set; }

        #region parent

        public virtual FiveZipArea FiveZipArea
        {
            get;
            set;
        }

        #endregion
    }
}
