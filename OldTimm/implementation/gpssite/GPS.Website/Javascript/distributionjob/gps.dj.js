/***************************************************
class GPS.DJ
***************************************************/
GPS.DJ = function(djObj) {
    this._isNew = null;
    this._id = null;
    this._name = null;
    this._campaign = null;
    this._dms = null;
    this._walkers = null;
    this._auditor = null;
    this._drivers = null;
    this._changed = null;
    this._eventTrigger = null;
    this.__Init__(djObj);
}

GPS.DJ.prototype = {
    __Init__: function(djObj) {
        if (djObj) {
            //this._dmrecords = [];
            this._isNew = true;
            this._id = djObj.Id;
            this._name = djObj.Name;
            this._campaign = djObj.Campaign;
            this._dms = new GPS.DJDMList(djObj.DistributionMaps);
            this._dms.GetDMRecords();
            this._walkers = djObj.WalkerAssignments;
            this._auditor = djObj.AuditorAssignment;
            this._drivers = djObj.DriverAssignments;
            this._eventTrigger = new GPS.EventTrigger();
        }
        else {
            this._isNew = true;
            this._id = Math.random().toString().replace('.', '');
            this._name = '';
            this._campaign = null;
            this._dms = new GPS.DMList();
            this._walkers = null;
            this._auditor = null;
            this._drivers = null;
            this._eventTrigger = new GPS.EventTrigger();
        }
    },

    GetId: function() { return this._id; },

    GetName: function() { return this._name; },

    GetCampaign: function() { return this._campaign; },

    GetWalkers: function() { return this._walkers; },

    GetDrivers: function() { return this._drivers; },

    GetAuditor: function() { return this._auditor; },

    GetDMs: function() { return this._dms; },

    SetName: function(name) { this._name = name; },

    SetId: function(id) { this._id = id; },

    SetDMs: function(dms) { this._dms = dms; },

    SetCampaign: function(campaign) { this._campaign = campaign; },

    SetWalkers: function(walkers) { this._walkers = walkers; },

    SetDrivers: function(drivers) { this._drivers = drivers; },

    SetAuditor: function(auditor) { this._auditor = auditor; },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    },

    Serialize: function() {
        var array = [];
        array.push(this._id);
        array.push(this._name);
        array.push(this._dms.GetDMRecords());
        return array.join('^');
    }
}

/***************************************************
class GPS.DJM
***************************************************/
GPS.DJM = function() {
    this._djid = null;
    this._dmid = null;
    this._eventTrigger = null;
    this.__Init__();
}

GPS.DJM.prototype = {
    __Init__: function() {
        this._djid = null;
        this._dmid = null;
    },

    GeDJId: function() { return this._djid; },

    GetDMId: function() { return this._dmid; },

    SetDJId: function(id) { this._djid = id; },

    SetDMId: function(id) { this._dmid = id; },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    }
}