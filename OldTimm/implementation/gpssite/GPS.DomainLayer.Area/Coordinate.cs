using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Area {
    [Serializable]
    public class Coordinate : ICoordinate {
        private double _latitude;
        private double _longitude;
        private int _shapeId;

        public Coordinate() { }

        public Coordinate(double latitude, double longitude) {
            _latitude = latitude;
            _longitude = longitude;
            _shapeId = 0;
        }

        public Coordinate(double latitude, double longitude, int shapeId)
        {
            _latitude = latitude;
            _longitude = longitude;
            _shapeId = shapeId;
        }

        #region ICoordinate Members

        public double Latitude {
            get {
                return _latitude;
            }
            set {
                _latitude = value;
            }
        }

        public double Longitude {
            get {
                return _longitude;
            }
            set {
                _longitude = value;
            }
        }

        public int ShapeId
        {
            get { return _shapeId; }
        }

        #endregion
    }
}
