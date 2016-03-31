define([
	'underscore',
	'backbone',
	'pubsub'
], function (_, Backbone, Topic) {
	return Backbone.Model.extend({
		urlRoot: 'gtu',
		idAttribute: 'Id',
		defaults: {
			Id: null,
			ShortUniqueID: null,
			UserColor: null
		},
		assignGTUToTask: function (postData, opts) {
			var model = this,
				options = {
					url: model.urlRoot + '/task/assign/',
					method: 'PUT',
					data: postData,
					processData: true,
					success: function (result) {
						if (result && result.success) {
							model.set(result.data);
						}
					}
				};
			options = _.extend(opts, options);

			return (this.sync || Backbone.sync).call(this, 'update', this, options);
		},
		unassignGTUFromTask: function (taskId, opts) {
			var model = this,
				options = {
					url: model.urlRoot + '/task/' + taskId + '/unassign/' + model.get('Id'),
					method: 'DELETE',
					success: function (result) {
						if (result && result.success) {
							model.set({
								IsAssign: false,
								UserColor: null,
								Company: null,
								Auditor: null,
								Role: null,
								TaskId: null,
								AuditorId: null
							});
						}
					}
				};
			options = _.extend(opts, options);

			return (this.sync || Backbone.sync).call(this, 'update', this, options);
		},
		getTrack: function (taskId, opts) {
			var model = this,
				lastTime = model.get('lastUpdateTime'),
				url = model.urlRoot + '/task/' + taskId + '/track/' + model.get('Id');
			if (lastTime) {
				url += '/' + lastTime;
			}
			var options = {
				url: url,
				method: 'GET',
				success: function (result) {
					if (result) {
						model.set('lastUpdateTime', result.lastUpdateTime);
						var existTrack = model.get('track') || [];
						model.set('track', _.concat(existTrack, _.map(result.data, function (i) {
							return {
								lat: parseFloat(i.lat),
								lng: parseFloat(i.lng)
							}
						})));
					}
				}
			};
			options = _.extend(opts, options);
			return (this.sync || Backbone.sync).call(this, '', this, options);
		}
	});
});