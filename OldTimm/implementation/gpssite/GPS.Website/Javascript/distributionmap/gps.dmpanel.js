

var dmPanel = null;
var args = GetUrlParms();
var campaignid = args["cid"];
function BindDMs(dmDatas) {
    if (dmPanel) {
        var dms = [];
        var i = 0;
        var length = dmDatas.length;
        while (i < length) {
            var dm = new GPS.SubMap(dmDatas[i]);
            

//            var len = dmDatas[i].DistributionMaps.length;
//            var j = 0;
//            while (j < len) {
//                var dmap = new GPS.DM(dmDatas[i].DistributionMaps[j]);       
//                j++;
//            }
            
            dms.push(dm);
            i++;
        }
        dmPanel.LoadDMs(dms);
    }
}


function GetAreaDMMap(areaObj) {
    var dmmap = null;
    if (dmPanel) {
        var dmmaps = dmPanel.GetDMs();
        var i = 0;
        var length = dmmaps.length;
        while (i < length) {
            var len = dmmaps[i]._dms._dmrecords.length;
            var j = 0;
            while (j < len) {
                if (dmmaps[i]._dms._dmrecords[j].ContainsArea(areaObj)) {
                    dmmap = dmmaps[i]._dms._dmrecords[j];
                    break;
                }
                j++;
            }
            i++;
        }
    }
    return dmmap;
}

function GetEditDM() {
    var dm = null;
    if (dmPanel) { dm = dmPanel.GetEditDM(); }
    return dm;
}

function GetDMsList() {
    var dms = null;
    if (dmPanel) {
        dms = dmPanel.GetDMList();
    }

    return dms;
}

function DMChanged() {
    if (dmPanel) { return dmPanel._changed; }
    else { return false; }
}

function UpdateDMChanged() {
    if (dmPanel) { dmPanel._changed = false; }
}

function ClearDMs() {
    if (dmPanel) { dmPanel.Clear(); }
}

/***************************************************
class GPS.DMPanel
***************************************************/
GPS.DMPanel = function(div) {
    this._changed = null;
    this._panelView = null;
    this._dmStore = null;

    this._submapStore = null;
    this._submapsTotal = null;
    this._submapsPenetration = null;
    
    this.__Init__(div);
    
    
}

