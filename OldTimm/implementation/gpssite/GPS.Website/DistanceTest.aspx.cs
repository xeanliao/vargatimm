using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website
{
    public partial class DistanceTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            
            
            double lat1=-39.26; 
            double lng1=117.30; 
            double alt1=100; //meter
            double lat2=-30.40; 
            double lng2=120.51; 
            double alt2=100;
            double EARTH_RADIUS = 6378.137;//kilometer

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
        }

        public double rad(double d)
        {
            double degree, mintue, second;
            degree = Math.Truncate(d);
            d = (d - degree) * 100;
            mintue = Math.Truncate(d);
            second = (d - mintue) * 100;

            return (degree + mintue / 60.0 + second / 3600.0) * Math.PI / 180.0;
        }
    }
}
