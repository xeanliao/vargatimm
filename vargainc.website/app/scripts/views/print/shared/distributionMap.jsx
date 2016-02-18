define([
	'jquery',
	'underscore',
	'moment',
	'backbone',
	'react',
	'numeral',
	'views/base',
	'views/layout/loading',
	'views/print/shared/mapZoom',

	'react.backbone'
], function ($, _, moment, Backbone, React, Numeral, BaseView, LoadingView, MapZoomView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function () {
			return {
				imageLoaded: null
			};
		},
		componentDidMount: function () {
			var id = this.getModel().get('DMapId');
			this.publish('print.map.imageloaded');
		},
		loadImage: function () {
			var model = this.getModel(),
				key = model.get('key'),
				self = this;

			$('.off-canvas-wrapper-inner').stop().animate({
			  	scrollTop: $(self.refs['mapContainer-' + key]).offset().top
			}, 500);
			this.setState({
				imageLoaded: false
			});
			model.fetchMapImage(this.props.options).always(function () {
				self.setState({
					imageLoaded: true
				});
				self.publish('print.map.imageloaded');
			});
		},
		onReloadImage: function(){
			this.setState({
				imageLoaded: null
			});
			this.publish('print.map.imageloaded');
		},
		onCreateDetailMap: function (rectBounds) {
			var model = this.getModel(),
				collection = model.collection,
				detailModel = model.clone(),
				dmapId = detailModel.get('DMapId'),
				topRight = rectBounds.getNorthEast(),
				bottomLeft = rectBounds.getSouthWest(),
				modelIndex = 0,
				serial = 0;
			_.forEach(collection.models, function(item, index){
				var currentDMapId = item.get('DMapId');
				if(currentDMapId == dmapId){
					serial++;
					modelIndex = index;
				}
			});
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
				'PolygonImage': null,
				'Serial': serial
			});
			collection.add(detailModel, {
				at: modelIndex + 1
			});
			this.publish('showDialog');
			this.publish('print.map.imageloaded');
		},
		onShowEditDialog: function () {
			var model = this.getModel(),
				self = this,
				def = $.Deferred();
			if (!this.state.imageLoaded || !model.get('MapImage') || !model.get('PolygonImage')) {
				return;
			}
			if (model.get('Boundary')) {
				def.resolve();
			} else {
				def = model.fetchBoundary({quite: true});
			}
			def.done(function () {
				var color = model.get('Color'),
					key = 'distribution-' + model.get('DMapId');
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
		render: function(){
			var model = this.getModel(),
				total = Numeral(model.get('Total')).format('0,0'),
				mapImage = model.get('MapImage'),
				polygonImage = model.get('PolygonImage');

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
				var mapImage = <LoadingView text={this.state.imageLoaded === null ? 'WAITING' : 'LOADING'} / >;
			}
			return (
				<div className="page">
					<div className="row">
	  					<div className="small-12 columns text-center title">
	  						DM MAP {model.get('DMapId')}({model.get('Name')})
  						</div>
					</div>
					<div className="row list" role="list">
						<div className="small-4 columns">DM MAP #:</div>
						<div className="small-8 columns">&nbsp;{model.get('DMapId')}</div>
						<div className="small-4 columns">DISTRIBUTION MAP NAME:</div>
						<div className="small-8 columns">&nbsp;{model.get('Name')}</div>
						<div className="small-4 columns">TOTAL:</div>
						<div className="small-8 columns">&nbsp;{total}</div>
					</div>
					<div className="row">
						<div className="small-12 columns text-center title mapTitle">
							{model.get('DisplayName')} - {model.get('Name')}
						</div>
					</div>
					<div className="row collapse">
						<div className="small-12 columns">
							<div className="map-container" ref={'mapContainer-' + model.get('key')} onClick={this.onShowEditDialog}>
								{mapImage}
							</div>
						</div>
					</div>
				</div>
			);
		}
	});
});