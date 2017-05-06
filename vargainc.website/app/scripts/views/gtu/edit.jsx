import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import moment from 'moment';
import {
	each,
	groupBy,
	indexOf,
	remove,
	filter,
	last,
	map,
	isEmpty,
	uniqueId,
	first,
	dropRight
} from 'lodash';
import $ from 'jquery';
// import FastMarker from 'lib/fastMarker';
import mapboxgl from 'mapbox-gl/dist/mapbox-gl-dev';

import BaseView from 'views/base';
// import MapBaseView from 'views/mapBase';

var dmapPolygon = null,
	dmapBounds = null,
	gtuData = null,
	gtuPoints = [];
const TAG = '[GTU-EDIT]';

export var MapContainer = React.createClass({
	mixins: [BaseView],
	shouldComponentUpdate: function () {
		return false;
	},
	componentWillUnmount: function () {
		if (this.state.map) {
			this.state.map.off('click');
			this.state.map.remove();
		}
	},
	componentDidMount: function () {
		var self = this;
		this.subscribe('Map.Draw.CustomGTU', latlng => {
			self.drawGtu();
			self.drawCustomGtu();
		});

		this.subscribe('Map.Draw.Recenter', latlng => {
			self.recenter();
		});
	},
	onInit: function (mapContainer) {
		if (mapContainer == null) {
			return;
		}
		var self = this;
		mapboxgl.accessToken = MapboxToken;
		var monitorMap = new mapboxgl.Map({
			container: mapContainer,
			zoom: 8,
			maxZoom: 20,
			center: [-73.987378, 40.744556],
			style: 'http://timm.vargainc.com/map/street.json',
		});
		// self.registerTopic(monitorMap);
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
			self.registerTopic(monitorMap);
			self.setState({
				map: monitorMap,
				mapContainer: mapContainer
			}, self.initMapLayer);
		});
	},
	registerTopic: function (monitorMap) {
		let self = this;
		monitorMap.on('click', function (e) {
			let gtuLayer = monitorMap.queryRenderedFeatures(e.point, {
				layers: ['gtu-new-layer']
			});
			if (gtuLayer && gtuLayer.length > 0) {
				let layer = first(gtuLayer);
				console.log(layer.properties.clientId);
				self.publish('Map.GTU.Remove', layer.properties.clientId);

				return;
			}

			let boundaryLayer = monitorMap.queryRenderedFeatures(e.point, {
				layers: ['boundary-dmap-fill-layer']
			});
			if (boundaryLayer && boundaryLayer.length > 0) {
				self.publish('Map.GTU.New', e.lngLat);
			}
		});
	},
	initMapLayer: function () {
		var monitorMap = this.state.map;
		var self = this;
		this.drawBoundary();
		this.drawGtu();

		this.subscribe('Global.Window.Resize', size => {
			if (self.state.mapContainer) {
				$('.mapbox-wrapper').width(`${size.width}px`);
				let mapHeight = size.height - $('.title-bar').outerHeight() - $('.section').outerHeight() - $('.map-toolbar').outerHeight() - 10;
				$('.mapbox-wrapper').height(`${mapHeight}px`);
				monitorMap.resize();
			}
		});

		$(window).trigger('resize');
	},
	drawBoundary: function () {
		var self = this;
		var monitorMap = this.state.map;
		if (!monitorMap) {
			console.error('map is not ready');
			return;
		}
		console.time("draw boundary");
		this.publish('showLoading');

		var boundary = this.props.dmap.get('Boundary');
		var fillColor = this.props.dmap.get('Color');
		var dmapBounds = new mapboxgl.LngLatBounds();
		let geoData = {
			type: 'Feature',
			geometry: {
				type: 'Polygon',
				coordinates: [
					[]
				]
			}
		};
		each(boundary, latlng => {
			geoData.geometry.coordinates[0].push([latlng.lng, latlng.lat]);
			dmapBounds.extend([latlng.lng, latlng.lat]);
		});
		monitorMap.addSource(`boundary-dmap`, {
			type: 'geojson',
			data: {
				"type": "FeatureCollection",
				"features": [geoData]
			}
		});
		monitorMap.addLayer({
			id: `boundary-dmap-fill-layer`,
			type: 'fill',
			source: `boundary-dmap`,
			layout: {},
			paint: {
				'fill-opacity': 0,
				'fill-color': 'rgb(0, 0, 0)'
			}
		});
		monitorMap.addLayer({
			id: `boundary-dmap-layer`,
			type: 'line',
			source: `boundary-dmap`,
			layout: {
				'line-join': 'round',
			},
			paint: {
				'line-color': 'rgb(0, 0, 0)',
				'line-width': {
					stops: [
						[10, 1],
						[20, 4]
					]
				}
			}
		});
		monitorMap.fitBounds(dmapBounds);
		this.setState({
			dmapBounds: dmapBounds
		}, () => {
			self.publish('hideLoading');
			console.timeEnd("draw boundary");
		});
	},
	recenter: function () {
		if (!this.state.map || !this.state.dmapBounds) {
			return;
		}
		var mapZoom = this.state.map.getZoom();
		this.state.map.flyTo({
			center: this.state.dmapBounds.getCenter(),
			zoom: mapZoom
		});
	},
	drawGtu: function () {
		let monitorMap = this.state.map;
		if (!monitorMap) {
			console.error('map is not ready');
			return;
		}
		console.time("draw gtu");
		let self = this;

		let gtus = this.props.dmap.get('Gtu') || [];
		let geoJson = {
			type: 'geojson',
			data: {
				"type": "FeatureCollection",
				"features": []
			}
		};
		each(gtus, function (colorGtu) {
			var color = colorGtu.color;
			each(colorGtu.points, function (gtu) {
				if (gtu.out) {
					return true;
				}
				geoJson.data.features.push({
					type: 'Feature',
					properties: {
						cellId: gtu.cellId,
						userColor: color,
						gtuId: colorGtu.gtuId,
					},
					geometry: {
						type: 'Point',
						coordinates: [gtu.lng, gtu.lat]
					}
				});
			});
		});

		let gtuSource = monitorMap.getSource('gtu-source');
		if (!gtuSource) {
			monitorMap.addSource(`gtu-source`, geoJson);

			monitorMap.addLayer({
				'id': `gut-layer`,
				'type': 'circle',
				'source': `gtu-source`,
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
			gtuSource.setData(geoJson.data);
		}

		console.timeEnd("draw gtu");
	},
	drawCustomGtu: function () {
		let monitorMap = this.state.map;
		if (!monitorMap) {
			console.error('map is not ready');
			return;
		}
		let source = monitorMap.getSource(`gtu-new-source`);
		if (source == null) {
			monitorMap.addSource(`gtu-new-source`, {
				type: 'geojson',
				data: {
					"type": "FeatureCollection",
					"features": map(this.props.customPoints, point => {
						return {
							type: 'Feature',
							properties: {
								clientId: point.ClientId,
								userColor: point.Color,
								gtuId: point.GtuId,
							},
							geometry: {
								type: 'Point',
								coordinates: [point.Location.Longitude, point.Location.Latitude]
							}
						};
					})
				}
			});
			monitorMap.addLayer({
				'id': `gtu-new-layer`,
				'type': 'circle',
				'source': `gtu-new-source`,
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
					'circle-stroke-width': 4,
					'circle-stroke-color': '#000000'
				}
			});
		} else {
			source.setData({
				"type": "FeatureCollection",
				"features": map(this.props.customPoints, point => {
					return {
						type: 'Feature',
						properties: {
							clientId: point.ClientId,
							userColor: point.Color,
							gtuId: point.GtuId,
						},
						geometry: {
							type: 'Point',
							coordinates: [point.Location.Longitude, point.Location.Latitude]
						}
					};
				})
			});
		}
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
				</div>
			</div>
		);
	}
});

