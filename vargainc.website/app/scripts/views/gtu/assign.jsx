import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import $ from 'jquery';
import 'select2';
import 'spectrum-colorpicker';
import {
	groupBy,
	map
} from 'lodash';

import BaseView from 'views/base';

export default React.createBackboneClass({
	mixins: [BaseView],
	getInitialState: function () {
		return {
			editId: null
		};
	},
	componentDidMount: function () {
		var self = this,
			model = this.getModel();

		$('form').foundation();
	},
	componentWillUpdate: function (newProps, newState) {
		if (this.state.editId != null && newState.editId != this.state.editId && this.refs['userSelector-' + this.state.editId]) {
			$(this.refs['userSelector-' + this.state.editId]).select2('destroy');
		}
	},
	componentDidUpdate: function (prevProps, prevState) {
		if (this.state.editId != null && this.state.editId != prevState.editId && this.refs['userSelector-' + this.state.editId]) {
			$(this.refs['userSelector-' + this.state.editId]).select2();
		}
	},
	onClose: function () {
		this.publish("showDialog");
	},
	onAdd: function (gtuId) {
		this.setState({
			editId: gtuId
		});
	},
	onSave: function (gtuId) {
		var taskId = this.props.taskId,
			collection = this.getCollection(),
			model = collection.get(gtuId),
			color = $(this.refs['userColor']).val(),
			user = $(this.refs['userSelector-' + gtuId]).val(),
			self = this;
		model.assignGTUToTask({
			Id: gtuId,
			TaskId: taskId,
			UserColor: color,
			AuditorId: user
		}).done(function () {
			self.setState({
				editId: null
			});
		});
	},
	onCacnel: function () {
		this.setState({
			editId: null
		});
	},
	onRemove: function (gtuId) {
		let msg = 'Are you sure you want to remove the assignment from GTU and Employee?';
		this.confirm(msg).then(() => {
			var taskId = this.props.taskId,
				collection = this.getCollection(),
				model = collection.get(gtuId);
			model.unassignGTUFromTask(taskId, gtuId);
		});
	},
	renderEditForm: function (gtu) {
		var self = this,
			user = groupBy(this.props.user.models, function (item) {
				return item.get('CompanyId');
			});
		return (
			<tr key={gtu.get('Id')}>
					<td className="text-center">{gtu.get('BagId')}</td>
					<td className="text-center">{gtu.get('ShortUniqueID')}</td>
					<td className="text-center">
						<input ref="userColor" className="color-block" type="color" autocomplete defaultValue={'#'+Math.floor(Math.random()*16777215).toString(16)} />
					</td>
					<td colSpan="2">
						<select ref={'userSelector-' + gtu.get('Id')}>
							{map(user, function(v, k){
								return (
									<optgroup key={k} label={v[0].get('CompanyName')}>
										{v.map(function(u){
											return <option key={u.get('UserId')} value={u.get('UserId')}>{u.get('UserName')} - {u.get('Role')}</option>
										})}
									</optgroup>
								);
							})}
						</select>
					</td>
					<td className="text-center">{gtu.get('IsOnline') ? 'Online' : 'Offline'}</td>
					<td className="text-center">
						<button className="button tiny" onClick={self.onSave.bind(self, gtu.get('Id'))}>Save</button>
						<button className="button tiny alert" onClick={self.onCacnel}>Cancel</button>
					</td>
				</tr>
		);
	},
	render: function () {
		var collection = this.getCollection() || [],
			showError = false,
			errorMessage = '',
			self = this;
		if (collection && collection.where) {
			collection = collection.where(function (i) {
				return i.get('IsAssignedToOther') == false;
			});
		}

		return (
			<div>
				<h3>Assign GTU</h3>
				<div data-abide-error className="alert callout" style={{display: showError ? 'block' : 'none'}}>
			    	<p><i className="fa fa-exclamation-circle"></i>&nbsp;{errorMessage}</p>
			  	</div>
				<table>
					<colgroup>
						<col style={{"width": "80px"}} />
						<col style={{"width": "160px"}} />
						<col style={{"width": "160px"}} />
						<col />
						<col style={{"width": "160px"}} />
						<col style={{"width": "160px"}} />
						<col style={{"width": "150px"}} />
					</colgroup>
					<thead>
						<tr>
							<th className="text-center">Bag#</th>
							<th className="text-center">GTU#</th>
							<th className="text-center">Color</th>
							<th className="text-center">
								<div className="row">
									<div className="small-6 column">Team</div>
									<div className="small-6 column">Auditor</div>
								</div>
							</th>
							<th className="text-center">Role</th>
							<th className="text-center">Status</th>
							<th className="text-center"></th>
						</tr>
					</thead>
					<tbody>
						{collection.map(function(gtu){
							var isAssign = gtu.get('IsAssign'),
								gtuId = gtu.get('Id'),
								assignButton = <button className="button tiny" onClick={self.onAdd.bind(null, gtuId)}><i className="fa fa-plus"></i>&nbsp;Assign</button>,
								removeButton = <button className="button alert tiny" onClick={self.onRemove.bind(null, gtuId)}><i className="fa fa-remove"></i>&nbsp;Remove</button>,
								colorInput = gtu.get('UserColor') ? <div className="color-block" style={{background:gtu.get('UserColor')}}></div> : null;
							var actionButton = isAssign ? removeButton : assignButton;
							if (gtu.get('Id') == self.state.editId) {
								return self.renderEditForm(gtu);
							} else {
								return(
									<tr key={gtu.get('Id')}>
										<td className="text-center">{gtu.get('BagId')}</td>
										<td className="text-center">{gtu.get('ShortUniqueID')}</td>
										<td className="text-center">{colorInput}</td>
										<td>
											<div className="row">
												<div className="small-6 column">{gtu.get('Company')}</div>
												<div className="small-6 column">{gtu.get('Auditor')}</div>
											</div>
										</td>
										<td className="text-center">{gtu.get('Role')}</td>
										<td className="text-center">{gtu.get('IsOnline') ? 'Online' : 'Offline'}</td>
										<td className="text-center">
											{actionButton}
										</td>
									</tr>
								);
							}
						})}
					</tbody>
				</table>
				<button onClick={this.onClose} className="close-button" data-close aria-label="Close reveal" type="button">
			    	<span aria-hidden="true">&times;</span>
			  	</button>
			</div>
		);
	}
});