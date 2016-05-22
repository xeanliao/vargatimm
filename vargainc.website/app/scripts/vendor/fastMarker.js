(function (factory) {
	if (typeof define === 'function' && define.amd) {
		define([
			'jquery',
			'd3',
			'async!//maps.googleapis.com/maps/api/js?key=AIzaSyBOhGHk0xE1WEG0laS4QGRp_AGhtB5LMHw&libraries=drawing'
		], factory);
	} else if (typeof exports === 'object') {
		module.exports = factory();
	} else {
		window.FastMarker = factory();
	}
}(function () {
	var hashMap = {
		keys: [],
		values: {},
		set: function (k, v) {
			var index = this.keys.indexOf(k);
			if (index > -1) {
				this.values[index + ''] = v;
			} else {
				this.keys.push(k);
				this.values[(this.keys.length - 1) + ''] = v;
			}
		},
		get: function (k) {
			var index = this.keys.indexOf(k);
			if (index > -1) {
				return this.values[index + ''];
			} else {
				return null;
			}
		},
		has: function (k) {
			return this.keys.indexOf(k) > -1;
		},
		delete: function (k) {
			var index = this.keys.indexOf(k);
			if (index > -1) {
				this.keys = this.keys.splice(index, 1);
				delete this.values[index + ''];
			}
		}
	}

	function FastMarkerLayer(map) {
		this.setMap(map);
		this._markers = [];
	}
	FastMarkerLayer.prototype = new google.maps.OverlayView();
	FastMarkerLayer.prototype.addMarker = function (marker) {
		this._markers.push(marker);
		this.draw();
	}
	FastMarkerLayer.prototype.removeMarker = function (marker) {
		var index = this._markers.indexOf(marker);
		if (index > -1) {
			if(this._markers.length == 1){
				this._markers = [];
				console.log('FastMarker remove last one');
			}else{
				this._markers = this._markers.slice(index, index + 1);
			}
		}
		this.draw();
	}
	FastMarkerLayer.prototype.onAdd = function () {
		var map = this.getMap(),
			panes = this.getPanes(),
			projection = this.getProjection(),
			targetLayer = panes.overlayLayer;
		map.overlayMapTypes.insertAt(0, this);
		if (projection && panes && targetLayer && $('.fastMarkerLayer', targetLayer).size() == 0) {
			var center = projection.fromLatLngToDivPixel(map.getCenter());
			this.width = center.x * 2;
			this.height = center.y * 2;
			var markerLayer = d3.select(targetLayer)
				.append("svg")
				.attr('style', 'position:absolute; top: 0; left: 0;')
				.attr('class', 'fastMarkerLayer');
			this.markerLayer = markerLayer;
		}
	}
	FastMarkerLayer.prototype.onRemove = function () {
		var map = this.getMap(),
			panes = this.getPanes(),
			targetLayer = panes.overlayLayer;
		$('.fastMarkerLayer', targetLayer).remove();
	}
	FastMarkerLayer.prototype.drawTimeout = null;
	FastMarkerLayer.prototype.draw = function () {
		window.clearTimeout(this.drawTimeout);
		this.drawTimeout = window.setTimeout($.proxy(this._draw, this), 500);
	}
	FastMarkerLayer.prototype._draw = function () {
		console.log('FastMarker _draw');
		var projection = this.getProjection(),
			layer = this.markerLayer,
			topLeft = {
				x: null,
				y: null
			},
			bottomRight = {
				x: null,
				y: null
			};
		if(!layer || !projection){
			console.log('FastMarker not ready error.');
			return;
		}
		layer.selectAll('*').remove();
		console.log('FastMarker draw', this._markers.length);
		$.each(this._markers, function () {
			var position = projection.fromLatLngToDivPixel(this.options.position);
			topLeft.x = topLeft.x == null || topLeft.x > position.x ? position.x : topLeft.x;
			topLeft.y = topLeft.y == null || topLeft.y > position.y ? position.y : topLeft.y;
			bottomRight.x = bottomRight.x == null || bottomRight.x < position.x ? position.x : bottomRight.x;
			bottomRight.y = bottomRight.y == null || bottomRight.y < position.y ? position.y : bottomRight.y;
		});
		//Todo: we just extend boundary 50px should use marker max size to extend
		topLeft.x -= 50;
		topLeft.y -= 50;
		bottomRight.x += 50;
		bottomRight.y += 50;
		layer.attr('width', Math.abs(bottomRight.x - topLeft.x))
			.attr('height', Math.abs(bottomRight.y - topLeft.y))
			.attr('style', 'position:absolute; left: ' + topLeft.x + 'px; top: ' + topLeft.y + 'px;');

		$.each(this._markers, function () {
			var position = projection.fromLatLngToDivPixel(this.options.position);
			layer.append('path')
				.attr('d', this.options.icon.path)
				.attr('fill', this.options.icon.fillColor)
				.attr('stroke', this.options.icon.strokeColor)
				.attr("transform", function (d) {
					return "translate(" + (position.x - topLeft.x) + ", " + (position.y - topLeft.y) + ")";
				});
		});
	}

	function FastMarker(options) {
		this.options = options;
		if (this.options && this.options.position) {
			this.options.position = new google.maps.LatLng(this.options.position.lat, this.options.position.lng);
		}
		if (options && options.map) {
			var map = options.map;
			this.setMap(map);
		}
	}

	FastMarker.prototype.setMap = function (map) {
		if (map != null) {
			var layer;
			if (!hashMap.has(map)) {
				layer = new FastMarkerLayer(map);
				hashMap.set(map, layer);
			} else {
				layer = hashMap.get(map);
			}
			this.Layer = layer;
			this.Map = map;
			layer.addMarker.call(this.Layer, this);
		} else {
			if (this.Layer && this.Map) {
				this.Layer.removeMarker.call(this.Layer, this);
				this.Map = null;
			}
		}
	}

	return FastMarker;
}));