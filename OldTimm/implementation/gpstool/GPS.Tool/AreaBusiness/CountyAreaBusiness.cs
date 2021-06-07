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
    public partial class CountyAreaBusiness
    {
        public static List<CountyAreaCoordinate> GetCountyAreaCoordinatas( 
            List<ShpReader.Polygon> polygons,
            ref int index)
        {
            List<CountyAreaCoordinate> countyAreaCoordList = new List<CountyAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    CountyAreaCoordinate countyAreaCoord = new CountyAreaCoordinate();
                    countyAreaCoord.CountyAreaId = index;
                    countyAreaCoord.Longitude = Double.Parse(point.X.ToString());
                    countyAreaCoord.Latitude = Double.Parse(point.Y.ToString());
                    countyAreaCoordList.Add(countyAreaCoord);
                }
                index++;
            }
            return countyAreaCoordList;
        }

        public static List<CountyArea> GetCountyAreas(DataTable dt,ShpReader shpReader)
        {
            List<CountyArea> countyAreaList = new List<CountyArea>();
            int shapeID = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeID++].box; 
                CountyArea countyArea = new CountyArea();
                countyArea.Code = dr["COUNTY"].ToString();
                countyArea.Name = dr["NAME"].ToString();
                countyArea.StateCode = dr["STATE"].ToString();
                countyArea.LSAD = dr["LSAD"].ToString();
                countyArea.LSAD_TRAN = dr["LSAD_TRANS"].ToString();
                countyArea.MinLongitude = box[0];
                countyArea.MinLatitude = box[1];
                countyArea.MaxLongitude = box[2];
                countyArea.MaxLatitude = box[3];
                countyArea.Latitude = (countyArea.MinLatitude + countyArea.MaxLatitude) / 2.0;
                countyArea.Longitude = (countyArea.MinLongitude + countyArea.MaxLongitude) / 2.0;
                countyAreaList.Add(countyArea);
            }
            return countyAreaList;
        }
    }
}
