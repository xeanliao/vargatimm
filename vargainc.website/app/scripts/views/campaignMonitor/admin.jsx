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

import ImportFiles from './importFiles';
import MapContainer from './mapContainer';

const TAG = '[CAMPAIGN-MONITOR]';
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
				targetTask.mergeFrom(sourceTask.get('Id')).then(() => {
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
	initSelect2: function(el){
		var self = this;
		if(el){
			$(el).select2();
			$(document).on('select2:select', e=>{
				let taskId = get(e, 'params.data.id');
				let model = self.getModel();
				let tasks = model.get('Tasks');
				if(tasks.get){
					let task = tasks.get(taskId);
					self.onSwitchActiveTask.call(self, task);	
				}
			});
		}
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
		var gtu = this.state.activeTask.get('gtuList'),
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
	onShowUrlForDriver: function(){
		let model = this.getModel();
		let campaignId = model.get('Id');

		this.alert(`http://${window.location.host}${window.location.pathname}#driver/${campaignId}`);
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
				<div className="section row gtu-monitor expanded">
					<div className="small-12 columns">
						<div className="row expanded">
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
				<div className="row gtu expanded">
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
					
					<span>{gtu.get('ShortUniqueID')}</span>
					{alertIcon}
				</button>
				<button className="button location text-center" onClick={this.onGotoGTU.bind(null, gtu.get('Id'))} style={{'backgroundColor': gtu.get('UserColor')}}>
					{gtuIcon}
				</button>
			</span>
		);
	},
	renderTaskController: function (task) {
		let finishClass = classNames('button', {
			hide: task.get('IsFinished')
		})
		switch (task.get('Status')) {
		case 0: //started
			return (
				<div>
					<button className="button" onClick={this.onPause.bind(this, task)}><i className="fa fa-pause"></i>Pause</button>
					<button className="button" onClick={this.onStop.bind(this, task)}><i className="fa fa-stop"></i>Stop</button>
					<button className={finishClass} onClick={this.onFinished.bind(this, task.get('Id'))}><i className="fa fa-check"></i>Finish</button>
				</div>
			);
			break;
		case 1: //stoped
			return (
				<div>
					<button className={finishClass} onClick={this.onFinished.bind(this, task.get('Id'))}><i className="fa fa-check"></i>Finish</button>
				</div>
			);
			break;
		case 2: //peased
			return (
				<div>
					<button className="button" onClick={this.onStart.bind(this, task)}><i className="fa fa-play"></i>Start</button>
					<button className="button" onClick={this.onStop.bind(this, task)}><i className="fa fa-stop"></i>Stop</button>
					<button className={finishClass} onClick={this.onFinished.bind(this, task.get('Id'))}><i className="fa fa-check"></i>Finish</button>
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
						<li>
							<a href="javascript:;" onClick={this.onSwitchDisplayMode.bind(this, 'cover')}>
								<i className="fa fa-map"></i>
								&nbsp;<span>Show Coverage</span>
							</a>
						</li>
						<li>
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
						<li>
							<a href="javascript:;" onClick={this.onShowUrlForDriver}>
								<i className='fa fa-external-link-square'></i>
								&nbsp;<span>URL for driver</span>
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