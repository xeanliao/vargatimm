// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using AjaxControlToolkit;
using System.Web.UI;
using System.ComponentModel;
using PietschSoft.VE.Converters;

[assembly: System.Web.UI.WebResource("PietschSoft.VE.Map.js", "text/javascript")]

namespace PietschSoft.VE
{
    [ClientScriptResource("PietschSoft.VE.Map", "PietschSoft.VE.Map.js")]
    public class Map : ScriptControlBase
    {
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            //return base.GetScriptReferences();
            List<ScriptReference> sr = base.GetScriptReferences() as List<ScriptReference>;

            ScriptReference r = new ScriptReference();

            if (this.VEMapJSInclude.Length != 0)
                r.Path = this.VEMapJSInclude;
            else
                r.Path = "http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2";

            sr.Add(r);

            return sr;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager sm = ScriptManager.GetCurrent(this.Page);

            if (sm.IsInAsyncPostBack)
            {
                AsyncMapData mapData = new AsyncMapData();

                if (_ZoomDirty) mapData.ZoomLevel = this.Zoom;
                if (_LatLongDirty) mapData.Latitude = this.Latitude;
                if (_LatLongDirty) mapData.Longitude = this.Longitude;
                if (_MapStyleDirty) mapData.MapStyle = this.MapStyle;
                if (_MapModeDirty) mapData.MapMode = this.MapMode;
                if (_ShowDashboardDirty) mapData.ShowDashboard = this.ShowDashboard;
                if (_ShowFindControlDirty) mapData.ShowFindControl = this.ShowFindControl;
                if (_RouteDirty) mapData.Route = this.Route;

                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                string strSerialized = jss.Serialize(mapData);
                sm.RegisterDataItem(this, strSerialized);
            }
        }


        #region Properties

        /// <summary>
        /// This property allows you to specify an alternative MS Virtual Earth API JavaScript include, other than the one hosted by Microsoft.
        /// </summary>
        public string VEMapJSInclude
        {
            get
            {
                return (string)(ViewState["VEMapJSInclude"] ?? "");
            }
            set
            {
                ViewState["VEMapJSInclude"] = value;
            }
        }

        private LatLong _LatLong;
        private bool _LatLongDirty = false;
        /// <summary>
        /// A LatLong object that represents the center of the map.
        /// </summary>
        [ExtenderControlProperty, TypeConverter(typeof(GenericConverter))]
        public LatLong LatLong
        {
            get { return _LatLong; }
            set { _LatLong = value; _LatLongDirty = true; }
        }

        public double Latitude
        {
            get { return this._LatLong.Latitude; }
            set { if (this._LatLong == null) this._LatLong = new LatLong(0, 0); this._LatLong.Latitude = value; _LatLongDirty = true; }
        }

        public double Longitude
        {
            get { return this._LatLong.Longitude; }
            set { if (this._LatLong == null) this._LatLong = new LatLong(0, 0); this._LatLong.Longitude = value; _LatLongDirty = true; }
        }

        private int _Zoom = 4;
        private bool _ZoomDirty = false;
        /// <summary>
        /// The zoom level to display. Valid values range from 1 through 19. Default is 4.
        /// </summary>
        [ExtenderControlProperty, DefaultValue(4)]
        public int Zoom
        {
            get { return _Zoom; }
            set
            {
                int v = value;
                if (v < 1) v = 1;
                if (v > 19) v = 19;
                
                if (_Zoom != v) _ZoomDirty = true;

                _Zoom = v;
            }
        }

        private MapStyle _MapStyle = MapStyle.Road;
        private bool _MapStyleDirty = false;
        /// <summary>
        /// The style of the map; Road, Aerial, Hybrid or Oblique (Bird's eye).
        /// </summary>
        [ExtenderControlProperty, DefaultValue(MapStyle.Road)]
        public MapStyle MapStyle
        {
            get { return _MapStyle; }
            set { _MapStyle = value; _MapStyleDirty = true; }
        }

        private bool _Fixed;
        /// <summary>
        /// A Boolean value that specifies whether the map view is displayed as a fixed map that the user cannot change. Default is false.
        /// </summary>
        [ExtenderControlProperty, DefaultValue(false)]
        public bool Fixed
        {
            get { return _Fixed; }
            set { _Fixed = value; }
        }

        private MapMode _MapMode = MapMode.Mode2D;
        private bool _MapModeDirty = false;
        /// <summary>
        /// A MapMode enumerator that specifies whether to load the map in 2D or 3D mode. Default is MapMode.Mode2D.
        /// </summary>
        [ExtenderControlProperty, DefaultValue(MapMode.Mode2D)]
        public MapMode MapMode
        {
            get { return _MapMode; }
            set { _MapMode = value; _MapModeDirty = true; }
        }

