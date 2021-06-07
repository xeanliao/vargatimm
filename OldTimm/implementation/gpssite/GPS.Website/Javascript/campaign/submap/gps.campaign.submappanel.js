

var submapPanel = null;

function GetAreaObjSubMap(areaObj) {
    var submap = null;
    if (submapPanel) {
        var submaps = submapPanel.GetSubMaps();
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            if (submaps[i].ContainsAreaObj(areaObj)) {
                submap = submaps[i];
                break;
            }
            i++;
        }
    }
    return submap;
}

function BindSubMaps(submapDatas) {
    if (submapPanel) {
        var submaps = [];
        var i = 0;
        var length = submapDatas.length;
        while (i < length) {
            var submap = new GPS.Map.SubMap(submapDatas[i]);
            submaps.push(submap);
            i++;
        }
        submapPanel.LoadSubMaps(submaps);
    }
}

function GetAreaSubMap(area) {
    var submap = null;
    if (submapPanel) {
        var submaps = submapPanel.GetSubMaps();
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            if (submaps[i].ContainsArea(area)) {
                submap = submaps[i];
                break;
            }
            i++;
        }
    }
    return submap;
}

function GetEditSubMap() {
    var submap = null;
    if (submapPanel) { submap = submapPanel.GetEditSubMap(); }
    return submap;
}

function GetSubMapsString() {
    var submapsString = '';
    var submaps = null;
    if (submapPanel) {
        submaps = submapPanel.GetSubMaps();
    }
    if (submaps) {
        var paramSubMaps = [];
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            paramSubMaps.push(submaps[i].Serialize());
            i++;
        }
        submapsString = paramSubMaps.join('*');
    }
    return submapsString;
}

function SubMapChanged() {
    if (submapPanel) { return submapPanel._changed; }
    else { return false; }
}

function UpdateSubMapChanged() {
    if (submapPanel) { submapPanel._changed = false; }
}

function ClearSubMaps() {
    if (submapPanel) { submapPanel.Clear(); }
}

var submapLayer = null;
function GetSubMapLayer() {
    if (!submapLayer) {
        submapLayer = new VEShapeLayer();
        mapPanel.GetMap().AddShapeLayer(submapLayer);
    }
    return submapLayer;
}



/***************************************************
class GPS.SubMapPanel
***************************************************/
GPS.SubMapPanel = function(div) {
    this._changed = null;
    this._panelView = null;
    this._submapStore = null;
    this._submapsTotal = null;
    this._submapsPenetration = null;
    //    this._addToSubMapDialog = null;
    this.__Init__(div);
}

