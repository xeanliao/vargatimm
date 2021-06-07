// map related objects for submap.html

var map = null;

function GetMap() {
	var center = new VELatLong(36.9, -119.1);
	var zoomLevel = 8;
	
	map = new VEMap('myMap');
	map.LoadMap(center, zoomLevel);
	
	map.AttachEvent("onclick", on_map_click);
}

function on_map_click(e) {
	present_areas(e);
	//pick_submap_center(e);
}

function present_areas(e) {
	var shape = map.GetShapeByID(e.elementID);
	
	if (!shape) return;
	
	// ensure shapes are loaded
	do_load_5zips();
	do_load_trks();
	do_load_bgs();

	// case 1: 3zip area selected
	if (shape.code && shape.code.length == 3) {
		shape.selected = !shape.selected;
		highlight_3zip_area(shape, shape.selected);
		select_5zip_areas(shape, shape.selected);
	} 
	// case 2: 5zip area selected
	else if (shape.code && shape.code.length == 5) {
		shape.selected = !shape.selected;
		highlight_5zip_area(shape, shape.selected);
		select_trks(shape, shape.selected);
	}
	// case 3: TRK is selected
	else if (shape.code && shape.code.length == 7) {
		shape.selected = !shape.selected;
		highlight_trk_area(shape, shape.selected);
		select_bgs(shape, shape.selected);
	}
	// case 4: BG is selected
	else if (shape.code && shape.code.length == 9) {
		shape.selected = !shape.selected;
		highlight_bg_area(shape, shape.selected);
	}
}

function highlight_3zip_area(shape, highlight) {
	if (highlight) {
		shape.SetLineColor(new VEColor(0,0,200,0.8));
		shape.SetLineWidth(4);
	} else {
		shape.SetLineColor(new VEColor(0,0,200,0.5));
		shape.SetLineWidth(2);
	}
}

function highlight_5zip_area(shape, highlight) {
	if (highlight) {
		shape.SetLineColor(new VEColor(255,0,0,0.8));
		shape.SetLineWidth(4);
	} else {
		shape.SetLineColor(new VEColor(255,0,0,0.5));
		shape.SetLineWidth(2);
	}
}

function highlight_trk_area(shape, highlight) {
	if (highlight) {
		shape.SetLineColor(new VEColor(0,200,255,0.8));
		shape.SetLineWidth(4);
	} else {
		shape.SetLineColor(new VEColor(0,200,255,0.5));
		shape.SetLineWidth(2);
	}
}

function highlight_bg_area(shape, highlight) {
	if (highlight) {
		shape.SetLineColor(new VEColor(200,0,200,0.8));
		shape.SetLineWidth(4);
	} else {
		shape.SetLineColor(new VEColor(200,0,200,0.5));
		shape.SetLineWidth(2);
	}
}

