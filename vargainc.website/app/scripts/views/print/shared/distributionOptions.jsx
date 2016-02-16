define([
	'backbone',
	'react',
	'views/print/shared/options',
	'views/print/shared/optionsDMapSelector',
	'react.backbone'
], function (Backbone, React, OptionsView, OptionsDMapSelector) {
	return React.createBackboneClass({
		mixins: [OptionsView],
		render: function(){
			var model = this.getModel();
			return (
				<div>
					<h3>Print Options</h3>
				    <div className="panel callout secondary">
				        <h6>Distribution Maps</h6>
				        <div className="row medium-up-1 large-up-2 collapse">
				            <div className="column">
				                <input id="suppressDMap" name="suppressDMap" type="checkbox" checked={model.get('suppressDMap')} onChange={this.OnValueChanged} />
			                	<label htmlFor="suppressDMap">Suppress Distribution Maps</label>
				            </div>
				            <div className="column">
				                <input id="suppressNDAInDMap" name="suppressNDAInDMap" type="checkbox" checked={model.get('suppressNDAInDMap')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressNDAInDMap">Suppress non-deliverables for distribution map</label>  
				            </div>
				        </div>
				    </div>
				   	<OptionsDMapSelector collection={model.get('DMaps')} />
				    <div className="small-12 columns">
						<div className="button-group float-right">
							<button className="success button" onClick={this.onApply}>Save</button>
							<button className="button" onClick={this.onClose}>Cancel</button>
						</div>
					</div>
					<button onClick={this.onClose} className="close-button" data-close aria-label="Close reveal" type="button">
				    	<span aria-hidden="true">&times;</span>
				  	</button>
				</div>
			);
		}
	});
});