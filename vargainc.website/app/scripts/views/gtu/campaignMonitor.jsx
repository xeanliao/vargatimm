import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import Promise from 'bluebird';
import L from 'leaflet';
import 'lib/leaflet-google';
import $ from 'jquery';
import {
	groupBy,
	map,
	each,
	extend
} from 'lodash';

import BaseView from 'views/base';

export default React.createBackboneClass({
	mixins: [BaseView],
	shouldComponentUpdate: function () {
		return false;
	},
	onInit: function (mapContainer) {
		var monitorMap = L.map(mapContainer, {
			center: {
				lat: 41.850033,
				lng: -87.6500523
			},
			zoom: 13,
			preferCanvas: false,
			animate: false,
		});
		new L.googleTile({
			mapTypeId: google.maps.MapTypeId.HYBRID
		}).addTo(monitorMap);
		var self = this;
		this.setState({
			map: monitorMap
		}, () => {
			self.drawBoundary(monitorMap);
		});
	},
	drawBoundary: function (monitorMap) {
		var model = this.getModel(),
			campaignId = model.get('Id'),
			tasks = model.get('Tasks'),
			submapTasks = groupBy(tasks.models, t => {
				return t.get('SubMapId')
			}),
			promiseSubmapArray = [],
			promiseTaskArray = [];

		each(submapTasks, (groupTasks, submapId) => {
			promiseSubmapArray.push(new Promise((resolve, reject) => {
				let url = `../api/print/campaign/${campaignId}/submap/${submapId}/boundary`;
				$.getJSON(url).then(result => {
					let latlngs = map(result.boundary, i => {
						return [i.lat, i.lng];
					});
					L.polyline(latlngs, {
						color: `rgb(${result.color.r}, ${result.color.g}, ${result.color.b})`,
						weight: 8,
						noClip: true
					}).addTo(monitorMap);
					resolve(L.latLngBounds(latlngs));
				})
			}));
		});

		return Promise.all(promiseSubmapArray).then(bounds => {
			monitorMap.flyToBounds(bounds);
		}).then(() => {
			each(submapTasks, groupTasks => {
				each(groupTasks, task => {
					promiseTaskArray.push(new Promise((resolve, reject) => {
						let url = `../api/print/campaign/${task.get('CampaignId')}/submap/${task.get('SubMapId')}/dmap/${task.get('DMapId')}/boundary`;
						let fillStyle = task.get('Status') == 1 ? {
							color: '#000',
							opacity: 0.75,
							fill: true,
							fillOpacity: 0.2,
						} : {
							opacity: 0.75,
							fill: true,
							fillOpacity: 0.1,
						};
						$.getJSON(url).then(result => {
							L.polyline(map(result.boundary, i => {
								return [i.lat, i.lng];
							}), assign({
								color: `rgb(${result.color.r}, ${result.color.g}, ${result.color.b})`,
								bindPopup: `${task.get('Name')}`,
								noClip: true
							}, fillStyle)).addTo(monitorMap);
						});
					}));
				});
			});

			return Promise.all(promiseTaskArray);
		});

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