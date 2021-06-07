/*
*   Class GPS.Container - Create object container
*
*   Public Functions
*       Get
*/
GPS.Container = function() {
    var mappings = [];
    mappings.push({
        Id: "mapbase.arealayer",
        IsSingleton: false,
        ClassObject: GPS.Map.AreaLayerBase,
        Instance: null
    });
    mappings.push({
        Id: "arealayerbase.area",
        IsSingleton: false,
        ClassObject: GPS.Map.AreaBase,
        Instance: null
    });
    return {
        //store mapping objects
        _mappings: mappings,
        // register mapping
        Register: function(mapping) {
            this._mappings.push(mapping);
        },
        // get object instance
        Get: function(key, options) {
            // find mapping
            var mapping = Array.selectSingle(this._mappings, function(value) {
                if (value.Id == key) {
                    return true;
                }
                else {
                    return false;
                }
            });
            // define return instance, default equals null
            var instance = null;
            // get instance by mapping
            if (mapping) {
                if (mapping.IsSingleton) {
                    if (mapping.Instance == null) {
                        mapping.Instance = new mapping.ClassObject(options);
                    }
                    instance = mapping.Instance;
                }
                else {
                    instance = new mapping.ClassObject(options);
                }
            }
            return instance;
        }
    };
} ();