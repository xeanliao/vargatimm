import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import moment from 'moment';
import {
	each,
	groupByd,
	indexOfd,
	removed,
	filterd,
	lastd,
	mapd,
	isEmptyd,
} from 'lodash';
import $ from 'jquery';
import FastMarker from 'lib/fastMarker';

import BaseView from 'views/base';
import MapBaseView from 'views/mapBase';

var dmapPolygon = null,
	dmapBounds = null,
	gtuData = null,
	gtuPoints = [];
const TAG = '[GTU-EDIT]'
export default React.createBackboneClass({
	mixins: [
		BaseView,
		MapBaseView
	],
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
			each(gtuPoints, function (item) {
				item.setMap(null);
			});
			dmapPolygon.setMap(null);
			google.maps.event.clearInstanceListeners(googleMap);
			$(document).off('resize.gtu-edit-view');
		} catch (ex) {
			console.log(`${TAG} google map clear error`, ex);
		}
	},
	setActiveGtu: function (gtuId) {
		var activeGut = this.props.gtu.get(gtuId);
		this.setState({
			activeGtu: activeGut
		});
	},
	getCirclePath: function (size) {
		return `M-${size},0a${size},${size} 0 1,0 ${size * 2},0a${size},${size} 0 1,0 -${size * 2},0`;
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

		each(boundary, function (i) {
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
		each(dots, function (gtu) {
			var filterGtu = groupBy(gtu.points, fnFilter);
			each(filterGtu, function (v, k) {
				var latlng = k.split(':');
				result.push({
					lat: parseFloat(latlng[0]),
					lng: parseFloat(latlng[1]),
					color: gtu.color
				})
			});
		});
		return result;
	},
	prepareGtu: function () {
		var gtu3 = this.filterGtu(function (latlng) {
				return (Math.round(latlng.lat * 1000) / 1000) + ':' + (Math.round(latlng.lng * 1000) / 1000);
			}),
			gtu4 = this.filterGtu(function (latlng) {
				return (Math.round(latlng.lat * 10000) / 10000) + ':' + (Math.round(latlng.lng * 10000) / 10000);
			}),
			gtu5 = this.filterGtu(function (latlng) {
				return (Math.round(latlng.lat * 100000) / 100000) + ':' + (Math.round(latlng.lng * 100000) / 100000);
			});
		gtuData = [gtu3, gtu4, gtu5];

		console.log(`${TAG} precision 1000   gut:${gtu3.length}`);
		console.log(`${TAG} precision 10000  gut:${gtu4.length}`);
		console.log(`${TAG} precision 100000 gut:${gtu5.length}`);
	},
	drawGtu: function () {
		var gtus = this.props.dmap.get('Gtu') || [],
			googleMap = this.getGoogleMap(),
			self = this;
		each(gtus, function (colorGtu) {
			var color = colorGtu.color;
			each(colorGtu.points, function (gtu) {
				if (gtu.out) {
					return true;
				}
				if (gtu.cellId == 1) {
					var editMarker = new google.maps.Marker({
						position: {
							lat: gtu.lat,
							lng: gtu.lng
						},
						icon: {
							path: self.getCirclePath(5),
							fillColor: color,
							fillOpacity: 1,
							strokeOpacity: 1,
							strokeWeight: 2,
							strokeColor: '#fff'
						},
						draggable: false,
						map: googleMap
					});
					editMarker.gtuInfoId = gtu.gtuInfoId;
					editMarker.status = 'added';
					self.state.customPoints.push(editMarker);
					editMarker.addListener('click', function () {
						self.onRemoveGtu(editMarker);
					});

				} else {
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
				}
			});
		});
	},
	onNewGtu: function (e) {
		var googleMap = this.getGoogleMap(),
			self = this,
			newMarker = new google.maps.Marker({
				position: e.latLng,
				icon: {
					path: this.getCirclePath(5),
					fillColor: this.state.activeGtu.get('UserColor'),
					fillOpacity: 1,
					strokeOpacity: 1,
					strokeWeight: 2,
					strokeColor: '#fff'
				},
				draggable: false,
				map: googleMap
			});

		newMarker.GtuId = this.state.activeGtu.get('Id');
		newMarker.date = moment().format('YYYY-MM-DD HH:mm:ss');
		newMarker.status = 'new';
		this.state.customPoints.push(newMarker);
		newMarker.addListener('click', function () {
			self.onRemoveGtu(newMarker);
		});
		this.forceUpdate();
	},
	onRemoveGtu: function (gtu) {
		var index = indexOf(this.state.customPoints, gtu);
		if (gtu.status === 'new') {
			remove(this.state.customPoints, gtu);
		} else {
			gtu.status = 'deleted';
		}
		gtu.setMap(null);
		this.forceUpdate();
	},
	onUnderGtu: function () {
		var googleMap = this.getGoogleMap(),
			customPoints = this.state.customPoints,
			canUndoPoints = filter(customPoints, function (i) {
				return i.status == 'new' || i.status == 'deleted';
			});
		if (canUndoPoints && canUndoPoints.length > 0) {
			var lastPoint = last(canUndoPoints);
			if (lastPoint.status == 'new') {
				lastPoint.setMap(null);
				remove(customPoints, lastPoint);
			} else {
				lastPoint.setMap(googleMap);
				lastPoint.status = 'added';
			}

			this.setState({
				customPoints: customPoints
			});
		}
	},
	onSaveGtu: function () {
		var self = this,
			googleMap = this.getGoogleMap(),
			addedGtu = filter(this.state.customPoints, function (i) {
				return i.status == 'new';
			}),
			postData = map(addedGtu, function (item) {
				return {
					GtuId: item.GtuId,
					Date: item.date,
					Location: {
						Latitude: item.getPosition().lat(),
						Longitude: item.getPosition().lng()
					}
				}
			}),
			putData = map(this.state.customPoints, function (i) {
				return i.status == 'deleted' ? i.gtuInfoId : null;
			});

		$.when([
			this.props.task.addGtuDots(postData),
			this.props.task.removeGtuDots(putData)
		]).done(function () {
			each(self.state.customPoints, function (point) {
				if (point && point.status == 'deleted') {
					remove(self.state.customPoints, point);
				} else if (point && point.status == 'new') {
					point.status = 'added';
				}
			});

			self.forceUpdate();
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
			newAddedPoints = filter(this.state.customPoints, function (i) {
				return i.status == 'new' || i.status == 'deleted';
			});
		canUndoSave = isEmpty(newAddedPoints) ? false : true;
		if (!canUndoSave) {
			var undoButton = <button className="button float-right" disabled onClick={this.onUnderGtu}><i className="fa fa-undo"></i>Undo</button>;
			var saveButton = <button className="button float-right" disabled onClick={this.onSaveGtu}><i className="fa fa-save"></i>Save</button>;
		} else {
			var undoButton = <button className="button float-right" onClick={this.onUnderGtu}><i className="fa fa-undo"></i>Undo</button>;
			var saveButton = <button className="button float-right" onClick={this.onSaveGtu}><i className="fa fa-save"></i>Save</button>;
		}

		return (
			<div className="section row">
				<div className="small-12 columns">
					<div className="section-header">
						<div className="row">
							<div className="small-12 column">
								<h5>Edit GTU - {this.props.task.get('Name')}</h5>
							</div>
							<div className="small-12 column">
								<nav aria-label="You are here:" role="navigation">
									<ul className="breadcrumbs">
										<li><a href="#">Control Center</a></li>
										<li><a href={'#report/' + this.props.task.get('Id')}>Report</a></li>
										<li>
											<span className="show-for-sr">Current: </span> Edit GTU
										</li>
									</ul>
								</nav>
							</div>
						</div>
					</div>
				</div>
				<div className="small-12 medium-7 large-9 columns">
					{gtuList.map(function(gtu) {
						return (
							<button key={gtu.get('Id')} className={self.state.activeGtu == gtu ? " button" : "button"} onClick={self.setActiveGtu.bind(null, gtu.get('Id'))}>
								<i className={self.state.activeGtu == gtu ? "fa fa-map-marker" : "fa fa-stop"} style={{color: gtu.get('UserColor')}}></i>
								{gtu.get('ShortUniqueID')}
							</button>
						);
					})}
				</div>
				<div className="small-12 medium-5 large-3 columns">
					{saveButton}
					{undoButton}
					<button className="button float-right" onClick={this.onReCenter}><i className="fa fa-refresh"></i>ReCenter</button>
				</div>
				<div className="small-12 columns" ref="mapArea"></div>
			</div>
		);
	}
});