/*
 * AspectJS.
 * Copyright (C) 2006  Sébastien LECACHEUR
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
var InvalidAspect = new Error ( 'Missing a valid aspect. Aspect is not a function.');
var InvalidAdvice = new Error ( 'Missing a valid advice. Advice is not a function.');
var InvalidAdviceType = new Error ( 'Missing a valid advice type. Advice type is not supported.');
var InvalidObject = new Error ( 'Missing valid object or an array of valid objects.');
var InvalidMethod = new Error ( 'Missing valid method to apply aspect on.');
var Weaver = new Object ( );

var Weaver = {
	/** 
	 * current version of AspectJS
	 * @static
	 */
  	version: "1.0.0"
}

/**
 * @param aspect the advice's name
 * @param name name is the advice's function name
 * @param type the joint point type (can be : before, after or around)
 * @param obj the pointcut's target Object
 * @param bindto the pointcut's function name
 */
Weaver.addAdvice = function ( aspect, name, type, obj, bindto) {
	var func = aspect.prototype?aspect.prototype[name]:aspect[name];
	if (!func) {
		func = name;
	}

	if ( typeof ( bindto) != 'object') bindto = Array ( bindto);
	
	if ( type != 'before'&& type != 'after' && type != 'around') {
		throw InvalidAdviceType;
	}

	var prototype = obj.prototype?obj.prototype:obj;

	for ( var n = 0; n < bindto.length; n++) {
		var fName = bindto[n];
		var old = prototype[fName];

		if ( !old) {
			throw InvalidMethod;
		}

		old = Weaver._prepareFunc(old,fName);
		prototype[fName] = Weaver._injectCode(new String(old),type,func);
	}
}
Weaver._prepareFunc = function ( func, name) {
	var src = new String ( func);
	if ( src.indexOf ( '_aspectjs_original')!=-1) {
		return func;
	} else {
		return new Function('var _aspectjs_original = ' + func + ';var _aspectjs_before = Array();var _aspectjs_after = Array();var _aspectjs_around = Array();return Weaver._weave(this, \''+name+'\', _aspectjs_original, _aspectjs_before, _aspectjs_after, _aspectjs_around, arguments);');
	}
}
Weaver._injectCode = function ( func, destination, name) {
	var src = func.substring(func.indexOf ( 'var _aspectjs_original'),func.indexOf ( 'return Weaver._weave('))+'_aspectjs_'+destination+'.push('+name+');'+func.substring(func.indexOf ( 'return Weaver._weave('));
	src = src.substring(0,src.length-1);
	return new Function(src);
}
Weaver._weaveBefore = function (obj,name,funcs,args) {
	for (var i = 0; i < funcs.length; i++) {
		args = funcs[i].apply(obj,Array(args),name);
	}

	return args;
}
Weaver._weaveAround = function (obj,name,original,around,position,args) {
	var result;
	if ( around.length > 0 && position < around.length) {
		result = around[position].apply(obj,Array(Weaver._weaveAround,Array(obj,name,original,around,position+1,args),name),name);
	} else {
		result = original.apply(obj,args,name);
	}

	return result;
}
Weaver._weaveAfter = function (obj,name,funcs,result) {
	for (var i = 0; i < funcs.length; i++) {
		result = funcs[i].apply(obj,Array(result),name);
	}

	return result;
}
Weaver._weave = function (obj,name,original,before,after,around,args) {
	return Weaver._weaveAfter(obj,name,after,Weaver._weaveAround(obj,name,original,around,0,Weaver._weaveBefore(obj,name,before,args)));
}