var data = function() {
	return {
		ThreeZIPs: [{ 
			points: [ 
				new VELatLong(37.5, -119.1), 
				new VELatLong(37.5, -121.1), 
				new VELatLong(36.3, -121.1),
				new VELatLong(36.3, -119.1) ],
			code: '999'	
		}, { 
			points: [ 
				new VELatLong(37.6, -117.1), 
				new VELatLong(37.6, -119.1), 
				new VELatLong(36.2, -119.1),
				new VELatLong(36.2, -117.1) ],
			code: '222'	
		}],
		FiveZIPs: [{ 
			points: [ 
				new VELatLong(37.5, -120.1), 
				new VELatLong(37.5, -121.1), 
				new VELatLong(37.0, -121.1),
				new VELatLong(37.0, -120.1) ],
			code: '99901'
		}, { 
			points: [ 
				new VELatLong(37.0, -120.1), 
				new VELatLong(37.0, -121.1), 
				new VELatLong(36.3, -121.1),
				new VELatLong(36.3, -120.1) ],
			code: '99902'
		}, { 
			points: [ 
				new VELatLong(37.5, -119.1), 
				new VELatLong(37.5, -120.1), 
				new VELatLong(37.0, -120.1),
				new VELatLong(37.0, -119.1) ],
			code: '99903'
		}, { 
			points: [ 
				new VELatLong(37.0, -119.1), 
				new VELatLong(37.0, -120.1), 
				new VELatLong(36.3, -120.1),
				new VELatLong(36.3, -119.1) ],
			code: '99904'
		}, { 
			points: [ 
				new VELatLong(37.6, -117.1), 
				new VELatLong(37.6, -119.1), 
				new VELatLong(36.9, -119.1),
				new VELatLong(37.0, -117.1) ],
			code: '22201'
		}, { 
			points: [ 
				new VELatLong(37.0, -117.1),
				new VELatLong(36.9, -119.1),
				new VELatLong(36.2, -119.1), 
				new VELatLong(36.2, -117.1) ],
			code: '22202'
		}],
		
		TRKs: [{ 
			points: [ 
				new VELatLong(37.4, -120.12), 
				new VELatLong(37.4, -120.3), 
				new VELatLong(37.3, -120.3),
				new VELatLong(37.3, -120.12) ],
			code: '9990101'
		}, { 
			points: [ 
				new VELatLong(37.25, -120.12), 
				new VELatLong(37.25, -120.35), 
				new VELatLong(37.15, -120.35),
				new VELatLong(37.15, -120.12) ],
			code: '9990102'
		}, { 
			points: [ 
				new VELatLong(37.51, -120.35), 
				new VELatLong(37.51, -120.4), 
				new VELatLong(37.3, -120.4),
				new VELatLong(37.3, -120.35) ],
			code: '9990103'
		}, { 
			points: [ 
				new VELatLong(37.45, -119.9), 
				new VELatLong(37.45, -120.06), 
				new VELatLong(37.35, -120.06),
				new VELatLong(37.35, -119.9) ],
			code: '9990301'
		}, { 
			points: [ 
				new VELatLong(37.25, -119.9), 
				new VELatLong(37.25, -120.06), 
				new VELatLong(37.15, -120.06),
				new VELatLong(37.15, -119.9) ],
			code: '9990302'
		}, { 
			points: [ 
				new VELatLong(37.5, -118.9), 
				new VELatLong(37.5, -119.06), 
				new VELatLong(37.3, -119.06),
				new VELatLong(37.3, -118.9) ],
			code: '2220101'
		}, { 
			points: [ 
				new VELatLong(37.25, -118.5), 
				new VELatLong(37.25, -119.06), 
				new VELatLong(37.15, -119.06),
				new VELatLong(37.15, -118.5) ],
			code: '2220102'
		}],
		BGs: [{ 
			points: [ 
				new VELatLong(37.4, -120.13), 
				new VELatLong(37.4, -120.3), 
				new VELatLong(37.36, -120.3),
				new VELatLong(37.36, -120.12) ],
			code: '999010101'
		}, { 
			points: [ 
				new VELatLong(37.35, -120.12), 
				new VELatLong(37.35, -120.3), 
				new VELatLong(37.3, -120.3),
				new VELatLong(37.3, -120.12) ],
			code: '999010102'
		}, { 
			points: [ 
				new VELatLong(37.25, -120.12), 
				new VELatLong(37.25, -120.35), 
				new VELatLong(37.20, -120.35),
				new VELatLong(37.20, -120.12) ],
			code: '999010201'
		}, { 
			points: [ 
				new VELatLong(37.20, -120.12), 
				new VELatLong(37.20, -120.35), 
				new VELatLong(37.15, -120.35),
				new VELatLong(37.15, -120.12) ],
			code: '999010202'
		}, { 
			points: [ 
				new VELatLong(37.51, -120.35), 
				new VELatLong(37.51, -120.4), 
				new VELatLong(37.3, -120.4),
				new VELatLong(37.3, -120.35) ],
			code: '999010301'
		}, { 
			points: [ 
				new VELatLong(37.45, -119.9), 
				new VELatLong(37.45, -120.0), 
				new VELatLong(37.35, -119.98),
				new VELatLong(37.35, -119.9) ],
			code: '999030101'
		}, { 
			points: [ 
				new VELatLong(37.45, -120.0), 
				new VELatLong(37.45, -120.06), 
				new VELatLong(37.35, -120.06),
				new VELatLong(37.35, -119.98) ],
			code: '999030102'
		}, { 
			points: [ 
				new VELatLong(37.25, -119.9), 
				new VELatLong(37.25, -120.06), 
				new VELatLong(37.15, -120.06),
				new VELatLong(37.15, -119.9) ],
			code: '999030201'
		}, { 
			points: [ 
				new VELatLong(37.5, -118.9), 
				new VELatLong(37.5, -119.06), 
				new VELatLong(37.3, -119.06),
				new VELatLong(37.3, -118.9) ],
			code: '222010101'
		}, { 
			points: [ 
				new VELatLong(37.25, -118.8), 
				new VELatLong(37.25, -119.06), 
				new VELatLong(37.15, -119.06),
				new VELatLong(37.15, -118.6) ],
			code: '222010201'
		}, { 
			points: [ 
				new VELatLong(37.25, -118.5), 
				new VELatLong(37.25, -118.8), 
				new VELatLong(37.15, -118.6),
				new VELatLong(37.15, -118.5) ],
			code: '222010202'
		}]
	}
}();

var threeZipLayer = new VEShapeLayer();
var threeZipLayerLoaded = false;
var iconFormat = '<span style="font-size:20px;color:red;">&nbsp;</span>';

function load_3zips(checked) {
	do_load_3zips();

	if (checked) {
		threeZipLayer.Show();
	} else {
		threeZipLayer.Hide();
	}
}

