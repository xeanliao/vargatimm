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
    public class CBSABusiness
    {
        public static List<MetropolitanCoreAreaCoordinate> GetCBSAAreaCoords(
                    List<ShpReader.Polygon> polygons,
                    ref int index)
        {
            List<MetropolitanCoreAreaCoordinate> cbsaCoordList = 
                new List<MetropolitanCoreAreaCoordinate>();
            foreach (ShpReader.Polygon polygon in polygons)
            {
                foreach (PointF point in polygon.points)
                {
                    MetropolitanCoreAreaCoordinate cbsaCoord = 
                        new MetropolitanCoreAreaCoordinate();
                    cbsaCoord.MetropolitanCoreAreaId = index;
                    cbsaCoord.Longitude = Double.Parse(point.X.ToString());
                    cbsaCoord.Latitude = Double.Parse(point.Y.ToString());
                    cbsaCoordList.Add(cbsaCoord);
                }
                index++;
            }
            return cbsaCoordList;
        }

        public static List<MetropolitanCoreArea> GetCBSAAreas(
            DataTable dt, ShpReader shpReader)
        {
            List<MetropolitanCoreArea> cbsaAreaList = new List<MetropolitanCoreArea>();
            int shapeId = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Double[] box = shpReader.polygons[shapeId++].box;
                MetropolitanCoreArea cbsa = new MetropolitanCoreArea();
                cbsa.Code = dr["CBSA"].ToString();
                cbsa.Name = dr["CBSA_NAME"].ToString();
                cbsa.Type = dr["TYPE"].ToString();
                cbsa.Status = dr["STATUS"].ToString();
                cbsa.GEOCODE = dr["GEOCODE"].ToString();
                cbsa.MinLongitude = box[0];
                cbsa.MinLatitude = box[1];
                cbsa.MaxLongitude = box[2];
                cbsa.MaxLatitude = box[3];
                cbsa.Latitude = (cbsa.MinLatitude + cbsa.MaxLatitude) / 2.0;
                cbsa.Longitude = (cbsa.MinLongitude + cbsa.MaxLongitude) / 2.0;
                cbsaAreaList.Add(cbsa);
            }
            return cbsaAreaList;
        }
    }
}
