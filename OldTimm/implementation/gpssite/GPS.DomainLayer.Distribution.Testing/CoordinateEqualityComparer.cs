using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Distribution.Testing
{
    class CoordinateEqualityComparer : IEqualityComparer<ICoordinate>
    {
        #region IEqualityComparer<ICoordinate> Members

        public bool Equals(ICoordinate x, ICoordinate y)
        {
            return x.Longitude == y.Longitude && x.Latitude == y.Latitude;
        }

        public int GetHashCode(ICoordinate obj)
        {
            return (int)Math.Ceiling(obj.Latitude + obj.Longitude);
        }

        #endregion
    }
}
