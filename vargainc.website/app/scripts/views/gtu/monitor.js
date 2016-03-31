define(['jquery', 'underscore', 'sprintf', 'moment', 'react', 'collections/user', 'models/user', 'views/base', 'views/mapBase', 'views/gtu/assign', 'views/user/employee', 'react.backbone'], function ($, _, helper, moment, React, UserCollection, UserModel, BaseView, MapBaseView, AssignView, EmployeeView) {
	var reloadTimeout = null,
	    dmapPolygon = null,
	    dmapBounds = null,
	    gtuData = [],
	    gtuPoints = {},
	    //{key: 'lat:lng', point: google.map.marker}
	gtuLocation = {},
	    gtuTrack = {},
	    //{key: 'gtu id', {track: google.map.polyline, startPoint: google.map.marker}
	startPointIcon = 'M1.8-25.1c-0.3-0.3-0.7-0.5-1.2-0.5c-0.4,0-0.8,0.2-1.2,0.5C-1-24.8-1-24.4-1-23.9c0,0.4,0.1,0.8,0.4,1c0.3,0.3,0.4,0.6,0.4,1v21.4c0,0.1,0,0.2,0.1,0.3C0,0,0.1,0,0.2,0H1c0.1,0,0.2,0,0.3-0.1s0.1-0.2,0.1-0.3v-21.4c0-0.4,0.2-0.7,0.4-1s0.4-0.6,0.4-1C2.3-24.4,2.1-24.8,1.8-25.1z M16.8-23.7c-0.2-0.2-0.4-0.2-0.6-0.2c-0.1,0-0.3,0.1-0.7,0.3c-0.4,0.2-0.6,0.4-1,0.5c-0.1,0,0.8,0.1,0.7,0.1c-0.4,0.1-0.8,0.3-1.3,0.5s-1,0.3-1.5,0.3c-0.4,0-0.8-0.1-1.1-0.2c-1.1-0.5-1-0.9-1.8-1.1c-0.8-0.3-1.7-0.4-2.6-0.4c-1.6,0-1.4,0.5-3.4,1.5c-0.5,0.2-0.8,0.4-1,0.5c-0.3,0.2-0.4,0.4-0.4,0.7v7.4c0,0.2,0.1,0.4,0.2,0.6S2.8-13,3-13c0.1,0,0.3,0,0.4-0.1C5.7-14.3,5.7-15,7.3-15c0.6,0,1.2,0.1,1.8,0.3s1.1,0.4,1.5,0.6c0.4,0.2-0.1,0.4,0.4,0.6c0.5,0.2,1.1,0.3,1.6,0.3c1.3,0,1.9-0.5,3.7-1.5c0.2-0.1,0.4-0.2,0.5-0.4c0.1-0.1,0.2-0.3,0.2-0.5v-7.5C17-23.4,16.9-23.5,16.8-23.7z',
	    walkerIcon = 'M-9-0.2c0,0.5,0.3,1,0.8,1.4S-7,2-6.1,2.2c0.9,0.3,1.8,0.4,2.8,0.6C-2.3,2.9-1.2,3-0.1,3S2,2.9,3.1,2.8c1-0.1,2-0.3,2.8-0.6c0.9-0.3,1.5-0.6,2-1s0.8-0.9,0.8-1.4c0-0.4-0.1-0.8-0.4-1.1C8-1.6,7.6-1.9,7.2-2.1c-0.5-0.2-1-0.4-1.5-0.6C5.2-2.8,4.7-3,4.1-3.1c-0.2,0-0.4,0-0.6,0.1C3.3-2.9,3.2-2.7,3.2-2.5s0,0.4,0.1,0.6s0.3,0.3,0.5,0.3c0.5,0.1,0.9,0.2,1.3,0.3c0.4,0.1,0.7,0.2,1,0.3c0.2,0.1,0.4,0.2,0.6,0.3C6.9-0.6,7-0.5,7-0.5c0.1,0.1,0.1,0.1,0.1,0.1c0,0.1-0.1,0.2-0.3,0.3C6.6,0,6.3,0.2,5.9,0.3S5,0.6,4.5,0.7S3.3,0.9,2.5,1C1.7,1.1,0.9,1.1,0,1.1s-1.7,0-2.5-0.1s-1.5-0.2-2-0.3s-1-0.3-1.4-0.4C-6.3,0.2-6.6,0-6.8-0.1s-0.3-0.2-0.3-0.3c0,0,0-0.1,0.1-0.1c0.1-0.1,0.2-0.1,0.3-0.2S-6.3-0.9-6.1-1c0.2-0.1,0.6-0.2,1-0.3c0.4-0.1,0.8-0.2,1.3-0.3c0.2,0,0.4-0.1,0.5-0.3c0.1-0.2,0.2-0.4,0.1-0.6c0-0.2-0.1-0.4-0.3-0.5c-0.2-0.1-0.4-0.2-0.6-0.1C-4.7-3-5.2-2.9-5.7-2.7s-1,0.3-1.5,0.6c-0.5,0.2-0.9,0.5-1.1,0.8S-9-0.6-9-0.2z M-4.2-11.4v4.8C-4.2-6.4-4.1-6.2-4-6c0.2,0.2,0.3,0.2,0.6,0.2h0.8V-1c0,0.2,0.1,0.4,0.2,0.6c0.2,0.2,0.3,0.2,0.6,0.2h3.2c0.2,0,0.4-0.1,0.6-0.2C2.2-0.6,2.2-0.7,2.2-1v-4.8H3c0.2,0,0.4-0.1,0.6-0.2c0.2-0.2,0.2-0.3,0.2-0.6v-4.8c0-0.4-0.2-0.8-0.5-1.1S2.6-13,2.2-13h-4.8c-0.4,0-0.8,0.2-1.1,0.5S-4.2-11.8-4.2-11.4z M-3-16.2c0,0.8,0.3,1.4,0.8,2s1.2,0.8,2,0.8s1.4-0.3,2-0.8s0.8-1.2,0.8-2c0-0.8-0.3-1.4-0.8-2c-0.5-0.6-1.2-0.8-2-0.8s-1.4,0.3-2,0.8S-3-17-3-16.2z',
	    driverIcon = 'M-10.8,1.3c-0.3,0.3-0.7,0.5-1.1,0.5c-0.4,0-0.8-0.2-1.1-0.5s-0.5-0.7-0.5-1.1s0.2-0.8,0.5-1.1s0.7-0.5,1.1-0.5c0.4,0,0.8,0.2,1.1,0.5c0.3,0.3,0.5,0.7,0.5,1.1C-10.3,0.7-10.4,1.1-10.8,1.3z M-15.2-6.6c0-0.1,0-0.2,0.1-0.3l2.5-2.5c0.1,0,0.2-0.1,0.3-0.1h2v3.3h-4.9V-6.6z M0.7,1.3C0.4,1.6-0.1,1.8-0.4,1.8s-0.8-0.2-1.1-0.5s-0.5-0.7-0.5-1.1s0.2-0.8,0.5-1.1c0.3-0.3,0.7-0.5,1.1-0.5s0.8,0.2,1.1,0.5c0.3,0.3,0.5,0.7,0.5,1.1C1.1,0.7,1,1.1,0.7,1.3z M4.2-14.1c-0.2-0.2-0.4-0.3-0.6-0.3h-13c-0.3,0-0.5,0.1-0.6,0.3c-0.1,0.2-0.3,0.3-0.3,0.6v2.4h-2c-0.2,0-0.5,0.1-0.7,0.2c-0.3,0.1-0.5,0.2-0.7,0.4l-2.5,2.5c-0.1,0.1-0.2,0.2-0.3,0.4c-0.1,0.1-0.1,0.2-0.2,0.4c0,0.1-0.1,0.3-0.1,0.5s0,0.3,0,0.4c0,0.1,0,0.3,0,0.5s0,0.4,0,0.4v4.1c-0.2,0-0.4,0.1-0.6,0.2s-0.2,0.4-0.2,0.6c0,0.1,0,0.2,0.1,0.3c0,0.1,0.1,0.2,0.2,0.2s0.2,0.1,0.2,0.1s0.2,0.1,0.3,0.1s0.2,0,0.3,0c0.1,0,0.2,0,0.3,0s0.3,0,0.3,0h0.8c0,0.9,0.3,1.7,1,2.3s1.4,1,2.3,1s1.7-0.3,2.3-1c0.6-0.6,1-1.4,1-2.3h4.9c0,0.9,0.3,1.7,1,2.3s1.4,1,2.3,1s1.7-0.3,2.3-1c0.6-0.6,1-1.4,1-2.3c0,0,0.1,0,0.3,0c0.2,0,0.3,0,0.3,0s0.1,0,0.3,0c0.1,0,0.2,0,0.3-0.1c0.1,0,0.1-0.1,0.2-0.1s0.1-0.1,0.2-0.2c0-0.1,0.1-0.2,0.1-0.3v-13C4.4-13.9,4.3-14,4.2-14.1z';

	return React.createBackboneClass({
		mixins: [BaseView, MapBaseView],
		getInitialState: function () {
			return {
				disableDefaultUI: false,
				scrollwheel: false,
				disableDoubleClickZoom: true,
				activeGtu: [],
				ShowOutOfBoundary: false,
				displayMode: 'cover', //cover: show gtu dots, track: show gtu path
				customPoints: [],
				maxDisplayCount: 1000,
				canDrawMap: false,
				lastDisplayGtuIndex: null
			};
		},
		onWindowResize: function () {
			console.log('window resize');
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
			this.publish('showLoading');
			/**
    * position google map to main area
    */
			$(window).on('resize', $.proxy(this.onWindowResize, this));
			this.onWindowResize();

			var self = this,
			    googleMap = this.getGoogleMap();

			this.drawDmapBoundary().then(this.filterGtu).then(this.drawGtu).done(function () {
				googleMap.addListener('zoom_changed', $.proxy(self.drawGtu, self));
				googleMap.addListener('dragend', $.proxy(self.drawGtu, self));
				self.drawLastLocation();
			}).done(function () {
				var allAssignedGtu = _.map(self.props.gtu.where(function (i) {
					return i.get('IsAssign') || i.get('WithData');
				}), function (gtu) {
					return gtu.get('Id');
				});
				self.setState({
					activeGtu: allAssignedGtu,
					canDrawMap: true
				});
				self.publish('hideLoading');
			});
			window.setInterval(this.reload, 0.5 * 60 * 1000);
			$("#monitor-more-menu").foundation();
		},
		componentWillUnmount: function () {
			try {
				this.clearMap();
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
		shouldComponentUpdate: function (nextProps, nextState) {
			var oldActiveGtu = nextState.activeGtu,
			    newActiveGtu = this.state.activeGtu,
			    xor = _.xor(oldActiveGtu, newActiveGtu);
			if (this.state.ShowOutOfBoundary != nextState.ShowOutOfBoundary) {
				this.reload();
			} else if (!_.isEmpty(xor)) {
				this.reload();
			} else if (this.state.displayMode != nextState.displayMode) {
				this.clearMap();
				this.reload();
			}
			return true;
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
			    needFilterOutOfBoundary = !this.state.ShowOutOfBoundary,
			    dots = this.props.dmap.get('Gtu') || [],
			    activeGtu = this.state.activeGtu,
			    result = [];

			_.forEach(dots, function (gtu) {
				if (_.indexOf(activeGtu, gtu.gtuId) == -1) {
					return true;
				}

				if (needFilterOutOfBoundary) {
					var points = _.filter(gtu.points, {
						out: false
					});
				} else {
					var points = gtu.points;
				}
				var filterGtu = _.groupBy(points, fnFilter);
				_.forEach(filterGtu, function (v, k) {
					var latlng = k.split(':');
					result.push({
						key: k,
						lat: parseFloat(latlng[0]),
						lng: parseFloat(latlng[1]),
						color: gtu.color
					});
				});
			});
			return result;
		},
		prepareGtu: function () {
			var gtu3 = this.filterGtu(function (latlng) {
				return _.round(latlng.lat, 3) + ':' + _.round(latlng.lng, 3);
			}),
			    gtu4 = this.filterGtu(function (latlng) {
				return _.round(latlng.lat, 4) + ':' + _.round(latlng.lng, 4);
			}),
			    gtu5 = this.filterGtu(function (latlng) {
				return _.round(latlng.lat, 5) + ':' + _.round(latlng.lng, 5);
			});
			gtuData = [gtu3, gtu4, gtu5];
			console.log("precision 1000   gut:", gtu3.length);
			console.log("precision 10000  gut:", gtu4.length);
			console.log("precision 100000 gut:", gtu5.length);
		},
		drawGtu: function () {
			if (this.state.displayMode != 'cover') {
				return;
			}
			this.setState({ busy: true });
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
					console.log('render gtu finished', _.keys(gtuPoints).length);
					self.setState({ busy: false });
				}
			},
			    filterGtus,
			    point;

			_.isEmpty(gtuData) && this.prepareGtu();
			//get less than max display gtu and in current view are latlng.
			if (lastDisplayGtuIndex === null || lastZoomLevel != zoomLevel) {
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
				});
			} else {
				filterGtus = _.filter(gtuData[lastDisplayGtuIndex], function (latlng) {
					return viewArea.contains(new google.maps.LatLng(latlng.lat, latlng.lng));
				});
				newDisplayGtuIndex = lastDisplayGtuIndex;
			}
			//remove not in current view are gtu pointes.
			if (lastDisplayGtuIndex == newDisplayGtuIndex) {
				var tempPoints = {};
				_.forEach(gtuPoints, function (item) {
					var position = item.getPosition();
					if (!viewArea.contains(position) || !_.some(filterGtus, {
						key: item.key
					})) {
						item.setMap(null);
					} else {
						tempPoints[item.key] = item;
					}
				});
				gtuPoints = tempPoints;
			} else {
				_.forEach(gtuPoints, function (item) {
					item.setMap(null);
				});
				gtuPoints = {};
				lastViewArea = null;
			}

			_.forEach(filterGtus, function (gtu) {
				// if (gtuPoints[gtu.key] != null && lastViewArea && lastViewArea.contains(new google.maps.LatLng(gtu.lat, gtu.lng))) {
				// 	return true;
				// }
				if (gtuPoints[gtu.key] != null) {
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
				point.key = gtu.key;
				gtuPoints[gtu.key] = point;
			});
			this.setState({
				lastDisplayGtuIndex: newDisplayGtuIndex,
				lastViewArea: viewArea,
				lastZoomLevel: zoomLevel
			});
			cutDown();
			return def;
		},
		prepareGTUTrack: function () {
			var activeGtu = this.state.activeGtu,
			    promise = [],
			    gtus = this.props.gtu.where(function (gtu) {
				return _.indexOf(activeGtu, gtu.get('Id')) > -1;
			}),
			    def = $.Deferred(),
			    self = this;
			gtus.forEach(function (gtu) {
				promise.push(gtu.getTrack(self.props.task.get('Id')).promise());
			});
			$.when(promise).done(function () {
				def.resolve();
			});
			return def;
		},
		drawGTUTrack: function () {
			if (this.state.displayMode != 'track') {
				return;
			}
			var googleMap = this.getGoogleMap(),
			    activeGtu = this.state.activeGtu,
			    gtus = this.props.gtu,
			    self = this;
			this.prepareGTUTrack().done(function () {
				//clear not in activeGtu track
				var tempTrack = {};
				_.forEach(gtuTrack, function (item, gtuId) {
					if (_.indexOf(activeGtu, gtuId) == -1) {
						item.track.setMap(null);
						item.startPoint.setMap(null);
					} else {
						tempTrack[gtuId] = item;
					}
				});
				gtuTrack = tempTrack;
				_.forEach(activeGtu, function (gtuId) {
					var gtu = gtus.get(gtuId),
					    item = gtuTrack[gtuId];
					if (item && item.track && item.startPoint) {
						item.track.setPath(gtu.get('track'));
					} else {
						var path = gtu.get('track');
						if (_.isEmpty(path)) {
							return true;
						}
						gtuTrack[gtuId] = {};
						var strokeWeight = gtu.get('Role') == 'Driver' ? 6 : 2;
						gtuTrack[gtuId].track = new google.maps.Polyline({
							path: path,
							strokeColor: gtu.get('UserColor'),
							strokeOpacity: 0.6,
							strokeWeight: strokeWeight,
							map: googleMap
						});
						gtuTrack[gtuId].startPoint = new google.maps.Marker({
							position: {
								lat: path[0].lat,
								lng: path[0].lng
							},
							icon: {
								path: startPointIcon,
								fillColor: gtu.get('UserColor'),
								fillOpacity: 1,
								strokeOpacity: 0,
								strokeColor: '#000'
							},
							draggable: false,
							map: googleMap
						});
					}
				});
			});
		},
		drawLastLocation: function () {
			var googleMap = this.getGoogleMap(),
			    point,
			    gtuList = this.props.gtu.where({
				IsAssign: true,
				IsOnline: true
			}),
			    willDrawGtu = _.map(gtuList, function (gtu) {
				return gtu.get('Id');
			}),
			    drawedGtu = _.keys(gtuLocation) || [],
			    needDeleteGtu = _.difference(drawedGtu, willDrawGtu);

			_.forEach(needDeleteGtu, function (id) {
				gtuLocation[id].setMap(null);
				delete gtuLocation[id];
			});

			_.forEach(gtuList, function (gtu) {
				if (gtu.get('Location') && gtuLocation[gtu.get('Id')]) {
					gtuLocation[gtu.get('Id')].setPoistion({
						lat: gtu.get('Location').lat,
						lng: gtu.get('Location').lng
					});
				} else {
					gtuLocation[gtu.get('Id')] = new google.maps.Marker({
						position: {
							lat: gtu.get('Location').lat,
							lng: gtu.get('Location').lng
						},
						icon: {
							path: gtu.get('Role') == 'Driver' ? driverIcon : walkerIcon,
							fillColor: gtu.get('UserColor'),
							fillOpacity: 1,
							strokeOpacity: 0,
							strokeWeight: 0
						},
						draggable: false,
						map: googleMap
					});
				}
			});
		},
		clearMap: function () {
			console.log('clear map');
			try {
				_.forEach(gtuPoints, function (item) {
					item.setMap(null);
				});

				_.forEach(gtuTrack, function (item) {
					item && item.track && item.track.setMap(null);
					item && item.startPoint && item.startPoint.setMap(null);
				});
				gtuTrack = {};
				this.setState({
					lastDisplayGtuIndex: null
				});
			} catch (ex) {
				console.log('google map clear error', ex);
			}
		},
		reload: function () {
			window.clearTimeout(reloadTimeout);
			reloadTimeout = window.setTimeout($.proxy(this._reload, this), 2 * 1000);
		},
		_reload: function () {
			var dmap = this.props.dmap,
			    gtu = this.props.gtu,
			    taskId = this.props.task.get('Id'),
			    self = this;
			if (this.state.displayMode == 'cover') {
				$.when([dmap.updateGtuAfterTime(null, {
					quite: true
				}), gtu.fetchGtuLocation(taskId, {
					quite: true
				})]).done(function () {
					self.prepareGtu();
					self.drawGtu();
					self.drawLastLocation();
				});
			} else {
				$.when(gtu.fetchGtuLocation(taskId, {
					quite: true
				})).done(function () {
					self.drawLastLocation();
					self.drawGTUTrack();
				});
			}
		},
		onReCenter: function () {
			var googleMap = this.getGoogleMap();
			googleMap.setCenter(dmapBounds.getCenter());
			this.drawGtu();
		},
		onSelectedGTU: function (gtuId) {
			var activeGtu = _.clone(this.state.activeGtu);
			if (_.indexOf(activeGtu, gtuId) > -1) {
				_.pull(activeGtu, gtuId);
			} else {
				activeGtu.push(gtuId);
			}
			this.setState({
				activeGtu: activeGtu
			});
		},
		onShowGTU: function (gtuId) {
			var googleMap = this.getGoogleMap(),
			    gtu = this.props.gtu.get(gtuId);

			googleMap.setCenter(gtu.get('Location'));
			// if (this.state.displayMode == 'cover') {
			// 	googleMap.setCenter(gtu.get('Location'));
			// } else {
			// 	this.drawGTUTrack(gtu);
			// }
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
		onAddEmployee: function () {
			var user = new UserCollection(),
			    self = this;
			user.fetchCompany().done(function () {
				self.publish('showDialog', EmployeeView, {
					model: new UserModel(),
					company: user
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
		onSwitchDisplayMode: function () {
			this.setState({
				displayMode: this.state.displayMode == 'cover' ? 'track' : 'cover'
			});
		},
		onSwitchShowOutOfBoundaryPoints: function () {
			this.setState({
				ShowOutOfBoundary: !this.state.ShowOutOfBoundary,
				lastViewArea: null
			});
		},
		onCopyShareLink: function () {},
		onOpenMoreMenu: function (e) {
			e.preventDefault();
			e.stopPropagation();
		},
		onCloseMoreMenu: function () {
			$('#monitor-more-menu').foundation('close');
		},
		renderMoreMenu: function () {
			if (this.props.task.get('Status') == 1) {
				var assignButton = null;
			} else {
				var assignButton = React.createElement(
					'li',
					null,
					React.createElement(
						'a',
						{ href: 'javascript:;', onClick: this.onAssign },
						React.createElement('i', { className: 'fa fa-users' }),
						' ',
						React.createElement(
							'span',
							null,
							'Assign GTU'
						)
					)
				);
			}
			return React.createElement(
				'span',
				{ className: 'float-right' },
				React.createElement(
					'button',
					{ className: 'button cirle', 'data-toggle': 'monitor-more-menu', onClick: this.onOpenMoreMenu },
					React.createElement('i', { className: 'fa fa-ellipsis-h' })
				),
				React.createElement(
					'div',
					{ id: 'monitor-more-menu', className: 'dropdown-pane bottom',
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false',
						onClick: this.onCloseMoreMenu },
					React.createElement(
						'ul',
						{ className: 'vertical menu' },
						assignButton,
						React.createElement(
							'li',
							null,
							React.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onAddEmployee },
								React.createElement('i', { className: 'fa fa-user-plus' }),
								' ',
								React.createElement(
									'span',
									null,
									'New Employee'
								)
							)
						),
						React.createElement(
							'li',
							null,
							React.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onSwitchDisplayMode },
								React.createElement('i', { className: this.state.displayMode == 'cover' ? 'fa fa-map' : 'fa fa-map-o' }),
								' ',
								React.createElement(
									'span',
									null,
									this.state.displayMode == 'cover' ? 'Show Track Mode' : 'Show Points Mode'
								)
							)
						),
						React.createElement(
							'li',
							null,
							React.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onSwitchShowOutOfBoundaryPoints },
								React.createElement('i', { className: !this.state.ShowOutOfBoundary ? 'fa fa-compress' : 'fa fa-expand' }),
								' ',
								React.createElement(
									'span',
									null,
									!this.state.ShowOutOfBoundary ? 'Show Out of Boundary GTU' : 'Hide Out of Boundary GTU'
								)
							)
						),
						React.createElement(
							'li',
							null,
							React.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onCopyShareLink },
								React.createElement('i', { className: 'fa fa-link' }),
								' ',
								React.createElement(
									'span',
									null,
									'Copy Distribution Link'
								)
							)
						)
					)
				)
			);
		},
		renderGtu: function (gtu) {
			var typeIcon = null,
			    alertIcon = null,
			    deleteIcon = null,
			    buttonClass = 'button text-left',
			    taskIsStopped = this.props.task.get('Status') == 1;

			if (taskIsStopped) {
				gtuIcon = React.createElement('i', { className: 'fa fa-stop', style: { color: gtu.get('UserColor') } });
			} else {
				switch (gtu.get('Role')) {
					case 'Driver':
						gtuIcon = React.createElement('i', { className: 'fa fa-truck', style: { color: gtu.get('UserColor') } });
						break;
					case 'Walker':
						gtuIcon = React.createElement('i', { className: 'fa fa-street-view', style: { color: gtu.get('UserColor') } });
						break;
					default:
						gtuIcon = null;
						break;
				}
			}

			if (_.indexOf(this.state.activeGtu, gtu.get('Id')) > -1) {
				buttonClass += ' active';
			}
			if (!taskIsStopped && !gtu.get('IsOnline')) {
				buttonClass += ' offline';
			}
			if (!taskIsStopped && gtu.get('IsOnline') && gtu.get('OutOfBoundary')) {
				alertIcon = React.createElement('i', { className: 'fa fa-bell faa-ring animated alert' });
			}
			if (!taskIsStopped && gtu.get('WithData')) {
				deleteIcon = React.createElement('i', { className: 'fa fa-warning alert' });
			}
			return React.createElement(
				'button',
				{ key: gtu.get('Id'), className: buttonClass, onClick: this.onSelectedGTU.bind(null, gtu.get('Id')) },
				deleteIcon,
				gtuIcon,
				'  ',
				React.createElement(
					'span',
					null,
					gtu.get('ShortUniqueID')
				),
				'  ',
				alertIcon
			);
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
					return React.createElement(
						'h5',
						null,
						'STOPPED'
					);
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
			    taskIsStopped = this.props.task.get('Status') == 1,
			    gtuList = this.props.gtu.where(function (i) {
				if (taskIsStopped) {
					return i.get('WithData');
				} else {
					return i.get('IsAssign') || i.get('WithData');
				}
			});

			return React.createElement(
				'div',
				null,
				React.createElement(
					'div',
					{ className: 'section row gtu-monitor' },
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'div',
							{ className: 'section-header' },
							React.createElement(
								'div',
								{ className: 'small-12 medium-5 large-3 columns' },
								this.renderController()
							),
							React.createElement(
								'div',
								{ className: 'small-12 medium-7 large-9 columns' },
								this.renderMoreMenu(),
								React.createElement(
									'button',
									{ className: 'button float-right', onClick: this.onReCenter },
									React.createElement('i', { className: 'fa fa-crosshairs' }),
									React.createElement(
										'span',
										null,
										'Center To DMap'
									)
								),
								React.createElement(
									'button',
									{ className: 'button float-right', onClick: this.reload },
									React.createElement('i', { className: 'fa fa-refresh' }),
									React.createElement(
										'span',
										null,
										'Refresh Data'
									)
								)
							)
						)
					)
				),
				React.createElement(
					'div',
					{ className: 'row gtu' },
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						gtuList.map(function (gtu) {
							return self.renderGtu(gtu);
						})
					)
				),
				React.createElement('div', { ref: 'mapArea' })
			);
		}
	});
});
