/*
*   Class GPS.Map.AreaLayerBase
*   
*/
GPS.Map.AreaLayerBase = GPS.EventTrigger.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function(options) {
        this._super(options);
        this._Define();
        this._InitAreaLayerBase(options);
    },
    // define basic attributes
    _Define: function() {
        this._symbol = "arealayerbase";
        this._classification = null;
        this._classificationSettings = null;
        this._visible = null;
        this._activeAreas = null;
        this._areaBoxes = null;
        this._shapeLayer = null;
        this._map = null;
    },
    //initalization method
    _InitAreaLayerBase: function(options) {
        if (options.Symbol) { this._symbol = options.Symbol; }
        this._classification = options.Classification;
        this._classificationSettings = GPS.Map.ClassificationSettings[this._classification];
        this._visible = false;
        this._activeAreas = new GPS.QArray();
        this._areaBoxes = new GPS.QArray();
        this._shapeLayer = new VEShapeLayer();
        this._map = options.Map;
        this._map.AddShapeLayer(this._shapeLayer);
        //        if (this._classification < 4) {
        var thisObj = this;
        this._map.AttachEvent("onchangeview", function(e) {
            thisObj._OnChangeView(e);
        });
        //        }
        this._OnChangeView();
    },
    // on map change view
    _OnChangeView: function(e) {
        var zoomLevel = this._map.GetZoomLevel();
        if (this._visible && zoomLevel >= this._classificationSettings.MinZoomLevel && zoomLevel <= this._classificationSettings.MaxZoomLevel) {
            this._LoadBoxes();
            this._shapeLayer.Show();
        }
        else {
            this._shapeLayer.Hide();
        }
    },
    // load boxes by view
    _LoadBoxes: function() {
        var boxes = this._GetScreenBoxes();
        var i = 0;
        var blen = boxes.length;
        while (i < blen) {
            boxes[i].SetSymbol(this._symbol);
            boxes[i].LoadBox(this._shapeLayer, this._activeAreas);
            this._areaBoxes.Set(boxes[i].GetId(), boxes[i]);
            i++;
        }
    },
    _UnLoadBoxes: function() {
        var thisObj = this;
        this._areaBoxes.Each(function(i, box) {
            box.UnLoad(thisObj._shapeLayer, thisObj._activeAreas);
            thisObj._areaBoxes.Set(i, null);
        });
    },
    //get screen box ids
    _GetScreenBoxes: function() {
        var boxes = [];
        var minLat = this._map.GetMapView().BottomRightLatLong.Latitude;
        var maxLat = this._map.GetMapView().TopLeftLatLong.Latitude;
        var minLon = this._map.GetMapView().TopLeftLatLong.Longitude;
        var maxLon = this._map.GetMapView().BottomRightLatLong.Longitude;
        var mountLat = this._classificationSettings.BoxLat;
        var mountLon = this._classificationSettings.BoxLon;

        var lat = Math.floor(minLat * 100);
        var lon = Math.floor(minLon * 100);
        lat = lat - (lat % mountLat);
        if (lat < 0) { lat -= mountLat; }
        lon = lon - (lon % mountLon);
        if (lon < 0) { lon -= mountLon; }

        while (lat < maxLat * 100) {
            var tempLon = lon;
            while (tempLon < maxLon * 100) {
                var id = lat * 100000 + tempLon;
                if (!this._areaBoxes.ContainsKey(id)) {
                    boxes.push(new GPS.Map.AreaBox(id, this._classification));
                }
                tempLon += mountLon;
            }
            lat += mountLat;
        }
        return boxes;
    },
    // get area by area id
    GetAreaById: function(areaId) {
        return this._activeAreas.Get(areaId);
    },
    // add area by options
    AddArea: function(options) {
        options.Classification = this._classification;
        var minLat = 90;
        var maxLat = -90;
        var minLon = 180;
        var maxLon = -180;
        var mountLat = this._classificationSettings.BoxLat;
        var mountLon = this._classificationSettings.BoxLon;
        var i = 0;
        var llen = options.Locations.length;
        while (i < llen) {
            if (options.Locations[i].Latitude < minLat) {
                minLat = options.Locations[i].Latitude;
            }
            if (options.Locations[i].Latitude > maxLat) {
                maxLat = options.Locations[i].Latitude;
            }
            if (options.Locations[i].Longitude < minLon) {
                minLon = options.Locations[i].Longitude;
            }
            if (options.Locations[i].Longitude > maxLon) {
                maxLon = options.Locations[i].Longitude;
            }
            i++;
        }
        var lat = Math.floor(minLat * 100);
        var lon = Math.floor(minLon * 100);
        lat = lat - (lat % mountLat);
        if (lat < 0) { lat -= mountLat; }
        lon = lon - (lon % mountLon);
        if (lon < 0) { lon -= mountLon; }
        while (lat < maxLat * 100) {
            var tempLon = lon;
            while (tempLon < maxLon * 100) {
                var id = lat * 100000 + tempLon;
                var box = this._areaBoxes.Get(id);
                if (box) {
                    box.AddArea(options, this._shapeLayer, this._activeAreas);
                }
                tempLon += mountLon;
            }
            lat += mountLat;
        }
    },
    // remove area by area id
    RemoveAreaById: function(areaId) {
        var area = this._activeAreas.Get(areaId);
        if (area) {
            this._areaBoxes.Each(function(i, box) {
                box.RemoveAreaById(areaId);
            });
            this._activeAreas.Set(areaId, null);
            this._shapeLayer.DeleteShape(area.GetShape());
        }
    },
    // get layer visible
    GetVisible: function() {
        return this._visible;
    },
    // set layer visible
    SetVisible: function(visible) {
        this._visible = visible;
        this._OnChangeView();
    },
    // reload areas
    ReLoad: function() {
        GPS.Map.ClassificationSettings[this._classification].LoadTimes = GPS.Map.ClassificationSettings[this._classification].LoadTimes + 1;
        this._UnLoadBoxes();
        this._OnChangeView();
    }
});

