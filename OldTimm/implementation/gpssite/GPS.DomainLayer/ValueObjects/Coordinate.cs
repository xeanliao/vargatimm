using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DataLayer.ValueObjects
{
    public struct Coordinate : ICoordinate
    {
        #region ICoordinate Members

        public double Latitude
        {
            get;
            set;
        }

        public double Longitude
        {
            get;
            set;
        }

        public int ShapeId
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
