import Backbone from 'backbone';
import React from 'react';
import ReactDOMServer from 'react-dom/server';
import 'react.backbone';
import Promise from 'bluebird';
import L from 'leaflet';
import 'lib/leaflet-google';
import 'leaflet-dvf';
import 'leaflet.polyline.snakeanim';
import 'leaflet-ant-path';
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
	filter
} from 'lodash';

import GTUCollection from 'collections/gtu';
import DMap from 'models/print/dmap';
import GtuCollection from 'collections/gtu';
import UserCollection from 'collections/user';
import UserModel from 'models/user';

import BaseView from 'views/base';
import AssignView from 'views/gtu/assign';
import EmployeeView from 'views/user/employee';

var MapContainer = React.createBackboneClass({
	mixins: [BaseView],
	shouldComponentUpdate: function () {
		return false;
	},
	getInitialState: function () {
		return {
			gtuMarkerLayer: {},
			gtuTranckLayer: {},
			showOutOfBoundaryGtu: false
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
		});
		new L.googleTile({
			mapTypeId: google.maps.MapTypeId.HYBRID
		}).addTo(monitorMap);

		this.setState({
			map: monitorMap
		}, () => {
			self.drawBoundary(monitorMap);
		});
		this.subscribe('Campaign.Monitor.DrawActiveGtu', data => {
			self.setState(data, () => {
				clearInterval(self.state.drawActiveTaskTimeout);
				self.drawGtu();
				self.state.drawActiveTaskTimeout = window.setInterval(self.reloadData, 60 * 1000);
			});
		});
		this.subscribe('Campaign.Monitor.ZoomToTask', task => {
			self.flyToTask(task);
		});
	},
	drawBoundary: function (monitorMap) {
		var self = this,
			model = this.getModel(),
			campaignId = model.get('Id'),
			tasks = model.get('Tasks'),
			submapTasks = groupBy(tasks.models, t => {
				return t.get('SubMapId')
			}),
			promiseSubmapArray = [],
			promiseTaskArray = [],
			boundaryLayerGroup = L.layerGroup(),
			boundaryBounds = L.latLngBounds();

		each(submapTasks, (groupTasks, submapId) => {
			promiseSubmapArray.push($.getJSON(`../api/print/campaign/${campaignId}/submap/${submapId}/boundary`));
		});
		return Promise.all(promiseSubmapArray).then(data => {
			return Promise.each(data, result => {
				let latlngs = map(result.boundary, i => {
					boundaryBounds.extend(i);
					return [i.lat, i.lng];
				});
				boundaryLayerGroup.addLayer(L.polyline(latlngs, {
					color: `rgb(${result.color.r}, ${result.color.g}, ${result.color.b})`,
					weight: 3,
					noClip: true,
					dropShadow: true
				}));
				return Promise.resolve();
			});
		}).then(() => {
			each(submapTasks, groupTasks => {
				each(groupTasks, task => {
					promiseTaskArray.push(new Promise((resolve, reject) => {
						let url = `../api/print/campaign/${task.get('CampaignId')}/submap/${task.get('SubMapId')}/dmap/${task.get('DMapId')}/boundary`;
						$.getJSON(url).then(result => {
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
								fillColor: task.get('Status') != 1 ? color : '#000',
								fillOpacity: task.get('Status') != 1 ? 0.1 : 0.75,
								noClip: true,
								clickable: task.get('Status') != 1,
								dropShadow: task.get('Status') != 1,
								fillPattern: task.get('Status') != 1 ? null : {
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
								// .addTo(boundaryLayerGroup)
								.on('click', self.onTaskAreaClickHandler, self);
							boundary.getCenter = function () {
								return {
									lat: center.getY(),
									lng: center.getX()
								};
							};
							boundaryLayerGroup.addLayer(boundary);
							// task.set('boundary', boundary);
							resolve();
						});
					}));
				});
			});

			return Promise.all(promiseTaskArray);
		}).then(() => {
			boundaryLayerGroup.addTo(monitorMap);
			monitorMap.flyToBounds(boundaryBounds);
			self.setState({
				boundaryLayerGroup: boundaryLayerGroup
			})
		});
	},
	buildGtuInPopup: function (task, gtu) {
		var typeIcon = null,
			alertIcon = null,
			deleteIcon = null,
			buttonClass = 'button text-left',
			taskIsStopped = task.get('Status') == 1,
			gtuIcon = null;

		if (taskIsStopped) {
			gtuIcon = <i className="fa fa-stop"></i>
		} else {
			// switch (gtu.get('Role')) {
			switch ('Walker') {
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
		buttonClass = classNames({
			'offline': !taskIsStopped && !gtu.get('IsOnline')
		});
		return (
			<div className="columns">
				{gtuIcon}&nbsp;{gtu.get('ShortUniqueID')}
			</div>
		);
	},
	buildTaskPopup: function (task, gtus) {
		var self = this;
		var taskIsStopped = task.get('Status') == 1;
		var gtuList = gtus.where(function (i) {
			if (taskIsStopped) {
				return i.get('WithData')
			} else {
				return i.get('IsAssign') || i.get('WithData');
			}
		});
		var taskButton = [<span key={'finished'}>FINISHED</span>];
		if (!taskIsStopped) {
			let startBtn = <button key={'btn-start'} className="button tiny"><i className="fa fa-play"></i>&nbsp;Start</button>;
			let stopBtn = <button key={'btn-stop'} className="button tiny"><i className="fa fa-stop"></i>&nbsp;Stop</button>;
			let pauseBtn = <button key={'btn-pause'} className="button tiny"><i className="fa fa-pause"></i>&nbsp;Pause</button>;
			switch (task.get('TaskTimeType')) {
			case 0:
				taskButton = [pauseBtn, stopBtn];
				break;
			case 1:
				taskButton = [startBtn]
				break;
			case 2:
				taskButton = [startBtn, stopBtn]
				break;
			default:
				taskButton = [startBtn];
				break;
			}
		}
		var templat = (
			<div className="row section" style={{width: '480px'}}>
				<div className="small-7 columns">{task.get('Name')}</div>
				<div className="small-5 columns text-right">{taskButton}</div>
				<div className="small-12 columns">
					<div className="row small-up-4">
						{map(gtuList.models, gtu=>{
							return self.buildGtuInPopup(task, gtu);
						})}
					</div>
				</div>
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
			taskIsStopped = task.get('Status') == 1,
			gtus = new GTUCollection();
		gtus.fetchGtuWithStatusByTask(taskId).then(() => {
			let popContent = self.buildTaskPopup(task, gtus);
			L.popup({
					maxWidth: 640
				}).setLatLng(taskAreaCenter)
				.setContent(popContent)
				.openOn(this.state.map);
		});
	},
	reloadData: function () {},
	drawGtuMarker: function (gtuId, data) {
		var self = this;
		var map = this.state.map;
		if (!map) {
			return null;
		}
		var dataLayer = this.state.gtuMarkerLayer[gtuId];
		if (!dataLayer) {
			var gtuRadiusFunction = new L.LinearFunction(new L.Point(0, 3), new L.Point(1, 10));
			var dataLayer = new L.MapMarkerDataLayer(data, {
				recordsField: 'points',
				locationMode: L.LocationModes.LATLNG,
				latitudeField: 'lat',
				longitudeField: 'lng',
				tooltipOptions: null,
				getMarker: function (location, options, record) {
					return new L.RegularPolygonMarker(location, options);
				},
				filter: function (value) {
					return self.state.showOutOfBoundaryGtu ? true : value.out == false;
				},
				layerOptions: {
					weight: 0.1,
					opacity: 0.7,
					fillColor: data.color,
					fillOpacity: 1.0,
					fill: true,
					stroke: true,
					numberOfSides: 50,
					stroke: false,
					fillOpacity: 0.75,
					dropShadow: true,
					radius: 5,
					clickable: false,
					showLegendTooltips: false,
					gradient: function () {
						return {
							gradientType: 'radial',
							stops: [{
								offset: '0%',
								style: {
									color: data.color,
									opacity: 1
								}
							}, {
								offset: '30%',
								style: {
									color: data.color,
									opacity: 0.5
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
				},
			});
			map.addLayer(dataLayer);
			this.state.gtuMarkerLayer[gtuId] = dataLayer;
		} else {
			dataLayer.setData(data);
		}
		return dataLayer;
	},
	drawGtuTrack: function (gtuId, data) {
		var trackLine = this.state.gtuTranckLayer[gtuId];
		if (!trackLine) {
			let latlngs = map(data.points, p => {
				return {
					lat: p.lat,
					lng: p.lng
				};
			});
			trackLine = L.polyline(latlngs, {
				weight: 1,
				color: data.color,
				opacity: 0.75,
				noClip: true,
				dropShadow: true,
				snakingSpeed: 200
			});
			trackLine.addTo(this.state.map);
			this.state.gtuTranckLayer[gtuId] = trackLine;
		}
		trackLine.on('snakeend', () => {
			this.options.paused = false;
		});
		trackLine.snakeIn();
	},
	drawGtu: function () {
		var map = this.state.map;
		var self = this;
		var gtuMarkerLayerGroup = L.LayerGroup();
		each(this.state.gtuTrack, data => {
			if (data.points && data.points.length > 0) {
				let gtuId = data.points[0].Id;
				// self.drawGtuMarker(gtuId, data);
				self.drawGtuTrack(gtuId, data);
				return false;
			}
		});
	},
	flyToTask: function (task) {
		var map = this.state.map,
			boundary = task.get('boundary');
		if (!map || !boundary) {
			return;
		}
		var taskBounds = boundary.getBounds()
		map.flyToBounds(taskBounds);
	},
	render: function () {
		return (
			<div className="row">
				<div className="small-12">
					<div className="leaflet-map-container" ref={this.onInit} style={{'minHeight': '640px'}}></div>
				</div>
			</div>
		);
	}
});

export default React.createBackboneClass({
	mixins: [BaseView],
	getInitialState: function () {
		return {
			activeTask: null
		}
	},
	componentDidMount: function () {
		var self = this;
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
		var gtuTrackInDMap = new DMap({
			CampaignId: task.get('CampaignId'),
			SubMapId: task.get('SubMapId'),
			DMapId: task.get('DMapId'),
			Gtu: []
		});
		var trackQuery = gtuTrackInDMap.updateGtuAfterTime(null, {
			quite: true
		});

		var gtuLocation = new GtuCollection();
		var locationQuery = gtuLocation.fetchGtuWithStatusByTask(task.get('Id'), {
			quite: true
		}).then(() => {
			return gtuLocation.fetchGtuLocation(task.get('Id'), {
				quite: true
			});
		});

		return Promise.all([trackQuery, locationQuery]).then(() => {
			return new Promise((resolve, reject) => {
				resolve({
					gtuTrack: gtuTrackInDMap.get('Gtu'),
				});
			});
		}).then(data => {
			task.set('track', data.gtuTrack);
			self.setState({
				activeTask: task
			}, () => {
				self.publish('Campaign.Monitor.DrawActiveGtu', data);
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
	onSwitchDisplayMode: function () {
		var self = this;
		var displayMode = this.state.displayMode == 'cover' ? 'track' : 'cover';
		this.setState({
			displayMode: displayMode
		}, () => {
			self.publish('Map.Monitor.GTU.DisplayMode', displayMode);
		});
	},
	onSwitchShowOutOfBoundaryGtu: function () {
		var showOutOfBoundaryGtu = !this.state.ShowOutOfBoundaryGtu;
		this.setState({
			ShowOutOfBoundaryGtu: showOutOfBoundaryGtu,
		}, () => {
			self.publish('Map.Monitor.GTU.ShowOutOfBoundaryGtu', showOutOfBoundaryGtu);
		});
	},
	onAddEmployee: function () {
		var user = new UserCollection(),
			self = this;
		user.fetchCompany().done(function () {
			self.publish('showDialog', EmployeeView, {
				model: new UserModel(),
				company: user
			});
		});
	},
	onAssign: function () {
		var gtu = this.state.activeTask.get('gtu'),
			taskId = this.state.activeTask.get('Id'),
			user = new UserCollection(),
			self = this;
		user.fetchForGtu().done(function () {
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
		this.publish('', this.state.activeTask.get('Id'));
	},
	onReload: function () {
		this.onSwitchActiveTask(this.state.activeTask);
	},
	renderTaskDropdown: function () {
		var self = this,
			model = this.getModel(),
			tasks = model && model.get('Tasks') ? model.get('Tasks').models : [];

		tasks = filter(tasks, t => {
			return t.get('Status') != 1;
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
						<div style={{'min-width': '768px'}} className={menuClass} onClick={this.onCloseDropDown}>
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
		var activeTask = this.state.activeTask;
		var gtuList = activeTask.get('gtu');
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
	renderTaskController: function (task) {
		switch (task.get('Status')) {
		case 0: //started
			return (
				<div>
					<button className="button" onClick={this.onPause.bind(this, task)}><i className="fa fa-pause"></i>Pause</button>
					<button className="button" onClick={this.onStop.bind(this, task)}><i className="fa fa-stop"></i>Stop</button>
				</div>
			);
			break;
		case 1: //stoped
			return <h5>STOPPED</h5>;
			break;
		case 2: //peased
			return (
				<div>
					<button className="button" onClick={this.onStart.bind(this, task)}><i className="fa fa-play"></i>Start</button>
					<button className="button" onClick={this.onStop.bind(this, task)}><i className="fa fa-stop"></i>Stop</button>
				</div>
			);
			break;
		default:
			return (
				<div>
					<button className="button" onClick={this.onStart.bind(this, task)}><i className="fa fa-play"></i>Start</button>
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
		return (
			<ul className="dropdown menu float-right">
				<li className={parentClass}>
					<button className="button cirle" data-toggle="monitor-more-menu" onClick={this.onOpenMoreMenu}>
						<i className="fa fa-ellipsis-h"></i>
					</button>
					<ul className={menuClass} onClick={this.onCloseMoreMenu}>
						<li>
							<a href="javascript:;" onClick={this.onAddEmployee}>
								<i className="fa fa-user-plus"></i>
								&nbsp;<span>New Employee</span>
							</a>
						</li>
						<li>
							<a href="javascript:;" onClick={this.onSwitchDisplayMode}>
								<i className={this.state.displayMode == 'cover' ? 'fa fa-map' : 'fa fa-map-o'}></i>
								&nbsp;<span>{this.state.displayMode == 'cover' ? 'Track Path' : 'Show Coverage'}</span>
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
			model = this.getModel();
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
				<MapContainer model={this.getModel()} />
			</div>
		);
	}
});