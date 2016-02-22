define([
	'jquery',
	'underscore',
	'react',
	'views/base',
	'async!http://maps.googleapis.com/maps/api/js?key=AIzaSyBOhGHk0xE1WEG0laS4QGRp_AGhtB5LMHw&libraries=drawing'
], function($, _, React, BaseView){
	var googleMap = null,
		googleItems = [],
		rectangle,
		drawingManager;
	return React.createClass({
		mixins: [BaseView],
		getInitialState: function(){
			return {
				activeButton: 'EnterMapDraw',
				activeMap: 'ROADMAP'
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
		componentWillMount: function(){
			if($('#google-map').size() == 0){
				$('<div id="google-map" class="google-map"></div>').appendTo('body');
			}
		},
		componentDidMount: function(){
			$(document).one('open.zf.reveal', $.proxy(this.initGoogleMap, this));
			$(document).one('closed.zf.reveal', $.proxy(this.componentWillUnmount, this));
		},
		componentWillUnmount: function(){
			try {
				_.forEach(googleItems, function (item) {
					item.setMap(null);
				});
				rectangle && rectangle.setMap(null);
				drawingManager && drawingManager.setMap(null);
				google.maps.event.clearInstanceListeners(googleMap);
				rectangle = null;
				drawingManager = null;
			} catch (ex) {
				console.log('mapZoom componentWillUnmount error', ex);
			}
			$('#google-map').css({
				'visibility': 'hidden',
			});
		},
		initGoogleMap: function(){
			console.log('init google map');
			$('#google-map').css({
				'visibility': 'visible'
			});
			
			if(!googleMap){
				googleMap = new google.maps.Map($('#google-map')[0], {
			        center: new google.maps.LatLng(40.744556, -73.987378),
			        zoom: 18,
			        disableDefaultUI: true,
			        mapTypeId: google.maps.MapTypeId.ROADMAP
			    });
			}else{
				googleMap.setMapTypeId(google.maps.MapTypeId[this.props.mapType]);
				this.setState({'activeMap': this.props.mapType});
			}

			google.maps.event.trigger(googleMap, "resize");

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
			console.log('fitBounds', mapBounds.getCenter().lat(), mapBounds.getCenter().lng(), googleMap.getZoom());
			googleMap.fitBounds(mapBounds);
		},
		onZoomIn: function(){
			googleMap.setZoom(googleMap.getZoom() + 1);
		},
		onZoomOut: function(){
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