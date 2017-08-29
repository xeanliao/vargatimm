var spritezero = require('@mapbox/spritezero');
var fs = require('fs');
var path = require('path');
var svgpath = require('svgpath');
var _ = require('lodash');
var svgContainer = '<?xml version="1.0" standalone="no"?><!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1//EN" "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd"><svg width="64px" height="64px" viewBox="0 0 64 64" version="1.1" xmlns="http://www.w3.org/2000/svg">';
var icons = {
    'walkerA': {
        scale: 0.5,
        path: 'M63.896,20.458c2.906,1.355,6.362,0.096,7.718-2.809c1.356-2.908,0.099-6.364-2.811-7.719  c-2.907-1.356-6.362-0.099-7.718,2.81C59.729,15.645,60.986,19.102,63.896,20.458z M74.014,44.699l-11.117-5.184l3.323-7.126  c1.669-3.578,0.121-7.832-3.459-9.502c-3.579-1.667-7.832-0.119-9.5,3.458H53.26l-3.324,7.127v0l-3.022,6.479l0.002,0L38.85,57.248  l0,0L24.342,88.356c-1.043,2.236-0.075,4.895,2.159,5.939c2.239,1.042,4.896,0.073,5.939-2.161l14.164-30.371L60.9,92.418  c1.041,2.236,3.701,3.205,5.938,2.162c2.236-1.044,3.204-3.702,2.162-5.939L54.495,57.535l5.38-11.539l11.117,5.184  c1.788,0.834,3.917,0.064,4.751-1.728C76.576,47.663,75.802,45.536,74.014,44.699z M50.826,26.693c0-2.468-2.002-4.469-4.471-4.469  c-1.791,0-3.327,1.062-4.039,2.586l-0.01-0.004l-7.518,16.113l0.001,0c-1.041,2.236-0.073,4.893,2.16,5.938  c2.24,1.042,4.896,0.074,5.939-2.161l7.518-16.113l-0.011-0.005C50.665,28.003,50.826,27.369,50.826,26.693z',
    },
    'truck': {
        scale: 1,
        path: 'M48.917,22.29 C48.875,22.326 48.834,22.362 48.79,22.395 C48.706,22.457 48.62,22.514 48.53,22.567 C48.459,22.61 48.387,22.649 48.312,22.686 C48.232,22.725 48.155,22.762 48.072,22.794 C47.97,22.833 47.863,22.863 47.756,22.892 C47.69,22.909 47.626,22.931 47.559,22.944 C47.377,22.978 47.191,23 47,23 C46.809,23 46.623,22.978 46.441,22.944 C46.374,22.931 46.31,22.909 46.244,22.892 C46.136,22.863 46.03,22.833 45.928,22.794 C45.845,22.762 45.768,22.725 45.688,22.686 C45.613,22.649 45.541,22.61 45.47,22.567 C45.38,22.514 45.294,22.457 45.21,22.395 C45.166,22.362 45.125,22.326 45.083,22.29 C44.427,21.74 44,20.924 44,20 C44,18.343 45.343,17 47,17 C48.657,17 50,18.343 50,20 C50,20.924 49.573,21.74 48.917,22.29 L48.917,22.29 Z M24,7 C24,7.551 23.552,8 23,8 L12.399,8 C12.507,7.903 12.609,7.805 12.707,7.707 C13.229,7.185 13.721,6.664 14.198,6.157 C16.389,3.834 18.118,2 21,2 L24,2 L24,7 Z M11.917,22.29 C11.875,22.326 11.834,22.362 11.79,22.395 C11.706,22.457 11.62,22.514 11.53,22.567 C11.459,22.61 11.387,22.649 11.312,22.686 C11.232,22.725 11.155,22.762 11.072,22.794 C10.97,22.833 10.863,22.863 10.756,22.892 C10.69,22.909 10.626,22.931 10.559,22.944 C10.377,22.978 10.191,23 10,23 C9.809,23 9.623,22.978 9.441,22.944 C9.374,22.931 9.31,22.909 9.244,22.892 C9.136,22.863 9.03,22.833 8.928,22.794 C8.845,22.762 8.768,22.725 8.688,22.686 C8.613,22.649 8.541,22.61 8.47,22.567 C8.38,22.514 8.294,22.457 8.21,22.395 C8.166,22.362 8.125,22.326 8.083,22.29 C7.427,21.74 7,20.924 7,20 C7,18.343 8.343,17 10,17 C11.657,17 13,18.343 13,20 C13,20.924 12.573,21.74 11.917,22.29 L11.917,22.29 Z M54,0 L25,0 L21,0 C17.255,0 15.063,2.324 12.743,4.785 C12.279,5.278 11.801,5.785 11.293,6.293 C11.133,6.453 10.955,6.609 10.769,6.763 C10.652,6.858 10.521,6.955 10.391,7.051 C10.319,7.104 10.252,7.158 10.178,7.211 C9.978,7.352 9.764,7.495 9.533,7.64 C9.526,7.644 9.521,7.648 9.514,7.652 C9.264,7.808 8.996,7.967 8.709,8.129 C8.706,8.131 8.705,8.133 8.702,8.135 C7.588,8.763 6.355,9.343 5.201,9.884 C2.182,11.298 0,12.32 0,14 L0,19 C0,20.654 1.346,22 3,22 L5.424,22 C6.197,23.763 7.955,25 10,25 C12.045,25 13.803,23.763 14.576,22 L42.424,22 C43.197,23.763 44.955,25 47,25 C49.045,25 50.803,23.763 51.576,22 L56,22 C58.206,22 60,20.206 60,18 L60,14 C60,9.811 60,0 54,0 L54,0 Z',
    },
    'marker': {
        scale: 0.5,
        path: 'm 41,967.37639 c -3.306703,0 -6,2.69275 -6,5.99878 0,3.2545 2.607906,5.91665 5.84375,5.99878 l -4.6875,24.12005 c -2.133137,0.1582 -4.114666,0.7996 -5.5625,2.1559 C 28.947926,1007.1915 28,1009.5067 28,1012.3672 a 1.0001,0.99989656 0 0 0 1,0.9998 l 20,0 0,22.9954 a 1.0001,0.99989656 0 1 0 2,0 l 0,-22.9954 20,0 a 1.0001,0.99989656 0 0 0 1,-0.9998 c 0,-2.8605 -0.947926,-5.1757 -2.59375,-6.7173 -1.447834,-1.3563 -3.429363,-1.9977 -5.5625,-2.1559 l -4.6875,-24.12005 C 62.392094,979.29182 65,976.62967 65,973.37517 c 0,-3.30603 -2.693297,-5.99878 -6,-5.99878 l -18,0 z m 0,1.9996 18,0 c 2.233297,0 4,1.76634 4,3.99918 0,2.23285 -1.766703,3.99919 -4,3.99919 l -18,0 c -2.233297,0 -4,-1.76634 -4,-3.99919 0,-2.23284 1.766703,-3.99918 4,-3.99918 z m 1.84375,9.99796 14.3125,0 4.875,25.18235 A 1.0001,0.99989656 0 0 0 63,1005.3687 c 2.086493,0 3.8192,0.6142 5.03125,1.7496 1.018562,0.9541 1.615526,2.4029 1.8125,4.2491 l -39.6875,0 c 0.196974,-1.8462 0.793938,-3.295 1.8125,-4.2491 1.21205,-1.1354 2.944757,-1.7496 5.03125,-1.7496 a 1.0001,0.99989656 0 0 0 0.96875,-0.8124 l 4.875,-25.18235 z',
    },
    'flag': {
        scale: 0.5,
        path: 'M28,14.603673 L28,14.0022583 C28,12.8958141 27.1045695,12 26,12 C24.8877296,12 24,12.8964416 24,14.0022583 L24,90.9977417 C24,92.1041859 24.8954305,93 26,93 C27.1122704,93 28,92.1035584 28,90.9977417 L28,53.5385791 C31.5079703,52.0264221 43.6821257,47.6689964 55.5,54 C69.5,61.4999994 84,54 84,54 L84,15 C84,15 68,22.0000003 55.5,15 C44.8614011,9.04238434 31.687575,13.225678 28,14.603673 Z',
    },
    'star': {
        scale: 1,
        path: 'M70.6,46.2L61.3,55.6l2,13.101c0.216,0.857-0.983,1.52-1.399,1.1L50,63.9L38.1,69.8c-0.577,0.385-1.609-0.265-1.4-1.1  l2-13.101L29.4,46.2c-0.683-0.684-0.044-1.539,0.6-1.7l13-2.1l6.1-11.7c0.3-0.7,1.4-0.7,1.8,0L57,42.4l13.1,2.2  C71.068,44.842,71.155,45.831,70.6,46.2z',
    },
};
_.each([1, 2], function (pxRatio) {
    var svgs = [];
    _.each(icons, (svg, id) => {
        var fixedSvg = svgpath(svg.path).scale(svg.scale).toString();
        svgs.push({
            id: id,
            svg: Buffer.from(`${svgContainer}<path fill="#000000" d="${fixedSvg}" /></svg>`)
        });
    });

    if (pxRatio == 1) {
        var pngPath = path.resolve(path.join(__dirname, 'sprite.png'));
        var jsonPath = path.resolve(path.join(__dirname, 'sprite.json'));
    } else {
        var pngPath = path.resolve(path.join(__dirname, 'sprite@' + pxRatio + 'x.png'));
        var jsonPath = path.resolve(path.join(__dirname, 'sprite@' + pxRatio + 'x.json'));
    }


    // Pass `true` in the layout parameter to generate a data layout
    // suitable for exporting to a JSON sprite manifest file.
    spritezero.generateLayout(svgs, pxRatio, true, function (err, dataLayout) {
        if (err) return;
        fs.writeFileSync(jsonPath, JSON.stringify(dataLayout));
    });

    // Pass `false` in the layout parameter to generate an image layout
    // suitable for exporting to a PNG sprite image file.
    spritezero.generateLayout(svgs, pxRatio, false, function (err, imageLayout) {
        spritezero.generateImage(imageLayout, function (err, image) {
            if (err) return;
            fs.writeFileSync(pngPath, image);
        });
    });

});