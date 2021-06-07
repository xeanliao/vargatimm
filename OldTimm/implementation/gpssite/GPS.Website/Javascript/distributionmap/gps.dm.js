/***************************************************
class GPS.DM
***************************************************/


var submapPanel = null;
var addressPanel = null;
var mapPanel = null;
var dmPanel = null;

function initMapPanel111() {
    GPS.Container.Register({
        Id: "campaignmap.arealayer",
        IsSingleton: false,
        ClassObject: GPS.DMMapPanel.AreaLayer,
        Instance: null
    });
    GPS.Container.Register({
        Id: "campaignarealayer.area",
        IsSingleton: false,
        ClassObject: GPS.DMMapPanel.Area,
        Instance: null
    });

    mapPanel = new GPS.DMMapPanel({
        DivId: "map-inner"
    });
}

function LoadCampaign111() {
    GPS.Loading.show();
    var args = GetUrlParms();
    var campaignId = args["cid"];
    if (campaignId) {
        //        var service = new TIMM.Website.CampaignServices.CampaignReaderService();
        //        service.GetCampaignByIdForDM(campaignId, function(toCampaign) {
        //            BindCampaign(toCampaign);
        //            if (toCampaign.SubMaps) {
        //                BindDMs(toCampaign.SubMaps.sort(function(a, b) { return a.OrderId - b.OrderId }));
        //            }
        //            GPS.Loading.hide();
        //            //setInterval("SaveCampaignStatus()", 20000);
        //        },
        //        function(e) {
        //            alert("too much data!")
        //            GPS.Loading.hide();
        //        }
        //        );
        var params = [];
        params.push("cid=" + campaignId);
        $.ajax({
            type: "get",
            url: "Handler/distributionmap.ashx",
            data: params.join('&'),
            dataType: "json",
            success: function (toCampaign, textStatus) {
                if (toCampaign) {
                    BindCampaign(toCampaign);
                    if (toCampaign.SubMaps) {
                        BindDMs(toCampaign.SubMaps.sort(function (a, b) {
                            return a.OrderId - b.OrderId
                        }));
                    }
                    /**
                    * init board
                    */
                    initNotIncludeArea(toCampaign.NotIncludeInSubMapArea);
                }
                GPS.Loading.hide();
            }
        });
    }
}

function initNotIncludeArea(area) {
    console.log('init map board');
    var board = $('#map-board ul');
    board.empty();
    if (area) {
        $.each(area, function () {
            if (this.Count > 0) {
                $('<li id="board-submap-' + this.Id + '"><span title="' + this.Name + '">' + this.Name + '</span><a class="button">' + this.Count + '</a></li>').data('NotIncludedArea', this).appendTo(board);
            } else {
                $('<li class="hide" id="board-submap-' + this.Id + '"><span title="' + this.Name + '">' + this.Name + '</span><a class="button">' + this.Count + '</a></li>').data('NotIncludedArea', this).appendTo(board);
            }
        });
    }
    $('#map-board ul').unbind('click');
    $('#map-board ul').bind('click', function (event) {
        var target = $(event.target);

        if (target.is('span')) {
            var li = target.closest('li');
            var data = li.data('NotIncludedArea');
            if (data && data.Areas) {
                var maxLat, minLat, maxLng, minLng;
                for (var i = 0; i < data.Areas.length; i++)
                    for (var j = 0; j < data.Areas[i].length; j++) {
                        maxLat = !maxLat || maxLat < data.Areas[i][j].Lat ? data.Areas[i][j].Lat : maxLat;
                        minLat = !minLat || minLat > data.Areas[i][j].Lat ? data.Areas[i][j].Lat : minLat;
                        maxLng = !maxLng || maxLng < data.Areas[i][j].Lng ? data.Areas[i][j].Lng : maxLng;
                        minLng = !minLng || minLng > data.Areas[i][j].Lng ? data.Areas[i][j].Lng : minLng;
                    }
                var topLeft = new VELatLong(maxLat, minLng);
                var bottomRight = new VELatLong(minLat, maxLng);
                bestView = new VELatLongRectangle(topLeft, bottomRight);
                mapPanel.GetMap().SetMapView(bestView);
            }
        } else if (target.is('a')) {
            var li = target.closest('li');
            if (li.hasClass('on')) {
                li.removeClass('on');
            } else {
                li.addClass('on');
            }
            showOrHideNotIncludeArea();
        }

    });
}