GPS.SubMapPanel.prototype = {
    __Init__: function(div) {
        this._changed = false;
        this._submapStore = new GPS.Map.ObjectStore();
        this._submapsTotal = 0;
        this._submapsPenetration = 0;
        //        this._addToSubMapDialog = new GPS.AddToSubMapDialog(div);
        this.__InitPanelView__(div);
    },

    __InitPanelView__: function(div) {
        this._panelView = new GPS.SubMapPanelView(div);
        var thisObj = this;
        var OnSubMapDoubleSelect = function(id) { thisObj.__OnSubMapDoubleSelect__(id); };
        var OnSubMapSelect = function(id) { thisObj.__OnSubMapSelect__(id); };
        var OnNewSubMap = function(submap) { thisObj.__OnNewSubMap__(submap); };
        var OnDeleteSubMap = function(submap) { thisObj.__OnDeleteSubMap__(submap); };
        var OnSubMapChange = function(submap) { thisObj.__OnSubMapsChange__(submap); };
        var OnAttachSubMapId = function(submap) { thisObj.__AttachNewSubMapId__(submap); };
        this._panelView.AttachEvent('onnewsubmap', OnNewSubMap);
        this._panelView.AttachEvent('onsubmapselect', OnSubMapSelect);
        this._panelView.AttachEvent('onsubmapdoubleselect', OnSubMapDoubleSelect);
        this._panelView.AttachEvent('ondeletesubmap', OnDeleteSubMap);
        this._panelView.AttachEvent('onsubmapchange', OnSubMapChange);
        this._panelView.AttachEvent('attachsubmapid', OnAttachSubMapId);

        var OnEndSubMapEdit = function() { thisObj._changed = true; };
        var OnSubMapEdit = function() { if (map) { map.Refresh(); } };

        this._panelView.AttachEvent('onendsubmapedit', OnEndSubMapEdit);
        //        this._panelView.AttachEvent('onsubmapedit', OnSubMapEdit);

    },

    __CalculateSubMaps__: function() {
        this._submapsTotal = 0;
        this._submapsPenetration = 0;

        var submaps = this._submapStore.GetObjects();
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            this._submapsTotal += submaps[i].GetTotal();
            this._submapsPenetration += submaps[i].GetPenetration();
            i++;
        }
    },

    __OnSubMapsChange__: function(submap) {
        this.__CalculateSubMaps__();
        var per = 0;
        if (this._submapsPenetration != 0) {
            per = this._submapsPenetration / this._submapsTotal * 100;
        }
        $('#lbCampaignPen').text(per.toFixed(2) + '%');
        $('#lbCampaignTotal').text(Number.addCommas(this._submapsTotal));
        $('#lbCampaignCount').text(Number.addCommas(this._submapsPenetration));
    },

    __AttachNewSubMapId__: function(submap) {
        var newId = this._submapStore.GetMaxObjectId() + 1;
        submap._id = newId;
    },

    __OnNewSubMap__: function(submap) {
        this._submapStore.Append(submap);
    },

    __OnSubMapSelect__: function(id) {
        var submap = this._submapStore.GetById(id);
        if (submap) {
            this._panelView.ActiveSubMap(submap);
        }
    },

    __OnSubMapDoubleSelect__: function(id) {
        var submap = this._submapStore.GetById(id);
        if (submap) {
            this._panelView.ActiveSubMap(submap);
            
            if (submap._centerLat != 0 && submap._centerLon != 0) {
                mapPanel.GetMap().SetCenter(new VELatLong(submap._centerLat, submap._centerLon));
            }
        }
    },

    __OnDeleteSubMap__: function(submap) {
        submap.RemoveShape();
        this._submapStore.Remove(submap);
        this.__OnSubMapsChange__();
        this._changed = true;
    },

    LoadSubMaps: function(submaps) {
        this._submapStore.SetObjects(submaps);
        this.Refresh();
        this._changed = false;
    },

    GetEditSubMap: function() {
        return this._panelView.GetEditSubMap();
    },

    GetSubMaps: function() {
        return this._submapStore.GetObjects();

    },

    ShowAddToSubMapDialog: function(area) {
        //        this._addToSubMapDialog.ShowDialog(this._submapStore.GetObjects(), area);
    },

    Refresh: function() {
        this._panelView.BindSubMaps(this._submapStore.GetObjects());
        this.__OnSubMapsChange__();
    },

    Clear: function() {
        this._submapStore.SetObjects([]);
        this.Refresh();
        this._changed = false;
    },

    GetActiveSubMap: function() { return this._panelView.GetActiveSubMap(); }
}

/***************************************************
class GPS.SubMapPanelView
***************************************************/

GPS.SubMapPanelView = function(div) {
    this._state = null;
    this._view = null;
    this._menuBar = null;
    this._submapList = null;
    this._submapForm = null;
    this._activeSubMap = null;
    this._eventTrigger = null;
    this.__Init__(div);
    this.yesBtn = "yes";
}

