define(['jquery', 'underscore', 'sprintf', 'moment', 'react', 'collections/user', 'views/base', 'views/mapBase', 'views/gtu/assign', 'react.backbone'], function ($, _, helper, moment, React, UserCollection, BaseView, MapBaseView, AssignView) {
	var dmapPolygon = null,
	    dmapBounds = null,
	    gtuData = null,
	    gtuPoints = [],
	    gtuLocation = [];

	return React.createBackboneClass({
		mixins: [BaseView, MapBaseView],
		getInitialState: function () {
			return {
				disableDefaultUI: false,
				scrollwheel: false,
				disableDoubleClickZoom: true,
				activeGtu: null,
				customPoints: [],
				maxDisplayCount: 2000,
				gtuQueryTime: moment()
			};
		},
		componentDidMount: function () {
			this.publish('showLoading');
			/**
    * position google map to main area
    */
			var size = $(this.refs.mapArea).width();
			$('#google-map').css({
				'position': 'relative',
				'top': 0,
				'left': 0,
				'right': 'auto',
				'bottom': 'auto',
				'width': '100%',
				'height': size
			});
			google.maps.event.trigger(this.getGoogleMap(), "resize");

			this.setState({
				activeGtu: _.find(this.props.gtu, { IsAssign: true })
			});

			var self = this,
			    googleMap = this.getGoogleMap();
			this.drawDmapBoundary().then(this.filterGtu).then(this.drawGtu).done(function () {
				googleMap.addListener('zoom_changed', $.proxy(self.drawGtu, self));
				googleMap.addListener('dragend', $.proxy(self.drawGtu, self));
				self.publish('hideLoading');
			});
			window.setInterval(this.reload, 1 * 60 * 1000);
		},
		componentWillUnmount: function () {
			try {
				_.forEach(gtuPoints, function (item) {
					item.setMap(null);
				});
				_.forEach(gtuLocation, function (item) {
					item.setMap(null);
				});
				dmapPolygon.setMap(null);
				google.maps.event.clearInstanceListeners(googleMap);
			} catch (ex) {
				console.log('google map clear error', ex);
			}
			$('#google-map').css({
				'visibility': 'hidden'
			});
		},
		setActiveGtu: function (gtuId) {
			var activeGut = this.props.gtu.get(gtuId);
			this.setState({ activeGtu: activeGut });
		},
		getCirclePath: function (size) {
			return helper.sprintf('M-%d,0a%d,%d 0 1,0 %d,0a%d,%d 0 1,0 -%d,0', size, size, size, size * 2, size, size, size * 2);
		},
		drawDmapBoundary: function () {
			var def = $.Deferred(),
			    boundary = this.props.dmap.get('Boundary'),
			    fillColor = this.props.dmap.get('Color'),
			    googleMap = this.getGoogleMap(),
			    timeout = null;

			dmapBounds = new google.maps.LatLngBounds();
			dmapPolygon = new google.maps.Polygon({
				paths: boundary,
				strokeColor: '#000',
				strokeOpacity: 1,
				strokeWeight: 6,
				fillColor: fillColor,
				fillOpacity: 0.1,
				map: googleMap
			});

			_.forEach(boundary, function (i) {
				var point = new google.maps.LatLng(i.lat, i.lng);
				dmapBounds.extend(point);
			});
			google.maps.event.addListenerOnce(googleMap, 'tilesloaded', function () {
				window.clearTimeout(timeout);
				def.resolve();
			});
			googleMap.fitBounds(dmapBounds);
			timeout = window.setTimeout(function () {
				def.resolve();
			}, 5 * 60 * 1000);
			return def;
		},
		filterGtu: function (fnFilter) {
			var def = $.Deferred(),
			    dots = this.props.dmap.get('Gtu') || [],
			    result = [];
			_.forEach(dots, function (gtu) {
				var filterGtu = _.groupBy(gtu.points, fnFilter);
				_.forEach(filterGtu, function (v, k) {
					var latlng = k.split(':');
					result.push({ lat: parseFloat(latlng[0]), lng: parseFloat(latlng[1]), color: gtu.color });
				});
			});
			return result;
		},
		prepareGtu: function () {
			var gtu3 = this.filterGtu(function (latlng) {
				return Math.round(latlng.lat * 1000) / 1000 + ':' + Math.round(latlng.lng * 1000) / 1000;
			}),
			    gtu4 = this.filterGtu(function (latlng) {
				return Math.round(latlng.lat * 10000) / 10000 + ':' + Math.round(latlng.lng * 10000) / 10000;
			}),
			    gtu5 = this.filterGtu(function (latlng) {
				return Math.round(latlng.lat * 100000) / 100000 + ':' + Math.round(latlng.lng * 100000) / 100000;
			});
			gtuData = [gtu3, gtu4, gtu5];

			console.log("precision 1000   gut:", gtu3.length);
			console.log("precision 10000  gut:", gtu4.length);
			console.log("precision 100000 gut:", gtu5.length);
		},
		drawGtu: function () {
			var def = $.Deferred(),
			    self = this,
			    maxDisplayCount = this.state.maxDisplayCount,
			    lastDisplayGtuIndex = this.state.lastDisplayGtuIndex,
			    lastViewArea = this.state.lastViewArea,
			    lastZoomLevel = this.state.lastZoomLevel,
			    newDisplayGtuIndex = null,
			    googleMap = this.getGoogleMap(),
			    viewArea = googleMap.getBounds(),
			    zoomLevel = googleMap.getZoom(),
			    cutDownNumber = 5,
			    cutDown = function () {
				cutDownNumber--;
				if (cutDownNumber > 0) {
					setTimeout(cutDown, 100);
				} else {
					def.resolve();
				}
			},
			    filterGtus,
			    point;

			gtuData || this.prepareGtu();
			console.log(lastDisplayGtuIndex, lastZoomLevel, zoomLevel);
			if (typeof lastDisplayGtuIndex === 'undefined' || lastZoomLevel != zoomLevel) {
				_.forEach(gtuData, function (points, index) {
					var visiableGtu = _.filter(points, function (latlng) {
						return viewArea.contains(new google.maps.LatLng(latlng.lat, latlng.lng));
					});

					if (!filterGtus || visiableGtu.length <= maxDisplayCount) {
						filterGtus = visiableGtu;
						newDisplayGtuIndex = index;
					} else {
						return false;
					}
					console.log(visiableGtu.length + ' / ' + points.length + ' dots in current zoom level ' + googleMap.getZoom() + ' view area');
				});
			} else {
				filterGtus = _.filter(gtuData[lastDisplayGtuIndex], function (latlng) {
					return viewArea.contains(new google.maps.LatLng(latlng.lat, latlng.lng));
				});
				newDisplayGtuIndex = lastDisplayGtuIndex;
			}

			if (lastDisplayGtuIndex == newDisplayGtuIndex) {
				var tempPoints = [];
				_.forEach(gtuPoints, function (item) {
					if (!viewArea.contains(item.getPosition())) {
						item.setMap(null);
					} else {
						tempPoints.push(item);
					}
				});
				gtuPoints = tempPoints;
			} else {
				_.forEach(gtuPoints, function (item) {
					item.setMap(null);
				});
				gtuPoints = [];
				lastViewArea = null;
			}

			_.forEach(filterGtus, function (gtu) {
				if (lastViewArea && lastViewArea.contains(new google.maps.LatLng(gtu.lat, gtu.lng))) {
					return true;
				}
				point = new google.maps.Marker({
					position: {
						lat: gtu.lat,
						lng: gtu.lng
					},
					icon: {
						path: self.getCirclePath(5),
						fillColor: gtu.color,
						fillOpacity: 1,
						strokeOpacity: 1,
						strokeWeight: 1,
						strokeColor: '#000'
					},
					draggable: false,
					map: googleMap
				});
				gtuPoints.push(point);
			});
			this.setState({
				lastDisplayGtuIndex: newDisplayGtuIndex,
				lastViewArea: viewArea,
				lastZoomLevel: zoomLevel
			});
			cutDown();
		},
		drawLastLocation: function () {
			var googleMap = this.getGoogleMap(),
			    point,
			    gtuList = this.props.gtu.where({
				IsAssign: true,
				IsOnline: true
			});
			_.forEach(gtuLocation, function (p) {
				p.setMap(null);
			});
			_.forEach(gtuList, function (gtu) {
				point = new google.maps.Marker({
					position: {
						lat: gtu.lat,
						lng: gtu.lng
					},
					draggable: false,
					map: googleMap
				});
				gtuLocation.push(point);
			});
		},
		reload: function () {
			var dmap = this.props.dmap,
			    gtu = this.props.gtu,
			    taskId = this.props.task.get('Id'),
			    self = this;
			$.when(dmap.updateGtuAfterTime(this.state.gtuQueryTime), gtu.fetchGtuLocation(taskId)).done(function () {
				self.setState({
					gtuQueryTime: moment()
				});
				self.prepareGtu();
				self.drawGtu();
				self.drawLastLocation();
			});
		},
		drawLastPosition: function () {},
		onReCenter: function () {
			var googleMap = this.getGoogleMap();
			googleMap.setCenter(dmapBounds.getCenter());
			this.drawGtu();
		},
		onAssign: function () {
			var gtu = this.props.gtu,
			    taskId = this.props.task.get('Id'),
			    user = new UserCollection(),
			    self = this;
			user.fetchForGtu().done(function () {
				self.publish('showDialog', AssignView, {
					collection: gtu,
					user: user,
					taskId: taskId
				}, {
					size: 'full'
				});
			});
		},
		onStart: function () {
			var model = this.props.task;
			model.setStart();
		},
		onStop: function () {
			var model = this.props.task;
			model.setStop();
		},
		onPause: function () {
			var model = this.props.task;
			model.setPause();
		},
		renderGtu: function (gtu) {
			switch (gtu.get('Role')) {
				case 'Auditor':
					gtuIcon = 'fa fa-group';
					break;
				case 'Driver':
					gtuIcon = 'fa fa-truck';
					break;
				case 'Walker':
					gtuIcon = 'fa fa-street-view';
					break;
				default:
					gtuIcon = 'fa fa-stop';
					break;
			}
			if (gtu.get('IsOnline')) {
				return React.createElement(
					'div',
					{ className: 'columns', key: gtu.get('Id') },
					React.createElement(
						'button',
						{ className: 'button text-left' },
						React.createElement('i', { className: gtuIcon, style: { color: gtu.get('UserColor') } }),
						'  ',
						gtu.get('ShortUniqueID')
					)
				);
			} else {
				return React.createElement(
					'div',
					{ className: 'columns', key: gtu.get('Id') },
					React.createElement(
						'button',
						{ className: 'button text-left', disabled: true },
						React.createElement('i', { className: gtuIcon, style: { color: gtu.get('UserColor') } }),
						'  ',
						gtu.get('ShortUniqueID')
					)
				);
			}
		},
		renderController: function () {
			var task = this.props.task;
			switch (task.get('Status')) {
				case 0:
					//started
					return React.createElement(
						'div',
						null,
						React.createElement(
							'button',
							{ className: 'button', onClick: this.onPause },
							React.createElement('i', { className: 'fa fa-pause' }),
							'Pause'
						),
						React.createElement(
							'button',
							{ className: 'button', onClick: this.onStop },
							React.createElement('i', { className: 'fa fa-stop' }),
							'Stop'
						)
					);
					break;
				case 1:
					//stoped
					return "STOPPED";
					break;
				case 2:
					//peased
					return React.createElement(
						'div',
						null,
						React.createElement(
							'button',
							{ className: 'button', onClick: this.onStart },
							React.createElement('i', { className: 'fa fa-play' }),
							'Start'
						),
						React.createElement(
							'button',
							{ className: 'button', onClick: this.onStop },
							React.createElement('i', { className: 'fa fa-stop' }),
							'Stop'
						)
					);
					break;
				default:
					return React.createElement(
						'div',
						null,
						React.createElement(
							'button',
							{ className: 'button', onClick: this.onStart },
							React.createElement('i', { className: 'fa fa-play' }),
							'Start'
						)
					);
					break;
			}
		},
		render: function () {
			var self = this,
			    gtuList = this.props.gtu.where({
				IsAssign: true
			});
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
								{ className: 'row' },
								React.createElement(
									'div',
									{ className: 'small-12 column' },
									React.createElement(
										'h5',
										null,
										'GTU Monitor - ',
										this.props.task.get('Name')
									)
								),
								React.createElement(
									'div',
									{ className: 'small-12 column' },
									React.createElement(
										'nav',
										{ 'aria-label': 'You are here:', role: 'navigation' },
										React.createElement(
											'ul',
											{ className: 'breadcrumbs' },
											React.createElement(
												'li',
												null,
												React.createElement(
													'a',
													{ href: '#' },
													'Control Center'
												)
											),
											React.createElement(
												'li',
												null,
												React.createElement(
													'a',
													{ href: '#report/' + this.props.task.get('Id') },
													'Report'
												)
											),
											React.createElement(
												'li',
												null,
												React.createElement(
													'span',
													{ className: 'show-for-sr' },
													'Current: '
												),
												' GTU Monitor'
											)
										)
									)
								)
							)
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 medium-7 large-9 columns' },
						this.renderController()
					),
					React.createElement(
						'div',
						{ className: 'small-12 medium-5 large-3 columns' },
						React.createElement(
							'button',
							{ className: 'button float-right', onClick: this.reload },
							React.createElement('i', { className: 'fa fa-update' }),
							'Update GTU'
						),
						React.createElement(
							'button',
							{ className: 'button float-right', onClick: this.onReCenter },
							React.createElement('i', { className: 'fa fa-refresh' }),
							'ReCenter'
						),
						React.createElement(
							'button',
							{ className: 'button float-right', onClick: this.onAssign },
							React.createElement('i', { className: 'fa fa-plus' }),
							'Assign GTU'
						)
					)
				),
				React.createElement(
					'div',
					{ className: 'row gtu small-up-8' },
					gtuList.map(function (gtu) {
						return self.renderGtu(gtu);
					})
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement('div', { className: 'small-12 columns', ref: 'mapArea' })
				)
			);
		}
	});
});
