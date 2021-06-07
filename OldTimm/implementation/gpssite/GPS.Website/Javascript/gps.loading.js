/**
* GPS.Loading shows a 'Loading...' message on the page.
*/

GPS.Loading = function() {
    return {
        show: function(msg) {
            var id = Math.random().toString().replace('.', '');
            var loading = msg ? msg : "Loading...";
            $('body').append('<div id="{0}" class="gps-loading-mask"><span class="gps-loading-text ui-widget-content ui-corner-all">{1}</span></div>'.replace('{0}', id).replace('{1}', loading));
            return id;
        },
        hide: function() {
            $('.gps-loading-mask').hide();
        }
    }
} ();
