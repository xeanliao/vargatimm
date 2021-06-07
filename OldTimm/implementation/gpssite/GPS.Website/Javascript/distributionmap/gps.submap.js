

GPS.SubMap = function(submapObj) {
    this._isNew = null;
    this._id = null;
    this._name = null;
    this._color = null;
    this._colorString = null;
    this._changed = null;
    this._total = null;
    this._orderid = null;
    this._penetration = null;
    this._percentage = null;
    this._eventTrigger = null;
    this._dms = null;
    this._campaignid = null;
    this._areaRecorder = null;
    this._adjustTotal = null;
    this._adjustCount = null;
    this._centerLat = null;
    this._centerLon = null;
    this.__Init__(submapObj);
}

GPS.SubMap.prototype = {
    __Init__: function (submapObj) {
        if (submapObj) {
            this._isNew = true;
            this._id = submapObj.Id;
            this._name = submapObj.Name;
            this._total = submapObj.Total;
            this._orderid = submapObj.OrderId;
            this._penetration = submapObj.Penetration;
            this._percentage = submapObj.Percentage;
            this._adjustTotal = submapObj.TotalAdjustment;
            this._adjustCount = submapObj.CountAdjustment;
            this._centerLat = submapObj.CenterLatitude;
            this._centerLon = submapObj.CenterLongitude;
            this._color = { r: submapObj.ColorR, g: submapObj.ColorG, b: submapObj.ColorB };
            this._colorString = submapObj.ColorString;
            this._eventTrigger = new GPS.EventTrigger();
            this._dms = new GPS.DMList(submapObj.DistributionMaps);
            this._dms.GetDMRecords();
            this._campaignid = submapObj.CampaignId;
            this._eventTrigger = new GPS.EventTrigger();
            this._areaRecorder = new GPS.Map.SubMapAreaRecorder();
            this._classification = submapObj.Classification;
            if (submapObj.SubMapRecords) {
                this._areaRecorder.PushServerRecords(submapObj.SubMapRecords);
                this._InitShape(submapObj);
            }
        }
        else {
            this._isNew = true;
            this._id = Math.random().toString().replace('.', '');
            this._name = '';
            this._total = 0;
            this._orderid = 0;
            this._penetration = 0;
            this._percentage = 0;
            this._centerLat = 0;
            this._centerLon = 0;
            this._color = { r: 68, g: 68, b: 68 };
            this._colorString = '444444';
            this._adjustTotal = 0;
            this._adjustCount = 0;
            this._dms = new GPS.DMList();
            this._campaignid = null;
            this._eventTrigger = new GPS.EventTrigger();
            this._areaRecorder = new GPS.Map.SubMapAreaRecorder();
        }
    },

    GetId: function () { return this._id; },

    GetOrderId: function () { return this._orderid; },

    SetName: function (name) { this._name = name; },

    GetName: function () { return this._name; },

    SetColorString: function (colorString) { this._colorString = colorString; },

    GetColorString: function () { return this._colorString; },

    SetColor: function (color) { this._color = color; },

    GetColor: function () { return this._color; },

    GetTotal: function () { return this._total + this._adjustTotal; },

    GetPercentage: function () { return this._percentage; },

    GetPenetration: function () { return this._penetration + this._adjustCount; },

    AttachEvent: function (eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },


    _InitShape: function (shapeObj) {
        if (shapeObj.SubMapCoordinates.length > 0) {
            var points = [];
            for (var i = 0; i < shapeObj.SubMapCoordinates.length; i++) {
                points.push(new VELatLong(shapeObj.SubMapCoordinates[i].Latitude, shapeObj.SubMapCoordinates[i].Longitude));
            }
            this._shape = new VEShape(VEShapeType.Polyline, points);
            this._shape.SetFillColor(new VEColor(this._color.r, this._color.g, this._color.b, 0));
            this._shape.SetLineColor(new VEColor(this._color.r, this._color.g, this._color.b, 1));
            this._shape.SetLineWidth(10);
            this._shape.HideIcon();
            //            this._shape.SetZIndex(51, 1001);
            var layer = GetSubMapLayer();
            if (layer) { layer.AddShape(this._shape); }
        }
    }
}
