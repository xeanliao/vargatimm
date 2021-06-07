// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

Type.registerNamespace("PietschSoft.VE");

PietschSoft.VE.Map = function(element) {
    PietschSoft.VE.Map.initializeBase(this, [element]);
    // Properties
    this._Map=null;
    this._LatLong=null;
    this._Zoom="4";
    this._MapStyle="r";
    this._Fixed=false;
    this._MapMode=VEMapMode.Mode2D;
    this._ShowSwitch=true;
    this._ShowDashboard=true;
    this._ShowFindControl=false;
    this._Pushpins=null;
    this._Polylines=null;
    this._Polygons=null;
    this._TileSources=null;
    this._Layers=null;
    this._Route=null;
}

PietschSoft.VE.Map.prototype = {
    initialize : function() {
        PietschSoft.VE.Map.callBaseMethod(this, "initialize");
        
        //Add ScriptManager.PageLoading Handler
        strLoading='function PietschSoftVEMap_PageLoadingHandler(sender, args){PietschSoftVEMap_HandlePageLoadingDataItems("'+this.get_id()+'", args.get_dataItems());}';
        eval(strLoading);
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoading(PietschSoftVEMap_PageLoadingHandler);

        //Pushpin fix for Firefox
        var ffv=0;var ffn="Firefox/";var ffp=navigator.userAgent.indexOf(ffn);if(ffp!=-1){ffv=parseFloat(navigator.userAgent.substring(ffp+ffn.length));}
        if(ffv>=1.5){Msn.Drawing.Graphic.CreateGraphic=function(f,b){return new Msn.Drawing.SVGGraphic(f,b);};}
        
        //Do all other map stuff
        if(this._LatLong!=null){this._LatLong=Sys.Serialization.JavaScriptSerializer.deserialize(this._LatLong);}
        if(this._Pushpins!=null){this._Pushpins=Sys.Serialization.JavaScriptSerializer.deserialize(this._Pushpins);}
        if(this._Polylines!=null){this._Polylines=Sys.Serialization.JavaScriptSerializer.deserialize(this._Polylines);}
        if(this._Polygons!=null){this._Polygons=Sys.Serialization.JavaScriptSerializer.deserialize(this._Polygons);}
        if(this._TileSources!=null){this._TileSources=Sys.Serialization.JavaScriptSerializer.deserialize(this._TileSources);}
        if(this._Layers!=null){this._Layers=Sys.Serialization.JavaScriptSerializer.deserialize(this._Layers);}
       
        if(!isNaN(this._MapStyle)){if(this._MapStyle==2){this._MapStyle="a";}else if(this._MapStyle==3){this._MapStyle="h";}else if(this._MapStyle==4){this._MapStyle="o";}else{this._MapStyle="r";}}
        
        this.get_element().style.position="relative";
        this._Map=new VEMap(this.get_element().id);
        this._Map.LoadMap(new VELatLong(this._LatLong.Latitude,this._LatLong.Longitude),this._Zoom,this._MapStyle,this._Fixed,this._MapMode,this._ShowSwitch);
            
        if(this._ShowDashboard == false){this._Map.HideDashboard();}
        if(this._ShowFindControl == true){this._Map.ShowFindControl();}

        if(this._Route != null)
        {
            this._Route = Sys.Serialization.JavaScriptSerializer.deserialize(this._Route);
            if (this._Route.Units == 1){this._Route.Units = "k";}else{this._Route.Units = "m";}
            if (this._Route.RouteType == 1){this._Route.RouteType = "q";}else{this._Route.RouteType = "s";}
            if (this._Route.Start.indexOf("Latitude\":") != -1)
            {
                this._Route.Start = Sys.Serialization.JavaScriptSerializer.deserialize(this._Route.Start);
                this._Route.Start = new VELatLong(this._Route.Start.Latitude, this._Route.Start.Longitude);
            }
            if (this._Route.End.indexOf("Latitude\":") != -1)
            {
                this._Route.End = Sys.Serialization.JavaScriptSerializer.deserialize(this._Route.End);
                this._Route.End = new VELatLong(this._Route.End.Latitude, this._Route.End.Longitude);
            }
            if (this._Route.JSCallback != null) { if (this._Route.JSCallback.length == 0) this._Route.JSCallback = null; }
            this._Map.GetRoute(this._Route.Start, this._Route.End, this._Route.Units, this._Route.RouteType, eval(this._Route.JSCallback));
        }
        
        if(this._Pushpins != null) 
        {
            for (var i = 0; i < this._Pushpins.length; i++)
            {
                var pin = new VEPushpin(
                   this._Pushpins[i].Id, 
                   new VELatLong(this._Pushpins[i].Location.Latitude, this._Pushpins[i].Location.Longitude), 
                   this._Pushpins[i].Icon_url, 
                   this._Pushpins[i].Title,this._Pushpins[i].Details,
                   this._Pushpins[i].IconStyle,this._Pushpins[i].TitleStyle,this._Pushpins[i].DetailStyle
                );
                pin.ShowDetailOnMouseOver = this._Pushpins[i].ShowDetailOnMouseOver;
                pin.OnMouseOverCallback = this._Pushpins[i].OnMouseOverCallback;
                this._Map.AddPushpin(pin);
            }
        }

        if(this._Polylines.length != 0)
        {
            for (var i = 0; i < this._Polylines.length; i++)
            {
                var locations = new Array();
                for (var x = 0; x < this._Polylines[i].Locations.length; x++){locations[locations.length] = new VELatLong(this._Polylines[i].Locations[x].Latitude, this._Polylines[i].Locations[x].Longitude);}
                var polyline = new VEPolyline(this._Polylines[0].Id,locations);
                polyline.SetWidth(this._Polylines[i].Width);
                polyline.SetColor(this._Polylines[i].Color);
                this._Map.AddPolyline(polyline);
            }
        }
        
        if(this._Polygons.length != 0)
        {
            for (var i = 0; i < this._Polygons.length; i++)
            {
                var locations = new Array();
                for (var x = 0; x < this._Polygons[i].Locations.length; x++)
                {
                    locations[locations.length] = new VELatLong(this._Polygons[i].Locations[x].Latitude, this._Polygons[i].Locations[x].Longitude);
                }
                var polygon = new VEPolygon(
                    this._Polygons[i].Id,
                    locations,
                    this._Polygons[i].FillColor,
                    this._Polygons[i].OutlineColor,
                    this._Polygons[i].OutlineWidth
                );
                this._Map.AddPolygon(polygon);               
            }
        }
        
        if(this._TileSources.length != 0)
        {
            for (var i = 0; i < this._TileSources.length; i++)
            {
                var bounds = new Array();
                for (var a = 0; a < this._TileSources[i].Bounds.length; a++)
                {
                    bounds[bounds.length] = new VELatLongRectangle(
                        (this._TileSources[i].Bounds[a].TopLeftLatLong == null) ? null : new VELatLong(this._TileSources[i].Bounds[a].TopLeftLatLong.Latitude, this._TileSources[i].Bounds[a].TopLeftLatLong.Longitude),
                        (this._TileSources[i].Bounds[a].BottomRightLatLong == null) ? null : new VELatLong(this._TileSources[i].Bounds[a].BottomRightLatLong.Latitude, this._TileSources[i].Bounds[a].BottomRightLatLong.Longitude),
                        (this._TileSources[i].Bounds[a].TopRightLatLong == null) ? null : new VELatLong(this._TileSources[i].Bounds[a].TopRightLatLong.Latitude, this._TileSources[i].Bounds[a].TopRightLatLong.Longitude),
                        (this._TileSources[i].Bounds[a].BottomLeftLatLong == null) ? null : new VELatLong(this._TileSources[i].Bounds[a].BottomLeftLatLong.Latitude, this._TileSources[i].Bounds[a].BottomLeftLatLong.Longitude)
                    );
                }
                var ts = new VETileSourceSpecification();
                ts.ID = this._TileSources[i].Id;
                ts.TileSource = this._TileSources[i].TileSource;
                ts.NumServers = this._TileSources[i].NumServers;
                ts.Bounds = bounds;
                ts.MinZoom = this._TileSources[i].MinZoom;
                ts.MaxZoom = this._TileSources[i].MaxZoom;
                this._Map.AddTileSource(ts);
            }
        }
        
        if(this._Layers.length != 0)
        {
            for (var i = 0; i < this._Layers.length; i++)
            {
                var layer = new VELayerSpecification();
                if(this._Layers[i].Type == 0){this._Layers[i].Type = VELayerType.GeoRSS;}
                else if(this._Layers[i].Type == 1){this._Layers[i].Type = VELayerType.VECollection;}
                else if(this._Layers[i].Type == 2){this._Layers[i].Type = VELayerType.VETileSource;}
                layer.Type = this._Layers[i].Type;
                layer.ID = this._Layers[i].Id;
                if(this._Layers[i].IconUrl != null)
                {
                    if(this._Layers[i].IconUrl.length != 0)
                    {
                        layer.IconUrl = this._Layers[i].IconUrl;
                    }
                }
                layer.LayerSource = this._Layers[i].LayerSource;
                layer.Method = (this._Layers[i].Method==0)?"get":"post";
                if(this._Layers[i].Callback != null)
                {
                    if(this._Layers[i].Callback.length != 0)
                    {
                        layer.FnCallback = eval(this._Layers[i].Callback);
                    }
                }
                layer.ZIndex = this._Layers[i].ZIndex;
                layer.Opacity = this._Layers[i].Opacity;
                this._Map.AddLayer(layer);
            }
        }
        
    },
    SetMapStyle : function(v) {
        if(!isNaN(v)){if(v==2){v="a";}else if(v==3){v="h";}else if(v==4){v="o";}else{v="r";}}
        this.get_Map().SetMapStyle(v);
    },
    SetMapMode : function(v) {
        this.get_Map().SetMapMode(v);
    },
    dispose : function() {
        this._Map.Dispose();
        Sys.WebForms.PageRequestManager.getInstance().remove_pageLoading(PietschSoftVEMap_PageLoadingHandler);
        PietschSoft.VE.Map.callBaseMethod(this,"dispose");
    },
    get_Map:function(){return this._Map;},
    get_LatLong:function(){return this._LatLong;},
    set_LatLong:function(value){this._LatLong = value;},
    get_Zoom:function(){return this._Zoom;},
    set_Zoom:function(value){this._Zoom = value;},
    get_MapStyle:function(){return this._MapStyle;},
    set_MapStyle:function(value){this._MapStyle = value;},
    get_Fixed:function(){return this._Fixed;},
    set_Fixed:function(value){this._Fixed = value;},
    get_MapMode:function(){return this._MapMode;},
    set_MapMode:function(value){this._MapMode = value;},
    get_ShowSwitch:function(){return this._ShowSwitch;},
    set_ShowSwitch:function(value){this._ShowSwitch = value;},
    get_ShowDashboard:function(){return this._ShowDashboard;},
    set_ShowDashboard:function(value){this._ShowDashboard = value;},
    get_ShowFindControl:function(){return this._ShowFindControl;},
    set_ShowFindControl:function(value){this._ShowFindControl = value;},
    get_Pushpins:function(){return this._Pushpins;},
    set_Pushpins:function(value){this._Pushpins = value;},
    get_Polylines:function(){return this._Polylines;},
    set_Polylines:function(value){this._Polylines = value;},
    get_Polygons:function(){return this._Polygons;},
    set_Polygons:function(value){this._Polygons = value;},
    get_TileSources:function(){return this._TileSources;},
    set_TileSources:function(value){this._TileSources = value;},
    get_Layers:function(){return this._Layers;},
    set_Layers:function(value){this._Layers = value;},
    get_Route:function(){return this._Route;},
    set_Route:function(value){this._Route = value;}
}

