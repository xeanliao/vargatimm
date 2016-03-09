define([
    'jquery',
    'underscore',
    'backbone'
], function ($, _, Backbone) {
    return Backbone.Model.extend({
        urlRoot: 'user',
        idAttribute: 'Id',
        defaults: {
            Id: null,
            UserCode: null,
            UserName: null,
            FullName: null,
            Emai: null,
            Token: null
        },
        fetchCurrentUser: function (opts) {
            var def = $.Deferred(),
                model = this,
                url = model.urlRoot + '/info',
                options = {
                    url: url,
                    type: 'GET',
                    success: function (result) {
                        if (result && result.success) {
                            model.set(result.data);
                            def.resolve();
                        } else {
                            def.reject();
                        }
                    },
                    fail: function () {
                        def.reject();
                    }
                };
            _.extend(options, opts);

            (this.sync || Backbone.sync).call(this, 'read', this, options);
            return def;
        },
        logout: function (opts) {
            var model = this,
                url = model.urlRoot + '/logout',
                options = {
                    url: url,
                    type: 'POST'
                };
            _.extend(options, opts);

            return (this.sync || Backbone.sync).call(this, null, this, options);
        }
    });
});