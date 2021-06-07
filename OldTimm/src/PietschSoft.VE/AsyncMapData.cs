// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

namespace PietschSoft.VE
{
    /// <summary>
    /// The AyncMapData object is used for passing Map data down to the client when an Asynchronous Postback occurrs.
    /// </summary>
    public class AsyncMapData
    {
        private int? _ZoomLevel = null;
        public int? ZoomLevel
        {
            get { return _ZoomLevel; }
            set { _ZoomLevel = value; }
        }

        private double? _Latitude = null;
        public double? Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }

        private double? _Longitude = null;
        public double? Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }
        private MapStyle? _MapStyle = null;
        public MapStyle? MapStyle
        {
            get { return _MapStyle; }
            set { _MapStyle = value; }
        }
        private MapMode? _MapMode = null;
        public MapMode? MapMode
        {
            get { return _MapMode; }
            set { _MapMode = value; }
        }
        private bool? _ShowDashboard = null;
        public bool? ShowDashboard
        {
            get { return _ShowDashboard; }
            set { _ShowDashboard = value; }
        }
        private bool? _ShowFindControl = null;
        public bool? ShowFindControl
        {
            get { return _ShowFindControl; }
            set { _ShowFindControl = value; }
        }
        private Route _Route = null;
        public Route Route
        {
            get { return _Route; }
            set { _Route = value; }
        }
    }
}
