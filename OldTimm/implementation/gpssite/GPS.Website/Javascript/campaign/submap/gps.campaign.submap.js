

GPS.Map.SubMap = function (submapObj) {
    this._isNew = null;
    this._uId;
    this._id = null;
    this._name = null;
    this._color = null;
    this._colorString = null;
    this._areaRecorder = null;
    this._submapAreas = null;
    this._changed = null;
    this._total = null;
    this._penetration = null;
    this._percentage = null;
    this._adjustTotal = null;
    this._adjustCount = null;
    this._eventTrigger = null;
    this._centerLat = null;
    this._centerLon = null;
    this._shape = null;
    this._hole = null;
    this._Init(submapObj);
}

GPS.Map.SubMap.prototype = {
    _Init: function (submapObj) {
        if (submapObj) {
            this._isNew = true;
            this._uId = submapObj.Id;
            this._id = submapObj.OrderId;
            this._name = submapObj.Name;
            this._total = submapObj.Total;
            this._penetration = submapObj.Penetration;
            this._adjustTotal = submapObj.TotalAdjustment;
            this._adjustCount = submapObj.CountAdjustment;
            this._percentage = submapObj.Percentage;
            this._centerLat = submapObj.CenterLatitude;
            this._centerLon = submapObj.CenterLongitude;
            this._color = { r: submapObj.ColorR, g: submapObj.ColorG, b: submapObj.ColorB };
            this._colorString = submapObj.ColorString;
            this._areaRecorder = new GPS.Map.SubMapAreaRecorder();
            this._submapAreas = new GPS.Map.SubMapAreas();
            this._eventTrigger = new GPS.EventTrigger();
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
            this._penetration = 0;
            this._percentage = 0;
            this._adjustTotal = 0;
            this._adjustCount = 0;
            this._percentage = 0;
            this._centerLat = 0;
            this._centerLon = 0;
            this._color = { r: 68, g: 68, b: 68 };
            this._colorString = '444444';
            this._areaRecorder = new GPS.Map.SubMapAreaRecorder();
            this._submapAreas = new GPS.Map.SubMapAreas();
            this._eventTrigger = new GPS.EventTrigger();
        }
    },

    SetOptions: function (options) {
        if (options.Id) {
            this._uId = options.Id;
        }
    },

    ToServerObj: function () {
        return {
            Id: this._uId,
            OrderId: this._id,
            Name: this._name,
            Total: this._total,
            Penetration: this._penetration,
            TotalAdjustment: this._adjustTotal,
            CountAdjustment: this._adjustCount,
            Percentage: this._percentage,
            ColorR: this._color.r,
            ColorG: this._color.g,
            ColorB: this._color.b,
            ColorString: this._colorString,
            CampaignId: campaign.Id
        }
    },

    _InitShape: function (shapeObj) {
        if (shapeObj.SubMapCoordinates.length > 0) {
            var points = [];
            for (var i = 0; i < shapeObj.SubMapCoordinates.length; i++) {
                points.push(new VELatLong(shapeObj.SubMapCoordinates[i].Latitude, shapeObj.SubMapCoordinates[i].Longitude));
            }
            this._shape = new VEShape(VEShapeType.Polygon, points);
            this._shape.SetFillColor(new VEColor(this._color.r, this._color.g, this._color.b, 0));
            this._shape.SetLineColor(new VEColor(this._color.r, this._color.g, this._color.b, 1));
            this._shape.SetLineWidth(10);
            this._shape.HideIcon();
            //            this._shape.SetZIndex(51, 1001);
            var layer = GetSubMapLayer();
            if (layer) { layer.AddShape(this._shape); }

            if (shapeObj.Holes && shapeObj.Holes.length > 0) {
                this.ShowHole(shapeObj.Holes);
            }
        }
    },

    GetUId: function () { return this._uId; },

    GetId: function () { return this._id; },

    SetName: function (name) { this._name = name; },

    GetName: function () { return this._name; },

    SetColorString: function (colorString) { this._colorString = colorString; },

    GetColorString: function () { return this._colorString; },

    SetColor: function (color) { this._color = color; },

    GetColor: function () { return this._color; },

    GetTotal: function () { return this._total + this._adjustTotal; },

    GetPercentage: function () { return this._percentage; },

    SetAdjustTotal: function (adjustTotal) {
        this._adjustTotal = Number(adjustTotal);
        this._eventTrigger.TriggerEvent('onareaschange', this);
    },

    GetAdjustTotal: function () { return this._adjustTotal; },

    GetAdjustCount: function () { return this._adjustCount; },

    SetAdjustCount: function (adjustCount) {
        this._adjustCount = Number(adjustCount);
        this._eventTrigger.TriggerEvent('onareaschange', this);
    },

    GetPenetration: function () { return this._penetration + this._adjustCount; },

    GetAreaRecorder: function () { return this._areaRecorder; },

    ContainsArea: function (area) {
        return this._areaRecorder.GetRecordByArea(area);
    },

    ContainsAreaObj: function (areaObj) {
        return this._areaRecorder.GetRecordByAreaObj(areaObj);
    },

    __UnmergeShape__: function (area) {
        var unMerged = false;
        if (this._shape) {
            var records = this._areaRecorder.GetMergeRecords(area, false);
            if (records.length > 0) {
                var thisObj = this;
                var backFun = function (data) {
                    var l = data.length;
                    if (l > 0 && l < 2) {
                        var ps = data[0];
                        var sps = [];
                        var j = 0;
                        var jl = ps.length;
                        while (j < jl) {
                            sps.push(new VELatLong(ps[j][0], ps[j][1]));
                            j++;
                        }
                        thisObj._submapShape.SetPoints(sps);
                    }
                };
                var merginpoints = GPS.Map.ShapeMethods.MergeAreas(records, backFun, null);
            }
            else {
                var layer = GetSubMapLayer();
                if (layer) { layer.DeleteShape(this._shape); }
                this._shape = null;
                unMerged = true;
                this.ShowHole(null);
            }
        }
        return unMerged;
    },

    RefreshPercentage: function () {
        if (this._total + this._adjustTotal > 0) { this._percentage = (this._penetration + this._adjustCount) / (this._total + this._adjustTotal); }
        else { this._percentage = 0; }
    },

    _AddArea: function (area, total, count, SignRecordArea) {
        var attributes = area.GetAttributes();

        this._total = Number(total);
        this._penetration = Number(count);
        this.RefreshPercentage();
        this._eventTrigger.TriggerEvent('onareaschange', this);

        this._areaRecorder.RecordArea(area, true);
        if (SignRecordArea) { SignRecordArea(area, true); }
    },

    ShowHole: function (holes) {
        //show hole
        var currentlayer = GetSubMapLayer();
        try {
            if (currentlayer && this._hole && this._hole.length > 0) {
                for (var i = 0; i < this._hole.length; i++) {
                    currentlayer.DeleteShape(this._hole[i]);
                }
            }
        }
        catch (e) {

        }
        this._hole = [];
        if (holes && holes.length > 0) {
            for (var i = 0; i < holes.length; i++) {
                var holePoints = [];
                for (var j = 0; j < holes[i].length; j++) {
                    holePoints.push(new VELatLong(holes[i][j][0], holes[i][j][1]));
                }
                var hole = new VEShape(VEShapeType.Polygon, holePoints);
                hole.SetFillColor(new VEColor(99, 0, 0, 0.5));
                hole.SetLineColor(new VEColor(254, 0, 0, 0.5));
                hole.SetLineWidth(3);
                hole.HideIcon();
                this._hole.push(hole);
            }
        }

        if (currentlayer && this._hole && this._hole.length > 0) {
            for (var i = 0; i < this._hole.length; i++) {
                currentlayer.AddShape(this._hole[i]);
            }
        }
    },

    AddArea: function (area, SignRecordArea) {
        var records = this._areaRecorder.GetMergeRecords(area, true);
        var thisObj = this;
        var backFun = function (data) {
            GPS.Loading.hide();
            var l = data ? data.Points.length : 0;

            if (l > 0) {


                var ps = data.Points;
                var sps = [];
                var j = 0;
                var jl = ps.length;
                while (j < jl) {
                    sps.push(new VELatLong(ps[j][0], ps[j][1]));
                    j++;
                }

                if (!thisObj._shape) {
                    thisObj._shape = new VEShape(VEShapeType.Polygon, sps);
                    thisObj._shape.SetFillColor(new VEColor(thisObj._color.r, thisObj._color.g, thisObj._color.b, 0));
                    thisObj._shape.SetLineColor(new VEColor(thisObj._color.r, thisObj._color.g, thisObj._color.b, 1));
                    thisObj._shape.SetLineWidth(10);
                    thisObj._shape.HideIcon();
                    var layer = GetSubMapLayer();
                    if (layer) { layer.AddShape(thisObj._shape); }


                }
                else {
                    thisObj._shape.SetPoints(sps);
                }

                thisObj._AddArea(area, data.Total, data.Count, SignRecordArea);
                thisObj.ShowHole(data.Holes);

                if (data.HaveNotMergedArea) {
                    GPSAlert("The selected shape is not connected to any submap group, cannot be added");
                }
            }
            else { GPSAlert("The selected shape is not connected to any submap group, cannot be added", SignRecordArea); }
        };
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        GPS.Loading.show();
        service.MergeAreas(campaign.Id, this._uId, records, false, backFun, GPS.Loading.hide);
    },

    AddAreas: function (areas, SignRecordArea) {
        var records = this._areaRecorder.GetBulkMergeRecords(areas, true);
        var thisObj = this;
        var backFun = function (data) {
            GPS.Loading.hide();
            var l = data ? data.Points.length : 0;

            if (l > 0) {
                var ps = data.Points;
                var sps = [];
                var j = 0;
                var jl = ps.length;
                while (j < jl) {
                    sps.push(new VELatLong(ps[j][0], ps[j][1]));
                    j++;
                }

                if (!thisObj._shape) {
                    thisObj._shape = new VEShape(VEShapeType.Polygon, sps);
                    thisObj._shape.SetFillColor(new VEColor(thisObj._color.r, thisObj._color.g, thisObj._color.b, 0));
                    thisObj._shape.SetLineColor(new VEColor(thisObj._color.r, thisObj._color.g, thisObj._color.b, 1));
                    thisObj._shape.SetLineWidth(10);
                    thisObj._shape.HideIcon();
                    var layer = GetSubMapLayer();
                    if (layer) { layer.AddShape(thisObj._shape); }

                }
                else {
                    thisObj._shape.SetPoints(sps);
                }

                for (var i in areas) {
                    var area = areas[i];
                    thisObj._AddArea(area, data.Total, data.Count, SignRecordArea);
                }
                thisObj.ShowHole(data.Holes);
            }
            else { GPSAlert("The selected shape is not connected to any submap group, cannot be added", SignRecordArea); }
        };
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        GPS.Loading.show();
        service.MergeAreas(campaign.Id, this._uId, records, true, backFun, GPS.Loading.hide);
    },

    _RemoveArea: function (area, total, count, SignRecordArea) {
        var attributes = area.GetAttributes();
        this._total = Number(total);
        this._penetration = Number(count);
        if (this._total > 0) { this._percentage = this._penetration / this._total; }
        else { this._percentage = 0; }
        this._eventTrigger.TriggerEvent('onareaschange', this);
        this._areaRecorder.RecordArea(area, false);
        this._submapAreas.RemoveArea(this._id, area._id);
        if (SignRecordArea) { SignRecordArea(area, false); }
    },

    __HasAreas__: function (records) {
        var has = false;
        for (var i in records) {
            if (records[i].Value) {
                has = true;
                break;
            }
        }
        return has;
    },

    RemoveArea: function (area, SignRecordArea) {
        if (this._shape) {
            var records = this._areaRecorder.GetMergeRecords(area, false);
            if (this.__HasAreas__(records)) {
                var thisObj = this;
                var backFun = function (data) {
                    GPS.Loading.hide();
                    var l = data ? data.Points.length : 0;

                    if (l > 0) {
                        var ps = data.Points;
                        var sps = [];
                        var j = 0;
                        var jl = ps.length;
                        while (j < jl) {
                            sps.push(new VELatLong(ps[j][0], ps[j][1]));
                            j++;
                        }
                        thisObj._shape.SetPoints(sps);
                        thisObj._RemoveArea(area, data.Total, data.Count, SignRecordArea);
                        thisObj.ShowHole(data.Holes);
                    }
                    else { GPSAlert("Unable remove this area.", SignRecordArea); }
                };
                var service = new TIMM.Website.CampaignServices.CampaignWriterService();
                GPS.Loading.show();
                service.MergeAreas(campaign.Id, this._uId, records, false, backFun, GPS.Loading.hide);
            }
            else {
                var service = new TIMM.Website.CampaignServices.CampaignWriterService();
                service.EmptySubmap(campaign.Id, this._uId);
                var layer = GetSubMapLayer();
                if (layer) { layer.DeleteShape(this._shape); }
                this._shape = null;
                this._RemoveArea(area, 0, 0, SignRecordArea);
                this.ShowHole(null);
            }
        }
    },

    RemoveAreas: function (areas, SignRecordArea) {
        if (this._shape) {
            var records = this._areaRecorder.GetBulkMergeRecords(areas, false);

            if (this.__HasAreas__(records)) {

                var thisObj = this;
                var backFun = function (data) {
                    GPS.Loading.hide();
                    var l = data ? data.Points.length : 0;

                    if (l > 0) {
                        var ps = data.Points;
                        var sps = [];
                        var j = 0;
                        var jl = ps.length;
                        while (j < jl) {
                            sps.push(new VELatLong(ps[j][0], ps[j][1]));
                            j++;
                        }
                        thisObj._shape.SetPoints(sps);

                        for (var i in areas) {
                            var area = areas[i];
                            thisObj._RemoveArea(area, data.Total, data.Count, SignRecordArea);
                        }
                        thisObj.ShowHole(data.Holes);
                    }
                    else { GPSAlert("Unable remove this area.", SignRecordArea); }
                };
                var service = new TIMM.Website.CampaignServices.CampaignWriterService();
                GPS.Loading.show();
                service.MergeAreas(campaign.Id, this._uId, records, false, backFun, GPS.Loading.hide);
            }
            else {

                var service = new TIMM.Website.CampaignServices.CampaignWriterService();
                service.EmptySubmap(campaign.Id, this._uId);
                var layer = GetSubMapLayer();
                if (layer) { layer.DeleteShape(this._shape); }
                this._shape = null;
                for (var i in areas) {
                    var area = areas[i];
                    this._RemoveArea(area, 0, 0, SignRecordArea);
                }
                this.ShowHole(null);

            }
        }
    },

    AttachEvent: function (eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    RemoveShape: function () {
        if (this._shape) {
            var layer = GetSubMapLayer();
            if (layer) { layer.DeleteShape(this._shape); }
            this._shape = null;
            //            this._RemoveArea(area);
        }
    },

    RefreshShape: function () {
        if (this._shape) {
            this._shape.SetFillColor(new VEColor(this._color.r, this._color.g, this._color.b, 0));
            this._shape.SetLineColor(new VEColor(this._color.r, this._color.g, this._color.b, 1));
        }
    }
}
