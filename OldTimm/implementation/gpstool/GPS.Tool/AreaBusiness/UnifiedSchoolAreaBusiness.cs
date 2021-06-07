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
    public class UnifiedSchoolAreaBusiness
    {
        public static List<UnifiedSchoolAreaCoordinate> GetUnifiedSchoolAreaCoords(
            List<ShpReader.Polygon> polygons,
            ref int index)
        {
            List<UnifiedSchoolAreaCoordinate> uniSchoolCoordList =
                new List<UnifiedSchoolAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    UnifiedSchoolAreaCoordinate unfiedSchoolAreaCoord = 
                        new UnifiedSchoolAreaCoordinate();
                    unfiedSchoolAreaCoord.UnifiedSchoolAreaId = index;
                    unfiedSchoolAreaCoord.Longitude = Double.Parse(point.X.ToString());
                    unfiedSchoolAreaCoord.Latitude = Double.Parse(point.Y.ToString());
                    uniSchoolCoordList.Add(unfiedSchoolAreaCoord);
                }
                index++;
            }
            return uniSchoolCoordList;
        }

        public static List<UnifiedSchoolArea> GetUnifiedSchoolAreas(
            DataTable dt, ShpReader shpReader,string stateId)
        {
            List<UnifiedSchoolArea> unifiedSchoolAreaList = 
                new List<UnifiedSchoolArea>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                UnifiedSchoolArea unifiedSchoolArea = new UnifiedSchoolArea();
                unifiedSchoolArea.Code = dr["SD_U"].ToString();
                unifiedSchoolArea.Name = dr["NAME"].ToString();
                unifiedSchoolArea.StateCode =
                    dr["STATE"] == DBNull.Value ? stateId : dr["STATE"].ToString();
                unifiedSchoolArea.LSAD = dr["LSAD"].ToString();
                unifiedSchoolArea.LSAD_TRAN = dr["LSAD_TRANS"].ToString();
                unifiedSchoolArea.MinLongitude = box[0];
                unifiedSchoolArea.MinLatitude = box[1];
                unifiedSchoolArea.MaxLongitude = box[2];
                unifiedSchoolArea.MaxLatitude = box[3];
                unifiedSchoolArea.Latitude = (unifiedSchoolArea.MinLatitude + unifiedSchoolArea.MaxLatitude) / 2.0;
                unifiedSchoolArea.Longitude = (unifiedSchoolArea.MinLongitude + unifiedSchoolArea.MaxLongitude) / 2.0;
                unifiedSchoolAreaList.Add(unifiedSchoolArea);
            }
            return unifiedSchoolAreaList;
        }
    }
}
