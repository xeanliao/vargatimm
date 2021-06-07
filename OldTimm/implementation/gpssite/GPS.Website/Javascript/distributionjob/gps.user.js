/***************************************************
class GPS.USER
***************************************************/
GPS.USER = function() {
    this._id = null;
    this._fullname = null;
    this._role = null;
    this._eventTrigger = null;
    this.__Init__();
}

GPS.USER.prototype = {
    __Init__: function() {
        this._id = null;
        this._fullname = '';
        this._role = null;
        this._eventTrigger = new GPS.EventTrigger();
    },

    GetId: function() { return this._id; },

    GetFullName: function() { return this._fullname; },

    GetRole: function() { return this._role; },

    SetId: function(id) { this._id = id; },

    SetFullName: function(fullname) { this._fullname = fullname; },

    SetRole: function(role) { this._role = role; },

    AttachEvent: function(eventName, eventHandler) {
        this._eventTrigger.AttachEvent(eventName, eventHandler);
    }
}