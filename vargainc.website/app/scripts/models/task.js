define([
    'jquery',
    'underscore',
    'backbone',
    'pubsub'
], function ($, _, Backbone, Topic) {
    return Backbone.Model.extend({
        urlRoot: 'task',
        idAttribute: 'Id',
        defaults: {
            Id: null,
            Name: null,
            Date: null,
            Status: null,
            Telephone: null,
            Email: null,
            DistributionMapId: null,
            AuditorId: null,
            AuditorName: null,
            DistributionMapId: null
        },
        markFinished: function (opts) {
            var model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/finish/',
                    method: 'PUT'
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        },
        reOpen: function (opts) {
            var model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/reopen/',
                    method: 'PUT'
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        },
        importGtu: function (file, opts) {
            var model = this,
                options = {
                    url: '../api/' + model.urlRoot + '/' + model.get('Id') + '/import/',
                    method: 'PUT'
                };
            options = _.extend(opts, options);

            var xhr = new XMLHttpRequest();
            var fd = new FormData();

            xhr.open(options.method, options.url, true);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4 && xhr.status == 200) {
                    //console.log(xhr.responseText);
                    options.success && options.success.call(model, JSON.parse(xhr.responseText));
                }
            };
            fd.append('gtuFile', file);
            xhr.send(fd);
        },
        addGtuDots: function (dots, opts) {
            var def = $.Deferred(),
                model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/dots/',
                    method: 'POST',
                    data: $.param({
                        '': dots
                    }),
                    success: function (result) {
                        if (result && result.success) {
                            return def.resolve();
                        }
                        return def.reject();
                    },
                    fail: function () {
                        def.reject();
                    }
                };
            options = _.extend(opts, options);

            (this.sync || Backbone.sync).call(this, '', this, options);
            return def;
        },
        setStart: function(opts){
            var model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/start/',
                    method: 'PUT',
                    success: function(result){
                        if(result && result.success){
                            model.set({
                                Status: result.status
                            });
                        }
                    }
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        },
        setPause: function(opts){
            var model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/pause/',
                    method: 'PUT',
                    success: function(result){
                        if(result && result.success){
                            model.set({
                                Status: result.status
                            });
                        }
                    }
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        },
        setStop: function(opts){
            var model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/stop/',
                    method: 'PUT',
                    success: function(result){
                        if(result && result.success){
                            model.set({
                                Status: result.status
                            });
                        }
                    }
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        }
    });
});