require.config({
    baseUrl: 'scripts',
    deps: ['main'],
    config: {
        baseUrl: 'http://dev.timm.vargainc.com/'
    },
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
        'react.backbone': {
            deps: ['backbone', 'react']
        },
        'react-day-picker': {
            deps: ['react'],
            exports: 'DayPicker'
        },
        'react-select': {
            deps: ['react', 'react-dom', 'classnames'],
            exports: 'ReactSelect'
        },
        'classnames': {
            exports: 'classNames'
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
            'foundation': 'foundation'
        }
    },
    paths: {
        'jquery': 'vendor/jquery',
        'underscore': 'vendor/lodash',
        'backbone': 'vendor/backbone',
        'backbone.route.control': 'vendor/backbone-route-control',
        'react-dom': 'vendor/react-dom',
        'react': 'vendor/react',
        'react.backbone': 'vendor/react.backbone',
        'react-intl': 'vendor/react-intl-with-locales',
        'moment': 'vendor/moment',
        'react-day-picker': 'vendor/DayPicker',
        'react-native-listener': 'vendor/react-native-listener',
        'react-select': 'vendor/react-select',
        'pubsub': 'vendor/pubsub',
        'classnames': 'vendor/dedupe',
        'numeral': 'vendor/numeral',
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
        'foundation.hack': 'vendor/foundation.hack'
    }
});


// require(['foundation'], function(){
//     console.log($.fn.foundation);
//     $(document).foundation();
// });