
var campaignAreaRecorder = null;
var campaignAddressRecorder = null;
function GetCampaignAreaRecord() {
    if (campaignAreaRecorder == null) {
        campaignAreaRecorder = new GPS.DMMapPanel.AreaRecorder();

    }
    return campaignAreaRecorder;
}



/*
*   Class GPS.DMMapPanel
*/
GPS.DMMapPanel = GPS.Map.MapBase.extend({
    init: function (options) {
        options.Symbol = "campaignmap";
        this._super(options);
        this.SetClassificationVisible(13, true);
    },
    // Resize map
    ResizeMap: function (width, height) {
        this._map.Resize(width, height);
    },
    _OnMouseDown: function (e) {
        // On mouse down, check to see if the control key is pressed.  If not, do nothing.  If, so start the selection process.
        if (e.ctrlKey || e.altKey) {
            // enable selecting mode
            this.selecting = true;
            this.addArea = e.ctrlKey;

            // set the start points 

            this.StartLatLon = e.latLong;
            return true;
        }
        return false;
    },

    _OnMouseMove: function (e) {
        //When moving the mouse, if in "selecting" mode, draw, otherwise do nothing.
        if (!this.selecting) {
            return false;
        } else {
            // clear select box
            if (this.selectBox) { this._map.DeleteShape(this.selectBox) };

            // set the latest endpoints (opposite side of the selection box) 

            this.EndLatLon = e.latLong;

            //        document.getElementById("EndLat").innerHTML = EndLatLon.Latitude.toFixed(4);
            //        document.getElementById("EndLng").innerHTML = EndLatLon.Longitude.toFixed(4);

            // create new selection box (VEShape) from start and end (corners) coordinates
            this.selectBox = new VEShape(
            VEShapeType.Polygon,
            [
            	this.StartLatLon,
            	new VELatLong(this.StartLatLon.Latitude, this.EndLatLon.Longitude),
            	this.EndLatLon,
            	new VELatLong(this.EndLatLon.Latitude, this.StartLatLon.Longitude),
                this.StartLatLon,
            ]);

            this.selectBox.HideIcon();
            this.selectBox.SetLineWidth(4);
            this.selectBox.SetLineColor(new VEColor(255, 255, 255, 1.0));

            this._map.AddShape(this.selectBox);
        }
    },

    _OnMouseUp: function (e) {
        // On mouse up, if in selecting mode cancel selecting mode
        if (this.selecting) {
            // cancel selecting mode
            this.selecting = false;

            //Determine if any of the push pins are within the selection box bounds
            if (this.StartLatLon.Latitude > this.EndLatLon.Latitude) {
                var eBound = this.StartLatLon.Latitude;
                var wBound = this.EndLatLon.Latitude;
            } else {
                var eBound = this.EndLatLon.Latitude;
                var wBound = this.StartLatLon.Latitude;
            }
            if (this.StartLatLon.Longitude > this.EndLatLon.Longitude) {
                var nBound = this.StartLatLon.Longitude;
                var sBound = this.EndLatLon.Longitude;
            } else {
                var nBound = this.EndLatLon.Longitude;
                var sBound = this.StartLatLon.Longitude;
            }
            var dm = dmPanel.GetActiveDM();
            if (!dm) {
                GPSAlert("Please select a distribution map first.");
                return false;
            }
            var subMap = dmPanel.GetActiveSubMap();

            var activeAreas = this._areaLayers[subMap._classification]._activeAreas._objects;
            var selectedArea = [];

            for (var i in activeAreas) {
                if (activeAreas[i]._latitude < eBound
                	&& activeAreas[i]._latitude > wBound
                	&& activeAreas[i]._longitude < nBound
                	&& activeAreas[i]._longitude > sBound) {
                    selectedArea.push(activeAreas[i]);
                }
            }

            if (selectedArea && selectedArea.length > 0) {
                if (dmPanel) {
                    var thisObj = this;
                    if (this.addArea) {
                        dm.AddSelectedAreas(selectedArea, function (addArea) {
                            thisObj._SignArea(addArea, false);
                        });
                    } else {
                        dm.RemoveMutliSelectedArea(selectedArea, function () {
                            $(selectedArea).each(function () {
                                thisObj._SignArea(this, true);
                            });
                        });
                    }
                }
            }
            return false;
        }
    },
    //VEMap onchangeview event method
    _OnClick: function (e) {
        this._super(e);
        if (e.rightMouseButton && e.elementID) {
            var areaObj = this._GetAreaAndShapeIdByElementId([0, 1, 2, 3, 15], e.elementID);
            if (areaObj) {
                var recorder = GetCampaignAreaRecord();
                var record = recorder.GetAreaRecord({ Classification: areaObj.Area.GetClassification(),
                    AreaId: areaObj.Area.GetId(),
                    ShapeId: areaObj.ShapeId,
                    Relations: areaObj.Area.GetRelations()
                });

                var inSpeed = 150;
                if ($("#area-context-memu").length > 0) {
                    $("#area-context-memu").remove();
                }
                var menus = [];
                //                if (record && record.Value) {
                //                    menus.push('<li><a href="#deselect">Deselect</a></li>');
                //                }
                //                else {
                //                    menus.push('<li><a href="#select">Select</a></li>');
                //                }
                var area = areaObj.Area;
                if (area.GetClassification() >= 1 && area.GetClassification() <= 3 || area.GetClassification() == 15) {
                    if (GetAreaDMMap(area)) { menus.push('<li class="separator"><a href="#removefromsubmap">Remove from distribution map</a></li>'); }
                    else { menus.push('<li class="separator"><a href="#addtosubmap">Add to distribution map</a></li>'); }
                }
                //                menus.push('<li class="separator"><a href="#adjustcount">Adjust Count</a></li>');

                $("body").append('<ul id="area-context-memu" class="contextMenu">' + menus.join('') + '</ul>');
                var menu = $('#area-context-memu');

                $(menu).find('A').mouseover(function () {
                    $(menu).find('LI.hover').removeClass('hover');
                    $(this).parent().addClass('hover');
                }).mouseout(function () {
                    $(menu).find('LI.hover').removeClass('hover');
                });
                var thisObj = this;
                $(menu).find('A').unbind('click');
                $(menu).find('LI:not(.disabled) A').click(function () {
                    $(document).unbind('click').unbind('keypress');
                    $(".contextMenu").hide();
                    // Callback
                    thisObj._OnClickPopMenu(areaObj, $(this).attr('href').substr($(this).attr('href').lastIndexOf('#') + 1));
                    return false;
                });

                $("#area-context-memu").css({ top: e.clientY, left: e.clientX }).fadeIn(inSpeed);
            }
        }
        else {
            $(".contextMenu").hide();
        }
    },

    RefreshColors: function () {
        var cls = [1, 2, 3, 15];
        for (var i in cls) {
            var cId = cls[i];
            this._areaLayers[cId].RefreshColors();
        }
    },

    _SignArea: function (area, selectValue) {
        var recorder = GetCampaignAreaRecord();
        recorder.SetAreaRecord({ Classification: area.GetClassification(),
            AreaId: area.GetId(),
            ShapeId: 0,
            Relations: area.GetRelations(),
            Value: selectValue
        });
        area.RefreshColors();
        var classification = area.GetClassification();
        var cls = [];
        if (classification < 3) {
            cls.push(3);
            if (classification < 2) {
                cls.push(2);
                cls.push(15);
                if (classification < 1) {
                    cls.push(1);
                }
            }
        }
        for (var i in cls) {
            var cId = cls[i];
            this._areaLayers[cId].RefreshColors();
        }
    },

    _SignSubmapArea: function (area, addValue, fnBack) {
        if (addValue) {
            if (dmPanel) {
                var dm = dmPanel.GetActiveDM();
                if (dm) {
                    var thisObj = this;
                    dm.AddArea(area, fnBack);
                }
                else { GPSAlert("Please select a distribution map first."); }
            }
        }
        else {
            var dm = GetAreaDMMap(area);
            if (dm) {
                dm.RemoveArea(area, fnBack);
            }
        }
    },

    _OnClickPopMenu: function (areaObj, action) {
        var area = areaObj.Area;
        var thisObj = this;
        if (action == "select" || action == "deselect") {
            var selectValue = action == "select";
            if ((!selectValue) && GetAreaSubMap(area)) {
                this._SignSubmapArea(area, false, function () {
                    thisObj._SignArea(area, false);
                });
            }
            else {
                this._SignArea(area, selectValue);
            }
        }
        else if (action == "addtosubmap" || action == "removefromsubmap") {
            if (action == "addtosubmap") {
                //                if (area.GetAttributes() && area.GetAttributes().PartCount > 1) {
                //                    GPSConfirm("This is a multi-polygon. Would you like to adjust the count?", function(ret) {
                //                        if (ret != 'yes') {
                //                            thisObj._SignSubmapArea(area, true, function() {
                //                                thisObj._SignArea(area, true);
                //                            });
                //                        }
                //                    });
                //                }
                //                else {
                this._SignSubmapArea(area, true, function () {
                    thisObj._SignArea(area, true);
                });
                //                }
            }
            else {
                this._SignSubmapArea(area, false);
            }
        }
        else if (action == "adjustcount") {
            if (!this._adjustCountDialog) {
                this._adjustCountDialog = new GPS.CampaignAdjustCountDialog({
                    DialogElement: 'adjust-count-dialog',
                    DialogTitle: 'Adjust Count'
                });
                this._adjustCountDialog.Show();
                this._adjustCountDialog.Bind(area);
            }
            else {
                this._adjustCountDialog.Show();
                this._adjustCountDialog.Bind(area);
            }
        }
    },

    PushRecords: function (records) {
        var classifications = new GPS.QArray();
        var recorder = GetCampaignAreaRecord();
        for (var i in records) {
            records[i].Relations = DictionaryConvertToClient(records[i].Relations);
            recorder.SetAreaRecord(records[i]);
            classifications.Set(records[i].Classification, true);
        }
        var thisObj = this;
        classifications.Each(function (classification, value) {
            thisObj._areaLayers[classification].ReLoad();
        });
    },

    SignArea: function (options) {
        var recorder = GetCampaignAreaRecord();
        recorder.SetAreaRecord(options);
        var area = this._GetArea(options.Classification, options.AreaId);
        if (area) {
            area.RefreshColors();
        }
        if (options.Center) {
            this._map.SetCenter(options.Center);
        }
    },

    HoldCampaign: function (campaignObj) {
        campaignObj.Latitude = this._map.GetCenter().Latitude;
        campaignObj.Longitude = this._map.GetCenter().Longitude;
        campaignObj.ZoomLevel = this._map.GetZoomLevel();
        GetCampaignAreaRecord().HoldCampaign(campaignObj);
        campaignObj.VisibleClassifications = [];
        for (var c in this._areaLayers) {
            if (this._areaLayers[c].GetVisible()) {
                campaignObj.VisibleClassifications.push(c)
            }
        }
    },

    BindCampaign: function (campaignObj) {
        if (campaignObj.ZoomLevel > 0) {
            this._map.SetCenterAndZoom(new VELatLong(campaignObj.Latitude, campaignObj.Longitude), campaignObj.ZoomLevel);
        }
        GetCampaignAreaRecord().BindCampaign(campaignObj);
        for (var i in campaignObj.VisibleClassifications) {
            var c = campaignObj.VisibleClassifications[i];
            this._areaLayers[c].SetVisible(true);

            switch (c) {
                case 0:
                    $('#ck_Z3').attr('checked', true);
                    break;
                case 1:
                    $('#ck_Z5').attr('checked', true);
                    break;
                case 2:
                    $('#ck_TRK').attr('checked', true);
                    break;
                case 3:
                    $('#ck_BG').attr('checked', true);
                    break;
                case 4:
                    $('#ck_CBSA').attr('checked', true);
                    break;
                case 5:
                    $('#ck_Urban').attr('checked', true);
                    break;
                case 6:
                    $('#ck_County').attr('checked', true);
                    break;
                case 7:
                    $('#ck_SLD_Senate').attr('checked', true);
                    break;
                case 8:
                    $('#ck_SLD_House').attr('checked', true);
                    break;
                case 9:
                    $('#ck_Voting_District').attr('checked', true);
                    break;
                case 10:
                    $('#ck_SD_Elem').attr('checked', true);
                    break;
                case 11:
                    $('#ck_SD_Secondary').attr('checked', true);
                    break;
                case 12:
                    $('#ck_SD_Unified').attr('checked', true);
                    break;
                case 15:
                    $('#ck_CRoute').attr('checked', true);
                    break;

            }

        }
    }


});

