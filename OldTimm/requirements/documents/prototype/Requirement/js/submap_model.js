/*******************************************************************************
 * Entity classes relevant to sub maps.
 ******************************************************************************/

/**
 * Class SubMap - represents a sub map entity on the user interface.
 * 
 * Public properties:
 * 		id
 * 		address
 * 		title
 * 		center
 * 		radius
 * 		total
 * 		pen
 * 
 * Please note the 'id' of a SubMap is automatically generated.
 * 
 * Constructor parameters:
 * 		@address - the street address which is the submap center, required
 * 		@center - the center coordinates of the submap, optional
 * 		@radius - the radius of the submap boundary circle, optional
 * 		@total - the 'total' value of the submap, optional
 * 		@pen - the 'penetration' value of the submap, optional
 */
function SubMap(address, center, radius, total, pen) {
	this.id = 'submap_' + Math.random().toString().replace('.', '');
	this.address = address;
	this.title = address;
	if (center) this.center = center;
	if (radius) this.radius = radius;
	if (total) this.total = total;
	if (pen) this.pen = pen;
}

SubMap.prototype = {
	id: '',
	address: '',
	title: '',
	center: {lat: 36.9, lon:-119.1},
	radius: 1,
	total: 0,
	pen: 0.00
}

/**
 * Class Address - represents a sub map address loaded by the user.
 * 
 * Public properties:
 * 		address
 * 		coord
 * 		radius
 * 
 * Constructor parameters:
 * 		@address - the street address, required
 *		@coord - the coordinates of the street address, optional
 * 		@radius - the radius in kilometer, optional
 */
function Address(address, coord, radius) {
	this.address = address;
	if (coord) this.coord = coord;
	if (radius) this.radius = radius;
}

Address.prototype = {
	address: '',
	coord: {lat: 0, lon: 0},
	radius: 1
}
