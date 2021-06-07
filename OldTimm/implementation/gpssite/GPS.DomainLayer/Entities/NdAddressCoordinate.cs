using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class NdAddressCoordinate : ICoordinate
    {
        public NdAddressCoordinate()
        {
        }
        public virtual long Id
        {
            get;
            set;
        }
        public virtual double Latitude
        {
            get;
            set;
        }
        public virtual double Longitude
        {
            get;
            set;
        }

        #region parent

        public virtual NdAddress NdAddress
        {
            get;
            set;
        }

        #endregion

        #region ICoordinate Members

        public virtual int ShapeId
        {
            get { return 0; }
        }

        #endregion
    }
}
