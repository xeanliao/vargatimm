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
    public class SLDSenateAreaBusiness
    {
        public static List<UpperSenateAreaCoordinate> GetSLDSenateAreaCoordinates(
            List<ShpReader.Polygon> polygons,
            ref int index)
        {
            List<UpperSenateAreaCoordinate> senateAreaCoordList = 
                new List<UpperSenateAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    UpperSenateAreaCoordinate senateAreaCoord = 
                        new UpperSenateAreaCoordinate();
                    senateAreaCoord.UpperSenateAreaId = index;
                    senateAreaCoord.Longitude = Double.Parse(point.X.ToString());
                    senateAreaCoord.Latitude = Double.Parse(point.Y.ToString());
                    senateAreaCoordList.Add(senateAreaCoord);
                }
                index++;
            }
            return senateAreaCoordList;
        }

        public static List<UpperSenateArea> GetSLDSenateAreas(
            DataTable dt, ShpReader shpReader)
        {
            List<UpperSenateArea> senateAreaList = new List<UpperSenateArea>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                UpperSenateArea senateArea = new UpperSenateArea();
                senateArea.Code = dr["SLDU"].ToString();
                senateArea.Name = dr["NAME"].ToString();
                senateArea.StateCode = dr["STATE"].ToString();
                senateArea.GEO_ID = dr["GEO_ID"].ToString();
                senateArea.LSAD = dr["LSAD"].ToString();
                senateArea.LSAD_TRAN = dr["LSAD_TRANS"].ToString();
                senateArea.MinLongitude = box[0];
                senateArea.MinLatitude = box[1];
                senateArea.MaxLongitude = box[2];
                senateArea.MaxLatitude = box[3];
                senateArea.Latitude = (senateArea.MinLatitude + senateArea.MaxLatitude) / 2.0;
                senateArea.Longitude = (senateArea.MinLongitude + senateArea.MaxLongitude) / 2.0;
                senateAreaList.Add(senateArea);
            }
            return senateAreaList;
        }
    }
}
