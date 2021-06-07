
/*
* Class GPS.Map.MapDrawing - draw shapes
*
* Public methods:
* 		StartDrawing
*       SetEndDrawingFunction
*       GetPoints
*
* Constructor parameters:
*       @map - the VEMap object required
*/
GPS.Map.MapDrawing = function(map) {
    this._startDrawing = null;
    this._map = null;
    this._activeLine = null;
    this._points = null;
    this._fnEndDrawing = null;
    this._Init(map);
};

GPS.Map.MapDrawing.prototype = {
    //Constructor method
    _Init: function(map) {
        this._startDrawing = false;
        this._map = map;
        this._AttachMapEvents();
    },
    //attach map events
    _AttachMapEvents: function() {
        var thisObj = this;
        this._map.AttachEvent("onclick", function(e) {
            thisObj._OnClick(e);
        });
        this._map.AttachEvent("ondoubleclick", function(e) {
            return thisObj._OnDoubleClick(e);
        });
        this._map.AttachEvent("onmousemove", function(e) {
            thisObj._OnMouseMove(e);
        });
    },
    //map onclick event method
    _OnClick: function(e) {
        if (this._startDrawing) {
            var point = this._map.PixelToLatLong(new VEPixel(e.mapX, e.mapY));
            this._points.push(point);
        }
    },
    //map ondoubleclick event method
    _OnDoubleClick: function(e) {
        if (this._startDrawing) {
            var i = $.browser.mozilla ? 3 : 2;
            if (this._points.length > i) {
                var point = new VELatLong(this._points[0].Latitude, this._points[0].Longitude);
                this._points.push(point);
                this._EndDrawing();
            } else { GPSAlert("Points must > 2"); }
            return true;
        } else { return false; }
    },
    //map onmousemove event method
    _OnMouseMove: function(e) {
        if (this._startDrawing && this._points.length > 0) {
            var point = this._map.PixelToLatLong(new VEPixel(e.mapX, e.mapY));
            var temppoints = this._points.concat();
            temppoints.push(point);
            if (!this._activeLine) {
                this._activeLine = new VEShape(VEShapeType.Polyline, temppoints);
                this._activeLine.HideIcon();
                this._map.AddShape(this._activeLine);
            }
            else {
                this._activeLine.SetPoints(temppoints);
            }
        }
    },
    //start drawing
    StartDrawing: function() {
        this._oldPoint = null;
        this._points = [];
        this._startDrawing = true;
    },
    //end drawing
    _EndDrawing: function() {
        if (this._fnEndDrawing) {
            this._fnEndDrawing();
        }
        if (this._activeLine) {
            this._map.DeleteShape(this._activeLine);
            this._activeLine = null;
        }
        this._startDrawing = false;

    },
    //set end drawing function
    SetEndDrawingFunction: function(endDrawing) {
        this._fnEndDrawing = endDrawing;
    },
    //get points
    GetPoints: function() {
        return this._points;
    },
    //get points array
    GetPointsArray: function() {
        var arr = [];
        var i = 0;
        var plen = this._points.length;
        while (i < plen) {
            arr.push([this._points[i].Latitude, this._points[i].Longitude]);
            i++;
        }
        return arr;
    }
};
