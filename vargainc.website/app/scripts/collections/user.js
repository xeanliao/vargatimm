define([
	'jquery',
	'underscore',
	'backbone',
	'models/user'
], function ($, _, Backbone, Model) {
	return Backbone.Collection.extend({
		model: Model,
		url: 'user',
		fetchInGroup: function (type, opts) {
			var collection = this,
				url = collection.url + '/group/' + type,
				options = {
					url: url,
					type: 'GET',
					// success: function(response){
					// 	if(response && response.data){
					// 		_.each(response.data, function(item){
					// 			collection.add(new Model(item));
					// 		});
					// 	}
					// }
				};
			_.extend(options, opts);

			//return (this.sync || Backbone.sync).call(this, 'read', this, options);
			return this.fetch(options);
		},
		fetchForGtu: function(opts){
			var collection = this,
				options = {
					url: collection.url + '/gtu/',
					type: 'GET',
				};
			_.extend(options, opts);

			return this.fetch(options);
		}
	});
});