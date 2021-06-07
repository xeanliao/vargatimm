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
    public class SLDHouseAreaBusiness
    {
        public static List<LowerHouseAreaCoordinate> GetSLDHouseAreaCoords(
            List<ShpReader.Polygon> polygons,
            ref int index)
        {
            List<LowerHouseAreaCoordinate> lowerHouseAreaCoordList =
                new List<LowerHouseAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    LowerHouseAreaCoordinate lowerHouseAreaCoord = 
                        new LowerHouseAreaCoordinate();
                    lowerHouseAreaCoord.LowerHouseAreaId = index;
                    lowerHouseAreaCoord.Longitude = Double.Parse(point.X.ToString());
                    lowerHouseAreaCoord.Latitude = Double.Parse(point.Y.ToString());
                    lowerHouseAreaCoordList.Add(lowerHouseAreaCoord);
                }
                index++;
            }
            return lowerHouseAreaCoordList;
        }

        public static List<LowerHouseArea> GetSLDHouseAreas(DataTable dt, ShpReader shpReader)
        {
            List<LowerHouseArea> lowerHouseAreaList = new List<LowerHouseArea>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                LowerHouseArea lowerHouseArea = new LowerHouseArea();
                lowerHouseArea.Code = dr["SLDL"].ToString();
                lowerHouseArea.Name = dr["NAME"].ToString();
                lowerHouseArea.StateCode = dr["STATE"].ToString();
                lowerHouseArea.GEO_ID = dr["GEO_ID"].ToString();
                lowerHouseArea.LSAD = dr["LSAD"].ToString();
                lowerHouseArea.LSAD_TRAN = dr["LSAD_TRANS"].ToString();
                lowerHouseArea.MinLongitude = box[0];
                lowerHouseArea.MinLatitude = box[1];
                lowerHouseArea.MaxLongitude = box[2];
                lowerHouseArea.MaxLatitude = box[3];
                lowerHouseArea.Latitude = (lowerHouseArea.MinLatitude + lowerHouseArea.MaxLatitude) / 2.0;
                lowerHouseArea.Longitude = (lowerHouseArea.MinLongitude + lowerHouseArea.MaxLongitude) / 2.0;
                lowerHouseAreaList.Add(lowerHouseArea);
            }
            return lowerHouseAreaList;
        }
    }
}
