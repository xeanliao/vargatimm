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
				activeButton: 'EnterMapDraw'
			};
		},
		getDefaultProps: function(){
			return {
				boundary: [],
				color: '#000',
				sourceKey: null
			};
		},
		componentWillMount: function(){
			if($('#google-map').size() == 0){
				$('<div id="google-map" class="hide google-map"></div>').appendTo('body');
			}
		},
		componentDidMount: function(){
			$(document).one('open.zf.reveal', $.proxy(this.initGoogleMap, this));
			$(document).one('closed.zf.reveal', $.proxy(this.componentWillUnmount, this));
		},
		componentWillUnmount: function(){
			try {
				$('.reveal').removeClass('google-map-pop');
				_.forEach(googleItems, function (item) {
					item.setMap(null);
				});
				rectangle && rectangle.setMap(null);
				drawingManager && drawingManager.setMap(null);
				google.maps.event.clearInstanceListeners(googleMap);
				rectangle = null;
				drawingManager = null;
			} catch (ex) {
				console.log(ex);
			}
			$('#google-map').addClass('hide');
		},
		initGoogleMap: function(){
			console.log('init google map');
			var container = $(window);
			$('#google-map').removeClass('hide').css({
				'width': container.width() + 'px',
				'height': container.height() + 'px',
				'top': '0px',
				'left': '0px'
			});
			
			$('.reveal').addClass('google-map-pop');
			
			if(!googleMap){
				googleMap = new google.maps.Map($('#google-map')[0], {
			        center: new google.maps.LatLng(40.744556, -73.987378),
			        zoom: 18,
			        mapTypeId: google.maps.MapTypeId.ROADMAP
			    });
			}

			google.maps.event.trigger(googleMap, "resize");

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

			var boundary = this.props.boundary,
				fillColor = this.props.color,
				mapBounds = new google.maps.LatLngBounds();
				polygon = new google.maps.Polygon({
		            paths: boundary,
		            strokeColor: '#000',
		            strokeOpacity: 1,
		            strokeWeight: 6,
		            fillColor: fillColor,
		            fillOpacity: 0.2,
		            map: googleMap
		        });
		    googleItems.push(polygon);
		    _.forEach(boundary, function(i){
		    	var point = new google.maps.LatLng(i.lat, i.lng);
            	mapBounds.extend(point);
		    });

			googleMap.fitBounds(mapBounds);
		},
		onExistMapDraw: function(){
			this.setState({'activeButton': 'ExistMapDraw'});
			drawingManager.setDrawingMode(null);
		},
		onEnterMapDraw: function(){
			this.setState({'activeButton': 'EnterMapDraw'});
			drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
		},
		onFinish: function(){
			if(!rectangle){
				return;
			}
			var bounds = rectangle.getBounds();
			this.publish('print.mapzoom@' + this.props.sourceKey, bounds);
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
						<button className={this.state.activeButton == 'EnterMapDraw' ? 'button active' : 'button'} onClick={this.onEnterMapDraw}><i className="fa fa-crop"></i>&nbsp;Begin Draw</button>
						<button className={this.state.activeButton == 'ExistMapDraw' ? 'button active' : 'button'}  onClick={this.onExistMapDraw}><i className="fa fa-arrows"></i>&nbsp;End Draw</button>
						<button className='button'  onClick={this.onFinish}><i className="fa fa-image"></i>&nbsp;Create Capture</button>
					</div>
				</div>
			);
		}
	});
});