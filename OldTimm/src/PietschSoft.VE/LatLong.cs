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
    /// Contains the latitude and longitude of a single point on the globe.
    /// </summary>
    [TypeConverter(typeof(GenericConverter))]
    public class LatLong
    {
        public LatLong(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        private double _latitude;
        /// <summary>
        /// Specifies the latitude of a single point on the globe.
        /// </summary>
        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        private double _longitude;
        /// <summary>
        /// Specifies the longitude of a single point on the globe.
        /// </summary>
        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
    }
}
