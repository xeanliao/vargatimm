// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using PietschSoft.VE.Converters;

namespace PietschSoft.VE
{
    /// <summary>
    /// Contains the information about a polygon on the map.
    /// </summary>
    [TypeConverter(typeof(GenericConverter))]
    public class Polygon
    {
        public Polygon()
        { }

        public Polygon(string id, List<LatLong> locations, PietschSoft.VE.Color fillColor, PietschSoft.VE.Color outlineColor, int outlineWidth)
        {
            this._id = id;
            this._locations = locations;
            this._fillcolor = fillColor;
            this._outlinecolor = outlineColor;
            this._outlinewidth = outlineWidth;
        }

        private string _id;
        /// <summary>
        /// The unique identifier of the polygon object that will be added to the map. The value for id must be unique for each polygon on the map control. Required.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private List<LatLong> _locations;
        /// <summary>
        /// An array of LatLong objects that specify all points of the polygon. Required.
        /// </summary>
        public List<LatLong> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }

        private PietschSoft.VE.Color _fillcolor;
        /// <summary>
        /// A Color object that specifies the color and transparency to use to fill the polygon.
        /// </summary>
        public PietschSoft.VE.Color FillColor
        {
            get { return _fillcolor; }
            set { _fillcolor = value; }
        }

        private PietschSoft.VE.Color _outlinecolor;
        /// <summary>
        /// A Color object that specifies the color and transparency to use fo the polygon's edges.
        /// </summary>
        public PietschSoft.VE.Color OutlineColor
        {
            get { return _outlinecolor; }
            set { _outlinecolor = value; }
        }

        private int _outlinewidth;
        /// <summary>
        /// The width of the line to draw, in pixels.
        /// </summary>
        public int OutlineWidth
        {
            get { return _outlinewidth; }
            set { _outlinewidth = value; }
        }

    }
}
