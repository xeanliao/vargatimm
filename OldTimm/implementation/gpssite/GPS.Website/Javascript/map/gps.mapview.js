
/***************************************************
class GPS.MapView
***************************************************/

 

GPS.MapView = function(div) {
    this._view = null;
    this._layers = null;
}

GPS.MapView.prototype = {
    __Init__: function(div) {

        this.__InitLayers__();
    },

    __InitMapView__: function() {
        this._view = new VEMap(div);
        var bingMapKey = $("#bingMapKey").val();
        if (!bingMapKey) {
            alert("Please add bing map key");
        }
        this._view.SetCredentials(bingMapKey);
    },

    __InitLayers__: function() {
        var i = 0;
        while (i < 13) {
            this._layers[i] = new VEShapeLayer();
            map.AddShapeLayer(this._layers[i]);
            i++;
        }
    },

    __OnChangeView__: function(screen, zoomLevel) {

    },

    AddAreas: function(area) {

    }
}

/***************************************************
class GPS.MapViewLayer
***************************************************/

GPS.MapViewLayer = function(classification) {
    this._classification = null;
    this._layer = null;
    this._activeAreas = null;
    this._boxes = null;
    this._settings = null;
    this._visible = null;
}

GPS.MapViewLayer.prototype = {
    __Init__: function(classification) {
        this._layer = new VEShapeLayer();
        this._activeAreas = [];
        this._boxes = [];
        this._settings = GPS.Map.ClassificationSettings[this._classification];
        this._visible = false;
    },

    GetClassification: function() { return this._classification; },

    GetShapeLayer: function() { return this._layer; },
    
    Display: function(zoomLevel) {
        if (this._visible && zoomLevel >= this._minZoomLevel && zoomLevel <= this._maxZoomLevel) {
            this._shapeLayer.Show();
        }
        else {
            this._shapeLayer.Hide();
        }
    },
}


GPS.AreaBoxPicker = function() {

}
