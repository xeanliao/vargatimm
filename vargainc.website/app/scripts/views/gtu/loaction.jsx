define([
	'moment',
	'backbone',
	'react',
	'views/base',
	'react.backbone'
], function (moment, Backbone, React, BaseView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		componentDidMount: function(){
			
		},
		onSave: function(e){
			e.preventDefault();
			e.stopPropagation();
			var self = this;
			
		},
		onClose: function(){
			$('.fdatepicker').off('changeDate').fdatepicker('remove');
			this.publish("showDialog");
		},
		onChange: function(e){
			this.getModel().set(e.currentTarget.name, e.currentTarget.value);
		},
		render: function(){
			var model = this.getModel();
			
			return (
				<form data-abide onSubmit={this.onSave}>
					<h3>{title}</h3>
					<div data-abide-error className="alert callout" style={{display: showError ? 'block' : 'none'}}>
				    	<p><i className="fa fa-exclamation-circle"></i>&nbsp;{errorMessage}</p>
				  	</div>
					<div className="row">
						<div className="small-12 columns">
							<label>Client Name
								<input onChange={this.onChange} name="ClientName" type="text" defaultValue={model.get('ClientName')} required />
								<span className="form-error">it is required.</span>
							</label>
						</div>
						<div className="small-12 columns">
							<label>Contact Name
								<input onChange={this.onChange} name="ContactName" type="text" defaultValue={model.get('ContactName')} required />
							</label>
						</div>
						<div className="small-12 columns">
							<label>Client Code
								<input onChange={this.onChange} name="ClientCode" type="text" defaultValue={model.get('ClientCode')} required />
							</label>
						</div>
						<fieldset className="small-12 columns">
							<label>Total Type</label>
							<input type="radio" onChange={this.onChange} name="AreaDescription" checked={"APT + HOME" == AreaDescription} value="APT + HOME" id="apthome" /><label htmlFor="apthome">APT + HOME</label>
							<input type="radio" onChange={this.onChange} name="AreaDescription" checked={"APT ONLY" == AreaDescription} value="APT ONLY" id="aptonly" /><label htmlFor="aptonly">APT ONLY</label>
							<input type="radio" onChange={this.onChange} name="AreaDescription" checked={"HOME ONLY" == AreaDescription} value="HOME ONLY" id="homeonly" /><label htmlFor="homeonly">HOME ONLY</label>
						</fieldset>
						<div className="small-12 medium-12 large-4 columns end">
							<label>Date
								<input className="fdatepicker" onChange={this.onChange} name="Date" type="date" readOnly value={displayDate} required />
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
});