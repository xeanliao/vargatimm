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
            console.log('ajax call success!');
            if (result && result.polygon) {
                var polygon = new google.maps.Polygon({
                    paths: result.polygon,
                    strokeColor: '#FF0000',
                    strokeOpacity: 0.8,
                    strokeWeight: 2,
                    fillColor: '#FF0000',
                    fillOpacity: 0.35
                });

                var bounds = new google.maps.LatLngBounds();
                $.each(result.bounds, function() {
                    bounds.extend(new google.maps.LatLng(this.lat, this.lng));
                });

                google.maps.event.addListenerOnce(map, 'tilesloaded', function() {
                    console.log("tilesloaded");
                    callback({
                        success: true
                    });
                });

                polygon.setMap(map);
                map.fitBounds(bounds);

                setTimeout(function() {
                    callback('render time out');
                }, 5 * 60 * 1000);
            }
        },
        error: function(error) {
            console.log(JSON.stringify(error));
            callback('ajax call failed');
        }
    });
}