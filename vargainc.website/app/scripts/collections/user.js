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
			var model = this,
				url = model.url + '/group/' + type,
				options = {
					url: url,
					type: 'GET',
					success: function(response){
						if(response && response.data){
							_.each(response.data, function(item){
								model.add(new Model(item));
							});
						}
					}
				};
			_.extend(options, opts);

			return (this.sync || Backbone.sync).call(this, 'read', this, options);
		}
	});
});