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
        window.setTimeout(function () {
            begin(params);
        }, 1000);
    }
}

function loadMap(params) {
    console.log('loadMap', params);

    var query = [];
    var locationUrl = params.baseUrl + 'print/campaign/' + params.campaignId + '/submap/' + params.submapId + '/dmap/' + params.dmapId + '/location';
    console.log(locationUrl);
    query.push($.getJSON(locationUrl));

    var addressUrl = params.baseUrl + 'print/campaign/' + params.campaignId + '/address';
    console.log(addressUrl);
    query.push($.getJSON(addressUrl));

    if (!params.suppressGTU) {
        var gtuUrl = params.baseUrl + 'print/campaign/' + params.campaignId + '/submap/' + params.submapId + '/dmap/' + params.dmapId + '/gtu';
        console.log(gtuUrl);
        query.push($.getJSON(gtuUrl));
    }
    if (!params.suppressNDAInDMap) {
        var ndAddressUrl = params.baseUrl + 'print/ndaddress';
        console.log(ndAddressUrl);
        query.push($.getJSON(ndAddressUrl));
    }

    $.when.apply(this, query).done(function () {
        var args = Array.prototype.slice.call(arguments);
        var result = {};
        $.each(args, function (i, v) {
            if (v && v[0]) {
                $.extend(result, v[0]);
            }
        });
        console.log(result);
        drawMap(params, result);
    }).fail(function () {
        callback('ajax call failed.');
    });
}

function drawMap(params, result) {
    console.log('begin find map best view');
    if (result && result.boundary) {
        var mapBounds = new google.maps.LatLngBounds();
        $.each(result.boundary, function () {
            var point = new google.maps.LatLng(this.lat, this.lng);
            mapBounds.extend(point);
        });

        google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
            console.log("end find best view");
            checkMapType(params, result);
        });

        map.fitBounds(mapBounds);

        /**
         * timeout for google map render all polygon
         */
        window.timeout = setTimeout(function () {
            callback('wait map tiles time out');
        }, 5 * 60 * 1000);
    } else {
        callback('data error');
    }
}

function checkMapType(params, result) {
    console.log('check map type');
    if (params.mapType != 'ROADMAP') {
        var mapType = google.maps.MapTypeId.ROADMAP;
        switch (params.mapType) {
        case 'ROADMAP':
            mapType = google.maps.MapTypeId.ROADMAP;
            break;
        case 'SATELLITE':
            mapType = google.maps.MapTypeId.SATELLITE;
            break;
        case 'HYBRID':
            mapType = google.maps.MapTypeId.HYBRID;
            break;
        case 'TERRAIN':
            mapType = google.maps.MapTypeId.TERRAIN;
            break;
        default:
            mapType = google.maps.MapTypeId.ROADMAP;
            break;
        }
        /**
         * change map type and montor tilesloaded event to make sure all map have loaded
         */
        google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
            console.log("tilesloaded");
            clearTimeout(window.timeout);
            setTimeout(function () {
                callback({
                    success: true,
                    status: 'tilesloaded'
                });
                draw(params, result);
            }, 2000);
        });
        map.setMapTypeId(mapType);
    } else {
        setTimeout(function () {
            callback({
                success: true,
                status: 'tilesloaded'
            });
            draw(params, result);
        }, 2000);
    }
}

function getCirclePath(size) {
    return sprintf('M-%d,0a%d,%d 0 1,0 %d,0a%d,%d 0 1,0 -%d,0', size, size, size, size * 2, size, size, size * 2);
}

