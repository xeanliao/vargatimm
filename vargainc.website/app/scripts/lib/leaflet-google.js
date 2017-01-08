L.GoogleTile = L.Layer.extend({

	// @section
	// @aka ImageOverlay options
	options: {
		pane: 'tilePane',
		interactive: false,
	},

	initialize: function (options) {
		L.setOptions(this, options);
	},
	onAdd: function () {
		if (!this._googleMap) {
			this._initMap();
		}

		this.getPane().appendChild(this._container);

		this._reset();

	},
	onRemove: function () {
		L.DomUtil.remove(this._container);
		//need destory google map
	},
	getElement: function () {
		return this._container;
	},
	isLoading: function () {
		return this._loading || false;
	},
	bringToFront: function () {
		if (this._map) {
			L.DomUtil.toFront(this._container);
		}
		return this;
	},
	bringToBack: function () {
		if (this._map) {
			L.DomUtil.toBack(this._container);
		}
		return this;
	},
	getEvents: function () {
		return {
			zoom: this._zoom,
			viewreset: this._update,
			moveend: this._update,
			resize: this._resize,
			move: this._update,
		};
	},
	_initMap: function () {
		if (!this._container) {
			var container = this._container = L.DomUtil.create('div', 'leaflet-google-layer ');
		} else {
			var container = this._container;
		}

		this._googleMap = new google.maps.Map(container, {
			disableDefaultUI: true,
			disableDoubleClickZoom: true,
			draggable: false,
			keyboardShortcuts: false,
			scrollwheel: false,
			animatedZoom: false,
			animated: false,
			animate: false
		});
	},
	_resize: function () {
		let size = this._map.getSize(),
			container = this._container;
		container.style.position = 'absolute';	
		container.style.width = `${size.x * 1.2}px`;
		container.style.height = `${size.y * 1.2}px`;
		container.style.left = `-${size.x * 0.1}px`;
		container.style.top = `-${size.y * 0.1}px`;
	},
	_zoom: function () {
		let zoom = this._map.getZoom();
		this._googleMap.setZoom(zoom);
	},
	_reset: function () {
		var map = this._map;
		if (!map) {
			return;
		}
		this._resize();
		this._zoom();
		this._update();
	},
	_update: function (e) {
		let center = this._map.getCenter();
		this._googleMap.setCenter(center);
		L.DomUtil.setPosition(this._container, this._map.containerPointToLayerPoint([0, 0]), false);
	}
});

// @factory L.imageOverlay(imageUrl: String, bounds: LatLngBounds, options?: ImageOverlay options)
// Instantiates an image overlay object given the URL of the image and the
// geographical bounds it is tied to.
L.googleTile = function (options) {
	return new L.GoogleTile(options);
};