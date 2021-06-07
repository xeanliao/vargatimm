using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Import.Reader
{
    class ZIPReader : ImportReader<PremiumZip>
    {
        public ZIPReader(string dbfFilePath, string shpFilePath)
            : base(dbfFilePath, shpFilePath)
        {
        }
        public override PremiumZip GetInstance(System.Data.Common.DbDataReader reader, ShpReader.Polygon polygon)
        {
            PremiumZip zip = new PremiumZip();
            zip.FIPSSTCO = Convert.ToString(reader["FIPSSTCO"]);
            zip.ZIP = Convert.ToString(reader["ZIP"]);
            zip.STATE_FIPS = Convert.ToString(reader["STATE_FIPS"]);
            zip.ZIP_NAME = Convert.ToString(reader["ZIP_NAME"]);
            zip.COUNTY = Convert.ToString(reader["COUNTY"]);
            zip.STATE = Convert.ToString(reader["STATE"]);
            zip.MinLongitude = polygon.box[0];
            zip.MinLatitude = polygon.box[1];
            zip.MaxLongitude = polygon.box[2];
            zip.MaxLatitude = polygon.box[3];
            zip.Latitude = (zip.MinLatitude + zip.MaxLatitude) / 2.0;
            zip.Longitude = (zip.MinLongitude + zip.MaxLongitude) / 2.0;
            zip.IsEnabled = true;
            zip.OTotal = 0;
            int p = 0;
            for (int i = 0; i < polygon.numPoints; i++)
            {
                if (polygon.numParts - 1 > p && i >= polygon.parts[p + 1])
                {
                    p++;
                }
                PremiumZipCoordinate coord = new PremiumZipCoordinate();
                coord.PreminumZipId = zip.ID;
                coord.ShapeId = p;
                coord.Latitude = polygon.points[i].Y * -1;
                coord.Longitude = polygon.points[i].X;
                zip.PremiumZipCoordinates.Add(coord);
            }

            return zip;
        }
    }
}
