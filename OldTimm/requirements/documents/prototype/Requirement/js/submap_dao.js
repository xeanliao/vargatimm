/*******************************************************************************
 * Data access objects for submap operations.
 * 
 * The following objects are defined as submap-relevant data access objects.
 * 		SubMapDao
 * 		AddressDao
 ******************************************************************************/

/**
 * Class SubMapDao - responsible for storing and fetching SubMaps locally or
 * remotely.
 * 
 * Public methods:
 * 		get_by_address
 * 		get_submap
 * 		add_submap
 * 		delete_submap
 */
function SubMapDao() {
}

SubMapDao.prototype = {
	// Internal representation for SubMap collection
	_items: new Array(),
	
	get_by_address: function(address) {
		var addrs = new Array();
		for (var i in this._items) {
			if (this._items[i].address == address) {
				addrs.push(this._items[i]);
			}
		}
		return addrs;
	},
	
	// Return SubMap object by sub map id.
	get_submap: function(submap_id) {
		var submap;
		for (var i = 0; i < this._items.length; i++) {
			submap = this._items[i];
			if (submap_id == submap.id) break;
		}
		return submap;
	},
	
	// Add a new SubMap
	add_submap: function(submap) {
		this._items.push(submap);
	},
	
	delete_submap: function(submap_id) {
	}
}

/**
 * Class AddressDao - responsible for storing and fetching Addresses locally
 * or remotely.
 * 
 * Public methods:
 * 		get_all
 */
function AddressDao() {
	this._items = [
		new Address('St 1001', {lat: 37.30, lon: -120.00}, 10),
		new Address('St Field 1002', {lat: 37.34, lon: -118.80}),
		new Address('Great 1003', {lat: 35.90, lon: -118.00}, 20)
	];
}

AddressDao.prototype = {
	// Internal representation for Address collection
	_items: undefined,
	
	get_all: function() {
		return this._items;
	}
}
