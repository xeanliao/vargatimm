// Aspect
function PerformanceAspect() {}

PerformanceAspect.prototype = {
/*
	before__draw_boundary: function(args) {
		return args;
	},
	after__draw_boundary: function(result) {
		return result;
	},
*/
	profile__draw_boundary: function(realFunc, args, funcName) {
		var start = (new Date()).getTime();
		//log.debug('Going to execute ' + funcName);
		var result = realFunc.apply(this,args);
		//log.debug('Elapsed time : ' + ((new Date()).getTime ( ) - start)/1000 + ' milliseconds in ' + funcName);
		//alert((new Date().getTime () - start)/1000 + ' milliseconds in ' + funcName);
		return result;
	}
}

Weaver.addAdvice(PerformanceAspect, 'profile__draw_boundary', 'around', SubMapForm, '_draw_boundary');
