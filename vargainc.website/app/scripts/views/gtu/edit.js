define(['jquery', 'underscore', 'sprintf', 'moment', 'react', 'views/base', 'views/mapBase', 'fastMarker', 'react.backbone'], function ($, _, helper, moment, React, BaseView, MapBaseView, FastMarker) {
	var dmapPolygon = null,
	    dmapBounds = null,
	    gtuData = null,
	    gtuPoints = [],
	    savedCustomPoints = [];

	return React.createBackboneClass({
		mixins: [BaseView, MapBaseView],
		getInitialState: function () {
			return {
				disableDefaultUI: false,
				scrollwheel: false,
				disableDoubleClickZoom: true,
				activeGtu: null,
				customPoints: [],
				maxDisplayCount: 2000
			};
		},
		onWindowResize: function () {
			if (this.refs.mapArea) {
				var pageLeftHeight = $(window).height() - $(this.refs.mapArea).position().top;
				this.setMapHeight(pageLeftHeight);
			}
		},
		componentDidMount: function () {
			this.publish('showLoading');
			/**
    * position google map to main area
    */
			$(window).on('resize.gtu-edit-view', $.proxy(this.onWindowResize, this));
			this.onWindowResize();

			this.setState({
				activeGtu: this.props.gtu.at(0)
			});

			var self = this,
			    googleMap = this.getGoogleMap();
			this.drawDmapBoundary().then(this.filterGtu).then(this.drawGtu).done(function () {
				dmapPolygon.addListener('click', $.proxy(self.onNewGtu, self));
				// googleMap.addListener('zoom_changed', $.proxy(self.drawGtu, self));
				// googleMap.addListener('dragend', $.proxy(self.drawGtu, self));
				self.publish('hideLoading');
			});
		},
		componentWillUnmount: function () {
			var googleMap = this.getGoogleMap();
			try {
				_.forEach(gtuPoints, function (item) {
					item.setMap(null);
				});
				_.forEach(savedCustomPoints, function (item) {
					item.setMap(null);
				});
				dmapPolygon.setMap(null);
				google.maps.event.clearInstanceListeners(googleMap);
				$(document).off('resize.gtu-edit-view');
			} catch (ex) {
				console.log('google map clear error', ex);
			}
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

			// console.log("precision 1000   gut:", gtu3.length);
			// console.log("precision 10000  gut:", gtu4.length);
			// console.log("precision 100000 gut:", gtu5.length);
		},
		drawGtu: function () {
			var gtus = this.props.dmap.get('Gtu') || [],
			    googleMap = this.getGoogleMap(),
			    self = this;
			_.forEach(gtus, function (colorGtu) {
				var color = colorGtu.color;
				_.forEach(colorGtu.points, function (gtu) {
					new FastMarker({
						position: {
							lat: gtu.lat,
							lng: gtu.lng
						},
						icon: {
							path: self.getCirclePath(5),
							fillColor: color,
							fillOpacity: 1,
							strokeOpacity: 1,
							strokeWeight: 1,
							strokeColor: '#000'
						},
						draggable: false,
						map: googleMap
					});
				});
			});
		},
		olddrawGtu: function () {
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
			//console.log(lastDisplayGtuIndex, lastZoomLevel, zoomLevel);
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
					//console.log(visiableGtu.length + ' / ' + points.length + ' dots in current zoom level ' + googleMap.getZoom() + ' view area');
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
		onNewGtu: function (e) {
			var googleMap = this.getGoogleMap(),
			    point = new google.maps.Marker({
				position: e.latLng,
				icon: {
					path: this.getCirclePath(5),
					fillColor: this.state.activeGtu.get('UserColor'),
					fillOpacity: 1,
					strokeOpacity: 1,
					strokeWeight: 2,
					strokeColor: '#fff'
				},
				draggable: true,
				map: googleMap
			});

			point.GtuId = this.state.activeGtu.get('Id');
			point.date = moment().format('YYYY-MM-DD HH:mm:ss');
			this.state.customPoints.push(point);
			this.forceUpdate();
		},
		onUnderGtu: function () {
			var customPoints = this.state.customPoints,
			    lastPoint = _.last(customPoints);

			lastPoint.setMap(null);
			this.setState({ customPoints: _.dropRight(customPoints) });
		},
		onSaveGtu: function () {
			var self = this,
			    googleMap = this.getGoogleMap(),
			    savedPoint,
			    postData = _.map(this.state.customPoints, function (item) {
				return {
					GtuId: item.GtuId,
					Date: item.date,
					Location: {
						Latitude: item.getPosition().lat(),
						Longitude: item.getPosition().lng()
					}
				};
			});
			//console.log('save gtu', postData);

			this.props.task.addGtuDots(postData).done(function () {
				_.forEach(self.state.customPoints, function (point) {
					var icon = point.getIcon();
					icon.strokeWeight = 1;
					icon.strokeColor = '#000';
					point.setIcon(icon);
					savedCustomPoints.push(savedPoint);
				});

				self.setState({
					customPoints: []
				});
			});
		},
		onSetMaxDisplayDots: function () {
			this.setState({
				maxDisplayCount: this.refs.txtMaxCount.value
			});
			setTimeout(this.drawGtu, 500);
		},
		onReCenter: function () {
			var googleMap = this.getGoogleMap();
			googleMap.setCenter(dmapBounds.getCenter());
			this.drawGtu();
		},
		render: function () {
			var self = this,
			    gtuList = this.props.gtu.models || [],
			    canUndoSave = this.state.customPoints.length > 0 ? true : false;
			if (!canUndoSave) {
				var undoButton = React.createElement(
					'button',
					{ className: 'button float-right', disabled: true, onClick: this.onUnderGtu },
					React.createElement('i', { className: 'fa fa-undo' }),
					'Undo'
				);
				var saveButton = React.createElement(
					'button',
					{ className: 'button float-right', disabled: true, onClick: this.onSaveGtu },
					React.createElement('i', { className: 'fa fa-save' }),
					'Save'
				);
			} else {
				var undoButton = React.createElement(
					'button',
					{ className: 'button float-right', onClick: this.onUnderGtu },
					React.createElement('i', { className: 'fa fa-undo' }),
					'Undo'
				);
				var saveButton = React.createElement(
					'button',
					{ className: 'button float-right', onClick: this.onSaveGtu },
					React.createElement('i', { className: 'fa fa-save' }),
					'Save'
				);
			}

			return React.createElement(
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
									'Edit GTU - ',
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
											' Edit GTU'
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
					gtuList.map(function (gtu) {
						return React.createElement(
							'button',
							{ key: gtu.get('Id'), className: self.state.activeGtu == gtu ? " button" : "button", onClick: self.setActiveGtu.bind(null, gtu.get('Id')) },
							React.createElement('i', { className: self.state.activeGtu == gtu ? "fa fa-map-marker" : "fa fa-stop", style: { color: gtu.get('UserColor') } }),
							gtu.get('ShortUniqueID')
						);
					})
				),
				React.createElement(
					'div',
					{ className: 'small-12 medium-5 large-3 columns' },
					saveButton,
					undoButton,
					React.createElement(
						'button',
						{ className: 'button float-right', onClick: this.onReCenter },
						React.createElement('i', { className: 'fa fa-refresh' }),
						'ReCenter'
					)
				),
				React.createElement('div', { className: 'small-12 columns', ref: 'mapArea' })
			);
		}
	});
});
