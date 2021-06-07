using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Common;

namespace GTU.Utilities.ShapeMethods
{
    class ShapeMethods
    {
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
    }
}