GPS.DMMapPanel.AreaLayer = GPS.Map.AreaLayerBase.extend({
    init: function(options) {
        options.Symbol = "campaignarealayer";
        this._super(options);
    },
    RefreshColors: function() {
        this._activeAreas.Each(function(i, area) {
            area.RefreshColors();
        });
    }
});

GPS.DMMapPanel.Area = GPS.Map.AreaBase.extend({
    init: function(options) {
        options.Symbol = "campaignarea";

        this._super(options);
    },

    // initalization shape style
    _InitShapesStyle: function(options) {
        var recorder = GetCampaignAreaRecord();
        var selectOptions = { Classification: this._classification,
            AreaId: this._id,
            Relations: this._relations
        };
        for (var shp in this._shapes) {
            //            selectOptions.ShapeId = this._shapes[shp].GPSShapeId;
            //            var record = recorder.GetAreaRecord(selectOptions);
            //            if (record != null) {
            //                options.IsHignLight = record.Value;
            //            }
            //            else {
            //                var addressRecorder = addressPanel;
            //                options.IsHignLight = addressRecorder.HasRecords(selectOptions);
            //            }
            var styleOptions = GPS.Map.AreaStyleManager.Get(options);
            this.SetShapeStyle(this._shapes[shp], styleOptions);
        }
        this.RefreshColors();
    },
    // set shap style
    SetShapeStyle: function(shape, options) {
        if (options.Title) {
            shape.SetTitle(options.Title);
        }
        if (options.Description) {
            shape.SetDescription(options.Description);
        }
        if (options.CustomIcon) {
            shape.SetCustomIcon(options.CustomIcon);
        }
        if (typeof (options.IconVisible) != 'undefined') {
            if (options.IconVisible) {
                shape.ShowIcon();
            }
            else {
                shape.HideIcon();
            }
        }
        if (options.LineColor) {
            shape.SetLineColor(options.LineColor);
        }
        if (options.LineWidth) {
            shape.SetLineWidth(options.LineWidth);
        }
        if (options.FillColor) {
            shape.SetFillColor(options.FillColor);
        }
    },

    RefreshColors: function() {
        var recorder = GetCampaignAreaRecord();
        var addressRecorder = addressPanel;
        if (recorder) {
            var options = { Classification: this._classification,
                AreaId: this._id,
                Relations: this._relations
            };
            for (var shp in this._shapes) {
                // options.ShapeId = this._shapes[shp].GPSShapeId;
                var record = recorder.GetAreaRecord(options);
                if (record != null) {
                    this._SetShapeColors(this._shapes[shp], record.Value);
                }
                else if (addressRecorder) {
                    this._SetShapeColors(this._shapes[shp], addressRecorder.HasRecords(options));
                }
                else {
                    this._SetShapeColors(this._shapes[shp], false);
                }
            }
        }
    },

    AdjustCount: function(total, count) {
        var pen = total > 0 ? Number(count) / Number(total * 1.0) : 0;
        this._attributes.Total = total;
        this._attributes.Count = count;
        this._attributes.Penetration = pen;
        this.RefreshColors();
    },

    _SetShapeColors: function(shape, highLight) {
        if (this._isEnabled) {
            var highWidth = highLight ? 0.2 : 0;
            var fColor, lColor;

            if (Number(this._attributes.Penetration) >= 0) {
                if (Number(this._attributes.Penetration) == 0 && Number(this._attributes.Count) == 0) {
                    fColor = GPS.ClsSettings[Number(this._classification)].FillColor;
                } else {
                    fColor = GPS.PenetrationColorManager.GetMapColor(Number(this._attributes.Penetration));
                }
            }
            if (!fColor) {
                fColor = GPS.ClsSettings[Number(this._classification)].FillColor;
            }
            lColor = GPS.ClsSettings[Number(this._classification)].LineColor;
            shape.SetLineColor(new VEColor(lColor.R, lColor.G, lColor.B, lColor.A + highWidth));
            shape.SetFillColor(new VEColor(fColor.R, fColor.G, fColor.B, fColor.A + highWidth));
        }
        else {
            shape.SetLineColor(new VEColor(0, 0, 0, 0.6));
            shape.SetFillColor(new VEColor(0, 0, 0, 0.6));
            shape.SetLineWidth(2);
        }
    }
});


