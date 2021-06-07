

var djPanel = null;

function BindDJs(djDatas, campaignid) {
    if (djPanel) {
        var djs = [];
        var i = 0;
        var length = djDatas.length;
        while (i < length) {
            var dj = new GPS.DJ(djDatas[i]);
            djs.push(dj);
            i++;
        }
        djPanel.LoadDJs(djs, campaignid);
    }
}

function GetEditDJ() {
    var dj = null;
    if (djPanel) { dj = djPanel.GetEditDJ(); }
    return dj;
}

function GetDJsList() {
    var djs = null;
    if (djPanel) {
        djs = djPanel.GetDJList();
    }

    return djs;
}

function DJChanged() {
    if (djPanel) { return djPanel._changed; }
    else { return false; }
}

function UpdateDJChanged() {
    if (djPanel) { djPanel._changed = false; }
}

function ClearDJs() {
    if (djPanel) { djPanel.Clear(); }
}

/***************************************************
class GPS.DJPanel
***************************************************/
GPS.DJPanel = function(div) {
    this._changed = null;
    this._panelView = null;
    this._djStore = null;
    this.__Init__(div);

}

GPS.DJPanel.prototype = {
    __Init__: function(div) {
        this._changed = false;
        this._djStore = new GPS.DJ.ObjectStore();
        this.__InitPanelView__(div);
    },

    __InitPanelView__: function(div) {
        this._panelView = new GPS.DJPanelView(div);
        var thisObj = this;
        var OnDMSelect = function(id) { thisObj.__OnDMSelect__(id); };
        var OnDJItemSelect = function(id) { thisObj.__OnDJItemSelect__(id); };
        var OnNewDJ = function(dj) { thisObj.__OnNewDJ__(dj); };
        var OnDeleteDJ = function(dj) { thisObj.__OnDeleteDJ__(dj); };
        var OnAttachDJId = function(dj) { thisObj.__AttachNewDJId__(dj); };
        var OnUpdateDJ = function(dj) { thisObj.__OnUpdateDJ__(dj); };
        var OnAssignDriversSelect = function(id) { thisObj.__OnAssignDriversSelect__(id); };
        var OnAssignAuditorsSelect = function(id) { thisObj.__OnAssignAuditorsSelect__(id); };
        var OnAssignWalkersSelect = function(id) { thisObj.__OnAssignWalkersSelect__(id); };
        var OnAssignGtusSelect = function(id) { thisObj.__OnAssignGtusSelect__(id); };

        this._panelView.AttachEvent('onnewdj', OnNewDJ);
        this._panelView.AttachEvent('ondmselect', OnDMSelect);
        this._panelView.AttachEvent('ondjselect', OnDJItemSelect);
        this._panelView.AttachEvent('ondeletedj', OnDeleteDJ);
        this._panelView.AttachEvent('attachdjid', OnAttachDJId);
        this._panelView.AttachEvent('onupdatedj', OnUpdateDJ);
        this._panelView.AttachEvent('assigndriversonclick', OnAssignDriversSelect);
        this._panelView.AttachEvent('assignauditorsonclick', OnAssignAuditorsSelect);
        this._panelView.AttachEvent('assignwalkersonclick', OnAssignWalkersSelect);
        this._panelView.AttachEvent('assigngtusonclick', OnAssignGtusSelect);

        var OnEndDJEdit = function() { thisObj._changed = true; };
        var OnDJEdit = function() { if (map) { map.Refresh(); } };
        this._panelView.AttachEvent('onenddjedit', OnEndDJEdit);
    },

    __AttachNewDJId__: function(dj) {
        var newId = parseInt(this._djStore.GetMaxObjectId()) + 1;
        dj._id = newId;
    },

    __OnNewDJ__: function(dj) {
        var obj = this;
        var panelViewObj = this._panelView;
        var distributionjob = {
            Name: dj._name,
            CampaignID: this.GetCurrentCampainId()
        };

        var djWriter = new TIMM.Website.DistributionMapServices.DJWriterService();
        djWriter.SaveDistributionJob(distributionjob, function(r) {
            if (r != 0) {
                dj._id = r;
                obj._djStore.Append(dj);
                obj._panelView._djList.Append(dj, null);
                panelViewObj.ActiveDJ(dj);
            }
        }, null, null);
    },

    __OnDJItemSelect__: function(id) {
        var dj = this._djStore.GetById(id);
        if (dj) {
            this._panelView.ActiveDJ(dj);
        }
    },

    __OnUpdateDJ__: function(dj) {
        this._djStore.Update(dj);
        var distributionjob = {
            Id: dj._id,
            Name: dj._name,
            CampaignID: this.GetCurrentCampainId()
        };
        var djWriter = new TIMM.Website.DistributionMapServices.DJWriterService();
        djWriter.UpdateDistributionJob(distributionjob);
    },

    //////
    __OnDMSelect__: function(id) {
        var idarray = id.split('^');
        var djid = idarray[0];
        var dmid = idarray[1];
        this._panelView._dmjselected.SetDJId(djid);
        this._panelView._dmjselected.SetDMId(dmid);
    },

    //assign drivers to the distribution job
    __OnAssignDriversSelect__: function(id) {
        $("#drivers-list").hide();
        $("#users-list").hide();
        $("#drivermenu-div").hide();
        ShowAssignDriverDialog('#assign_drivers_dialog');
        $("#assigndrivers-progressbar_section").show();
        LoadAll(id);
    },

    //assign auditor to the distribution job
    __OnAssignAuditorsSelect__: function(id) {
        $("#auditors-list").hide();
        $("#auditor-users-list").hide();
        $("#auditormenu-div").hide();
        ShowAssignAuditorDialog('#assign_auditors_dialog');
        $("#assignauditors-progressbar_section").show();
        LoadAuditors(id);
    },

    //assign walkers to the distribution job
    __OnAssignWalkersSelect__: function(id) {
        $("#walkers-list").hide();
        $("#walkers-users-list").hide();
        $("#walkermenu-div").hide();
        ShowAssignWalkersDialog('#assign_walkers_dialog');
        $("#assignwalkers-progressbar_section").show();
        LoadWalkers(id);
    },

    //assign walkers to the distribution job
    __OnAssignGtusSelect__: function(id) {
        $("#gtus-users-list").hide();
        ShowAssignGtusDialog('#assign_gtus_dialog');
        $("#assigngtus-progressbar_section").show();
        LoadGtus(id);
    },

    __OnDeleteDJ__: function(dj) {
        this._djStore.Remove(dj);
        var distributionjob = {
            Id: dj._id,
            Name: dj._name,
            CampaignID: this.GetCurrentCampainId()
        };
        var djWriter = new TIMM.Website.DistributionMapServices.DJWriterService();
        djWriter.DeleteDistributionJob(distributionjob);
        this._changed = true;
    },

    LoadDJs: function(djs, campaignid) {
        this._djStore.SetObjects(djs);
        this.SetCurrentCampainId(campaignid);
        this.Refresh();
        this._changed = false;
    },

    GetEditDJ: function() {
        return this._panelView.GetEditDJ();
    },

    GetDJs: function() {
        return this._djStore.GetObjects();

    },


    GetDJList: function() {
        //return this._djStore.GetDJobjects();
        return this._djStore.GetObjects();
    },

    SetCurrentCampainId: function(campaignid) {
        this._panelView._currentcampaign = campaignid;
    },

    GetCurrentCampainId: function() {
        return this._panelView._currentcampaign;
    },


    Refresh: function() {
        this._panelView.BindDJs(this._djStore.GetObjects());
    },

    Clear: function() {
        this._djStore.SetObjects([]);
        this.Refresh();
        this._changed = false;
    },

    GetActiveDJ: function() { return this._panelView.GetActiveJM(); }

}

