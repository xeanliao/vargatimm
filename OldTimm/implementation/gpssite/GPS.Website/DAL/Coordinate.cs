using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPS.Website.DAL
{
    public class Coordinate
    {
        public double Longitude;
        public double Latitude;

        public Coordinate(double latitude, double longitude)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
        }
    }
}