GPS.DMPanel.prototype = {
    __Init__: function (div) {
        this._changed = false;
        this._dmStore = new GPS.DM.ObjectStore();
        this.__InitPanelView__(div);
    },

    __InitPanelView__: function (div) {
        this._panelView = new GPS.DMPanelView(div);
        var thisObj = this;
        var OnDMSelect = function (id) { thisObj.__OnDMSelect__(id); };
        var OnSubMapSelect = function (id) { thisObj.__OnSubMapSelect__(id); };
        var OnNewDM = function (dm) { thisObj.__OnNewDM__(dm); };
        var OnDeleteDM = function (dm) { thisObj.__OnDeleteDM__(dm); };
        var OnAttachDMId = function (dm) { thisObj.__AttachNewDMId__(dm); };
        var OnUpdateDM = function (dm) { thisObj.__OnUpdateDM__(dm); };
        var OnDMDoubleSelect = function (id) { thisObj.__OnDMDoubleSelect__(id); };
        this._panelView.AttachEvent('onnewdm', OnNewDM);
        this._panelView.AttachEvent('ondmselect', OnDMSelect);
        this._panelView.AttachEvent('onsubmapselect', OnSubMapSelect);
        this._panelView.AttachEvent('ondeletedm', OnDeleteDM);
        this._panelView.AttachEvent('attachdmid', OnAttachDMId);
        this._panelView.AttachEvent('onupdatedm', OnUpdateDM);
        this._panelView.AttachEvent('ondmdoubleselect', OnDMDoubleSelect);
        var OnEndDMEdit = function () { thisObj._changed = true; };
        var OnDMEdit = function () { if (map) { map.Refresh(); } };
        this._panelView.AttachEvent('onenddmedit', OnEndDMEdit);
    },



    __CalculateSubMaps: function () {
        this._submapsTotal = 0;
        this._submapsPenetration = 0;

        var submaps = this._dmStore.GetObjects();
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            this._submapsTotal += submaps[i].GetTotal();
            this._submapsPenetration += submaps[i].GetPenetration();
            i++;
        }

        var per = 0;
        if (this._submapsPenetration != 0) {
            per = this._submapsPenetration / this._submapsTotal * 100;
        }
        $('#lbCampaignPen').text(per.toFixed(2) + '%');
        $('#lbCampaignTotal').text(Number.addCommas(this._submapsTotal));
        $('#lbCampaignCount').text(Number.addCommas(this._submapsPenetration));
    },




    __AttachNewDMId__: function (dm) {
        var newId = parseInt(this._dmStore.GetMaxObjectId()) + 1;
        dm._id = newId;
    },

    __OnNewDM__: function (dm) {
        this._dmStore.Append(dm);
        //var campaignid = this._dmStore.GetCampaignId();


    },

    __OnDMSelect__: function (id) {
        var idarray = id.split('+');
        var dmid = idarray[0];
        var submapid = idarray[1];
        var dm = this._dmStore.GetById(dmid, submapid);
        if (dm) {
            this._panelView.ActiveDM(dm);
        }
    },


    __OnDMDoubleSelect__: function (id) {
        var idarray = id.split('+');
        var dmid = idarray[0];
        var submapid = idarray[1];
        var dm = this._dmStore.GetById(dmid, submapid);
        if (dm) {
            this._panelView.ActiveDM(dm);

            if (dm._centerLat != 0 && dm._centerLon != 0) {
                mapPanel.GetMap().SetCenter(new VELatLong(dm._centerLat, dm._centerLon));
            }
        }
    },

    __OnUpdateDM__: function (dm) {
        this._dmStore.Update(dm);
        //var campaignid = this._dmStore.GetCampaignId();

    },

    //////
    __OnSubMapSelect__: function (id) {
        //this._panelView._selectedsubmapId = id;

        this._panelView._activeDM = null;

        var submap = this._dmStore.GetSubmapById(id);
        if (submap) {
            //this._panelView.ActiveDM(dm);

            if (submap._centerLat != 0 && submap._centerLon != 0) {
                mapPanel.GetMap().SetCenter(new VELatLong(submap._centerLat, submap._centerLon));
            }
        }
    },

    __OnDeleteDM__: function (dm) {
        dm.RemoveShape();
        this._dmStore.Remove(dm);
        //var campaignid = this._dmStore.GetCampaignId();

        this._changed = true;
    },

    LoadDMs: function (dms) {
        this._dmStore.SetObjects(dms);
        this.Refresh();
        this._changed = false;

        this.__CalculateSubMaps();
    },

    GetEditDM: function () {
        return this._panelView.GetEditDM();
    },

    GetDMs: function () {
        return this._dmStore.GetObjects();

    },


    GetDMList: function () {
        return this._dmStore.GetObjects();
    },

    Refresh: function () {
        this._panelView.BindDMs(this._dmStore.GetObjects());

    },

    Clear: function () {
        this._dmStore.SetObjects([]);
        this.Refresh();
        this._changed = false;
    },

    GetActiveDM: function () { return this._panelView.GetActiveDM(); },
    GetActiveSubMap: function () {
        var dmap = this._panelView.GetActiveDM();
        if (dmap && dmap._submapid) {
            return this._dmStore.GetSubmapById(dmap._submapid);
        }
        return null;
    }

}

/***************************************************
class GPS.DMPanelView
***************************************************/

GPS.DMPanelView = function(div) {
    this._state = null;
    this._view = null;
    this._dmList = null;
    this._dmForm = null;
    this._activeDM = null;
    this._eventTrigger = null;
    this.__Init__(div);
    this._selectedsubmapId = null;
}

