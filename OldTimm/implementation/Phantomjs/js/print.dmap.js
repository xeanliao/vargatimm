function initMap() {
    var mapDom = $('#map');

    window.map = new google.maps.Map(mapDom[0], {
        center: new google.maps.LatLng(40.744556, -73.987378),
        zoom: 18,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        disableDefaultUI: true
    });

    console.log('map is ready');
    window.mapIsReady = true;
}

function callback(msg) {
    console.log('callback', msg);
    typeof window.callPhantom === 'function' && window.callPhantom(msg);
}

var tryReadyCount = 0;

function begin(params) {
    tryReadyCount++;
    if (window.mapIsReady == true) {
        loadMap(params);
    } else if (tryReadyCount > 5 * 60) {
        callback('wait map ready timeout');
    } else {
        console.log('waiting google map ready');
        window.setTimeout(function() {
            begin(params);
        }, 1000);
    }
}

function loadMap(params) {
    console.log(params);
    var query = [];
    var locationUrl = params.baseUrl + 'print/campaign/' + params.campaignId + '/submap/' + params.submapId + '/dmap/' + params.dmapId + '/location';
    console.log(locationUrl);
    query.push($.getJSON(locationUrl));
    
    var gtuUrl = params.baseUrl + 'print/campaign/' + params.campaignId + '/submap/' + params.submapId + '/dmap/' + params.dmapId + '/gtu';
    console.log(gtuUrl);
    query.push($.getJSON(gtuUrl));
    
    $.when.apply(this, query).done(function(location, gtu){
        // var args = Array.prototype.slice.call(arguments);
        // console.log("args", args);
        var result = $.extend({}, location[0], gtu[0]);
        console.log(result);
        draw(params, result);
    }).fail(function(){
        callback('ajax call failed.');
    });
}

function getCirclePath(size){
    return sprintf('M-%d,0a%d,%d 0 1,0 %d,0a%d,%d 0 1,0 -%d,0', size, size, size, size * 2, size, size, size*2);
}
function draw(params, result) {
    if (result && result.boundary) {
        console.log('ajax call success!');
        var fillColor = 'rgb(' + result.color.r + ',' + result.color.g + ',' + result.color.b + ')';
        /**
         * draw polyline for submap boundary
         */
        var boundaryPolyline = new google.maps.Polygon({
            paths: result.boundary,
            strokeColor: fillColor,
            strokeOpacity: 1,
            strokeWeight: 6,
            fillColor: fillColor,
            fillOpacity: 0.1,
        });
        boundaryPolyline.setMap(map);

        /**
         * draw gtu point
         */
        var index = 0;
        $.each(result.points, function(){
            var color = result.pointsColors[index];
            $.each(this, function(){
                var marker = new google.maps.Marker({
                    position: this,
                    icon: {
                        path: getCirclePath(params.gTUDotsRadii),
                        fillColor: color,
                        fillOpacity: 0.6,
                        strokeOpacity: 0,
                        strokeWeight: 1
                    },
                    draggable:true,
                    map: map
                });
            });
            console.log(index, color);
            index++;
        });

        /**
         * find best view
         * @type {google}
         */
        console.log("begin find best view");

        google.maps.event.addListenerOnce(map, 'tilesloaded', function() {
            console.log("tilesloaded");
            callback({
                success: true
            });
        });
        
        // if(params.boundSWLat && params.boundSWLng && params.boundNELat && params.boundNELng){
        //     var sw = new google.maps.LatLng(params.boundSWLat, params.boundSWLng);
        //     var ne = new google.maps.LatLng(params.boundNELat, params.boundNELng);
        //     var mapBounds = new google.maps.LatLngBounds(sw, ne);
        //     map.fitBounds(mapBounds);
        // }else{
            var mapBounds = new google.maps.LatLngBounds();
            $.each(result.boundary, function() {
                var point = new google.maps.LatLng(this.lat, this.lng);
                mapBounds.extend(point);
            });
            map.fitBounds(mapBounds);
        //}
        
        console.log("draw map finished");
        /**
         * timeout for google map render all polygon
         */
        setTimeout(function() {
            callback('render time out');
        }, 10 * 60 * 1000);
    }
}