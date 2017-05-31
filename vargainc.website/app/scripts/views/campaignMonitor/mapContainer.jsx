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
	head,
	last,
	concat,
	clone,
	get,
	set,
	trim,
	isFunction,
	includes
} from 'lodash';

import 'select2';

import GTUCollection from 'collections/gtu';
import DMap from 'models/print/dmap';
import GtuCollection from 'collections/gtu';
import UserCollection from 'collections/user';
import UserModel from 'models/user';
import TaskModel from 'models/task';
import GeoCollection from 'collections/geo';

import BaseView from 'views/base';
import AssignView from 'views/gtu/assign';
import EmployeeView from 'views/user/employee';
import EditView from 'views/monitor/edit';

import GeoControl from './geoControl';
const TAG = '[CAMPAIGN-MONITOR]';

export default React.createBackboneClass({
	mixins: [BaseView],
	shouldComponentUpdate: function () {
		return false;
	},
	getInitialState: function () {
		return {
			tasks: [],
			gtuMarkerLayer: {},
			gtuTrackLayer: {},
			gtuLocationLayer: null,
			displayGtus: [],
			showOutOfBoundaryGtu: true,
			taskBounds: {},
			mapCache: {},
			animations: {},
			popup: null,
			gtuMarkerFilter: ['all'],
			trackBeginTime: 0,
			gtuCount: {},
			displayMode: 'cover',
			showAllDMap: false
		}
	},
	onInit: function (mapContainer) {
		var self = this;
		mapboxgl.accessToken = MapboxToken;
		var monitorMap = new mapboxgl.Map({
			container: mapContainer,
			zoom: 8,
			maxZoom: 20,
			center: [-73.987378, 40.744556],
			sprite: "//timm.vargainc.com/map/sprite.json",
			style: '//timm.vargainc.com/map/street.json',
		});
		self.registerTopic(monitorMap);
		var nav = new mapboxgl.NavigationControl();
		monitorMap.addControl(nav, 'top-right');
		monitorMap.on('load', function () {
			monitorMap.addSource('google-road-tiles', {
				"type": "raster",
				"tiles": [
					"http://mt0.google.com/vt/lyrs=m&x={x}&y={y}&z={z}",
					"http://mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}",
					"http://mt2.google.com/vt/lyrs=m&x={x}&y={y}&z={z}",
					"http://mt3.google.com/vt/lyrs=m&x={x}&y={y}&z={z}",
				],
				"tileSize": 256
			});
			monitorMap.addSource('google-satellite-tiles', {
				"type": "raster",
				"tiles": [
					"http://mt0.google.com/vt/lyrs=y&x={x}&y={y}&z={z}",
					"http://mt1.google.com/vt/lyrs=y&x={x}&y={y}&z={z}",
					"http://mt2.google.com/vt/lyrs=y&x={x}&y={y}&z={z}",
					"http://mt3.google.com/vt/lyrs=y&x={x}&y={y}&z={z}",
				],
				"tileSize": 256
			});
			monitorMap.addLayer({
				"id": "google-road-tiles-layer",
				"type": "raster",
				"source": "google-road-tiles",
				"minzoom": 0,
				"maxzoom": 21,
				"layout": {
					"visibility": "visible"
				}
			});
			monitorMap.addLayer({
				"id": "google-satellite-tiles-layer",
				"type": "raster",
				"source": "google-satellite-tiles",
				"minzoom": 0,
				"maxzoom": 21,
				"layout": {
					"visibility": "none"
				}
			});

			self.setState({
				map: monitorMap,
				mapContainer: mapContainer
			}, self.initMapLayer);
		});
	},
	initMapLayer: function () {
		var monitorMap = this.state.map;
		var self = this;
		this.addGeoLayer();
		this.drawBoundary();

		/**
		 * animate
		 */
		requestAnimationFrame(self.animate);

		$(window).trigger('resize');
	},
	registerTopic: function (monitorMap) {
		var self = this;
		this.subscribe('Campaign.Monitor.GeoResult', data => {
			let source = monitorMap.getSource('geo-source');
			source && source.setData(data);
		});
		this.subscribe('Campaign.Monitor.ZoomToTask', taskId => {
			self.flyToTask(taskId);
		});
		this.subscribe('Campaign.Monitor.FlyToLocation', latlng => {
			self.flyToLocation(latlng);
		});
		this.subscribe('Campaign.Monitor.DrawGtu', data => {
			self.setState({
				tasks: data.tasks,
				displayGtus: data.displayGtus,
				displayMode: data.displayMode,
				trackBeginTime: null
			}, () => {
				self.drawGtu();
				window.clearInterval(self.state.reloadTimeout);
				self.setState({
					reloadTimeout: window.setInterval(self.reload, 10 * 1000)
				});
			});
		});
		this.subscribe('Campaign.Monitor.DisplayMode', mode => {
			self.setState({
				displayMode: mode,
				trackBeginTime: null
			});
		});
		this.subscribe('Campaign.Monitor.ShowOutOfBoundaryGtu', boolean => {
			self.setState({
				showOutOfBoundaryGtu: boolean
			}, () => {
				self.setGtuPointsFilter();
			});
		});
		this.subscribe('Campaign.Monitor.ShowAllDMap', boolean => {
			self.setState({
				showAllDMap: boolean
			}, () => {
				self.setDMapFilter();
			});
		});
		this.subscribe('Campaign.Monitor.SwitchGtu', gtus => {
			self.setState({
				displayGtus: gtus,
				trackBeginTime: null
			}, () => {
				self.setGtuPointsFilter();
			});
		});
		this.subscribe('Campaign.Monitor.LocatGtu', gtuId => {
			var source = monitorMap.getSource('GTU-LastLocation');
			if (source) {
				each(source._data.features, feature => {
					if (get(feature, 'properties.gtuId') == gtuId) {
						monitorMap.flyTo({
							center: [feature.geometry.coordinates[0], feature.geometry.coordinates[1]]
						});
						return false;
					}
				});
			}
		});
		this.subscribe('Campaign.Monitor.CenterToTask', () => {
			self.flyToTask(head(self.state.tasks).get('Id'), true);
		});
		this.subscribe('Campaign.Monitor.Reload', () => {
			self.reload();
		});
		this.subscribe('Campaign.Monitor.Redraw.FinishedTaskBoundary', taskId => {
			/**
			 * update dmap layer source
			 */
			var source = self.state.map.getSource('boundary-dmap');
			if (source) {
				let updateSource = map(source._data.features, feature => {
					if (get(feature, 'properties.TaskId') == taskId) {
						set(feature, 'properties.IsFinished', true);
						let task = get(feature, 'properties.Task');
						set(task, 'properties.Task.IsFinished', true);
					}
					return feature;
				});
				source.setData({
					"type": "FeatureCollection",
					"features": updateSource
				});
			}
			self.setDMapFilter();
		})
		this.subscribe('Global.Window.Resize', size => {
			if (self.state.mapContainer) {
				$('.mapbox-wrapper').width(`${size.width}px`);
				let mapHeight = size.height - $('.title-bar').outerHeight() - $('.section-header').outerHeight() - $('.map-toolbar').outerHeight() - 10;
				$('.mapbox-wrapper').height(`${mapHeight}px`);
				monitorMap.resize();
			}
		});

		this.on('click.map.popup', '.btnSetActiveTask', function () {
			var taskId = $(this).attr("id");
			self.publish("Map.Popup.SetActiveTask", taskId);
			self.state.popup && self.state.popup.remove();
			return false;
		});

		this.on('click.map.popup', '.btnMergeTask', function () {
			var taskId = $(this).attr("id");
			self.publish("Map.Popup.MergeTask", taskId);
			self.state.popup && self.state.popup.remove();
			return false;
		});
		this.on('click.map.popup', '.btnAbortMergeTask', function () {
			self.publish("Map.Popup.AbortMergeTask");
			self.state.popup && self.state.popup.remove();
			return false;
		});
		this.on('click.map.popup', '.btnMergeTaskDone', function () {
			self.publish("Map.Popup.ConfirmMergeTask");
			self.state.popup && self.state.popup.remove();
			return false;
		});



		monitorMap.on('click', function (e) {
			self.onTaskAreaClickHandler.call(self, e);
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
			return fn.call(self, monitorMap, time);
		})).then(() => {
			requestAnimationFrame(self.animate);
		});
	},
	addGeoLayer: function () {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			console.error('map is not ready');
			return;
		}
		monitorMap.addSource('geo-source', {
			type: 'geojson',
			data: {
				type: 'FeatureCollection',
				features: []
			}
		});
		monitorMap.addLayer({
			id: 'geo-layer',
			source: 'geo-source',
			type: 'symbol',
			layout: {
				'icon-allow-overlap': true,
				'icon-image': 'za-provincial-2',
				'icon-size': 2,
				'text-field': '{serial}'
			},
			paint: {
				'text-color': '#ffffff'
			}
		})
	},
	loadSubmapBoundary: function (campaignId, submapId) {
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
			return Promise.resolve([data, center]);
		});
	},
	loadDMapBoundary: function (task) {
		let url = `../api/print/campaign/${task.get('CampaignId')}/submap/${task.get('SubMapId')}/dmap/${task.get('DMapId')}/boundary`;
		return $.getJSON(url).then(response => {
			var data = {
				type: 'Feature',
				properties: {
					userColor: `rgb(${response.color.r}, ${response.color.g}, ${response.color.b})`,
					Name: task.get('Name'),
					IsFinished: task.get('IsFinished'),
					TaskId: task.get('Id'),
					Task: task.toJSON()
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
			return Promise.resolve([data, center]);
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
				return self.loadSubmapBoundary(campaignId, submapId)
			})).then(data => {
				let features = [];
				each(data, d => {
					features.push(d[0]);
					features.push(d[1]);
					each(d[0].geometry.coordinates, latlngGroup => {
						each(latlngGroup, latlng => {
							campaignBounds.extend([latlng[0], latlng[1]])
						});
					});
				});
				monitorMap.addSource(`boundary-submap`, {
					type: 'geojson',
					data: {
						"type": "FeatureCollection",
						"features": features
					}
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
						'line-color': 'black',
						'line-width': {
							stops: [
								[10, 1],
								[20, 4]
							]
						}
					}
				});
				return Promise.resolve();
			});
		}
		var drawDMapPromise = function () {
			return Promise.all(map(tasks.models, task => {
				return self.loadDMapBoundary(task)
			})).then(data => {
				let features = [];
				each(data, d => {
					features.push(d[0]);
					features.push(d[1]);
				});
				monitorMap.addSource(`boundary-dmap`, {
					type: 'geojson',
					data: {
						"type": "FeatureCollection",
						"features": features
					}
				});
				monitorMap.addLayer({
					id: `boundary-dmap-finished-layer`,
					type: 'fill',
					filter: [
						"all", ["==", "$type", "Polygon"],
						["==", "IsFinished", true]
					],
					source: `boundary-dmap`,
					paint: {
						'fill-pattern': 'bg-red-x',
						'fill-color': {
							property: 'userColor',
							type: 'identity',
						},
						'fill-opacity': 0.55,
					}
				});
				monitorMap.addLayer({
					id: `boundary-dmap-layer`,
					type: 'fill',
					filter: [
						"all", ["==", "$type", "Polygon"],
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
				});
				monitorMap.addLayer({
					id: `boundary-dmap-line-layer`,
					type: 'line',
					filter: ["==", "$type", "Polygon"],
					source: `boundary-dmap`,
					paint: {
						'line-color': 'black',
						'line-width': {
							stops: [
								[10, 1],
								[20, 4]
							]
						},
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
								[12, 16],
								[20, 36]
							]
						}
					},
					paint: {}
				});
				/**
				 * merge layers
				 */
				monitorMap.addLayer({
					id: `boundary-dmap-merge-source-layer`,
					type: 'line',
					filter: [
						"all", ["==", "$type", "Polygon"],
					],
					source: `boundary-dmap`,
					layout: {
						visibility: "none"
					},
					paint: {
						'line-color': ' #3498db',
						'line-width': 6
					}
				});
				monitorMap.addLayer({
					id: `boundary-dmap-merge-source-fill-layer`,
					type: 'fill',
					filter: [
						"all", ["==", "$type", "Polygon"],
					],
					source: `boundary-dmap`,
					layout: {
						visibility: "none"
					},
					paint: {
						'fill-color': ' #3498db',
						'fill-opacity': 0.25
					}
				});
				monitorMap.addLayer({
					id: `boundary-dmap-merge-target-layer`,
					type: 'line',
					filter: [
						"all", ["==", "$type", "Polygon"],
					],
					source: `boundary-dmap`,
					layout: {
						visibility: "none"
					},
					paint: {
						'line-color': '#c0392b',
						'line-width': 6
					}
				});
				monitorMap.addLayer({
					id: `boundary-dmap-merge-target-fill-layer`,
					type: 'fill',
					filter: [
						"all", ["==", "$type", "Polygon"],
					],
					source: `boundary-dmap`,
					layout: {
						visibility: "none"
					},
					paint: {
						'fill-color': '#c0392b',
						'fill-opacity': 0.25
					}
				});
				return Promise.resolve();
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
	setDMapFilter: function () {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			console.error('map is not ready');
			return;
		}
		var activeTask = head(this.state.tasks);
		var activeTaskId = activeTask ? activeTask.get('Id') : null;

		if (!this.state.showAllDMap && activeTaskId) {
			monitorMap.setFilter('boundary-dmap-finished-layer', [
				"all", ["==", "$type", "Polygon"],
				["==", "IsFinished", true],
				["==", "TaskId", activeTaskId]
			]);
			monitorMap.setFilter('boundary-dmap-layer', [
				"all", ["==", "$type", "Polygon"],
				["==", "IsFinished", false],
				["==", "TaskId", activeTaskId]
			]);
			monitorMap.setFilter('boundary-dmap-line-layer', [
				"all", ["==", "$type", "Polygon"],
				["==", "TaskId", activeTaskId]
			]);
			monitorMap.setFilter('boundary-dmap-label-layer', [
				"all", ["==", "$type", "Point"],
				["==", "TaskId", activeTaskId]
			]);
			monitorMap.setLayoutProperty('boundary-submap-layer', 'visibility', 'none');
		} else {
			monitorMap.setFilter('boundary-dmap-finished-layer', [
				"all", ["==", "$type", "Polygon"],
				["==", "IsFinished", true],
			]);
			monitorMap.setFilter('boundary-dmap-layer', [
				"all", ["==", "$type", "Polygon"],
				["==", "IsFinished", false],
			]);
			monitorMap.setFilter('boundary-dmap-line-layer', [
				"all", ["==", "$type", "Polygon"],
			]);
			monitorMap.setFilter('boundary-dmap-label-layer', [
				"all", ["==", "$type", "Point"],
			]);
			monitorMap.setLayoutProperty('boundary-submap-layer', 'visibility', 'visible');
		}

		var displayTasks = map(this.state.tasks, task => {
			return task.get('Id');
		});
		if (displayTasks.length > 1) {
			monitorMap.setFilter('boundary-dmap-finished-layer', [
				"all", ["==", "$type", "Polygon"],
				["==", "IsFinished", true],
				concat(["!in", "TaskId"], displayTasks)
			]);
			each(['boundary-dmap-merge-source-layer', 'boundary-dmap-merge-source-fill-layer'], layer => {
				monitorMap.setFilter(layer, [
					"all", ["==", "$type", "Polygon"],
					["==", "TaskId", displayTasks[0]]
				]);
				monitorMap.setLayoutProperty(layer, 'visibility', 'visible');
			});
			each(['boundary-dmap-merge-target-layer', 'boundary-dmap-merge-target-fill-layer'], layer => {
				monitorMap.setFilter(layer, [
					"all", ["==", "$type", "Polygon"],
					["==", "TaskId", displayTasks[1]]
				]);
				monitorMap.setLayoutProperty(layer, 'visibility', 'visible');
			});
		} else {
			monitorMap.setFilter('boundary-dmap-finished-layer', [
				"all", ["==", "$type", "Polygon"],
				["==", "IsFinished", true]
			]);
			each(['boundary-dmap-merge-source-layer', 'boundary-dmap-merge-source-fill-layer',
				'boundary-dmap-merge-target-layer', 'boundary-dmap-merge-target-fill-layer'
			], layer => {
				monitorMap.setLayoutProperty(layer, 'visibility', 'none');
			});
		}
	},
	buildTaskPopup: function (task) {
		var self = this;
		var taskStatus = null;
		var setActiveButton = null;
		var mergeTaskButton = null;
		var isFinished = task.IsFinished;
		var mergeTaskIdArray = map(self.state.tasks, task => {
			return task.get('Id');
		});
		if (isFinished) {
			taskStatus = [<span key={'finished'}>FINISHED</span>];
			if (self.state.tasks.length == 0) {
				mergeTaskButton = (
					<div className="small-7 small-centered columns">
						<div className="button-group stacked-for-small" style={{marginTop: "20px"}}>
							<button id={task.Id} className="button btnMergeTask">Merge From Here</button>
						</div>
					</div>
				);
			} else if (self.state.tasks.length == 1) {
				if (includes(mergeTaskIdArray, task.Id)) {
					mergeTaskButton = (
						<div className="small-7 small-centered columns">
							<div className="button-group stacked-for-small" style={{marginTop: "20px"}}>
								<button id={task.Id} className="button btnAbortMergeTask">Abort Merge</button>
							</div>
						</div>
					);
				} else {
					mergeTaskButton = (
						<div className="small-10 small-centered columns">
							<div className="button-group stacked-for-small" style={{marginTop: "20px"}}>
								<button id={task.Id} className="button btnAbortMergeTask">Abort Merge</button>
								<button id={task.Id} className="button btnMergeTask">Merge Preview</button>
							</div>
						</div>
					);
				}

			} else {
				if (includes(mergeTaskIdArray, task.Id)) {
					mergeTaskButton = (
						<div className="small-10 small-centered columns">
							<div className="button-group stacked-for-small" style={{marginTop: "20px"}}>
								<button id={task.Id} className="button btnAbortMergeTask">Abort Merge</button>
								<button id={task.Id} className="button btnMergeTaskDone">Confirm Merge</button>
							</div>
						</div>
					);
				} else {
					mergeTaskButton = (
						<div className="small-7 small-centered columns">
							<div className="button-group stacked-for-small" style={{marginTop: "20px"}}>
								<button id={task.Id} className="button btnAbortMergeTask">Abort Merge</button>
							</div>
						</div>
					);
				}
			}

		} else {
			setActiveButton = (
				<div className="small-8 align-center columns">
					<div className="button-group" style={{marginTop: "20px"}}>
						<button id={task.Id} className="button btnSetActiveTask">GO</button>
					</div>
				</div>
			);

			switch (task.Status) {
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
				<div className="small-7 columns">{task.Name}</div>
				<div className="small-5 columns text-right">{taskStatus}</div>
				{setActiveButton}
				{mergeTaskButton}
			</div>
		);

		return ReactDOMServer.renderToStaticMarkup(templat);
	},
	onTaskAreaClickHandler: function (e) {
		var monitorMap = this.state.map;
		var feature = head(monitorMap.queryRenderedFeatures(e.point, {
			layers: [
				'boundary-dmap-layer',
				'boundary-dmap-finished-layer',
				'boundary-dmap-merge-source-fill-layer',
				'boundary-dmap-merge-target-fill-layer'
			]
		}));
		if (feature) {
			var task = JSON.parse(feature.properties.Task);
			var popContent = this.buildTaskPopup(task);
			this.state.popup && this.state.popup.remove();
			this.state.popup = new mapboxgl.Popup({
					closeButton: false,
					anchor: 'top'
				}).setLngLat(e.lngLat)
				.setHTML(popContent)
				.addTo(monitorMap);
		}
	},
	reload: function () {
		if (!this.state.tasks || this.state.reloading) {
			return;
		}

		var self = this;
		return new Promise((resolve, reject) => {
			this.setState({
				reloading: true
			}, () => {
				var promiseQuery = [];
				each(self.state.tasks, task => {
					var dmap = task.get('dmap'),
						gtus = task.get('gtuList');
					promiseQuery.push(dmap.updateGtuAfterTime(null, {
						quite: true
					}));
					promiseQuery.push(gtus.fetchGtuLocation(task.get('Id'), {
						quite: true
					}));
				});


				Promise.all(promiseQuery).then(() => {
					self.setState({
						reloading: false
					}, () => {
						return resolve();
					});
				}).catch(() => {
					return reject(new Error('unkown error in reload'));
				});
			});
		}).then(() => {
			self.drawGtu();
			return Promise.resolve();
		})
	},
	drawGtu: function () {
		var map = this.state.map;
		var tasks = this.state.tasks;
		if (!map || !tasks) {
			return;
		}
		var gtus = [];
		each(tasks, task => {
			gtus = concat(gtus, task.get('dmap').get('Gtu'));
		});

		this.drawGtuLocation(gtus);

		this.drawGtuPoints(gtus);
	},
	drawGtuPoints: function (gtus) {
		var self = this;
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		// var taskId = this.state.task.get('Id');
		let geoJson = {
			type: "FeatureCollection",
			features: []
		};
		each(gtus, data => {
			each(data.points, (latlng, index) => {
				geoJson.features.push({
					"type": "Feature",
					"properties": {
						userColor: data.color,
						GtuId: latlng.Id,
						Out: latlng.out,
						Serial: index
					},
					"geometry": {
						"type": "Point",
						"coordinates": [latlng.lng, latlng.lat]
					}
				});
			});
			self.state.gtuCount[data.points[0].Id] = data.points.length;
		});
		var source = monitorMap.getSource(`gut-marker-source`);
		if (source == null) {
			monitorMap.addSource(`gut-marker-source`, {
				'type': 'geojson',
				'data': geoJson
			});
			monitorMap.addLayer({
				'id': `gut-marker-layer`,
				'type': 'circle',
				'source': `gut-marker-source`,
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
			}, "GTU-LastLocation-Walker-Icon-Layer");
			var self = this;
			this.state.animations['gut-marker-layer-animation'] = function (monitorMap, time) {
				if (this.state.trackBeginTime != null && this.state.displayMode != 'cover') {
					var filter = clone(this.state.gtuMarkerFilter);
					let currentTime = time - this.state.trackBeginTime;
					let gtu = head(this.state.displayGtus);
					let maxSerial = this.state.gtuCount[gtu] || 0;
					let currentSerial = parseInt(currentTime / 100);
					if(currentSerial > maxSerial){
						delete self.state.animations['gut-marker-layer-animation'];
						return;
					}
					filter.push(['<', 'Serial', currentSerial % maxSerial]);
					monitorMap.setFilter('gut-marker-layer', filter);
				} else {
					this.state.trackBeginTime = time;
				}
			};
		} else {
			source.setData(geoJson);
		}
		this.setGtuPointsFilter();
	},
	setGtuPointsFilter: function () {
		var monitorMap = this.state.map;
		var self = this;
		if (!monitorMap) {
			return;
		}
		var filter = ['all'];
		if (!this.state.showOutOfBoundaryGtu) {
			filter.push(['==', 'Out', false]);
		}
		if (this.state.displayGtus.length > 0) {
			filter.push(concat(['in', 'GtuId'], this.state.displayGtus));
		}
		this.setState({
			gtuMarkerFilter: filter
		}, () => {
			if (self.state.displayMode == 'cover') {
				monitorMap.setFilter('gut-marker-layer', filter);
			}
		});
	},
	drawGtuLocation: function (gtus) {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		var self = this;
		let gtuList = [];
		each(this.state.tasks, task => {
			task.get('gtuList').each(function (gtu) {
				if (gtu.get('Location') != null) {
					gtuList.push(gtu);
				}
			});
		})
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
						gtuId: gtu.get('Id')
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
					"icon-size": 0.45,
					"icon-allow-overlap": true
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
					"icon-size": 0.45,
					"icon-allow-overlap": true
				},
				"paint": {}
			});
		} else {
			source.setData(geoJson);
		}
	},
	flyToTask: function (taskId, keepZoom) {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		var taskSource = monitorMap.getSource('boundary-dmap');
		var feature = head(filter(taskSource._data.features, o => {
			return get(o, 'properties.TaskId') == taskId && get(o, 'geometry.type') == 'Polygon';
		}));
		if (feature) {
			let taskBounds = new mapboxgl.LngLatBounds();
			each(feature.geometry.coordinates, latlngGroup => {
				each(latlngGroup, latlng => {
					taskBounds.extend([latlng[0], latlng[1]]);
				});
			});
			if (!keepZoom) {
				monitorMap.fitBounds(taskBounds)
			} else {
				var mapZoom = monitorMap.getZoom();
				monitorMap.flyTo({
					center: taskBounds.getCenter(),
					zoom: mapZoom
				});
			}

		}
	},
	flyToLocation: function (latlng) {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		var mapZoom = monitorMap.getZoom();
		monitorMap.flyTo({
			center: latlng,
			zoom: mapZoom
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
	/**
	 * mapbox://styles/mapbox/streets-v9
	 * mapbox://styles/mapbox/outdoors-v9
	 * mapbox://styles/mapbox/light-v9
	 * mapbox://styles/mapbox/dark-v9
	 * mapbox://styles/mapbox/satellite-v9
	 * mapbox://styles/mapbox/satellite-streets-v9
	 */
	switchMapStyle: function (style) {
		var monitorMap = this.state.map;
		if (!monitorMap) {
			return;
		}
		var self = this;
		// map.off('style.load', this.initMapLayer);
		// map.on('style.load', this.initMapLayer);
		// map.setStyle('http://timm.vargainc.com/map/' + style + '.json?v9');
		if (style == 'streets') {
			monitorMap.setLayoutProperty('google-road-tiles-layer', 'visibility', 'visible');
			monitorMap.setLayoutProperty('google-satellite-tiles-layer', 'visibility', 'none');
		} else {
			monitorMap.setLayoutProperty('google-road-tiles-layer', 'visibility', 'none');
			monitorMap.setLayoutProperty('google-satellite-tiles-layer', 'visibility', 'visible');
		}
	},
	render: function () {
		return (
			<div className="mapbox-wrapper">
				<div className="mapbox-container" ref={this.onInit} style={{width: '100%', height: '100%'}}></div>
				<div className='map-overlay'>
					<div className="mapboxgl-ctrl-top-left">
						<div className="mapboxgl-ctrl mapboxgl-ctrl-group">
							<button className="mapboxgl-ctrl-icon mapboxgl-ctrl-img mapboxgl-ctrl-stree" onClick={this.switchMapStyle.bind(this, "streets")}></button>
							<button className="mapboxgl-ctrl-icon mapboxgl-ctrl-img mapboxgl-ctrl-satellite" onClick={this.switchMapStyle.bind(this, "satellite")}></button>
						</div>
					</div>
				</div>
				<div className="map-top-center">
					<GeoControl collection={this.props.geo} />
				</div>
			</div>
		);
	}
});