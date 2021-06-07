using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Import.Reader
{
    class CBSAReader : ImportReader<MetropolitanCoreArea>
    {
        public CBSAReader(string dbfFilePath, string shpFilePath)
            : base(dbfFilePath, shpFilePath)
        {
        }
        public override MetropolitanCoreArea GetInstance(System.Data.Common.DbDataReader reader, ShpReader.Polygon polygon)
        {
            MetropolitanCoreArea area = new MetropolitanCoreArea();
            area.Code = reader["CBSA"].ToString();
            area.Name = reader["CBSA_NAME"].ToString();
            area.Type = reader["TYPE"].ToString();
            area.Status = reader["STATUS"].ToString();
            area.GEOCODE = reader["GEOCODE"].ToString();
            area.MinLongitude = polygon.box[0];
            area.MinLatitude = polygon.box[1];
            area.MaxLongitude = polygon.box[2];
            area.MaxLatitude = polygon.box[3];
            area.Latitude = (area.MinLatitude + area.MaxLatitude) / 2.0;
            area.Longitude = (area.MinLongitude + area.MaxLongitude) / 2.0;

            int p = 0;
            for (int i = 0; i < polygon.numPoints; i++)
            {
                if (polygon.numParts - 1 > p && i >= polygon.parts[p + 1])
                {
                    p++;
                }
                MetropolitanCoreAreaCoordinate coord = new MetropolitanCoreAreaCoordinate();
                coord.MetropolitanCoreAreaId = area.Id;
                coord.ShapeId = p;
                coord.Latitude = polygon.points[i].Y * -1;
                coord.Longitude = polygon.points[i].X;
                area.MetropolitanCoreAreaCoordinates.Add(coord);
            }


            return area;
        }
    }
}
