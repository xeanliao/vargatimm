define([
	'jquery',
	'underscore',
	'moment',
	'backbone',
	'react',
	'numeral',
	'views/base',
	'views/print/shared/mapZoom',
	'react.backbone'
], function ($, _, moment, Backbone, React, Numeral, BaseView, MapZoomView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function(){
			return {
				imageLoaded: false
			};
		},
		loadImage: function(){
			var model = this.getModel(),
				self = this;
			this.setState({imageLoaded: false});
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
		onRemove: function(){
			var model = this.getModel();
			model.collection.remove(model);
		},
		getExportParamters: function(){
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
	  						DM MAP {model.get('DMapId')}({model.get('Name')}) -- detail map
	  						<button className="button float-right" onClick={this.onRemove}>
								<i className="fa fa-delete"></i>Remove
	  						</button>
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
							<div className="map-container">
								{mapImage}
							</div>
						</div>
					</div>
				</div>
			);
		}
	});
});