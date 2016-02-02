define([
	'jquery',
	'alertify',
	//'async!googleMap'
], function ($, alertify) {
	var Map = function () {};
	$.extend(Map.prototype, {
		_map: null,
		_overlay: [],
		_page: null,
		init: function (option, location) {
			$(document).off('click', '.btnApplyMapChange');
			$(document).off('click', '.dot-item');
			$(document).on('click', '.btnApplyMapChange', $.proxy(this.changeMap, this));
			$(document).on('click', '.dot-item', this.changeRadii);
			var mapEditor = $('#mapEditor');
			var mapContainer = $('#map');
			if (this._map == null) {
				if (typeof google.maps.Map === 'undefined') {
					alertify.okBtn("Got It!")
						.alert("Oops! google map load failed. please contact us.")
						.reset();
					return;
				}
				this._map = new google.maps.Map($('#map')[0], {
					center: new google.maps.LatLng(40.744556, -73.987378),
					zoom: 18,
					mapTypeId: google.maps.MapTypeId.ROADMAP,
					disableDefaultUI: true
				});
			}
			var option = $("#mapEditor").data('option');
			var locations = $("#mapEditor").data('locations');
			this._page = mapEditor.data('page');
			/**
			 * restore dotsRadii
			 */
			if (option.type == 'DMap') {
				mapEditor.find('.dotsRadii').removeClass('hide');
				mapEditor.find('.dot-item[size=' + option.gTUDotsRadii + ']').addClass('active');
			}
			this.drawMap(option, locations);
		},
		drawMap: function (option, mapData) {
			var mutliBundary = [];
			if (!$.isArray(mapData)) {
				mutliBundary.push(mapData);
			} else {
				mutliBundary = mapData;
			}
			var mapBounds = new google.maps.LatLngBounds();
			var self = this;
			$.each(mutliBundary, function () {
				var fillColor = 'rgb(' + this.color.r + ',' + this.color.g + ',' + this.color.b + ')';
				var boundaryPolyline = new google.maps.Polygon({
					paths: this.bundary,
					strokeColor: fillColor,
					strokeOpacity: 1,
					strokeWeight: 6,
					fillOpacity: 0.6,
					fillColor: fillColor,
					map: self._map
				});
				self._overlay.push(boundaryPolyline);
				$.each(this.bundary, function () {
					var point = new google.maps.LatLng(this.lat, this.lng);
					mapBounds.extend(point);
				});
			});

			self._map.fitBounds(mapBounds);

			$("#mapEditor").data('locations', null);
		},
		changeRadii: function () {
			var container = $(this).parents('.dotsRadii').find('.dot-item').removeClass('active');
			$(this).addClass('active');
		},
		changeMap: function () {
			console.log('click apply map change');
			var editor = $("#mapEditor");
			//var center = this._map.getCenter();
			var bounds = this._map.getBounds();
			var sw = bounds.getSouthWest();
			var ne = bounds.getNorthEast();
			var newRadii = editor.find('.dot-item.active').attr('size');
			var option = editor.data('option');
			//console.log('bounds', bounds.lat(), bounds.lng());
			var haveChanges = false;
			// if (option.boundSWLat != sw.lat() || option.boundSWLng != sw.lng()
			// 	|| option.boundNELat != ne.lat() || option.boundNELng != ne.lng()) {
			// 	haveChanges = true;
			// }
			if (option.type === 'DMap' && option.gTUDotsRadii != newRadii) {
				haveChanges = true;
			}
			var params = option;
			if (haveChanges) {
				$.extend(params, {
					gTUDotsRadii: newRadii
				});
				console.log('new option', params);
				this._page.data('option', params);
			}
			this._page.find('.map-container').html('<div class="loading hexdots-loader"></div>');
			switch (params.type) {
			case 'Campaign':
				this.App.onLoadCampaignImage(params);
				break;
			case 'SubMap':
				this.App.onLoadSubMapImage(params);
				break;
			case 'DMap':
				this.App.onLoadDMapImage(params);
				break;
			default:
				break;
			};

			editor.foundation('reveal', 'close');
		},
		close: function () {
			console.log('map editor close');
			var editor = $("#mapEditor");
			if (this._map != null) {
				if (this._overlay && this._overlay.length > 0) {
					$(this._overlay).each(function () {
						this.setMap(null);
					});
				}
				this._overlay = [];
				this._page = null;
				editor.find('.dot-item').removeClass('active');
				editor.find('.dotsRadii').addClass('hide');
			}
			editor.data('option', null);
		}
	});
	return Map;
});