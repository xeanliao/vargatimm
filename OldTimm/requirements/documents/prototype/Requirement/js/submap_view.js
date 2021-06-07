/*******************************************************************************
 * Submap view objects.
 * 
 * The following objects are defined as submap view objects.
 * 		SubMapList
 * 		SubMapForm
 * 		BoundaryLayer
 * 		BoundaryManager
 * 
 * In addition, some objects are defined as assistant objects for the main 
 * objects above. These objects include:
 * 		BoundaryState
 * 		BoundaryColor
 * 		BoundaryWidth
 ******************************************************************************/ 

/**
 * BoundaryState - defines boundary states.
 */ 
BoundaryState = { 
	NORMAL: 0,
	SELECTED: 1
}

/**
 * BoundaryColor - defines boundary colors related to boundary states.
 */ 
BoundaryColor = {
	NORMAL: new VEColor(0,0,200,0.5),
	SELECTED: new VEColor(0,250,0,0.8)
}

/**
 * BoundaryState - defines boundary widths related to boundary states.
 */ 
BoundaryWidth = {
	NORMAL: 4,
	SELECTED: 4
}

/**
 * Class SubMapList - responsible for updating the submap list and getting 
 * information displayed on the submap list.
 * 
 * Public methods:
 * 		boundary_visible
 * 		get_selected_id
 * 		has_submap_selected
 * 		append_submap
 * 		render_info
 * 		select_submap
 * 		update_submap
 */
function SubMapList() {}

SubMapList.prototype = {
	// Return a value indicating whether submap boundary should be visible
	boundary_visible: function() {
		return $('#cb_boundary')[0].checked;
	},
	// Return the 'id' value of the selected html element, which uniquely
	// identifies a submap in the submap list
	get_selected_id: function() {
		return $('#submap_list div.selected')[0].id;
	},
	// Return a value indicating whether any submap is selected
	has_submap_selected: function() {
		return $('#submap_list div.selected').length > 0;
	},
	// Generate a html element for the specified submap, and append it to the 
	// end of the submap list
	append_submap: function(submap) {
		$('#submap_list').append([
			'<div id="',
			submap.id,
			'" onclick="javascript:on_select_submap(this)">',
			submap.title,
			'</div>'].join(''));
	},
	render_info: function(submap) {
		$('#total')[0].innerHTML = submap.total;
		$('#pen')[0].innerHTML = '{0}%'.replace('{0}', submap.pen * 100);
	},
	// Change the style of the specified submap so that it looks selected
	select_submap: function(submap) {
		$('#submap_list div.selected').removeClass('selected');
		$('#' + submap.id).addClass('selected');
		this.render_info(submap);
	},
	// Update the display title of the specified submap
	update_submap: function(submap) {
		$('#' + submap.id)[0].innerHTML = submap.title;
	}
}

/**
 * Class SubMapForm - provides a user interface for the user to edit submap.
 * 
 * Public methods:
 * 		get_center
 * 		get_picked_radius
 * 		is_editing
 * 		clear
 * 		hide
 * 		hide_and_clear
 * 		increase_center_lat
 * 		increase_center_lon
 * 		decrease_center_lat
 * 		decrease_center_lon
 * 		save
 * 		set_center
 * 		set_editing
 * 		show
 * 		toggle_pick_button
 * 
 * Public properties:
 * 		submap
 * 
 * Constructor parameters:
 * 		@submapList - a SubMapList object, required.
 */
function SubMapForm(submapList) {
	this.submap = this._default_submap;
	this._submapList = submapList;
}

