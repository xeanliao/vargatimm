define([
	'numeral',
	'backbone',
	'react',
	'views/base',
	'views/layout/loading',
	'views/print/shared/footer',
	'react.backbone'
], function (Numeral, Backbone, React, BaseView, LoadingView, FooterView) {
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
		onReloadImage: function(){
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
			colors.push(100),
				legend = [];

			for (var i = 0; i < colors.length - 1; i++) {
				if (min >= colors[i + 1]) {
					continue;
				}
				legend.push(React.createElement(
					'i', {
						key: 'i' + i,
						className: penetrationColorGroup[i].toLowerCase()
					}
				));
				legend.push(React.createElement(
					'label', {
						key: 'l' + i
					},
					penetrationColorGroup[i] + '(' + colors[i] + '%-' + colors[i + 1] + '%)'
				));
				min = colors[i + 1];
			}
			return React.createElement('div', {
					className: 'color-legend'
				},
				React.createElement(
					'div', {
						key: 'tips',
						className: 'tips'
					},
					'COLOR LEGEND'),
				React.createElement(
					'div', {
						key: 'legend'
					}, legend.map(function (i) {
						return i;
					})));
		},
		getExportParamters: function () {
			var model = this.getModel(),
			options = this.props && this.props.options || {};
			return {
				'type': 'submap',
				'options': [{
					'title': 'SUB MAP ' + model.get('OrderId') + '(' + model.get('Name') + ')'
				}, {
					'list': [{
						'key': 'SUB MAP #:',
						'text': model.get('OrderId')
					}, {
						'key': 'SUB MAP NAME:',
						'text': model.get('Name')
					},  {
						'key': 'TARGETING METHOD:',
						'text': options.targetMethod || ''
					},  {
						'key': 'TOTAL HOUSEHOLDS:',
						'text': Numeral(model.get('TotalHouseHold')).format('0,0')
					},  {
						'key': 'TARGET HOUSEHOLDS:',
						'text': Numeral(model.get('TargetHouseHold')).format('0,0')
					},  {
						'key': 'PENETRATION:',
						'text': Numeral(model.get('Penetration')).format('0.00%')
					}]
				}, {
					'title': 'Map'
				}, {
					'map': model.get('PolygonImage'),
					'bg': model.get('MapImage'),
					"legend": "true",
					"color": !options.showPenetrationColors ? null : options.penetrationColors
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
					var mapImage = ( 
						<div style = {
							{
								'backgroundImage': 'url(' + mapImage + ')',
								'backgroundRepeat': 'no-repeat',
								'backgroundSize': '100% auto',
								'backgroundPosition': '0 0'
							}
						}><img src = {polygonImage} />
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
				<div className="page submap" ref="page">
					<div className="row">
	  					<div className="small-12 columns text-center title">SUB MAP {model.get('OrderId')}({model.get('Name')})</div>
					</div>
					<div className="row list" role="list">
						<div className="small-4 columns">SUB MAP #:</div>
						<div className="small-8 columns">&nbsp;{model.get('OrderId')}</div>
						<div className="small-4 columns">SUB MAP NAME:</div>
						<div className="small-8 columns">&nbsp;{model.get('Name')}</div>
						<div className="small-4 columns">TARGETING METHOD:</div>
						<div className="small-8 columns">&nbsp;{targetMethod}</div>
						<div className="small-4 columns">TOTAL HOUSEHOLDS:</div>
						<div className="small-8 columns">&nbsp;{displayTotalHouseHold}</div>
						<div className="small-4 columns">TARGET HOUSEHOLDS:</div>
						<div className="small-8 columns">&nbsp;{displayTargetHouseHold}</div>
						<div className="small-4 columns">PENETRATION:</div>
						<div className="small-8 columns">&nbsp;{displayPenetration}</div>
					</div>
					<div className="row">
						<div className="small-12 columns text-center title">Map</div>
					</div>
					<div className="row collapse">
						<div className="small-12 columns">
							<div className="map-container" ref={model.get('key')}>
								{mapImage}
							</div>
							<div className="small-12 columns">
								{colorLegend}
								<div className="direction-legend"></div>
							</div>
						</div>
					</div>
					<FooterView model={model.get('Footer')} />
				</div>
			);
		}
	});
});