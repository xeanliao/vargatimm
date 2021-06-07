using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using Microsoft.Maps.MapControl;
using System.Collections.Generic;

namespace TIMM.GPS.ControlCenter.Extends
{
    public class GeoCodeCalculation
    {
        public enum DistanceMeasure
        {
            Miles,
            Kilometers
        }

        public const double EarthRadiusInMiles = 3956.0;
        public const double EarthRadiusInKilometers = 6367.0;

        public static double ToRadian(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static LocationCollection CreateCircle(Location center, double radius, DistanceMeasure units)
        {
            var earthRadius = (units == DistanceMeasure.Miles ? EarthRadiusInMiles : EarthRadiusInKilometers);
            var lat = ToRadian(center.Latitude); //radians
            var lng = ToRadian(center.Longitude); //radians
            var d = radius / earthRadius; // d = angular distance covered on earth's surface
            var locations = new LocationCollection();

            for (var x = 0; x <= 360; x++)
            {
                var brng = ToRadian(x);
                var latRadians = Math.Asin(Math.Sin(lat) * Math.Cos(d) + Math.Cos(lat) * Math.Sin(d) * Math.Cos(brng));
                var lngRadians = lng + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat), Math.Cos(d) - Math.Sin(lat) * Math.Sin(latRadians));

                locations.Add(new Location(ToDegrees(latRadians), ToDegrees(lngRadians)));
            }

            return locations;
        }

        public static LocationCollection Create4CornerLocation(Location center, double radius, DistanceMeasure units)
        {
            var earthRadius = (units == DistanceMeasure.Miles ? EarthRadiusInMiles : EarthRadiusInKilometers);
            var lat = ToRadian(center.Latitude); //radians
            var lng = ToRadian(center.Longitude); //radians
            var d = radius / earthRadius; // d = angular distance covered on earth's surface
            var locations = new LocationCollection();
            var corner = new int[] { 0, 90, 180, 270 };
            for (var x = 0; x < corner.Length; x++)
            {
                var brng = ToRadian(corner[x]);
                var latRadians = Math.Asin(Math.Sin(lat) * Math.Cos(d) + Math.Cos(lat) * Math.Sin(d) * Math.Cos(brng));
                var lngRadians = lng + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat), Math.Cos(d) - Math.Sin(lat) * Math.Sin(latRadians));

                locations.Add(new Location(ToDegrees(latRadians), ToDegrees(lngRadians)));
            }

            return locations;
        }

        public static Location GetPloygonCenter(LocationCollection locations)
        {
            //double xSum = 0.0d; 
            //double ySum = 0.0d; 
            //double area = 0.0d; 
            //for(int i = 0; i < locations.Count - 1; i++) 
            //{ 
            //    Location p0 = locations[i]; 
            //    Location p1 = locations[i+1];

            //    double areaSum = (p0.Latitude * p1.Longitude) - (p1.Latitude * p0.Longitude);

            //    xSum += (p0.Latitude + p1.Latitude) * areaSum;
            //    ySum += (p0.Longitude + p1.Longitude) * areaSum; 
            //    area += areaSum; 
            //}

            //double centerX = xSum / (area * 6d);
            //double centerY = ySum / (area * 6d);
            var centerX = locations.Average(i => i.Latitude);
            var centerY = locations.Average(i => i.Longitude);
            return new Location(centerX, centerY);
        }

        public static LocationCollection CreatePoint(double radii, Location center)
        {
            var lat = ToRadian(center.Latitude); //radians
            var lng = ToRadian(center.Longitude); //radians
            var d = radii / EarthRadiusInMiles; // d = angular distance covered on earth's surface
            var locations = new LocationCollection();

            for (var x = 0; x <= 360; x+=10)
            {
                var brng = ToRadian(x);
                var latRadians = Math.Asin(Math.Sin(lat) * Math.Cos(d) + Math.Cos(lat) * Math.Sin(d) * Math.Cos(brng));
                var lngRadians = lng + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat), Math.Cos(d) - Math.Sin(lat) * Math.Sin(latRadians));

                locations.Add(new Location(ToDegrees(latRadians), ToDegrees(lngRadians)));
            }

            return locations;
        }

        public static bool PointInPolygon(IList<Location> coordinates, double lat, double lon)
        {
            bool inPolygon = false;
            int j = coordinates.Count - 1;

            for (int i = 0; i < coordinates.Count; i++)
            {
                if (coordinates[i].Longitude < lon && coordinates[j].Longitude >= lon
                  || coordinates[j].Longitude < lon && coordinates[i].Longitude >= lon)
                {
                    if (coordinates[i].Latitude + (lon - coordinates[i].Longitude) /
                      (coordinates[j].Longitude - coordinates[i].Longitude) * (coordinates[j].Latitude
                        - coordinates[i].Latitude) < lat)
                    {
                        inPolygon = !inPolygon;
                    }
                }
                j = i;
            }

            return inPolygon;
        }

        public static bool PointInPolygon(double[][] coordinates, double[] point)
        {
            bool inPolygon = false;
            int j = coordinates.Length - 1;

            double lat = point[0];
            double lon = point[1];

            for (int i = 0; i < coordinates.Length; i++)
            {
                if (coordinates[i][1] < lon && coordinates[j][1] >= lon
                  || coordinates[j][1] < lon && coordinates[i][1] >= lon)
                {
                    if (coordinates[i][0] + (lon - coordinates[i][1]) /
                      (coordinates[j][1] - coordinates[i][1]) * (coordinates[j][0]
                        - coordinates[i][0]) < lat)
                    {
                        inPolygon = !inPolygon;
                    }
                }
                j = i;
            }

            return inPolygon;
        }
    }
}
