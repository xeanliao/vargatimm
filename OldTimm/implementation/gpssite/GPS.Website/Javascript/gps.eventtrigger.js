/*
*   Class GPS.EventTrigger - trigger event
*
*   Public methods:
* 		AttachEvent
*       TriggerEvent
*/
GPS.EventTrigger = Class.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function() {
        this._triggers = [];
    },
    // get event trigger by event name
    _GetTrigger: function(eventName) {
        var trigger = null;
        for (var i = 0; i < this._triggers.length; i++) {
            if (this._triggers[i][0] == eventName) {
                trigger = this._triggers[i][1];
                break;
            }
        }
        return trigger;
    },
    // attach event in event triggers
    AttachEvent: function(eventName, eventHandler) {
        var trigger = this._GetTrigger(eventName);
        if (!trigger) {
            trigger = new Array();
            this._triggers.push([eventName, trigger]);
        }
        trigger.push(eventHandler);
    },
    // trigger event by event name
    TriggerEvent: function(eventName, e) {
        var trigger = this._GetTrigger(eventName);
        if (trigger) {
            for (var i = 0; i < trigger.length; i++) {
                trigger[i](e);
            }
        }
    }
});

//GPS.EventTrigger = function() {
//    this._triggers = null;
//    this._Init();
//}
//GPS.EventTrigger.prototype = {
//    // initalization method
//    _Init: function() {
//        this._triggers = new Array();
//    },
//    // get event trigger by event name
//    _GetTrigger: function(eventName) {
//        var trigger = null;
//        for (var i = 0; i < this._triggers.length; i++) {
//            if (this._triggers[i][0] == eventName) {
//                trigger = this._triggers[i][1];
//                break;
//            }
//        }
//        return trigger;
//    },
//    // attach event in event triggers
//    AttachEvent: function(eventName, eventHandler) {
//        var trigger = this._GetTrigger(eventName);
//        if (!trigger) {
//            trigger = new Array();
//            this._triggers.push([eventName, trigger]);
//        }
//        trigger.push(eventHandler);
//    },
//    // trigger event by event name
//    TriggerEvent: function(eventName, e) {
//        var trigger = this._GetTrigger(eventName);
//        if (trigger) {
//            for (var i = 0; i < trigger.length; i++) {
//                trigger[i](e);
//            }
//        }
//    }
//}