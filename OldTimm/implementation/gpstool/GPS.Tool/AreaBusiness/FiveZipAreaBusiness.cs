using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using GPS.Tool;
using GPS.Tool.Data;

namespace GPS.Tool.AreaBusiness
{
    public class FiveZipAreaBusiness
    {
        public static List<FiveZipAreaCoordinate> GetFiveZipAreaCoords(
            List<ShpReader.Polygon> polygons,
            ref int index)
        {
            List<FiveZipAreaCoordinate> fiveZipCoordList = new List<FiveZipAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    FiveZipAreaCoordinate fiveZipAreaCoord = new FiveZipAreaCoordinate();
                    fiveZipAreaCoord.FiveZipAreaId = index;
                    fiveZipAreaCoord.Longitude = Double.Parse( point.X.ToString());
                    fiveZipAreaCoord.Latitude = Double.Parse(point.Y.ToString());
                    fiveZipCoordList.Add(fiveZipAreaCoord);
                }
                index++;
            }
            return fiveZipCoordList;
        }

        public static List<FiveZipArea> GetFiveZipAreas(
            DataTable dt,ShpReader shpReader,string stateID)
        {
            List<FiveZipArea> fiveZipAreaList = new List<FiveZipArea>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                FiveZipArea fiveZipArea = new FiveZipArea();
                Double[] box = shpReader.polygons[shapeId++].box;
                fiveZipArea.Code = dr["ZCTA"].ToString();
                fiveZipArea.Name = dr["NAME"].ToString();
                fiveZipArea.LSAD = dr["LSAD"].ToString();
                fiveZipArea.LSADTrans = dr["LSAD_TRANS"].ToString();
                fiveZipArea.StateCode = stateID;
                fiveZipArea.MinLongitude = box[0];
                fiveZipArea.MinLatitude = box[1];
                fiveZipArea.MaxLongitude = box[2];
                fiveZipArea.MaxLatitude = box[3];
                fiveZipArea.Latitude = (fiveZipArea.MinLatitude + fiveZipArea.MaxLatitude) / 2.0;
                fiveZipArea.Longitude = (fiveZipArea.MinLongitude + fiveZipArea.MaxLongitude) / 2.0;
                fiveZipAreaList.Add(fiveZipArea);
            }
            return fiveZipAreaList;
        }
    }
}