function updateNotIncludeAreas(area) {
    console.log(area);
    var board = $('#map-board ul');
    if (area) {
        $.each(area, function () {
            var item = board.find('#board-submap-' + this.Id);
            if (this.Count > 0) {
                item.data('NotIncludedArea', this);
                item.show();
                item.find('a').text(this.Count);
            } else {
                item.hide();
                item.data('NotIncludedArea', null);
            }
        });
    }
    showOrHideNotIncludeArea();
}
var _notIncludeAreas = [];

function showOrHideNotIncludeArea() {
    var currentlayer = GetSubMapLayer();
    try {
        if (currentlayer && _notIncludeAreas) {
            for (var i = 0; i < _notIncludeAreas.length; i++) {
                currentlayer.DeleteShape(_notIncludeAreas[i]);
            }
        }
    } catch (e) { }
    _notIncludeAreas = [];
    $('#map-board ul li.on').each(function () {
        var data = $(this).data('NotIncludedArea');
        if (data && data.Areas) {
            for (var i = 0; i < data.Areas.length; i++) {
                var leftPoints = [];
                for (var j = 0; j < data.Areas[i].length; j++) {
                    leftPoints.push(new VELatLong(data.Areas[i][j].Lat, data.Areas[i][j].Lng));
                }
                var leftArea = new VEShape(VEShapeType.Polygon, leftPoints);
                leftArea.SetLineColor(new VEColor(87, 31, 28, 0.75));
                leftArea.SetFillColor(new VEColor(87, 31, 28, 0.75));
                leftArea.SetLineWidth(1);
                leftArea.HideIcon();
                _notIncludeAreas.push(leftArea);
            }
        }
    });
    if (currentlayer && _notIncludeAreas && _notIncludeAreas.length > 0) {
        for (var i = 0; i < _notIncludeAreas.length; i++) {
            currentlayer.AddShape(_notIncludeAreas[i]);
        }
    }
}

//Init map panel
function initSubPannel() {
    //submapPanel = new GPS.SubMapPanel('sub-map-panel');
    addressPanel = new GPS.AddressPanel({
        MapObj: mapPanel.GetMap()
    });
    //$("#sub-map-container").tabs();
}

$(document).ready(function () {

    initLayout();
    initMenu();
    initClassificationColors();
    initMapPanel111();
    initSubPannel();
    LoadCampaign111();
    //LoadSubMap();
});


/**
* Called when the user clicks on the 'Log Out' menu item.
*/
function OnLogOutClick() {
    var data = "logout=true";
    $.ajax({
        type: "POST",
        url: "Handler/LoginHandler.ashx",
        data: data,
        success: function (msg) {
            window.open('login.html', '_self');
        }
    });
}


// Called when the user clicks the "Print" menu item.
function OnPrintClick(version) {
    if (DMExits()) {
        var url = "DMPrintSettings.aspx?campaign=" + campaign.Id;
        if (version && version == "silverlight") {
            url = "NewControlCenter.aspx?id=" + campaign.Id + "#DistributionMapPrint";
        } else if (version && version == 'phantomjs') {
            url = 'Handler/PhantomjsPrintHandler.ashx?type=dmap&campaignId=' + campaign.Id;
        }
        //window.location = url;    
        window.open(url, '_blank', 'resizable=yes,status=yes,toolbar=no,scrollbars=yes,menubar=no,location=no');
    } else
        GPSAlert("No distribution maps to print.");
}

