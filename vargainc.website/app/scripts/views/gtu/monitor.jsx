define([
	'underscore',
	'react',
	'views/base',

	'react.backbone',
	'async!http://maps.googleapis.com/maps/api/js?key=AIzaSyBOhGHk0xE1WEG0laS4QGRp_AGhtB5LMHw&libraries=drawing'
], function (_, React, BaseView) {
	var googleMap = window.GoogleMap,
		googleItems = window.GoogleItems || [];

	return React.createBackboneClass({
		mixins: [
			BaseView
		],
		getInitialState: function(){
			return {
				activeGtu: null
			};
		},
		componentWillMount: function(){
			if($('#google-map').size() == 0){
				$('<div id="google-map" class="google-map"></div>').appendTo('body');
			}
		},
		componentWillUnmount: function(){
			try {
				_.forEach(googleItems, function (item) {
					item.setMap(null);
				});
				google.maps.event.clearInstanceListeners(googleMap);
			} catch (ex) {
				console.log('gtu monitor componentWillUnmount error', ex);
			}
			$('#google-map').css({
				'visibility': 'hidden',
			});
		},
		componentDidMount: function(){
			this.publish('showLoading');
			this.initGoogleMap();
			this.drawDmapBoundary();
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
				googleMap.setMapTypeId(google.maps.MapTypeId.ROADMAP);
			}
			/**
			 * position google map to main area
			 */
			var mapPosition = $(this.refs.mapArea).offset();
			console.log(mapPosition);
			$('#google-map').css({
				'top': mapPosition.top,
				'left': mapPosition.left
			});
			google.maps.event.trigger(googleMap, "resize");
			this.publish('hideLoading');
		},
		drawDmapBoundary: function(){
			var boundary = this.props.dmap.get('Boundary'),
				fillColor = this.props.dmap.get('Color'),
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
		},
		render: function () {
			var gtuList = this.props.gtu.models || []; 
			return (
				<div className="section row">
					<div className="small-12 columns">
						<div className="section-header">
							<div className="row">
								<div className="small-12 column"><h5>GTU Monitor</h5></div>
							</div>
						</div>
					</div>
					<div className="small-12 columns">
						<div className="menu-centered">
							<ul className="menu">
							{gtuList.map(function(gtu){
								return (
									<li key={gtu.get('Id')}>
										<a><i className="" style={{backgroundColor: gtu.get('UserColor')}}></i>{gtu.get('ShortUniqueID')}</a>
									</li>
								);
							})}	
							</ul>
						</div>
					</div>
					<div className="small-12 columns" ref="mapArea">

					</div>
				</div>
			);
		}
	});
});