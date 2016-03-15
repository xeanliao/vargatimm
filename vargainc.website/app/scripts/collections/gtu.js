define([
	'backbone',
	'models/gtu'
], function (Backbone, Model) {
	return Backbone.Collection.extend({
		model: Model,
		urlRoot: 'gtu',
        fetchByTask: function(taskId, opts){
        	var model = this,
                options = {
                    url: 'task/' + taskId +'/gtu',
                };
            options = _.extend(opts, options);

            return this.fetch(options);
        },
        fetchGtuWithStatusByTask: function(taskId, opts){
            var model = this,
                options = {
                    url: model.urlRoot + '/task/' + taskId,
                };
            options = _.extend(opts, options);
            return this.fetch(options);
        },
	});
});