SubMapForm.prototype = {
	_default_submap: new SubMap('', {lat: 36.9, lon:-119.1}, 1),
	// Indicating whether a submap is being edited or a new submap is being
	// created
	_is_editing: false,
	_lat_change_step: 0.001,
	_lon_change_step: 0.001,
	// The submap which is being created or whose information is being edited
	submap: undefined,
	// Indicating whether the user is picking the center value
	picking_center: false,
	// Boundary manager
	boundaryManager: undefined,
	
	// Return the center coordinates displayed on the form
	get_center: function() {
		return {
			lat: parseFloat($('#submap_lat')[0].innerHTML),
			lon: parseFloat($('#submap_lon')[0].innerHTML)
		}
	},
	// Return the radius value displayed
	get_picked_radius: function() {
		return $('#slider').slider('value');
	},
	increase_center_lat: function() {
		var l = parseFloat($('#submap_lat')[0].innerHTML);
		if (l + this._lat_change_step >= 90.00) {
			$('#submap_lat')[0].innerHTML = '90.00';
		} else {
			$('#submap_lat')[0].innerHTML = 
				(l + this._lat_change_step).toFixed(3);
		}
		this._draw_boundary();
	},
	decrease_center_lat: function() {
		var l = parseFloat($('#submap_lat')[0].innerHTML);
		if (l - this._lat_change_step <= -90.00) {
			$('#submap_lat')[0].innerHTML = '-90.00';
		} else {
			$('#submap_lat')[0].innerHTML = 
				(l - this._lat_change_step).toFixed(3);
		}
		this._draw_boundary();
	},
	increase_center_lon: function() {
		var l = parseFloat($('#submap_lon')[0].innerHTML);
		if (l + this._lon_change_step >= 180.00) {
			$('#submap_lon')[0].innerHTML = '180.00';
		} else {
			$('#submap_lon')[0].innerHTML = 
				(l + this._lon_change_step).toFixed(3);
		}
		this._draw_boundary();
	},
	decrease_center_lon: function() {
		var l = parseFloat($('#submap_lon')[0].innerHTML);
		if (l - this._lon_change_step <= -180.00) {
			$('#submap_lon')[0].innerHTML = '-180.00';
		} else {
			$('#submap_lon')[0].innerHTML = 
				(l - this._lon_change_step).toFixed(3);
		}
		this._draw_boundary();
	},
	// Return a valuding indicating whether a submap is being edited, which is
	// opposite to creating a new submap
	is_editing: function() {
		return this._is_editing;
	},
	// Reset the form
	clear: function() {
		// reset data
		this.submap = new SubMap('', {lat: 36.9, lon:-119.1}, 1);
		
		// clear field values
		$('#submap_title')[0].value = '';
		$('#slider').slider('value', 1);
		$('#radius')[0].innerHTML = 1;
		
		this.picking_center = false;
		$('#pick_button')[0].innerHTML = 'Pick a Center';
		
		this._is_editing = false;
	},
	// Hide the form
	hide: function() {
		$('#submap_create_form').addClass('hidden');
		$('#submap_list').removeClass('hidden');
	},
	// Hide and reset the form
	hide_and_clear: function() {
		this.hide();
		this.clear();
	},
	// Toggle between 'Pick' and 'Stop'
	toggle_pick_button: function() {
		this.picking_center = !(this.picking_center);
		$('#pick_button')[0].innerHTML = 
			this.picking_center ? 'Stop' : 'Pick a Center';
	},
	// Update the submap with new values displayed on the form
	save: function() {
		this.submap.title = $('#submap_title').val();
		this.submap.center.lat = parseFloat($('#submap_lat')[0].innerHTML);
		this.submap.center.lon = parseFloat($('#submap_lon')[0].innerHTML);
		this.submap.radius = parseFloat($('#radius')[0].innerHTML);
	},	
	// Display the specified center
	set_center: function(lat, lon) {
		$('#submap_lat')[0].innerHTML = lat.toFixed(3);
		$('#submap_lon')[0].innerHTML = lon.toFixed(3);
	},
	// Change the status of the form to 'editing'
	set_editing: function() {
		this._is_editing = true;
	},
	// Show the form
	show: function() {
		// create slider
		this._create_slider();
		
		// show submap form
		$('#submap_create_form').removeClass('hidden');
		$('#submap_list').addClass('hidden');
		
		// render form fields with submap data
		this._render_submap();
	},
	// Create a slider
	_create_slider: function() {
		var thisObj = this;
		
		$('#slider').slider({
			range: "min",
			value: 1,
			min: 0,
			max: 30,
			slide: function(event, ui) {
				$("#radius")[0].innerHTML = ui.value.toFixed(2);
				thisObj._draw_boundary();
			}
		})
	},
	_draw_boundary: function() {
		this.boundaryManager.draw_boundary(
			this._submapList.boundary_visible(), {
				id: this.submap.id,
				lat: parseFloat($('#submap_lat')[0].innerHTML), 
				lon: parseFloat($('#submap_lon')[0].innerHTML), 
				radius: parseFloat($('#radius')[0].innerHTML),
				state: BoundaryState.SELECTED
			});
	},
	// Display submap information on the form
	_render_submap: function() {
		$('#submap_title')[0].value = this.submap.title;
		$('#slider').slider('value', this.submap.radius);
		$('#submap_lat')[0].innerHTML = this.submap.center.lat.toFixed(3);
		$('#submap_lon')[0].innerHTML = this.submap.center.lon.toFixed(3);
		$('#radius')[0].innerHTML = this.submap.radius.toFixed(2);
	}
}

