define([
	'jquery',
	'underscore',
	'fastMarker'
], function ($, _, FastMarker) {
	var googleMap = window.GoogleMap,
		googleItems = window.GoogleItems || [];
	return {
		getGoogleMap: function () {
			return googleMap;
		},
		getGoogleItems: function () {
			return googleItems;
		},
		showMap: function(){
			$('#google-map-wrapper').css({
				'visibility': 'visible'
			});
			google.maps.event.trigger(googleMap, 'resize');
		},
		hideMap: function(){
			$('#google-map-wrapper').css({
				'visibility': 'hidden'
			});
			google.maps.event.trigger(googleMap, 'resize');
		},
		setMapHeight: function(height){
			$('#google-map-wrapper').height(height);
			google.maps.event.trigger(googleMap, 'resize');
		},
		componentWillUnmount: function () {
			console.log('map base clearMap');
			try {
				_.forEach(googleItems, function (item) {
					item && item.setMap && item.setMap(null);
				});
				google.maps.event.clearInstanceListeners(googleMap);
			} catch (ex) {
				console.log('google map clear error', ex);
			}
			$('#google-map-wrapper').css({
				'visibility': 'hidden',
			});
		},
		componentDidMount: function () {
			console.log('init google map');
			var def = $.Deferred(),
				mapType = this.state.mapType && google.maps.MapTypeId[this.state.mapType] ? google.maps.MapTypeId[this.state.mapType] : google.maps.MapTypeId.ROADMAP,
				disableDefaultUI = _.has(this.state, 'disableDefaultUI') ? this.state.disableDefaultUI : false,
				scrollwheel = _.has(this.state, 'scrollwheel') ? this.state.scrollwheel : true,
				disableDoubleClickZoom = _.has(this.state, 'disableDoubleClickZoom') ? this.state.disableDoubleClickZoom : false;
			if (this.state.mapStyle) {
				$('#google-map-wrapper').css(this.state.mapStyle);
			} else {
				$('#google-map-wrapper').css({
					'visibility': 'visible',
					'position': '',
					'width': '',
					'height': '',
					'left': '',
					'top': '',
					'right': '',
					'bottom': ''
				});
			}

			if (!googleMap) {
				googleMap = new google.maps.Map($('#google-map')[0], {
					center: new google.maps.LatLng(40.744556, -73.987378),
					zoom: 18,
					disableDefaultUI: disableDefaultUI,
					streetViewControl: false,
					zoomControlOptions: {
						position: google.maps.ControlPosition.RIGHT_TOP
					},
					scrollwheel: scrollwheel,
					disableDoubleClickZoom: disableDoubleClickZoom,
					mapTypeId: mapType
				});
			} else {
				googleMap.setMapTypeId(mapType);
				googleMap.setOptions({
					disableDefaultUI: disableDefaultUI,
					streetViewControl: false,
					scrollwheel: scrollwheel,
					disableDoubleClickZoom: disableDoubleClickZoom,
					zoomControlOptions: {
						position: google.maps.ControlPosition.RIGHT_TOP
					}
				});
			}
			google.maps.event.trigger(googleMap, 'resize');
		}
	};
});