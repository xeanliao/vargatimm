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
        addEmployee: function (file, opts) {
            var def = $.Deferred(),
                model = this,
                options = {
                    url: '../api/' + model.urlRoot + '/employee/',
                    method: 'POST'
                };
            options = _.extend(opts, options);

            var xhr = new XMLHttpRequest();
            var fd = new FormData();

            xhr.open(options.method, options.url, true);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    if (xhr.status == 200) {
                        var result = JSON.parse(xhr.responseText);
                        model.set(result);
                        options.success && options.success.call(model, result);
                        def.resolve();
                    } else {
                        def.reject();
                    }
                }
            };
            fd.append('Picture', file);
            fd.append('CompanyId', model.get('CompanyId'));
            fd.append('Role', model.get('Role'));
            fd.append('FullName', model.get('FullName'));
            fd.append('CellPhone', model.get('CellPhone'));
            fd.append('DateOfBirth', model.get('DateOfBirth'));
            fd.append('Notes', model.get('Notes'));
            xhr.send(fd);
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