/**
 * Class BoundaryLayer - responsible for managing the state of the boundary 
 * shape layer.
 * 
 * Public methods:
 * 		get_shape_layer
 * 		clear_shapes
 * 		load
 * 
 * Constructor parameters:
 * 		@map - a VEMap object on which to load the boundary layer, required
 */
function BoundaryLayer(map) {
	this._map = map;
}

BoundaryLayer.prototype = {
	// Indicating whether this boundary layer has beend loaded
	_loaded: false,
	// The map object holds this boundary layer
	_map: undefined,
	// The shape layer object
	_shape_layer: new VEShapeLayer(),
	// Rerutn the shape layer object used to hold shapes
	get_shape_layer: function() {
		return this._shape_layer;
	},
	// Load the shape layer to the map
	load: function() {
		if (!this._loaded) {
			this._map.AddShapeLayer(this._shape_layer);
			this._loaded = true;
		}
	},
	// Clear shapes contained by the shape layer
	clear_shapes: function() {
		this._shape_layer.DeleteAllShapes();
	}
}

/**
 * Class BoundaryManager - responsible to control the drawing of boundaries.
 * 
 * Public methods:
 * 		adjust_boundary_center
 * 		show_boundary
 * 		draw_boundary
 * 		highlight
 * 
 * Constructor parameters:
 * 		@boundaryLayer - a BoundaryLayer object, required
 * 		@map - a VEMap object on which to show the boundaries, required
 */ 
function BoundaryManager(boundaryLayer, map) {
	this._boundaryLayer = boundaryLayer;
	this._map = map;
	// Set up mappings betweeen boundary states and colors
	this._colors[BoundaryState.NORMAL] = BoundaryColor.NORMAL;
	this._colors[BoundaryState.SELECTED] = BoundaryColor.SELECTED;
	// Set up mappings betweeen boundary states and boundary widths
	this._widths[BoundaryState.NORMAL] = BoundaryWidth.NORMAL;
	this._widths[BoundaryState.SELECTED] = BoundaryWidth.SELECTED;
}

BoundaryManager.prototype = {
	_items: [],
	_colors: [],
	_widths: [],
	
	adjust_boundary_center: function(id, lat, lon, visible) {
		var s = this._items[id];
		s.lat = lat;
		s.lon = lon;
		this.show_boundary(visible);
	},
	// Show or hide boundary
	show_boundary: function(visible) {
		// Make boundary layer ready for use
		this._boundaryLayer.load();
		this._boundaryLayer.clear_shapes();

		// Draw boundaries
		this._show_items(visible);
	},
	
	draw_boundary: function(visible, spec) {
		// Save the spec to local
		this._items[spec.id] = spec;

		// Make sure only one boundary is highlighted
		if (spec.state == BoundaryState.SELECTED) {
			this._clear_state();
			this._items[spec.id].state = BoundaryState.SELECTED;
		}

		// Make boundary layer ready for use
		this._boundaryLayer.load();
		this._boundaryLayer.clear_shapes();

		// Draw boundaries
		this._show_items(visible);
	},
	
	highlight: function(visible, id) {
		this._clear_state();
		this._items[id].state = BoundaryState.SELECTED;

		// Make boundary layer ready for use
		this._boundaryLayer.load();
		this._boundaryLayer.clear_shapes();
		
		// Draw boundaries
		this._show_items(visible);
		this._set_map_view(this._items[id]._points, visible);
	},
	
	_clear_state: function() {
		for (var i in this._items) {
			this._items[i].state = BoundaryState.NORMAL;
		}
	},
	
	_set_map_view: function(points, visible) {
		if (visible) this._map.SetMapView(points);
	},
	
	_show_items: function(visible) {
		if (visible) {
			for (var i in this._items) {
				var s = this._items[i];
				var color = this._colors[s.state];
				var width = this._widths[s.state];
				var points = this._do_draw_boundary(
					s.lat, s.lon, s.radius, color, width);
				this._items[i]._points = points;
			}
		}
	},

	// Draw a boundary according to the specification
	_do_draw_boundary: function(lat, lon, radius, color, width) {
		// Draw a circle which indicates the boundary
		return new Circle(this._boundaryLayer.get_shape_layer()).draw(
			new VELatLong(lat, lon), radius, color, width);
	}
}
