using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.Area {
    public class MapCircle : ICircle {
        private ICoordinate _center;
        private double _radius;
        private List<ICoordinate> _coordinates;

        #region constructor

        public MapCircle(ICoordinate center, Double radius) {
            this._center = center;
            this._radius = radius;
        }

        #endregion

        #region Method

        public void CalculateCoordinates() {
            double earthRadius = 3959;// 6371;
            //latitude in radians
            double lat = (this._center.Latitude * Math.PI) / 180;
            //longitude in radians
            double lon = (this._center.Longitude * Math.PI) / 180;
            //angular distance covered on earth's surface
            double d = this._radius / earthRadius;
            List<ICoordinate> points = new List<ICoordinate>();
            for (int i = 0; i <= 360; i++) {
                ICoordinate point = new Coordinate(0, 0);
                double bearing = i * Math.PI / 180; //rad
                point.Latitude = Math.Asin(Math.Sin(lat) * Math.Cos(d) +
                  Math.Cos(lat) * Math.Sin(d) * Math.Cos(bearing));
                point.Longitude = ((lon + Math.Atan2(Math.Sin(bearing) * Math.Sin(d) * Math.Cos(lat),
                  Math.Cos(d) - Math.Sin(lat) * Math.Sin(point.Latitude))) * 180) / Math.PI;
                point.Latitude = (point.Latitude * 180) / Math.PI;
                points.Add(point);
            }
            this._coordinates = points;
        }

        #endregion

        #region ICircle Members

        public ICoordinate Center {
            get {
                return _center;
            }
            set {
                _center = value;
            }
        }

        public double Radius {
            get {
                return _radius;
            }
            set {
                _radius = value;
            }
        }

        public List<ICoordinate> Coordinates {
            get {
                return _coordinates;
            }
            set {
                _coordinates = value;
            }
        }

        #endregion

    }
}
