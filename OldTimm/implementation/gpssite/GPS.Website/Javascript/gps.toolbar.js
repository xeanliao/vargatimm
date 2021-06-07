/**
 * This file contains the logic used to manipulate toolbars.
 */
$(function() {
    $('.gps-toolbar-button').hover(
        function() { $(this).addClass('ui-state-hover'); },
        function() { $(this).removeClass('ui-state-hover'); }
    );
});