        private bool _ShowSwitch = true;
        /// <summary>
        /// A boolean value that specifies whether to show the map mode switch on the dashboard control. Default is true (the switch is displayed).
        /// </summary>
        [ExtenderControlProperty, DefaultValue(true)]
        public bool ShowSwitch
        {
            get { return _ShowSwitch; }
            set { _ShowSwitch = value; }
        }

        private bool _ShowDashboard = true;
        private bool _ShowDashboardDirty = false;
        [ExtenderControlProperty, DefaultValue(true)]
        public bool ShowDashboard
        {
            get { return _ShowDashboard; }
            set { _ShowDashboard = value; _ShowDashboardDirty = true; }
        }

        private bool _ShowFindControl = false;
        private bool _ShowFindControlDirty = false;
        [ExtenderControlProperty, DefaultValue(false)]
        public bool ShowFindControl
        {
            get { return _ShowFindControl; }
            set { _ShowFindControl = value; _ShowFindControlDirty = true; }
        }

        private Route _Route;
        private bool _RouteDirty = false;
        /// <summary>
        /// Draws a route on the map
        /// </summary>
        [ExtenderControlProperty, TypeConverter(typeof(GenericConverter))]
        public Route Route
        {
            get { return _Route; }
            set { _Route = value; _RouteDirty = true; }
        }

        private List<Pushpin> _Pushpins = new List<Pushpin>();
        /// <summary>
        /// A collection of Pushpin objects.
        /// </summary>
        [ExtenderControlProperty, TypeConverter(typeof(GenericConverter))]
        public List<Pushpin> Pushpins
        {
            get { return _Pushpins; }
            set { _Pushpins = value; }
        }

        private List<Polyline> _Polylines = new List<Polyline>();
        /// <summary>
        /// A collection of Polyline objects.
        /// </summary>
        [ExtenderControlProperty, TypeConverter(typeof(GenericConverter))]
        public List<Polyline> Polylines
        {
            get { return _Polylines; }
            set { _Polylines = value; }
        }

        private List<Polygon> _Polygons = new List<Polygon>();
        [ExtenderControlProperty, TypeConverter(typeof(GenericConverter))]
        public List<Polygon> Polygons
        {
            get { return _Polygons; }
            set { _Polygons = value; }
        }

        private List<LayerSpecification> _Layers = new List<LayerSpecification>();
        /// <summary>
        /// A collection of LayerSpecification objects.
        /// </summary>
        [ExtenderControlProperty, TypeConverter(typeof(GenericConverter))]
        public List<LayerSpecification> Layers
        {
            get { return _Layers; }
            set { _Layers = value; }
        }

        private List<TileSourceSpecification> _TileSources = new List<TileSourceSpecification>();
        /// <summary>
        /// A collection of TileSourceSpecification objects.
        /// </summary>
        [ExtenderControlProperty, TypeConverter(typeof(GenericConverter))]
        public List<TileSourceSpecification> TileSources
        {
            get { return _TileSources; }
            set { _TileSources = value; }
        }
	
        #endregion

        #region GetRoute Methods

        /// <summary>
        /// Draws a route on the map
        /// </summary>
        /// <param name="start">The start location. This can be a string value of an address, a place name, or a LatLong object that specifies the start location.</param>
        /// <param name="end">The ending location. This can be a string value of an address, a place name, or a LatLong object that specifies the end location.</param>
        public void GetRoute(object start, object end)
        {
            this._Route = new Route(start, end);
        }

        /// <summary>
        /// Draws a route on the map.
        /// </summary>
        /// <param name="start">The start location. This can be a string value of an address, a place name, or a LatLong object that specifies the start location.</param>
        /// <param name="end">The ending location. This can be a string value of an address, a place name, or a LatLong object that specifies the end location.</param>
        /// <param name="units">A DistanceUnit value that specifies either miles or kilometers.</param>
        /// <param name="routeType">A RouteType value specifying either the shortest route or the quickest route.</param>
        public void GetRoute(object start, object end, DistanceUnit units, RouteType routeType)
        {
            this._Route = new Route(start, end, units, routeType);
        }

        /// <summary>
        /// Draws a route on the map and return details on the Client-Side in a VERoute object to the defined Callback function.
        /// </summary>
        /// <param name="start">The start location. This can be a string value of an address, a place name, or a LatLong object that specifies the start location.</param>
        /// <param name="end">The ending location. This can be a string value of an address, a place name, or a LatLong object that specifies the end location.</param>
        /// <param name="units">A DistanceUnit value that specifies either miles or kilometers.</param>
        /// <param name="routeType">A RouteType value specifying either the shortest route or the quickest route.</param>
        /// <param name="jsCallback">Specifies the Client-Side JavaScript function to call when the VERoute object is returned.</param>
        public void GetRoute(object start, object end, DistanceUnit units, RouteType routeType, string jsCallback)
        {
            this._Route = new Route(start, end, units, routeType, jsCallback);
        }

        /// <summary>
        /// Clears the route from the map.
        /// </summary>
        public void DeleteRoute()
        {
            this._Route = null;
        }

        #endregion
    }
}
