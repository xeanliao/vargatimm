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
    public class SecondarySchoolAreaBusiness
    {
        public static List<SecondarySchoolAreaCoordinate> GetSecondarySchoolAreaCoords(
            List<ShpReader.Polygon> polygons,
            ref int index)
        {
            List<SecondarySchoolAreaCoordinate> secSchoolAreaCoordList =
                new List<SecondarySchoolAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    SecondarySchoolAreaCoordinate secSchoolAreaCoord = 
                        new SecondarySchoolAreaCoordinate();
                    secSchoolAreaCoord.SecondarySchoolAreaId = index;
                    secSchoolAreaCoord.Longitude = Double.Parse(point.X.ToString());
                    secSchoolAreaCoord.Latitude = Double.Parse(point.Y.ToString());
                    secSchoolAreaCoordList.Add(secSchoolAreaCoord);
                }
                index++;
            }
            return secSchoolAreaCoordList;
        }

        public static List<SecondarySchoolArea> GetSecondarySchoolAreas(
            DataTable dt, ShpReader shpReader , string stateId)
        {
            List<SecondarySchoolArea> secSchoolAreaList = new List<SecondarySchoolArea>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                SecondarySchoolArea secSchoolArea = new SecondarySchoolArea();
                secSchoolArea.Code = dr["SD_S"].ToString();
                secSchoolArea.Name = dr["NAME"].ToString();
                secSchoolArea.StateCode = 
                    dr["STATE"] == DBNull.Value ? stateId : dr["STATE"].ToString();
                secSchoolArea.LSAD = dr["LSAD"].ToString();
                secSchoolArea.LSAD_TRAN = dr["LSAD_TRANS"].ToString();
                secSchoolArea.MinLongitude = box[0];
                secSchoolArea.MinLatitude = box[1];
                secSchoolArea.MaxLongitude = box[2];
                secSchoolArea.MaxLatitude = box[3];
                secSchoolArea.Latitude = (secSchoolArea.MinLatitude + secSchoolArea.MaxLatitude) / 2.0;
                secSchoolArea.Longitude = (secSchoolArea.MinLongitude + secSchoolArea.MaxLongitude) / 2.0;
                secSchoolAreaList.Add(secSchoolArea);
            }
            return secSchoolAreaList;
        }
    }
}
