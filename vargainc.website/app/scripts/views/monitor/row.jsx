import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import $ from 'jquery';
import moment from 'moment';
import {
	each,
} from 'lodash';

import TaskModel from 'models/task';
import BaseView from 'views/base';
import DismissView from 'views/monitor/dismiss';
import EditView from 'views/monitor/edit';

export default React.createBackboneClass({
	mixins: [BaseView],
	menuKey: 'monitor-menu-ddl-',
	getDefaultProps: function () {
		return {
			address: null,
			icon: null,
			name: null
		};
	},
	componentDidMount: function () {
		var menuKey = this.menuKey;
		each(this.getModel().get('Tasks'), function (task) {
			$("#" + menuKey + task.Id).foundation();
		});
	},
	onDismiss: function (e) {
		e.preventDefault();
		e.stopPropagation();
		var self = this,
			model = this.getModel();
		this.confirm('Are you sure you want to delete all selected Tasks?').then(() => {
			self.publish('showDialog', DismissView);
			self.unsubscribe('monitor/dismiss');
			self.subscribe('monitor/dismiss', function (user) {
				model.dismissToDMap(user, {
					success: function (result) {
						if (result && result.success) {
							self.publish('monitor/refresh');
						} else {
							self.alert("something wrong");
						}
					},
					complete: function () {
						self.publish('showDialog');
					}
				});
			});
		});
	},
	onFinished: function (taskId, reactEvent, reactNumber, evt) {
		evt.preventDefault();
		evt.stopPropagation();

		var self = this,
			model = new TaskModel({
				Id: taskId
			});
		model.markFinished({
			success: function (result) {
				if (result && result.success) {
					self.publish('monitor/refresh');
				} else {
					self.alert(result.error);
				}
			}
		});
	},
	onOpenUploadFile: function (taskId) {
		$("#upload-file-" + taskId).click();
	},
	onImport: function (taskId, e) {
		$(e.currentTarget).closest('.dropdown-pane').foundation('close');
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

		model.importGtu(uploadFile, {
			success: function (result) {
				$("#upload-file-" + taskId).val('');
				if (result && result.success) {
					self.publish('monitor/refresh');
				}
				if (result && result.error && result.error.length > 0) {
					self.alert(result.error.join('\r\n'));
				}
			}
		});
	},
	onEdit: function (taskId, e) {
		e.preventDefault();
		e.stopPropagation();
		$(e.currentTarget).closest('.dropdown-pane').foundation('close');

		var model = new TaskModel({
				Id: taskId
			}),
			self = this;

		model.fetch().then(function () {
			self.publish('showDialog', {
				view: EditView,
				params: {
					model: model,
				}
			});
		});
	},
	onGotoMonitor: function (campaignId, taskName, taskId) {
		window.open(`./#campaign/${campaignId}/${taskName}/${taskId}/monitor`);
	},
	onOpenMoreMenu: function (e) {
		e.preventDefault();
		e.stopPropagation();
	},
	onCloseMoreMenu: function (key) {
		$(`#${this.menuKey}${key}`).foundation('close');
	},
	renderMoreMenu: function (key) {
		var id = this.menuKey + key;
		return (
			<span>
				<button className="button cirle" data-toggle={id} onClick={this.onOpenMoreMenu}>
					<i className="fa fa-ellipsis-h"></i>
				</button>
				<div id={id} className="dropdown-pane bottom" 
					data-dropdown
					data-close-on-click="true" 
					data-auto-focus="false"
					onClick={this.onCloseMoreMenu.bind(null, key)}>
					<ul className="vertical menu">
						<li><a href="javascript:;" onClick={this.onEdit.bind(null, key)}><i className="fa fa-edit"></i><span>Edit</span></a></li>
						<li><a href="javascript:;" onClick={this.onOpenUploadFile.bind(null, key)}><i className="fa fa-cloud-upload"></i><span>Import</span></a></li>
						<input type="file" id={'upload-file-' + key} multiple style={{'display':'none'}} onChange={this.onImport.bind(null, key)} />
					</ul>
				</div>
			</span>
		);
	},
	onCompanyMonitor: function (campaignId) {
		window.open(`./#campaign/${campaignId}/monitor`);
	},
	render: function () {
		var model = this.getModel(),
			date = model.get('Date'),
			displayDate = date ? moment(date).format("MMM DD, YYYY") : '',
			self = this;
		return (
			<div className="row scroll-list-item">
				<div className="small-12 columns" onClick={this.onCompanyMonitor.bind(this, model.get('Id'))}>
					<div className="hide-for-small-only medium-2 columns">
						{model.get('ClientName')}
					</div>
					<div className="small-12 medium-5 columns">
						{model.get('ClientCode')}
					</div>
					<div className="hide-for-small-only medium-2 columns">
						{displayDate}
					</div>
					<div className="small-12 medium-3 columns">
						<span className="show-for-large">{model.get('AreaDescription')}</span>
						<div className="float-right tool-bar">
							<a onClick={this.onDismiss} className="button row-button">
								<i className="fa fa-reply"></i><small>Dismiss</small>
							</a>
						</div>
					</div>
				</div>
			</div>
		);
	}
});