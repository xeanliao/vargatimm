/***************************************************
class GPS.DJUSER
***************************************************/
GPS.DJUSER = function(djObj) {
    this._fullname = null;
    this._djrole = null;
    this._gtuuniqueid = null;
    this._userid = null;
    this._distributionjobid = null;
    this._eventTrigger = null;
    this.__Init__(djObj);
}

GPS.DJUSER.prototype = {
    __Init__: function(djObj) {
    if (djObj) {
            this._fullname = djObj.FullName;
            this._djrole = djObj.DjRole;
            this._gtuid = djObj.GtuId;
            this._userid = djObj.LoginUserId;
            this._distributionjobid = djObj.DistributionJobId;
            this._eventTrigger = new GPS.EventTrigger();
        }
        else {
            this._fullname = '';
            this._djrole = null;
            this._gtuid = null;
            this._userid = null;
            this._distributionjobid = null;
            this._eventTrigger = new GPS.EventTrigger();
         }
    },

    GetFullName: function() { return this._fullname; },

    GetDjRole: function() { return this._djrole; },

    GetGtuId: function() { return this._gtuid; },

    GetUserId: function() { return this._userid; },

    GetDistributionJobId: function() { return this._distributionjobid; },

    SetFullName: function(fullname) { this._fullname = fullname; },

    SetDjRole: function(djrole) { this._djrole = djrole; },

    SetGtuId: function(gtuid) { this._gtuid = gtuid; },

    SetUserId: function(userid) { this._userid = userid; },

    SetDistributionJobId: function(id) { this._distributionjobid = id; },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    }

}
