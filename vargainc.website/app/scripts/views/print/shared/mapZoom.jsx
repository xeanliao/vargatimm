define([
	'jquery',
	'underscore',
	'react',
	'views/base',
	'views/mapBase'
], function($, _, React, BaseView, MapBaseView){
	var googleItems = [],
		rectangle,
		drawingManager;
	return React.createClass({
		mixins: [
			BaseView,
			MapBaseView
		],
		getInitialState: function(){
			return {
				disableDefaultUI: true,
				activeButton: 'EnterMapDraw',
				activeMap: 'ROADMAP',
				mapStyle: {
					'visibility': 'hidden',
					'position': 'fixed',
					'width': '100%',
					'height': '100%',
					'left': '0',
					'top': '0',
					'right': '0',
					'bottom': '0'
				}
			};
		},
		getDefaultProps: function(){
			return {
				boundary: [],
				color: '#000',
				sourceKey: null,
				mapType: 'ROADMAP'
			};
		},
		componentDidMount: function(){
			$(document).one('open.zf.reveal', $.proxy(this.initGoogleMap, this));
			$(document).one('closed.zf.reveal', $.proxy(this.clearMap, this));
		},
		clearMap: function(){
			console.log('mapZoom clearMap');
			var googleMap = this.getGoogleMap();
			try {
				_.forEach(googleItems, function (item) {
					item && item.setMap && item.setMap(null);
				});
				rectangle && rectangle.setMap && rectangle.setMap(null);
				drawingManager && drawingManager.setMap && drawingManager.setMap(null);
				google.maps.event.clearInstanceListeners(googleMap);
				rectangle = null;
				drawingManager = null;
			} catch (ex) {
				console.log('mapZoom componentWillUnmount error', ex);
			}
			this.hideMap();
		},
		initGoogleMap: function(){
			console.log('init google map');
			var googleMap = this.getGoogleMap();
			this.showMap();
			
			var boundary = this.props.boundary,
				fillColor = this.props.color,
				mapBounds = new google.maps.LatLngBounds();
				polygon = new google.maps.Polygon({
		            paths: boundary,
		            strokeColor: '#000',
		            strokeOpacity: 1,
		            strokeWeight: 6,
		            fillColor: fillColor,
		            fillOpacity: 0.1,
		            map: googleMap
		        });
		    googleItems.push(polygon);
		    _.forEach(boundary, function(i){
		    	var point = new google.maps.LatLng(i.lat, i.lng);
            	mapBounds.extend(point);
		    });
			googleMap.fitBounds(mapBounds);

			drawingManager = new google.maps.drawing.DrawingManager({
				drawingMode: google.maps.drawing.OverlayType.MARKER,
				drawingControl: false,
				drawingControlOptions: {
					position: google.maps.ControlPosition.TOP_CENTER,
					drawingModes: [
						google.maps.drawing.OverlayType.RECTANGLE
					],
					rectangleOptions: {
						strokeColor: '#FF0000',
						strokeOpacity: 0.8,
						strokeWeight: 2,
						fillColor: '#FF0000',
						fillOpacity: 0.6,
					}
				}
			});
			drawingManager.setMap(googleMap);
			drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
			google.maps.event.addListener(drawingManager, 'rectanglecomplete', function (rect) {
				rectangle && rectangle.setMap(null);
				rectangle = new google.maps.Rectangle({
					strokeColor: '#FF0000',
					strokeOpacity: 0.8,
					strokeWeight: 1,
					fillColor: '#FF0000',
					fillOpacity: 0.35,
					map: googleMap,
					bounds: rect.getBounds()
				});
				rect.setMap(null);
			});
			googleItems.push(drawingManager);

			this.publish('hideLoading');
		},
		onReset: function(){
			var googleMap = this.getGoogleMap(),
				boundary = this.props.boundary,
				fillColor = this.props.color,
				mapBounds = new google.maps.LatLngBounds();
				polygon = new google.maps.Polygon({
		            paths: boundary,
		            strokeColor: '#000',
		            strokeOpacity: 1,
		            strokeWeight: 6,
		            fillColor: fillColor,
		            fillOpacity: 0.1,
		            map: googleMap
		        });
		    googleItems.push(polygon);
		    _.forEach(boundary, function(i){
		    	var point = new google.maps.LatLng(i.lat, i.lng);
            	mapBounds.extend(point);
		    });
			console.log('fitBounds', mapBounds.getCenter().lat(), mapBounds.getCenter().lng(), googleMap.getZoom());
			googleMap.fitBounds(mapBounds);
		},
		onZoomIn: function(){
			var googleMap = this.getGoogleMap();
			googleMap.setZoom(googleMap.getZoom() + 1);
		},
		onZoomOut: function(){
			var googleMap = this.getGoogleMap();
			googleMap.setZoom(googleMap.getZoom() - 1);
		},
		onExistMapDraw: function(){
			this.setState({'activeButton': 'ExistMapDraw'});
			drawingManager.setDrawingMode(null);
		},
		onEnterMapDraw: function(){
			this.setState({'activeButton': 'EnterMapDraw'});
			drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
		},
		onSwitchMapType: function(mapTypeName){
			var googleMap = this.getGoogleMap();
			if(googleMap && google.maps.MapTypeId && google.maps.MapTypeId[mapTypeName]){
				googleMap.setMapTypeId(google.maps.MapTypeId[mapTypeName]);
			}
			this.setState({'activeMap': mapTypeName});
		},
		onFinish: function(){
			if(!rectangle){
				return;
			}
			var bounds = rectangle.getBounds();
			this.publish('print.mapzoom@' + this.props.sourceKey, bounds, this.state.activeMap);
		},
		onClose: function(){
			this.publish("showDialog");
		},
		render: function(){
			return (
				<div className='google_map_container'>
					<button onClick={this.onClose} className="close-button" data-close aria-label="Close reveal" type="button">
				    	<span aria-hidden="true">&times;</span>
				  	</button>
					<div className="pop-tool-bar button-group text-center">
						<button className={this.state.activeButton == 'EnterMapDraw' ? 'button active' : 'button'} onClick={this.onEnterMapDraw}>
							<i className="fa fa-crop"></i>
						</button>
						<button className={this.state.activeButton == 'ExistMapDraw' ? 'button active' : 'button'}  onClick={this.onExistMapDraw}>
							<i className="fa fa-arrows"></i>
						</button>

						<button className={this.state.activeMap == 'ROADMAP' ? 'button active' : 'button'}  onClick={this.onSwitchMapType.bind(null, 'ROADMAP')}>
							<i className="fa fa-map-o"></i>
						</button>
						<button className={this.state.activeMap == 'HYBRID' ? 'button active' : 'button'}  onClick={this.onSwitchMapType.bind(null, 'HYBRID')}>
							<i className="fa fa-map"></i>
						</button>
						
						<button className='button' onClick={this.onZoomIn}>
							<i className="fa fa-search-plus"></i>
						</button>
						<button className='button' onClick={this.onZoomOut}>
							<i className="fa fa-search-minus"></i>
						</button>
						<button className='button' onClick={this.onReset}>
							<i className="fa fa-refresh"></i>
						</button>

						<button className='button' onClick={this.onFinish}>
							<i className="fa fa-image"></i>
						</button>
					</div>
				</div>
			);
		}
	});
});