GPS.SubMapPanelView.prototype = {
    __Init__: function (div) {
        this._state = ViewState.Normal;
        this._view = $('#' + div);
        var prefix = div + '-';
        this.__InitMenuBar__(prefix);
        this.__InitSubMapListView__(prefix);
        this.__InitSubMapFormView__(prefix);
        this.__InitEvents__();
    },

    __InitMenuBar__: function (prefix) {
        this._menuBar = new GPS.SubMapMenuBarView(prefix);
        var thisObj = this;
        var OnMenuClick = function (action) { thisObj.__OnMenuClick__(action); };
        this._menuBar.AttachEvent('onmenuclick', OnMenuClick);
        $(this._view).append($(this._menuBar.GetView()));
    },

    __InitMapInfo__: function (prefix) {
        this._mapInfo = new GPS.SubMapInfoView(prefix);
        $(this._view).append($(this._mapInfo.GetView()));
    },

    __InitSubMapListView__: function (prefix) {
        this._submapList = new GPS.SubMapListView(prefix);
        var thisObj = this;
        var OnItemSelect = function (id) {
            thisObj.__OnListItemSelect__(id);
        };
        var OnItemDoubleSelect = function (id) {
            thisObj.__OnListItemDoubleSelect__(id);
        };
        this._submapList.AttachEvent('onitemselect', OnItemSelect);
        this._submapList.AttachEvent('onitemdoubleselect', OnItemDoubleSelect);
        $(this._view).append($(this._submapList.GetView()));
    },

    __InitSubMapFormView__: function (prefix) {
        this._submapForm = new GPS.SubMapFormView(prefix);
        var thisObj = this;
        var OnFormButtonClick = function (action) { thisObj.__OnFormButtonClick__(action); };
        this._submapForm.AttachEvent('onformbuttonclick', OnFormButtonClick);
        $(this._view).append($(this._submapForm.GetView()));
    },

    __InitEvents__: function () {
        this._eventTrigger = new GPS.EventTrigger();
    },

    __NewSubMap__: function () {
        var submap = new GPS.Map.SubMap();
        this._eventTrigger.TriggerEvent('attachsubmapid', submap);
        var thisObj = this;
        var OnAreasChange = function (submap) { thisObj.__OnSubMapAreasChange__(submap); };
        submap.AttachEvent('onareaschange', OnAreasChange);
        return submap;
    },

    __OnSubMapAreasChange__: function (submap) {
        this._submapList.UpdateSubMap(submap);
        this._eventTrigger.TriggerEvent('onsubmapchange', submap);
    },

    __OnMenuClick__: function (action) {
        if (action == 'new') {
            var submap = this.__NewSubMap__();
            this._submapForm.SetSubMap(submap);
            this._submapForm.DataBind();
            this.__ShowSubMapForm__();
        }
        else if (action == 'edit') {
            if (this._activeSubMap) {
                this._submapForm.SetSubMap(this._activeSubMap);
                this._submapForm.DataBind();
                this.__ShowSubMapForm__();
            }
            else { GPSAlert('To edit, please select a sub map first.'); }
        }
        else if (action == 'delete') {
            if (this._activeSubMap) {
                var thisObj = this;
                var confirmationMessage = "You are about to delete : " + this._activeSubMap._name + " are you sure you would like to continue?";
                GPSConfirm(confirmationMessage, function (btn) {
                    if (btn == thisObj.yesBtn)
                        thisObj._DoBulkDeleteSubmap();
                });
            }
            else { GPSAlert('To edit, please select a sub map first.'); }
        }
        else if (action == "addAllShapes") {

            // Find all selected dots
            var recorder = GetCampaignAreaRecord();

            var clsRecords = recorder._records.Get("15");

            var checkedList = [];
            if (clsRecords) {
                clsRecords.Each(function (i, values) {
                    if (values.Value) checkedList.push(i);
                });
            }

            // Get all croute dots
            var areas = mapPanel._areaLayers[15]._activeAreas._objects;

            var addedAreas = [];

            for (var ia in areas) {
                var area = areas[ia];

                for (var ich in checkedList) {
                    if (area._id == checkedList[ich]) {

                        var hasContained = GetAreaSubMap(area);

                        if (!hasContained) {
                            addedAreas.push(area);
                            //                        mapPanel._SignSubmapArea(area, true, null);
                        }

                    }
                }
            }

            // add bulk of croutes to actived submap.
            if (submapPanel && addedAreas.length > 0) {
                var submap = submapPanel.GetActiveSubMap();
                if (submap) {
                    submap.AddAreas(addedAreas, function () {
                        if (arguments[1] == true) {
                            mapPanel._SignArea(arguments[0], true);
                        }
                    });
                }
                else { GPSAlert("Please select a sub map first."); }
            }
        }
        else if (action == "removeAllShapes") {

            var activedSubmap = submapPanel.GetActiveSubMap();
            if (!activedSubmap) {
                GPSAlert("Please select a submap firstly");
                return;
            }

            // Find all selected dots
            var recorder = GetCampaignAreaRecord();

            var clsRecords = recorder._records.Get("15");

            var checkedList = [];

            if (clsRecords) {
                clsRecords.Each(function (i, values) {
                    if (values.Value) {
                        checkedList.push(i); 
                    }
                });
            }

            // Get all croute dots
            var areas = mapPanel._areaLayers[15]._activeAreas._objects;

            var removedAreas = [];

            for (var ia in areas) {
                var area = areas[ia];

                var hasFound = false;

                for (var ich in checkedList) {
                    if (area._id == checkedList[ich]) {
                        hasFound = true;
                        break;
                    }
                }

                if (hasFound) {
                    var submap = GetAreaSubMap(area);
                    //                    mapPanel._SignArea(area, false);

                    if (submap == activedSubmap) {

                        removedAreas.push(area);
                    }
                }
            }

            // add bulk of croutes to actived submap.
            if (submapPanel && removedAreas.length > 0) {
                activedSubmap.RemoveAreas(removedAreas, function () {
                    if (arguments[1] == false) {
                        mapPanel._SignArea(arguments[0], false);
                    }
                });
            }
        }
        else if (action == "deselectAllShapes") {
            // Find all selected dots
            var recorder = GetCampaignAreaRecord();

            var clsRecords = recorder._records.Get("15");

            var checkedList = [];

            clsRecords.Each(
            function (i, values) {
                if (values.Value) checkedList.push(i);
            });

            // Get all croute dots
            var areas = mapPanel._areaLayers[15]._activeAreas._objects;

            var addedAreas = [];

            for (var ia in areas) {
                var area = areas[ia];

                for (var ich in checkedList) {
                    if (area._id == checkedList[ich]) {
                        mapPanel._SignArea(area, false);
                    }
                }
            }
        }
    },

    _DoBulkDeleteSubmap: function () {
        var thisObj = this;
        var service = new TIMM.Website.CampaignServices.CampaignWriterService();
        service.DeleteSubMap(campaign.Id, this._activeSubMap.GetUId(), function () {
            thisObj._eventTrigger.TriggerEvent('ondeletesubmap', thisObj._activeSubMap);
            thisObj._submapList.Remove(thisObj._activeSubMap);
            thisObj._activeSubMap = null;
        });
    },

    __OnFormButtonClick__: function (action) {
        if (action == 'cancel') {
            this.__ShowSubMapList__();
        }
        else if (action == 'save') {
            this._submapForm.DataHold();
            var service = new TIMM.Website.CampaignServices.CampaignWriterService();
            var submap = this._submapForm.GetSubMap();
            var thisObj = this;
            if (this._activeSubMap != submap) {
                $(this._view).find('A').hide();
                service.NewSubMap(submap.ToServerObj(), function (ret) {
                    if (ret) {
                        $(this._view).find('A').show();
                        submap.SetOptions(ret);
                        thisObj._eventTrigger.TriggerEvent('onnewsubmap', submap);
                        thisObj._submapList.Append(submap);
                        thisObj.ActiveSubMap(submap);
                    }
                });
            }
            else {
                var submap = thisObj._activeSubMap;
                service.UpdateSubMapDetails(submap.ToServerObj(), function (ret) {
                    thisObj._submapForm.SetSubMap(submap);
                    thisObj._submapList.UpdateSubMap(submap);
                    submap.RefreshShape();
                });
            }

            this.__ShowSubMapList__();
        }
    },

    __OnListItemSelect__: function (id) {
        this._eventTrigger.TriggerEvent('onsubmapselect', id);
    },

    __OnListItemDoubleSelect__: function (id) {
        this._eventTrigger.TriggerEvent('onsubmapdoubleselect', id);
    },

    __ShowSubMapList__: function () {
        this._submapList.Show();
        this._submapForm.Hide();
        this._state = ViewState.Normal;
        this._eventTrigger.TriggerEvent('onendsubmapedit');
    },

    __ShowSubMapForm__: function () {
        this._submapList.Hide();
        this._submapForm.Show();
        this._state = ViewState.Edit;
        this._eventTrigger.TriggerEvent('onsubmapedit');
    },

    ActiveSubMap: function (submap) {
        this._activeSubMap = submap;
    },

    GetActiveSubMap: function () {
        return this._activeSubMap;
    },

    AttachEvent: function (eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    GetEditSubMap: function () {
        var submap = null;
        if (this._state == ViewState.Edit) {
            submap = this._submapForm.GetSubMap();
        }
        return submap;
    },

    BindSubMaps: function (submaps) {
        this._submapList.BindSubMaps(submaps);
        this.__ShowSubMapList__();
        this._activeSubMap = null;

        var thisObj = this;
        $.each(submaps, function (i, submap) {
            var OnAreasChange = function (submap) { thisObj.__OnSubMapAreasChange__(submap); };
            submap.AttachEvent('onareaschange', OnAreasChange);
        });
    }
}


/***************************************************
enum ViewState
***************************************************/

ViewState = {
    Normal: 0,
    Edit: 1
}