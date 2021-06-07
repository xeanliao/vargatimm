// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using PietschSoft.VE.Converters;

namespace PietschSoft.VE
{
    /// <summary>
    /// Contains the information about a line or collection of lines on the map.
    /// </summary>
    [TypeConverter(typeof(GenericConverter))]
    public class Polyline
    {
        public Polyline()
        {
            this.Locations = new List<LatLong>();
        }

        public Polyline(string id, List<LatLong> locations, PietschSoft.VE.Color color, int width)
        {
            this.Id = id;
            this.Locations = locations;
            this.Color = color;
            this.Width = width;
        }

        private string _id;
        /// <summary>
        /// The ID of the polyline that will be added to the map. The id value must be unique for each polyline on a map control.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private int _width;
        /// <summary>
        /// The width of the polyline.
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private PietschSoft.VE.Color _color;
        /// <summary>
        /// The color of the polyline, represented by a Color object.
        /// </summary>
        public PietschSoft.VE.Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private List<LatLong> _locations;

        /// <summary>
        /// An array of LatLong objects that specify all points of the polyline.
        /// </summary>
        public List<LatLong> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }
    }
}

