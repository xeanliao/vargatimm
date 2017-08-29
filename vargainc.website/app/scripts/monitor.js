define(function (require, exports, module) {
	var $ = require('jquery'),
		Backbone = require('backbone'),
		React = require('react'),
		ReactDOM = require('react-dom'),
		Topic = require('pubsub');

	/**
	 * register loading event except ajax request is quite
	 */
	$(document).ajaxSend(function (event, xhr, settings) {
		if (settings.quite !== true) {
			Topic.publish('showLoading');
		}
	});
	$(document).ajaxComplete(function (event, xhr, settings) {
		if (settings.quite !== true) {
			Topic.publish('hideLoading');
		}
	});

	/**
	 * override base url
	 */
	var backboneSync = Backbone.sync;
	Backbone.sync = function (method, model, options) {
		if (!options.url) {
			options.url = _.isFunction(model.url) ? model.url() : model.url;
		}
		if (!options.url) {
			options.url = model.urlRoot;
		}
		options.url = '../api/' + options.url;
		return backboneSync(method, model, options);
	}

	var AppRouter = Backbone.Router.extend({
		routes: {
			':taskId': 'gtuMonitorAction',
			'*actions': 'defaultAction'
		},
		defaultAction: function () {
			Topic.publish('showDialog', 'There is nothing to display');
		},
		gtuMonitorAction: function (taskId) {
			require([
				'models/task',
				'models/print/dmap',
				'collections/gtu',
				'views/gtu/monitor'
			], function (Task, DMap, Gtu, View) {
				var gtu = new Gtu(),
					task = new Task({
						Id: taskId
					});
				task.fetch().then(function () {
					var dmap = new DMap({
						CampaignId: task.get('CampaignId'),
						SubMapId: task.get('SubMapId'),
						DMapId: task.get('DMapId')
					});
					task.set('Id', taskId);

					$.when(dmap.fetchBoundary(), dmap.fetchAllGtu(), gtu.fetchGtuWithStatusByTask(taskId)).done(function () {
						Topic.publish('loadView', View, {
							dmap: dmap,
							gtu: gtu,
							task: task
						}, {
							showMenu: false,
							showUser: false,
							showSearch: false,
							pageTitle: 'GTU Monitor - ' + task.get('ClientName') + ', ' + task.get('ClientCode') + ': ' + task.get('Name')
						});
					});
				});

			});
		}
	});
	var LayoutView = require('views/layout/main');
	var LayoutViewInstance = React.createFactory(LayoutView);
	var layoutViewInstance = LayoutViewInstance({
		user: null
	});
	var layout = ReactDOM.render(layoutViewInstance, document.getElementById('main-container'));
	var appRouter = new AppRouter;
	Backbone.history.start();
});