GPS.DMPanelView.prototype = {
    __Init__: function (div) {
        this._state = ViewState.Normal;
        this._view = $('#' + div);
        var prefix = div + '-';
        this.__InitMenuBar__(prefix);
        this.__InitDMListView__(prefix);
        this.__InitDMFormView__(prefix);
        this.__InitEvents__();
    },

    __InitMenuBar__: function (prefix) {
        this._menuBar = new GPS.DMMenuBarView(prefix);
        var thisObj = this;
        var OnMenuClick = function (action) { thisObj.__OnMenuClick__(action); };
        this._menuBar.AttachEvent('onmenuclick', OnMenuClick);
        $(this._view).append($(this._menuBar.GetView()));
    },

    __InitMapInfo__: function (prefix) {
        this._mapInfo = new GPS.DMInfoView(prefix);
        $(this._view).append($(this._mapInfo.GetView()));
    },

    __InitDMListView__: function (prefix) {
        this._dmList = new GPS.DMListView(prefix);
        var thisObj = this;
        var OnItemSelect = function (id) { thisObj.__OnListItemSelect__(id); };
        this._dmList.AttachEvent('onitemselect', OnItemSelect);

        var OnItemDoubleSelect = function (id) { thisObj.__OnListItemDoubleSelect__(id); };
        this._dmList.AttachEvent('onitemdoubleselect', OnItemDoubleSelect);

        var OnSubMapItemSelect = function (id) { thisObj.__OnSubMapItemSelect__(id); };
        this._dmList.AttachEvent('onsubmapitemselect', OnSubMapItemSelect);

        $(this._view).append($(this._dmList.GetView()));
    },

    __InitDMFormView__: function (prefix) {
        this._dmForm = new GPS.DMFormView(prefix);
        var thisObj = this;
        var OnFormButtonClick = function (action) { thisObj.__OnFormButtonClick__(action); };
        this._dmForm.AttachEvent('onformbuttonclick', OnFormButtonClick);
        $(this._view).append($(this._dmForm.GetView()));

        //add submaps dropdown list
        var formItems = [];
        formItems.push("<br/><span>Submap:</span>");
        formItems.push('<select id="' + prefix + 'dm-submapid" style="width:190px">');
        var args = GetUrlParms();
        var campaignid = args["cid"];
        var cmReader = new TIMM.Website.CampaignServices.CampaignReaderService();
        cmReader.GetCampaignByIdForDMList(campaignid, function (cmCurrent) {
            if (cmCurrent.SubMaps != null && cmCurrent.SubMaps.length > 0) {
                for (i = 0; i < cmCurrent.SubMaps.length; i++) {
                    formItems.push('<option value="' + cmCurrent.SubMaps[i].Id);
                    formItems.push('">' + cmCurrent.SubMaps[i].Name + '</option>');
                }
            } else {
                alert("No submap yet, please add submap to campaign firstly.");
            }
            formItems.push('</select><br/>');
            $('#distributionNew').append(formItems.join(''));
        }
        );
    },

    __InitEvents__: function () {
        this._eventTrigger = new GPS.EventTrigger();
    },

    __NewDM__: function () {
        var dm = new GPS.DM();
        this._eventTrigger.TriggerEvent('attachdmid', dm);
        var thisObj = this;
        return dm;
    },

    __OnDMAreasChange__: function (dm) {
        this._dmList.UpdateDM(dm);
        this._eventTrigger.TriggerEvent('ondmchange', dm);
    },

    __OnMenuClick__: function (action) {
        if (action == 'new') {
            //if (this._selectedsubmapId) {
            var dm = this.__NewDM__();
            //dm.SetSumMapId(this._selectedsubmapId);

            //dm.SetSumMapId(115179672);
            this._dmForm.SetDM(dm);
            this._dmForm.DataBind();
            this.__ShowDMForm__();
            var thisObj = this;
            var OnAreasChange = function (dm) { thisObj.__OnDMAreasChange__(dm); };
            dm.AttachEvent('onareaschange', OnAreasChange);
            //}
            //else { GPSAlert('Please select a sub map firstly before adding a new distribution map.'); }
        }
        else if (action == 'edit') {
            if (this._activeDM) {
                this._dmForm.SetDM(this._activeDM);
                this._dmForm.DataBind();
                this.__ShowDMForm__();
            }
            else { GPSAlert('To edit, please select a distribution map firstly.'); }
        }
        else if (action == 'delete') {
            if (this._activeDM) {
                //                this._activeDM = new GPS.DM();
                //                this._activeDM.SetName("campainid");
                //                this._activeDM.SetId(-1032455492);
                //                this._activeDM.SetSumMapId(768544273);
                var dm = this._activeDM;
                this._dmForm.SetDM(dm);
                var thisObj = this;
                var distributionmap = {
                    Id: dm._id,
                    Name: dm._name,
                    SubMapId: dm._submapid,
                    ColorB: dm._color.b,
                    ColorR: dm._color.r,
                    ColorG: dm._color.g,
                    ColorString: dm._colorString
                };
                var dmWriter = new TIMM.Website.DistributionMapServices.DMWriterService();
                dmWriter.DeleteDistributionMap(campaignid, distributionmap, function (ret) {
                    thisObj._eventTrigger.TriggerEvent('ondeletedm', thisObj._activeDM);
                    thisObj._dmList.Remove(thisObj._activeDM);
                    thisObj._activeDM = null;
                    updateNotIncludeAreas(ret);
                });



            }
            else { GPSAlert('To delete, please select a distribution map firstly.'); }
        }
    },

    __OnFormButtonClick__: function (action) {
        if (action == 'cancel') {
            this.__ShowDMList__();
        }
        else if (action == 'save') {
            this._dmForm.DataHold();
            var dm = this._dmForm.GetDM();
            var thisObj = this;
            if (this._activeDM != dm) {
                var distributionmap = {
                    Id: dm._id,
                    Name: dm._name,
                    SubMapId: dm._submapid,
                    ColorB: dm._color.b,
                    ColorR: dm._color.r,
                    ColorG: dm._color.g,
                    ColorString: dm._colorString
                };
                var dmWriter = new TIMM.Website.DistributionMapServices.DMWriterService();
                dmWriter.SaveDistributionMap(campaignid, distributionmap, function (ret) {
                    if (ret) {
                        dm._id = ret.Id;
                        thisObj._eventTrigger.TriggerEvent('onnewdm', dm);
                        thisObj._dmList.AppendNewDM(dm);
                        thisObj.ActiveDM(dm);
                    }
                });
            }
            else {
                var dm = this._activeDM;
                this._dmForm.SetDM(dm);
                var thisObj = this;

                var distributionmap = {
                    Id: dm._id,
                    Name: dm._name,
                    SubMapId: dm._submapid,
                    ColorB: dm._color.b,
                    ColorR: dm._color.r,
                    ColorG: dm._color.g,
                    ColorString: dm._colorString
                };
                var dmWriter = new TIMM.Website.DistributionMapServices.DMWriterService();
                dmWriter.UpdateDistributionMap(campaignid, distributionmap, function (ret) {
                    thisObj._eventTrigger.TriggerEvent('onupdatedm', dm);
                    thisObj._dmList.UpdateDM(dm);
                    dm.RefreshShape();
                });

            }

            this.__ShowDMList__();
        }
    },

    __OnListItemSelect__: function (id) {
        this._eventTrigger.TriggerEvent('ondmselect', id);
    },

    __OnListItemDoubleSelect__: function (id) {
        this._eventTrigger.TriggerEvent('ondmdoubleselect', id);
    },

    __OnSubMapItemSelect__: function (id) {
        this._eventTrigger.TriggerEvent('onsubmapselect', id);
    },

    __ShowDMList__: function () {
        this._dmList.Show();
        this._dmForm.Hide();
        this._state = ViewState.Normal;
        this._eventTrigger.TriggerEvent('onenddmedit');
    },

    __ShowDMForm__: function () {
        this._dmList.Hide();
        this._dmForm.Show();
        this._state = ViewState.Edit;
        this._eventTrigger.TriggerEvent('ondmedit');
    },

    ActiveDM: function (dm) {
        this._activeDM = dm;
    },

    GetActiveDM: function () {
        return this._activeDM;
    },

    AttachEvent: function (eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    GetEditDM: function () {
        var dm = null;
        if (this._state == ViewState.Edit) {
            dm = this._dmForm.GetDM();
        }
        return dm;
    },

    BindDMs: function (dms) {
        this._dmList.BindDMs(dms);
        this.__ShowDMList__();
        this._activeDM = null;

        var thisObj = this;

        var length = dms.length;
        var j = 0;
        while (j < length) {

            $.each(dms[j]._dms._dmrecords, function (i, dm) {
                var OnAreasChange = function (dm) { thisObj.__OnDMAreasChange__(dm); };
                dm.AttachEvent('onareaschange', OnAreasChange);
            });

            j++;
        }
    }
}


/***************************************************
enum ViewState
***************************************************/

ViewState = {
    Normal: 0,
    Edit: 1
}