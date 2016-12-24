import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import Promise from 'bluebird';
import L from 'leaflet';
import $ from 'jquery';

import BaseView from 'views/base';

export default React.createBackboneClass({
	mixins: [BaseView],
	onInit: function (mapContainer) {
		monitorMap = L.map(mapContainer, {
			preferCanvas: true
		});
		//google road map
		L.tileLayer('http://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}', {
			maxZoom: 20,
			subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
		}).addTo(monitorMap);
		monitorMap.setView({
			lat: 30.567027824766257,
			lng: 103.94928932189943
		}, 13);
	},
	render: function () {
		var model = this.getModel();
		return (
			<div>
				<div className="section row">
					<div className="small-12 columns">
						<div className="section-header">
							<div className="row">
								<div className="small-12 column"><h5>{model.getDisplayName()}</h5></div>
							</div>
						</div>
				    </div>
		        </div>
				<div className="row">
					<div className="small-12">
						<div ref={this.onInit} style={{'minHeight': '640px'}}></div>
					</div>
				</div>
			</div>
		);
	}
});