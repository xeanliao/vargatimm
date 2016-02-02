define([
	'jquery', 
	'foundation.core',
	'foundation.util.box',
    'foundation.util.keyboard',
    'foundation.util.triggers',
    'foundation.util.mediaQuery',
    'foundation.util.motion',
    'foundation.reveal',
    'foundation.dropdown',
    'foundation.abide',
    'foundation.tooltip',
    'foundation-datepicker'
], function($){
	console.log($.fn.foundation, window.Foundation);
	return window.Foundation;
})