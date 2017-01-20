import Backbone from 'backbone';
import React from 'react';
import ReactDOMServer from 'react-dom/server';
import 'react.backbone';
import Promise from 'bluebird';
import L from 'leaflet';
import 'leaflet-dvf';
import 'lib/leaflet-google';
import 'lib/animateSVGLayer';
import $ from 'jquery';
import classNames from 'classnames';
import {
	geom
} from 'jsts';
import {
	groupBy,
	map,
	each,
	find,
	filter,
	indexOf,
	xor,
	extend
} from 'lodash';

import d3 from 'd3';

import GTUCollection from 'collections/gtu';
import DMap from 'models/print/dmap';
import GtuCollection from 'collections/gtu';
import UserCollection from 'collections/user';
import UserModel from 'models/user';
import TaskModel from 'models/task';

import BaseView from 'views/base';
import AssignView from 'views/gtu/assign';
import EmployeeView from 'views/user/employee';
import EditView from 'views/monitor/edit';

const TAG = '[CAMPAIGN-MONITOR]';

var MapContainer = React.createBackboneClass({
	mixins: [BaseView],
	shouldComponentUpdate: function () {
		return false;
	},
	getInitialState: function () {
		return {
			gtuMarkerLayer: {},
			gtuTrackLayer: {},
			gtuLocationLayer: null,
			displayGtus: [],
			showOutOfBoundaryGtu: false,
			drawMode: 'marker'
		}
	},
	onInit: function (mapContainer) {
		var self = this;
		var monitorMap = L.map(mapContainer, {
			center: {
				lat: 41.850033,
				lng: -87.6500523
			},
			zoom: 13,
			preferCanvas: false,
			animate: false,
			scrollWheelZoom: false,
		});

		/**
		 * prepare track pane
		 */
		monitorMap.createPane('GtuTrackPane', monitorMap.getPane('overlayPane'));
		monitorMap.createPane('GtuMarkerPane', monitorMap.getPane('markerPane'));

		this.setState({
			map: monitorMap,
			mapContainer: mapContainer
		}, () => {
			self.drawBoundary(monitorMap);
			self.registerTopic(monitorMap);
			$(window).trigger('resize');
		});
	},
	registerTopic: function (monitorMap) {
		var self = this;
		this.subscribe('Campaign.Monitor.ZoomToTask', taskId => {
			self.flyToTask(taskId);
		});
		this.subscribe('Campaign.Monitor.DrawGtu', data => {
			self.setState({
				task: data.task,
				displayGtus: data.displayGtus
			}, () => {
				self.drawGtu();
				window.clearInterval(self.state.reloadTimeout);
				self.setState({
					reloadTimeout: window.setInterval(self.reload, 10 * 1000)
				});
			});
		});
		this.subscribe('Campaign.Monitor.DrawMode', mode => {
			self.setState({
				drawMode: mode
			}, () => {
				self.drawGtu();
			});
		});
		this.subscribe('Campaign.Monitor.ShowOutOfBoundaryGtu', boolean => {
			self.setState({
				showOutOfBoundaryGtu: boolean
			}, () => {
				self.drawGtu();
			});
		});
		this.subscribe('Campaign.Monitor.SwitchGtu', gtus => {
			self.setState({
				displayGtus: gtus
			}, () => {
				self.drawGtu();
			});
		});
		this.subscribe('Campaign.Monitor.LocatGtu', gtuId => {
			self.state.gtuLocationLayer.eachLayer(layer => {
				if (layer && layer.latlng && layer.gtuId == gtuId) {
					self.state.map.flyTo(layer.latlng);
					return false;
				}
			})
		});
		this.subscribe('Campaign.Monitor.CenterToTask', () => {
			self.flyToTask(self.state.task.get('Id'));
		});
		this.subscribe('Campaign.Monitor.Reload', () => {
			self.reload();
		});
		this.subscribe('Campaign.Monitor.Redraw.FinishedTaskBoundary', taskId => {
			self.redrawBoundary(taskId);
		})
		this.subscribe('Global.Window.Resize', size => {
			if (self.state.mapContainer) {
				console.log(`${TAG} window resize width: ${size.width} height: ${size.height}`);
				self.state.mapContainer.style.width = `${size.width}px`;
				self.state.mapContainer.style.height = `${size.height}px`;
			}
		});


		monitorMap.on('zoomstart', () => {
			$('.leaflet-GtuMarker-pane').hide();
		});
		monitorMap.on('zoomend', () => {
			$('.leaflet-GtuMarker-pane').show();
		});

		this.on('click.map.popup', '.btnSetActiveTask', function () {
			var taskId = $(this).attr("id");
			self.publish("Map.Popup.SetActiveTask", taskId);
			self.state.map.closePopup()
		});
	},
	drawBoundary: function (monitorMap) {
		this.publish('showLoading');
		var self = this,
			model = this.getModel(),
			campaignId = model.get('Id'),
			tasks = model.get('Tasks'),
			submapTasks = groupBy(tasks.models, t => {
				return t.get('SubMapId')
			}),
			promiseSubmapArray = [],
			promiseTaskArray = [],
			taskBoundaryLayerGroup = L.layerGroup(),
			boundaryBounds = L.latLngBounds();

		each(submapTasks, (groupTasks, submapId) => {
			promiseSubmapArray.push($.getJSON(`../api/print/campaign/${campaignId}/submap/${submapId}/boundary`));
		});
		return Promise.all(promiseSubmapArray).then(data => {
			console.timeEnd();
			return Promise.each(data, result => {
				let latlngs = map(result.boundary, i => {
					boundaryBounds.extend(i);
					return [i.lat, i.lng];
				});
				L.polyline(latlngs, {
					color: `rgb(${result.color.r}, ${result.color.g}, ${result.color.b})`,
					weight: 3,
					noClip: true,
					dropShadow: true
				}).addTo(monitorMap);
				return Promise.resolve();
			});
		}).then(() => {
			each(submapTasks, groupTasks => {
				each(groupTasks, task => {
					promiseTaskArray.push(new Promise((resolve, reject) => {
						let url = `../api/print/campaign/${task.get('CampaignId')}/submap/${task.get('SubMapId')}/dmap/${task.get('DMapId')}/boundary`;
						$.getJSON(url).then(result => {
							resolve({
								task: task,
								result: result
							});
						});
					}));
				});
			});

			return Promise.all(promiseTaskArray);
		}).then(response => {
			console.timeEnd();
			return Promise.each(response, data => {
				let task = data.task;
				let result = data.result;
				let latlngArray = []; //for draw map polygon
				let coordinateList = []; //for use jsts get polygon center
				each(result.boundary, i => {
					latlngArray.push([i.lat, i.lng]);
					coordinateList.push(new geom.Coordinate(i.lng, i.lat));
				});
				let factory = new geom.GeometryFactory();
				let boundaryLineRing = factory.createLinearRing(coordinateList);
				let polygon = factory.createPolygon(boundaryLineRing);
				var center = polygon.getCentroid();
				let color = `rgb(${result.color.r}, ${result.color.g}, ${result.color.b})`;
				let opt = {
					taskId: task.get('Id'),
					center: {
						lat: center.getY(),
						lng: center.getX()
					},
					text: {
						text: `${task.get('Name')}`
					},
					weight: 1,
					color: color,
					opacity: 0.75,
					fill: true,
					fillColor: !task.get('IsFinished') ? color : '#000',
					fillOpacity: !task.get('IsFinished') ? 0.1 : 0.75,
					noClip: true,
					clickable: !task.get('IsFinished'),
					dropShadow: !task.get('IsFinished'),
					fillPattern: !task.get('IsFinished') ? null : {
						url: 'data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHdpZHRoPSc4JyBoZWlnaHQ9JzgnPgogIDxyZWN0IHdpZHRoPSc4JyBoZWlnaHQ9JzgnIGZpbGw9JyNmZmYnLz4KICA8cGF0aCBkPSdNMCAwTDggOFpNOCAwTDAgOFonIHN0cm9rZS13aWR0aD0nMC41JyBzdHJva2U9JyNhYWEnLz4KPC9zdmc+Cg==',
						pattern: {
							width: '8px',
							height: '8px',
							patternUnits: 'userSpaceOnUse',
							patternContentUnits: 'Default'
						},
						image: {
							width: '8px',
							height: '8px'
						}
					}
				};
				var boundary = L.polyline(latlngArray, opt)
					.on('click', self.onTaskAreaClickHandler, self);
				boundary.getCenter = function () {
					return {
						lat: center.getY(),
						lng: center.getX()
					};
				};
				taskBoundaryLayerGroup.addLayer(boundary);
				return Promise.resolve();
			});
		}).then(() => {
			taskBoundaryLayerGroup.addTo(monitorMap);
			monitorMap.fitBounds(boundaryBounds);
			self.setState({
				taskBoundaryLayerGroup: taskBoundaryLayerGroup
			}, () => {
				new L.googleTile({
					mapTypeId: google.maps.MapTypeId.HYBRID
				}).addTo(monitorMap);
				self.publish('hideLoading');
			})
		});
	},
	redrawBoundary: function (taskId) {
		var self = this;
		this.state.taskBoundaryLayerGroup.eachLayer(layer => {
			if (layer.options.taskId == taskId) {
				extend(layer.options, {
					fillColor: '#000',
					fillOpacity: 0.75,
					clickable: false,
					dropShadow: false,
					fillPattern: {
						url: 'data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHdpZHRoPSc4JyBoZWlnaHQ9JzgnPgogIDxyZWN0IHdpZHRoPSc4JyBoZWlnaHQ9JzgnIGZpbGw9JyNmZmYnLz4KICA8cGF0aCBkPSdNMCAwTDggOFpNOCAwTDAgOFonIHN0cm9rZS13aWR0aD0nMC41JyBzdHJva2U9JyNhYWEnLz4KPC9zdmc+Cg==',
						pattern: {
							width: '8px',
							height: '8px',
							patternUnits: 'userSpaceOnUse',
							patternContentUnits: 'Default'
						},
						image: {
							width: '8px',
							height: '8px'
						}
					}
				});
				layer.remove();
				layer.addTo(self.state.taskBoundaryLayerGroup);
				return false;
			}
		});
	},
	buildTaskPopup: function (task) {
		var self = this;
		var taskStatus = null;
		var setActiveButton = null;
		var isFinished = task.get('IsFinished');
		if (isFinished) {
			taskStatus = [<span key={'finished'}>FINISHED</span>];
		} else {
			setActiveButton = (
				<div className="small-8 align-center columns">
					<div className="button-group" style={{marginTop: "20px"}}>
						<button id={task.get('Id')} className="button btnSetActiveTask">Switch</button>
					</div>
				</div>
			);

			switch (task.get('Status')) {
			case 0: //started
				taskStatus = [<span key={'started'}>STARTED</span>];
				break;
			case 1: //stoped
				taskStatus = [<span key={'stopped'}>STOPPED</span>];
				break;
			case 2: //peased
				taskStatus = [<span key={'peased'}>PEASED</span>];
				break;
			default:
				taskStatus = [<span key={'started'}>NOT STARTED</span>];
				break;
			}
		}
		var gtuList = [];
		var templat = (
			<div className="row align-center section" style={{'minWidth': '320px'}}>
				<div className="small-7 columns">{task.get('Name')}</div>
				<div className="small-5 columns text-right">{taskStatus}</div>
				{setActiveButton}
			</div>
		);

		return ReactDOMServer.renderToStaticMarkup(templat);
	},
	onTaskAreaClickHandler: function (evt) {
		console.log(this);
		var self = this;
		var taskAreaCenter = evt.target.options.center,
			taskId = evt.target.options.taskId,
			allTask = this.getModel().get('Tasks'),
			task = find(allTask.models, i => {
				return i.get('Id') == taskId
			}),
			popContent = self.buildTaskPopup(task);
		L.popup({
				maxWidth: 640
			}).setLatLng(taskAreaCenter)
			.setContent(popContent)
			.openOn(this.state.map);
	},
	reload: function () {
		if (!this.state.task || this.state.reloading) {
			return;
		}

		var self = this;
		return new Promise((resolve, reject) => {
			this.setState({
				reloading: true
			}, () => {
				var task = self.state.task,
					dmap = task.get('dmap'),
					gtus = task.get('gtuList');

				Promise.all([
					dmap.updateGtuAfterTime(null, {
						quite: true
					}),
					gtus.fetchGtuLocation(task.get('Id'), {
						quite: true
					})
				]).then(() => {
					self.setState({
						reloading: false
					}, () => {
						return resolve();
					});
				}).catch(() => {
					return reject(new Error('unkown error in reload'));
				})
			});
		}).then(() => {
			self.drawGtu();
		})
	},
	drawGtu: function () {
		var map = this.state.map;
		var task = this.state.task;
		if (!map || !task) {
			return;
		}
		var gtus = task.get('dmap').get('Gtu');
		this.drawGtuMarker(gtus);
		this.drawGtuTrack(gtus);
		this.drawGtuLocation();
	},
	updateGtu: function () {
		// setLatLngs
	},
	drawGtuMarker: function (gtus) {
		var self = this;
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		if (this.state.drawMode.indexOf('marker') == -1) {
			//hide all marker
			$('.leaflet-GtuMarker-pane').hide();
			return;
		}
		$('.leaflet-GtuMarker-pane').show();
		each(gtus, data => {
			let gtuId = data.points[0].Id;
			var markerLayer = this.state.gtuMarkerLayer[gtuId];
			if (indexOf(this.state.displayGtus, gtuId) == -1 && markerLayer) {
				markerLayer.remove();
				return true;
			}
			if (markerLayer) {
				markerLayer.remove();
			}
			markerLayer = L.markerGroup();
			each(data.points, latlng => {
				if (!latlng.lat || !latlng.lng || !self.state.showOutOfBoundaryGtu && latlng.out) {
					return true;
				}
				L.triangleMarker(latlng, {
					gtuId: gtuId,
					weight: 0.1,
					opacity: 0.65,
					fillColor: data.color,
					fillOpacity: 0.65,
					fill: true,
					stroke: true,
					numberOfSides: 50,
					stroke: false,
					dropShadow: true,
					rotation: 0,
					radius: 5,
					clickable: false,
					noClip: true,
					showLegendTooltips: false,
					pane: 'GtuMarkerPane',
					gradient: function () {
						return {
							gradientType: 'radial',
							stops: [{
								offset: '0%',
								style: {
									color: data.color,
									opacity: 0.65
								}
							}, {
								offset: '30%',
								style: {
									color: data.color,
									opacity: 0.3
								}
							}, {
								offset: '100%',
								style: {
									color: data.color,
									opacity: 0
								}
							}]
						}
					},
				}).addTo(markerLayer);
			});
			self.state.gtuMarkerLayer[gtuId] = markerLayer;
			markerLayer.addTo(monitorMap);
		});
	},
	drawGtuTrack: function (gtus) {
		// each(this.state.gtuMarkerLayer, layer => {
		// 	layer.eachLayer(marker => {

		// 	});
		// });
	},
	drawGtuLocation: function (gtus) {
		var self = this;
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		let taskIsStopped = this.state.task.get('Status') == 1,
			gtuList = this.state.task.get('gtuList').where(function (i) {
				if (taskIsStopped) {
					return i.get('WithData')
				} else {
					return i.get('IsAssign') || i.get('WithData');
				}
			}),
			gtuLocationLayer = this.state.gtuLocationLayer;
		if (!gtuLocationLayer) {
			// gtuLocationLayer.remove();
			gtuLocationLayer = L.layerGroup();
		}

		each(gtuList, gtu => {
			let latlng = gtu.get('Location');
			if (!latlng || !latlng.lat || !latlng.lng) {
				return true;
			}
			let gtuId = gtu.get('Id');
			let exisitMarkerLayer = false;
			gtuLocationLayer.eachLayer(markerLayer => {
				if (markerLayer.gtuId == gtuId) {
					markerLayer.setLatLng(latlng);
					exisitMarkerLayer = true;
					return false;
				}
			});
			if (exisitMarkerLayer) {
				return true;
			}
			let locationMarker = L.markerGroup();
			let gtuLocationPoint = L.triangleMarker(latlng, {
				gtuId: gtu.get('Id'),
				radius: 5,
				fillColor: gtu.get('UserColor'),
				fillOpacity: 0.75,
				fill: true,
				stroke: false,
				numberOfSides: 50,
				pane: 'GtuMarkerPane',
				gradient: true,
			}).addTo(locationMarker);

			var gtuLocationMarker = L.triangleMarker(latlng, {
				gtuId: gtu.get('Id'),
				radius: 20,
				fillColor: gtu.get('UserColor'),
				fillOpacity: 0.2,
				fill: true,
				stroke: false,
				numberOfSides: 50,
				pane: 'GtuMarkerPane',
				gradient: false
			}).addTo(locationMarker);
			L.AnimationUtils.animate(gtuLocationMarker, {
				duration: 0,
				easing: L.AnimationUtils.easingFunctions.easeIn,
				from: gtuLocationMarker.options,
				to: L.extend({}, gtuLocationMarker.options, {
					fillOpacity: '0',
					radius: 20,
				})
			});
			locationMarker.gtuId = gtuId;
			locationMarker.latlng = latlng;
			locationMarker.addTo(gtuLocationLayer);
		});

		/**
		 * use d3 to add animate
		 */
		// let panes = monitorMap.getPanes();
		// var svg = d3.select(panes.GtuLocationPane.firstChild).node();

		gtuLocationLayer.addTo(monitorMap);
		self.setState({
			gtuLocationLayer: gtuLocationLayer
		});
	},
	flyToTask: function (taskId) {
		var map = this.state.map;
		if (!map) {
			return;
		}
		this.state.taskBoundaryLayerGroup.eachLayer(layer => {
			if (layer.options.taskId == taskId) {
				var taskBounds = layer.getBounds()
				map.fitBounds(taskBounds);
			}
		});
	},
	clearTask: function () {
		each(gtuMarkerLayer, layer => {
			layer && layer.remove && layer.remove();
		});
		this.setState({
			gtuMarkerLayer: []
		});
	},
	render: function () {
		return (
			<div className="leaflet-map-container" ref={this.onInit} style={{'minHeight': '640px'}}></div>
		);
	}
});

