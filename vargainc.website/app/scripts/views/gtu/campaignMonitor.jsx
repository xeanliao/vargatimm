import Backbone from 'backbone';
import React from 'react';
import ReactDOMServer from 'react-dom/server';
import 'react.backbone';
import localStorage from 'localforage';
import Promise from 'bluebird';
import mapboxgl from 'mapbox-gl/dist/mapbox-gl-dev';
import turfCentroid from '@turf/centroid';
import turfEnvelope from '@turf/envelope';
import $ from 'jquery';
import classNames from 'classnames';
import {
	groupBy,
	map,
	each,
	find,
	filter,
	indexOf,
	xor,
	extend,
	startsWith,
	keys,
	head
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
			drawMode: 'marker',
			taskBounds: {},
			mapCache: {},
			animations: {}
		}
	},
	onInit: function (mapContainer) {
		var self = this;
		mapboxgl.accessToken = 'pk.eyJ1IjoiZ2hvc3R1eiIsImEiOiJjaXczc2tmc3cwMDEyMm9tb29pdDRwOXUzIn0.KPSiOO6DWTY59x1zHdvYSA';
		var monitorMap = new mapboxgl.Map({
			container: mapContainer,
			zoom: 8,
			center: [-73.987378, 40.744556],
			style: 'http://timm.vargainc.com/map/street.json',
		});
		self.registerTopic(monitorMap);
		var nav = new mapboxgl.NavigationControl();
		monitorMap.addControl(nav, 'top-right');
		monitorMap.on('load', function () {
			self.setState({
				map: monitorMap,
				mapContainer: mapContainer
			}, self.initMapLayer);
		});
	},
	initMapLayer: function () {
		var monitorMap = this.state.map;
		var self = this;
		each(monitorMap.style._layers, (v, k) => {
			console.log(k);
		});

		this.loadBoundary().then(function () {
			return self.drawBoundary();
		});

		/**
		 * animate
		 */
		requestAnimationFrame(self.animate);

		$(window).trigger('resize');
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
			var feature = head(monitorMap.querySourceFeatures('GTU-LastLocation', {
				filter: ["==", "gtuId", gtuId]
			}));
			if (feature) {
				monitorMap.flyTo({
					center: [feature.geometry.coordinates[0], feature.geometry.coordinates[1]]
				});
			}
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
				$('.mapbox-wrapper').width(`${size.width}px`);
				let mapHeight = size.height - $('.title-bar').outerHeight() - $('.section-header').outerHeight() - $('.map-toolbar').outerHeight() - 30;
				$('.mapbox-wrapper').height(`${mapHeight}px`);
				monitorMap.resize();
			}
		});

		this.on('click.map.popup', '.btnSetActiveTask', function () {
			var taskId = $(this).attr("id");
			self.publish("Map.Popup.SetActiveTask", taskId);
			self.state.map.closePopup()
		});
	},
	animate: function (time) {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		var self = this;
		time = parseInt(time);
		Promise.all(map(this.state.animations, fn => {
			return fn.call(monitorMap, time);
		})).then(() => {
			requestAnimationFrame(self.animate);
		});
	},
	loadBoundary: function () {
		console.time("load boundary");
		this.publish('showLoading');
		var self = this,
			model = this.getModel(),
			campaignId = model.get('Id'),
			tasks = model.get('Tasks'),
			submapTasks = groupBy(tasks.models, t => {
				return t.get('SubMapId')
			}),
			submaps = keys(submapTasks);
		var loadSubmapBoundaryPromise = function () {
			return Promise.each(submaps, submapId => {
				return localStorage.getItem(`boundary-submap-${submapId}`).then(result => {
					if (result) {
						return Promise.resolve();
					} else {
						return $.getJSON(`../api/print/campaign/${campaignId}/submap/${submapId}/boundary`).then(response => {
							let data = {
								type: 'Feature',
								properties: {
									userColor: `rgb(${response.color.r}, ${response.color.g}, ${response.color.b})`
								},
								geometry: {
									type: 'Polygon',
									coordinates: [map(response.boundary, i => {
										return [i.lng, i.lat]
									})]
								}
							};
							let center = turfCentroid(data);
							center.properties = data.properties;
							return Promise.all([
								localStorage.setItem(`boundary-submap-${submapId}`, data),
								localStorage.setItem(`boundary-submap-${submapId}-center`, center),
							]);
						});
					}
				});
			});
		}
		var loadDMapBoundaryPromise = function () {
			return Promise.each(tasks.models, task => {
				return localStorage.getItem(`boundary-dmap-${task.get('DMapId')}`).then(result => {
					if (result) {
						return Promise.resolve();
					} else {
						let url = `../api/print/campaign/${task.get('CampaignId')}/submap/${task.get('SubMapId')}/dmap/${task.get('DMapId')}/boundary`;
						$.getJSON(url).then(response => {
							var data = {
								type: 'Feature',
								properties: {
									userColor: `rgb(${response.color.r}, ${response.color.g}, ${response.color.b})`,
									Name: task.get('Name'),
									IsFinished: task.get('IsFinished'),
									TaskId: task.get('Id')
								},
								geometry: {
									type: 'Polygon',
									coordinates: [map(response.boundary, i => {
										return [i.lng, i.lat]
									})]
								},
							};
							let center = turfCentroid(data);
							center.properties = data.properties;
							return Promise.all([
								localStorage.setItem(`boundary-dmap-${task.get('DMapId')}`, data),
								localStorage.setItem(`boundary-dmap-${task.get('DMapId')}-center`, center),
							]);
						});
					}
				});
			});
		}
		return Promise.all([
			loadSubmapBoundaryPromise(),
			loadDMapBoundaryPromise()
		]).then(() => {
			self.publish('hideLoading');
			console.timeEnd("load boundary");
		});
	},
	drawBoundary: function () {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			console.error('map is not ready');
			return;
		}
		console.time("draw boundary");
		this.publish('showLoading');
		var self = this;
		var model = this.getModel();
		var campaignId = model.get('Id');
		var tasks = model.get('Tasks');
		var submapTasks = groupBy(tasks.models, t => {
			return t.get('SubMapId');
		});
		var submaps = keys(submapTasks);
		var submapBoudnar = [];
		var campaignBounds = new mapboxgl.LngLatBounds();
		var drawSubmapPromise = function () {
			return Promise.all(map(submaps, submapId => {
				return localStorage.getItem(`boundary-submap-${submapId}`);
			})).then(data => {
				var geoData = {
					"type": "FeatureCollection",
					"features": data
				};
				monitorMap.addSource(`boundary-submap`, {
					type: 'geojson',
					data: geoData
				});
				monitorMap.addLayer({
					id: `boundary-submap-layer`,
					type: 'line',
					source: `boundary-submap`,
					layout: {
						'line-join': 'round',
						'line-cap': 'round',
					},
					paint: {
						// 'line-color': {
						// 	property: 'userColor',
						// 	type: 'identity',
						// },
						'line-color': 'black',
						'line-width': {
							stops:[
								[10, 1],
								[20, 4]
							]
						}
					}
				});
				// monitorMap.addLayer({
				// 	id: `boundary-submap-shadow-layer`,
				// 	type: 'line',
				// 	source: `boundary-submap`,
				// 	layout: {
				// 		'line-join': 'round',
				// 		'line-cap': 'round',
				// 	},
				// 	paint: {
				// 		// 'line-color': {
				// 		// 	property: 'userColor',
				// 		// 	type: 'identity',
				// 		// },
				// 		'line-color': 'black',
				// 		'line-width': {
				// 			stops:[
				// 				[10, 2],
				// 				[20, 4]
				// 			]
				// 		},
				// 		'line-blur': 4,
				// 		'line-offset': 4,
				// 		'line-opacity': 0.25,
				// 	}
				// });
				each(data, feature => {
					each(feature.geometry.coordinates, latlngGroup => {
						each(latlngGroup, latlng=>{
							campaignBounds.extend([latlng[0], latlng[1]])	
						});
					});
				})
				return Promise.resolve();
			})
		}
		var drawDMapPromise = function () {
			var geoData = {
				"type": "FeatureCollection",
				"features": []
			};
			return Promise.all(map(tasks.models, task => {
				return Promise.all([
					localStorage.getItem(`boundary-dmap-${task.get('DMapId')}`),
					localStorage.getItem(`boundary-dmap-${task.get('DMapId')}-center`)
				]).then(result => {
					geoData.features.push(result[0]);
					geoData.features.push(result[1]);
				});
			})).then(() => {
				monitorMap.addSource(`boundary-dmap`, {
					type: 'geojson',
					data: geoData
				});
				monitorMap.addLayer({
					id: `boundary-dmap-finished-layer`,
					type: 'fill',
					filter: [
						"all", 
						["==", "$type", "Polygon"],
						["==", "IsFinished", true]
					],
					source: `boundary-dmap`,
					paint: {
						// 'fill-pattern': 'bg-red-x',
						'fill-color': {
							property: 'userColor',
							type: 'identity',
						},
						'fill-opacity': 0.25,
					}
				}, 'ferry');
				monitorMap.addLayer({
					id: `boundary-dmap-layer`,
					type: 'fill',
					filter: [
						"all", 
						["==", "$type", "Polygon"],
						["==", "IsFinished", false]
					],
					source: `boundary-dmap`,
					paint: {
						'fill-color': {
							property: 'userColor',
							type: 'identity',
						},
						'fill-opacity': 0.25,
					}
				}, 'ferry');
				monitorMap.addLayer({
					id: `boundary-dmap-line-layer`,
					type: 'line',
					filter: ["==", "$type", "Polygon"],
					source: `boundary-dmap`,
					paint: {
						// 'line-color': {
						// 	property: 'userColor',
						// 	type: 'identity',
						// },
						'line-color': 'black',
						'line-width': {
							stops:[
								[10, 1],
								[20, 4]
							]
						},
						// 'line-offset': 2,
						'line-opacity': 0.75,
					}
				});
				monitorMap.addLayer({
					id: `boundary-dmap-label-layer`,
					type: 'symbol',
					minzoom: 12,
					filter: ["==", "$type", "Point"],
					source: `boundary-dmap`,
					layout: {
						"text-field": "{Name}",
						"text-size": {
							stops: [
								[12, 8],
								[20, 26]
							]
						}
					},
					paint: {
					}
				});
			});
		}
		return Promise.all([
			drawSubmapPromise(),
			drawDMapPromise()
		]).then(() => {
			monitorMap.fitBounds(campaignBounds);
			self.publish('hideLoading');
			console.timeEnd("draw boundary");
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
						<button id={task.get('Id')} className="button btnSetActiveTask">GO</button>
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
		this.drawGtuPoints(gtus);
		this.drawGtuLocation(gtus);
	},
	updateGtu: function () {
		// setLatLngs
	},
	drawGtuPoints: function (gtus) {
		var self = this;
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		var taskId = this.state.task.get('Id');
		each(gtus, data => {
			let gtuId = data.points[0].Id;
			let geoJson = {
				'type': 'Feature',
				'geometry': {
					'type': 'MultiPoint',
					'coordinates': map(data.points, latlng => {
						return [latlng.lng, latlng.lat]
					})
				},
				properties: {
					userColor: data.color
				},
			};
			var source = monitorMap.getSource(`gut-marker-${taskId}-${gtuId}-source`);
			if (source == null) {
				monitorMap.addSource(`gut-marker-${taskId}-${gtuId}-source`, {
					'type': 'geojson',
					'data': geoJson
				});
				monitorMap.addLayer({
					'id': `gut-marker-${taskId}-${gtuId}-layer`,
					'type': 'circle',
					'source': `gut-marker-${taskId}-${gtuId}-source`,
					'paint': {
						'circle-radius': {
							stops: [
								[4, 2],
								[20, 8]
							]
						},
						'circle-color': {
							property: 'userColor',
							type: 'identity',
						},
						'circle-stroke-width': 1,
						'circle-stroke-color': 'black'
					}
				});
			} else {
				source.setData(geoJson);
			}
			if (indexOf(this.state.displayGtus, gtuId) == -1) {
				monitorMap.setLayoutProperty(`gut-marker-${taskId}-${gtuId}-layer`, 'visibility', 'none');
			} else {
				monitorMap.setLayoutProperty(`gut-marker-${taskId}-${gtuId}-layer`, 'visibility', 'visible');
			}
		});
	},
	drawGtuTrack: function (gtus) {
		// each(this.state.gtuMarkerLayer, layer => {
		// 	layer.eachLayer(marker => {

		// 	});
		// });
	},
	drawGtuLocation: function (gtus) {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		var self = this;
		let taskIsStopped = this.state.task.get('Status') == 1,
			gtuList = this.state.task.get('gtuList').where(function (gtu) {
				return gtu.get('Location') != null;
			});
		var geoJson = {
			type: 'FeatureCollection',
			features: map(gtuList, gtu => {
				let latlng = gtu.get('Location');
				if (!latlng || !latlng.lat || !latlng.lng) {
					return null;
				}
				return {
					type: 'Feature',
					geometry: {
						type: 'Point',
						coordinates: [latlng.lng, latlng.lat]
					},
					properties: {
						userColor: '#ff0000',
						role: gtu.get('Role'),
						gtuId: gtu.get('Id'),
						outOfBoundary: gtu.get('OutOfBoundary'),
						visiable: taskIsStopped ? gtu.get('WithData') : gtu.get('IsAssign') || gtu.get('WithData'),
					}
				}
			})
		};
		var source = monitorMap.getSource(`GTU-LastLocation`);
		if (source == null) {
			monitorMap.addSource(`GTU-LastLocation`, {
				"type": 'geojson',
				"data": geoJson
			});
			monitorMap.addLayer({
				"filter": [
					"all", ["==", "role", "Walker"]
				],
				"id": `GTU-LastLocation-Walker-Icon-Layer`,
				"source": 'GTU-LastLocation',
				"type": "symbol",
				"layout": {
					"icon-image": "walker",
					"icon-size": 0.5
				},
				"paint": {}
			});
			monitorMap.addLayer({
				"filter": [
					"all", ["!=", "role", "Walker"]
				],
				"id": `GTU-LastLocation-Trunk-Icon-Layer`,
				"source": 'GTU-LastLocation',
				"type": "symbol",
				"layout": {
					"icon-image": "truck",
					"icon-size": 0.5
				},
				"paint": {}
			});
		} else {
			source.setData(geoJson);
		}
	},
	flyToTask: function (taskId) {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		var feature = head(monitorMap.querySourceFeatures('boundary-dmap', {
			filter: ["==", "TaskId", taskId]
		}));
		if (feature) {
			let taskBounds = new mapboxgl.LngLatBounds();
			each(feature.geometry.coordinates, latlngGroup=>{
				each(latlngGroup, latlng=>{
					taskBounds.extend([latlng[0], latlng[1]]);	
				});				
			});
			monitorMap.fitBounds(taskBounds)
		}
	},
	clearTask: function () {
		each(gtuMarkerLayer, layer => {
			layer && layer.remove && layer.remove();
		});
		this.setState({
			gtuMarkerLayer: []
		});
	},
	/**
	 * mapbox://styles/mapbox/streets-v9
	 * mapbox://styles/mapbox/outdoors-v9
	 * mapbox://styles/mapbox/light-v9
	 * mapbox://styles/mapbox/dark-v9
	 * mapbox://styles/mapbox/satellite-v9
	 * mapbox://styles/mapbox/satellite-streets-v9
	 */
	switchMapStyle: function (style) {
		var map = this.state.map;
		if (!map) {
			return;
		}
		var self = this;
		map.off('style.load', this.initMapLayer);
		map.on('style.load', this.initMapLayer);
		map.setStyle('http://timm.vargainc.com/map/' + style + '.json?v9');
	},
	repaint: function () {
		this.drawBoundary();
	},
	render: function () {
		return (
			<div className="mapbox-wrapper">
				<div className="mapbox-container" ref={this.onInit} style={{width: '100%', height: '100%'}}></div>
				<div className='map-overlay'>
					<div className="mapboxgl-ctrl-top-left">
						<div className="mapboxgl-ctrl mapboxgl-ctrl-group">
							<button className="mapboxgl-ctrl-icon mapboxgl-ctrl-img mapboxgl-ctrl-stree" onClick={this.switchMapStyle.bind(this, "street")}></button>
							<button className="mapboxgl-ctrl-icon mapboxgl-ctrl-img mapboxgl-ctrl-satellite" onClick={this.switchMapStyle.bind(this, "satellite")}></button>
						</div>
					</div>
				</div>
			</div>
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
				$(window).trigger('resize');
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
			return true;
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
			<div className="map-toolbar">
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
				<button className="button location" onClick={this.onGotoGTU.bind(null, gtu.get('Id'))} style={{'backgroundColor': gtu.get('UserColor')}}>
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
				<div className="section row expanded">
					<div className="small-12 columns">
						<div className="section-header">
							<div className="row expanded">
								<div className="small-10 column">
									<nav aria-label="You are here:" role="navigation">
									  	<ul className="breadcrumbs">
									    	<li>{model.getDisplayName()}</li>
									    	<li>
									      		<span className="show-for-sr">Current: </span> {this.state.activeTask ? this.state.activeTask.get('Name') : null}
									    	</li>
									  	</ul>
									</nav>
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