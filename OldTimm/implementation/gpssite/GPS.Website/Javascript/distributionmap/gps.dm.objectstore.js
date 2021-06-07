
GPS.DM.ObjectStore = function() {
    this._objects = null;
    this.__Init__();
}

GPS.DM.ObjectStore.prototype = {
    __Init__: function() {
        this._objects = [];
    },

    Append: function(object) {
        var i = 0;
        var length = this._objects.length;

        while (i < length) {
            if (this._objects[i]._id == object._submapid) {
                this._objects[i]._dms._dmrecords.push(object);
                break;
            }
            i++;
        }
    },

    Update: function(object) {
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (this._objects[i]._id == object._submapid) {
                var j = 0;
                var dmlength = this._objects[i]._dms._dmrecords.length;
                while (j < dmlength) {
                    if (this._objects[i]._dms._dmrecords[j]._id == object._id) {
                        this._objects[i]._dms._dmrecords[j] = object;
                        break;
                    }
                    j++;
                }
                break;
            }
            i++;
        }
    },

    DeleteById: function(id) {
    },

    Remove: function(object) {
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (this._objects[i]._id == object._submapid) {
                var j = 0;
                var dmlength = this._objects[i]._dms._dmrecords.length;
                while (j < dmlength) {
                    if (this._objects[i]._dms._dmrecords[j]._id == object._id) {
                        this._objects[i]._dms._dmrecords.splice(j, 1);
                        break;
                    }
                    j++;
                }
                break;
            }
            i++;
        }
    },

    GetById: function(mdid, submapid) {
        var object = null;
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (this._objects[i]._id == submapid) {
                var j = 0;
                var dmlength = this._objects[i]._dms._dmrecords.length;
                while (j < dmlength) {
                    if (this._objects[i]._dms._dmrecords[j]._id == mdid) {
                        object = this._objects[i]._dms._dmrecords[j];
                        break;
                    }
                    j++;
                }
                break;
            }
            i++;
        }
        return object;
    },


    GetSubmapById: function(submapid) {
        var object = null;
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (this._objects[i]._id == submapid) {
                object = this._objects[i];
                break;
            }
            i++;
        }
        return object;
    },

    GetCampaignId: function() {
        if (this._objects.length > 0) {
            return this._objects[0]._campaignid;
        }
        else {
            return null;
        }
    },

    GetObjects: function() { return this._objects; },

    GetDMobjects: function() {
        var object = []
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            var j = 0;
            var dmlength = this._objects[i]._dms._dmrecords.length;
            while (j < dmlength) {
                var dm = new GPS.DM();
                dm._id = this._objects[i]._dms._dmrecords[j]._id;
                dm._name = this._objects[i]._dms._dmrecords[j]._name;
                dm._submapid = this._objects[i]._dms._dmrecords[j]._submapid;
                object.push(dm);
                j++;
            }
            i++;
        }
        return object;
    },

    SetObjects: function(objects) { return this._objects = objects; },

    GetMaxObjectId: function() {
        var maxId = 0;
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (this._objects[i]._dms._dmrecords.length > 0) {
                var j = 0;
                var dmlength = this._objects[i]._dms._dmrecords.length;
                while (j < dmlength) {
                    if (maxId < this._objects[i]._dms._dmrecords[j]._id) { maxId = this._objects[i]._dms._dmrecords[j]._id; }
                    j++;
                }
            }

            i++;
        }
        return maxId;
    },

    Clear: function() { this._objects = []; }

}