export default React.createBackboneClass({
	mixins: [
		BaseView,
		// MapBaseView
	],
	componentDidMount: function () {
		var self = this;
		this.subscribe('Map.GTU.New', latlng => {
			self.onNewGtu.call(self, latlng);
		});
		this.subscribe('Map.GTU.Remove', clientId => {
			self.onRemoveGtu.call(self, clientId);
		});
	},
	getInitialState: function () {
		return {
			disableDefaultUI: false,
			scrollwheel: false,
			disableDoubleClickZoom: true,
			activeGtu: null,
			customPoints: []
		};
	},
	setActiveGtu: function (gtuId) {
		let self = this;
		let activeGut = this.props.gtu.get(gtuId);
		this.setState({
			activeGtu: activeGut
		});
	},
	onNewGtu: function (latlng) {
		let activeGtu = this.state.activeGtu;
		if (activeGtu) {
			let customPoints = this.state.customPoints;
			customPoints.push({
				ClientId: uniqueId(),
				TaskId: this.props.task.get('Id'),
				GtuId: activeGtu.get('Id'),
				Date: moment().format('YYYY-MM-DD HH:mm:ss'),
				Color: activeGtu.get('UserColor'),
				Location: {
					Latitude: latlng.lat,
					Longitude: latlng.lng
				}
			});
			this.setState({
				customPoints: customPoints
			}, () => {
				this.publish('Map.Draw.CustomGTU');
			});
		}
	},
	onRemoveGtu: function (clientId) {
		if (!clientId) {
			return;
		}
		let self = this;
		let customPoints = filter(this.state.customPoints, gtu => {
			return gtu.ClientId != clientId
		});
		this.setState({
			customPoints: customPoints
		}, () => {
			self.publish('Map.Draw.CustomGTU');
		});
	},
	onUnderGtu: function () {
		let self = this;
		let customPoints = dropRight(this.state.customPoints);
		this.setState({
			customPoints: customPoints
		}, () => {
			self.publish('Map.Draw.CustomGTU');
		});
	},
	onSaveGtu: function () {
		let self = this;
		let newGtuDots = map(this.state.customPoints, p => {
			return {
				GtuId: p.GtuId,
				Date: p.Date,
				Location: p.Location
			}
		});
		Promise.all([
			this.props.task.addGtuDots(newGtuDots),
			// this.props.task.removeGtuDots(putData)
		]).then(() => {
			return self.props.dmap.fetchGtuForEdit()
		}).then(function () {
			self.setState({
				customPoints: []
			}, () => {
				self.publish('Map.Draw.CustomGTU');
			});
		});
	},
	onReCenter: function () {
		this.publish('Map.Draw.Recenter');
	},
	render: function () {
		let self = this;
		let gtuList = this.props.gtu.models || [];
		let newAddedPoints = filter(this.state.customPoints, function (i) {
			return i.status == 'new' || i.status == 'deleted';
		});
		let canUndoSave = this.state.customPoints.length > 0;
		if (!canUndoSave) {
			var undoButton = <button className="button float-right" disabled onClick={this.onUnderGtu}><i className="fa fa-undo"></i>Undo</button>;
			var saveButton = <button className="button float-right" disabled onClick={this.onSaveGtu}><i className="fa fa-save"></i>Save</button>;
		} else {
			var undoButton = <button className="button float-right" onClick={this.onUnderGtu}><i className="fa fa-undo"></i>Undo</button>;
			var saveButton = <button className="button float-right" onClick={this.onSaveGtu}><i className="fa fa-save"></i>Save</button>;
		}

		return (
			<div className="edit-gtu">
				<div className="section row">
					<div className="small-12 columns">
						<div className="section-header">
							<div className="row">
								<div className="small-12 column">
									<h5>Edit GTU - {this.props.task.get('Name')}</h5>
								</div>
								<div className="small-12 column">
									<nav aria-label="You are here:" role="navigation">
										<ul className="breadcrumbs">
											<li><a href="#">Control Center</a></li>
											<li><a href={'#report/' + this.props.task.get('Id')}>Report</a></li>
											<li>
												<span className="show-for-sr">Current: </span> Edit GTU
											</li>
										</ul>
									</nav>
								</div>
							</div>
						</div>
					</div>
					<div className="small-12 medium-7 large-9 columns">
						{gtuList.map(function(gtu) {
							return (
								<button key={gtu.get('Id')} className={self.state.activeGtu == gtu ? " button" : "button"} onClick={self.setActiveGtu.bind(null, gtu.get('Id'))}>
									<i className={self.state.activeGtu == gtu ? "fa fa-map-marker" : "fa fa-stop"} style={{color: gtu.get('UserColor')}}></i>
									{gtu.get('ShortUniqueID')}
								</button>
							);
						})}
					</div>
					<div className="small-12 medium-5 large-3 columns">
						{saveButton}
						{undoButton}
						<button className="button float-right" onClick={this.onReCenter}><i className="fa fa-refresh"></i>ReCenter</button>
					</div>				
				</div>
				<MapContainer dmap={this.props.dmap} customPoints={this.state.customPoints} />
			</div>
		);
	}
});