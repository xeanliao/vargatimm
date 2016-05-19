define([
	'jquery',
	'underscore',
	'sprintf',
	'moment',
	'react',
	'collections/user',
	'models/user',
	'views/base',
	'views/mapBase',
	'views/gtu/assign',
	'views/user/employee',
	'fastMarker',

	'react.backbone'
], function ($, _, helper, moment, React, UserCollection, UserModel, BaseView, MapBaseView, AssignView, EmployeeView, FastMarker) {
	var reloadTimeout = null,
		backgroundIntervalReload = null,
		trackAnimationFrame = null,
		dmapPolygon = null,
		dmapBounds = null,
		gtuPoints = [],//{key: 'lat:lng', point: google.map.marker}
		gtuLocation = {},
		gtuTrack = {},//{key: 'gtu id', {track: google.map.polyline, startPoint: google.map.marker}
		animateIndex = 0,
		animateSerial = 0;
		infowindow = null,
		startPointIcon = 'M1.8-25.1c-0.3-0.3-0.7-0.5-1.2-0.5c-0.4,0-0.8,0.2-1.2,0.5C-1-24.8-1-24.4-1-23.9c0,0.4,0.1,0.8,0.4,1c0.3,0.3,0.4,0.6,0.4,1v21.4c0,0.1,0,0.2,0.1,0.3C0,0,0.1,0,0.2,0H1c0.1,0,0.2,0,0.3-0.1s0.1-0.2,0.1-0.3v-21.4c0-0.4,0.2-0.7,0.4-1s0.4-0.6,0.4-1C2.3-24.4,2.1-24.8,1.8-25.1z M16.8-23.7c-0.2-0.2-0.4-0.2-0.6-0.2c-0.1,0-0.3,0.1-0.7,0.3c-0.4,0.2-0.6,0.4-1,0.5c-0.1,0,0.8,0.1,0.7,0.1c-0.4,0.1-0.8,0.3-1.3,0.5s-1,0.3-1.5,0.3c-0.4,0-0.8-0.1-1.1-0.2c-1.1-0.5-1-0.9-1.8-1.1c-0.8-0.3-1.7-0.4-2.6-0.4c-1.6,0-1.4,0.5-3.4,1.5c-0.5,0.2-0.8,0.4-1,0.5c-0.3,0.2-0.4,0.4-0.4,0.7v7.4c0,0.2,0.1,0.4,0.2,0.6S2.8-13,3-13c0.1,0,0.3,0,0.4-0.1C5.7-14.3,5.7-15,7.3-15c0.6,0,1.2,0.1,1.8,0.3s1.1,0.4,1.5,0.6c0.4,0.2-0.1,0.4,0.4,0.6c0.5,0.2,1.1,0.3,1.6,0.3c1.3,0,1.9-0.5,3.7-1.5c0.2-0.1,0.4-0.2,0.5-0.4c0.1-0.1,0.2-0.3,0.2-0.5v-7.5C17-23.4,16.9-23.5,16.8-23.7z',
		walkerIcon = 'M-9-0.2c0,0.5,0.3,1,0.8,1.4S-7,2-6.1,2.2c0.9,0.3,1.8,0.4,2.8,0.6C-2.3,2.9-1.2,3-0.1,3S2,2.9,3.1,2.8c1-0.1,2-0.3,2.8-0.6c0.9-0.3,1.5-0.6,2-1s0.8-0.9,0.8-1.4c0-0.4-0.1-0.8-0.4-1.1C8-1.6,7.6-1.9,7.2-2.1c-0.5-0.2-1-0.4-1.5-0.6C5.2-2.8,4.7-3,4.1-3.1c-0.2,0-0.4,0-0.6,0.1C3.3-2.9,3.2-2.7,3.2-2.5s0,0.4,0.1,0.6s0.3,0.3,0.5,0.3c0.5,0.1,0.9,0.2,1.3,0.3c0.4,0.1,0.7,0.2,1,0.3c0.2,0.1,0.4,0.2,0.6,0.3C6.9-0.6,7-0.5,7-0.5c0.1,0.1,0.1,0.1,0.1,0.1c0,0.1-0.1,0.2-0.3,0.3C6.6,0,6.3,0.2,5.9,0.3S5,0.6,4.5,0.7S3.3,0.9,2.5,1C1.7,1.1,0.9,1.1,0,1.1s-1.7,0-2.5-0.1s-1.5-0.2-2-0.3s-1-0.3-1.4-0.4C-6.3,0.2-6.6,0-6.8-0.1s-0.3-0.2-0.3-0.3c0,0,0-0.1,0.1-0.1c0.1-0.1,0.2-0.1,0.3-0.2S-6.3-0.9-6.1-1c0.2-0.1,0.6-0.2,1-0.3c0.4-0.1,0.8-0.2,1.3-0.3c0.2,0,0.4-0.1,0.5-0.3c0.1-0.2,0.2-0.4,0.1-0.6c0-0.2-0.1-0.4-0.3-0.5c-0.2-0.1-0.4-0.2-0.6-0.1C-4.7-3-5.2-2.9-5.7-2.7s-1,0.3-1.5,0.6c-0.5,0.2-0.9,0.5-1.1,0.8S-9-0.6-9-0.2z M-4.2-11.4v4.8C-4.2-6.4-4.1-6.2-4-6c0.2,0.2,0.3,0.2,0.6,0.2h0.8V-1c0,0.2,0.1,0.4,0.2,0.6c0.2,0.2,0.3,0.2,0.6,0.2h3.2c0.2,0,0.4-0.1,0.6-0.2C2.2-0.6,2.2-0.7,2.2-1v-4.8H3c0.2,0,0.4-0.1,0.6-0.2c0.2-0.2,0.2-0.3,0.2-0.6v-4.8c0-0.4-0.2-0.8-0.5-1.1S2.6-13,2.2-13h-4.8c-0.4,0-0.8,0.2-1.1,0.5S-4.2-11.8-4.2-11.4z M-3-16.2c0,0.8,0.3,1.4,0.8,2s1.2,0.8,2,0.8s1.4-0.3,2-0.8s0.8-1.2,0.8-2c0-0.8-0.3-1.4-0.8-2c-0.5-0.6-1.2-0.8-2-0.8s-1.4,0.3-2,0.8S-3-17-3-16.2z',
		driverIcon = 'M-10.8,1.3c-0.3,0.3-0.7,0.5-1.1,0.5c-0.4,0-0.8-0.2-1.1-0.5s-0.5-0.7-0.5-1.1s0.2-0.8,0.5-1.1s0.7-0.5,1.1-0.5c0.4,0,0.8,0.2,1.1,0.5c0.3,0.3,0.5,0.7,0.5,1.1C-10.3,0.7-10.4,1.1-10.8,1.3z M-15.2-6.6c0-0.1,0-0.2,0.1-0.3l2.5-2.5c0.1,0,0.2-0.1,0.3-0.1h2v3.3h-4.9V-6.6z M0.7,1.3C0.4,1.6-0.1,1.8-0.4,1.8s-0.8-0.2-1.1-0.5s-0.5-0.7-0.5-1.1s0.2-0.8,0.5-1.1c0.3-0.3,0.7-0.5,1.1-0.5s0.8,0.2,1.1,0.5c0.3,0.3,0.5,0.7,0.5,1.1C1.1,0.7,1,1.1,0.7,1.3z M4.2-14.1c-0.2-0.2-0.4-0.3-0.6-0.3h-13c-0.3,0-0.5,0.1-0.6,0.3c-0.1,0.2-0.3,0.3-0.3,0.6v2.4h-2c-0.2,0-0.5,0.1-0.7,0.2c-0.3,0.1-0.5,0.2-0.7,0.4l-2.5,2.5c-0.1,0.1-0.2,0.2-0.3,0.4c-0.1,0.1-0.1,0.2-0.2,0.4c0,0.1-0.1,0.3-0.1,0.5s0,0.3,0,0.4c0,0.1,0,0.3,0,0.5s0,0.4,0,0.4v4.1c-0.2,0-0.4,0.1-0.6,0.2s-0.2,0.4-0.2,0.6c0,0.1,0,0.2,0.1,0.3c0,0.1,0.1,0.2,0.2,0.2s0.2,0.1,0.2,0.1s0.2,0.1,0.3,0.1s0.2,0,0.3,0c0.1,0,0.2,0,0.3,0s0.3,0,0.3,0h0.8c0,0.9,0.3,1.7,1,2.3s1.4,1,2.3,1s1.7-0.3,2.3-1c0.6-0.6,1-1.4,1-2.3h4.9c0,0.9,0.3,1.7,1,2.3s1.4,1,2.3,1s1.7-0.3,2.3-1c0.6-0.6,1-1.4,1-2.3c0,0,0.1,0,0.3,0c0.2,0,0.3,0,0.3,0s0.1,0,0.3,0c0.1,0,0.2,0,0.3-0.1c0.1,0,0.1-0.1,0.2-0.1s0.1-0.1,0.2-0.2c0-0.1,0.1-0.2,0.1-0.3v-13C4.4-13.9,4.3-14,4.2-14.1z',
		angleIcon = 'M0.9,0C0.9,0,0.9,0,0.9,0l-1.4-1.5c0,0,0,0-0.1,0c0,0,0,0-0.1,0l-0.2,0.2c0,0,0,0,0,0.1s0,0,0,0.1L0.3,0l-1.2,1.2c0,0,0,0,0,0.1c0,0,0,0,0,0.1l0.2,0.2c0,0,0,0,0.1,0c0,0,0,0,0.1,0L0.9,0C0.9,0,0.9,0,0.9,0z';


	return React.createBackboneClass({
		mixins: [
			BaseView,
			MapBaseView
		],
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

			this.drawDmapBoundary().done(function () {
				self.drawGtu();
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
				backgroundIntervalReload = window.setInterval(self.reload, 30 * 1000);
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
			} catch (ex) {
			}
		},
		shouldComponentUpdate: function (nextProps, nextState) {
			var oldActiveGtu = nextState.activeGtu,
				newActiveGtu = this.state.activeGtu,
				xor = _.xor(oldActiveGtu, newActiveGtu);

			if (nextState.displayMode == 'track') {
				window.clearInterval(backgroundIntervalReload);
			} else {
				window.clearInterval(backgroundIntervalReload);
				backgroundIntervalReload = window.setInterval(this.reload, 30 * 1000);
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
			def.resolve();
			return def;
		},
		drawGtu: function () {
			if (this.state.displayMode != 'cover') {
				return;
			}
			var self = this,
				googleMap = this.getGoogleMap(),
				needFilterOutOfBoundary = !this.state.ShowOutOfBoundary,
				dots = this.props.dmap.get('Gtu') || [],
				activeGtu = this.state.activeGtu;

			_.forEach(gtuPoints, function (p) {
				p.setMap(null);
			});
			gtuPoints = [];

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
				var color = gtu.color;

				_.forEach(points, function (latlng) {
					if (latlng && latlng.lat && latlng.lng) {
						gtuPoints.push(new FastMarker({
							position: {
								lat: latlng.lat,
								lng: latlng.lng
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
						}));
					}
				});
			});
		},
		prepareGTUTrack: function(){
			var def = $.Deferred(),
				taskId = this.props.task.get('Id'),
				activeGtu = this.state.activeGtu;
			if(activeGtu && activeGtu.length == 1){
				var currentGtu = this.props.gtu.get(activeGtu[0]),
					track = currentGtu.get('track');
				if(!_.isEmpty(track)){
					def.resolve();
				}else{
					def = currentGtu.getTrack(taskId);	
				}
			}else{
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
				if(activeGtu && activeGtu.length == 1){
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
						return i.get('WithData')
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
				if(!location){
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
		clearMap: function(){
			try {
				_.forEach(gtuPoints, function (item) {
					item.setMap(null);
				});
				window.window.cancelAnimationFrame(trackAnimationFrame);
				animateIndex = 0;
				_.forEach(gtuTrack, function(item){
					item && _.forEach(item, function(point){
						point && point.setMap && point.setMap(null);
					});
				});
			} catch (ex) {
			}
		},
		reload: function(){
			window.clearTimeout(reloadTimeout);
			reloadTimeout = window.setTimeout($.proxy(this._reload, this), 2 * 1000);
		},
		_reload: function () {
			var dmap = this.props.dmap,
				gtu = this.props.gtu,
				taskId = this.props.task.get('Id'),
				self = this;
			if (this.state.displayMode == 'cover') {
				$.when([
					dmap.updateGtuAfterTime(null, {
						quite: true
					}).promise,
					gtu.fetchGtuLocation(taskId, {
						quite: true
					}).promise
				]).done(function () {
					self.drawGtu();
					self.drawLastLocation();
				});
			} else {
				self.drawLastLocation();
				self.drawGTUTrack();
			}
		},
		onReCenter: function () {
			var googleMap = this.getGoogleMap();
			googleMap.setCenter(dmapBounds.getCenter());
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
		onSwitchDisplayMode: function(){
			var activeGtu = _.clone(this.state.activeGtu),
				singleActiveGtu = [];
			if(activeGtu && activeGtu.length > 0){
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
		onCopyShareLink: function(){
			var location = window.location,
				path = location.pathname.substr(1);
				firstPath = location.pathname.substr(0, path.indexOf('/') + 1),
				task = this.props.task.get('PublicUrl'),
				address = location.protocol + '//' + window.location.host + firstPath + '/monitor/#' + task;
			this.publish('showDialog', address);
		},
		onOpenMoreMenu: function(e){
			e.preventDefault();
			e.stopPropagation();
		},
		onCloseMoreMenu: function(){
			$('#monitor-more-menu').foundation('close');
		},
		renderMoreMenu: function(){
			if (this.props.task.get('Status') == 1) {
				var assignButton = null;
			} else {
				var assignButton = (
					<li>
						<a href="javascript:;" onClick={this.onAssign}>
							<i className="fa fa-users"></i>
							&nbsp;<span>Assign GTU</span>
						</a>
					</li>
				);
			}
			//for for Distribution drivers 
			if (!_.isInteger(this.props.task.get('Id'))) {
				return (
					<span className="float-right">
						<button className="button cirle" data-toggle="monitor-more-menu" onClick={this.onOpenMoreMenu}>
							<i className="fa fa-ellipsis-h"></i>
						</button>
						<div id="monitor-more-menu" className="dropdown-pane bottom" 
							data-dropdown
							data-close-on-click="true" 
							data-auto-focus="false"
							onClick={this.onCloseMoreMenu}>
							<ul className="vertical menu">
								<li>
									<a href="javascript:;" onClick={this.onSwitchDisplayMode}>
										<i className={this.state.displayMode == 'cover' ? 'fa fa-map' : 'fa fa-map-o'}></i>
										&nbsp;<span>{this.state.displayMode == 'cover' ? 'Track Path' : 'Show Coverage'}</span>
									</a>
								</li>
								<li>
									<a href="javascript:;" onClick={this.onSwitchShowOutOfBoundaryPoints}>
										<i className={!this.state.ShowOutOfBoundary ? 'fa fa-compress' : 'fa fa-expand'}></i>
										&nbsp;<span>{!this.state.ShowOutOfBoundary ? 'Show Out of Bounds' : 'Hide Out of Bounds'}</span>
									</a>
								</li>
							</ul>
						</div>
					</span>
				);
			}
			return (
				<span className="float-right">
					<button className="button cirle" data-toggle="monitor-more-menu" onClick={this.onOpenMoreMenu}>
						<i className="fa fa-ellipsis-h"></i>
					</button>
					<div id="monitor-more-menu" className="dropdown-pane bottom" 
						data-dropdown
						data-close-on-click="true" 
						data-auto-focus="false"
						onClick={this.onCloseMoreMenu}>
						<ul className="vertical menu">
							{assignButton}
							<li>
								<a href="javascript:;" onClick={this.onAddEmployee}>
									<i className="fa fa-user-plus"></i>
									&nbsp;<span>New Employee</span>
								</a>
							</li>
							<li>
								<a href="javascript:;" onClick={this.onSwitchDisplayMode}>
									<i className={this.state.displayMode == 'cover' ? 'fa fa-map' : 'fa fa-map-o'}></i>
									&nbsp;<span>{this.state.displayMode == 'cover' ? 'Track Path' : 'Show Coverage'}</span>
								</a>
							</li>
							<li>
								<a href="javascript:;" onClick={this.onSwitchShowOutOfBoundaryPoints}>
									<i className={!this.state.ShowOutOfBoundary ? 'fa fa-compress' : 'fa fa-expand'}></i>
									&nbsp;<span>{!this.state.ShowOutOfBoundary ? 'Show Out of Bounds' : 'Hide Out of Bounds'}</span>
								</a>
							</li>
							<li>
								<a href="javascript:;" onClick={this.onCopyShareLink}>
									<i className="fa fa-link"></i>
									&nbsp;<span>URL for Distribution drivers</span>
								</a>
							</li>
						</ul>
					</div>
				</span>
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
				gtuIcon = <i className="fa fa-stop"></i>
			} else {
				switch (gtu.get('Role')) {
				case 'Driver':
					//gtuIcon = <i className="fa fa-truck" style={{color: gtu.get('UserColor')}}></i>
					gtuIcon = <i className="fa fa-truck"></i>
					break;
				case 'Walker':
					//gtuIcon = <i className="fa fa-street-view" style={{color: gtu.get('UserColor')}}></i>
					gtuIcon = <i className="fa fa-street-view"></i>
					break;
				default:
					gtuIcon = null;
					break;
				}
			}

			if(isActive){
				buttonClass += ' active';
			}
			if (!taskIsStopped && !gtu.get('IsOnline')) {
				buttonClass += ' offline';
			}
			if(!taskIsStopped && gtu.get('IsOnline') && gtu.get('OutOfBoundary')){
				alertIcon = <i className="fa fa-bell faa-ring animated alert"></i>;
			}
			if(!taskIsStopped && gtu.get('WithData')){
				deleteIcon = <i className="fa fa-warning alert"></i>;
			}
			return (
				<span className="group" key={gtu.get('Id')}>
					<button className={buttonClass} style={{'backgroundColor': isActive ? gtu.get('UserColor') : 'transparent'}} onClick={this.onSelectedGTU.bind(null, gtu.get('Id'))}>
						{deleteIcon}
						{gtuIcon}
						&nbsp;&nbsp;<span>{gtu.get('ShortUniqueID')}</span>&nbsp;&nbsp;
						{alertIcon}
					</button>
					<button className="button location" onClick={this.onGotoGTU.bind(null, gtu.get('Id'))}>
						<i className="location fa fa-crosshairs" style={{color:'black'}}></i>
					</button>
				</span>
			);
		},
		renderController: function () {
			var task = this.props.task;
			//for for Distribution drivers 
			if (!_.isInteger(this.props.task.get('Id'))) {
				switch (task.get('Status')) {
				case 0: //started
					return <h5>STARTED</h5>;
					break;
				case 1: //stoped
					return <h5>STOPPED</h5>;
					break;
				case 2: //peased
					return <h5>PEASED</h5>;
					break;
				default:
					return null
					break;
				}
			}
			switch (task.get('Status')) {
			case 0: //started
				return (
					<div>
						<button className="button" onClick={this.onPause}><i className="fa fa-pause"></i>Pause</button>
						<button className="button" onClick={this.onStop}><i className="fa fa-stop"></i>Stop</button>
					</div>
				);
				break;
			case 1: //stoped
				return <h5>STOPPED</h5>;
				break;
			case 2: //peased
				return (
					<div>
						<button className="button" onClick={this.onStart}><i className="fa fa-play"></i>Start</button>
						<button className="button" onClick={this.onStop}><i className="fa fa-stop"></i>Stop</button>
					</div>
				);
				break;
			default:
				return (
					<div>
						<button className="button" onClick={this.onStart}><i className="fa fa-play"></i>Start</button>
					</div>
				);
				break;
			}
		},
		render: function () {
			var self = this,
				taskIsStopped = this.props.task.get('Status') == 1,
				gtuList = this.props.gtu.where(function (i) {
					if (taskIsStopped) {
						return i.get('WithData')
					} else {
						return i.get('IsAssign') || i.get('WithData');
					}
				});
			
			return (
				<div>
					<div className="section row gtu-monitor">
						<div className="small-12 columns">
							<div className="section-header">
								<div className="small-12 medium-5 large-3 columns">
									{this.renderController()}
								</div>
								<div className="small-12 medium-7 large-9 columns">
									{this.renderMoreMenu()}
									<button className="button float-right" onClick={this.onReCenter}>
										<i className="fa fa-crosshairs"></i>
										<span>Center</span>
									</button>
									<button className="button float-right" onClick={this.reload}>
										<i className="fa fa-refresh"></i>
										<span>Refresh</span>
									</button>
								</div>
							</div>
						</div>
					</div>
					<div className="row gtu">
						<div className="small-12 columns">
							{gtuList.map(function(gtu) {
								return self.renderGtu(gtu);
							})}
						</div>
					</div>
					<div ref="mapArea"></div>
				</div>
			);
		}
	
	});
});