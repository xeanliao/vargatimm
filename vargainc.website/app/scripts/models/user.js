define([
    'underscore',
    'backbone',
    'pubsub'
], function (_, Backbone, Topic) {
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
            var model = this,
                url = model.urlRoot + '/info',
                options = {
                    url: url,
                    type: 'GET',
                    success: function (result) {
                        if (result && result.success) {
                            model.set(result.data);
                        } else {
                            Topic.publish("NOT_LOGIN");
                        }
                    },
                    error: function () {
                        Topic.publish("NOT_LOGIN");
                    }
                };
            _.extend(options, opts);

            return (this.sync || Backbone.sync).call(this, 'read', this, options);
        },
        logout: function (opts) {
            var model = this,
                url = model.urlRoot + '/logout',
                options = {
                    url: url,
                    type: 'POST',
                    success: function (result) {
                        Topic.publish("NOT_LOGIN");
                    },
                    error: function () {
                        Topic.publish("NOT_LOGIN");
                    }
                };
            _.extend(options, opts);

            return (this.sync || Backbone.sync).call(this, null, this, options);
        }
    });
});