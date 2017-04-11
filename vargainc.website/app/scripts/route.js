import Backbone from 'backbone';
import React from 'react';
import Topic from 'postal';
import Promise from 'bluebird';

export default Backbone.Router.extend({
	routes: {
		'campaign': 'defaultAction',
		'distribution': 'distributionAction',
		'monitor': 'monitorAction',
		'campaign/:campaignId/monitor': 'campaignMonitorAction',
		'report': 'reportAction',
		'report/:taskId': 'reportAction',
		'admin': 'adminAction',
		'admin/gtu': 'availableGTUAction',
		'print/:campaignId/:printType': 'printAction',
		'campaign/:campaignId/:taskName/:taskId/edit': 'gtuEditAction',
		'campaign/:campaignId/:taskName/:taskId/monitor': 'gtuMonitorAction',
		'campaign/import': 'importCampaign',
		'frame/:page': 'frameAction',
		'frame/*page?*queryString': 'frameAction',
		'*actions': 'defaultAction'
	},
	defaultAction: function () {
		var Collection = require('collections/campaign').default;
		var View = require('views/campaign/list').default;
		var campaignlist = new Collection();
		campaignlist.fetch().then(() => {
			Topic.publish({
				channel: 'View',
				topic: 'loadView',
				data: {
					view: View,
					params: {
						collection: campaignlist
					}
				}
			});
		});
	},
	frameAction: function (page, query) {
		var url = "../" + page;
		if (query) {
			url += "?" + query;
		}
		window.open(url, '_blank', 'resizable=yes,status=yes,toolbar=no,scrollbars=yes,menubar=no,location=no');
		Backbone.history.history.back(-2);
	},
	distributionAction: function () {
		let Collection = require('collections/campaign').default;
		let View = require('views/distribution/list').default;
		let campaignlist = new Collection();
		campaignlist.fetchForDistribution().then(() => {
			Topic.publish({
				channel: 'View',
				topic: 'loadView',
				data: {
					view: View,
					params: {
						collection: campaignlist
					}
				}
			});
		});
	},
	monitorAction: function () {
		let Collection = require('collections/campaign').default;
		let View = require('views/monitor/list').default;
		let campaignlist = new Collection();
		campaignlist.fetchForTask().then(() => {
			Topic.publish({
				channel: 'View',
				topic: 'loadView',
				data: {
					view: View,
					params: {
						collection: campaignlist
					}
				}
			});
		});
	},
	gtuMonitorAction: function (campaignId, taskName, taskId) {
		let Task = require('models/task').default;
		let DMap = require('models/print/dmap').default;
		let Gtu = require('collections/gtu').default;
		let View = require('views/gtu/monitor').default;

		let gtu = new Gtu(),
			task = new Task({
				Id: taskId
			});
		task.fetch().then(() => {
			let dmap = new DMap({
				CampaignId: task.get('CampaignId'),
				SubMapId: task.get('SubMapId'),
				DMapId: task.get('DMapId')
			});

			Promise.all([
				dmap.fetchBoundary(),
				dmap.fetchAllGtu(),
				gtu.fetchGtuWithStatusByTask(taskId)
			]).then(() => {
				let pageTitle = `GTU Monitor - ${task.get('ClientName')}, ${task.get('ClientCode')}: ${task.get('Name')}`;
				Topic.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							dmap: dmap,
							gtu: gtu,
							task: task
						}
					},
					options: {
						showMenu: false,
						showUser: false,
						showSearch: false,
						pageTitle: pageTitle
					}
				});
			});
		});
	},
	campaignMonitorAction: function (campaignId) {
		var View = require('views/campaignMonitor/driver').default;
		var Model = require('models/campaign').default;
		var GeoCollection = require('collections/geo').default;
		var campaignWithTaskModel = new Model();
		var geoCollection = new GeoCollection();
		campaignWithTaskModel.loadWithAllTask(campaignId).then(() => {
			Topic.publish({
				channel: 'View',
				topic: 'loadView',
				data: {
					view: View,
					params: {
						model: campaignWithTaskModel,
						geo : geoCollection
					},
					options: {
						showMenu: false,
						showUser: true,
						showSearch: false,
						pageTitle: 'GTU Campaign Monitor'
					}
				}
			});
		});
	},
	reportAction: function (taskId) {
		let Model = require('models/campaign').default;
		let Collection = require('collections/campaign').default;
		let View = require('views/report/list').default;
		let campaignlist = new Collection();
		campaignlist.fetchForReport().then(() => {
			Topic.publish({
				channel: 'View',
				topic: 'loadView',
				data: {
					view: View,
					params: {
						collection: campaignlist,
						taskId: taskId
					}
				}
			});
		});
	},
	adminAction: function () {
		let View = require('views/admin/dashboard').default;
		Topic.publish({
			channel: 'View',
			topic: 'loadView',
			data: {
				view: View,
			}
		});
	},
	importCampaign: function () {
		let Collection = require('collections/campaign').default;
		let View = require('views/campaign/import').default;
		Topic.publish({
			channel: 'View',
			topic: 'loadView',
			data: {
				view: View,
				params: {
					collection: new Collection()
				},
			}
		});
	},
	printAction: function (campaignId, printType) {
		switch (printType) {
		case 'campaign':
			var Collection = require('collections/print').default;
			var View = require('views/print/campaign').default;
			var print = new Collection();
			print.fetchForCampaignMap(campaignId).then(() => {
				Topic.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							collection: print
						},
						options: {
							showMenu: false,
							showUser: false,
							showSearch: false,
							pageTitle: 'Timm Print Preview'
						}
					}
				});
			});
			break;
		case 'distribution':
			var Collection = require('collections/print').default;
			var View = require('views/print/distribution').default;
			var print = new Collection();
			print.fetchForDistributionMap(campaignId).then(() => {
				Topic.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							collection: print
						},
						options: {
							showMenu: false,
							showUser: false,
							showSearch: false,
							pageTitle: 'Timm Print Preview'
						}
					}
				});
			});
			break;
		case 'report':
			var Collection = require('collections/print').default;
			var View = require('views/print/report').default;
			var print = new Collection();
			print.fetchForReportMap(campaignId).then(() => {
				Topic.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							collection: print
						},
						options: {
							showMenu: false,
							showUser: false,
							showSearch: false,
							pageTitle: 'Timm Print Preview'
						}
					}
				});
			});
			break;
		default:

			break;
		}
	},
	gtuEditAction: function (campaignId, taskName, taskId) {
		let Task = require('models/task').default;
		let DMap = require('models/print/dmap').default;
		let Gtu = require('collections/gtu').default;
		let View = require('views/gtu/edit').default;

		let gtu = new Gtu(),
			task = new Task({
				Id: taskId
			});
		task.fetch().then(() => {
			let dmap = new DMap({
				CampaignId: task.get('CampaignId'),
				SubMapId: task.get('SubMapId'),
				DMapId: task.get('DMapId')
			});

			Promise.all([
				dmap.fetchBoundary(),
				dmap.fetchGtuForEdit(),
				gtu.fetchByTask(taskId)
			]).then(() => {
				Topic.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							dmap: dmap,
							gtu: gtu,
							task: task
						}
					}
				});
			});

		});
	},
	availableGTUAction: function () {
		let Collection = require('collections/gtu').default;
		let View = require('views/admin/availableGTU').default;
		let gtuList = new Collection();
		gtuList.fetch().then(() => {
			Topic.publish({
				channel: 'View',
				topic: 'loadView',
				data: {
					view: View,
					params: {
						collection: gtuList,
					},
					options: {
						showSearch: false
					}
				}
			});
		});
	}
});