function DMExits() {
    var dms = GetDMsList();
    var isDMExits = false;
    if (dms.length > 0) {
        for (var i = 0; i < dms.length; i++) {
            if (dms[i]._dms._dmrecords.length > 0) {
                isDMExits = true;
                break;
            }
        }
    }
    return isDMExits;
}


GPS.DM = function (dmObj) {
    this._isNew = null;
    //this._dmrecords = null;
    this._id = null;
    this._name = null;
    this._color = null;
    this._colorString = null;
    this._submapid = null;
    this._changed = null;
    this._dmAreas = null;
    this._total = null;
    this._penetration = null;
    this._adjustTotal = null;
    this._adjustCount = null;
    this._percentage = null;
    this._areaRecorder = null;
    this._eventTrigger = null;
    this._centerLat = null;
    this._centerLon = null;
    this.__Init__(dmObj);
}

GPS.DM.prototype = {
    __Init__: function (dmObj) {
        if (dmObj) {
            //this._dmrecords = [];
            this._isNew = true;
            this._id = dmObj.Id;
            this._name = dmObj.Name;
            this._color = {
                r: dmObj.ColorR,
                g: dmObj.ColorG,
                b: dmObj.ColorB
            };
            this._colorString = dmObj.ColorString;
            this._total = dmObj.Total;
            this._penetration = dmObj.Penetration;
            this._percentage = dmObj.Percentage;
            this._submapid = dmObj.SubMapId;
            this._centerLat = dmObj.CenterLatitude;
            this._centerLon = dmObj.CenterLongitude;
            this._adjustTotal = dmObj.TotalAdjustment;
            this._adjustCount = dmObj.CountAdjustment;
            this._areaRecorder = new GPS.Map.DMAreaRecorder();
            this._dmAreas = new GPS.Map.DMAreas();
            this._eventTrigger = new GPS.EventTrigger();
            this._holes = [];

            if (dmObj.DistributionMapRecords) {
                this._areaRecorder.PushServerRecords(dmObj.DistributionMapRecords);
                this._InitShape(dmObj);
            }
        } else {
            //this._dmrecords = [];
            this._isNew = true;
            this._id = Math.random().toString().replace('.', '');
            this._name = '';
            this._color = {
                r: 68,
                g: 68,
                b: 68
            };
            this._colorString = '444444';
            this._total = 0;
            this._orderid = 0;
            this._penetration = 0;
            this._percentage = 0;
            this._centerLat = 0;
            this._centerLon = 0;
            this._adjustTotal = 0;
            this._adjustCount = 0;
            this._areaRecorder = new GPS.Map.DMAreaRecorder();
            this._dmAreas = new GPS.Map.DMAreas();
            this._eventTrigger = new GPS.EventTrigger();
            this._holes = [];
        }
    },
    Clear: function () {
        //this._dmrecords = [];
    },


    GetId: function () {
        return this._id;
    },

    GetName: function () {
        return this._name;
    },

    GetSubMapId: function () {
        return this._submapid;
    },

    GetAreaRecorder: function () {
        return this._areaRecorder;
    },

    ContainsArea: function (area) {
        return this._areaRecorder.GetRecordByArea(area);
    },

    ContainsAreaObj: function (areaObj) {
        return this._areaRecorder.GetRecordByAreaObj(areaObj);
    },

    SetName: function (name) {
        this._name = name;
    },

    SetId: function (id) {
        this._id = id;
    },

    SetSumMapId: function (submapid) {
        this._submapid = submapid;
    },

    SetColorString: function (colorString) {
        this._colorString = colorString;
    },

    GetColorString: function () {
        return this._colorString;
    },

    SetColor: function (color) {
        this._color = color;
    },

    GetColor: function () {
        return this._color;
    },


    GetTotal: function () {
        return this._total;
    },

    GetPercentage: function () {
        return this._percentage;
    },

    GetPenetration: function () {
        return this._penetration;
    },


    AttachEvent: function (eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },



    _InitShape: function (shapeObj) {

        if (shapeObj.DistributionMapCoordinates.length > 0) {
            var points = [];
            for (var i = 0; i < shapeObj.DistributionMapCoordinates.length; i++) {
                points.push(new VELatLong(shapeObj.DistributionMapCoordinates[i].Latitude, shapeObj.DistributionMapCoordinates[i].Longitude));
            }
            this._shape = new VEShape(VEShapeType.Polygon, points);
            this._shape.SetFillColor(new VEColor(this._color.r, this._color.g, this._color.b, 0.7));
            this._shape.SetLineColor(new VEColor(this._color.r, this._color.g, this._color.b, 1));
            this._shape.SetLineWidth(5);
            this._shape.HideIcon();
            //            this._shape.SetZIndex(51, 1001);
            var layer = GetSubMapLayer();
            if (layer) {
                layer.AddShape(this._shape);
            }
        }
        if (shapeObj.Holes && shapeObj.Holes.length > 0) {
            this.ShowHole(shapeObj.Holes);
        }

    },

    //add area to dm
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
            } else {
                var layer = GetSubMapLayer();
                if (layer) {
                    layer.DeleteShape(this._shape);
                }
                this._shape = null;
                unMerged = true;
                this.ShowHole(null);
                updateNotIncludeAreas();
            }
        }
        return unMerged;
    },

    _AddArea: function (area, total, count, SignRecordArea) {
        //var attributes = area.GetAttributes();

        this._total = Number(total);
        this._penetration = Number(count);
        //this.RefreshPercentage();
        if (this._total > 0) {
            this._percentage = this._penetration / this._total;
        } else {
            this._percentage = 0;
        }
        this._eventTrigger.TriggerEvent('onareaschange', this);

        this._areaRecorder.RecordArea(area, true);
        if (SignRecordArea) {
            SignRecordArea(area, true);
        }
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
        } catch (e) {

        }
        this._hole = [];
        if (holes && holes.length > 0) {
            for (var i = 0; i < holes.length; i++) {
                var holePoints = [];
                for (var j = 0; j < holes[i].length; j++) {
                    holePoints.push(new VELatLong(holes[i][j][0], holes[i][j][1]));
                }
                var hole = new VEShape(VEShapeType.Polygon, holePoints);
                hole.SetLineColor(new VEColor(99, 0, 0, 0.5));
                hole.SetFillColor(new VEColor(254, 0, 0, 0.5));
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
            if (data != null && data.Points == null) {
                GPSAlert("Can not add area outside of current submap");
            } else if (data != null && data.Points.length > 0) {
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
                    thisObj._shape.SetFillColor(new VEColor(thisObj._color.r, thisObj._color.g, thisObj._color.b, 0.7));
                    thisObj._shape.SetLineColor(new VEColor(thisObj._color.r, thisObj._color.g, thisObj._color.b, 1));
                    thisObj._shape.SetLineWidth(5);
                    thisObj._shape.HideIcon();
                    var layer = GetSubMapLayer();
                    if (layer) {
                        layer.AddShape(thisObj._shape);
                    }

                } else {
                    thisObj._shape.SetPoints(sps);
                }

                thisObj._AddArea(area, data.Total, data.Count, SignRecordArea);
                thisObj.ShowHole(data.Holes);
                updateNotIncludeAreas(data.NotIncludeInSubMapArea);
            } else {
                GPSAlert("The selected shape is not connected to any distribution map, cannot be added");
            }
        };
        var service = new TIMM.Website.DistributionMapServices.DMWriterService();
        GPS.Loading.show();
        service.MergeAreas(campaign.Id, this._submapid, this._id, records, backFun, GPS.Loading.hide);
    },

    /**
    * add mutipule selected area to dmap
    * SignRecordArea is callback function
    */
    AddSelectedAreas: function (area, SignRecordArea) {
        var thisObj = this;
        var records = thisObj._areaRecorder.GetExistMergeRecords();
        console.log(area);
        var newRecords = thisObj._areaRecorder.GetRecordsByArea(area, true);

        var backFun = function (data) {
            GPS.Loading.hide();

            if (data != null && data.Points == null) {
                GPSAlert("Can not add area outside of current submap");
            } else if (data != null && data.Points.length > 0) {
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
                    thisObj._shape.SetFillColor(new VEColor(thisObj._color.r, thisObj._color.g, thisObj._color.b, 0.7));
                    thisObj._shape.SetLineColor(new VEColor(thisObj._color.r, thisObj._color.g, thisObj._color.b, 1));
                    thisObj._shape.SetLineWidth(5);
                    thisObj._shape.HideIcon();
                    var layer = GetSubMapLayer();
                    if (layer) {
                        layer.AddShape(thisObj._shape);
                    }

                } else {
                    thisObj._shape.SetPoints(sps);
                }
                $(data.ValidAreas).each(function () {
                    var currentArea = this;
                    /**
                    * find orignal area
                    */
                    var oringalArea = $.grep(area, function (value, index) {
                        return value.GetId() == currentArea.AreaId;
                    });
                    thisObj._AddArea(oringalArea[0], data.Total, data.Count, SignRecordArea);
                });

                thisObj.ShowHole(data.Holes);
                updateNotIncludeAreas(data.NotIncludeInSubMapArea);
            } else {
                GPSAlert("The selected shape is not connected to any distribution map, cannot be added");
            }
        };
        var service = new TIMM.Website.DistributionMapServices.DMWriterService();
        GPS.Loading.show();
        console.log(records, newRecords);
        service.MergeSelectedAreas(campaign.Id, this._submapid, this._id, records, newRecords, backFun, GPS.Loading.hide);
    },

    _RemoveArea: function (area, total, count, SignRecordArea) {
        //var attributes = area.GetAttributes();
        this._total = Number(total);
        this._penetration = Number(count);
        if (this._total > 0) {
            this._percentage = this._penetration / this._total;
        } else {
            this._percentage = 0;
        }
        this._eventTrigger.TriggerEvent('onareaschange', this);
        this._areaRecorder.RecordArea(area, false);
        this._dmAreas.RemoveArea(this._id, area._id);
        if (SignRecordArea) {
            SignRecordArea(area, false);
        }
    },
    _RemoveAreaArray: function (area, total, count, SignRecordArea) {
        //var attributes = area.GetAttributes();
        this._total = Number(total);
        this._penetration = Number(count);
        if (this._total > 0) {
            this._percentage = this._penetration / this._total;
        } else {
            this._percentage = 0;
        }
        this._eventTrigger.TriggerEvent('onareaschange', this);
        var self = this;
        $.each(area, function () {
            self._areaRecorder.RecordArea(this, false);
            self._dmAreas.RemoveArea(self._id, this._id);
        });

        if (SignRecordArea) {
            SignRecordArea(area, false);
        }
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
    RemoveMutliSelectedArea: function (area, SignRecordArea) {
        if (this._shape) {
            var self = this;
            var result = this._areaRecorder.GetMergeRecordsForRemove(area);
            if (this.__HasAreas__(result.records)) {
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
                        thisObj._RemoveAreaArray(result.validArea, data.Total, data.Count, SignRecordArea);
                        thisObj.ShowHole(data.Holes);
                        updateNotIncludeAreas(data.NotIncludeInSubMapArea);
                    } else {
                        GPSAlert("Unable remove this area.");
                    }
                };
                var service = new TIMM.Website.DistributionMapServices.DMWriterService();
                GPS.Loading.show();
                service.MergeAreas(campaign.Id, this._submapid, this._id, result.records, backFun, GPS.Loading.hide);
            } else {
                var thisObj = this;
                var service = new TIMM.Website.DistributionMapServices.DMWriterService();
                service.EmptyDM(campaign.Id, this._submapid, this._id, function (data) {
                    var layer = GetSubMapLayer();
                    if (layer) {
                        layer.DeleteShape(thisObj._shape);
                    }
                    thisObj._shape = null;
                    thisObj._RemoveAreaArray(area, 0, 0, SignRecordArea);
                    updateNotIncludeAreas(data);
                });
            }
        }
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
                        updateNotIncludeAreas(data.NotIncludeInSubMapArea);
                    } else {
                        GPSAlert("Unable remove this area.");
                    }
                };
                var service = new TIMM.Website.DistributionMapServices.DMWriterService();
                GPS.Loading.show();
                service.MergeAreas(campaign.Id, this._submapid, this._id, records, backFun, GPS.Loading.hide);
            } else {
                var thisObj = this;
                var service = new TIMM.Website.DistributionMapServices.DMWriterService();
                service.EmptyDM(campaign.Id, this._submapid, this._id, function () {
                    var layer = GetSubMapLayer();
                    if (layer) {
                        layer.DeleteShape(thisObj._shape);
                    }
                    thisObj._shape = null;
                    thisObj._RemoveArea(area, 0, 0, SignRecordArea);
                });
            }
        }
    },

    RefreshShape: function () {
        if (this._shape) {
            this._shape.SetFillColor(new VEColor(this._color.r, this._color.g, this._color.b, 0.7));
            this._shape.SetLineColor(new VEColor(this._color.r, this._color.g, this._color.b, 1));
        }
    },

    RemoveShape: function () {
        if (this._shape) {
            var layer = GetSubMapLayer();
            if (layer) {
                layer.DeleteShape(this._shape);
            }
            this._shape = null;
            //            this._RemoveArea(area);
        }
    },

    Serialize: function () {
        var array = [];
        array.push(this._id);
        array.push(this._name);
        array.push(this._submapid);
        return array.join('^');
    }
}

