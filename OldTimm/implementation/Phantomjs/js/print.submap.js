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
    var url = params.baseUrl + 'print/campaign/' + params.campaignId + '/submap/' + params.submapId + '/location';
    console.log(url);
    $.ajax({
        url: url,
        method: 'GET',
        success: function(result) {
            draw(params, result);
        },
        error: function(error) {
            console.log(JSON.stringify(error));
            callback('ajax call failed');
        }
    });
}

function draw(params, result) {
    if (result && result.boundary) {
        console.log('ajax call success!');
        var fillColor = 'rgb(' + result.color.r + ',' + result.color.g + ',' + result.color.b + ')';

        var index = 1;
        $.each(result.polygon, function() {
            /**
             * use google bounds to find the center
             * which will used for draw the area serial number on it
             */
            var bounds = new google.maps.LatLngBounds();
            $.each(this, function() {
                var point = new google.maps.LatLng(this.lat, this.lng);
                bounds.extend(point);
            });
            var polygon = new google.maps.Polygon({
                paths: this,
                strokeColor: fillColor,
                strokeOpacity: 1,
                strokeWeight: 1,
                fillColor: fillColor,
                fillOpacity: 0.1,
            });
            polygon.setMap(map);

            // var marker = new google.maps.Marker({
            //     position: bounds.getCenter(),
            //     icon: label,
            //     map: map
            // });

            index++;
        });

        /**
         * draw polyline for submap boundary
         */
        var boundaryPolyline = new google.maps.Polygon({
            paths: result.boundary,
            strokeColor: fillColor,
            strokeOpacity: 1,
            strokeWeight: 6,
            fillOpacity: 0,
        });
        boundaryPolyline.setMap(map);

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