var ImportFiles = React.createClass({
	shouldComponentUpdate: function () {
		return false;
	},
	onImport: function (taskId, e) {
		e.bubbles = false;

		var uploadFile = e.currentTarget.files[0];
		if (uploadFile.size == 0) {
			alert('please select an not empty file!');
			return;
		}

		var model = new TaskModel({
				Id: taskId
			}),
			self = this;

		model.importGtu(uploadFile).then(() => {
			alert('import success');
		});
	},
	render: function () {
		var self = this,
			tasks = this.props.tasks || [];
		return (
			<div>
				{map(tasks, taskId=>{
					return (
						<input key={`file-import-${taskId}`} type="file" id={`upload-file-${taskId}`} multiple style={{'display': 'none'}} onChange={self.onImport.bind(this, taskId)} />
					);
				})}
			</div>
		);
	}
});

export default React.createBackboneClass({
	mixins: [BaseView],
	getInitialState: function () {
		return {
			activeTask: null,
			displayGtus: []
		}
	},
	componentDidMount: function () {
		var self = this;

		this.subscribe('Map.Popup.SetActiveTask', taskId => {
			var model = self.getModel();
			var tasks = model && model.get('Tasks');
			if (tasks && tasks.size && tasks.size() > 0) {
				var currentTask = find(tasks.models, i => {
					return i.get('Id') == taskId;
				});
				if (currentTask) {
					self.onSwitchActiveTask.call(self, currentTask);
				}
			}
		});

		this.on('click.import.task', '.btnImportTask', function (evt) {
			evt.preventDefault();
			evt.stopPropagation();
			var taskId = $(this).attr('id');
			$("#upload-file-" + taskId).click();
		})

		this.subscribe('Global.Window.Click', () => {
			self.onCloseDropDown();
			self.onCloseMoreMenu();
		});
	},
	showTask: function (task) {
		this.publish('Campaign.Monitor.ZoomToTask', task);
	},
	onSwitchActiveTask: function (task) {
		var self = this;
		this.publish('clearTask');
		var taskDMap = new DMap({
			CampaignId: task.get('CampaignId'),
			SubMapId: task.get('SubMapId'),
			DMapId: task.get('DMapId'),
			Gtu: []
		});
		var trackQuery = taskDMap.updateGtuAfterTime(null, {
			quite: false
		});

		var gtu = new GtuCollection();
		var locationQuery = gtu.fetchGtuWithStatusByTask(task.get('Id'), {
			quite: false
		}).then(() => {
			return gtu.fetchGtuLocation(task.get('Id'), {
				quite: false
			});
		});

		return Promise.all([trackQuery, locationQuery]).then(() => {
			task.set('dmap', taskDMap);
			task.set('gtuList', gtu);
			/**
			 * set all gtu as default display
			 */
			var displayGtus = taskDMap.get('Gtu') || [];
			displayGtus = map(displayGtus, gtu => {
				return gtu.points && gtu.points.length > 0 ? gtu.points[0].Id : null;
			});
			self.setState({
				activeTask: task,
				displayGtus: displayGtus
			}, () => {
				self.publish('Campaign.Monitor.ZoomToTask', task.get('Id'));
				self.publish('Campaign.Monitor.DrawGtu', {
					task: task,
					displayGtus: displayGtus
				});
			});

		});
	},
	onOpenDropDown: function (evt) {
		evt && evt.preventDefault();
		evt && evt.stopPropagation();
		this.setState({
			taskDropdownActive: true,
		});
	},
	onCloseDropDown: function (evt) {
		evt && evt.preventDefault();
		evt && evt.stopPropagation();
		this.setState({
			taskDropdownActive: false,
		});
	},
	onOpenMoreMenu: function (evt) {
		evt && evt.preventDefault();
		evt && evt.stopPropagation();
		this.setState({
			taskMoreMenuActive: true,
		});
	},
	onCloseMoreMenu: function (evt) {
		evt && evt.preventDefault();
		evt && evt.stopPropagation();
		this.setState({
			taskMoreMenuActive: false,
		});
	},
	onStart: function (task) {
		task.setStart();
	},
	onStop: function (task) {
		task.setStop();
	},
	onPause: function (task) {
		task.setPause();
	},
	onSwitchDisplayMode: function (mode) {
		var self = this;
		this.setState({
			displayMode: mode
		}, () => {
			self.publish('Campaign.Monitor.DrawMode', mode);
		});
	},
	onSwitchShowOutOfBoundaryGtu: function () {
		var self = this;
		var showOutOfBoundaryGtu = !this.state.ShowOutOfBoundaryGtu;
		this.setState({
			ShowOutOfBoundaryGtu: showOutOfBoundaryGtu,
		}, () => {
			self.publish('Campaign.Monitor.ShowOutOfBoundaryGtu', showOutOfBoundaryGtu);
		});
	},
	onAddEmployee: function () {
		var user = new UserCollection(),
			self = this;
		user.fetchCompany().done(function () {
			self.publish('showDialog', {
				view: EmployeeView,
				params: {
					model: new UserModel(),
					company: user
				}
			});
		});
	},
	onAssign: function () {
		var gtu = this.state.activeTask.get('gtu'),
			taskId = this.state.activeTask.get('Id'),
			user = new UserCollection(),
			self = this;
		user.fetchForGtu().then(function () {
			self.publish('showDialog', {
				view: AssignView,
				params: {
					collection: gtu,
					user: user,
					taskId: taskId,
				},
				options: {
					size: 'full'
				}
			});
		});
	},
	onReCenter: function () {
		this.publish('Campaign.Monitor.CenterToTask');
	},
	onReload: function () {
		this.publish('Campaign.Monitor.Reload');
	},
	onGotoGTU: function (gtuId) {
		this.publish('Campaign.Monitor.LocatGtu', gtuId);
	},
	onSelectedGTU: function (gtuId) {
		var self = this;
		let displayGtus = xor(this.state.displayGtus, [gtuId]);
		this.setState({
			displayGtus: displayGtus
		}, () => {
			self.publish('Campaign.Monitor.SwitchGtu', displayGtus);
		});
	},
	onFinished: function (taskId) {
		var self = this,
			taskModel = new TaskModel({
				Id: taskId
			});
		taskModel.markFinished({
			success: function (result) {
				if (result && result.success) {
					var model = self.getModel();
					var tasks = model.get('Tasks');
					if (tasks) {
						var currentTask = tasks.get(taskId);
						currentTask && currentTask.set('IsFinished', true);
						self.forceUpdate();
						self.publish('Campaign.Monitor.Redraw.FinishedTaskBoundary', taskId);
					}
				} else {
					self.alert(result.error);
				}
			}
		});
	},
	onEdit: function (taskId, e) {
		var model = new TaskModel({
				Id: taskId
			}),
			self = this;

		model.fetch().then(function () {
			self.publish('showDialog', EditView, model);
		});
	},
	onOpenUploadFile: function (taskId) {
		$("#upload-file-" + taskId).click();
	},
	renderTaskDropdown: function () {
		var self = this,
			model = this.getModel(),
			tasks = model && model.get('Tasks') ? model.get('Tasks').models : [];

		tasks = filter(tasks, t => {
			return !t.get('IsFinished');
		});
		var parentClass = classNames({
			'is-dropdown-submenu-parent': true,
			'opens-left': true,
			'is-active': this.state.taskDropdownActive,
		});
		var menuClass = classNames({
			'menu': true,
			'submenu': true,
			'is-dropdown-submenu': true,
			'first-sub': true,
			'vertical': true,
			'js-dropdown-active': this.state.taskDropdownActive,
		});
		if (tasks.length > 10) {
			menuClass = classNames('section row collapse small-up-2 medium-up-3 large-up-4', {
				'menu': true,
				'submenu': true,
				'is-dropdown-submenu': true,
				'first-sub': true,
				'vertical': true,
				'js-dropdown-active': this.state.taskDropdownActive,
			});
			return (
				<ul className="dropdown menu float-right">
					<li className={parentClass}>
						<a href="javascript:;" onClick={this.onOpenDropDown}>Switch Active Task</a>
						<div style={{'minWidth': '768px'}} className={menuClass} onClick={this.onCloseDropDown}>
							{map(tasks, t=>{
								return (
									<div className="column" key={t.get('Id')}>
										<a href="javascript:;" style={{width: '100%'}} className="button row-button text-left" onClick={self.onSwitchActiveTask.bind(self, t)}>
											{t.get('Name')}
										</a>
									</div>
								);
							})}
						</div>
					</li>
				</ul>
			);
		}
		return (
			<ul className="dropdown menu float-right">
				<li className={parentClass}>
					<a href="javascript:;" onClick={this.onOpenDropDown}>Switch Active Task</a>
					<ul className={menuClass} onClick={this.onCloseDropDown}>
						{map(tasks, t=>{
							return (
								<li key={t.get('Id')}>
									<a href="javascript:;" onClick={self.onSwitchActiveTask.bind(self, t)}>
										{t.get('Name')}
									</a>
								</li>
							);
						})}
					</ul>
				</li>
			</ul>
		);
	},
	renderActiveTask: function () {
		if (!this.state.activeTask) {
			return null;
		}
		var self = this;
		var activeTask = this.state.activeTask;
		var taskIsFinished = activeTask.get('IsFinished');
		var gtuList = activeTask.get('gtuList') || [];
		if (gtuList.where) {
			gtuList = gtuList.where(function (i) {
				if (taskIsFinished) {
					return i.get('WithData')
				} else {
					return i.get('IsAssign') || i.get('WithData');
				}
			});
		}

		return (
			<div>
				<div className="section row gtu-monitor">
					<div className="small-12 columns">
						<div className="row">
							<div className="small-12 medium-5 large-3 columns">
								{this.renderTaskController(activeTask)}
							</div>
							<div className="small-12 medium-7 large-9 columns">
								{this.renderTaskMoreMenu(activeTask)}
								<button className="button float-right" onClick={this.onReCenter}>
									<i className="fa fa-crosshairs"></i>
									<span>Center</span>
								</button>
								<button className="button float-right" onClick={this.onReload}>
									<i className="fa fa-refresh"></i>
									<span>Refresh</span>
								</button>
							</div>
						</div>
					</div>
				</div> 
				<div className="row gtu">
					<div className="small-12 columns">
						{map(gtuList, function(gtu) {
							return self.renderGtu(gtu);
						})}
					</div> 
				</div>
			</div>
		);
	},
	renderGtu: function (gtu) {
		var typeIcon = null,
			alertIcon = null,
			deleteIcon = null,
			buttonClass = 'button text-left',
			taskIsStopped = this.state.activeTask.get('Status') == 1,
			isActive = indexOf(this.state.displayGtus, gtu.get('Id')) > -1,
			gtuIcon = null;

		if (taskIsStopped) {
			gtuIcon = <i className="fa fa-stop"></i>
		} else {
			switch (gtu.get('Role')) {
			case 'Driver':
				gtuIcon = <i className="fa fa-truck"></i>
				break;
			case 'Walker':
				gtuIcon = <i className="fa fa-street-view"></i>
				break;
			default:
				gtuIcon = null;
				break;
			}
		}

		if (isActive) {
			buttonClass += ' active';
		}
		if (!taskIsStopped && !gtu.get('IsOnline')) {
			buttonClass += ' offline';
		}
		if (!taskIsStopped && gtu.get('IsOnline') && gtu.get('OutOfBoundary')) {
			alertIcon = <i className="fa fa-bell faa-ring animated alert"></i>;
		}
		if (!taskIsStopped && gtu.get('WithData')) {
			deleteIcon = <i className="fa fa-warning alert"></i>;
		}
		return (
			<span className="group" key={gtu.get('Id')}>
				<button className={buttonClass} style={{'backgroundColor': isActive ? gtu.get('UserColor') : 'transparent'}} onClick={this.onSelectedGTU.bind(null, gtu.get('Id'))}>
					{deleteIcon}
					{gtuIcon}
					&nbsp;&nbsp;<span>{gtu.get('ShortUniqueID')}</span>&nbsp;&nbsp;
					{alertIcon}
				</button>
				<button className="button location" onClick={this.onGotoGTU.bind(null, gtu.get('Id'))}>
					<i className="location fa fa-crosshairs" style={{color:'black', 'backgroundColor': gtu.get('UserColor')}}></i>
				</button>
			</span>
		);
	},
	renderTaskController: function (task) {
		switch (task.get('Status')) {
		case 0: //started
			return (
				<div>
					<button className="button" onClick={this.onPause.bind(this, task)}><i className="fa fa-pause"></i>Pause</button>
					<button className="button" onClick={this.onStop.bind(this, task)}><i className="fa fa-stop"></i>Stop</button>
					<button className="button" onClick={this.onFinished.bind(this, task.get('Id'))}><i className="fa fa-check"></i>Finish</button>
				</div>
			);
			break;
		case 1: //stoped
			return (
				<div>
					<button className="button" onClick={this.onFinished.bind(this, task.get('Id'))}><i className="fa fa-check"></i>Finish</button>
				</div>
			);
			break;
		case 2: //peased
			return (
				<div>
					<button className="button" onClick={this.onStart.bind(this, task)}><i className="fa fa-play"></i>Start</button>
					<button className="button" onClick={this.onStop.bind(this, task)}><i className="fa fa-stop"></i>Stop</button>
					<button className="button" onClick={this.onFinished.bind(this, task.get('Id'))}><i className="fa fa-check"></i>Finish</button>
				</div>
			);
			break;
		default:
			return (
				<div>
					<button className="button" onClick={this.onStart.bind(this, task)}><i className="fa fa-play"></i>Start</button>
					<button className="button" onClick={this.onFinished.bind(this, task.get('Id'))}><i className="fa fa-check"></i>Finish</button>
				</div>
			);
			break;
		}
	},
	renderTaskMoreMenu: function (task) {
		if (task.get('Status') == 1) {
			var assignButton = null;
		} else {
			var assignButton = (
				<li>
					<a href="javascript:;" onClick={this.onAssign.bind(this, task)}>
						<i className="fa fa-users"></i>
						&nbsp;<span>Assign GTU</span>
					</a>
				</li>
			);
		}
		var parentClass = classNames({
			'is-dropdown-submenu-parent': true,
			'opens-left': true,
			'is-active': this.state.taskMoreMenuActive,
		});
		var menuClass = classNames({
			'menu': true,
			'submenu': true,
			'is-dropdown-submenu': true,
			'first-sub': true,
			'vertical': true,
			'js-dropdown-active': this.state.taskMoreMenuActive,
		});
		var taskId = task.get('Id');
		return (
			<ul className="dropdown menu float-right">
				<li className={parentClass}>
					<button className="button cirle" data-toggle="monitor-more-menu" onClick={this.onOpenMoreMenu}>
						<i className="fa fa-ellipsis-h"></i>
					</button>
					<ul className={menuClass} onClick={this.onCloseMoreMenu}>
						{assignButton}
						<li>
							<a href="javascript:;" onClick={this.onAddEmployee}>
								<i className="fa fa-user-plus"></i>
								&nbsp;<span>New Employee</span>
							</a>
						</li>
						<li>
							<a href="javascript:;" onClick={this.onEdit.bind(this, taskId)}>
								<i className="fa fa-edit"></i>
								&nbsp;<span>Edit</span>
							</a>
						</li>
						<li>
							<a id={taskId} href="javascript:;" onClick={this.onOpenUploadFile.bind(null, taskId)}>
								<i className="fa fa-cloud-upload"></i>
								&nbsp;<span>Import</span>
							</a>
						</li>
						<li>
							<a href="javascript:;" onClick={this.onSwitchDisplayMode.bind(this, 'marker')}>
								<i className="fa fa-map"></i>
								&nbsp;<span>Show Coverage</span>
							</a>
						</li>
						<li>
							<a href="javascript:;" onClick={this.onSwitchShowOutOfBoundaryGtu}>
								<i className={!this.state.ShowOutOfBoundaryGtu ? 'fa fa-compress' : 'fa fa-expand'}></i>
								&nbsp;<span>{!this.state.ShowOutOfBoundaryGtu ? 'Show Out of Bounds' : 'Hide Out of Bounds'}</span>
							</a>
						</li>
					</ul>
				</li>
			</ul>
		);
	},
	render: function () {
		var self = this,
			model = this.getModel(),
			tasks = model && model.get('Tasks') ? model.get('Tasks').models : [];

		tasks = filter(tasks, t => {
			return !t.get('IsFinished');
		});
		tasks = map(tasks, t => {
			return t.get('Id');
		});
		return (
			<div className="campaign-monitor">
				<div className="section row">
					<div className="small-12 columns">
						<div className="section-header">
							<div className="row">
								<div className="small-10 column">
									<blockquote>
										<h5>{model.getDisplayName()}</h5>
										<cite>{this.state.activeTask ? this.state.activeTask.get('Name') : 'NO ACTIVE TASK'}</cite>
									</blockquote>
								</div>
								<div className="small-2 column">
									{this.renderTaskDropdown()}
								</div>
							</div>
						</div>
				    </div>
		        </div>
		        {this.renderActiveTask()}
				<MapContainer model={this.getModel()} onSwitchActiveTask={this.onSwitchActiveTask} />
				<ImportFiles tasks={tasks} />
			</div>
		);
	}
});