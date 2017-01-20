import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import BaseView from 'views/base';

export default React.createBackboneClass({
	mixins: [BaseView],
	componentDidMount: function () {
		var self = this,
			model = this.getModel();

		$('#birthdayDatePicker').fdatepicker({
			format: 'yyyy-mm-dd'
		}).on('changeDate', function (e) {
			self.getModel().set('DateOfBirth', e.date);
		});

		$(this.refs.companySelector).select2();

		$('form').foundation();
	},
	componentWillUnmount: function () {
		let ddl = $(this.refs.companySelector);
		if(ddl && ddl.select2){
			ddl.select2('destroy');
		}
		$('#birthdayDatePicker').off('changeDate').fdatepicker('remove');
	},
	onSave: function (e) {
		e.preventDefault();
		e.stopPropagation();
		var model = this.getModel(),
			file = this.refs.employeePicture.files.length > 0 ? this.refs.employeePicture.files[0] : null,
			self = this;
		model.set('CompanyId', $(this.refs.companySelector).val());
		model.addEmployee(file).done(function () {
			self.publish("showDialog");
		});
	},
	onClose: function () {
		this.publish("showDialog");
	},
	onChange: function (e) {
		this.getModel().set(e.currentTarget.name, e.currentTarget.value);
	},
	render: function () {
		var showError = this.state && this.state.error ? true : false;
		var errorMessage = showError ? this.state.error : "";
		return (
			<form data-abide onSubmit={this.onSave}>
				<h3>Add Employee</h3>
				<div data-abide-error className="alert callout" style={{display: showError ? 'block' : 'none'}}>
			    	<p><i className="fa fa-exclamation-circle"></i>&nbsp;{errorMessage}</p>
			  	</div>
				<div className="row">
					<div className="small-12 columns">
						<label>Distributor</label>
						<select ref="companySelector">
							{this.props.company.models.map(function(item){
								return (
									<option key={item.get('Id')} value={item.get('Id')}>{item.get('Name')}</option>
								);
							})}
						</select>
						<span className="form-error">it is required.</span>
					</div>
					<div className="small-12 columns">
						<label>Full Name
							<input onChange={this.onChange} name="FullName" type="text" required />
						</label>
					</div>
					<fieldset className="small-12 columns">
						<label>Role</label>
						<input type="radio" onChange={this.onChange} name="Role" value="Walker" id="walker" /><label htmlFor="walker">Walker</label>
						<input type="radio" onChange={this.onChange} name="Role" value="Driver" id="driver" /><label htmlFor="driver">Driver</label>
						<input type="radio" onChange={this.onChange} name="Role" value="Auditor" id="autitor" /><label htmlFor="autitor">Auditor</label>
					</fieldset>
					<div className="small-12 columns">
						<label>Cell Phone
							<input onChange={this.onChange} name="CellPhone" type="text" />
						</label>
					</div>
					<div className="small-12 medium-12 large-4 columns end">
						<label>Birthday 
							<input id="birthdayDatePicker" className="fdatepicker" onChange={this.onChange} name="DateOfBirth" type="date" readOnly />
						</label>
					</div>
					<div className="small-12 columns">
						<label>Photo
							<input ref="employeePicture" name="picture" type="file" />
						</label>
					</div>
					<div className="small-12 columns">
						<label>Notes
							<textarea onChange={this.onChange} name="Notes"></textarea>
						</label>
					</div>
				</div>
				<div className="small-12 columns">
					<div className="button-group float-right">
						<button type="submit" className="success button">Save</button>
						<a href="javascript:;" className="button" onClick={this.onClose}>Cancel</a>
					</div>
				</div>
				<button onClick={this.onClose} className="close-button" data-close aria-label="Close reveal" type="button">
			    	<span aria-hidden="true">&times;</span>
			  	</button>
			</form>
		);
	}
});