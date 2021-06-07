
GPS.AreaStore = function() {

}

/***************************************************
class GPS.MapPanel
***************************************************/

GPS.Map.MapPanel = function(div) {
    this._div = null;
    this._mapObj = null;
    this._areaLayers = null;
    this._Init(div);
};

GPS.Map.MapPanel.prototype = {
    //Constructor
    _Init: function(div) {
        this._div = div;
        this._InitMapObj();
    },
    //Init VEMap Object
    _InitMapObj: function() {
        var mapDiv = $("<div id=\"" + this._div + "-map-container\" class=\"mapobj\"></div>");
        $("#" + this._div).append($(mapDiv));
        this._mapObj = new VEMap(this._div + "-map-container");
        var bingMapKey = $("#bingMapKey").val();
        if (!bingMapKey) {
            alert("Please add bing map key");
        }
        this._mapObj.SetCredentials(bingMapKey);
        this._mapObj.LoadMap(new VELatLong(34.05, -118.27), 11);
        var thisObj = this;
        this._mapObj.AttachEvent("onchangeview", function(e) {
            thisObj._OnChangeView(e);
        });
    },
    _InitAreaLayers: function() {
        var i = 0;
        var llen = 13;
        while (i < 13) {
            i++;
        }
    },
    //VEMap onchangeview event method
    _OnChangeView: function(e) {

    },
    //Get Map Object
    GetMapObj: function() {
        return this._mapObj;
    },
    //Show Classification Areas
    ShowClassification: function(classifacation) {
    },
    //Hide Classification Areas
    HideClassification: function(classification) {
    }
};