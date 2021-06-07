// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System.ComponentModel;
using PietschSoft.VE.Converters;

namespace PietschSoft.VE
{
    [TypeConverter(typeof(GenericConverter))]
    public class Route
    {
        public Route(object start, object end)
        {
            this._start = start;
            this._end = end;
        }

        public Route(object start, object end, string jsCallback)
        {
            this._start = start;
            this._end = end;
            this._jscallback = jsCallback;
        }

        public Route(object start, object end, DistanceUnit units, RouteType routeType)
        {
            this._start = start;
            this._end = end;
            this._units = units;
            this._routetype = routeType;
        }

        public Route(object start, object end, DistanceUnit units, RouteType routeType, string jsCallback)
        {
            this._start = start;
            this._end = end;
            this._units = units;
            this._routetype = routeType;
            this._jscallback = jsCallback;
        }

        private object _start;
        public object Start
        {
            get { return _start; }
        }

        private object _end;
        public object End
        {
            get { return _end; }
        }

        private DistanceUnit _units = DistanceUnit.Miles;
        public DistanceUnit Units
        {
            get { return _units; }
        }

        private RouteType _routetype = RouteType.Shortest;
        public RouteType RouteType
        {
            get { return _routetype; }
        }

        private string _jscallback;
        public string JSCallback
        {
            get { return _jscallback; }
        }

    }
}
