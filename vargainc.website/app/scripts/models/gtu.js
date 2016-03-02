define([
	'underscore',
	'backbone',
	'pubsub'
], function (_, Backbone, Topic) {
	return Backbone.Model.extend({
		urlRoot: 'task',
        idAttribute: 'Id',
        defaults: {
            Id: null,
            ShortUniqueID: null,
            UserColor: null
        }
	});
});