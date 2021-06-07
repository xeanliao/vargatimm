using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Data.Linq;

namespace GPS.Tool.Mapping
{
    public class ShapeMethods
    {
        public static bool PointInPolygon(List<ICoordinate> coordinates, double lat, double lon)
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
        public static bool BoxInBox(ICoordinateArea mca, ICoordinateArea ica)
        {
            return BoxInBox(mca.MinLatitude, mca.MaxLatitude, mca.MinLongitude, mca.MaxLongitude, ica.MinLatitude, ica.MaxLatitude, ica.MinLongitude, ica.MaxLongitude);
        }

        public static List<int> GetBoxIds(ICoordinateArea ca, int mountLat, int mountLon)
        {
            List<int> ids = new List<int>();

            int minLat = Convert.ToInt32(Math.Floor(ca.MinLatitude * 100));
            minLat = minLat - (minLat % mountLat);
            if (ca.MinLatitude < 0)
            {
                minLat -= mountLat;
            }

            if (ca.MinLongitude < -170 && ca.MaxLongitude > 170)
            {
                double temp = ca.MinLongitude;
                ca.MinLongitude = ca.MaxLongitude;
                ca.MaxLongitude = ca.MinLongitude + 360;
            }

            int minLon =Convert.ToInt32(Math.Floor(ca.MinLongitude * 100));
            minLon = minLon - (minLon % mountLon);
            if (ca.MinLongitude < 0)
            {
                minLon -= mountLon;
            }

            while (minLat < ca.MaxLatitude * 100)
            {
                int tempLon = minLon;
                while (tempLon < ca.MaxLongitude * 100)
                {
                    ids.Add(minLat * 100000 + tempLon);
                    tempLon += mountLon;
                }
                minLat += mountLat;
            }

            return ids;
        }
    }
}