GPS.DMMapPanel.AddressRecorder = Class.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function() {
        this._Define();
        this._records = new GPS.QArray();
    },
    // define basic attributes
    _Define: function() {
        this._records = null;
    },
    //
    Clear: function() {
        this._records = new GPS.QArray();
    },

    Remove: function(addressId) {
        this._records.Set(addressId, null);
    },

    GetRecord: function(classification, areaId) {
        var record = null;
        this._records.Each(function(addressId, aRecords) {
            aRecords.Each(function(rId, rRecords) {
                if (rRecords.Enabled) {
                    var cRecords = rRecords.Get(classification);
                    if (cRecords) {
                        record = cRecords.Get(areaId);
                    }
                }
                if (record) {
                    return true;
                }
            });
            if (record) {
                return true;
            }
        });
        return record;
    },

    Push: function(address) {
        var aRecords = new GPS.QArray();
        for (var i in address.Radiuses) {
            var rRecords = new GPS.QArray();
            var rRelations = address.Radiuses[i].Relations;
            for (var j in rRelations) {
                var cRecords = new GPS.QArray();
                var cRelations = rRelations[j].Value;
                for (var k in cRelations) {
                    cRecords.Set(cRelations[k].Key, cRelations[k].Value);
                }
                rRecords.Set(rRelations[j].Key, cRecords);
            }
            rRecords.Enabled = address.Radiuses[i].IsDisplay;
            aRecords.Set(address.Radiuses[i].Id, rRecords);
        }
        this._records.Set(address.Id, aRecords);
    },

    SetRadiusEnabled: function(options) {
        var aRecords = this._records.Get(options.AddressId);
        if (aRecords) {
            var rRecords = aRecords.Get(options.RadiusId);
            if (rRecords) {
                rRecords.Enabled = options.Enabled;
            }
        }
    },

    GetExportString: function(addressId, radiusId) {
        var items = [];
        var aRecords = this._records.Get(addressId);
        if (aRecords) {
            var rRecords = aRecords.Get(radiusId);
            if (rRecords) {
                rRecords.Each(function(i, cRecords) {
                    cRecords.Each(function(j, record) {
                        items.push([i, j, true].join(','));
                    });
                });
            }
        }
        return items.join(';');
    }

});

