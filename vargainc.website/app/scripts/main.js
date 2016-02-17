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

	var LayoutView = require('views/layout');
	var layoutViewInstance = React.createFactory(LayoutView)();
	var layout = ReactDOM.render(layoutViewInstance, document.getElementById('main-container'));



	var AppRouter = Backbone.Router.extend({
		routes: {
			'campaign': 'defaultAction',
			'distribution': 'distributionAction',
			'monitor': 'monitorAction',
			'report': 'reportAction',
			'admin': 'adminAction',
			'print/:campaignId/:printType': 'printAction',
			'frame/:page': 'frameAction',
			'frame/*page?*queryString': 'frameAction',
			'*actions': 'defaultAction'
		},
		defaultAction: function () {
			require([
				'models/campaign',
				'collections/campaign',
				'views/campaign/list'
			], function (Model, Collection, View) {
				var campaignlist = new Collection();
				campaignlist.fetch().done(function(){
					Topic.publish('loadView', View, campaignlist, null);
				});
			});
		},
		frameAction: function (page, query) {
			// require([
			// 	'views/frame'
			// ], function (View) {
			// 	var FrameView = React.createFactory(View);
			// 	var frameView = FrameView({
			// 		page: '../' + page,
			// 		query: query
			// 	});
			// 	Topic.publish('loadView', frameView);
			// });
			var url = "../" + page;
			if (query) {
				url += "?" + query;
			}
			window.open(url, '_blank', 'resizable=yes,status=yes,toolbar=no,scrollbars=yes,menubar=no,location=no');
			Backbone.history.history.back(-2);
		},
		distributionAction: function () {
			require([
				'models/campaign',
				'collections/campaign',
				'views/distribution/list'
			], function (Model, Collection, View) {
				var campaignlist = new Collection();
				campaignlist.fetchForDistribution().done(function(){
					Topic.publish('loadView', View, campaignlist, null);
				});
			});
		},
		monitorAction: function () {
			require([
				'models/campaign',
				'collections/campaign',
				'views/monitor/list'
			], function (Model, Collection, View) {
				var campaignlist = new Collection();
				campaignlist.fetchForTask().done(function(){
					Topic.publish('loadView', View, campaignlist, null);
				});
			});
		},
		reportAction: function () {
			require([
				'models/campaign',
				'collections/campaign',
				'views/report/list'
			], function (Model, Collection, View) {
				var campaignlist = new Collection();
				campaignlist.fetchForReport().done(function(){
					Topic.publish('loadView', View, campaignlist, null);
				});
			});
		},
		adminAction: function () {
			require([
				'views/admin/dashboard'
			], function (View) {
				Topic.publish('loadView', View);
			});
		},
		printAction: function (campaignId, printType) {
			require([
				'views/print/distribution',
				'collections/print'
			], function (View, Collection) {
				var print = new Collection();
				print.fetchForDistributionMap(campaignId).done(function () {
					Topic.publish('loadView', View, print, null);
				});
			});
		}
	});

	var appRouter = new AppRouter;

	Backbone.history.start();
});