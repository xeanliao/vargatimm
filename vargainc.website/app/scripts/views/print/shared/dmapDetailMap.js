define(['numeral', 'backbone', 'react', 'views/base', 'views/layout/loading', 'views/print/shared/footer', 'views/print/shared/mapZoom', 'react.backbone'], function (Numeral, Backbone, React, BaseView, LoadingView, FooterView, MapZoomView) {
	return React.createBackboneClass({
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
				if (_.isEmpty(mapImage) || _.isEmpty(ploygonImage)) {
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
		onReloadImage: function () {
			this.setState({
				imageLoaded: null,
				imageLoading: false
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
					'legend': true
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
				var mapImage = React.createElement(LoadingView, { text: this.state.imageLoading ? 'LOADING' : 'WAITING' });
			}

			return React.createElement(
				'div',
				{ className: 'page', ref: 'page' },
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
						displayTotal
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
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
							{ className: 'map-container' },
							mapImage
						),
						React.createElement(
							'div',
							{ className: 'small-12 columns' },
							React.createElement('div', { className: 'color-legend' }),
							React.createElement('div', { className: 'direction-legend' })
						)
					)
				),
				React.createElement(FooterView, { model: model.get('Footer') })
			);
		}
	});
});
