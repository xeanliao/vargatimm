define(['jquery', 'underscore', 'moment', 'backbone', 'react', 'numeral', 'views/base', 'views/layout/loading', 'views/print/shared/mapZoom', 'react.backbone'], function ($, _, moment, Backbone, React, Numeral, BaseView, LoadingView, MapZoomView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function () {
			return {
				imageLoaded: false
			};
		},
		loadImage: function () {
			var model = this.getModel(),
			    self = this;
			this.setState({ imageLoaded: false });
			$('.off-canvas-wrapper-inner').stop().animate({
				scrollTop: $(self.refs.mapContainer).offset().top
			}, 500);
			model.fetchMapImage(this.props.options).always(function () {
				self.setState({
					imageLoaded: true
				});
				self.publish('print.map.imageloaded');
			});
		},
		onReloadImage: function () {
			this.setState({
				imageLoaded: null
			});
			this.publish('print.map.imageloaded');
		},
		onRemove: function () {
			var model = this.getModel(),
			    dmapId = model.get('DMapId'),
			    serial = model.get('Serial'),
			    collection = model.collection;
			collection.remove(model);
			_.forEach(collection.models, function (item) {
				var currentDmapId = item.get('DMapId'),
				    currentSerial = item.get('Serial');
				currentDmapId == dmapId && currentSerial > serial && item.set('Serial', currentSerial - 1);
			});
		},
		getExportParamters: function () {
			var model = this.getModel();
			return {
				'type': 'dmap',
				'options': [{
					'title': 'DM MAP ' + model.get('DMapId') + '(' + model.get('Name') + ') -- ' + model.get('Serial')
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
			var model = this.getModel(),
			    total = Numeral(model.get('Total')).format('0,0'),
			    mapImage = model.get('MapImage'),
			    polygonImage = model.get('PolygonImage');

			if (this.state.imageLoaded) {
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
					var mapImage = React.createElement(
						'button',
						{ className: 'button reload', onClick: this.onReloadImage },
						React.createElement('i', { className: 'fa fa-2x fa-refresh' })
					);
				}
			} else {
				var mapImage = React.createElement(LoadingView, { text: this.state.imageLoaded === null ? 'WAITING' : 'LOADING' });
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
						') -- ',
						model.get('Serial'),
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
							{ className: 'map-container', ref: 'mapContainer' },
							mapImage
						)
					)
				)
			);
		}
	});
});
