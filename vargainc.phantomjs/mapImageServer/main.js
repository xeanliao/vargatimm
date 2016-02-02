var DEBUG = true;
console.log("begin");
phantom.injectJs('js/jquery-2.1.4.min.js');

function PrintTask(){
	this.init();
}

$.extend(PrintTask.prototype, {
	options: {
		importFile: "data.txt",
		outputFile: "result.txt"
	},
	init: function(){
		DEBUG && console.log("init task" + this.taskId);
		var system = require('system');
		system.args.length === 0 && console.log("use default setting to run this scripts!");
  		this.options = $.extend(this.options, {
  			importFile: system.args[1],
  			outputFile: system.args[2],
  			webPage: "map.html"
  		});
  		DEBUG && console.log("import data file: " + this.options.importFile);
  		DEBUG && console.log("output data file: " + this.options.outputFile);
  		this.taskId++;
	},
	startTask: function(){
		DEBUG && console.log("task begin");
		DEBUG && console.log("begin load page: " + this.options.webPage);
		this.loadPage(this.options.webPage);
	},
	loadPage: function(address){
		this.page = require('webpage').create();
		this.page.viewportSize = { width: 320, height: 320 };
		// DEBUG && this.page.onResourceReceived = function(response) {
		// 	console.log('Receive ' + JSON.stringify(response, undefined, 4));
		// };
		// DEBUG && this.page.onError = function (msg, trace) {
		//     console.log(msg);
		//     trace.forEach(function(item) {
		//         console.log('  ', item.file, ':', item.line);
		//     });
		// };
		var self = this;
		this.page.open(address, function(){
			self.checkMapInit.call(self);
		});
	},
	checkMapInit: function(){
		console.log("checking google map init");
		var result = this.page.evaluate(function () {
			
			return window.init;

		});
		console.log(result);

		var self = this;
		if(!result){
			console.log("not ready.");
			setTimeout(function(){
				self.checkMapInit.call(self);
			}, 5000);
		}else{
			this.onPageReady();
		}
	},
	onPageReady: function(){
		DEBUG && console.log("page loaded.");

		this.page.evaluate(function () {
			geoLocation("Sydney, NSW");
		});
		DEBUG && console.log("creat screenshot.");
		this.page.render('screenshot.png');
		phantom.exit();
	}
});

console.log("start");
var task = new PrintTask();
task.startTask();

