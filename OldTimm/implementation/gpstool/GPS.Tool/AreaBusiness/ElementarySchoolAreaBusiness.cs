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
    public class ElementarySchoolAreaBusiness
    {
        public static List<ElementarySchoolAreaCoordinate> GetElementarySchoolAreaCoords(
            List<ShpReader.Polygon> polygons,
            ref int index)
        {
            List<ElementarySchoolAreaCoordinate> eleSchoolAreaCoordList =
                new List<ElementarySchoolAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    ElementarySchoolAreaCoordinate eleSchoolAreaCoord = 
                        new ElementarySchoolAreaCoordinate();
                    eleSchoolAreaCoord.ElementarySchoolAreaId = index;
                    eleSchoolAreaCoord.Longitude = Double.Parse(point.X.ToString());
                    eleSchoolAreaCoord.Latitude = Double.Parse(point.Y.ToString());
                    eleSchoolAreaCoordList.Add(eleSchoolAreaCoord);
                }
                index++;
            }
            return eleSchoolAreaCoordList;
        }

        public static List<ElementarySchoolArea> GetElementarySchoolAreas(
            DataTable dt,
            ShpReader shpReader,
            string stateId)
        {
            List<ElementarySchoolArea> eleSchoolAreaList =
                new List<ElementarySchoolArea>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                ElementarySchoolArea eleSchoolArea = new ElementarySchoolArea();
                eleSchoolArea.Code = dr["SD_E"].ToString();
                eleSchoolArea.Name = dr["NAME"].ToString();
                eleSchoolArea.StateCode =
                    dr["STATE"] == DBNull.Value ? stateId : dr["STATE"].ToString();
                eleSchoolArea.LSAD = dr["LSAD"].ToString();
                eleSchoolArea.LSAD_TRAN = dr["LSAD_TRANS"].ToString();
                eleSchoolArea.MinLongitude = box[0];
                eleSchoolArea.MinLatitude = box[1];
                eleSchoolArea.MaxLongitude = box[2];
                eleSchoolArea.MaxLatitude = box[3];
                eleSchoolArea.Latitude = (eleSchoolArea.MinLatitude + eleSchoolArea.MaxLatitude) / 2.0;
                eleSchoolArea.Longitude = (eleSchoolArea.MinLongitude + eleSchoolArea.MaxLongitude) / 2.0;
                eleSchoolAreaList.Add(eleSchoolArea);
            }

            return eleSchoolAreaList;
        }
    }
}
