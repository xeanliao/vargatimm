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
    public class UrbanAreaBusiness
    {
        public static List<UrbanAreaCoordinate> GetUrbanAreaCoords(
            List<ShpReader.Polygon> polygons,
            ref int index)
        {
            List<UrbanAreaCoordinate> urbanCoordList = new List<UrbanAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    UrbanAreaCoordinate urbanAreaCoord = new UrbanAreaCoordinate();
                    urbanAreaCoord.UrbanAreaId = index;
                    urbanAreaCoord.Longitude = Double.Parse(point.X.ToString());
                    urbanAreaCoord.Latitude = Double.Parse(point.Y.ToString());
                    urbanCoordList.Add(urbanAreaCoord);
                }
                index++;
            }
            return urbanCoordList;
        }

        public static List<UrbanArea> GetUrbanAreas(DataTable dt, ShpReader shpReader)
        {
            List<UrbanArea> urbanAreaList = new List<UrbanArea>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                UrbanArea urbanArea = new UrbanArea();
                urbanArea.Code = dr["UA"].ToString();
                urbanArea.Name = dr["NAME"].ToString();
                urbanArea.LSAD = dr["LSAD"].ToString();
                urbanArea.LSAD_TRAN = dr["LSAD_TRANS"].ToString();
                urbanArea.MinLongitude = box[0];
                urbanArea.MinLatitude = box[1];
                urbanArea.MaxLongitude = box[2];
                urbanArea.MaxLatitude = box[3];
                urbanArea.Latitude = (urbanArea.MinLatitude + urbanArea.MaxLatitude) / 2.0;
                urbanArea.Longitude = (urbanArea.MinLongitude + urbanArea.MaxLongitude) / 2.0;
                urbanAreaList.Add(urbanArea);
            }
            return urbanAreaList;
        }
    }
}
