define([
	'jquery',
	'backbone',
	'react',
	'views/base',

	'react.backbone',
	'select2',
	'spectrum'
], function ($, Backbone, React, BaseView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function(){
			return {editId: null};
		},
		componentDidMount: function(){
			var self = this,
				model = this.getModel();
			
			$('form').foundation();
		},
		componentWillUpdate: function(newProps, newState){
			if(this.state.editId != null && newState.editId != this.state.editId && this.refs['userSelector-' + this.state.editId]){
				console.log('destory');
				$(this.refs['userSelector-' + this.state.editId]).select2('destroy');
			}
		},
		componentDidUpdate: function(prevProps, prevState){
			if(this.state.editId != null && this.state.editId != prevState.editId && this.refs['userSelector-' + this.state.editId]){
				console.log('init');
				$(this.refs['userSelector-' + this.state.editId]).select2();
			}
		},
		onClose: function(){
			this.publish("showDialog");
		},
		onAdd: function(gtuId){
			this.setState({editId: gtuId});
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
		onCacnel: function(){
			this.setState({editId: null});
		},
		onRemove: function (gtuId) {
			var result = confirm('Are you sure you want to remove the assignment from GTU and Employee?');
			if (result) {
				var taskId = this.props.taskId,
					collection = this.getCollection(),
					model = collection.get(gtuId);
				model.unassignGTUFromTask(taskId, gtuId);
			}
		},
		renderEditForm: function(gtu){
			var user = _.groupBy(this.props.user.models, function (item) {
					return item.get('CompanyId');
				}),
				self = this;
			return (
				<tr key={gtu.get('Id')}>
					<td>{gtu.get('ShortUniqueID')}</td>
					<td><input ref="userColor" type="color" autocomplete defaultValue={'#'+Math.floor(Math.random()*16777215).toString(16)} /></td>
					<td colSpan="2">
						<select ref={'userSelector-' + gtu.get('Id')}>
							{_.map(user, function(v, k){
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
					<td>{gtu.get('IsOnline') ? 'Online' : 'Offline'}</td>
					<td>
						<button className="button tiny" onClick={self.onSave.bind(self, gtu.get('Id'))}>Save</button>
						<button className="button tiny alert" onClick={self.onCacnel}>Cancel</button>
					</td>
				</tr>
			);
		},
		render: function(){
			var collection = this.getCollection(),
				showError = false,
				errorMessage = '',
				self = this;
			return (
				<div>
					<h3>Assign GTU</h3>
					<div data-abide-error className="alert callout" style={{display: showError ? 'block' : 'none'}}>
				    	<p><i className="fa fa-exclamation-circle"></i>&nbsp;{errorMessage}</p>
				  	</div>
					<table>
						<colgroup>
							<col style={{"width": "160px"}} />
							<col style={{"width": "160px"}} />
							<col />
							<col style={{"width": "160px"}} />
							<col style={{"width": "160px"}} />
							<col style={{"width": "150px"}} />
						</colgroup>
						<thead>
							<tr>
								<th>GTU#</th>
								<th>Color</th>
								<th>
									<div className="row">
										<div className="small-6 column">Company</div>
										<div className="small-6 column">Auditor</div>
									</div>
								</th>
								<th>Role</th>
								<th>Status</th>
								<th></th>
							</tr>
						</thead>
						<tbody>
							{collection.models.map(function(gtu){
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
											<td>{gtu.get('ShortUniqueID')}</td>
											<td>{colorInput}</td>
											<td>
												<div className="row">
													<div className="small-6 column">{gtu.get('Company')}</div>
													<div className="small-6 column">{gtu.get('Auditor')}</div>
												</div>
											</td>
											<td>{gtu.get('Role')}</td>
											<td>{gtu.get('IsOnline') ? 'Online' : 'Offline'}</td>
											<td>
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
});