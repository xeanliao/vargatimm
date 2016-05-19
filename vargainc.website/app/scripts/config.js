require.config({
    baseUrl: 'scripts',
    // deps: ['main'],
    // bundles: {
    //     'foundation': ['foundation.core', 'foundation.util.box','foundation.util.keyboard','foundation.util.triggers','foundation.util.mediaQuery','foundation.util.motion','foundation.reveal']
    // },
    shim: {
        'backbone': {
            deps: ['underscore', 'jquery'],
            exports: 'Backbone'
        },
        'react': {
            exports: 'React'
        },
        'react-dom': {
            deps: ['react'],
            exports: 'ReactDOM'
        },
        'react-dom-server': {
            deps: ['react'],
            exports: 'ReactDOM'
        },
        'react.backbone': {
            deps: ['backbone', 'react']
        },
        'markerclusterer': {
            exports: 'MarkerClusterer'
        },
        'foundation': {
            deps: ['jquery'],
            exports: 'Foundation'
        },
        'foundation.core': {
            deps: ['jquery'],
            exports: 'Foundation'
        },
        'foundation.util.box': {
            deps: ['foundation.core']
        },
        'foundation.util.keyboard': {
            deps: ['foundation.core']
        },
        'foundation.util.triggers': {
            deps: ['foundation.core']
        },
        'foundation.util.mediaQuery': {
            deps: ['foundation.core']
        },
        'foundation.util.motion': {
            deps: ['foundation.core']
        },
        'foundation.reveal': {
            deps: ['foundation.core', 'foundation.util.box', 'foundation.util.keyboard', 'foundation.util.triggers', 'foundation.util.mediaQuery', 'foundation.util.motion']
        },
        'foundation.dropdown': {
            deps: ['foundation.core', 'foundation.util.box', 'foundation.util.keyboard']
        },
        'foundation.abide': {
            deps: ['foundation.core']
        },
        'foundation.tooltip': {
            deps: ['foundation.core', 'foundation.util.box', 'foundation.util.triggers']
        },
        'foundation-datepicker': {
            deps: ['jquery']
        }
    },
    map: {
        "*": {
            'foundation': 'foundation.hack'
        },
        'foundation.hack': {
            'foundation': 'foundation.core'
        }
    },
    paths: {
        'async': 'vendor/async',
        'jquery': 'vendor/jquery',
        'jquery-ui': 'vendor/jquery-ui',
        'underscore': 'vendor/lodash',
        'backbone': 'vendor/backbone',
        'backbone.route.control': 'vendor/backbone-route-control',
        'react': 'vendor/react',
        'react-dom': 'vendor/react-dom',
        'react-dom-server': 'vendor/react-dom-server',
        'react.backbone': 'vendor/react.backbone',
        'moment': 'vendor/moment',
        'pubsub': 'vendor/pubsub',
        'numeral': 'vendor/numeral',
        'sprintf': 'vendor/sprintf',
        'select2': 'vendor/select2',
        'spectrum': 'vendor/spectrum',
        'markerclusterer': 'vendor/markerclusterer',
        'd3': '//d3js.org/d3.v3.min',
        'fastMarker': 'vendor/fastMarker',
        'foundation.core': 'vendor/foundation.core',
        'foundation.util.box': 'vendor/foundation.util.box',
        'foundation.util.keyboard': 'vendor/foundation.util.keyboard',
        'foundation.util.triggers': 'vendor/foundation.util.triggers',
        'foundation.util.mediaQuery': 'vendor/foundation.util.mediaQuery',
        'foundation.util.motion': 'vendor/foundation.util.motion',
        'foundation.reveal': 'vendor/foundation.reveal',
        'foundation.dropdown': 'vendor/foundation.dropdown',
        'foundation.abide': 'vendor/foundation.abide',
        'foundation.tooltip': 'vendor/foundation.tooltip',
        'foundation-datepicker': 'vendor/foundation-datepicker',
        'foundation': 'vendor/foundation',
        'foundation.hack': 'foundation.hack'
    }
});


// require(['foundation'], function(){
//     console.log($.fn.foundation);
//     $(document).foundation();
// });