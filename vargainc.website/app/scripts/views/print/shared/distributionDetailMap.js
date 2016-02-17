define(['jquery', 'underscore', 'moment', 'backbone', 'react', 'numeral', 'views/base', 'views/print/shared/mapZoom', 'react.backbone'], function ($, _, moment, Backbone, React, Numeral, BaseView, MapZoomView) {
	var mapZoomHandler = null;
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function () {
			return {
				ImageLoaded: false
			};
		},
		loadImage: function () {
			var model = this.getModel(),
			    self = this;
			model.fetchMapImage(this.props.options).always(function () {
				self.setState({
					ImageLoaded: true
				});
				self.publish('print.map.imageloaded');
			});
		},
		onRemove: function () {
			var model = this.getModel();
			model.collection.remove(model);
		},
		getExportParamters: function () {
			var model = this.getModel();
			return {
				'type': 'dmap',
				'options': [{
					'title': 'DM MAP ' + model.get('DMapId') + '(' + model.get('Name') + ') -- detail map'
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
						') -- detail map',
						React.createElement(
							'button',
							{ className: 'button float-right', onClick: this.onRemove },
							React.createElement('i', { className: 'fa fa-delete' }),
							'Remove'
						)
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
							{ className: 'map-container dmapPrint', onClick: this.onEdit },
							mapImage
						)
					)
				)
			);
		}
	});
});
