import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import $ from 'jquery';
import Numeral from 'numeral';
import {
	isEmpty,
	each
} from 'lodash';

import BaseView from 'views/base';
import LoadingView from 'views/layout/loading';
import FooterView from 'views/print/shared/footer';
import MapZoomView from 'views/print/shared/mapZoom';

export default React.createBackboneClass({
	mixins: [BaseView],
	getInitialState: function () {
		return {
			imageLoaded: null,
			imageLoading: false
		};
	},
	componentDidMount: function () {
		this.publish('print.map.imageloaded');
	},
	scrollToPage: function () {
		var model = this.getModel(),
			height = $(this.refs.page).position().top;
		$('.off-canvas-wrapper-inner').stop().animate({
			scrollTop: height
		}, 600);
	},
	preloadImage: function (imageAddress) {
		var def = $.Deferred();
		$(new Image()).one('load', function () {
			def.resolve();
		}).attr('src', imageAddress);
		return def;
	},
	loadImage: function () {
		var model = this.getModel(),
			key = model.get('key'),
			self = this;
		this.setState({
			imageLoaded: null,
			imageLoading: true
		});
		this.scrollToPage();
		model.fetchMapImage(this.props.options).then(function () {
			var def = $.Deferred(),
				mapImage = model.get('MapImage'),
				ploygonImage = model.get('PolygonImage');
			if (isEmpty(mapImage) || isEmpty(ploygonImage)) {
				def.reject();
			} else {
				$.when(self.preloadImage(mapImage), self.preloadImage(ploygonImage)).done(function () {
					def.resolve();
				});
			}
			return def;
		}).always(function () {
			self.setState({
				imageLoaded: true,
				imageLoading: false
			});
			self.publish('print.map.imageloaded');
		});
	},
	onReloadImage: function (e) {
		e.stopPropagation();
		this.setState({
			imageLoaded: null,
			imageLoading: false
		});
		this.publish('print.map.imageloaded');
	},
	onShowEditDialog: function () {
		var model = this.getModel(),
			self = this,
			def = $.Deferred();
		if (!this.state.imageLoaded || !model.get('MapImage') || !model.get('PolygonImage')) {
			return;
		}
		this.publish('showLoading');
		if (model.get('Boundary')) {
			def.resolve();
		} else {
			def = model.fetchBoundary({
				quite: true
			});
		}
		def.done(function () {
			var color = model.get('Color'),
				key = 'dmap-' + model.get('DMapId');
			self.unsubscribe('print.mapzoom@' + key);
			self.subscribe('print.mapzoom@' + key, $.proxy(self.onCreateDetailMap, self));
			self.publish('showDialog', MapZoomView, {
				sourceKey: key,
				boundary: model.get('Boundary'),
				color: 'rgb(' + color.r + ',' + color.g + ',' + color.b + ')'
			}, {
				size: 'full',
				customClass: 'google-map-pop'
			});
		});
	},
	onCreateDetailMap: function (rectBounds) {
		var model = this.getModel(),
			collection = model.collection,
			detailModel = model.clone(),
			dmapId = detailModel.get('DMapId'),
			topRight = rectBounds.getNorthEast(),
			bottomLeft = rectBounds.getSouthWest(),
			modelIndex = 0,
			serial = 0,
			key = model.get('key') + '-' + topRight.lat() + '-' + topRight.lng() + '-' + bottomLeft.lat() + '-' + bottomLeft.lng();

		each(collection.models, function (item, index) {
			var currentDMapId = item.get('DMapId');
			if (currentDMapId == dmapId) {
				serial++;
				modelIndex = index;
			}
		});
		detailModel.set({
			'key': key,
			'type': 'DMapDetailMap',
			'TopRight': {
				lat: topRight.lat(),
				lng: topRight.lng()
			},
			'BottomLeft': {
				lat: bottomLeft.lat(),
				lng: bottomLeft.lng()
			},
			'ImageStatus': 'waiting',
			'MapImage': null,
			'PolygonImage': null,
			'Serial': serial
		});
		collection.add(detailModel, {
			at: modelIndex + 1
		});
		this.publish('showDialog');
		this.publish('print.map.imageloaded');
	},
	getExportParamters: function () {
		var model = this.getModel();
		return {
			'type': 'dmap',
			'options': [{
				'title': 'DM MAP ' + model.get('DMapId') + '(' + model.get('Name') + ')'
			}, {
				'list': [{
					'key': 'DM MAP #:',
					'text': model.get('DMapId') + ''
				}, {
					'key': 'DISTRIBUTION MAP NAME:',
					'text': model.get('Name')
				}, {
					'key': 'TOTAL:',
					'text': Numeral(model.get('Total')).format('0,0')
				}]
			}, {
				'title': model.get('DisplayName') + ' - ' + model.get('Name')
			}, {
				'map': model.get('PolygonImage'),
				'bg': model.get('MapImage'),
				'legend': true,
			}]
		};
	},
	render: function () {
		var model = this.getModel(),
			displayTotal = Numeral(model.get('Total')).format('0,0'),
			mapImage = model.get('MapImage'),
			polygonImage = model.get('PolygonImage');

		if (this.state.imageLoaded) {
			if (mapImage && polygonImage) {
				let style = {
					'backgroundImage': 'url(' + mapImage + ')',
					'backgroundRepeat': 'no-repeat',
					'backgroundSize': '100% auto',
					'backgroundPosition': '0 0'
				};
				var mapImage = (
					<div style={style}>
							<img src = {polygonImage} />
						</div>
				);
			} else {
				var mapImage = (
					<button className="button reload" onClick={this.onReloadImage}>
							<i className="fa fa-2x fa-refresh"></i>
						</button>
				);
			}
		} else {
			var mapImage = <LoadingView text={this.state.imageLoading ? 'LOADING' : 'WAITING'} / >;
		}

		return (
			<div className="page" ref="page">
				<div className="row">
  					<div className="small-12 columns text-center title">DM MAP {model.get('DMapId')}({model.get('Name')})</div>
				</div>
				<div className="row list" role="list">
					<div className="small-4 columns">DM MAP #:</div>
					<div className="small-8 columns">&nbsp;{model.get('DMapId')}</div>
					<div className="small-4 columns">DISTRIBUTION MAP NAME:</div>
					<div className="small-8 columns">&nbsp;{model.get('Name')}</div>
					<div className="small-4 columns">TOTAL:</div>
					<div className="small-8 columns">&nbsp;{displayTotal}</div>
				</div>
				<div className="row">
					<div className="small-12 columns text-center title">{model.get('DisplayName')} - {model.get('Name')}</div>
				</div>
				<div className="row collapse">
					<div className="small-12 columns">
						<div className="map-container" onClick={this.onShowEditDialog}>
							{mapImage}
						</div>
						<div className="small-12 columns">
							<div className="color-legend"></div>
							<div className="direction-legend"></div>
						</div>
					</div>
				</div>
				<FooterView model={model.get('Footer')} />
			</div>
		);
	}
});