define([
	'backbone',
	'models/gtu'
], function (Backbone, Model) {
	return Backbone.Collection.extend({
		model: Model,
		urlRoot: 'task',
        fetchByTask: function(taskId, opts){
        	var model = this,
                options = {
                    url: model.urlRoot + '/' + taskId +'/gtu',
                };
            options = _.extend(opts, options);

            return this.fetch(options);
        }
	});
});