define(['numeral', 'backbone', 'react', 'views/base', 'views/layout/loading', 'views/print/shared/footer', 'react.backbone'], function (Numeral, Backbone, React, BaseView, LoadingView, FooterView) {
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
		/**
   * template exmaple:
   * <div class="tips">COLOR LEGEND</div>
   * <div>
   *     <i class="blue"></i>
   *     <label>Blue(0%-20%)</label>
   *     <i class="green"></i>
   *     <label>Green(20%-40%)</label>
   *     <i class="yellow"></i>
   *     <label>Yellow(40%-60%)</label>
   *     <i class="orange"></i>
   *     <label>Orange(60%-80%)</label>
   *     <i class="red"></i>
   *     <label>Red(80%-100%)</label>
   * </div>
   */
		formatePenetrationColor: function (values) {
			var penetrationColorGroup = ['Blue', 'Green', 'Yellow', 'Orange', 'Red'];

			var colors = [0].concat(values),
			    min = 0;
			colors.push(100), legend = [];

			for (var i = 0; i < colors.length - 1; i++) {
				if (min >= colors[i + 1]) {
					continue;
				}
				legend.push(React.createElement('i', {
					key: 'i' + i,
					className: penetrationColorGroup[i].toLowerCase()
				}));
				legend.push(React.createElement('label', {
					key: 'l' + i
				}, penetrationColorGroup[i] + '(' + colors[i] + '%-' + colors[i + 1] + '%)'));
				min = colors[i + 1];
			}
			return React.createElement('div', {
				className: 'color-legend'
			}, React.createElement('div', {
				key: 'tips',
				className: 'tips'
			}, 'COLOR LEGEND'), React.createElement('div', {
				key: 'legend'
			}, legend.map(function (i) {
				return i;
			})));
		},
		getExportParamters: function () {
			var model = this.getModel(),
			    options = this.props && this.props.options || {};
			return {
				'type': 'campaign',
				'options': [{
					'title': 'Campaign Summary'
				}, {
					'list': [{
						'key': 'MASTER CAMPAIGN #:',
						'text': model.get('DisplayName')
					}, {
						'key': 'CLIENT NAME:',
						'text': model.get('ClientName')
					}, {
						'key': 'CONTACT NAME:',
						'text': model.get('ContactName')
					}, {
						'key': 'TARGETING METHOD:',
						'text': options.targetMethod || ''
					}, {
						'key': 'TOTAL HOUSEHOLDS:',
						'text': Numeral(model.get('TotalHouseHold')).format('0,0')
					}, {
						'key': 'TARGET HOUSEHOLDS:',
						'text': Numeral(model.get('TargetHouseHold')).format('0,0')
					}, {
						'key': 'PENETRATION:',
						'text': Numeral(model.get('Penetration')).format('0.00%')
					}]
				}, {
					'title': 'Campaign Summary Map'
				}, {
					'map': model.get('PolygonImage'),
					'bg': model.get('MapImage'),
					'legend': 'true'
				}]
			};
		},
		render: function () {
			var model = this.getModel(),
			    options = this.props && this.props.options ? this.props.options : {},
			    targetMethod = options.targetMethod || '',
			    displayTotalHouseHold = Numeral(model.get('TotalHouseHold')).format('0,0'),
			    displayTargetHouseHold = Numeral(model.get('TargetHouseHold')).format('0,0'),
			    displayPenetration = Numeral(model.get('Penetration')).format('0.00%'),
			    mapImage = model.get('MapImage'),
			    polygonImage = model.get('PolygonImage');

			if (options.showPenetrationColors) {
				var colorLegend = this.formatePenetrationColor(options.penetrationColors);
			} else {
				var colorLegend = null;
			}
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
				{ className: 'page campaign', ref: 'page' },
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'Campaign Summary'
					)
				),
				React.createElement(
					'div',
					{ className: 'row list', role: 'list' },
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'MASTER CAMPAIGN #:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						model.get('DisplayName')
					),
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'CLIENT NAME:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						model.get('ClientName')
					),
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'CONTACT NAME:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						model.get('ContactName')
					),
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TARGETING METHOD:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						targetMethod
					),
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TOTAL HOUSEHOLDS:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						displayTotalHouseHold
					),
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TARGET HOUSEHOLDS:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						displayTargetHouseHold
					),
					React.createElement(
						'div',
						{ className: 'small-4 columns' },
						'PENETRATION:'
					),
					React.createElement(
						'div',
						{ className: 'small-8 columns' },
						' ',
						displayPenetration
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'Campaign Summary Map'
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
							{ className: 'map-container', ref: model.get('key') },
							mapImage
						),
						React.createElement(
							'div',
							{ className: 'small-12 columns' },
							colorLegend,
							React.createElement('div', { className: 'direction-legend' })
						)
					)
				),
				React.createElement(FooterView, { model: model.get('Footer') })
			);
		}
	});
});