function draw(params, result) {
    console.log('draw map polygon');
    /**
     * draw ndaddress
     */
    var flagx24 = 'M22,17.1c-2.4-0.6-3.9-5.8-3.9-5.8c2.3-2.4,1.5-5.7,1.5-5.7c-6.2,1.9-8.2-1.1-8.2-1.1c0,0,0,0,0,0.1C10.1,3.1,6.8,3.7,5,4.1l0-0.2c0.6-0.3,0.9-1,0.8-1.7C5.5,1.5,4.7,1,3.9,1.1c-0.8,0.2-1.3,1-1.2,1.8    C2.9,3.6,3.4,4,4,4.1l0.1,0.3C3.9,4.4,3.8,4.4,3.8,4.4C4,5,4.3,5.6,4.5,6.2l2,9.5c0,0.1,0,0.3,0,0.3c0,0,0.1,0,0.1,0l1.6,7.5c0.1,0.2,0.4,0.5,0.6,0.5c0.2-0.1,0.3-0.6,0.2-0.8l-1.6-7.4c2.7-0.7,5-0.3,5-0.3';
    var flagx32 = 'M29.3,22.7c-3.3-0.8-5.2-7.9-5.2-7.9c3.1-3.3,2-7.8,2-7.8C17.7,9.6,15,5.5,15,5.5c0,0,0,0.1,0,0.1c-1.7-1.8-6.2-1-8.6-0.4l0-0.2c0.8-0.4,1.3-1.3,1.1-2.2C7.2,1.6,6.1,0.9,5,1.2C3.9,1.4,3.2,2.5,3.4,3.6C3.6,4.4,4.3,5,5.1,5.2l0.1,0.3C4.9,5.6,4.8,5.6,4.8,5.6c0.3,0.8,0.6,1.6,0.9,2.3l2.8,12.8c0,0.2,0,0.3,0,0.4c0,0,0.1,0,0.1,0l2.2,10.1c0.1,0.3,0.5,0.7,0.9,0.7c0.3-0.1,0.4-0.8,0.3-1.1l-2.1-10c3.6-0.9,6.8-0.4,6.8-0.4';
    var flagx64 = 'M55.4,43.5c-5.8-1.5-9.3-14-9.3-14c5.5-5.8,3.5-13.8,3.5-13.8c-14.9,4.6-19.7-2.8-19.7-2.8c0,0.1,0,0.1,0.1,0.2c-3-3.2-11-1.8-15.3-0.7L14.6,12c1.4-0.7,2.2-2.3,1.9-4c-0.4-2-2.3-3.2-4.3-2.8c-2,0.4-3.2,2.3-2.8,4.3c0.3,1.5,1.6,2.6,3,2.8l0.1,0.6c-0.4,0.1-0.7,0.2-0.7,0.2c0.6,1.4,1.1,2.8,1.6,4.1L18.3,40c0,0.3,0,0.6,0,0.8c0.1,0,0.1,0,0.2-0.1l3.8,17.9c0.1,0.6,0.9,1.3,1.5,1.2c0.6-0.1,0.7-1.4,0.6-2l-3.8-17.8c6.4-1.7,12-0.7,12-0.7';
    if (result && result.nda) {
        console.log('draw dna');
        $.each(result.nda, function () {
            var marker = new google.maps.Marker({
                position: this,
                icon: {
                    path: flagx24,
                    fillColor: '#C22034',
                    fillOpacity: 0.8,
                    strokeOpacity: 0.1,
                    strokeWeight: 1,
                    strokeColor: '#ED0E69'
                },
                draggable: false,
                map: map
            });
        });
    }

    if (result && result.boundary) {
        var fillColor = 'rgb(' + result.color.r + ',' + result.color.g + ',' + result.color.b + ')';
        /**
         * draw polyline for boundary
         */
        new google.maps.Polygon({
            paths: result.boundary,
            strokeColor: '#000',
            strokeOpacity: 1,
            strokeWeight: 6,
            fillColor: fillColor,
            fillOpacity: 0.2,
            map: map
        });

        /**
         * draw gtu point
         */
        var index = 0;
        params.gTUDotsRadii = params.gTUDotsRadii ? params.gTUDotsRadii : 8;
        if (result.points && result.points.length > 0) {
            $.each(result.points, function () {
                var color = result.pointsColors[index];
                $.each(this, function () {
                    new google.maps.Marker({
                        position: this,
                        icon: {
                            path: getCirclePath(params.gTUDotsRadii),
                            fillColor: color,
                            fillOpacity: 1,
                            strokeOpacity: 1,
                            strokeWeight: 1,
                            strokeColor: '#000'
                        },
                        draggable: true,
                        map: map
                    });
                });
                index++;
            });
        }

        console.log("draw map done");
    }

    /**
     * draw campaign address
     */
    if (result && result.address) {
        console.log('begin draw campaign address');
        var starx24 = 'M10.8,2.7c0,0-2,4.1-2.5,5.2C7.1,8.1,2.7,8.7,2.7,8.7c-0.5,0.1-1,0.5-1.2,1c0,0.1-0.1,0.3-0.1,0.5c0,0.4,0.2,0.8,0.4,1.1c0,0,3.2,3.2,4,4.1c-0.2,1.2-1,5.7-1,5.7c0,0.1,0,0.2,0,0.3c0,0.5,0.2,0.9,0.6,1.2c0.4,0.3,1,0.4,1.5,0.1c0,0,4-2.1,5-2.7c1,0.6,5,2.7,5,2.7c0.5,0.3,1.1,0.2,1.5-0.1c0.4-0.3,0.6-0.7,0.6-1.2c0-0.1,0-0.2,0-0.2c0,0-0.7-4.5-0.9-5.7c0.8-0.8,4.1-4,4.1-4c0.3-0.3,0.4-0.7,0.4-1.1c0-0.2,0-0.3-0.1-0.5c-0.2-0.5-0.6-0.9-1.2-1c0,0-4.4-0.7-5.6-0.8c-0.5-1.1-2.5-5.2-2.5-5.2c-0.2-0.5-0.7-0.8-1.3-0.8C11.5,1.9,11,2.2,10.8,2.7z';
        $.each(result.address, function () {
            if (this.circle && this.circle.length > 0) {
                var center = this.center;
                if (!params.suppressLocations) {
                    new google.maps.Marker({
                        position: center,
                        icon: {
                            path: starx24,
                            fillColor: this.color,
                            fillOpacity: 0.8,
                            strokeOpacity: 0.1,
                            strokeWeight: 1,
                            strokeColor: '#ED0E69'
                        },
                        map: map
                    });
                }
                if (!params.suppressRadii) {
                    $.each(this.circle, function () {
                        console.log('address radius:', this.radius);
                        var circle = new google.maps.Circle({
                            strokeColor: '#00008B',
                            strokeOpacity: 0.8,
                            strokeWeight: 1,
                            fillOpacity: 0,
                            center: center,
                            radius: parseFloat(this.radius),
                            map: map
                        });
                        var circleBounds = circle.getBounds();
                        console.log(circleBounds);
                        new MapLabel({
                            position: new google.maps.LatLng(circleBounds.getNorthEast().lat(), center.lng),
                            map: map,
                            vAlign: 'bottom',
                            text: this.label
                        });
                        new MapLabel({
                            position: new google.maps.LatLng(center.lat, circleBounds.getNorthEast().lng()),
                            map: map,
                            align: 'left',
                            text: this.label
                        });
                        new MapLabel({
                            position: new google.maps.LatLng(circleBounds.getSouthWest().lat(), center.lng),
                            map: map,
                            vAlign: 'top',
                            text: this.label
                        });
                        new MapLabel({
                            position: new google.maps.LatLng(center.lat, circleBounds.getSouthWest().lng()),
                            map: map,
                            align: 'right',
                            text: this.label
                        });
                    });
                }
            }
        });
        console.log('draw campaign address done!');
    }
    console.log('wait for all map done.');

    setTimeout(prepareScreenshot, 1000);
}

var finallyCutdown = 5;
function prepareScreenshot() {
    console.log('cutdown', finallyCutdown);
    if(--finallyCutdown <= 0){
        console.log('mission completed');
        $("#map").css("background", "transparent");
        $("#map img").hide();

        callback({
            success: true,
            status: 'finished'
        });
    }else{
        setTimeout(prepareScreenshot, 1000);
    }
}