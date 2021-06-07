using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPS.Map;

namespace GPS.Website
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Page.IsPostBack)
            //{
                
            //}
        }

        private GPS.Map.GPSPolygon CreatePolygon(
            List<PietschSoft.VE.LatLong> coordinates)
        {
            return new GPS.Map.GPSPolygon(
                "p001", coordinates, GPSColor.Red, GPSColor.Green, 5);
        }

        protected void rbUpperClassifications_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void rbLowerClassifications_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filePath = @"/App_Data/z399_d00.shp";
            filePath = Server.MapPath(filePath);
            if (System.IO.File.Exists(filePath))
            {
                ShapeReader shape = new ShapeReader(filePath);
                shape.readShapeFile();

                for (int iLoop = 0; iLoop < 101; iLoop++)
                {
                    //PietschSoft.VE.Polygon polygon = 
                    //CreatePolygon(shape.polygons[iLoop].Locations);
                    GPS.Map.GPSPolygon polygon =
                        CreatePolygon(shape.polygons[iLoop].Locations);
                    polygon.Id = iLoop.ToString();
                    veGPSMap.Polygons.Add(polygon);
                    veGPSMap.LatLong = polygon.Locations[0];
                    veGPSMap.Zoom = 4;
                }
                shape = null;
            }
        }
    }
}
