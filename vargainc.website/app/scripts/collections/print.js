define([
	'backbone',
	'models/campaign'
], function (Backbone, Model) {
	return Backbone.Collection.extend({
		model: Model,
		urlRoot: 'print',
        fetchForDistributionMap: function(campaignId, opts){
            var model = this,
                options = {
                    url: model.urlRoot + '/report/campaign/' + campaignId,
                    parse: function(result){
                        console.log(result);
                    }
                };
            options = _.extend(opts, options);

            return this.fetch(options);
        }
	});
});