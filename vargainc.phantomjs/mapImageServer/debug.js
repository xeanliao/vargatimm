DEBUG = true;
console.log('begin debug');
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

page.onConsoleMessage = function (msg) {
	DEBUG && console.log('CONSOLE: ' + msg);
};

page.onResourceRequested = function (request) {
	DEBUG && console.log('Request ' + request.method + ' ' + request.url);
};

page.onResourceReceived = function (response) {
	DEBUG && console.log('Receive ' + response.status + ' ' + response.url + ' ' + response.contentType + ' ' + response.bodySize);
};
page.onCallback = function (data) {
	page.render('debug.jpg', {
		format: 'jpeg',
		quality: 75
	});
};
console.log('load debug.html');
page.open('debug.html', function (status) {
	if (status === 'success') {
		console.log('open debug html success');
	} else {
		console.log('open debug html faild');
	}
});