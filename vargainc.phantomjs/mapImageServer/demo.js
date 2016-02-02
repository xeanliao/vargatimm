var DEBUG = false;
console.log("begin");
var page = require('webpage').create();
var system = require('system');
var fileStream = require('fs');
page.viewportSize = { width: 800, height: 600 };
page.settings.userAgent = 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.120 Safari/537.36';
page.settings.javascriptEnabled = true;
page.settings.loadImages = true;
page.settings.localToRemoteUrlAccessEnabled = true;
page.settings.webSecurityEnabled = false;
page.settings.resourceTimeout = 60 * 1000;
/**
 * include jquery & queue
 */
phantom.injectJs('js/jquery-2.1.4.min.js');
phantom.injectJs('js/Queue.src.js');
page.onResourceReceived = function(response) {
	DEBUG && console.log('Receive ' + response.status + " : " + response.url);
};
page.onError = function (msg, trace) {
    console.log(msg);
    trace.forEach(function(item) {
        console.log('  ', item.file, ':', item.line);
    });
};
page.onConsoleMessage = function(msg, lineNum, sourceId) {
  	DEBUG && console.log('CONSOLE: ' + msg + ' (from line #' + lineNum + ' in "' + sourceId + '")');
};
page.onCallback = function(data) {
  	console.log('CALLBACK: ' + JSON.stringify(data));
};

function Task(timeout, inputFile, outputFile){
	this.timeout = timeout;
	this.inputFile = inputFile;
	this.outputFile = outputFile;
	this.queue = new Queue();

	this.init();
}

$.extend(Task.prototype, {
	init: function(){
		page.onCallback = $.proxy(this.parseResult, this);
		this.loadData();
		this.queryGeoLocation();
	},
	loadData: function(){
		DEBUG && console.log("begin load data. file name: " + this.inputFile);
		var fs = fileStream.open(this.inputFile, 'r');
		var line = fs.readLine();
		while(line){
			var address = line.split(',');
			var queryAddress = address[0] + address[1] + ", " + address[2] + ", " + address[3] + " " + address[4] + ", United States";
			console.log(queryAddress);
			this.queue.enqueue(queryAddress);
			line = fs.readLine();
		}
		fs.close();
	},
	queryGeoLocation: function(){
		DEBUG && console.log("begin query geo location");

		if(this.queue.isEmpty()){
			setTimeout($.proxy(this.generateScreenshot, this), 1000);
			return;
		}
		var nextAddress = this.queue.dequeue();
		console.log("address: " + nextAddress);
		page.evaluateAsync(function (address) {
			geoLocation(address);
		}, this.timeout, nextAddress);
	},
	parseResult: function(result){
		//console.log(JSON.stringify(result));
		//return;
		if(result && result.success){
			try {
				var content = [];
				$.each(result.data, function(){
					content.push(result.query + "," + this.formatted_address + "," + this.geometry.location[0] + "," + this.geometry.location[1]);
				});
				var output = result.query + "," + 
					result.data[0].formatted_address + "," + 
					result.data[0].geometry.location[0] + "," + 
					result.data[0].geometry.location[1] + "\n";
			    fileStream.write(this.outputFile, output, 'a');
			    
			} catch(e) {
			    console.log(e);
			}
		}else{
			try {
			    fileStream.write(this.outputFile, result.query + ",,,\n", 'a');			    
			} catch(e) {
			    console.log(e);
			}
		}
		this.queryGeoLocation();
	},
	generateScreenshot: function(){
		console.log("begin generate screenshot");
		page.onCallback = $.proxy(this.mapReadyForScreenshot, this);
		page.evaluate(function(){
			showAllMarkers();
		});
	},
	mapReadyForScreenshot: function(){
		console.log("recived map is ready");
		page.render("screenshot.png");
		phantom.exit();
	}
});

/**
 * open map.html and query geo location 
 */
page.open("map.html", function(status){
	console.log("Status: " + status);
	if(status === "success") {
		/**
		 * start geo location task
		 */
		var task = new Task(1000, "address.csv", "geolocation.csv");
	}else{
		console.log("open page failed. exit");
		phantom.exit();
	}
});