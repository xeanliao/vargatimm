using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Map
{
    public class GPSCoordinate:PietschSoft.VE.LatLong
    {
        public GPSCoordinate(double latitude, double longitude)
            : base(latitude, longitude)
        { }
    }
}
