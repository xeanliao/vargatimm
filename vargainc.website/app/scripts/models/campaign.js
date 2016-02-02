define([
    'jquery',
    'underscore',
    'backbone'
], function ($, _, Backbone) {
    return Backbone.Model.extend({
        urlRoot: 'campaign',
        idAttribute: 'Id',
        defaults: {
            'AreaDescription': null,
            'ClientCode': null,
            'ClientName': null,
            'ContactName': null,
            'CreatorName': null,
            'CustemerName': null,
            'Date': null,
            'Description': null,
            'Id': null,
            'Name': null,
            'Sequence': 0,
            'UserName': null
        },
        getDisplayName: function(){
            return this.get('ClientCode') + '-' + this.get('CreatorName') + '-' + this.get('AreaDescription');
        },
        copy: function (opts) {
            var model = this,
                options = {
                    method: 'POST',
                    url: model.urlRoot + '/copy/' + model.get('Id')
                };
            _.extend(options, opts);

            return (this.sync || Backbone.sync).call(this, 'create', this, options);
        },
        publishToDMap: function (user, opts) {
            var model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/publish/',
                    method: 'PUT',
                    processData: true,
                    data: $.param({
                        '': user.get('Id')
                    })
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        },
        dismissToCampaign: function (user, opts) {
            var model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/dismiss/',
                    method: 'PUT',
                    processData: true,
                    data: $.param({
                        '': user.get('Id')
                    })
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        },
        publishToMonitor: function (user, opts) {
            var model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/monitor/',
                    method: 'PUT',
                    processData: true,
                    data: $.param({
                        '': user.get('Id')
                    })
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        },
        dismissToDMap: function (user, opts) {
            var model = this,
                options = {
                    url: model.urlRoot + '/' + model.get('Id') + '/dismiss/',
                    method: 'PUT',
                    processData: true,
                    data: $.param({
                        '': user.get('Id')
                    })
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        }
    });
});