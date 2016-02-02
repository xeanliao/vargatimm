(function() {
	'use strict';
	var system = require('system'),
		fs = require('fs');
	var config = {
		baseUrl: 'http://dev.timm.vargainc.com/',
		tmpFolder: '/Volumes/RamDisk/'
	};
	config = {
		baseUrl: 'http://98.189.6.210:9000/timm201507/api/',
		tmpFolder: '/Volumes/RamDisk/'
	};
	/**
	 * dev config
	 */
	// config = {
	// 	baseUrl: 'http://98.189.6.210:9000/timm201507/api/',
	// 	tmpFolder: '/var/www/img.timm.vargainc.com/'
	// };
	var guid = function() {
		function s4() {
			return Math.floor((1 + Math.random()) * 0x10000)
				.toString(16)
				.substring(1);
		}
		return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
			s4() + '-' + s4() + s4() + s4();
	};
	var render = function(params, exitCallback) {
		params.baseUrl = config.baseUrl;
		console.log(JSON.stringify(params));
		var page = require('webpage').create();
		page.viewportSize = {
			width: 800,
			height: 800
		};
		page.settings.userAgent = 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.120 Safari/537.36';
		page.settings.javascriptEnabled = true;
		page.settings.loadImages = true;
		page.settings.localToRemoteUrlAccessEnabled = true;
		page.settings.webSecurityEnabled = false;
		page.settings.resourceTimeout = 5 * 60 * 1000;

		page.onConsoleMessage = function(msg) {
			console.log('CONSOLE: ' + JSON.stringify(msg));
		};

		page.onCallback = function(data) {
			if (data && data.success) {
				setTimeout(function() {
					console.log('CALLBACK: ' + JSON.stringify(data));
					
					if(params.output){
						console.log(params.output);
						page.render(params.output);
					}else{
						var fileName = guid() + '.png';
						console.log(fileName);
						page.render(config.tmpFolder + fileName);
					}					
					exit(fileName);
				}, 500);
			} else {
				exit({
					success: false
				});
			}
		};

		var exit = function(result) {
			page.close();
			exitCallback(result);
		};
		var url = '';
		switch (params.type) {
			case 'SubMap':
				page.viewportSize = {
					width: 816 * 2,
					height: 816 * 2
				};
				url = 'print.submap.html';
				break;
			case 'DMap':
				page.viewportSize = {
					width: 1920 * 2,
					height: 1920 * 2
				};
				url = 'print.dmap.html';
				break;
			default:
				
				break;
		}
		if (url === '') {
			exit('bad params');
			return;
		}
		page.open(url, function(status) {
			if (status === 'success') {
				console.log('open map html success');
				console.log(JSON.stringify(params));
				page.evaluateAsync(function(params) {
					begin(JSON.parse(params));
				}, 1000, JSON.stringify(params));
			} else {
				console.log('open page failed. exit');
				exit('open page failed. exit');
			}
		});
	};

	var parseUrl = function(url) {
		var params = url.replace('/', '').replace('?', '').split('&');
		var map = {};
		for (var i = 0; i < params.length; i++) {
			var index = params[i].indexOf('=');
			var key = params[i].substr(0, index);
			var value = params[i].substring(index + 1);
			map[key] = value;
		}
		return map;
	};

	var startServer = function(host, port) {
		var server = require('webserver').create();

		server.listen(host + ':' + port,
			function(request, response) {
				console.log(request.url);
				var params = parseUrl(request.url);
				console.log(params.status);
				try {
					if (typeof params.status !== 'undefined') {
						// for server health validation
						response.statusCode = 200;
						response.write('OK');
						response.close();
					} else {
						render(params, function(result) {
							response.statusCode = 200;
							//response.write(fs.read('tmp/' + result, 'b'));
							if (typeof result === 'string') {
								response.write(JSON.stringify({
									success: true,
									path: result
								}));
							} else {
								response.write(JSON.stringify(result));
							}

							response.close();
						});
					}
				} catch (e) {
					var msg = 'Failed rendering: \n' + e;
					response.statusCode = 500;
					response.setHeader('Content-Type', 'text/plain');
					response.setHeader('Content-Length', msg.length);
					response.write(msg);
					response.close();
				}
			});

		console.log('OK, PhantomJS is ready.');
	};

	var mapCLArguments = function() {
		var map = {},
			i,
			key;

		if (system.args.length < 2) {
			console.log('run PhantomJS as server: sudo phantomjs print.js -host 0.0.0.0 -port 9001');
		}

		for (i = 0; i < system.args.length; i += 1) {
			if (system.args[i].charAt(0) === '-') {
				key = system.args[i].substr(1, i.length);
				map[key] = system.args[i + 1];
			}
		}
		return map;
	};

	var args = mapCLArguments();
	if (args.host !== undefined && args.port !== undefined) {
		startServer(args.host, args.port);
	} else {
		console.log("run cmd to generate img", JSON.stringify(args));
		render(args, function(result) {
			console.log(result);
			phantom.exit();
		});
	}
}());