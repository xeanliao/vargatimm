import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';

import {
	each
} from 'lodash';

import BaseView from 'views/base';

export default React.createBackboneClass({
	mixins: [BaseView],
	onSelectAll: function (e) {
		var collection = this.getCollection(),
			value = e.currentTarget.checked;
		each(collection, function (item) {
			item.set('Selected', value);
		});
		this.forceUpdate();
	},
	onItemSelect: function (model, e) {
		model.set('Selected', e.currentTarget.checked);
		this.forceUpdate();
	},
	render: function () {
		var dmaps = this.getCollection(),
			btnSelectedAllStatus = dmaps.every({
				Selected: true
			}),
			self = this;
		return (
			<div className="panel callout primary">
		        <input id="btnCheckAllDMap" className='btnCheckAllDMap' type="checkbox" 
		        	checked={btnSelectedAllStatus} 
		        	onChange={this.onSelectAll} />
	        	<label htmlFor="btnCheckAllDMap">Suppress All Distribute Maps</label>
		        <div className="row small-up-1 medium-up-2 large-up-4 collapse">
		            {dmaps.map(function(map){
		            	return (
		            		<div className="column" key={'optionDmap-' + map.get('Id')}>
		            			<input id={'optionDmap-' + map.get('Id')} name={map.get('Id')} type="checkbox" 
		            				checked={map.get('Selected')} 
		            				onChange={self.onItemSelect.bind(null, map)} />
		            			<label htmlFor={'optionDmap-' + map.get('Id')}>{map.get('Name')}</label>
	            			</div>
	            		);
		            })}
		        </div>
		    </div>
		);
	}
});