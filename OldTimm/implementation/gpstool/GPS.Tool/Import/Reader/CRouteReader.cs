using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Import.Reader
{
    class CRouteReader : ImportReader<PremiumCRoute>
    {
        public CRouteReader(string dbfFilePath, string shpFilePath)
            : base(dbfFilePath, shpFilePath)
        {
        }
        public override PremiumCRoute GetInstance(System.Data.Common.DbDataReader reader, ShpReader.Polygon polygon)
        {
            PremiumCRoute cRoute = new PremiumCRoute();
            cRoute.FIPSSTCO = Convert.ToString(reader["FIPSSTCO"]);
            cRoute.GEOCODE = Convert.ToString(reader["GEOCODE"]);
            cRoute.ZIP = Convert.ToString(reader["ZIP"]);
            cRoute.CROUTE = Convert.ToString(reader["CROUTE"]);
            cRoute.STATE_FIPS = Convert.ToString(reader["STATE_FIPS"]);
            cRoute.STATE = Convert.ToString(reader["STATE"]);
            cRoute.COUNTY = Convert.ToString(reader["COUNTY"]);
            cRoute.ZIP_NAME = Convert.ToString(reader["ZIP_NAME"]);
            cRoute.HOME_COUNT = reader["HOME_COUNT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["HOME_COUNT"]);
            cRoute.BUSINESS_COUNT = reader["BUSINESS_C"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BUSINESS_C"]);
            cRoute.APT_COUNT = reader["APT_COUNT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["APT_COUNT"]);
            cRoute.TOTAL_COUNT = reader["TOTAL_COUN"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TOTAL_COUN"]);
            cRoute.MinLongitude = polygon.box[0];
            cRoute.MinLatitude = polygon.box[1];
            cRoute.MaxLongitude = polygon.box[2];
            cRoute.MaxLatitude = polygon.box[3];
            cRoute.Latitude = (cRoute.MinLatitude + cRoute.MaxLatitude) / 2.0;
            cRoute.Longitude = (cRoute.MinLongitude + cRoute.MaxLongitude) / 2.0;
            cRoute.IsEnabled = 1;
            cRoute.OTotal = 0;
            int p = 0;
            for (int i = 0; i < polygon.numPoints; i++)
            {
                if (polygon.numParts - 1 > p && i >= polygon.parts[p + 1])
                {
                    p++;
                }
                PremiumCRouteCoordinate coord = new PremiumCRouteCoordinate();
                coord.PreminumCRouteId = cRoute.ID;
                coord.ShapeId = p;
                coord.Latitude = polygon.points[i].Y * -1;
                coord.Longitude = polygon.points[i].X;
                cRoute.PremiumCRouteCoordinates.Add(coord);
            }
            return cRoute;
        }
    }
}
