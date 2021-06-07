
/*
*   Namespace GPS.Map
*/
GPS.Map = function() { }

/*  
*   Reference:
*   ~/Javascript/jquery-1.3.2.js
*   ~/Javascript/gps.js
*   ~/Javascript/gps.eventtrigger.js
*   ~/Javascript/gps.container.js
*
*   invoke symbol - mapbase
*   return symbol - map
*
*   Class GPS.Map.MapBase - GPS map base contains gps areas
*   @options - class { DivId: - map div id
*                      Center: - map center position (VELatLong calss)
*                      ZoomLevel: - map zoom level
*                    }
*
*/
GPS.Map.MapBase = GPS.EventTrigger.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function (options) {
        this._super(options);
        this._Define();
        this._InitMapBase(options);
    },
    // define basic attributes
    _Define: function () {
        this._map = null;
        this._symbol = "mapbase";
        this._areaLayers = null;
        this._areaLoader = null;
        this._navBar = null;        
    },
    //initalization method
    _InitMapBase: function (options) {
        if (options.Symbol) { this._symbol = options.Symbol; }
        this._InitMap(options);
        this._InitAreaLayers();
        this._InitNavBar(options);
    },

    // initalization map
    _InitMap: function (options) {
        var divId = options.DivId;
        var center = options.Center ? options.Center : new VELatLong(34.05, -118.27);
        var zoomLevel = options.ZoomLevel ? ZoomLevel : 11;
        // create map container div
        var mapDiv = $("<div id=\"" + divId + "-map-container\" class=\"mapobj\"></div>");
        $("#" + divId).append($(mapDiv));
        // create VEMap instance
        var bingMapKey = $("#bingMapKey").val();
        if (!bingMapKey) {
            alert("Please add bing map key");
        }
        this._map = new VEMap(divId + "-map-container");
        this._map.SetCredentials(bingMapKey);
        this._map.LoadMap(center, zoomLevel);
        // attach VEMap events
        var thisObj = this;
        this._map.AttachEvent("onchangeview", function (e) {
            thisObj._OnChangeView(e);
        });

        this._map.AttachEvent("onclick", function (e) {
            thisObj._OnClick(e);
        });

        this._map.AttachEvent("onmousedown", function (e) {
            thisObj._OnMouseDown(e);
        });

        this._map.AttachEvent("onmousemove", function (e) {
            thisObj._OnMouseMove(e);
        });

        this._map.AttachEvent("onmouseup", function (e) {
            thisObj._OnMouseUp(e);
        });

        this._map.SetMapStyle(VEMapStyle.Shaded);
    },
    // initalization area layers
    _InitAreaLayers: function () {
        this._areaLayers = [];
        var i = 0;
        var containerKey = this._symbol + ".arealayer";
        while (i < 16) {
            this._areaLayers.push(GPS.Container.Get(containerKey, {
                Classification: i,
                Map: this._map
            }));
            i++;
        }
    },
    // initalization navigation bar
    _InitNavBar: function (options) {
        this._navBar = new GPS.Map.NavBar(options);
        this._navBar.SetZoomLevel(this._map.GetZoomLevel());
        var thisObj = this;
        this._navBar.AttachEvent("onchangezoom", function (e) {
            thisObj._map.SetZoomLevel(e.ZoomLevel);
        });
        this._map.AttachEvent("onendzoom", function (e) {
            thisObj._navBar.SetZoomLevel(e.zoomLevel);
        });
    },
    //VEMap onchangeview event method
    _OnChangeView: function (e) { },
    //VEMap onchangeview event method
    _OnClick: function (e) { },

    _OnMouseDown: function (e) { },

    _OnMouseMove: function (e) { },

    _OnMouseUp: function (e) { },

    // get area by vemap element id
    _GetAreaByElementId: function (classifications, elementId) {
        var area = null;
        var shape = this._map.GetShapeByID(elementId);
        if (shape) {
            var i = 0;
            var clen = classifications.length;
            while (i < clen) {
                area = this._areaLayers[classifications[i]].GetAreaById(shape.GPSAreaId);
                if (area) {
                    break;
                }
                i++;
            }
        }
        return area;
    },
    // get area by vemap element id
    _GetAreaAndShapeIdByElementId: function (classifications, elementId) {
        var areaObj = null;
        var shape = this._map.GetShapeByID(elementId);
        if (shape) {
            var i = 0;
            var clen = classifications.length;
            while (i < clen) {
                var area = this._areaLayers[classifications[i]].GetAreaById(shape.GPSAreaId);
                if (area) {
                    areaObj = { Area: area, ShapeId: shape.GPSShapeId };
                    break;
                }
                i++;
            }
        }
        return areaObj;
    },
    // get area by areaId
    _GetArea: function (classification, areaId) {
        return this._areaLayers[classification].GetAreaById(areaId);
    },
    // Get VEMap Object
    GetMap: function () {
        return this._map;
    },
    // get classification area layer visible
    GetClassificationVisible: function (classification) {
        return this._areaLayers[Number(classification)].GetVisible();
    },
    // set classification area layer visible
    SetClassificationVisible: function (classification, visible) {
        this._areaLayers[Number(classification)].SetVisible(visible);
    },
    // reload areas
    Reload: function () {
        var clssifications = [1, 2, 3, 15];
        for (var i in clssifications) {
            this._areaLayers[clssifications[i]].ReLoad();
        }
    },
    // reload classification layer areas
    ReloadClassification: function (classification) {
        this._areaLayers[Number(classification)].ReLoad();
    },

    ResizeMap: function (width, height) {
        this._map.Resize(width, height);
    }
});

/*
*   Class GPS.Map.NavBar
*/
GPS.Map.NavBar = GPS.EventTrigger.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function(options) {
        this._super(options);
        this._Define();
        this._InitNavBar(options);
    },
    // define basic attributes
    _Define: function() {
        this._div = null;
    },
    // initalization nav bar
    _InitNavBar: function(options) {
        var classes = [{ Id: 0, Name: '3 ZIP' },
                     { Id: 1, Name: '5 ZIP' },
                     { Id: 2, Name: 'TRK' },
                     { Id: 3, Name: 'BG\'s'}];
        var divId = options.DivId;
        var div = $("<div id=\"" + divId + "-navbar\" class=\"mapnavbar\"></div>");
        var thisObj = this;
        $.each(classes, function(i, value) {
            if (i > 0) {
                $(div).append($("<div class=\"zoom-button-separator\"></div>"));
            }
            var button = $("<div id=\"" + divId + "-navbar-btn" + value.Id + "\" class=\"lower-classification-zoom-button\">" + value.Name + "</div>")
            $(button).click(function() {
                thisObj.TriggerEvent("onchangezoom", { ZoomLevel: GPS.Map.ClassificationSettings[value.Id].MinZoomLevel });
            });
            $(div).append($(button));
        });
        $('#' + divId).append($(div));
        this._div = div;
    },
    // set zoom level
    SetZoomLevel: function(zoomLevel) {
        $(this._div).find(".lower-classification-zoom-button").removeClass('lower-classification-zoom-button-select');
        var classification = -1;
        var i = 3;
        // find classification 
        while (i >= 0) {
            if (GPS.Map.ClassificationSettings[i].MinZoomLevel <= zoomLevel && GPS.Map.ClassificationSettings[i].MaxZoomLevel >= zoomLevel) {
                classification = i;
                break;
            }
            i--;
        }
        // change classification button selected 
        if (classification >= 0) {
            $("#" + $(this._div).attr("id") + "-btn" + classification).addClass('lower-classification-zoom-button-select');
        }
        $(this._div).hide();
        $(this._div).show();
    }
});

