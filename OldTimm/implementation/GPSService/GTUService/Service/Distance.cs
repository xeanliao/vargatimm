using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMM.GTUService.Service
{
    public static class Distance
    {
        private static double EARTH_RADIUS = 6378.137;//earth average radius, kilometre 
        private static double rad(double d)
        {
            double degree, mintue, second;
            degree = Math.Truncate(d);
            d = (d - degree) * 100;
            mintue = Math.Truncate(d);
            second = (d - mintue) * 100;

            return (degree + mintue / 60.0 + second / 3600.0) * Math.PI / 180.0;
        }
        public static double CalcDist(double lat1, double lng1, double alt1, double lat2, double lng2, double alt2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double deltaHeight, minHeight;
            double bevel_distance;
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            minHeight = Math.Min(alt1, alt2);
            s = s * (EARTH_RADIUS + minHeight / 1000.0);
            deltaHeight = (alt2 - alt1) / 1000.0;  //height difference 
            bevel_distance = Math.Sqrt(s * s + deltaHeight * deltaHeight); // beeline distance 
            return bevel_distance;
        }
    }
}
