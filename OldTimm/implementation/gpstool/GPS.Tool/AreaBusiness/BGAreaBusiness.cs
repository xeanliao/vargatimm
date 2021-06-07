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
    public class BGAreaBusiness
    {
        public static List<BlockGroupCoordinate> GetBGAreaCoords(
                    List<ShpReader.Polygon> polygons,
                    ref int index)
        {
            List<BlockGroupCoordinate> bgCoordList = new List<BlockGroupCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    BlockGroupCoordinate bgCoord = new BlockGroupCoordinate();
                    bgCoord.BlockGroupId = index;
                    bgCoord.Longitude = Double.Parse(point.X.ToString());
                    bgCoord.Latitude = Double.Parse(point.Y.ToString());
                    bgCoordList.Add(bgCoord);
                }
                index++;
            }
            return bgCoordList;
        }

        public static List<BlockGroup> GetBGAreas(DataTable dt,ShpReader shpReader)
        {
            List<BlockGroup> bgAreaList = new List<BlockGroup>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                BlockGroup bg = new BlockGroup();                
                bg.Code = dr["BLKGROUP"].ToString();
                bg.Name = dr["NAME"].ToString();
                bg.StateCode = dr["STATE"].ToString();
                bg.CountyCode = dr["COUNTY"].ToString();
                bg.TractCode = dr["TRACT"].ToString();
                bg.LSAD = dr["LSAD"].ToString();
                bg.LSADTrans = dr["LSAD_TRANS"].ToString();
                bg.MinLongitude = box[0];
                bg.MinLatitude = box[1];
                bg.MaxLongitude = box[2];
                bg.MaxLatitude = box[3];
                bg.Latitude = (bg.MinLatitude + bg.MaxLatitude) / 2.0;
                bg.Longitude = (bg.MinLongitude + bg.MaxLongitude) / 2.0;
                bgAreaList.Add(bg);
            }
            return bgAreaList;
        }
    }
}
