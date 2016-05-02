define(['jquery', 'underscore', 'sprintf', 'moment', 'react', 'collections/user', 'models/user', 'views/base', 'views/mapBase', 'views/gtu/assign', 'views/user/employee', 'react.backbone'], function ($, _, helper, moment, React, UserCollection, UserModel, BaseView, MapBaseView, AssignView, EmployeeView) {
	var reloadTimeout = null,
	    backgroundIntervalReload = null,
	    trackAnimationFrame = null,
	    dmapPolygon = null,
	    dmapBounds = null,
	    gtuData = [],
	    gtuPoints = {},
	    //{key: 'lat:lng', point: google.map.marker}
	gtuLocation = {},
	    gtuTrack = {},
	    //{key: 'gtu id', {track: google.map.polyline, startPoint: google.map.marker}
	animateIndex = 0,
	    animateSerial = 0;
	infowindow = null, startPointIcon = 'M1.8-25.1c-0.3-0.3-0.7-0.5-1.2-0.5c-0.4,0-0.8,0.2-1.2,0.5C-1-24.8-1-24.4-1-23.9c0,0.4,0.1,0.8,0.4,1c0.3,0.3,0.4,0.6,0.4,1v21.4c0,0.1,0,0.2,0.1,0.3C0,0,0.1,0,0.2,0H1c0.1,0,0.2,0,0.3-0.1s0.1-0.2,0.1-0.3v-21.4c0-0.4,0.2-0.7,0.4-1s0.4-0.6,0.4-1C2.3-24.4,2.1-24.8,1.8-25.1z M16.8-23.7c-0.2-0.2-0.4-0.2-0.6-0.2c-0.1,0-0.3,0.1-0.7,0.3c-0.4,0.2-0.6,0.4-1,0.5c-0.1,0,0.8,0.1,0.7,0.1c-0.4,0.1-0.8,0.3-1.3,0.5s-1,0.3-1.5,0.3c-0.4,0-0.8-0.1-1.1-0.2c-1.1-0.5-1-0.9-1.8-1.1c-0.8-0.3-1.7-0.4-2.6-0.4c-1.6,0-1.4,0.5-3.4,1.5c-0.5,0.2-0.8,0.4-1,0.5c-0.3,0.2-0.4,0.4-0.4,0.7v7.4c0,0.2,0.1,0.4,0.2,0.6S2.8-13,3-13c0.1,0,0.3,0,0.4-0.1C5.7-14.3,5.7-15,7.3-15c0.6,0,1.2,0.1,1.8,0.3s1.1,0.4,1.5,0.6c0.4,0.2-0.1,0.4,0.4,0.6c0.5,0.2,1.1,0.3,1.6,0.3c1.3,0,1.9-0.5,3.7-1.5c0.2-0.1,0.4-0.2,0.5-0.4c0.1-0.1,0.2-0.3,0.2-0.5v-7.5C17-23.4,16.9-23.5,16.8-23.7z', walkerIcon = 'M-9-0.2c0,0.5,0.3,1,0.8,1.4S-7,2-6.1,2.2c0.9,0.3,1.8,0.4,2.8,0.6C-2.3,2.9-1.2,3-0.1,3S2,2.9,3.1,2.8c1-0.1,2-0.3,2.8-0.6c0.9-0.3,1.5-0.6,2-1s0.8-0.9,0.8-1.4c0-0.4-0.1-0.8-0.4-1.1C8-1.6,7.6-1.9,7.2-2.1c-0.5-0.2-1-0.4-1.5-0.6C5.2-2.8,4.7-3,4.1-3.1c-0.2,0-0.4,0-0.6,0.1C3.3-2.9,3.2-2.7,3.2-2.5s0,0.4,0.1,0.6s0.3,0.3,0.5,0.3c0.5,0.1,0.9,0.2,1.3,0.3c0.4,0.1,0.7,0.2,1,0.3c0.2,0.1,0.4,0.2,0.6,0.3C6.9-0.6,7-0.5,7-0.5c0.1,0.1,0.1,0.1,0.1,0.1c0,0.1-0.1,0.2-0.3,0.3C6.6,0,6.3,0.2,5.9,0.3S5,0.6,4.5,0.7S3.3,0.9,2.5,1C1.7,1.1,0.9,1.1,0,1.1s-1.7,0-2.5-0.1s-1.5-0.2-2-0.3s-1-0.3-1.4-0.4C-6.3,0.2-6.6,0-6.8-0.1s-0.3-0.2-0.3-0.3c0,0,0-0.1,0.1-0.1c0.1-0.1,0.2-0.1,0.3-0.2S-6.3-0.9-6.1-1c0.2-0.1,0.6-0.2,1-0.3c0.4-0.1,0.8-0.2,1.3-0.3c0.2,0,0.4-0.1,0.5-0.3c0.1-0.2,0.2-0.4,0.1-0.6c0-0.2-0.1-0.4-0.3-0.5c-0.2-0.1-0.4-0.2-0.6-0.1C-4.7-3-5.2-2.9-5.7-2.7s-1,0.3-1.5,0.6c-0.5,0.2-0.9,0.5-1.1,0.8S-9-0.6-9-0.2z M-4.2-11.4v4.8C-4.2-6.4-4.1-6.2-4-6c0.2,0.2,0.3,0.2,0.6,0.2h0.8V-1c0,0.2,0.1,0.4,0.2,0.6c0.2,0.2,0.3,0.2,0.6,0.2h3.2c0.2,0,0.4-0.1,0.6-0.2C2.2-0.6,2.2-0.7,2.2-1v-4.8H3c0.2,0,0.4-0.1,0.6-0.2c0.2-0.2,0.2-0.3,0.2-0.6v-4.8c0-0.4-0.2-0.8-0.5-1.1S2.6-13,2.2-13h-4.8c-0.4,0-0.8,0.2-1.1,0.5S-4.2-11.8-4.2-11.4z M-3-16.2c0,0.8,0.3,1.4,0.8,2s1.2,0.8,2,0.8s1.4-0.3,2-0.8s0.8-1.2,0.8-2c0-0.8-0.3-1.4-0.8-2c-0.5-0.6-1.2-0.8-2-0.8s-1.4,0.3-2,0.8S-3-17-3-16.2z', driverIcon = 'M-10.8,1.3c-0.3,0.3-0.7,0.5-1.1,0.5c-0.4,0-0.8-0.2-1.1-0.5s-0.5-0.7-0.5-1.1s0.2-0.8,0.5-1.1s0.7-0.5,1.1-0.5c0.4,0,0.8,0.2,1.1,0.5c0.3,0.3,0.5,0.7,0.5,1.1C-10.3,0.7-10.4,1.1-10.8,1.3z M-15.2-6.6c0-0.1,0-0.2,0.1-0.3l2.5-2.5c0.1,0,0.2-0.1,0.3-0.1h2v3.3h-4.9V-6.6z M0.7,1.3C0.4,1.6-0.1,1.8-0.4,1.8s-0.8-0.2-1.1-0.5s-0.5-0.7-0.5-1.1s0.2-0.8,0.5-1.1c0.3-0.3,0.7-0.5,1.1-0.5s0.8,0.2,1.1,0.5c0.3,0.3,0.5,0.7,0.5,1.1C1.1,0.7,1,1.1,0.7,1.3z M4.2-14.1c-0.2-0.2-0.4-0.3-0.6-0.3h-13c-0.3,0-0.5,0.1-0.6,0.3c-0.1,0.2-0.3,0.3-0.3,0.6v2.4h-2c-0.2,0-0.5,0.1-0.7,0.2c-0.3,0.1-0.5,0.2-0.7,0.4l-2.5,2.5c-0.1,0.1-0.2,0.2-0.3,0.4c-0.1,0.1-0.1,0.2-0.2,0.4c0,0.1-0.1,0.3-0.1,0.5s0,0.3,0,0.4c0,0.1,0,0.3,0,0.5s0,0.4,0,0.4v4.1c-0.2,0-0.4,0.1-0.6,0.2s-0.2,0.4-0.2,0.6c0,0.1,0,0.2,0.1,0.3c0,0.1,0.1,0.2,0.2,0.2s0.2,0.1,0.2,0.1s0.2,0.1,0.3,0.1s0.2,0,0.3,0c0.1,0,0.2,0,0.3,0s0.3,0,0.3,0h0.8c0,0.9,0.3,1.7,1,2.3s1.4,1,2.3,1s1.7-0.3,2.3-1c0.6-0.6,1-1.4,1-2.3h4.9c0,0.9,0.3,1.7,1,2.3s1.4,1,2.3,1s1.7-0.3,2.3-1c0.6-0.6,1-1.4,1-2.3c0,0,0.1,0,0.3,0c0.2,0,0.3,0,0.3,0s0.1,0,0.3,0c0.1,0,0.2,0,0.3-0.1c0.1,0,0.1-0.1,0.2-0.1s0.1-0.1,0.2-0.2c0-0.1,0.1-0.2,0.1-0.3v-13C4.4-13.9,4.3-14,4.2-14.1z', angleIcon = 'M0.9,0C0.9,0,0.9,0,0.9,0l-1.4-1.5c0,0,0,0-0.1,0c0,0,0,0-0.1,0l-0.2,0.2c0,0,0,0,0,0.1s0,0,0,0.1L0.3,0l-1.2,1.2c0,0,0,0,0,0.1c0,0,0,0,0,0.1l0.2,0.2c0,0,0,0,0.1,0c0,0,0,0,0.1,0L0.9,0C0.9,0,0.9,0,0.9,0z';

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
			var pageLeftHeight = $(window).height() - $(this.refs.mapArea).position().top;
			this.setMapHeight(pageLeftHeight);
		},
		componentDidMount: function () {
			this.publish('showLoading');
			/**
    * position google map to main area
    */
			$(window).on('resize.gtu-monitor-view', $.proxy(this.onWindowResize, this));
			this.onWindowResize();

			var self = this,
			    googleMap = this.getGoogleMap();

			this.drawDmapBoundary().then(this.filterGtu).then(this.drawGtu).done(function () {
				googleMap.addListener('zoom_changed', $.proxy(self.drawGtu, self));
				googleMap.addListener('dragend', $.proxy(self.drawGtu, self));
			}).done(function () {
				self.drawLastLocation();
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
				infowindow = new google.maps.InfoWindow();
				window.clearInterval(backgroundIntervalReload);
				backgroundIntervalReload = window.setInterval(self.reload, 15 * 1000);
			});

			$("#monitor-more-menu").foundation();
		},
		componentWillUnmount: function () {
			var googleMap = this.getGoogleMap();
			try {
				this.clearMap();
				_.forEach(gtuLocation, function (item) {
					item.setMap(null);
				});
				dmapPolygon.setMap(null);
				google.maps.event.clearInstanceListeners(googleMap);
				$(document).off('resize.gtu-monitor-view');
			} catch (ex) {}
		},
		shouldComponentUpdate: function (nextProps, nextState) {
			var oldActiveGtu = nextState.activeGtu,
			    newActiveGtu = this.state.activeGtu,
			    xor = _.xor(oldActiveGtu, newActiveGtu);

			if (nextState.displayMode == 'track') {
				window.clearInterval(backgroundIntervalReload);
			} else {
				window.clearInterval(backgroundIntervalReload);
				backgroundIntervalReload = window.setInterval(this.reload, 15 * 1000);
			}

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
				strokeColor: fillColor,
				strokeOpacity: 1,
				strokeWeight: 6,
				fillOpacity: 0,
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
		filterGtu: function (precision) {
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
				var filterGtu = _.groupBy(points, function (latlng) {
					if (latlng && latlng.lat && latlng.lng) {
						return gtu.color + ':' + (_.round(latlng.lat, precision) + ':' + _.round(latlng.lng, precision));
					}
					return null;
				});
				precision++;
				_.forEach(filterGtu, function (v, k) {
					if (k) {
						var latlng = k.split(':');
						var random = Math.pow(0.1, precision) * _.random(9);
						result.push({
							key: k,
							lat: parseFloat(latlng[1]) + _.round(random, precision),
							lng: parseFloat(latlng[2]),
							color: gtu.color
						});
					}
				});
			});
			return result;
		},
		prepareGtu: function () {
			var gtu2 = this.filterGtu(2),
			    gtu3 = this.filterGtu(3),
			    gtu4 = this.filterGtu(4),
			    gtu5 = this.filterGtu(5);
			gtuData = [gtu2, gtu3, gtu4, gtu5];
		},
		drawGtu: function () {
			if (this.state.displayMode != 'cover') {
				return;
			}
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
					self.setState({ busy: false });
				}
			},
			    filterGtus,
			    point;
			console.log(gtuData);
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
			var def = $.Deferred(),
			    taskId = this.props.task.get('Id'),
			    activeGtu = this.state.activeGtu;
			if (activeGtu && activeGtu.length == 1) {
				var currentGtu = this.props.gtu.get(activeGtu[0]),
				    track = currentGtu.get('track');
				if (!_.isEmpty(track)) {
					def.resolve();
				} else {
					def = currentGtu.getTrack(taskId);
				}
			} else {
				def.resolve();
			}
			return def;
		},
		drawGTUTrack: function () {
			if (this.state.displayMode != 'track') {
				window.cancelAnimationFrame(trackAnimationFrame);
				return;
			}
			var googleMap = this.getGoogleMap(),
			    activeGtu = this.state.activeGtu,
			    gtus = this.props.gtu,
			    self = this;
			this.prepareGTUTrack().then(function () {
				self.clearMap();
				if (activeGtu && activeGtu.length == 1) {
					animateIndex = 0;
					self.animateDrawTrack(gtus.get(activeGtu[0]));
				}
			});
		},
		animateDrawTrack: function (gtu) {
			if (this.state.displayMode != 'track') {
				return;
			}
			if (animateSerial % 10 == 0) {
				animateSerial = 1;
			} else {
				animateSerial++;
				trackAnimationFrame = window.requestAnimationFrame($.proxy(function () {
					this.animateDrawTrack(gtu);
				}, this));
				return;
			}
			var path = gtu.get('track');
			if (!path || animateIndex >= path.length) {
				return;
			}
			var gtuId = gtu.get('Id'),
			    googleMap = this.getGoogleMap();
			if (!gtuTrack[gtuId]) {
				gtuTrack[gtuId] = [];
			}
			var trackPoint = new google.maps.Marker({
				position: path[animateIndex++],
				icon: {
					path: this.getCirclePath(6),
					fillColor: gtu.get('UserColor'),
					fillOpacity: 1,
					strokeOpacity: 1,
					strokeColor: '#000'
				},
				draggable: false,
				map: googleMap
			});
			gtuTrack[gtuId].push(trackPoint);
			trackAnimationFrame = window.requestAnimationFrame($.proxy(function () {
				this.animateDrawTrack(gtu);
			}, this));
		},
		drawLastLocation: function () {
			var googleMap = this.getGoogleMap(),
			    point,
			    taskIsStopped = this.props.task.get('Status') == 1,
			    gtuList = this.props.gtu.where(function (i) {
				if (taskIsStopped) {
					return i.get('WithData');
				} else {
					return i.get('IsAssign') || i.get('WithData');
				}
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

				var gtuId = gtu.get('Id'),
				    location = gtu.get('Location');
				if (!location) {
					delete gtuLocation[gtuId];
					return true;
				}
				if (gtuLocation[gtuId]) {
					gtuLocation[gtuId].setPosition({
						lat: location.lat,
						lng: location.lng
					});
				} else {
					gtuLocation[gtuId] = new google.maps.Marker({
						position: {
							lat: location.lat,
							lng: location.lng
						},
						icon: {
							path: gtu.get('Role') == 'Driver' ? driverIcon : walkerIcon,
							fillColor: gtu.get('UserColor'),
							fillOpacity: 1,
							strokeOpacity: 1,
							strokeWeight: 1,
							strokeColor: '#fff'
						},
						draggable: false,
						map: googleMap
					});
				}
			});
		},
		clearMap: function () {
			try {
				_.forEach(gtuPoints, function (item) {
					item.setMap(null);
				});
				window.window.cancelAnimationFrame(trackAnimationFrame);
				animateIndex = 0;
				_.forEach(gtuTrack, function (item) {
					item && _.forEach(item, function (point) {
						point && point.setMap && point.setMap(null);
					});
				});
			} catch (ex) {}
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
				}).promise, gtu.fetchGtuLocation(taskId, {
					quite: true
				}).promise]).done(function () {
					self.drawLastLocation();
					self.prepareGtu();
					self.drawGtu();
				});
			} else {
				self.drawLastLocation();
				self.drawGTUTrack();
			}
		},
		onReCenter: function () {
			var googleMap = this.getGoogleMap();
			googleMap.setCenter(dmapBounds.getCenter());
			this.drawGtu();
		},
		onSelectedGTU: function (gtuId) {
			var activeGtu = _.clone(this.state.activeGtu);
			if (this.state.displayMode == 'cover') {
				if (_.indexOf(activeGtu, gtuId) > -1) {
					_.pull(activeGtu, gtuId);
				} else {
					activeGtu.push(gtuId);
				}
			} else {
				activeGtu = [gtuId];
			}
			this.setState({
				activeGtu: activeGtu
			});
		},
		onGotoGTU: function (gtuId, e) {
			e.preventDefault();
			e.stopPropagation();
			var googleMap = this.getGoogleMap(),
			    gtu = this.props.gtu.get(gtuId),
			    location = gtu.get('Location'),
			    marker = gtuLocation[gtu.get('Id')];
			if (location) {
				googleMap.setCenter(gtu.get('Location'));
				infowindow.setContent(gtu.get('ShortUniqueID'));
				infowindow.open(googleMap, marker);
			} else {
				this.publish('showDialog', 'No Data');
			}
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
			var activeGtu = _.clone(this.state.activeGtu),
			    singleActiveGtu = [];
			if (activeGtu && activeGtu.length > 0) {
				singleActiveGtu = [activeGtu[0]];
			}
			this.setState({
				activeGtu: this.state.displayMode == 'cover' ? singleActiveGtu : activeGtu,
				displayMode: this.state.displayMode == 'cover' ? 'track' : 'cover',
				lastDisplayGtuIndex: null
			});
		},
		onSwitchShowOutOfBoundaryPoints: function () {
			this.setState({
				ShowOutOfBoundary: !this.state.ShowOutOfBoundary,
				lastViewArea: null
			});
		},
		onCopyShareLink: function () {
			var location = window.location,
			    path = location.pathname.substr(1);
			firstPath = location.pathname.substr(0, path.indexOf('/') + 1), task = this.props.task.get('PublicUrl'), address = location.protocol + '//' + window.location.host + firstPath + '/monitor/#' + task;
			this.publish('showDialog', address);
		},
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
			//for for Distribution drivers
			if (!_.isInteger(this.props.task.get('Id'))) {
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
										this.state.displayMode == 'cover' ? 'Track Path' : 'Show Coverage'
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
										!this.state.ShowOutOfBoundary ? 'Show Out of Bounds' : 'Hide Out of Bounds'
									)
								)
							)
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
									this.state.displayMode == 'cover' ? 'Track Path' : 'Show Coverage'
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
									!this.state.ShowOutOfBoundary ? 'Show Out of Bounds' : 'Hide Out of Bounds'
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
									'URL for Distribution drivers'
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
			    taskIsStopped = this.props.task.get('Status') == 1,
			    isActive = _.indexOf(this.state.activeGtu, gtu.get('Id')) > -1;

			if (taskIsStopped) {
				//gtuIcon = <i className="fa fa-stop" style={{color: gtu.get('UserColor')}}></i>
				gtuIcon = React.createElement('i', { className: 'fa fa-stop' });
			} else {
				switch (gtu.get('Role')) {
					case 'Driver':
						//gtuIcon = <i className="fa fa-truck" style={{color: gtu.get('UserColor')}}></i>
						gtuIcon = React.createElement('i', { className: 'fa fa-truck' });
						break;
					case 'Walker':
						//gtuIcon = <i className="fa fa-street-view" style={{color: gtu.get('UserColor')}}></i>
						gtuIcon = React.createElement('i', { className: 'fa fa-street-view' });
						break;
					default:
						gtuIcon = null;
						break;
				}
			}

			if (isActive) {
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
				'span',
				{ className: 'group', key: gtu.get('Id') },
				React.createElement(
					'button',
					{ className: buttonClass, style: { 'backgroundColor': isActive ? gtu.get('UserColor') : 'transparent' }, onClick: this.onSelectedGTU.bind(null, gtu.get('Id')) },
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
				),
				React.createElement(
					'button',
					{ className: 'button location', onClick: this.onGotoGTU.bind(null, gtu.get('Id')) },
					React.createElement('i', { className: 'location fa fa-crosshairs', style: { color: 'black' } })
				)
			);
		},
		renderController: function () {
			var task = this.props.task;
			//for for Distribution drivers
			if (!_.isInteger(this.props.task.get('Id'))) {
				switch (task.get('Status')) {
					case 0:
						//started
						return React.createElement(
							'h5',
							null,
							'STARTED'
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
							'h5',
							null,
							'PEASED'
						);
						break;
					default:
						return null;
						break;
				}
			}
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
										'Center'
									)
								),
								React.createElement(
									'button',
									{ className: 'button float-right', onClick: this.reload },
									React.createElement('i', { className: 'fa fa-refresh' }),
									React.createElement(
										'span',
										null,
										'Refresh'
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
