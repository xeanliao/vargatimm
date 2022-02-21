import L from 'leaflet'

L.AnimateMarker = L.Path.extend({
    onAdd: function () {
        this._path
    },

    onRemove: function () {
        this._path
    },
})
L.animateMarker = function (centerLatLng, options) {
    return new L.AnimateMarker(centerLatLng, options)
}
