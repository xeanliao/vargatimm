
GPS.DJUSER.ObjectStore = function() {
    this._objects = null;
    this.__Init__();
}

GPS.DJUSER.ObjectStore.prototype = {
    __Init__: function() {
        this._objects = [];
    },

    Append: function(object) {
        this._objects.push(object);
    },

    Remove: function(object) {
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (this._objects[i]._userid == object._userid) {
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
            if (this._objects[i]._userid == id) {
                object = this._objects[i];
                break;
            }
            i++;
        }
        return object;
    },

    GetByFullName: function(fullname) {
        var object = null;
        var i = 0;
        var length = this._objects.length;
        while (i < length) {
            if (this._objects[i]._fullname == fullname) {
                object = this._objects[i];
                break;
            }
            i++;
        }
        return object;
    },

    GetObjects: function() { return this._objects; },


    SetObjects: function(objects) { return this._objects = objects; },

    Clear: function() { this._objects = []; }

}