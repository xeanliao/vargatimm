define([
	'backbone',
	'models/campaign'
], function (Backbone, Model) {
	return Backbone.Collection.extend({
		model: Model,
		urlRoot: 'campaign',
		fetchForDistribution: function (opts) {
            var model = this,
                options = {
                    url: model.urlRoot + '/distribution',
                };
            options = _.extend(opts, options);

            return this.fetch(options);
        },
        fetchForTask: function(opts){
        	var model = this,
                options = {
                    url: model.urlRoot + '/monitor',
                };
            options = _.extend(opts, options);

            return this.fetch(options);
        },
        fetchForReport: function(opts){
            var model = this,
                options = {
                    url: model.urlRoot + '/report',
                };
            options = _.extend(opts, options);

            return this.fetch(options);
        }
	});
});