GPS.DMMapPanel.AreaRecorder = Class.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function() {
        this._Define();
        this._records = new GPS.QArray();
    },
    // define basic attributes
    _Define: function() {
        this._records = null;
    },
    //
    Clear: function() {
        this._records = new GPS.QArray();
    },
    // Record
    _Record: function(classification, areaId, value, relations) {
        var record = this._GetRecord(classification, areaId, relations);
        if ((record != null && record.Value != value) || (record == null && value == true)) {
            this._HardRecord(classification, areaId, value, relations);
        }
        else {
            this._ClearSubRecords(classification, areaId);
        }
    },
    // Clear Sub Records
    _ClearSubRecords: function(classification, areaId) {
        var cls = [];
        if (classification < 3) {
            cls.push(3);
            if (classification < 2) {
                cls.push(2);
                cls.push(15);
                if (classification < 1) {
                    cls.push(1);
                }
            }
        }
        for (var t in cls) {
            var cId = cls[t];
            var cRecords = this._records.Get(cId);
            if (cRecords != null) {
                cRecords.Each(function(i, aRecord) {
                    var aRelations = aRecord.Relations;
                    if ((typeof (aRelations[classification]) != "undefined") &&
                        (typeof (aRelations[classification][areaId]) != "undefined")) {
                        cRecords.Set(i, null);
                    }
                });
            }
        }
    },
    // new record
    _HardRecord: function(classification, areaId, value, relations) {
        this._ClearSubRecords(classification, areaId);
        var clsRecords = this._records.Get(classification);
        if (clsRecords == null) {
            clsRecords = new GPS.QArray();
            this._records.Set(classification, clsRecords);
        }
        clsRecords.Set(areaId, { Id: areaId, Value: value, Relations: relations });
    },
    //Get relations record
    _GetRelationsRecord: function(relations) {
        var record = null;
        for (var cId in relations) {
            var cRelations = relations[cId];
            for (var aId in cRelations) {
                record = this._GetAreaShapeSingleRecord(cId, aId);
                if (record != null) {
                    break;
                }
            }
            if (record != null) {
                break;
            }
        }
        return record;
    },
    // get area shape record
    _GetAreaShapeSingleRecord: function(classification, areaId) {
        var record = null;
        var clsRecords = this._records.Get(classification);
        if (clsRecords != null) {
            var record = clsRecords.Get(areaId);
        }
        return record;
    },
    // get record
    _GetRecord: function(classification, areaId, relations) {
        var record = this._GetAreaShapeSingleRecord(classification, areaId);
        if (record == null) {
            record = this._GetRelationsRecord(relations);
        }
        return record;
    },
    // Get Area Record
    _GetSingleAreaRecords: function(classifation, areaId) {
        var clsRecords = this._GetClsRecords(classification);
        var areaRecords = clsRecords.Get(areaId);
        if (areaRecords == null) {
            areaRecords = new GPS.QArray();
            clsRecords.Set(areaId, areaRecords);
        }
        return areaRecords;
    },
    // Get Classification Records
    _GetClsRecords: function(classification) {
        var clsRecords = this._records.Get(classification);
        if (clsRecords == null) {
            clsRecords = new GPS.QArray();
            this._records.Set(classification, clsRecords);
        }
        return clsRecords;
    },
    // Set Area Record
    SetAreaRecord: function(options) {
        return this._Record(options.Classification, options.AreaId, options.Value, options.Relations);
    },
    // Get Area Record
    GetAreaRecord: function(options) {
        return this._GetRecord(options.Classification, options.AreaId, options.Relations);
    },
    GetExportString: function() {
        var items = [];
        this._records.Each(function(i, cRecords) {
            cRecords.Each(function(j, aRecord) {
                items.push([i, aRecord.Id, aRecord.Value].join(','));
            });
        });
        return items.join(';');
    },

    HoldCampaign: function(campaignObj) {
        var items = [];
        this._records.Each(function(i, cRecords) {
            cRecords.Each(function(j, aRecord) {
                items.push({ Classification: i, AreaId: aRecord.Id, Value: aRecord.Value });
            });
        });
        campaignObj.CampaignRecords = items;
    },

    BindCampaign: function(campaignObj) {
        var items = campaignObj.CampaignRecords;
        for (var i in items) {
            this._Record(items[i].Classification, items[i].AreaId, items[i].Value, DictionaryConvertToClient(items[i].Relations));
        }
    },

    PushServerRecords: function(items) {
        for (var i in items) {
            this._Record(items[i].Classification, items[i].AreaId, items[i].Value, DictionaryConvertToClient(items[i].Relations));
        }
    }
});

