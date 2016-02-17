define(['jquery', 'underscore', 'moment', 'backbone', 'react', 'numeral', 'views/base', 'views/print/shared/mapZoom', 'react.backbone'], function ($, _, moment, Backbone, React, Numeral, BaseView, MapZoomView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function () {
			return { ImageLoaded: false };
		},
		componentDidMount: function () {
			var id = this.getModel().get('DMapId');
			this.publish('print.map.imageloaded');
		},
		loadImage: function () {
			var model = this.getModel(),
			    self = this;
			model.fetchMapImage(this.props.options).always(function () {
				console.log('always');
				self.setState({ ImageLoaded: true });
				self.publish('print.map.imageloaded');
			});
		},
		onCreateDetailMap: function (rectBounds) {
			var model = this.getModel(),
			    detailModel = model.clone(),
			    topRight = rectBounds.getNorthEast(),
			    bottomLeft = rectBounds.getSouthWest(),
			    modelIndex = model.collection.indexOf(model);
			detailModel.set({
				'key': model.get('key') + '-' + topRight.lat() + '-' + topRight.lng() + '-' + bottomLeft.lat() + '-' + bottomLeft.lng(),
				'type': 'DistributionDetail',
				'TopRight': {
					lat: topRight.lat(),
					lng: topRight.lng()
				},
				'BottomLeft': {
					lat: bottomLeft.lat(),
					lng: bottomLeft.lng()
				},
				'Boundary': null,
				'ImageStatus': 'waiting',
				'MapImage': null,
				'PolygonImage': null
			});
			model.collection.add(detailModel, {
				at: modelIndex + 1
			});
			this.publish('showDialog');
			this.publish('print.map.imageloaded');
		},
		onShowEditDialog: function () {
			var model = this.getModel(),
			    self = this,
			    def = $.Deferred();
			if (!this.state.ImageLoaded || !model.get('MapImage') || !model.get('PolygonImage')) {
				return;
			}
			if (model.get('Boundary')) {
				def.resolve();
			} else {
				def = model.fetchBoundary();
			}
			def.done(function () {
				var mapZoomView = React.createFactory(MapZoomView),
				    color = model.get('Color'),
				    key = 'distribution-' + model.get('DMapId'),
				    view = mapZoomView({
					sourceKey: key,
					boundary: model.get('Boundary'),
					color: 'rgb(' + color.r + ',' + color.g + ',' + color.b + ')'
				});
				self.unsubscribe('print.mapzoom@' + key);
				self.subscribe('print.mapzoom@' + key, $.proxy(self.onCreateDetailMap, self));
				self.publish('showDialog', view, null, {
					size: 'full'
				});
			});
		},
		getExportParamters: function () {
			var model = this.getModel();
			return {
				'type': 'dmap',
				'options': [{
					'title': 'DM MAP ' + model.get('DMapId') + '(' + model.get('Name') + ')'
				}, {
					'list': [{
						'key': 'DM MAP #',
						'text': model.get('DMapId')
					}, {
						'key': 'DISTRIBUTION MAP NAME',
						'text': model.get('Name')
					}, {
						'key': 'TOTAL',
						'text': Numeral(model.get('Total')).format('0,0')
					}]
				}, {
					'title': model.get('DisplayName') + ' - ' + model.get('Name')
				}, {
					'map': model.get('PolygonImage'),
					'bg': model.get('MapImage')
				}]
			};
		},
		render: function () {
			console.log('render');
			var model = this.getModel();
			var total = Numeral(model.get('Total')).format('0,0');
			var mapImage = model.get('MapImage');
			var polygonImage = model.get('PolygonImage');
			if (this.state.ImageLoaded) {
				if (mapImage && polygonImage) {
					var mapImage = React.createElement(
						'div',
						{ style: {
								'backgroundImage': 'url(' + mapImage + ')',
								'backgroundRepeat': 'no-repeat',
								'backgroundSize': '100% auto',
								'backgroundPosition': '0 0'
							} },
						React.createElement('img', { src: polygonImage })
					);
				} else {
					var mapImage = React.createElement('div', { className: 'retry' }, null);
				}
			} else {
				var mapImage = React.createElement('div', { className: 'loading' }, null);
			}
			return React.createElement(
				'div',
				{ className: 'page' },
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'DM MAP ',
						model.get('DMapId'),
						'(',
						model.get('Name'),
						')'
					)
				),
				React.createElement(
					'div',
					{ className: 'row list', role: 'list' },
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DM MAP #:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						model.get('DMapId')
					),
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DISTRIBUTION MAP NAME:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						model.get('Name')
					),
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TOTAL:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						total
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center title mapTitle' },
						model.get('DisplayName'),
						' - ',
						model.get('Name')
					)
				),
				React.createElement(
					'div',
					{ className: 'row collapse' },
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'div',
							{ className: 'map-container dmapPrint', onClick: this.onShowEditDialog },
							mapImage
						)
					)
				)
			);
		}
	});
});