/***************************************************
class GPS.DMList
***************************************************/
GPS.DMList = function (Obj) {
    this._dmrecords = [];
    this._eventTrigger = null;
    this.__Init__(Obj);
}

GPS.DMList.prototype = {
    __Init__: function (Obj) {
        if (Obj) {
            var i = 0;
            var length = Obj.length;
            while (i < length) {
                //var record = [];
                var dm = new GPS.DM(Obj[i]);
                //                    dm._id = Obj[i].Id;
                //                    dm._name = Obj[i].Name;
                //                    dm._submapid = Obj[i].SubMapId;
                this._dmrecords.push(dm);
                i++;
            }
            this._eventTrigger = new GPS.EventTrigger();
        } else {
            this._dmrecords = [];
            this._eventTrigger = new GPS.EventTrigger();
        }
    },
    Clear: function () {
        this._dmrecords = [];
    },

    GetDMRecords: function () {
        return this._dmrecords;
    },

    AttachEvent: function (eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    }
}

/***************************************************
class GPS.DJDMList
***************************************************/
GPS.DJDMList = function (Obj) {
    this._dmrecords = [];
    this._eventTrigger = null;
    this.__Init__(Obj);
}

GPS.DJDMList.prototype = {
    __Init__: function (Obj) {
        if (Obj) {
            var i = 0;
            var length = Obj.length;
            while (i < length) {
                var dm = new GPS.DM(Obj[i]);
                //                dm._id = Obj[i].Id;
                //                dm._name = Obj[i].Name;
                //                dm._submapid = Obj[i].SubMapId;
                this._dmrecords.push(dm);
                i++;
            }
            this._eventTrigger = new GPS.EventTrigger();
        } else {
            this._dmrecords = [];
            this._eventTrigger = new GPS.EventTrigger();
        }
    },
    Clear: function () {
        this._dmrecords = [];
    },

    GetDMRecords: function () {
        return this._dmrecords;
    },

    AttachEvent: function (eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    }
}