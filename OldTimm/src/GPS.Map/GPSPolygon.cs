using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PietschSoft.VE;

namespace GPS.Map
{
    public class GPSPolygon:PietschSoft.VE.Polygon
    {
        #region Property
        /// <summary>
        /// Bounding Box --- Xmin,Ymin,Xmax,Ymax
        /// </summary>
        public List<double> box;
        /// <summary>
        /// Integer Parts Number
        /// </summary>
        public int numParts;
        /// <summary>
        /// point total
        /// </summary>
        public int numPoints;
        /// <summary>
        /// 
        /// </summary>
        public List<int> parts;

        #endregion

        #region Constructor
        public GPSPolygon() { }

        public GPSPolygon(string id, 
            List<LatLong> locations, 
            Color fillColor, 
            Color outlineColor, 
            int outlineWidth)
            :base(id,locations,fillColor,outlineColor,outlineWidth)
        {
            //this.Id = id;
            //this.Locations = locations;
            //this.FillColor = fillColor;
            //this.OutlineColor = outlineColor;
            //this.OutlineWidth = outlineWidth;
        }
        #endregion
    }
}
