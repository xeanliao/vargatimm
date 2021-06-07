
GPS.DJ.ObjectStore = function() {
    this._objects = null;
    this.__Init__();
}

GPS.DJ.ObjectStore.prototype = {
    __Init__: function() {
        this._objects = [];
    },

    Append: function(object) {
        this._objects.push(object);
    },

    Update: function(object) {
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (this._objects[i]._id == object._id) {
                this._objects[i]._name = object._name;
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
            if (this._objects[i] == object) {
                this._objects.splice(i, 1);
                break;
            }
            i++;
        }
    },

    GetById: function(id) {
        var object = null;
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (this._objects[i]._id == id) {
                object = this._objects[i];
                break;
            }
            i++;
        }
        return object;
    },

    GetObjects: function() { return this._objects; },

    GetDJobjects: function() {
        var object = []
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            var j = 0;
            var dmlength = this._objects[i]._djs._djrecords.length;
            while (j < dmlength) {
                var dm = new GPS.DJ();
                dm._id = this._objects[i]._djs._djrecords[j]._id;
                dm._name = this._objects[i]._djs._djrecords[j]._name;
                dm._submapid = this._objects[i]._djs._djrecords[j]._submapid;
                object.push(dm);
                j++;
            }
            i++;
        }
        return object;
    },

    SetObjects: function(objects) {
        return this._objects = objects;
    },

    GetMaxObjectId: function() {
        var maxId = 0;
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (maxId < this._objects[i]._id) {
                maxId = this._objects[i]._id;
            }
            i++;
        }
        return maxId;
    },

    Clear: function() { this._objects = []; },

    CheckGTUIsUsed: function(object) {
        var isused = false;
        var i = 0;
        var j = 0;
        //check auditor
        if (null != this._objects._auditor)
        {
            if (this._objects._auditor.UniqueID == object._gtuuniqueid & this._objects._auditor.LoginUserId != object._userid) {
                isused = true;
            } 
        }
        //check walkers
        if (!isused) {
            while (i < this._objects._walkers.length) {
                if (this._objects._walkers[i].UniqueID == object._gtuuniqueid & this._objects._walkers[i].LoginUserId != object._userid) {
                    isused = true;
                    break;
                }
                i++;
            }
        }
        //check drivers
        if (!isused) {
            while (j < this._objects._drivers.length) {
                if (this._objects._drivers[j].UniqueID == object._gtuuniqueid & this._objects._drivers[j].LoginUserId != object._userid) {
                    isused = true;
                    break;
                }
                j++;
            }
        }
        return isused;
    },

    UpdateAuditorGtu: function(object) {
        if (this._objects._auditor.LoginUserId == object._userid)
            this._objects._auditor.UniqueID = object._gtuuniqueid;
    },

    UpdateWalkerGtu: function(object) {
        var i = 0;
        var length = this._objects._walkers.length;
        while (i < length) {
            if (this._objects._walkers[i].LoginUserId == object._userid) {
                this._objects._walkers[i].UniqueID = object._gtuuniqueid;
                break;
            }
            i++;
        }
    },

    UpdateDriverGtu: function(object) {
        var i = 0;
        var length = this._objects._drivers.length;
        while (i < length) {
            if (this._objects._drivers[i].LoginUserId == object._userid) {
                this._objects._drivers[i].UniqueID = object._gtuuniqueid;
                break;
            }
            i++;
        }
    }
}