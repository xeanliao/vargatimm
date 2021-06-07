using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Common;

namespace GTU.ModelLayer.Track
{
    public class ShapeMethods
    {
        public bool PointInPolygons(Coordinate[] coordinates, double lat, double lon)
        {
            bool inPolygon = false;
            int j = coordinates.Length - 1;

            for (int i = 0; i < coordinates.Length; i++)
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

        private static Double Rad(Double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// Get Distance between two coordinates
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="log1"></param>
        /// <param name="lat2"></param>
        /// <param name="log2"></param>
        /// <returns>distance in kilometres</returns>
        public Double GetDistance(Double lat1, Double lon1, Double lat2, Double lon2)
        {
            Double EARTH_RADIUS = 6378.137;
            Double radLat1 = Rad(lat1);
            Double radLat2 = Rad(lat2);
            Double la = radLat1 - radLat2;
            Double lo = Rad(lon1) - Rad(lon2);
            Double distance = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(la / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(lo / 2), 2)));
            distance = distance * EARTH_RADIUS;
            distance = Math.Round(distance * 10000) / 10000;
            return distance;
        }

        public bool PointInAddress(Address[] add, double lat2, double lon2)
        {
            bool inAddress = false;
            Double distance;

            for (int i = 0; i < add.Length; i++)
            {
                distance = GetDistance(add[i].Latitude, add[i].Longitude, lat2, lon2);
                if(distance <= add[i].Radius)
                {
                    inAddress = true;
                }
            }

            return inAddress;
        }

        private static bool PointInPolygon(List<ICoordinate> coordinates, double lat, double lon)
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

        public static bool PolygonInPolygon(List<ICoordinate> masterCoordinates, List<ICoordinate> innerCoordinates)
        {
            bool inPolygon = false;
            foreach (ICoordinate coordinate in innerCoordinates)
            {
                if (PointInPolygon(masterCoordinates, coordinate.Latitude, coordinate.Longitude))
                {
                    inPolygon = true;
                    break;
                }
            }
            return inPolygon;
        }

