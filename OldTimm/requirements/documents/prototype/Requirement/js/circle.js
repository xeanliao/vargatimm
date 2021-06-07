/**
 * Class Circle - responsible for the drawing of circles on a shape layer.
 * 
 * Public methods:
 * 		draw
 * 
 * Constructor parameters:
 * 		@layer - a VEShapeLayer object on which to draw the circle.
 */
function Circle(layer) {	
	this._layer = layer;
}

Circle.prototype = {
	_layer: undefined,
	
	draw: function(origin, radius, color, width) {
	  var earthRadius = 6371;
		  
	  //latitude in radians
	  var lat = (origin.Latitude*Math.PI)/180; 
			
	  //longitude in radians
	  var lon = (origin.Longitude*Math.PI)/180; 
	  //angular distance covered on earth's surface
	  var d = parseFloat(radius)/earthRadius;  
	  var points = new Array();
	  for (i = 0; i <= 360; i++) {
		var point = new VELatLong(0,0)            
		var bearing = i * Math.PI / 180; //rad
		point.Latitude = Math.asin(Math.sin(lat)*Math.cos(d) + 
		  Math.cos(lat)*Math.sin(d)*Math.cos(bearing));
		point.Longitude = ((lon + Math.atan2(Math.sin(bearing)*Math.sin(d)*Math.cos(lat),
		  Math.cos(d)-Math.sin(lat)*Math.sin(point.Latitude))) * 180) / Math.PI;
		point.Latitude = (point.Latitude * 180) / Math.PI;
		points.push(point);
	  }

	  var circle = new VEShape(VEShapeType.Polyline, points);
	  circle.HideIcon();
	  circle.SetLineColor(new VEColor(0,0,200,0.5));
	  if (color) circle.SetLineColor(color);
	  circle.SetFillColor(new VEColor(0,255,0,0.3));
	  circle.SetLineWidth(4);
	  if (width) circle.SetLineWidth(width);
	  this._layer.AddShape(circle);
	  return points;
	}      
}
