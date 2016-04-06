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
			'report/:taskId': 'reportAction',
			'admin': 'adminAction',
			'admin/gtu': 'availableGTUAction',
			'print/:campaignId/:printType': 'printAction',
			'campaign/:campaignId/:taskName/:taskId/edit': 'gtuEditAction',
			'campaign/:campaignId/:taskName/:taskId/monitor': 'gtuMonitorAction',
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
		reportAction: function (taskId) {
			require([
				'models/campaign',
				'collections/campaign',
				'views/report/list'
			], function (Model, Collection, View) {
				var campaignlist = new Collection();
				campaignlist.fetchForReport().done(function () {
					Topic.publish('loadView', View, {
						collection: campaignlist,
						taskId: taskId
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
		gtuEditAction: function (campaignId, taskName, taskId) {
			require([
				'models/task',
				'models/print/dmap',
				'collections/gtu',
				'views/gtu/edit'
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

					$.when(dmap.fetchBoundary(), dmap.fetchGtu(), gtu.fetchByTask(taskId)).done(function () {
						Topic.publish('loadView', View, {
							dmap: dmap,
							gtu: gtu,
							task: task
						});
					});
				});

			});
		},
		gtuMonitorAction: function (campaignId, taskName, taskId) {
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
		},
		availableGTUAction: function(){
			require([
				'collections/gtu',
				'views/admin/availableGTU'
			], function (Collection, View) {
				var gtuList = new Collection();
				gtuList.fetch().done(function () {
					Topic.publish('loadView', View, {
						collection: gtuList
					}, {showSearch: false});
				});
			});
		}
	});
});