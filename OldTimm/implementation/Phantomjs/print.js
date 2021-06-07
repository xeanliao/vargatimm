(function() {
	'use strict';

	var DEBUG = true;

	var system = require('system'),
		fs = require('fs');
	var config = {
		tmpFolder: '/var/www/img.timm.vargainc.com/',
		//baseUrl: "http://192.168.1.9:9000/timm201404/api/",
		baseUrl: "http://98.189.6.210:9000/timm201404/api/",
	};

	var guid = function(callback) {
		var backGuid = function() {
			function s4() {
				return Math.floor((1 + Math.random()) * 0x10000)
					.toString(16)
					.substring(1);
			}
			return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
				s4() + '-' + s4() + s4() + s4();
		};
		console.log("begin generate uuid");
		var spawn = require("child_process").spawn;

		var child = spawn("uuidgen", []);

		child.stdout.on("data", function(data) {
			console.log("spawnSTDOUT:", JSON.stringify(data));
			callback && callback(data.replace('\n', ''));
		});

		child.stderr.on("data", function(data) {
			callback && callback(backGuid());
		});

		child.on("exit", function(code) {
			console.log("spawnEXIT:", code);
		});
	};

	var render = function(params, exitCallback) {
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
			DEBUG && console.log('CONSOLE: ' + JSON.stringify(msg));
		};


		var url = '';
		switch (params.type) {
			case 'Campaign':
				page.viewportSize = {
					width: 780 * 2,
					height: 580 * 2
				};
				url = 'print.submap.html';
				break;
			case 'SubMap':
				page.viewportSize = {
					width: 780 * 2,
					height: 580 * 2
				};
				url = 'print.submap.html';
				break;
			case 'DMap':
				page.viewportSize = {
					width: 780 * 2,
					height: 650 * 2
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
				/**
				 * monitor resource to make sure all google map download.
				 */
				// var resourceCount = 0;
				// page.onResourceRequested = function(request) {
				// 	resourceCount++;
				// };
				// page.onResourceReceived = function(requestData) {
				// 	setTimeout(function() {
				// 		resourceCount--;
				// 	}, 3000);
				// };
				// page.onResourceTimeout = function(request) {
				// 	console.log('Resource Timeout for request ' + request.url);
				// 	setTimeout(function() {
				// 		resourceCount--;
				// 	}, 3000);
				// };
				// page.onResourceError = function(resourceError) {
				// 	console.log('Resource error for request ' + resourceError.url);
				// 	setTimeout(function() {
				// 		resourceCount--;
				// 	}, 3000);
				// };

				page.onCallback = function(data) {
					if (data && data.success) {
						console.log('CALLBACK: ' + JSON.stringify(data));
						setTimeout(rendeScreenshot, 10 * 1000);
					} else {
						exit({
							success: false
						});
					}
				};

				var checkResource = function() {
					if (resourceCount <= 0) {
						console.log("resource finished");
						setTimeout(rendeScreenshot, 5000);
					} else {
						console.log("waiting resource finish");
						setTimeout(checkResource, 1000);
					}
				};

				var rendeScreenshot = function() {
					guid(function(uuid) {
						var fileName = uuid + '.jpg';
						console.log(fileName);
						page.render(config.tmpFolder + fileName, {
							format: 'jpeg',
							quality: '75'
						});
						exit(fileName);
					});
				};

				var exit = function(result) {
					page.close();
					exitCallback(result);
				};
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
			map[key] = decodeURIComponent(value);
		}
		return map;
	};

	var startServer = function(host, port) {
		var server = require('webserver').create();

		server.listen(host + ':' + port, {
				keepAlive: true
			},
			function(request, response) {
				try {
					if (request.url === '/status') {
						// for server health validation
						response.statusCode = 200;
						response.headers = {
							'Content-Type': 'text/html',
							'Content-Length': 0
						};
						response.write('');
						response.close();
					} else {
						var jsonStr = request.postRaw || request.post;
						var params = parseUrl(jsonStr);
						params.baseUrl = config.baseUrl;
						render(params, function(result) {
							response.statusCode = 200;
							response.setHeader('Content-Type', 'application/json; charset=utf-8');
							response.setHeader('access-control-allow-origin', '*');
							var msg = '';
							if (typeof result === 'string') {
								msg = JSON.stringify({
									success: true,
									path: result,
									campaignId: params.campaignId,
									submapId: params.submapId,
									dmapId: params.dmapId
								});
							} else {
								msg = JSON.stringify(result);
							}
							console.log(msg);
							response.setHeader("Content-Length", msg.length);
							response.write(msg);
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
		console.log("run cmd to generate img", args);
		render(args, function(result) {
			console.log(result);
			phantom.exit();
		});
	}
}());