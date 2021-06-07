/*
*   Class AreaBox
*/
GPS.Map.AreaBox = function(id, classification) {
    this._symbol = "areabox";
    this._id = null;
    this._classification = null;
    this._areas = null;
    this._Init(id, classification);
};

GPS.Map.AreaBox.prototype = {
    // initalization AreaBox
    _Init: function(id, classification) {
        this._id = id;
        this._classification = classification;
        this._areas = new GPS.QArray();
    },
    // set container invoke symbol
    SetSymbol: function(symbol) {
        this._symbol = symbol;
    },
    // GetId
    GetId: function() {
        return this._id;
    },
    // load box
    LoadBox: function(shapeLayer, activeAreas) {
        var params = [];
        params.push("classification=" + this._classification);
        if (typeof (campaign) != 'undefined' && campaign != null) {
            params.push("campaign=" + campaign.Id);
        }
        params.push("box=" + this._id);
        params.push("times=" + GPS.Map.ClassificationSettings[this._classification].LoadTimes);
        var thisObj = this;
        $.ajax({
            type: "get",
            url: "Handler/maparea.ashx",
            data: params.join('&'),
            dataType: "json",
            success: function(areaObjs, textStatus) {
                var i = 0;
                var alen = areaObjs.length;
                while (i < alen) {
                    var area = activeAreas.Get(areaObjs[i].Id);
                    if (area == null) {
                        area = GPS.Container.Get(thisObj._symbol + ".area", areaObjs[i]);
                        activeAreas.Set(area.GetId(), area);
                        area.AddToShapeLayer(shapeLayer);
                    }
                    thisObj._areas.Set(area.GetId(), area);
                    area.AddReferenceBox();
                    i++;
                }
            }
        });
    },
    // add area by options
    AddArea: function(options, shapeLayer, activeAreas) {
        var area = activeAreas.Get(options.Id);
        if (area == null) {
            area = GPS.Container.Get(this._symbol + ".area", options);
            activeAreas.Set(area.GetId(), area);
            area.AddToShapeLayer(shapeLayer);
        }
        this._areas.Set(area.GetId(), area);
        area.AddReferenceBox();
    },
    // remove area by area id
    RemoveAreaById: function(areaId) {
        var area = this._areas.Get(areaId);
        if (area) {
            area.RemoveReferenceBox();
            this._areas.Set(areaId, null);
        }
    },
    // unload box
    UnLoad: function(shapeLayer, activeAreas) {
        var thisObj = this;
        thisObj.Layer = shapeLayer;
        this._areas.Each(function(i, area) {
            thisObj._areas.Set(area.GetId(), null);
            area.RemoveReferenceBox();
            if (area.GetReferenceBox() == 0) {
                area.DeleteFromShapeLayer(thisObj.Layer);
                activeAreas.Set(area.GetId(), null);
            }
        });
    }
};

GPS.Map.AreaBox.prototype.constructor = GPS.Map.AreaBox;