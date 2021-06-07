
GPS.Map.ObjectStore = function() {
    this._objects = null;
    this.__Init__();
}

GPS.Map.ObjectStore.prototype = {
    __Init__: function() {
        this._objects = [];
    },

    Append: function(object) {
        this._objects.push(object);
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
            if (this._objects[i].GetId() == id) {
                object = this._objects[i];
                break;
            }
            i++;
        }
        return object;
    },

    GetObjects: function() { return this._objects; },

    SetObjects: function(objects) { return this._objects = objects; },

    GetMaxObjectId: function() {
        var maxId = 0;
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (maxId < this._objects[i].GetId()) { maxId = this._objects[i].GetId(); }
            i++;
        }
        return maxId;
    },

    Clear: function() { this._objects = []; }

}