(function () {
	'use strict';

	var DEBUG = true;

	var system = require('system'),
		fs = require('fs');
	var config = {
		tmpFolder: '/var/www/img.timm.vargainc.com/',
		//baseUrl: "http://dev.timm.vargainc.com/",
		baseUrl: "http://98.189.6.210:9000/timm201507/api/",
		//baseUrl: "http://192.168.1.9:9000/timm201507/api/",
	};

	var guid = function (callback) {
		var backGuid = function () {
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

		child.stdout.on("data", function (data) {
			console.log("spawnSTDOUT:", JSON.stringify(data));
			callback && callback(data.replace('\n', ''));
		});

		child.stderr.on("data", function (data) {
			callback && callback(backGuid());
		});

		child.on("exit", function (code) {
			console.log("spawnEXIT:", code);
		});
	};

	var render = function (params, exitCallback) {
		var exit = function (result) {
			page.close();
			exitCallback(result);
		};

		console.log(JSON.stringify(params));
		var page = require('webpage').create();
		page.viewportSize = {
			width: 800,
			height: 800
		};
		page.settings.userAgent = 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10.11; rv:44.0) Gecko/20100101 Firefox/44.0';
		page.settings.javascriptEnabled = true;
		page.settings.loadImages = true;
		page.settings.localToRemoteUrlAccessEnabled = true;
		page.settings.webSecurityEnabled = false;
		page.settings.resourceTimeout = 5 * 60 * 1000;

		page.onConsoleMessage = function (msg) {
			DEBUG && console.log('CONSOLE: ' + msg);
		};

		page.onResourceRequested = function(request) {
		  	//DEBUG && console.log('Request ' + request.method + ' ' + request.url);
		};

		page.onResourceReceived = function(response) {
		  	//DEBUG && console.log('Receive ' + response.status + ' ' + response.url + ' ' + response.contentType + ' ' + response.bodySize);
		};


		var url = '';
		switch (params.type) {
		case 'Campaign':
			page.viewportSize = {
				width: 828,
				height: 766
			};
			url = 'print.campaign.html';
			break;
		case 'SubMap':
			page.viewportSize = {
				width: 828,
				height: 788
			};
			url = 'print.submap.html';
			break;
		case 'DMap':
			page.viewportSize = {
				width: 828,
				height: 877
			};
			url = 'print.dmap.html';
			break;
		case 'Distribute':
			page.viewportSize = {
				width: 2298,
				height: 2177
			};
			url = 'print.distribute.html';
			break;
		default:

			break;
		}
		if (url === '') {
			exit('bad params');
			return;
		}

		page.open(url, function (status) {
			if (status === 'success') {
				console.log('open map html success');
				var responseResult = {};
				page.onCallback = function (data) {
					if (data && data.success) {
						switch (data.status) {
						case 'tilesloaded':
							guid(function (uuid) {
								var fileName = uuid + '.jpg';
								console.log(fileName);
								page.render(config.tmpFolder + fileName, {
									format: 'jpeg',
									quality: data.quality ?  data.quality : '75'
								});
								responseResult.tiles = fileName;
							});
							break;
						case 'finished':
							guid(function (uuid) {
								var fileName = uuid + '.png';
								console.log(fileName);
								page.render(config.tmpFolder + fileName, {
									format: 'png'
								});
								responseResult.geometry = fileName;
								responseResult.success = true;
								exit(responseResult);
							});

							break;
						case 'changeSize':
							page.viewportSize = {
								width: data.width,
								height: data.height
							};
							break;
						default:
							exit({
								success: false,
								error: data
							});
							break;
						}
						console.log('CALLBACK: ' + JSON.stringify(data));
					} else {
						exit({
							success: false,
							msg: data
						});
					}
				};

				
				page.evaluateAsync(function (params) {
					begin(JSON.parse(params));
				}, 1000, JSON.stringify(params));
			} else {
				console.log('open page failed. exit');
				exit('open page failed. exit');
			}
		});
	};

	var parseUrl = function (url) {
		var params = url.replace('/', '').replace('?', '').split('&');
		var map = {};
		for (var i = 0; i < params.length; i++) {
			var index = params[i].indexOf('=');
			var key = decodeURIComponent(params[i].substr(0, index));
			var value = decodeURIComponent(params[i].substring(index + 1));

			if(['true', 'false'].indexOf(value) > -1){
				map[key] = value == 'true';
			}else if(key.indexOf('[]') > -1){
				key = key.replace('[]', '');
				if(typeof map[key] === 'undefined'){
					map[key] = [];
				}
				map[key].push(value);
			}else{
				map[key] = value;
			}
		}
		return map;
	};

	var startServer = function (host, port) {
		var server = require('webserver').create();

		server.listen(host + ':' + port, {
				keepAlive: true
			},
			function (request, response) {
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
					} else if (false) {
						var jsonStr = request.postRaw || request.post;
						var params = parseUrl(jsonStr);
						response.statusCode = 200;
						response.setHeader('Content-Type', 'application/json; charset=utf-8');
						response.setHeader('access-control-allow-origin', '*');
						var msg = '';
						var result = {
							"tiles": "93F38CAD-7216-42B1-B3FC-BB54B43816C5.jpg",
							"geometry": "A1F4CBFE-358D-4175-A98F-70038D745A16.png",
							"success": false
						};
						result.campaignId = params.campaignId,
						result.submapId = params.submapId,
						result.dmapId = params.dmapId
						msg = JSON.stringify(result);

						console.log(msg);
						response.setHeader("Content-Length", msg.length);
						response.write(msg);
						response.close();

					} else {
						var jsonStr = request.postRaw || request.post;
						var params = parseUrl(jsonStr);
						//params.baseUrl = config.baseUrl;
						render(params, function (result) {
							response.statusCode = 200;
							response.setHeader('Content-Type', 'application/json; charset=utf-8');
							response.setHeader('access-control-allow-origin', '*');
							var msg = '';

							result.campaignId = params.campaignId,
							result.submapId = params.submapId,
							result.dmapId = params.dmapId
							msg = JSON.stringify(result);

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

	var mapCLArguments = function () {
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
		render(args, function (result) {
			console.log(result);
			phantom.exit();
		});
	}
}());