        private static bool BoxInBox(double mMinLat, double mMaxLat, double mMinLon, double mMaxLon, double iMinLat, double iMaxLat, double iMinLon, double iMaxLon)
        {
            if ((mMaxLat > iMinLat) && (mMinLon < iMaxLon) &&
                (mMinLat < iMaxLat) && (mMaxLon > iMinLon))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public static bool BoxInBox(ICoordinateArea mca, ICoordinateArea ica)
        //{
        //    return BoxInBox(mminLat, mca.MaxLatitude, mminLon, mca.MaxLongitude, iminLat, ica.MaxLatitude, iminLon, ica.MaxLongitude);
        //}

        public static List<int> GetBoxIds(double minLat, double maxLat, double minLon, double maxLon, int mountLat, int mountLon)
        {
            List<int> ids = new List<int>();

            int tempMinLat = Convert.ToInt32(Math.Floor(minLat * 100));
            tempMinLat = tempMinLat - (tempMinLat % mountLat);
            if (minLat < 0)
            {
                tempMinLat -= mountLat;
            }

            int tempMinLon = Convert.ToInt32(Math.Floor(minLon * 100));
            tempMinLon = tempMinLon - (tempMinLon % mountLon);
            if (minLon < 0)
            {
                tempMinLon -= mountLon;
            }

            while (tempMinLat < maxLat * 100)
            {
                int tempLon = tempMinLon;
                while (tempLon < maxLon * 100)
                {
                    ids.Add(tempMinLat * 100000 + tempLon);
                    tempLon += mountLon;
                }
                tempMinLat += mountLat;
            }

            return ids;
        }

        //public static List<ICoordinate> GetCircleVertices(ICoordinate origin, double radius)
        //{
        //    double earthRadius = 6371;
        //    //latitude in radians
        //    double lat = (origin.Latitude * Math.PI) / 180;
        //    //longitude in radians
        //    double lon = (origin.Longitude * Math.PI) / 180;
        //    //angular distance covered on earth's surface
        //    double d = radius / earthRadius;
        //    List<ICoordinate> points = new List<ICoordinate>();
        //    for (int i = 0; i <= 360; i++)
        //    {
        //        ICoordinate point = new Coordinate(0, 0);
        //        double bearing = i * Math.PI / 180; //rad
        //        point.Latitude = Math.Asin(Math.Sin(lat) * Math.Cos(d) +
        //          Math.Cos(lat) * Math.Sin(d) * Math.Cos(bearing));
        //        point.Longitude = ((lon + Math.Atan2(Math.Sin(bearing) * Math.Sin(d) * Math.Cos(lat),
        //          Math.Cos(d) - Math.Sin(lat) * Math.Sin(point.Latitude))) * 180) / Math.PI;
        //        point.Latitude = (point.Latitude * 180) / Math.PI;
        //        points.Add(point);
        //    }
        //    return points;
        //}

        
        

        public static List<int> GetShapeBoxIds(List<ICoordinate> points, int boxLat, int boxLon)
        {
            double minLat = 90;
            double minLon = 180;
            double maxLat = -90;
            double maxLon = -180;
            foreach (ICoordinate point in points)
            {
                if (point.Latitude < minLat)
                {
                    minLat = point.Latitude;
                }
                if (point.Longitude < minLon)
                {
                    minLon = point.Longitude;
                }
                if (point.Latitude > maxLat)
                {
                    maxLat = point.Latitude;
                }
                if (point.Longitude > maxLon)
                {
                    maxLon = point.Longitude;
                }
            }
            return GetBoxIds(minLat, maxLat, minLon, maxLon, boxLat, boxLon);
        }

        ///// <summary>
        ///// Get Box by the circle
        ///// </summary>
        ///// <param name="circle">the circle</param>
        ///// <param name="boxLat">the boxLat</param>
        ///// <param name="boxLon">the boxLat</param>
        ///// <returns>the box ids</returns>
        //public static List<int> GetShapeBoxIds(ICircle circle, int boxLat, int boxLon)
        //{
        //    if (circle == null) return null;
        //    if (circle.Coordinates == null) return null;

        //    return GetShapeBoxIds(circle.Coordinates, boxLat, boxLon);
        //}

        //public static List<int> GetCircleBoxIds(ICoordinate origin, double radius, int boxLat, int boxLon)
        //{
        //    List<ICoordinate> points = GetCircleVertices(origin, radius);
        //    return GetShapeBoxIds(points, boxLat, boxLon);
        //}

        /// <summary>
        /// Get Near Point between line and point
        /// </summary>
        /// <param name="lc1"></param>
        /// <param name="lc2"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        //public static ICoordinate GetNearPoint(ICoordinate lc1, ICoordinate lc2, ICoordinate c)
        //{
        //    ICoordinate rc = new Coordinate();
        //    double k = (lc2.Longitude - lc1.Longitude) / (lc2.Latitude - lc1.Latitude);
        //    rc.Latitude = (k * k * lc1.Latitude + k * (c.Longitude - lc1.Longitude) + c.Latitude) / (k * k + 1);
        //    rc.Longitude = k * (rc.Latitude - lc1.Latitude) + lc1.Longitude;
        //    if (!((lc1.Latitude >= rc.Latitude && rc.Latitude >= lc2.Latitude) || (lc1.Latitude >= rc.Latitude && rc.Latitude >= lc2.Latitude)))
        //    {
        //        double d1 = Math.Abs(lc1.Latitude - rc.Latitude);
        //        double d2 = Math.Abs(lc2.Latitude - rc.Latitude);
        //        if (d1 < d2)
        //        {
        //            rc = lc1;
        //        }
        //        else
        //        {
        //            rc = lc2;
        //        }
        //    }
        //    return rc;
        //}

        //public static bool ConnectedCirclePolygon(ICircle circle, List<ICoordinate> coordinates)
        //{
        //    bool ret = false;
        //    if (ShapeMethods.PointInPolygon(coordinates, circle.Center.Latitude, circle.Center.Longitude))
        //    {
        //        ret = true;
        //    }
        //    else
        //    {
        //        int i = 0;
        //        int j = 1;
        //        int count = coordinates.Count;
        //        while (i < count)
        //        {
        //            ICoordinate c = GetNearPoint(coordinates[i], coordinates[j], circle.Center);
        //            if (GeoCodeCalc.CalcDistance(c.Latitude, c.Longitude, circle.Center.Latitude, circle.Center.Longitude, GeoCodeCalcMeasurement.Miles) < circle.Radius)
        //            {
        //                ret = true;
        //                break;
        //            }
        //            i++;
        //            j = (i + 1) % count;
        //        }
        //    }
        //    return ret;
        //}


        public static class GeoCodeCalc
        {
            public const double EarthRadiusInMiles = 3956.0;//3956.0;
            public const double EarthRadiusInKilometers = 6371.0;//6367.0;
            public static double ToRadian(double val) { return val * (Math.PI / 180); }
            public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }
            /// <summary>
            /// Calculate the distance between two geocodes. Defaults to using Miles.
            /// </summary>
            public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
            {
                return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
            }
            /// <summary>
            /// Calculate the distance between two geocodes.
            /// </summary>
            public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
            {
                double radius = GeoCodeCalc.EarthRadiusInMiles;
                if (m == GeoCodeCalcMeasurement.Kilometers) { radius = GeoCodeCalc.EarthRadiusInKilometers; }
                return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
            }
        }
        public enum GeoCodeCalcMeasurement : int
        {
            Miles = 0,
            Kilometers = 1
        }
    }
}
