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
	trim,
	isFunction,
	includes
} from 'lodash';

import d3 from 'd3';

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

const TAG = '[CAMPAIGN-MONITOR]';
var GeoControl = React.createBackboneClass({
	mixins: [BaseView],
	getInitialState: function () {
		return {
			inputText: '',
			placeHolder: 'Please input Address or Zip code here',
			activeItem: null,
			searchTimeout: null
		}
	},
	search: function () {
		clearTimeout(this.state.searchTimeout);
		var self = this;
		this.state.searchTimeout = setTimeout(() => {
			var geocode = trim(self.state.inputText);
			if (geocode.length > 0) {
				var promiseQuery = null;
				if (/^\d+$/.test(geocode)) {
					promiseQuery = self.getCollection().fetchByZipcode(geocode);
				} else {
					promiseQuery = self.getCollection().fetchByAddress(geocode);
				}
				promiseQuery.then(geoJson => {
					self.publish('Campaign.Monitor.GeoResult', geoJson);
				});
			}
		}, 500);
	},
	onInputChange: function (event) {
		var self = this;
		this.setState({
			inputText: event.target.value
		}, () => {
			self.search();
		})
	},
	onInputKeyUp: function (event) {
		var results = this.getCollection() || [];
		if (results.length == 0) {
			return;
		}
		switch (event.key) {
		case 'ArrowUp':
			var index = results.indexOf(this.state.activeItem);
			index--;
			index = Math.max(0, index);
			this.onSelectItem(results.at(index));
			event.preventDefault();
			event.stopPropagation();
			break;
		case 'ArrowDown':
			var index = results.indexOf(this.state.activeItem);
			index++;
			index = Math.min(index, results.length - 1);
			this.onSelectItem(results.at(index));
			event.preventDefault();
			event.stopPropagation();
			break;
		case 'Enter':
			var activeItem = null;
			if (this.state.activeItem == null) {
				activeItem = results.at(0);
			} else {
				activeItem = this.state.activeItem;
			}
			this.onSelectItem(activeItem);
			event.preventDefault();
			event.stopPropagation();
			break;
		}
	},
	onSelectItem: function (item, callback) {
		if (item) {
			var self = this;
			this.setState({
				activeItem: item,
				inputText: item.get('place')
			}, () => {
				this.publish('Campaign.Monitor.FlyToLocation', item.get('latlng'));
				if (isFunction(callback)) {
					callback.call(self);
				}
			});
		}
	},
	clearUp: function () {
		var self = this;
		var placeHolder = '';
		if (this.state.activeItem) {
			placeHolder = this.state.activeItem.get('text');
		}
		this.setState({
			activeItem: null,
			inputText: '',
			placeHolder: placeHolder
		}, () => {
			self.getCollection().reset();
			$(window).focus();
			self.publish('Campaign.Monitor.GeoResult', {
				type: 'FeatureCollection',
				features: []
			});
		});
	},
	stopClearUp: function (e) {
		e.preventDefault();
		e.stopPropagation();
	},
	render: function () {
		var self = this;
		var results = this.getCollection() || [];
		var geoClass = classNames("geocode", {
			active: results.length > 1
		});
		return (
			<div className={geoClass} onKeyUp={this.onInputKeyUp} onClick={this.stopClearUp}>
				<input type="text" value={this.state.inputText} onKeyUp={this.onInputKeyUp} onChange={this.onInputChange} placeholder={this.state.placeHolder} />
				<ul className="accordion">
					{results.map((item, index)=>{
						let itemClass = classNames('accordion-item', {
							active: self.state.activeItem == item
						});
						return (
							<li key={item.get('id')} className={itemClass} onClick={self.onSelectItem.bind(self, item)}>
								<i className="fa fa-stop"></i>
								<span>{index + 1}</span>
								&nbsp;{item.get('place')}
							</li>
						);
					})}
				</ul>
			</div>
		);
	},
	componentWillUnmount: function () {
		$(window).off('click.geo');
	},
	componentDidMount: function () {
		var self = this;
		$(window).on('click.geo', () => {
			self.clearUp();
		});
	}
});
var MapContainer = React.createBackboneClass({
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
			style: 'http://timm.vargainc.com/map/street.json',
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
		this.loadBoundary().delay(500).then(function () {
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
		this.subscribe('Campaign.Monitor.GeoResult', data => {
			monitorMap.getSource('geo-source').setData(data);
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
			self.flyToTask(head(self.state.tasks).get('Id'), true);
		});
		this.subscribe('Campaign.Monitor.Reload', () => {
			self.reload();
		});
		this.subscribe('Campaign.Monitor.Redraw.FinishedTaskBoundary', taskId => {
			self.drawBoundary();
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
			return Promise.resolve();
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
				let geoData = {
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
						'line-color': 'black',
						'line-width': {
							stops: [
								[10, 1],
								[20, 4]
							]
						}
					}
				});

				each(data, feature => {
					each(feature.geometry.coordinates, latlngGroup => {
						each(latlngGroup, latlng => {
							campaignBounds.extend([latlng[0], latlng[1]])
						});
					});
				})
				return Promise.resolve();
			});
		}
		var drawDMapPromise = function () {
			let geoData = {
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
								[12, 8],
								[20, 26]
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
			boundary - submap - layer
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
		var mergeTaskIdArray = map(self.state.tasks, task=>{
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

			this.state.animations['gut-marker-layer-animation'] = function (monitorMap, time) {
				if (this.state.trackBeginTime != null && this.state.displayMode != 'cover') {
					var filter = clone(this.state.gtuMarkerFilter);
					let currentTime = time - this.state.trackBeginTime;
					let gtu = head(this.state.displayGtus);
					let maxSerial = this.state.gtuCount[gtu] || 0;
					filter.push(['<', 'Serial', parseInt(currentTime / 100) % maxSerial]);
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
		// var self = this;
		// let taskIsStopped = this.state.task.get('Status') == 1,
		// 	gtuList = this.state.task.get('gtuList').where(function (gtu) {
		// 		return gtu.get('Location') != null;
		// 	});
		// var geoJson = {
		// 	type: 'FeatureCollection',
		// 	features: map(gtuList, gtu => {
		// 		let latlng = gtu.get('Location');
		// 		if (!latlng || !latlng.lat || !latlng.lng) {
		// 			return null;
		// 		}
		// 		return {
		// 			type: 'Feature',
		// 			geometry: {
		// 				type: 'Point',
		// 				coordinates: [latlng.lng, latlng.lat]
		// 			},
		// 			properties: {
		// 				userColor: '#ff0000',
		// 				role: gtu.get('Role'),
		// 				gtuId: gtu.get('Id'),
		// 				outOfBoundary: gtu.get('OutOfBoundary'),
		// 				visiable: taskIsStopped ? gtu.get('WithData') : gtu.get('IsAssign') || gtu.get('WithData'),
		// 			}
		// 		}
		// 	})
		// };
		// var source = monitorMap.getSource(`GTU-LastLocation`);
		// if (source == null) {
		// 	monitorMap.addSource(`GTU-LastLocation`, {
		// 		"type": 'geojson',
		// 		"data": geoJson
		// 	});
		// 	monitorMap.addLayer({
		// 		"filter": [
		// 			"all", ["==", "role", "Walker"]
		// 		],
		// 		"id": `GTU-LastLocation-Walker-Icon-Layer`,
		// 		"source": 'GTU-LastLocation',
		// 		"type": "symbol",
		// 		"layout": {
		// 			"icon-image": "walker",
		// 			"icon-size": 0.45,
		// 			"icon-allow-overlap": true
		// 		},
		// 		"paint": {}
		// 	});
		// 	monitorMap.addLayer({
		// 		"filter": [
		// 			"all", ["!=", "role", "Walker"]
		// 		],
		// 		"id": `GTU-LastLocation-Trunk-Icon-Layer`,
		// 		"source": 'GTU-LastLocation',
		// 		"type": "symbol",
		// 		"layout": {
		// 			"icon-image": "truck",
		// 			"icon-size": 0.45,
		// 			"icon-allow-overlap": true
		// 		},
		// 		"paint": {}
		// 	});
		// } else {
		// 	source.setData(geoJson);
		// }
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
			displayGtus: [],
			showOutOfBoundaryGtu: true,
			displayMode: 'cover',
			showAllDMap: false,
			mergeTasks: []
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

		this.subscribe('Map.Popup.MergeTask', taskId => {
			var model = self.getModel();
			var tasks = model && model.get('Tasks');
			if (tasks && tasks.size && tasks.size() > 0) {
				var currentTask = find(tasks.models, i => {
					return i.get('Id') == taskId;
				});
				if (currentTask) {
					self.state.mergeTasks.push(currentTask);
					self.onMergeTask.call(self, currentTask);
				}
			}
		});
		this.subscribe('Map.Popup.AbortMergeTask', () => {
			self.setState({
				mergeTasks: []
			}, () => {
				self.publish('Campaign.Monitor.DrawGtu', {
					'tasks': [],
					'displayGtus': [],
					'displayMode': self.state.displayMode
				});
				self.publish('Campaign.Monitor.ShowAllDMap', self.state.showAllDMap);
				self.publish('Campaign.Monitor.ShowOutOfBoundaryGtu', self.state.showOutOfBoundaryGtu);
			});
		});
		this.subscribe('Map.Popup.ConfirmMergeTask', tasks => {
			var model = self.getModel();
			var tasks = model && model.get('Tasks');
			if (tasks && tasks.size && self.state.mergeTasks.length == 2) {
				var sourceTask = head(self.state.mergeTasks);
				var targetTask = last(self.state.mergeTasks);
				targetTask.mergeFrom(sourceTask.get('Id')).then(()=>{
					self.state.mergeTasks = [];
					self.state.mergeTasks.push(targetTask);
					self.onMergeTask.call(self, targetTask);
				});
			}
		});

		this.on('click.import.task', '.btnImportTask', function (evt) {
			evt.preventDefault();
			evt.stopPropagation();
			var taskId = $(this).attr('id');
			$("#upload-file-" + taskId).click();
		});

		this.subscribe('Global.Window.Click', () => {
			self.onCloseDropDown();
			self.onCloseMoreMenu();
		});
	},
	showTask: function (task) {
		this.publish('Campaign.Monitor.ZoomToTask', task);
	},
	loadTask: function (task) {
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
			return Promise.resolve();
		});
	},
	onSwitchActiveTask: function (task) {
		var self = this;
		this.loadTask(task).then(() => {
			return new Promise((resolve, reject) => {
				var taskDMap = task.get('dmap');
				/**
				 * set all gtu as default display
				 */
				var displayGtus = taskDMap.get('Gtu') || [];
				displayGtus = map(displayGtus, gtu => {
					return gtu.points && gtu.points.length > 0 ? gtu.points[0].Id : null;
				});
				self.setState({
					'activeTask': task,
					'displayGtus': displayGtus,
					'displayMode': 'cover'
				}, () => {
					self.publish('Campaign.Monitor.ZoomToTask', task.get('Id'));
					self.publish('Campaign.Monitor.DrawGtu', {
						'tasks': [task],
						'displayGtus': displayGtus,
						'displayMode': 'cover'
					});
					self.publish('Campaign.Monitor.ShowAllDMap', self.state.showAllDMap);
					$(window).trigger('resize');
					return resolve();
				});
			});
		});
	},
	onMergeTask: function () {
		var self = this;
		return Promise.all(map(this.state.mergeTasks, task => {
			return self.loadTask(task);
		})).then(() => {
			var displayGtus = [];
			each(self.state.mergeTasks, task => {
				var gtus = task.get('dmap').get('Gtu');
				each(gtus, gtu => {
					gtu.points && gtu.points.length > 0 && displayGtus.push(gtu.points[0].Id);
				});
			});
			self.publish('Campaign.Monitor.DrawGtu', {
				'tasks': this.state.mergeTasks,
				'displayGtus': displayGtus,
				'displayMode': 'cover'
			});
			self.publish('Campaign.Monitor.ShowAllDMap', true);
			self.publish('Campaign.Monitor.ShowOutOfBoundaryGtu', true);
			$(window).trigger('resize');
			return Promise.resolve();
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
			self.publish('Campaign.Monitor.DisplayMode', mode);
		});
		if (mode != 'marker') {
			var displayGtus = [head(this.state.displayGtus)];
			this.setState({
				displayGtus: displayGtus
			}, () => {
				self.publish('Campaign.Monitor.SwitchGtu', displayGtus);
			})
		}
	},
	onSwitchShowOutOfBoundaryGtu: function () {
		var self = this;
		var showOutOfBoundaryGtu = !this.state.showOutOfBoundaryGtu;
		this.setState({
			showOutOfBoundaryGtu: showOutOfBoundaryGtu,
		}, () => {
			self.publish('Campaign.Monitor.ShowOutOfBoundaryGtu', showOutOfBoundaryGtu);
		});
	},
	onSwitchShowAllDMap: function (bool) {
		var self = this;
		this.setState({
			showAllDMap: bool,
		}, () => {
			self.publish('Campaign.Monitor.ShowAllDMap', bool);
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
		var displayGtus;
		if (this.state.displayMode == 'track') {
			displayGtus = [gtuId];
		} else {
			displayGtus = xor(this.state.displayGtus, [gtuId]);
		}

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
			buttonClass = 'button text-left btn-gtu',
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
					<span>{gtu.get('ShortUniqueID')}</span>
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
						<li className={this.state.showAllDMap ? 'hide' : ''}>
							<a href="javascript:;" onClick={this.onSwitchShowAllDMap.bind(this, true)}>
								<i className="fa fa-th-large"></i>
								&nbsp;<span>Show Other DMap</span>
							</a>
						</li>
						<li className={this.state.showAllDMap ? '' : 'hide'}>
							<a href="javascript:;" onClick={this.onSwitchShowAllDMap.bind(this, false)}>
								<i className="fa fa-square-o"></i>
								&nbsp;<span>Hide Other DMap</span>
							</a>
						</li>
						<li className={`${this.state.displayMode == 'cover' ? 'hide': ''}`}>
							<a href="javascript:;" onClick={this.onSwitchDisplayMode.bind(this, 'cover')}>
								<i className="fa fa-map"></i>
								&nbsp;<span>Show Coverage</span>
							</a>
						</li>
						<li className={`${this.state.displayMode == 'track' ? 'hide': ''}`}>
							<a href="javascript:;" onClick={this.onSwitchDisplayMode.bind(this, 'track')}>
								<i className="fa fa-map-o"></i>
								&nbsp;<span>Track Path</span>
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
				<MapContainer model={this.getModel()} geo={this.props.geo} onSwitchActiveTask={this.onSwitchActiveTask} />
				<ImportFiles tasks={tasks} />
			</div>
		);
	}
});