/*******************************************************************************
 * Submap controllers.
 ******************************************************************************/ 

var addressDao;
var submapDao;
var subMapList;
var submapForm;
var boundaryLayer;
var boundaryManager;

// Use an interval object to wait for the map to load successfully
var submap_init_interval = setInterval('on_init_submap_objects()', 100);

// Initialize global objects for submap operations
function on_init_submap_objects() {
	if (map) {
		addressDao = new AddressDao();
		submapDao = new SubMapDao();
		subMapList = new SubMapList();
		submapForm = new SubMapForm(subMapList);
		boundaryLayer = new BoundaryLayer(map);
		boundaryManager = new BoundaryManager(boundaryLayer, map);
		submapForm.boundaryManager = boundaryManager;
		
		clearInterval(submap_init_interval);
	}
	
	return map;
}

// Perform action when the user requests to load addresses
function on_load_addresses() {
	var addresses = addressDao.get_all();
	var last_id;
	
	for (var i = 0; i < addresses.length; i++) {
		var addr = addresses[i];		
		// If the address hasn't been loaded
		if (submapDao.get_by_address(addr.address).length == 0) {
			// Create and save this submap
			var submap = new SubMap(
				addr.address, 
				{ lat: addr.coord.lat, lon:addr.coord.lon }, 
				addr.radius,
				500 * (i + 1),
				0.01 * (i + 1)
			);
			submapDao.add_submap(submap);
			// Display this submap in submap list
			subMapList.append_submap(submap);
			subMapList.select_submap(submap);
			subMapList.render_info(submap);
			// Draw the boundary of this submap
			boundaryManager.draw_boundary(
				subMapList.boundary_visible(), {
					id: submap.id,
					lat: submap.center.lat,
					lon: submap.center.lon,
					radius: submap.radius,
					state: BoundaryState.NORMAL});
			
			last_id = submap.id;
		}
	}
	
	if (last_id) {
		boundaryManager.highlight(subMapList.boundary_visible(), last_id);
	}
}

// Perform action when the user requests to create a new submap.
function on_create_submap() {
	// Show submap editing form
	submapForm.clear();
	submapForm.show();	
	// Show a default boundary for the new submap
	boundaryManager.draw_boundary(
		subMapList.boundary_visible(), {
			lat: submapForm.submap.center.lat,
			lon: submapForm.submap.center.lon,
			radius: submapForm.submap.radius});
}

// Perform action when the user requests to delete a submap
function on_delete_submap() {
	alert('Not implemented');
}

// Perform action when the user requests to save the submap being edited/created
function on_save_submap() {
	// Make sure data is synchronized to SubMapForm.submap
	submapForm.save();
	
	// If a submap is being edited
	if (submapForm.is_editing()) {
		// Update displayed information of the submap
		subMapList.update_submap(submapForm.submap);
	} else {
		// Save this submap and display it in the submap list
		insert_submap_item();
	}
	
	// Hide and reset the submap form
	submapForm.hide_and_clear();
}

function insert_submap_item() {
	submapDao.add_submap(submapForm.submap);
	
	subMapList.append_submap(submapForm.submap);
	subMapList.select_submap(submapForm.submap);
	boundaryManager.show_boundary(subMapList.boundary_visible());
}

// Perform action when the user clicks on a submap
function on_select_submap(div) {
	var submap = submapDao.get_submap(div.id);
	subMapList.select_submap(submap);
	boundaryManager.highlight(subMapList.boundary_visible(), submap.id);
}

// Perform action when the user requests to pick or stop picking center
function on_pick_a_center() {
	submapForm.toggle_pick_button();
}

// Pick a submap center when the user clicks on the map
function on_picking_center(e) {
	if (submapForm.picking_center) {
		var pixel = new VEPixel(e.mapX, e.mapY);
		var submap_ll = map.PixelToLatLong(pixel);
		submapForm.set_center(submap_ll.Latitude, submap_ll.Longitude);

		boundaryManager.adjust_boundary_center(
			submapForm.submap.id,
			submapForm.get_center().lat,
			submapForm.get_center().lon,
			subMapList.boundary_visible());
	}
}

// Perform action when the user requests to display the boundary
function on_show_boundary() {
	boundaryManager.show_boundary(subMapList.boundary_visible());
}

// Perform action when the user requests to edit a submap
function on_edit_submap() {
	if (!subMapList.has_submap_selected()) {
		alert('To edit, please select a sub map first.');
	} else {
		var id = subMapList.get_selected_id();		
		// Show the selected in submap editing form
		submapForm.submap = submapDao.get_submap(id);
		submapForm.set_editing();
		submapForm.show();
		// Highlight the boundary of the submap being edited
		boundaryManager.highlight(subMapList.boundary_visible(), id);
	}
}

// Perform action when the user requests to increase the center latitude
function on_increase_center_lat() {
	submapForm.increase_center_lat();
}

// Perform action when the user requests to decrease the center latitude
function on_decrease_center_lat() {
	submapForm.decrease_center_lat();
}

// Perform action when the user requests to increase the center longitude
function on_increase_center_lon() {
	submapForm.increase_center_lon();
}

// Perform action when the user requests to decrease the center longitude
function on_decrease_center_lon() {
	submapForm.decrease_center_lon();
}