GPS.Map.DMAreaRecorder = GPS.DMMapPanel.AreaRecorder.extend({
    GetRecordByArea: function (area) {
        var record = this._GetRecord(area.GetClassification(), area.GetId(), area.GetRelations());
        return record && record.Value;
    },

    GetRecordByAreaObj: function (areaObj) {

    },

    RecordArea: function (area, value) {
        this._Record(area.GetClassification(), area.GetId(), value, area.GetRelations());
    },
    filterArea: function (area) {

    },
    GetMergeRecordsForRemove: function (area) {
        var mRecords = [];
        var validArea = [];
        this._records.Each(function (c, cRecords) {
            cRecords.Each(function (aId, aRecord) {
                //we need keep these area. if these area need remove com dmap
                // just set the Value to false. only remove not in submap areas
                // true keep this area in dmap
                // false remove this area from dmap
                var status = true;
                $.each(area, function () {
                    if (this.GetClassification() == c && this.GetId() == aId) {
                        status = false;
                        validArea.push(this);
                        return false;
                    }
                });
                // the _records still have many have been removed area. 
                // only area orignal in dmap (aRecord.Value is true) and is not need removed area set true
                mRecords.push({ 
                    Classification: c,
                    AreaId: aId,
                    Value: aRecord.Value && status
                });
            });
        });

        return {
            records: mRecords,
            validArea: validArea
        };
    },
    GetMergeRecords: function (area, add) {
        var mRecords = [];

        this._records.Each(function (c, cRecords) {
            cRecords.Each(function (aId, aRecord) {
                if (area.GetClassification() != c || area.GetId() != aId) {
                    mRecords.push({ Classification: c,
                        AreaId: aId,
                        Value: aRecord.Value
                    });
                }
            });
        });

        mRecords.push({
            Classification: area.GetClassification(),
            AreaId: Number(area.GetId()),
            Value: add
        });

        return mRecords;
    },

    GetExistMergeRecords: function () {
        var mRecords = [];

        this._records.Each(function (c, cRecords) {
            cRecords.Each(function (aId, aRecord) {
                mRecords.push({ Classification: c,
                    AreaId: aId,
                    Value: aRecord.Value
                });
            });
        });

        return mRecords;
    },

    GetRecordsByArea: function (areas, value) {
        var mRecords = [];
        $(areas).each(function () {
            mRecords.push({
                Classification: this.GetClassification(),
                AreaId: Number(this.GetId()),
                Value: value
            });
        });

        return mRecords;
    }
});