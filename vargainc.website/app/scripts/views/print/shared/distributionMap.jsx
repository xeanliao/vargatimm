define([
	'underscore',
	'moment',
	'backbone',
	'react',
	'numeral',
	'views/base',
	'views/print/shared/mapZoom',
	'react.backbone'
], function (_, moment, Backbone, React, Numeral, BaseView, MapZoom) {
	return React.createClass({
		mixins: [BaseView],
		getDefaultProps: function(){
			return {
				Id: null,
				Name: null,
				Total: null,
				DisplayName: null,
				MapImage: null,
				PolygonImage: null,
				Boundary: [],
				ZoomImage: false
			}
		},
		onEdit: function(){
			var mapZoom = MapZoom();

			this.publish('print/')
		},
		render: function(){
			var total = Numeral(this.props.Total).format('0,0.0000');
			return (
				<div>
					<div class="row">
	  					<div class="small-12 columns text-center title"><span class="editable" role="title">DM MAP {this.props.Id}({this.props.Name})</span></div>
					</div>
					<div class="row list" role="list">
						<div class="small-4 columns">&nbsp;<span class="editable" role="key">DM MAP #:</span></div>
						<div class="small-8 columns">&nbsp;<span class="editable" role="text">{dmap.Id}</span></div>
						<div class="small-4 columns">&nbsp;<span class="editable" role="key">DISTRIBUTION MAP NAME</span>:</div>
						<div class="small-8 columns">&nbsp;<span class="editable" role="text">{dmap.Name}</span></div>
						<div class="small-4 columns">&nbsp;<span class="editable" role="key">TOTAL</span>:</div>
						<div class="small-8 columns">&nbsp;<span class="editable" role="text">{total}</span></div>
					</div>
					<div class="row">
						<div class="small-12 columns text-center title mapTitle"><span class="editable" role="title">{this.props.DisplayName} - {this.props.Name}</span></div>
					</div>
					<div class="row collapse">
						<div class="small-12 columns">
							<div class="map-container" role="map" onClick={this.onEdit}>
								<div class="loading hexdots-loader"></div>
							</div>
						</div>
					</div>
				</div>
			);
		}
	});
});