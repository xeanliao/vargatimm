define([
	'backbone',
	'react',
	'views/print/shared/options',
	'views/print/shared/penetrationColor',
	'views/print/shared/optionsDMapSelector',
	'react.backbone'
], function (Backbone, React, OptionsView, PenetrationColorView, OptionsDMapSelectorView) {
	return React.createBackboneClass({
		mixins: [OptionsView],
		onPenetrationColorsChanged: function (values) {
			var model = this.getModel();
			model.set('penetrationColors', values, {
				silent: true
			});
		},
		render: function(){
			var model = this.getModel();

			return (
				<div>
					<h3>Print Options</h3>
					<div className="row collapse">
						<div className="small-12 columns">
							<label>
								Target Method:<input type="text" name="targetMethod" defaultValue={model.get('targetMethod')} onChange={this.OnValueChanged} />
							</label>
						</div>
					</div>
					<div className="panel callout secondary">
				        <h6>Campaign Maps</h6>
				        <div className="row medium-up-2 large-up-2 collapse">
				            <div className="column">
				                <input id="suppressCover" name="suppressCover" type="checkbox" checked={model.get('suppressCover')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressCover">Suppress Cover</label>
				            </div>
				            <div className="column">
				                <input id="suppressCampaign" name="suppressCampaign" type="checkbox" checked={model.get('suppressCampaign')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressCampaign">Suppress Campaign Page</label>
				            </div>
				            <div className="column">
				                <input id="suppressSubMap" name="suppressSubMap" type="checkbox" checked={model.get('suppressSubMap')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressSubMap">Suppress Sub Maps</label>
				            </div>
				            <div className="column">
				                <input id="suppressCampaignSummary" name="suppressCampaignSummary" type="checkbox" checked={model.get('suppressCampaignSummary')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressCampaignSummary">Suppress Sub Map Summary</label>
				            </div>
				            <div className="column">
				                <input id="suppressSubMapCountDetail" name="suppressSubMapCountDetail" type="checkbox" checked={model.get('suppressSubMapCountDetail')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressSubMapCountDetail">Suppress Sub Map Croute Counts</label>
				            </div>
				            <div className="column">
				                <input id="suppressNDAInCampaign" name="suppressNDAInCampaign" type="checkbox" checked={model.get('suppressNDAInCampaign')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressNDAInCampaign">Suppress non-deliverables for campaign map</label>
				            </div>
				            <div className="column">
				                <input id="suppressNDAInSubMap" name="suppressNDAInSubMap" type="checkbox" checked={model.get('suppressNDAInSubMap')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressNDAInSubMap">Suppress non-deliverables for sub map</label>
				            </div>
				            <div className="column">
				                <input id="suppressLocations" name="suppressLocations" type="checkbox" checked={model.get('suppressLocations')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressLocations">Suppress Locations</label>
				            </div>
				            <div className="column">
				                <input id="suppressRadii" name="suppressRadii" type="checkbox" checked={model.get('suppressRadii')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressRadii">Suppress Radii</label>
				            </div>
				        </div>
				        <div className="row small-up-1 collapse">
				        	<div className="column">
				                <input id="showPenetrationColors" name="showPenetrationColors" type="checkbox" checked={model.get('showPenetrationColors')} onChange={this.OnValueChanged} />
				                <label htmlFor="showPenetrationColors">Show Penetration Colors:</label>
				            </div>
				            <PenetrationColorView colors={model.get('penetrationColors')} onChange={this.onPenetrationColorsChanged} />
				        </div>
				    </div>
					<div className="panel callout secondary">
				        <h6>GTU Reports</h6>
				        <div className="row medium-up-1 large-up-2 collapse">
				            <div className="column">
				                <input id="suppressGTU" name="suppressGTU" type="checkbox" checked={model.get('suppressGTU')} onChange={this.OnValueChanged} />
				                <label htmlFor="suppressGTU">Suppress GTU Tracking</label>
				            </div>
				        </div>
				    </div>
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
				   	<OptionsDMapSelectorView collection={model.get('DMaps')} />
				    <div className="small-12 columns">
						<div className="button-group float-right">
							<button className="success button" onClick={this.onApply}>Apply</button>
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