function do_load_3zips() {
	if (!threeZipLayerLoaded) {        		
		for (var i = 0; i < data.ThreeZIPs.length; i++) {
			var newShape = new VEShape(VEShapeType.Polygon, data.ThreeZIPs[i].points);
			newShape.SetLineColor(new VEColor(0,0,200,0.5));
			newShape.SetFillColor(new VEColor(0,255,0,0.3));
			newShape.SetLineWidth(2);
			newShape.SetCustomIcon(iconFormat.replace('{0}', data.ThreeZIPs[i].code));
			newShape.code = data.ThreeZIPs[i].code;
			newShape.selected = false;
			threeZipLayer.AddShape(newShape);
		}
		
		threeZipLayer.Hide();
		map.AddShapeLayer(threeZipLayer);
		
		threeZipLayerLoaded = true;
	}
}

var fiveZipLayer = new VEShapeLayer();
var fiveZipLayerLoaded = false;

function load_5zips(checked) {
	do_load_5zips();

	if (checked) {
		fiveZipLayer.Show();
	} else {
		fiveZipLayer.Hide();
	}
}

function do_load_5zips() {
	if (!fiveZipLayerLoaded) {        		
		for (var i = 0; i < data.FiveZIPs.length; i++) {
			var newShape = new VEShape(VEShapeType.Polygon, data.FiveZIPs[i].points);
			newShape.SetLineColor(new VEColor(255,0,0,0.5));
			newShape.SetFillColor(new VEColor(255,255,0,0.3));
			newShape.SetLineWidth(2);
			newShape.SetCustomIcon(iconFormat.replace('{0}', data.FiveZIPs[i].code));
			newShape.code = data.FiveZIPs[i].code;
			newShape.selected = false;
			fiveZipLayer.AddShape(newShape);
		}

		fiveZipLayer.Hide();
		map.AddShapeLayer(fiveZipLayer);
		
		fiveZipLayerLoaded = true;
	}
}

var trkLayer = new VEShapeLayer();
var trkLayerLoaded = false;

function load_trks(checked) {
	do_load_trks();

	if (checked) {
		trkLayer.Show();
	} else {
		trkLayer.Hide();
	}
}

function do_load_trks() {
	if (!trkLayerLoaded) {        		
		for (var i = 0; i < data.TRKs.length; i++) {
			var newShape = new VEShape(VEShapeType.Polygon, data.TRKs[i].points);
			newShape.SetLineColor(new VEColor(0,200,255,0.5));
			newShape.SetFillColor(new VEColor(0,100,100,0.3));
			newShape.SetLineWidth(2);
			newShape.SetCustomIcon(iconFormat.replace('{0}', data.TRKs[i].code));
			newShape.code = data.TRKs[i].code;
			newShape.selected = false;
			trkLayer.AddShape(newShape);
		}

		trkLayer.Hide();
		map.AddShapeLayer(trkLayer);
		
		trkLayerLoaded = true;
	}
}

var bgLayer = new VEShapeLayer();
var bgLayerLoaded = false;

function load_bgs(checked) {
	do_load_bgs();

	if (checked) {
		bgLayer.Show();
	} else {
		bgLayer.Hide();
	}
}

function do_load_bgs() {
	if (!bgLayerLoaded) {        		
		for (var i = 0; i < data.BGs.length; i++) {
			var newShape = new VEShape(VEShapeType.Polygon, data.BGs[i].points);
			newShape.SetLineColor(new VEColor(200,0,200,0.5));
			newShape.SetFillColor(new VEColor(100,100, 0,0.3));
			newShape.SetLineWidth(2);
			newShape.SetCustomIcon(iconFormat.replace('{0}', data.BGs[i].code));
			newShape.code = data.BGs[i].code;
			newShape.selected = false;
			bgLayer.AddShape(newShape);
		}

		bgLayer.Hide();
		map.AddShapeLayer(bgLayer);
		
		bgLayerLoaded = true;
	}
}

function select_5zip_areas(parentShape, selected) {
	for (var i = 0; i < fiveZipLayer.GetShapeCount(); i++) {
		var shape = fiveZipLayer.GetShapeByIndex(i);
		if (shape.code.indexOf(parentShape.code) == 0) {
			shape.selected = selected;
			highlight_5zip_area(shape, selected);
			select_trks(shape, selected);
		}
	}
}

function select_trks(zipShape, selected) {
	for (var i = 0; i < trkLayer.GetShapeCount(); i++) {
		var shape = trkLayer.GetShapeByIndex(i);
		if (shape.code.indexOf(zipShape.code) == 0) {
			shape.selected = selected;
			highlight_trk_area(shape, selected);
			select_bgs(shape, selected);
		}
	}
}

function select_bgs(trkShape, selected) {
	for (var i = 0; i < bgLayer.GetShapeCount(); i++) {
		var shape = bgLayer.GetShapeByIndex(i);
		if (shape.code.indexOf(trkShape.code) == 0) {
			shape.selected = selected;
			highlight_bg_area(shape, selected);
		}
	}
}
