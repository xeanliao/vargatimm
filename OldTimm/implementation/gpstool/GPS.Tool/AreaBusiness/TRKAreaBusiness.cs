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
    public class TRKAreaBusiness
    {
        public static List<TractCoordinate> GetTRKAreaCoords(
                    List<ShpReader.Polygon> polygons,
                    ref int index)
        {
            List<TractCoordinate> trkCoordList = new List<TractCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    TractCoordinate trkCoord = new TractCoordinate();
                    trkCoord.TractId = index;
                    trkCoord.Longitude = Double.Parse(point.X.ToString());
                    trkCoord.Latitude = Double.Parse(point.Y.ToString());
                    trkCoordList.Add(trkCoord);
                }
                index++;
            }
            return trkCoordList;
        }

        public static List<Tract> GetTRKAreas(DataTable dt, ShpReader shpReader)
        {
            List<Tract> trkAreaList = new List<Tract>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                Tract trk = new Tract();
                trk.Code = dr["TRACT"].ToString();
                trk.Name = dr["NAME"].ToString();
                trk.StateCode = dr["STATE"].ToString();
                trk.CountyCode = dr["COUNTY"].ToString();
                trk.LSAD = dr["LSAD"].ToString();
                trk.LSADTrans = dr["LSAD_TRANS"].ToString();
                trk.MinLongitude = box[0];
                trk.MinLatitude = box[1];
                trk.MaxLongitude = box[2];
                trk.MaxLatitude = box[3];
                trk.Latitude = (trk.MinLatitude + trk.MaxLatitude) / 2.0;
                trk.Longitude = (trk.MinLongitude + trk.MaxLongitude) / 2.0;
                trkAreaList.Add(trk);
            }
            return trkAreaList;
        }
    }
}
