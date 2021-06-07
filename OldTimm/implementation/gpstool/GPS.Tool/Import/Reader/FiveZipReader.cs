using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Import.Reader
{
    class FiveZipReader : ImportReader<FiveZipArea>
    {
        public FiveZipReader(string dbfFilePath, string shpFilePath)
            : base(dbfFilePath, shpFilePath)
        {
        }
        public override FiveZipArea GetInstance(System.Data.Common.DbDataReader reader, ShpReader.Polygon polygon)
        {
            FiveZipArea zip = new FiveZipArea();
            zip.Name = Convert.ToString(reader["ZIP"]);
            zip.Code = Convert.ToString(reader["ZIP"]);
            zip.LSAD = "Z5";
            zip.LSADTrans = "5-Digit ZCTA";
            zip.StateCode = Convert.ToString(reader["STATE"]);
            zip.MinLongitude = polygon.box[0];
            zip.MinLatitude = polygon.box[1];
            zip.MaxLongitude = polygon.box[2];
            zip.MaxLatitude = polygon.box[3];
            zip.Latitude = (zip.MinLatitude + zip.MaxLatitude) / 2.0;
            zip.Longitude = (zip.MinLongitude + zip.MaxLongitude) / 2.0;
            zip.IsEnabled = 1;
            zip.OTotal = 0;
            int p = 0;
            for (int i = 0; i < polygon.numPoints; i++)
            {
                if (polygon.numParts - 1 > p && i >= polygon.parts[p + 1])
                {
                    p++;
                }
                FiveZipAreaCoordinate coord = new FiveZipAreaCoordinate();
                coord.FiveZipAreaId = zip.Id;
                coord.ShapeId = p;
                coord.Latitude = polygon.points[i].Y * -1;
                coord.Longitude = polygon.points[i].X;
                zip.FiveZipAreaCoordinates.Add(coord);
            }

            return zip;
        }
    }
}
