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
	var mapZoomHandler = null;
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function(){
			return {
				ImageLoaded: false
			};
		},
		loadImage: function(){
			var model = this.getModel(),
				self = this;
			model.fetchMapImage(this.props.options).always(function () {
				self.setState({
					ImageLoaded: true
				});
				self.publish('print.map.imageloaded');
			});
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
			var model = this.getModel();
			var total = Numeral(model.get('Total')).format('0,0');
			var mapImage = model.get('MapImage');
			var polygonImage = model.get('PolygonImage');
			if(this.state.ImageLoaded){
				if(mapImage && polygonImage){
					var mapImage = (
						<div style={{
							'backgroundImage': 'url(' + mapImage + ')', 
							'backgroundRepeat': 'no-repeat',
							'backgroundSize': '100% auto',
							'backgroundPosition': '0 0'
						}}>
							<img src={polygonImage} />
						</div>
					);
				}else{
					var mapImage = React.createElement('div', { className: 'retry' }, null);
				}
			}else{
				var mapImage = React.createElement('div', { className: 'loading' }, null);
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
							<div className="map-container dmapPrint" onClick={this.onEdit}>
								{mapImage}
							</div>
						</div>
					</div>
				</div>
			);
		}
	});
});