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
    public class ThreeZipBusiness
    {
        public static List<ThreeZipAreaCoordinate> GetThreeZipAreaCoords(
                    List<ShpReader.Polygon> polygons,
                    ref int index)
        {
            List<ThreeZipAreaCoordinate> threeZipCoordList = 
                new List<ThreeZipAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    ThreeZipAreaCoordinate threeZipCoord = new ThreeZipAreaCoordinate();
                    threeZipCoord.ThreeZipAreaId = index;
                    threeZipCoord.Longitude = Double.Parse(point.X.ToString());
                    threeZipCoord.Latitude = Double.Parse(point.Y.ToString());
                    threeZipCoordList.Add(threeZipCoord);
                }
                index++;
            }
            return threeZipCoordList;
        }

        public static List<ThreeZipArea> GetThreeZipAreas(
            DataTable dt, ShpReader shpReader,string stateID)
        {
            List<ThreeZipArea> ThreeZipList = new List<ThreeZipArea>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                ThreeZipArea threeZip = new ThreeZipArea();
                threeZip.Code = dr["ZCTA3"].ToString();
                threeZip.Name = dr["NAME"].ToString();
                threeZip.StateCode = stateID;
                threeZip.LSAD = dr["LSAD"].ToString();
                threeZip.LSADTrans = dr["LSAD_TRANS"].ToString();
                threeZip.MinLongitude = box[0];
                threeZip.MinLatitude = box[1];
                threeZip.MaxLongitude = box[2];
                threeZip.MaxLatitude = box[3];
                threeZip.Latitude = (threeZip.MinLatitude + threeZip.MaxLatitude) / 2.0;
                threeZip.Longitude = (threeZip.MinLongitude + threeZip.MaxLongitude) / 2.0;
                ThreeZipList.Add(threeZip);
            }
            return ThreeZipList;
        }
    }
}