/***************************************************
class GPS.DJPanelView
***************************************************/

GPS.DJPanelView = function(div) {
    this._state = null;
    this._view = null;
    this._djList = null;
    this._djForm = null;
    this._activeDJ = null;
    this._eventTrigger = null;
    this.__Init__(div);
    this._dmjselected = new GPS.DJM();
    this._currentcampaign = null;
}

GPS.DJPanelView.prototype = {
    __Init__: function(div) {
        this._state = ViewState.Normal;
        this._view = $('#' + div);
        var prefix = div + '-';
        this.__InitMenuBar__(prefix);
        this.__InitDJListView__(prefix);
        this.__InitDJFormView__(prefix);
        this.__InitEvents__();
    },

    __InitMenuBar__: function(prefix) {
        this._menuBar = new GPS.DJMenuBarView(prefix);
        var thisObj = this;
        var OnMenuClick = function(action) { thisObj.__OnMenuClick__(action); };
        this._menuBar.AttachEvent('onmenuclick', OnMenuClick);
        $(this._view).append($(this._menuBar.GetView()));
    },

    __InitMapInfo__: function(prefix) {
        this._mapInfo = new GPS.DJInfoView(prefix);
        $(this._view).append($(this._mapInfo.GetView()));
    },

    __InitDJListView__: function(prefix) {
        this._djList = new GPS.DJListView(prefix);
        var thisObj = this;
        var OnItemSelect = function(id) { thisObj.__OnListItemSelect__(id); };
        this._djList.AttachEvent('ondmitemselect', OnItemSelect);

        var OnDistributionJobItemSelect = function(id) { thisObj.__OnDistributionJobItemSelect__(id); };
        this._djList.AttachEvent('ondjitemselect', OnDistributionJobItemSelect);

        var OnAssignDriversSelect = function(id) { thisObj.__OnAssignDriversSelect__(id); };
        this._djList.AttachEvent('onassigndriversclick', OnAssignDriversSelect);

        var OnAssignAuditorsSelect = function(id) { thisObj.__OnAssignAuditorsSelect__(id); };
        this._djList.AttachEvent('onassignauditorsclick', OnAssignAuditorsSelect);

        var OnAssignWalkersSelect = function(id) { thisObj.__OnAssignWalkersSelect__(id); };
        this._djList.AttachEvent('onassignwalkersclick', OnAssignWalkersSelect);

        var OnAssignGtusSelect = function(id) { thisObj.__OnAssignGtusSelect__(id); };
        this._djList.AttachEvent('onassigngtusclick', OnAssignGtusSelect);

        $(this._view).append($(this._djList.GetView()));
    },

    __InitDJFormView__: function(prefix) {
        this._djForm = new GPS.DJFormView(prefix);
        var thisObj = this;
        var OnFormButtonClick = function(action) { thisObj.__OnFormButtonClick__(action); };
        this._djForm.AttachEvent('onformbuttonclick', OnFormButtonClick);
        $(this._view).append($(this._djForm.GetView()));
    },

    __InitEvents__: function() {
        this._eventTrigger = new GPS.EventTrigger();
    },

    __NewDJ__: function() {
        var dj = new GPS.DJ();
        this._eventTrigger.TriggerEvent('attachdjid', dj);
        var thisObj = this;
        return dj;
    },

    __OnDJAreasChange__: function(dj) {
        this._djList.UpdateDJ(dj);
        this._eventTrigger.TriggerEvent('ondjchange', dj);
    },

    __OnMenuClick__: function(action) {
        if (action == 'new') {
            var dj = this.__NewDJ__();
            dj.SetCampaign(this._currentcampaign);
            this._djForm.SetDJ(dj);
            this._djForm.DataBind();
            this.__ShowDJForm__();
        }
        else if (action == 'edit') {
            if (this._activeDJ) {
                this._djForm.SetDJ(this._activeDJ);
                this._djForm.DataBind();
                this.__ShowDJForm__();
            }
            else { GPSAlert('To edit, please select a distribution job firstly.'); }
        }
        else if (action == 'delete') {
            if (this._activeDJ) {
                this._eventTrigger.TriggerEvent('ondeletedj', this._activeDJ);
                this._djList.Remove(this._activeDJ);
                this._activeDJ = null;
            }
            else { GPSAlert('To delete, please select a distribution job firstly.'); }
        }
    },

    __OnFormButtonClick__: function(action) {
        if (action == 'cancel') {
            this.__ShowDJList__();
        }
        else if (action == 'save') {
            this._djForm.DataHold();
            var dj = this._djForm.GetDJ();
            if (this._activeDJ != dj) {
                this._eventTrigger.TriggerEvent('onnewdj', dj);
            }
            else {
                var dj = this._activeDJ;
                this._djForm.SetDJ(dj);
                this._eventTrigger.TriggerEvent('onupdatedj', dj);
                this._djList.UpdateDJ(dj);
            }

            this.__ShowDJList__();
        }
    },

    __OnListItemSelect__: function(id) {
        this._eventTrigger.TriggerEvent('ondmselect', id);
    },

    __OnDistributionJobItemSelect__: function(id) {
        this._eventTrigger.TriggerEvent('ondjselect', id);
    },

    __OnAssignDriversSelect__: function(id) {
        this._eventTrigger.TriggerEvent('assigndriversonclick', id);
    },

    __OnAssignAuditorsSelect__: function(id) {
        this._eventTrigger.TriggerEvent('assignauditorsonclick', id);
    },

    __OnAssignWalkersSelect__: function(id) {
        this._eventTrigger.TriggerEvent('assignwalkersonclick', id);
    },

    __OnAssignGtusSelect__: function(id) {
        this._eventTrigger.TriggerEvent('assigngtusonclick', id);
    },

    __ShowDJList__: function() {
        this._djList.Show();
        this._djForm.Hide();
        this._state = ViewState.Normal;
        this._eventTrigger.TriggerEvent('onenddjedit');
    },

    __ShowDJForm__: function() {
        this._djList.Hide();
        this._djForm.Show();
        this._state = ViewState.Edit;
        this._eventTrigger.TriggerEvent('ondjedit');
    },

    ActiveDJ: function(dj) {
        this._activeDJ = dj;
    },

    GetActiveDJ: function() {
        return this._activeDJ;
    },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    GetEditDJ: function() {
        var dj = null;
        if (this._state == ViewState.Edit) {
            dj = this._djForm.GetDJ();
        }
        return dj;
    },

    BindDJs: function(djs) {
        if (null != djs & djs.length > 0) {
            this._djList.BindDJs(djs);
        }
        this.__ShowDJList__();
        this._activeDJ = null;
        var thisObj = this;
        $.each(djs, function(i, dj) {
            var OnAreasChange = function(dj) { thisObj.__OnDJAreasChange__(dj); };
            dj.AttachEvent('onareaschange', OnAreasChange);
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