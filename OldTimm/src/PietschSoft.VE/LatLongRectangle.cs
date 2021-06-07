// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System.ComponentModel;
using PietschSoft.VE.Converters;

namespace PietschSoft.VE
{
    /// <summary>
    /// Contains LatLong objects that define the boundaries of the current map view.
    /// </summary>
    [TypeConverter(typeof(GenericConverter))]
    public class LatLongRectangle
    {
        public LatLongRectangle() { }

        public LatLongRectangle(LatLong topLeftLatLong, LatLong bottomRightLatLong)
        {
            this._TopLeftLatLong = topLeftLatLong;
            this._BottomRightLatLong = bottomRightLatLong;
        }

        private LatLong _TopLeftLatLong;
        /// <summary>
        /// A LatLong object that specifies the latitude and longitude of the upper-left corner of the map view.
        /// </summary>
        public LatLong TopLeftLatLong
        {
            get { return _TopLeftLatLong; }
            set { _TopLeftLatLong = value; }
        }

        private LatLong _BottomRightLatLong;
        /// <summary>
        /// A LatLong object that specifies the latitude longitude of the lower-right corner of the map view.
        /// </summary>
        public LatLong BottomRightLatLong
        {
            get { return _BottomRightLatLong; }
            set { _BottomRightLatLong = value; }
        }

        private LatLong _TopRightLatLong;
        /// <summary>
        /// If the map is in 3D mode, a LatLong object that specifies the latitude and longitude of the upper-right corder of the map view.
        /// </summary>
        public LatLong TopRightLatLong
        {
            get { return _TopRightLatLong; }
            set { _TopRightLatLong = value; }
        }

        private LatLong _BottomLeftLatLong;
        /// <summary>
        /// If the map is in 3D mode, a LatLong object that specifies the latitude and longitude of the lower-left corner of the map view.
        /// </summary>
        public LatLong BottomLeftLatLong
        {
            get { return _BottomLeftLatLong; }
            set { _BottomLeftLatLong = value; }
        }
	
    }
}