PietschSoft.VE.Map.registerClass("PietschSoft.VE.Map", Sys.UI.Control);

function PietschSoftVEMap_HandlePageLoadingDataItems(str, dataItems)
{
    var m = $find(str);
    var map = m.get_Map();
    //alert("Map Zoom Level: " + map.GetZoomLevel());
//    //eval(dataItems[str]);
    //alert(dataItems[str]);
    
    var mapData = Sys.Serialization.JavaScriptSerializer.deserialize(dataItems[str]);
    if(mapData.ZoomLevel!=null)map.SetZoomLevel(mapData.ZoomLevel);
    if(mapData.Latitude!=null&&mapData.Longitude!=null)map.PanToLatLong(new VELatLong(mapData.Latitude,mapData.Longitude));
    if(mapData.MapStyle!=null)m.SetMapStyle(mapData.MapStyle);
    if(mapData.MapMode!=null)m.SetMapMode(mapData.MapMode);
    if(mapData.ShowDashboard!=null){if(mapData.ShowDashboard){map.ShowDashboard();}else{map.HideDashboard();}}
    if(mapData.ShowFindControl!=null){if(mapData.ShowFindControl){map.ShowFindControl();}else{map.HideFindControl();}}
    alert('Not Finished Yet');
    if(mapData.Route!=null){m.set_Route(mapData.Route);}
        
}

Sys.Application.notifyScriptLoaded();