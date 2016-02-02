function run_cmd(cmd, args, callBack ) {
    var spawn = require('child_process').spawn;
    var child = spawn(cmd, args);
    var resp = "";

    child.stdout.on('data', function (buffer) { resp += buffer.toString() });
    child.stdout.on('end', function() { callBack (resp) });
}
var total = 10, finished = 0;
var result = {};
var fs = require('fs');
for(var i = 1; i <= total; i++){

	var callback = function(){
		finished++;
		console.log(i);
		fs.readFile(i + '.svg', function(err, data){
			result[i] = data;
			if(finished == total){
				console.log(result);
				console.log("completed");
			}
		});
	}
	run_cmd("convert", ['-fill', 'black', '-font', 'Verdana', 'label:' + i, i + '.svg'], callback);
}