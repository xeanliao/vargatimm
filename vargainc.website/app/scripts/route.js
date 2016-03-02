define([
	'backbone',
	'pubsub'
], function (Backbone, Topic) {
	return Backbone.Router.extend({
		routes: {
			'campaign': 'defaultAction',
			'distribution': 'distributionAction',
			'monitor': 'monitorAction',
			'report': 'reportAction',
			'admin': 'adminAction',
			'print/:campaignId/:printType': 'printAction',
			'gtu/:taskId/monitor': 'gtuMonitorAction',
			'frame/:page': 'frameAction',
			'frame/*page?*queryString': 'frameAction',
			'*actions': 'defaultAction'
		},
		defaultAction: function () {
			require([
				'collections/campaign',
				'views/campaign/list'
			], function (Collection, View) {
				var campaignlist = new Collection();
				campaignlist.fetch().done(function () {
					Topic.publish('loadView', View, {
						collection: campaignlist
					});
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
				'collections/campaign',
				'views/distribution/list'
			], function (Collection, View) {
				var campaignlist = new Collection();
				campaignlist.fetchForDistribution().done(function () {
					Topic.publish('loadView', View, {
						collection: campaignlist
					});
				});
			});
		},
		monitorAction: function () {
			require([
				'collections/campaign',
				'views/monitor/list'
			], function (Collection, View) {
				var campaignlist = new Collection();
				campaignlist.fetchForTask().done(function () {
					Topic.publish('loadView', View, {
						collection: campaignlist
					});
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
				campaignlist.fetchForReport().done(function () {
					Topic.publish('loadView', View, {
						collection: campaignlist
					});
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
			switch (printType) {
			case 'campaign':
				require([
					'views/print/campaign',
					'collections/print'
				], function (View, Collection) {
					var print = new Collection();
					print.fetchForCampaignMap(campaignId).done(function () {
						Topic.publish('loadView', View, {
							collection: print
						}, {
							showMenu: false,
							showUser: false,
							showSearch: false,
							pageTitle: 'Timm Print Preview'
						});
					});
				});
				break;
			case 'distribution':
				require([
					'views/print/distribution',
					'collections/print'
				], function (View, Collection) {
					var print = new Collection();
					print.fetchForDistributionMap(campaignId).done(function () {
						Topic.publish('loadView', View, {
							collection: print
						}, {
							showMenu: false,
							showUser: false,
							showSearch: false,
							pageTitle: 'Timm Print Preview'
						});
					});
				});
				break;
			case 'report':
				require([
					'views/print/report',
					'collections/print'
				], function (View, Collection) {
					var print = new Collection();
					print.fetchForReportMap(campaignId).done(function () {
						Topic.publish('loadView', View, {
							collection: print
						}, {
							showMenu: false,
							showUser: false,
							showSearch: false,
							pageTitle: 'Timm Print Preview'
						});
					});
				});
				break;
			default:

				break;
			}
		},
		gtuMonitorAction: function (taskId) {
			require([
				'models/print/distribution',
				'collections/gtu',
				'views/gtu/monitor'
			], function (DMap, Gtu, View) {
				var dmap = new DMap({
						CampaignId: 7,
						SubMapId: 89,
						DMapId: 22
					}),
					gtu = new Gtu();
				$.when(dmap.fetchBoundary(), gtu.fetchByTask(taskId)).done(function () {
					Topic.publish('loadView', View, {
						dmap: dmap,
						gtu: gtu
					});
				});
			});
		}
	});
});