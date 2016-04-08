define(['jquery', 'underscore', 'sprintf', 'moment', 'react', 'react-dom-server', 'views/base', 'views/mapBase', 'react.backbone', 'select2', 'markerclusterer'], function ($, _, helper, moment, React, ReactDOMServer, BaseView, MapBaseView) {
	var reloadTimeout = null,
	    gtuManager = null,
	    refreshTimeout = null,
	    infowindow;

	return React.createBackboneClass({
		mixins: [BaseView, MapBaseView],
		getInitialState: function () {
			return {
				disableDefaultUI: false,
				scrollwheel: true,
				disableDoubleClickZoom: false
			};
		},
		onWindowResize: function () {
			var pageLeftHeight = $(window).height() - $(this.refs.mapArea).position().top;
			$('#google-map').css({
				'position': 'relative',
				'top': 0,
				'left': 0,
				'right': 'auto',
				'bottom': 'auto',
				'width': '100%',
				'height': pageLeftHeight
			});
			google.maps.event.trigger(this.getGoogleMap(), "resize");
		},
		componentDidMount: function () {
			/**
    * position google map to main area
    */
			$(window).on('resize', $.proxy(this.onWindowResize, this));
			this.onWindowResize();
			this.drawGtu();
			var collection = this.getCollection();
			sorted = _.sortBy(collection.models, function (m) {
				var seed = 0;
				if (!m.get('HaveLocation')) {
					seed += 10000000;
				}
				if (m.get('IsAssign')) {
					seed += 1000000;
				}
				seed + parseInt(m.get('Id'));
				return seed;
			});
			var sourceData = _.map(sorted, function (model) {
				return {
					id: model.get('Id'),
					text: model.get('Code')
				};
			});
			sourceData = _.concat({ id: -1, text: 'Please select GTU to show on map' }, sourceData);
			$('#gtuFilter').select2({
				placeholder: 'Please input the GTU code to search',
				tags: false,
				multiple: false,
				data: sourceData
			});
			var googleMap = this.getGoogleMap();
			$('#gtuFilter').on('change', $.proxy(function () {
				var selectedGTU = $('#gtuFilter').val();
				if (selectedGTU > 0) {
					this.gotoMarker(parseInt(selectedGTU));
				}
			}, this));
			infowindow = new google.maps.InfoWindow();
		},
		showGTUInfo: function (marker) {
			var googleMap = this.getGoogleMap();
			infowindow.setContent(this.buildContent(marker.gtuId));
			infowindow.open(googleMap, marker);
		},
		gotoMarker: function (showGtu) {
			var googleMap = this.getGoogleMap(),
			    allMarkers = gtuManager.getMarkers(),
			    bounds = new google.maps.LatLngBounds(),
			    marker = _.find(allMarkers, function (m) {
				return m.gtuId == showGtu;
			}),
			    self = this;
			console.log('search', showGtu, marker.gtuId, marker.getPosition().lat(), marker.getPosition().lng());
			if (marker) {
				googleMap.setCenter(marker.getPosition());
				googleMap.setZoom(22);
				setTimeout(function () {
					self.showGTUInfo(marker);
				}, 1000);
			} else {
				this.publish('showDialog', "You searched GTU do not find the last position");
			}
		},
		componentWillUnmount: function () {
			try {
				this.clearMap();
				_.forEach(gtuLocation, function (item) {
					item.setMap(null);
				});
				google.maps.event.clearInstanceListeners(googleMap);
			} catch (ex) {
				console.log('google map clear error', ex);
			}
			$('#google-map').css({
				'visibility': 'hidden'
			});
		},
		buildContent: function (gtuId) {
			var gtu = this.getCollection().get(gtuId);
			var template = React.createElement(
				'ul',
				{ className: 'menu vertical' },
				React.createElement(
					'li',
					null,
					React.createElement(
						'h5',
						null,
						gtu.get('Code')
					)
				),
				React.createElement(
					'li',
					null,
					'IsAssign: ',
					gtu.get('IsAssign') ? 'YES' : 'NO'
				),
				React.createElement(
					'li',
					null,
					'IsOnline: ',
					gtu.get('IsOnline') ? 'YES' : 'NO'
				),
				React.createElement(
					'li',
					{ style: { display: gtu.get('IsAssign') ? 'inherit' : 'none' } },
					'Team: ',
					gtu.get('Company')
				),
				React.createElement(
					'li',
					{ style: { display: gtu.get('IsAssign') ? 'inherit' : 'none' } },
					'Auditor: ',
					gtu.get('Auditor')
				),
				React.createElement(
					'li',
					{ style: { display: gtu.get('IsAssign') ? 'inherit' : 'none' } },
					'Role: ',
					gtu.get('Role')
				)
			);
			return ReactDOMServer.renderToStaticMarkup(template);
		},
		drawGtu: function () {
			var googleMap = this.getGoogleMap(),
			    gtu = this.getCollection(),
			    bounds = new google.maps.LatLngBounds(),
			    markers = [],
			    self = this;

			_.forEach(gtu.models, function (i) {
				if (i.get('HaveLocation')) {
					var latlng = new google.maps.LatLng(i.get('Latitude'), i.get('Longitude'));
					bounds.extend(latlng);
					var marker = new google.maps.Marker({
						'position': latlng
					});
					marker.gtuId = i.get('Id');
					marker.addListener('click', function () {
						self.showGTUInfo(marker);
					});
					markers.push(marker);
				}
			});
			gtuManager = new MarkerClusterer(googleMap, markers);
			googleMap.fitBounds(bounds);
		},
		render: function () {
			return React.createElement(
				'div',
				null,
				React.createElement(
					'div',
					{ className: 'section row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'div',
							{ className: 'section-header' },
							React.createElement(
								'div',
								{ className: 'small-10 small-centered columns' },
								React.createElement('select', { id: 'gtuFilter' })
							)
						)
					)
				),
				React.createElement('div', { ref: 'mapArea' })
			);
		}
	});
});
