webpackJsonp([1],[
/* 0 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	var _isFunction2 = __webpack_require__(88);

	var _isFunction3 = _interopRequireDefault(_isFunction2);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	var _reactDom = __webpack_require__(320);

	var _reactDom2 = _interopRequireDefault(_reactDom);

	var _postal = __webpack_require__(129);

	var _postal2 = _interopRequireDefault(_postal);

	var _route = __webpack_require__(363);

	var _route2 = _interopRequireDefault(_route);

	var _user = __webpack_require__(71);

	var _user2 = _interopRequireDefault(_user);

	var _main = __webpack_require__(377);

	var _main2 = _interopRequireDefault(_main);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	/**
	 * register loading event except ajax request is quite
	 */
	(0, _jquery2.default)(document).ajaxSend(function (event, xhr, settings) {
		if (settings.quite !== true) {
			_postal2.default.publish({
				channel: 'View',
				topic: 'showLoading'
			});
		}
	});
	(0, _jquery2.default)(document).ajaxComplete(function (event, xhr, settings) {
		if (settings.quite !== true) {
			_postal2.default.publish({
				channel: 'View',
				topic: 'hideLoading'
			});
		}
	});

	/**
	 * override base url
	 */
	var backboneSync = _backbone2.default.sync;
	_backbone2.default.sync = function (method, model, options) {
		if (!options.url) {
			options.url = (0, _isFunction3.default)(model.url) ? model.url() : model.url;
		}
		if (!options.url) {
			options.url = model.urlRoot;
		}
		options.url = '../api/' + options.url;
		if (typeof RELEASE_VERSION !== 'undefined') {
			options.url = 'http://timm.vargainc.com/' + RELEASE_VERSION + '/api/' + options.url;
		}
		return backboneSync(method, model, options);
	};

	var appRouter = new _route2.default();
	var userModel = new _user2.default();
	userModel.fetchCurrentUser().then(function () {
		var LayoutViewInstance = _react2.default.createFactory(_main2.default);
		var layoutViewInstance = LayoutViewInstance({
			user: userModel
		});
		var layout = _reactDom2.default.render(layoutViewInstance, document.getElementById('main-container'));
		var appRouter = new _route2.default();
		appRouter.on('route', function () {
			// Topic.publish({
			// 	channel: 'View',
			// 	topic: 'showLoading'
			// });
		});
		_backbone2.default.history.start();
	}).catch(function () {
		// window.location = '../login.html';
	});

/***/ },
/* 1 */,
/* 2 */,
/* 3 */,
/* 4 */,
/* 5 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _isObject2 = __webpack_require__(20);

	var _isObject3 = _interopRequireDefault(_isObject2);

	var _isFunction2 = __webpack_require__(88);

	var _isFunction3 = _interopRequireDefault(_isFunction2);

	var _isString2 = __webpack_require__(210);

	var _isString3 = _interopRequireDefault(_isString2);

	var _forEach2 = __webpack_require__(59);

	var _forEach3 = _interopRequireDefault(_forEach2);

	var _unset2 = __webpack_require__(536);

	var _unset3 = _interopRequireDefault(_unset2);

	var _assign2 = __webpack_require__(516);

	var _assign3 = _interopRequireDefault(_assign2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	var _postal = __webpack_require__(129);

	var _postal2 = _interopRequireDefault(_postal);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	__webpack_require__(4);

	__webpack_require__(164);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = {
		getDefaultProps: function getDefaultProps() {
			return {
				registeredTopic: {}
			};
		},
		subscribe: function subscribe(opt) {
			var params;
			if (arguments.length == 2 && (0, _isString3.default)(arguments[0]) && (0, _isFunction3.default)(arguments[1])) {
				params = {
					channel: 'View',
					topic: arguments[0],
					callback: arguments[1]
				};
			} else {
				params = (0, _assign3.default)({
					channel: 'View',
					topic: 'undefined'
				}, opt);
			}

			var name = params.channel + '.*/-+-\*.' + params.topic;
			this.props.registeredTopic[name] && this.props.registeredTopic[name].unsubscribe();
			var signal = _postal2.default.subscribe(params);
			this.props.registeredTopic[name] = signal;
		},
		unsubscribe: function unsubscribe(topic) {
			var name = 'View' + '.*/-+-\*.' + topic;
			if (this.props.registeredTopic[name]) {
				this.props.registeredTopic[name].unsubscribe();
				(0, _unset3.default)(this.props.registeredTopic, name);
			}
		},
		publish: function publish() {
			var opt;
			if (arguments.length == 1 && (0, _isString3.default)(arguments[0])) {
				opt = {
					channel: 'View',
					topic: arguments[0],
					data: null
				};
			} else if (arguments.length == 4 && (0, _isString3.default)(arguments[0]) && (0, _isFunction3.default)(arguments[1]) && arguments[2] instanceof _backbone2.default.Model && (0, _isObject3.default)(arguments[3])) {
				opt = {
					channel: 'View',
					topic: arguments[0],
					data: {
						view: arguments[1],
						params: {
							model: arguments[2]
						},
						options: arguments[3]
					}
				};
			} else if (arguments.length == 4 && (0, _isString3.default)(arguments[0]) && (0, _isFunction3.default)(arguments[1]) && arguments[2] instanceof _backbone2.default.Collection && (0, _isObject3.default)(arguments[3])) {
				opt = {
					channel: 'View',
					topic: arguments[0],
					data: {
						view: arguments[1],
						params: {
							collection: arguments[2]
						},
						options: arguments[3]
					}
				};
			} else if (arguments.length == 3 && (0, _isString3.default)(arguments[0]) && (0, _isFunction3.default)(arguments[1]) && arguments[2] instanceof _backbone2.default.Model) {
				opt = {
					channel: 'View',
					topic: arguments[0],
					data: {
						view: arguments[1],
						params: {
							model: arguments[2]
						}
					}
				};
			} else if (arguments.length == 3 && (0, _isString3.default)(arguments[0]) && (0, _isFunction3.default)(arguments[1]) && arguments[2] instanceof _backbone2.default.Collection) {
				opt = {
					channel: 'View',
					topic: arguments[0],
					data: {
						view: arguments[1],
						params: {
							collection: arguments[2]
						}
					}
				};
			} else if (arguments.length == 2 && (0, _isString3.default)(arguments[0]) && (0, _isFunction3.default)(arguments[1])) {
				opt = {
					channel: 'View',
					topic: arguments[0],
					data: {
						view: arguments[1]
					}
				};
			} else if (arguments.length > 1 && (0, _isString3.default)(arguments[0])) {
				opt = {
					channel: 'View',
					topic: arguments[0],
					data: arguments[1]
				};
			} else {
				opt = arguments[0];
			}
			_postal2.default.publish((0, _assign3.default)({
				channel: 'View',
				topic: 'undefined'
			}, opt));
		},
		componentWillUnmount: function componentWillUnmount() {
			(0, _forEach3.default)(this.props.registeredTopic, function (i) {
				i.unsubscribe();
			});
		},
		scrollTop: function scrollTop(ele) {
			$('.off-canvas-wrapper-inner').stop().animate({
				scrollTop: $(ele).offset().top
			}, 600);
		},
		confirm: function confirm(content) {
			var self = this;
			return new _bluebird2.default(function (resolve, reject) {
				var cancel = function cancel() {
					self.publish('showDialog');
					reject(new Error('user cancel'));
				};
				var okay = function okay() {
					self.publish('showDialog');
					resolve();
				};
				if ((0, _isString3.default)(content)) {
					content = content.replace('\r', '').split('\n');
					content = content.map(function (row) {
						return _react2.default.createElement(
							'p',
							null,
							row
						);
					});
				}
				var view = _react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'p',
							null,
							'\xA0'
						),
						_react2.default.createElement(
							'h5',
							null,
							content
						),
						_react2.default.createElement(
							'p',
							null,
							'\xA0'
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'button-group float-right' },
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', className: 'button success tiny', onClick: okay },
								'Okay'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'button-group float-right' },
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', className: 'button tiny', onClick: cancel },
								'Cancel'
							)
						)
					),
					_react2.default.createElement(
						'button',
						{ onClick: cancel, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
						_react2.default.createElement(
							'span',
							{ 'aria-hidden': 'true' },
							'\xD7'
						)
					)
				);
				self.publish('showDialog', {
					view: view
				});
			});
		},
		alert: function alert(content) {
			var self = this;
			if ((0, _isString3.default)(content)) {
				content = content.replace('\r', '').split('\n');
				content = content.map(function (row) {
					return _react2.default.createElement(
						'p',
						null,
						row
					);
				});
			}
			var closeDialog = function closeDialog() {
				self.publish('showDialog');
			};
			var view = _react2.default.createElement(
				'div',
				{ className: 'row' },
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'p',
						null,
						'\xA0'
					),
					_react2.default.createElement(
						'h5',
						null,
						content
					),
					_react2.default.createElement(
						'p',
						null,
						'\xA0'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'button-group float-right' },
						_react2.default.createElement(
							'a',
							{ href: 'javascript:;', className: 'button tiny', onClick: closeDialog },
							'Okay'
						)
					)
				),
				_react2.default.createElement(
					'button',
					{ onClick: closeDialog, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					_react2.default.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'\xD7'
					)
				)
			);
			this.publish('showDialog', {
				view: view
			});
		}
	};

/***/ },
/* 6 */,
/* 7 */,
/* 8 */,
/* 9 */,
/* 10 */,
/* 11 */,
/* 12 */
/***/ function(module, exports) {

	/**
	 * Checks if `value` is classified as an `Array` object.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is an array, else `false`.
	 * @example
	 *
	 * _.isArray([1, 2, 3]);
	 * // => true
	 *
	 * _.isArray(document.body.children);
	 * // => false
	 *
	 * _.isArray('abc');
	 * // => false
	 *
	 * _.isArray(_.noop);
	 * // => false
	 */
	var isArray = Array.isArray;

	module.exports = isArray;


/***/ },
/* 13 */,
/* 14 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = __webpack_require__(517);


/***/ },
/* 15 */,
/* 16 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = __webpack_require__(59);


/***/ },
/* 17 */
/***/ function(module, exports, __webpack_require__) {

	var arrayMap = __webpack_require__(31),
	    baseIteratee = __webpack_require__(32),
	    baseMap = __webpack_require__(184),
	    isArray = __webpack_require__(12);

	/**
	 * Creates an array of values by running each element in `collection` thru
	 * `iteratee`. The iteratee is invoked with three arguments:
	 * (value, index|key, collection).
	 *
	 * Many lodash methods are guarded to work as iteratees for methods like
	 * `_.every`, `_.filter`, `_.map`, `_.mapValues`, `_.reject`, and `_.some`.
	 *
	 * The guarded methods are:
	 * `ary`, `chunk`, `curry`, `curryRight`, `drop`, `dropRight`, `every`,
	 * `fill`, `invert`, `parseInt`, `random`, `range`, `rangeRight`, `repeat`,
	 * `sampleSize`, `slice`, `some`, `sortBy`, `split`, `take`, `takeRight`,
	 * `template`, `trim`, `trimEnd`, `trimStart`, and `words`
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Collection
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} [iteratee=_.identity] The function invoked per iteration.
	 * @returns {Array} Returns the new mapped array.
	 * @example
	 *
	 * function square(n) {
	 *   return n * n;
	 * }
	 *
	 * _.map([4, 8], square);
	 * // => [16, 64]
	 *
	 * _.map({ 'a': 4, 'b': 8 }, square);
	 * // => [16, 64] (iteration order is not guaranteed)
	 *
	 * var users = [
	 *   { 'user': 'barney' },
	 *   { 'user': 'fred' }
	 * ];
	 *
	 * // The `_.property` iteratee shorthand.
	 * _.map(users, 'user');
	 * // => ['barney', 'fred']
	 */
	function map(collection, iteratee) {
	  var func = isArray(collection) ? arrayMap : baseMap;
	  return func(collection, baseIteratee(iteratee, 3));
	}

	module.exports = map;


/***/ },
/* 18 */,
/* 19 */,
/* 20 */
/***/ function(module, exports) {

	/**
	 * Checks if `value` is the
	 * [language type](http://www.ecma-international.org/ecma-262/7.0/#sec-ecmascript-language-types)
	 * of `Object`. (e.g. arrays, functions, objects, regexes, `new Number(0)`, and `new String('')`)
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is an object, else `false`.
	 * @example
	 *
	 * _.isObject({});
	 * // => true
	 *
	 * _.isObject([1, 2, 3]);
	 * // => true
	 *
	 * _.isObject(_.noop);
	 * // => true
	 *
	 * _.isObject(null);
	 * // => false
	 */
	function isObject(value) {
	  var type = typeof value;
	  return value != null && (type == 'object' || type == 'function');
	}

	module.exports = isObject;


/***/ },
/* 21 */,
/* 22 */
/***/ function(module, exports, __webpack_require__) {

	var freeGlobal = __webpack_require__(194);

	/** Detect free variable `self`. */
	var freeSelf = typeof self == 'object' && self && self.Object === Object && self;

	/** Used as a reference to the global object. */
	var root = freeGlobal || freeSelf || Function('return this')();

	module.exports = root;


/***/ },
/* 23 */
/***/ function(module, exports, __webpack_require__) {

	var arrayLikeKeys = __webpack_require__(176),
	    baseKeys = __webpack_require__(183),
	    isArrayLike = __webpack_require__(25);

	/**
	 * Creates an array of the own enumerable property names of `object`.
	 *
	 * **Note:** Non-object values are coerced to objects. See the
	 * [ES spec](http://ecma-international.org/ecma-262/7.0/#sec-object.keys)
	 * for more details.
	 *
	 * @static
	 * @since 0.1.0
	 * @memberOf _
	 * @category Object
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of property names.
	 * @example
	 *
	 * function Foo() {
	 *   this.a = 1;
	 *   this.b = 2;
	 * }
	 *
	 * Foo.prototype.c = 3;
	 *
	 * _.keys(new Foo);
	 * // => ['a', 'b'] (iteration order is not guaranteed)
	 *
	 * _.keys('hi');
	 * // => ['0', '1']
	 */
	function keys(object) {
	  return isArrayLike(object) ? arrayLikeKeys(object) : baseKeys(object);
	}

	module.exports = keys;


/***/ },
/* 24 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	  value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _backbone2.default.Model.extend({});

/***/ },
/* 25 */
/***/ function(module, exports, __webpack_require__) {

	var isFunction = __webpack_require__(88),
	    isLength = __webpack_require__(121);

	/**
	 * Checks if `value` is array-like. A value is considered array-like if it's
	 * not a function and has a `value.length` that's an integer greater than or
	 * equal to `0` and less than or equal to `Number.MAX_SAFE_INTEGER`.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is array-like, else `false`.
	 * @example
	 *
	 * _.isArrayLike([1, 2, 3]);
	 * // => true
	 *
	 * _.isArrayLike(document.body.children);
	 * // => true
	 *
	 * _.isArrayLike('abc');
	 * // => true
	 *
	 * _.isArrayLike(_.noop);
	 * // => false
	 */
	function isArrayLike(value) {
	  return value != null && isLength(value.length) && !isFunction(value);
	}

	module.exports = isArrayLike;


/***/ },
/* 26 */,
/* 27 */,
/* 28 */,
/* 29 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createClass({
		displayName: 'loading',

		mixins: [_base2.default],
		getDefaultProps: function getDefaultProps() {
			return {
				text: 'LOADING'
			};
		},
		render: function render() {
			return _react2.default.createElement(
				'div',
				{ className: 'loading' },
				_react2.default.createElement(
					'div',
					{ className: 'gear one' },
					_react2.default.createElement(
						'svg',
						{ id: 'blue', viewBox: '0 0 100 100', fill: '#94DDFF' },
						_react2.default.createElement('path', { d: 'M97.6,55.7V44.3l-13.6-2.9c-0.8-3.3-2.1-6.4-3.9-9.3l7.6-11.7l-8-8L67.9,20c-2.9-1.7-6-3.1-9.3-3.9L55.7,2.4H44.3l-2.9,13.6      c-3.3,0.8-6.4,2.1-9.3,3.9l-11.7-7.6l-8,8L20,32.1c-1.7,2.9-3.1,6-3.9,9.3L2.4,44.3v11.4l13.6,2.9c0.8,3.3,2.1,6.4,3.9,9.3      l-7.6,11.7l8,8L32.1,80c2.9,1.7,6,3.1,9.3,3.9l2.9,13.6h11.4l2.9-13.6c3.3-0.8,6.4-2.1,9.3-3.9l11.7,7.6l8-8L80,67.9      c1.7-2.9,3.1-6,3.9-9.3L97.6,55.7z M50,65.6c-8.7,0-15.6-7-15.6-15.6s7-15.6,15.6-15.6s15.6,7,15.6,15.6S58.7,65.6,50,65.6z' })
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'gear two' },
					_react2.default.createElement(
						'svg',
						{ id: 'pink', viewBox: '0 0 100 100', fill: '#FB8BB9' },
						_react2.default.createElement('path', { d: 'M97.6,55.7V44.3l-13.6-2.9c-0.8-3.3-2.1-6.4-3.9-9.3l7.6-11.7l-8-8L67.9,20c-2.9-1.7-6-3.1-9.3-3.9L55.7,2.4H44.3l-2.9,13.6      c-3.3,0.8-6.4,2.1-9.3,3.9l-11.7-7.6l-8,8L20,32.1c-1.7,2.9-3.1,6-3.9,9.3L2.4,44.3v11.4l13.6,2.9c0.8,3.3,2.1,6.4,3.9,9.3      l-7.6,11.7l8,8L32.1,80c2.9,1.7,6,3.1,9.3,3.9l2.9,13.6h11.4l2.9-13.6c3.3-0.8,6.4-2.1,9.3-3.9l11.7,7.6l8-8L80,67.9      c1.7-2.9,3.1-6,3.9-9.3L97.6,55.7z M50,65.6c-8.7,0-15.6-7-15.6-15.6s7-15.6,15.6-15.6s15.6,7,15.6,15.6S58.7,65.6,50,65.6z' })
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'gear three' },
					_react2.default.createElement(
						'svg',
						{ id: 'yellow', viewBox: '0 0 100 100', fill: '#FFCD5C' },
						_react2.default.createElement('path', { d: 'M97.6,55.7V44.3l-13.6-2.9c-0.8-3.3-2.1-6.4-3.9-9.3l7.6-11.7l-8-8L67.9,20c-2.9-1.7-6-3.1-9.3-3.9L55.7,2.4H44.3l-2.9,13.6      c-3.3,0.8-6.4,2.1-9.3,3.9l-11.7-7.6l-8,8L20,32.1c-1.7,2.9-3.1,6-3.9,9.3L2.4,44.3v11.4l13.6,2.9c0.8,3.3,2.1,6.4,3.9,9.3      l-7.6,11.7l8,8L32.1,80c2.9,1.7,6,3.1,9.3,3.9l2.9,13.6h11.4l2.9-13.6c3.3-0.8,6.4-2.1,9.3-3.9l11.7,7.6l8-8L80,67.9      c1.7-2.9,3.1-6,3.9-9.3L97.6,55.7z M50,65.6c-8.7,0-15.6-7-15.6-15.6s7-15.6,15.6-15.6s15.6,7,15.6,15.6S58.7,65.6,50,65.6z' })
					)
				),
				_react2.default.createElement('div', { className: 'lil-circle' }),
				_react2.default.createElement(
					'div',
					{ className: 'text' },
					this.props.text
				)
			);
		}
	});

/***/ },
/* 30 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		render: function render() {
			var model = this.getModel(),
			    date = model.get('Date'),
			    displayDate = date ? (0, _moment2.default)(model.get('Date')).format("MMM DD, YYYY") : '';
			return _react2.default.createElement(
				'div',
				{ className: 'footer' },
				_react2.default.createElement(
					'div',
					{ className: 'left' },
					_react2.default.createElement('div', { className: 'vargainc-logo' })
				),
				_react2.default.createElement(
					'div',
					{ className: 'center' },
					_react2.default.createElement(
						'ul',
						{ className: 'no-bullet' },
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'span',
								null,
								'MC#:' + model.get('DisplayName')
							),
							_react2.default.createElement(
								'span',
								null,
								'www.vargainc.com'
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'span',
								null,
								'Created on:' + displayDate
							),
							_react2.default.createElement(
								'span',
								null,
								'PH:949-768-1500'
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'span',
								null,
								'Created for:' + model.get('ContactName')
							),
							_react2.default.createElement(
								'span',
								null,
								'FX:949-768-1501'
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'span',
								null,
								'Created by:' + model.get('CreatorName')
							),
							_react2.default.createElement(
								'span',
								null,
								'&copyright;2010 Varga Media Solutions,Inc.All rights reserved.'
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'right' },
					_react2.default.createElement('div', { className: 'timm-logo' })
				)
			);
		}
	});

/***/ },
/* 31 */
/***/ function(module, exports) {

	/**
	 * A specialized version of `_.map` for arrays without support for iteratee
	 * shorthands.
	 *
	 * @private
	 * @param {Array} [array] The array to iterate over.
	 * @param {Function} iteratee The function invoked per iteration.
	 * @returns {Array} Returns the new mapped array.
	 */
	function arrayMap(array, iteratee) {
	  var index = -1,
	      length = array == null ? 0 : array.length,
	      result = Array(length);

	  while (++index < length) {
	    result[index] = iteratee(array[index], index, array);
	  }
	  return result;
	}

	module.exports = arrayMap;


/***/ },
/* 32 */
/***/ function(module, exports, __webpack_require__) {

	var baseMatches = __webpack_require__(436),
	    baseMatchesProperty = __webpack_require__(437),
	    identity = __webpack_require__(60),
	    isArray = __webpack_require__(12),
	    property = __webpack_require__(528);

	/**
	 * The base implementation of `_.iteratee`.
	 *
	 * @private
	 * @param {*} [value=_.identity] The value to convert to an iteratee.
	 * @returns {Function} Returns the iteratee.
	 */
	function baseIteratee(value) {
	  // Don't store the `typeof` result in a variable to avoid a JIT bug in Safari 9.
	  // See https://bugs.webkit.org/show_bug.cgi?id=156034 for more details.
	  if (typeof value == 'function') {
	    return value;
	  }
	  if (value == null) {
	    return identity;
	  }
	  if (typeof value == 'object') {
	    return isArray(value)
	      ? baseMatchesProperty(value[0], value[1])
	      : baseMatches(value);
	  }
	  return property(value);
	}

	module.exports = baseIteratee;


/***/ },
/* 33 */
/***/ function(module, exports, __webpack_require__) {

	var assignValue = __webpack_require__(77),
	    baseAssignValue = __webpack_require__(109);

	/**
	 * Copies properties of `source` to `object`.
	 *
	 * @private
	 * @param {Object} source The object to copy properties from.
	 * @param {Array} props The property identifiers to copy.
	 * @param {Object} [object={}] The object to copy properties to.
	 * @param {Function} [customizer] The function to customize copied values.
	 * @returns {Object} Returns `object`.
	 */
	function copyObject(source, props, object, customizer) {
	  var isNew = !object;
	  object || (object = {});

	  var index = -1,
	      length = props.length;

	  while (++index < length) {
	    var key = props[index];

	    var newValue = customizer
	      ? customizer(object[key], source[key], key, object, source)
	      : undefined;

	    if (newValue === undefined) {
	      newValue = source[key];
	    }
	    if (isNew) {
	      baseAssignValue(object, key, newValue);
	    } else {
	      assignValue(object, key, newValue);
	    }
	  }
	  return object;
	}

	module.exports = copyObject;


/***/ },
/* 34 */
/***/ function(module, exports, __webpack_require__) {

	var baseIsNative = __webpack_require__(433),
	    getValue = __webpack_require__(475);

	/**
	 * Gets the native function at `key` of `object`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @param {string} key The key of the method to get.
	 * @returns {*} Returns the function if it's native, else `undefined`.
	 */
	function getNative(object, key) {
	  var value = getValue(object, key);
	  return baseIsNative(value) ? value : undefined;
	}

	module.exports = getNative;


/***/ },
/* 35 */
/***/ function(module, exports) {

	/**
	 * Performs a
	 * [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
	 * comparison between two values to determine if they are equivalent.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to compare.
	 * @param {*} other The other value to compare.
	 * @returns {boolean} Returns `true` if the values are equivalent, else `false`.
	 * @example
	 *
	 * var object = { 'a': 1 };
	 * var other = { 'a': 1 };
	 *
	 * _.eq(object, object);
	 * // => true
	 *
	 * _.eq(object, other);
	 * // => false
	 *
	 * _.eq('a', 'a');
	 * // => true
	 *
	 * _.eq('a', Object('a'));
	 * // => false
	 *
	 * _.eq(NaN, NaN);
	 * // => true
	 */
	function eq(value, other) {
	  return value === other || (value !== value && other !== other);
	}

	module.exports = eq;


/***/ },
/* 36 */
/***/ function(module, exports, __webpack_require__) {

	var baseIndexOf = __webpack_require__(80),
	    toInteger = __webpack_require__(127);

	/* Built-in method references for those with the same name as other `lodash` methods. */
	var nativeMax = Math.max;

	/**
	 * Gets the index at which the first occurrence of `value` is found in `array`
	 * using [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
	 * for equality comparisons. If `fromIndex` is negative, it's used as the
	 * offset from the end of `array`.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Array
	 * @param {Array} array The array to inspect.
	 * @param {*} value The value to search for.
	 * @param {number} [fromIndex=0] The index to search from.
	 * @returns {number} Returns the index of the matched value, else `-1`.
	 * @example
	 *
	 * _.indexOf([1, 2, 1, 2], 2);
	 * // => 1
	 *
	 * // Search from the `fromIndex`.
	 * _.indexOf([1, 2, 1, 2], 2, 2);
	 * // => 3
	 */
	function indexOf(array, value, fromIndex) {
	  var length = array == null ? 0 : array.length;
	  if (!length) {
	    return -1;
	  }
	  var index = fromIndex == null ? 0 : toInteger(fromIndex);
	  if (index < 0) {
	    index = nativeMax(length + index, 0);
	  }
	  return baseIndexOf(array, value, index);
	}

	module.exports = indexOf;


/***/ },
/* 37 */
/***/ function(module, exports, __webpack_require__) {

	var baseKeys = __webpack_require__(183),
	    getTag = __webpack_require__(115),
	    isArguments = __webpack_require__(86),
	    isArray = __webpack_require__(12),
	    isArrayLike = __webpack_require__(25),
	    isBuffer = __webpack_require__(87),
	    isPrototype = __webpack_require__(57),
	    isTypedArray = __webpack_require__(122);

	/** `Object#toString` result references. */
	var mapTag = '[object Map]',
	    setTag = '[object Set]';

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * Checks if `value` is an empty object, collection, map, or set.
	 *
	 * Objects are considered empty if they have no own enumerable string keyed
	 * properties.
	 *
	 * Array-like values such as `arguments` objects, arrays, buffers, strings, or
	 * jQuery-like collections are considered empty if they have a `length` of `0`.
	 * Similarly, maps and sets are considered empty if they have a `size` of `0`.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is empty, else `false`.
	 * @example
	 *
	 * _.isEmpty(null);
	 * // => true
	 *
	 * _.isEmpty(true);
	 * // => true
	 *
	 * _.isEmpty(1);
	 * // => true
	 *
	 * _.isEmpty([1, 2, 3]);
	 * // => false
	 *
	 * _.isEmpty({ 'a': 1 });
	 * // => false
	 */
	function isEmpty(value) {
	  if (value == null) {
	    return true;
	  }
	  if (isArrayLike(value) &&
	      (isArray(value) || typeof value == 'string' || typeof value.splice == 'function' ||
	        isBuffer(value) || isTypedArray(value) || isArguments(value))) {
	    return !value.length;
	  }
	  var tag = getTag(value);
	  if (tag == mapTag || tag == setTag) {
	    return !value.size;
	  }
	  if (isPrototype(value)) {
	    return !baseKeys(value).length;
	  }
	  for (var key in value) {
	    if (hasOwnProperty.call(value, key)) {
	      return false;
	    }
	  }
	  return true;
	}

	module.exports = isEmpty;


/***/ },
/* 38 */
/***/ function(module, exports) {

	/**
	 * Checks if `value` is object-like. A value is object-like if it's not `null`
	 * and has a `typeof` result of "object".
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is object-like, else `false`.
	 * @example
	 *
	 * _.isObjectLike({});
	 * // => true
	 *
	 * _.isObjectLike([1, 2, 3]);
	 * // => true
	 *
	 * _.isObjectLike(_.noop);
	 * // => false
	 *
	 * _.isObjectLike(null);
	 * // => false
	 */
	function isObjectLike(value) {
	  return value != null && typeof value == 'object';
	}

	module.exports = isObjectLike;


/***/ },
/* 39 */,
/* 40 */,
/* 41 */,
/* 42 */,
/* 43 */
/***/ function(module, exports, __webpack_require__) {

	var root = __webpack_require__(22);

	/** Built-in value references. */
	var Symbol = root.Symbol;

	module.exports = Symbol;


/***/ },
/* 44 */
/***/ function(module, exports, __webpack_require__) {

	var Symbol = __webpack_require__(43),
	    getRawTag = __webpack_require__(474),
	    objectToString = __webpack_require__(502);

	/** `Object#toString` result references. */
	var nullTag = '[object Null]',
	    undefinedTag = '[object Undefined]';

	/** Built-in value references. */
	var symToStringTag = Symbol ? Symbol.toStringTag : undefined;

	/**
	 * The base implementation of `getTag` without fallbacks for buggy environments.
	 *
	 * @private
	 * @param {*} value The value to query.
	 * @returns {string} Returns the `toStringTag`.
	 */
	function baseGetTag(value) {
	  if (value == null) {
	    return value === undefined ? undefinedTag : nullTag;
	  }
	  value = Object(value);
	  return (symToStringTag && symToStringTag in value)
	    ? getRawTag(value)
	    : objectToString(value);
	}

	module.exports = baseGetTag;


/***/ },
/* 45 */
/***/ function(module, exports, __webpack_require__) {

	var isArray = __webpack_require__(12),
	    isKey = __webpack_require__(117),
	    stringToPath = __webpack_require__(514),
	    toString = __webpack_require__(63);

	/**
	 * Casts `value` to a path array if it's not one.
	 *
	 * @private
	 * @param {*} value The value to inspect.
	 * @param {Object} [object] The object to query keys on.
	 * @returns {Array} Returns the cast property path array.
	 */
	function castPath(value, object) {
	  if (isArray(value)) {
	    return value;
	  }
	  return isKey(value, object) ? [value] : stringToPath(toString(value));
	}

	module.exports = castPath;


/***/ },
/* 46 */
/***/ function(module, exports, __webpack_require__) {

	var isSymbol = __webpack_require__(61);

	/** Used as references for various `Number` constants. */
	var INFINITY = 1 / 0;

	/**
	 * Converts `value` to a string key if it's not a string or symbol.
	 *
	 * @private
	 * @param {*} value The value to inspect.
	 * @returns {string|symbol} Returns the key.
	 */
	function toKey(value) {
	  if (typeof value == 'string' || isSymbol(value)) {
	    return value;
	  }
	  var result = (value + '');
	  return (result == '0' && (1 / value) == -INFINITY) ? '-0' : result;
	}

	module.exports = toKey;


/***/ },
/* 47 */
/***/ function(module, exports, __webpack_require__) {

	var arraySome = __webpack_require__(178),
	    baseIteratee = __webpack_require__(32),
	    baseSome = __webpack_require__(445),
	    isArray = __webpack_require__(12),
	    isIterateeCall = __webpack_require__(116);

	/**
	 * Checks if `predicate` returns truthy for **any** element of `collection`.
	 * Iteration is stopped once `predicate` returns truthy. The predicate is
	 * invoked with three arguments: (value, index|key, collection).
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Collection
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} [predicate=_.identity] The function invoked per iteration.
	 * @param- {Object} [guard] Enables use as an iteratee for methods like `_.map`.
	 * @returns {boolean} Returns `true` if any element passes the predicate check,
	 *  else `false`.
	 * @example
	 *
	 * _.some([null, 0, 'yes', false], Boolean);
	 * // => true
	 *
	 * var users = [
	 *   { 'user': 'barney', 'active': true },
	 *   { 'user': 'fred',   'active': false }
	 * ];
	 *
	 * // The `_.matches` iteratee shorthand.
	 * _.some(users, { 'user': 'barney', 'active': false });
	 * // => false
	 *
	 * // The `_.matchesProperty` iteratee shorthand.
	 * _.some(users, ['active', false]);
	 * // => true
	 *
	 * // The `_.property` iteratee shorthand.
	 * _.some(users, 'active');
	 * // => true
	 */
	function some(collection, predicate, guard) {
	  var func = isArray(collection) ? arraySome : baseSome;
	  if (guard && isIterateeCall(collection, predicate, guard)) {
	    predicate = undefined;
	  }
	  return func(collection, baseIteratee(predicate, 3));
	}

	module.exports = some;


/***/ },
/* 48 */,
/* 49 */,
/* 50 */,
/* 51 */,
/* 52 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _campaign = __webpack_require__(99);

	var _campaign2 = _interopRequireDefault(_campaign);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _backbone2.default.Collection.extend({
	    model: _campaign2.default,
	    urlRoot: 'campaign',
	    fetchForDistribution: function fetchForDistribution(opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/distribution'
	        };
	        options = (0, _extend3.default)(opts, options);

	        return this.fetch(options);
	    },
	    fetchForTask: function fetchForTask(opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/monitor'
	        };
	        options = (0, _extend3.default)(opts, options);

	        return this.fetch(options);
	    },
	    fetchForReport: function fetchForReport(opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/report'
	        };
	        options = (0, _extend3.default)(opts, options);

	        return this.fetch(options);
	    }
	});

/***/ },
/* 53 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

	exports.default = _backbone2.default.Model.extend({
	    urlRoot: 'task',
	    idAttribute: 'Id',
	    defaults: _defineProperty({
	        Id: null,
	        Name: null,
	        Date: null,
	        Status: null,
	        Telephone: null,
	        Email: null,
	        DistributionMapId: null,
	        AuditorId: null,
	        AuditorName: null
	    }, 'DistributionMapId', null),
	    markFinished: function markFinished(opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/finish/',
	            method: 'PUT'
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    reOpen: function reOpen(opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/reopen/',
	            method: 'PUT'
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    importGtu: function importGtu(file, opts) {

	        var model = this,
	            options = {
	            url: '../api/' + model.urlRoot + '/' + model.get('Id') + '/import/',
	            method: 'PUT'
	        };
	        options = (0, _extend3.default)(opts, options);
	        var xhr = new XMLHttpRequest();
	        var fd = new FormData();

	        xhr.open(options.method, options.url, true);
	        xhr.onreadystatechange = function () {
	            if (xhr.readyState == 4 && xhr.status == 200) {
	                options.success && options.success.call(model, JSON.parse(xhr.responseText));
	            }
	        };
	        fd.append('gtuFile', file);
	        xhr.send(fd);
	    },
	    addGtuDots: function addGtuDots(dots, opts) {

	        var model = this;
	        var options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/dots/',
	            method: 'POST',
	            data: _jquery2.default.param({
	                '': dots
	            })
	        };
	        options = (0, _extend3.default)(opts, options);
	        return (this.sync || _backbone2.default.sync).call(this, '', this, options).then(function (result) {
	            if (result && result.success) {
	                return _bluebird2.default.resolve();
	            }
	            return _bluebird2.default.reject();
	        });
	    },
	    removeGtuDots: function removeGtuDots(dots, opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/dots/',
	            method: 'PUT',
	            data: _jquery2.default.param({
	                '': dots
	            })
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, '', this, options).then(function (result) {
	            if (result && result.success) {
	                return _bluebird2.default.resolve();
	            }
	            return _bluebird2.default.reject();
	        });
	    },
	    setStart: function setStart(opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/start/',
	            method: 'PUT',
	            success: function success(result) {
	                if (result && result.success) {
	                    model.set({
	                        Status: result.status
	                    });
	                }
	            }
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    setPause: function setPause(opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/pause/',
	            method: 'PUT',
	            success: function success(result) {
	                if (result && result.success) {
	                    model.set({
	                        Status: result.status
	                    });
	                }
	            }
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    setStop: function setStop(opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/stop/',
	            method: 'PUT',
	            success: function success(result) {
	                if (result && result.success) {
	                    model.set({
	                        Status: result.status
	                    });
	                }
	            }
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    }
	});

/***/ },
/* 54 */
/***/ function(module, exports, __webpack_require__) {

	var baseForOwn = __webpack_require__(425),
	    createBaseEach = __webpack_require__(466);

	/**
	 * The base implementation of `_.forEach` without support for iteratee shorthands.
	 *
	 * @private
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} iteratee The function invoked per iteration.
	 * @returns {Array|Object} Returns `collection`.
	 */
	var baseEach = createBaseEach(baseForOwn);

	module.exports = baseEach;


/***/ },
/* 55 */
/***/ function(module, exports, __webpack_require__) {

	var arrayPush = __webpack_require__(76),
	    isFlattenable = __webpack_require__(485);

	/**
	 * The base implementation of `_.flatten` with support for restricting flattening.
	 *
	 * @private
	 * @param {Array} array The array to flatten.
	 * @param {number} depth The maximum recursion depth.
	 * @param {boolean} [predicate=isFlattenable] The function invoked per iteration.
	 * @param {boolean} [isStrict] Restrict to values that pass `predicate` checks.
	 * @param {Array} [result=[]] The initial result value.
	 * @returns {Array} Returns the new flattened array.
	 */
	function baseFlatten(array, depth, predicate, isStrict, result) {
	  var index = -1,
	      length = array.length;

	  predicate || (predicate = isFlattenable);
	  result || (result = []);

	  while (++index < length) {
	    var value = array[index];
	    if (depth > 0 && predicate(value)) {
	      if (depth > 1) {
	        // Recursively flatten arrays (susceptible to call stack limits).
	        baseFlatten(value, depth - 1, predicate, isStrict, result);
	      } else {
	        arrayPush(result, value);
	      }
	    } else if (!isStrict) {
	      result[result.length] = value;
	    }
	  }
	  return result;
	}

	module.exports = baseFlatten;


/***/ },
/* 56 */
/***/ function(module, exports, __webpack_require__) {

	var identity = __webpack_require__(60),
	    overRest = __webpack_require__(202),
	    setToString = __webpack_require__(203);

	/**
	 * The base implementation of `_.rest` which doesn't validate or coerce arguments.
	 *
	 * @private
	 * @param {Function} func The function to apply a rest parameter to.
	 * @param {number} [start=func.length-1] The start position of the rest parameter.
	 * @returns {Function} Returns the new function.
	 */
	function baseRest(func, start) {
	  return setToString(overRest(func, start, identity), func + '');
	}

	module.exports = baseRest;


/***/ },
/* 57 */
/***/ function(module, exports) {

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/**
	 * Checks if `value` is likely a prototype object.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is a prototype, else `false`.
	 */
	function isPrototype(value) {
	  var Ctor = value && value.constructor,
	      proto = (typeof Ctor == 'function' && Ctor.prototype) || objectProto;

	  return value === proto;
	}

	module.exports = isPrototype;


/***/ },
/* 58 */
/***/ function(module, exports, __webpack_require__) {

	var arrayFilter = __webpack_require__(173),
	    baseFilter = __webpack_require__(423),
	    baseIteratee = __webpack_require__(32),
	    isArray = __webpack_require__(12);

	/**
	 * Iterates over elements of `collection`, returning an array of all elements
	 * `predicate` returns truthy for. The predicate is invoked with three
	 * arguments: (value, index|key, collection).
	 *
	 * **Note:** Unlike `_.remove`, this method returns a new array.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Collection
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} [predicate=_.identity] The function invoked per iteration.
	 * @returns {Array} Returns the new filtered array.
	 * @see _.reject
	 * @example
	 *
	 * var users = [
	 *   { 'user': 'barney', 'age': 36, 'active': true },
	 *   { 'user': 'fred',   'age': 40, 'active': false }
	 * ];
	 *
	 * _.filter(users, function(o) { return !o.active; });
	 * // => objects for ['fred']
	 *
	 * // The `_.matches` iteratee shorthand.
	 * _.filter(users, { 'age': 36, 'active': true });
	 * // => objects for ['barney']
	 *
	 * // The `_.matchesProperty` iteratee shorthand.
	 * _.filter(users, ['active', false]);
	 * // => objects for ['fred']
	 *
	 * // The `_.property` iteratee shorthand.
	 * _.filter(users, 'active');
	 * // => objects for ['barney']
	 */
	function filter(collection, predicate) {
	  var func = isArray(collection) ? arrayFilter : baseFilter;
	  return func(collection, baseIteratee(predicate, 3));
	}

	module.exports = filter;


/***/ },
/* 59 */
/***/ function(module, exports, __webpack_require__) {

	var arrayEach = __webpack_require__(172),
	    baseEach = __webpack_require__(54),
	    castFunction = __webpack_require__(450),
	    isArray = __webpack_require__(12);

	/**
	 * Iterates over elements of `collection` and invokes `iteratee` for each element.
	 * The iteratee is invoked with three arguments: (value, index|key, collection).
	 * Iteratee functions may exit iteration early by explicitly returning `false`.
	 *
	 * **Note:** As with other "Collections" methods, objects with a "length"
	 * property are iterated like arrays. To avoid this behavior use `_.forIn`
	 * or `_.forOwn` for object iteration.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @alias each
	 * @category Collection
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} [iteratee=_.identity] The function invoked per iteration.
	 * @returns {Array|Object} Returns `collection`.
	 * @see _.forEachRight
	 * @example
	 *
	 * _.forEach([1, 2], function(value) {
	 *   console.log(value);
	 * });
	 * // => Logs `1` then `2`.
	 *
	 * _.forEach({ 'a': 1, 'b': 2 }, function(value, key) {
	 *   console.log(key);
	 * });
	 * // => Logs 'a' then 'b' (iteration order is not guaranteed).
	 */
	function forEach(collection, iteratee) {
	  var func = isArray(collection) ? arrayEach : baseEach;
	  return func(collection, castFunction(iteratee));
	}

	module.exports = forEach;


/***/ },
/* 60 */
/***/ function(module, exports) {

	/**
	 * This method returns the first argument it receives.
	 *
	 * @static
	 * @since 0.1.0
	 * @memberOf _
	 * @category Util
	 * @param {*} value Any value.
	 * @returns {*} Returns `value`.
	 * @example
	 *
	 * var object = { 'a': 1 };
	 *
	 * console.log(_.identity(object) === object);
	 * // => true
	 */
	function identity(value) {
	  return value;
	}

	module.exports = identity;


/***/ },
/* 61 */
/***/ function(module, exports, __webpack_require__) {

	var baseGetTag = __webpack_require__(44),
	    isObjectLike = __webpack_require__(38);

	/** `Object#toString` result references. */
	var symbolTag = '[object Symbol]';

	/**
	 * Checks if `value` is classified as a `Symbol` primitive or object.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is a symbol, else `false`.
	 * @example
	 *
	 * _.isSymbol(Symbol.iterator);
	 * // => true
	 *
	 * _.isSymbol('abc');
	 * // => false
	 */
	function isSymbol(value) {
	  return typeof value == 'symbol' ||
	    (isObjectLike(value) && baseGetTag(value) == symbolTag);
	}

	module.exports = isSymbol;


/***/ },
/* 62 */,
/* 63 */
/***/ function(module, exports, __webpack_require__) {

	var baseToString = __webpack_require__(187);

	/**
	 * Converts `value` to a string. An empty string is returned for `null`
	 * and `undefined` values. The sign of `-0` is preserved.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to convert.
	 * @returns {string} Returns the converted string.
	 * @example
	 *
	 * _.toString(null);
	 * // => ''
	 *
	 * _.toString(-0);
	 * // => '-0'
	 *
	 * _.toString([1, 2, 3]);
	 * // => '1,2,3'
	 */
	function toString(value) {
	  return value == null ? '' : baseToString(value);
	}

	module.exports = toString;


/***/ },
/* 64 */,
/* 65 */,
/* 66 */,
/* 67 */,
/* 68 */,
/* 69 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _forEach2 = __webpack_require__(59);

	var _forEach3 = _interopRequireDefault(_forEach2);

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _gtu = __webpack_require__(356);

	var _gtu2 = _interopRequireDefault(_gtu);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _backbone2.default.Collection.extend({
	    model: _gtu2.default,
	    urlRoot: 'gtu',
	    fetchByTask: function fetchByTask(taskId, opts) {
	        var model = this,
	            options = {
	            url: 'task/' + taskId + '/gtu'
	        };
	        options = (0, _extend3.default)(opts, options);

	        return this.fetch(options);
	    },
	    fetchGtuWithStatusByTask: function fetchGtuWithStatusByTask(taskId, opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/task/' + taskId
	        };
	        options = (0, _extend3.default)(opts, options);
	        return this.fetch(options);
	    },
	    fetchGtuLocation: function fetchGtuLocation(taskId, opts) {
	        var collection = this,
	            options = {
	            url: collection.urlRoot + '/task/' + taskId + '/online/',
	            method: 'GET',
	            success: function success(result) {
	                if (result) {
	                    (0, _forEach3.default)(result, function (item) {
	                        var gtu = collection.get(item.Id);
	                        if (gtu) {
	                            var location = null;
	                            if (item.Location && item.Location.lat && item.Location.lng) {
	                                location = {
	                                    lat: parseFloat(item.Location.lat),
	                                    lng: parseFloat(item.Location.lng)
	                                };
	                            }

	                            gtu.set({
	                                IsOnline: item.IsOnline,
	                                Location: location,
	                                OutOfBoundary: item.OutOfBoundary
	                            });
	                        }
	                    });
	                }
	            }
	        };
	        (0, _extend3.default)(options, opts);
	        return (this.sync || _backbone2.default.sync).call(this, '', this, options);
	    }
	});

/***/ },
/* 70 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _concat2 = __webpack_require__(120);

	var _concat3 = _interopRequireDefault(_concat2);

	var _find2 = __webpack_require__(205);

	var _find3 = _interopRequireDefault(_find2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _base = __webpack_require__(24);

	var _base2 = _interopRequireDefault(_base);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _base2.default.extend({
	    urlRoot: 'map',
	    idAttribute: 'key',
	    defaults: {
	        'Id': null,
	        'Name': null,
	        'Total': null,
	        'DisplayName': null,
	        'MapImage': null,
	        'PolygonImage': null,
	        'ImageStatus': 'waiting'
	    },
	    fetchMapImage: function fetchMapImage(mapOption) {
	        var model = this,
	            params = _jquery2.default.extend({
	            mapType: 'HYBRID'
	        }, mapOption, {
	            campaignId: model.get('CampaignId'),
	            submapId: model.get('SubMapId'),
	            dmapId: model.get('DMapId')
	        }),
	            options = {
	            quite: true,
	            url: model.urlRoot + '/dmap/',
	            method: 'POST',
	            processData: true,
	            data: _jquery2.default.param(params),
	            success: function success(result) {
	                var mapImage = null,
	                    polygonImage = null;
	                if (result && result.success) {
	                    mapImage = result.tiles;
	                    polygonImage = result.geometry;
	                }
	                model.set('MapImage', mapImage, {
	                    silent: true
	                });
	                model.set('PolygonImage', polygonImage, {
	                    silent: true
	                });
	            }
	        };
	        if (model.get('TopRight') && model.get('BottomLeft')) {
	            options.data = _jquery2.default.param(_jquery2.default.extend(params, {
	                topRightLat: model.get('TopRight').lat,
	                topRightLng: model.get('TopRight').lng,
	                bottomLeftLat: model.get('BottomLeft').lat,
	                bottomLeftLng: model.get('BottomLeft').lng
	            }));
	        }
	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    fetchBoundary: function fetchBoundary(opts) {
	        var model = this,
	            options = {
	            url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/boundary/',
	            method: 'GET',
	            success: function success(result) {
	                model.set({
	                    'Boundary': result.boundary,
	                    'Color': result.color
	                });
	            }
	        };
	        (0, _extend3.default)(options, opts);
	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    },
	    fetchGtu: function fetchGtu(opts) {
	        var model = this,
	            options = {
	            url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/gtu/',
	            method: 'GET',
	            success: function success(result) {
	                var gtus = [];
	                for (var i = 0; i < result.pointsColors.length; i++) {
	                    if (result.points[i] && result.points[i].length > 0) {
	                        gtus.push({
	                            gtuId: result.points[i][0].Id,
	                            color: result.pointsColors[i],
	                            points: result.points[i]
	                        });
	                    }
	                }
	                model.set({
	                    'Gtu': gtus,
	                    lastUpdateTime: result.lastUpdateTime
	                });
	            }
	        };
	        (0, _extend3.default)(options, opts);
	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    },
	    fetchGtuForEdit: function fetchGtuForEdit(opts) {
	        var model = this,
	            options = {
	            url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/gtu/foredit',
	            method: 'GET',
	            success: function success(result) {
	                var gtus = [];
	                for (var i = 0; i < result.pointsColors.length; i++) {
	                    if (result.points[i] && result.points[i].length > 0) {
	                        gtus.push({
	                            gtuId: result.points[i][0].Id,
	                            color: result.pointsColors[i],
	                            points: result.points[i]
	                        });
	                    }
	                }
	                model.set({
	                    'Gtu': gtus,
	                    lastUpdateTime: result.lastUpdateTime
	                });
	            }
	        };
	        (0, _extend3.default)(options, opts);
	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    },
	    fetchAllGtu: function fetchAllGtu(opts) {
	        var model = this,
	            options = {
	            url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/gtu/all/',
	            method: 'GET',
	            success: function success(result) {
	                var gtus = [];
	                for (var i = 0; i < result.pointsColors.length; i++) {
	                    if (result.points[i] && result.points[i].length > 0) {
	                        gtus.push({
	                            gtuId: result.points[i][0].Id,
	                            color: result.pointsColors[i],
	                            points: result.points[i]
	                        });
	                    }
	                }
	                model.set({
	                    'Gtu': gtus,
	                    lastUpdateTime: result.lastUpdateTime
	                });
	            }
	        };
	        (0, _extend3.default)(options, opts);
	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    },
	    updateGtuAfterTime: function updateGtuAfterTime(time, opts) {
	        var model = this,
	            lastTime = time ? time.utc().format('YYYYMMDDTHHmmss[Z]') : model.get('lastUpdateTime'),
	            options = {
	            url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/gtu/all/' + lastTime,
	            method: 'GET',
	            success: function success(result) {
	                var gtus = model.get('Gtu') || [];
	                for (var i = 0; i < result.pointsColors.length; i++) {
	                    var colorItem = (0, _find3.default)(gtus, {
	                        color: result.pointsColors[i]
	                    });
	                    if (!colorItem) {
	                        gtus.push({
	                            color: result.pointsColors[i],
	                            points: result.points[i]
	                        });
	                    } else {
	                        colorItem.points = (0, _concat3.default)(colorItem.points, result.points[i]);
	                    }
	                }
	                model.set({
	                    lastUpdateTime: result.lastUpdateTime
	                });
	            }
	        };
	        (0, _extend3.default)(options, opts);
	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    }
	});

/***/ },
/* 71 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _backbone2.default.Model.extend({
	    urlRoot: 'user',
	    idAttribute: 'Id',
	    defaults: {
	        Id: null,
	        UserCode: null,
	        UserName: null,
	        FullName: null,
	        Emai: null,
	        Token: null
	    },
	    fetchCurrentUser: function fetchCurrentUser(opts) {
	        var model = this,
	            url = model.urlRoot + '/info',
	            options = {
	            url: url,
	            type: 'GET'
	        };
	        (0, _extend3.default)(options, opts);

	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options).then(function (result) {
	            if (result && result.success) {
	                model.set(result.data);
	                return _bluebird2.default.resolve();
	            }
	            return _bluebird2.default.reject();
	        });
	    },
	    addEmployee: function addEmployee(file, opts) {
	        var model = this,
	            options = {
	            url: '../api/' + model.urlRoot + '/employee/',
	            method: 'POST'
	        };
	        options = (0, _extend3.default)(opts, options);
	        return new _bluebird2.default(function (resolve, reject) {
	            var xhr = new XMLHttpRequest();
	            var fd = new FormData();

	            xhr.open(options.method, options.url, true);
	            xhr.onreadystatechange = function () {
	                if (xhr.readyState == 4) {
	                    if (xhr.status == 200) {
	                        var result = JSON.parse(xhr.responseText);
	                        model.set(result);
	                        options.success && options.success.call(model, result);
	                        return resolve();
	                    }
	                    return reject(new Error('xhr states not OK'));
	                }
	            };
	            fd.append('Picture', file);
	            fd.append('CompanyId', model.get('CompanyId'));
	            fd.append('Role', model.get('Role'));
	            fd.append('FullName', model.get('FullName'));
	            fd.append('CellPhone', model.get('CellPhone'));
	            fd.append('DateOfBirth', model.get('DateOfBirth'));
	            fd.append('Notes', model.get('Notes'));
	            xhr.send(fd);
	        });
	    },
	    logout: function logout(opts) {
	        var model = this,
	            url = model.urlRoot + '/logout',
	            options = {
	            url: url,
	            type: 'POST'
	        };
	        (0, _extend3.default)(options, opts);

	        return (this.sync || _backbone2.default.sync).call(this, null, this, options);
	    }
	});

/***/ },
/* 72 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _has2 = __webpack_require__(207);

	var _has3 = _interopRequireDefault(_has2);

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _fastMarker = __webpack_require__(98);

	var _fastMarker2 = _interopRequireDefault(_fastMarker);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	var googleMap = window.GoogleMap,
	    googleItems = window.GoogleItems || [];

	exports.default = {
		getGoogleMap: function getGoogleMap() {
			return googleMap;
		},
		getGoogleItems: function getGoogleItems() {
			return googleItems;
		},
		showMap: function showMap() {
			(0, _jquery2.default)('#google-map-wrapper').css({
				'visibility': 'visible'
			});
			google.maps.event.trigger(googleMap, 'resize');
		},
		hideMap: function hideMap() {
			(0, _jquery2.default)('#google-map-wrapper').css({
				'visibility': 'hidden'
			});
			google.maps.event.trigger(googleMap, 'resize');
		},
		setMapHeight: function setMapHeight(height) {
			(0, _jquery2.default)('#google-map-wrapper').height(height);
			google.maps.event.trigger(googleMap, 'resize');
		},
		componentWillUnmount: function componentWillUnmount() {
			console.log('map base clearMap');
			try {
				(0, _each3.default)(googleItems, function (item) {
					item && item.setMap && item.setMap(null);
				});
				google.maps.event.clearInstanceListeners(googleMap);
			} catch (ex) {
				console.log('google map clear error', ex);
			}
			(0, _jquery2.default)('#google-map-wrapper').css({
				'visibility': 'hidden'
			});
		},
		componentDidMount: function componentDidMount() {
			console.log('init google map');
			var mapType = this.state.mapType && google.maps.MapTypeId[this.state.mapType] ? google.maps.MapTypeId[this.state.mapType] : google.maps.MapTypeId.ROADMAP,
			    disableDefaultUI = (0, _has3.default)(this.state, 'disableDefaultUI') ? this.state.disableDefaultUI : false,
			    scrollwheel = (0, _has3.default)(this.state, 'scrollwheel') ? this.state.scrollwheel : true,
			    disableDoubleClickZoom = (0, _has3.default)(this.state, 'disableDoubleClickZoom') ? this.state.disableDoubleClickZoom : false;
			if (this.state.mapStyle) {
				(0, _jquery2.default)('#google-map-wrapper').css(this.state.mapStyle);
			} else {
				(0, _jquery2.default)('#google-map-wrapper').css({
					'visibility': 'visible',
					'position': '',
					'width': '',
					'height': '',
					'left': '',
					'top': '',
					'right': '',
					'bottom': ''
				});
			}

			if (!googleMap) {
				googleMap = new google.maps.Map((0, _jquery2.default)('#google-map')[0], {
					center: new google.maps.LatLng(40.744556, -73.987378),
					zoom: 18,
					disableDefaultUI: disableDefaultUI,
					streetViewControl: false,
					zoomControlOptions: {
						position: google.maps.ControlPosition.RIGHT_TOP
					},
					scrollwheel: scrollwheel,
					disableDoubleClickZoom: disableDoubleClickZoom,
					mapTypeId: mapType
				});
			} else {
				googleMap.setMapTypeId(mapType);
				googleMap.setOptions({
					disableDefaultUI: disableDefaultUI,
					streetViewControl: false,
					scrollwheel: scrollwheel,
					disableDoubleClickZoom: disableDoubleClickZoom,
					zoomControlOptions: {
						position: google.maps.ControlPosition.RIGHT_TOP
					}
				});
			}
			google.maps.event.trigger(googleMap, 'resize');
		}
	};

/***/ },
/* 73 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _mapBase = __webpack_require__(72);

	var _mapBase2 = _interopRequireDefault(_mapBase);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	var googleItems = [],
	    rectangle,
	    drawingManager;
	exports.default = _react2.default.createClass({
		displayName: 'mapZoom',

		mixins: [_base2.default, _mapBase2.default],
		getInitialState: function getInitialState() {
			return {
				disableDefaultUI: true,
				activeButton: 'EnterMapDraw',
				activeMap: 'ROADMAP',
				mapStyle: {
					'visibility': 'hidden',
					'position': 'fixed',
					'width': '100%',
					'height': '100%',
					'left': '0',
					'top': '0',
					'right': '0',
					'bottom': '0'
				}
			};
		},
		getDefaultProps: function getDefaultProps() {
			return {
				boundary: [],
				color: '#000',
				sourceKey: null,
				mapType: 'ROADMAP'
			};
		},
		componentDidMount: function componentDidMount() {
			(0, _jquery2.default)(document).one('open.zf.reveal', _jquery2.default.proxy(this.initGoogleMap, this));
			(0, _jquery2.default)(document).one('closed.zf.reveal', _jquery2.default.proxy(this.clearMap, this));
		},
		clearMap: function clearMap() {
			console.log('mapZoom clearMap');
			var googleMap = this.getGoogleMap();
			try {
				(0, _each3.default)(googleItems, function (item) {
					item && item.setMap && item.setMap(null);
				});
				rectangle && rectangle.setMap && rectangle.setMap(null);
				drawingManager && drawingManager.setMap && drawingManager.setMap(null);
				google.maps.event.clearInstanceListeners(googleMap);
				rectangle = null;
				drawingManager = null;
			} catch (ex) {
				console.log('mapZoom componentWillUnmount error', ex);
			}
			this.hideMap();
		},
		initGoogleMap: function initGoogleMap() {
			console.log('init google map');
			var googleMap = this.getGoogleMap();
			this.showMap();

			var boundary = this.props.boundary,
			    fillColor = this.props.color,
			    mapBounds = new google.maps.LatLngBounds();
			polygon = new google.maps.Polygon({
				paths: boundary,
				strokeColor: '#000',
				strokeOpacity: 1,
				strokeWeight: 6,
				fillColor: fillColor,
				fillOpacity: 0.1,
				map: googleMap
			});
			googleItems.push(polygon);
			(0, _each3.default)(boundary, function (i) {
				var point = new google.maps.LatLng(i.lat, i.lng);
				mapBounds.extend(point);
			});
			googleMap.fitBounds(mapBounds);

			drawingManager = new google.maps.drawing.DrawingManager({
				drawingMode: google.maps.drawing.OverlayType.MARKER,
				drawingControl: false,
				drawingControlOptions: {
					position: google.maps.ControlPosition.TOP_CENTER,
					drawingModes: [google.maps.drawing.OverlayType.RECTANGLE],
					rectangleOptions: {
						strokeColor: '#FF0000',
						strokeOpacity: 0.8,
						strokeWeight: 2,
						fillColor: '#FF0000',
						fillOpacity: 0.6
					}
				}
			});
			drawingManager.setMap(googleMap);
			drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
			google.maps.event.addListener(drawingManager, 'rectanglecomplete', function (rect) {
				rectangle && rectangle.setMap(null);
				rectangle = new google.maps.Rectangle({
					strokeColor: '#FF0000',
					strokeOpacity: 0.8,
					strokeWeight: 1,
					fillColor: '#FF0000',
					fillOpacity: 0.35,
					map: googleMap,
					bounds: rect.getBounds()
				});
				rect.setMap(null);
			});
			googleItems.push(drawingManager);

			this.publish('hideLoading');
		},
		onReset: function onReset() {
			var googleMap = this.getGoogleMap(),
			    boundary = this.props.boundary,
			    fillColor = this.props.color,
			    mapBounds = new google.maps.LatLngBounds();
			polygon = new google.maps.Polygon({
				paths: boundary,
				strokeColor: '#000',
				strokeOpacity: 1,
				strokeWeight: 6,
				fillColor: fillColor,
				fillOpacity: 0.1,
				map: googleMap
			});
			googleItems.push(polygon);
			(0, _each3.default)(boundary, function (i) {
				var point = new google.maps.LatLng(i.lat, i.lng);
				mapBounds.extend(point);
			});
			console.log('fitBounds', mapBounds.getCenter().lat(), mapBounds.getCenter().lng(), googleMap.getZoom());
			googleMap.fitBounds(mapBounds);
		},
		onZoomIn: function onZoomIn() {
			var googleMap = this.getGoogleMap();
			googleMap.setZoom(googleMap.getZoom() + 1);
		},
		onZoomOut: function onZoomOut() {
			var googleMap = this.getGoogleMap();
			googleMap.setZoom(googleMap.getZoom() - 1);
		},
		onExistMapDraw: function onExistMapDraw() {
			this.setState({
				'activeButton': 'ExistMapDraw'
			});
			drawingManager.setDrawingMode(null);
		},
		onEnterMapDraw: function onEnterMapDraw() {
			this.setState({
				'activeButton': 'EnterMapDraw'
			});
			drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
		},
		onSwitchMapType: function onSwitchMapType(mapTypeName) {
			var googleMap = this.getGoogleMap();
			if (googleMap && google.maps.MapTypeId && google.maps.MapTypeId[mapTypeName]) {
				googleMap.setMapTypeId(google.maps.MapTypeId[mapTypeName]);
			}
			this.setState({
				'activeMap': mapTypeName
			});
		},
		onFinish: function onFinish() {
			if (!rectangle) {
				return;
			}
			var bounds = rectangle.getBounds();
			this.publish('print.mapzoom@' + this.props.sourceKey, bounds, this.state.activeMap);
		},
		onClose: function onClose() {
			this.publish("showDialog");
		},
		render: function render() {
			return _react2.default.createElement(
				'div',
				{ className: 'google_map_container' },
				_react2.default.createElement(
					'button',
					{ onClick: this.onClose, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					_react2.default.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'\xD7'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'pop-tool-bar button-group text-center' },
					_react2.default.createElement(
						'button',
						{ className: this.state.activeButton == 'EnterMapDraw' ? 'button active' : 'button', onClick: this.onEnterMapDraw },
						_react2.default.createElement('i', { className: 'fa fa-crop' })
					),
					_react2.default.createElement(
						'button',
						{ className: this.state.activeButton == 'ExistMapDraw' ? 'button active' : 'button', onClick: this.onExistMapDraw },
						_react2.default.createElement('i', { className: 'fa fa-arrows' })
					),
					_react2.default.createElement(
						'button',
						{ className: this.state.activeMap == 'ROADMAP' ? 'button active' : 'button', onClick: this.onSwitchMapType.bind(null, 'ROADMAP') },
						_react2.default.createElement('i', { className: 'fa fa-map-o' })
					),
					_react2.default.createElement(
						'button',
						{ className: this.state.activeMap == 'HYBRID' ? 'button active' : 'button', onClick: this.onSwitchMapType.bind(null, 'HYBRID') },
						_react2.default.createElement('i', { className: 'fa fa-map' })
					),
					_react2.default.createElement(
						'button',
						{ className: 'button', onClick: this.onZoomIn },
						_react2.default.createElement('i', { className: 'fa fa-search-plus' })
					),
					_react2.default.createElement(
						'button',
						{ className: 'button', onClick: this.onZoomOut },
						_react2.default.createElement('i', { className: 'fa fa-search-minus' })
					),
					_react2.default.createElement(
						'button',
						{ className: 'button', onClick: this.onReset },
						_react2.default.createElement('i', { className: 'fa fa-refresh' })
					),
					_react2.default.createElement(
						'button',
						{ className: 'button', onClick: this.onFinish },
						_react2.default.createElement('i', { className: 'fa fa-image' })
					)
				)
			);
		}
	});

/***/ },
/* 74 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _user = __webpack_require__(97);

	var _user2 = _interopRequireDefault(_user);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

	var adminCollection = new _user2.default();

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getDefaultProps: function getDefaultProps() {
			return {
				collection: adminCollection,
				group: null,
				onSelect: null
			};
		},
		getInitialState: function getInitialState() {
			return {
				selected: null
			};
		},
		componentWillMount: function componentWillMount() {
			if (this.props.group) {
				this.getCollection().fetchInGroup(this.props.group);
			}
		},
		onSelected: function onSelected(userId) {
			var user = this.getCollection().get(userId);
			this.props.onSelect && this.props.onSelect(user);
			this.setState({
				selected: user
			});
		},
		onDbSelected: function onDbSelected(userId) {
			var user = this.getCollection().get(userId);
			this.props.onDbSelect && this.props.onDbSelect(user);
			this.setState({
				selected: user
			});
		},
		render: function render() {
			var list = this.getCollection(),
			    self = this;

			list = list ? list : [];
			return _react2.default.createElement(
				'ul',
				{ className: 'vertical menu list-group' },
				list.map(function (model) {
					//var active = this.state && this.state.selectedUserId && this.state.selectedUserId == model.get('Id') ? "active" : "";
					var id = model.get('Id'),
					    selected = self.state.selected && self.state.selected == model,
					    activeClass = selected ? 'list-group-item active' : 'list-group-item';

					return _react2.default.createElement(
						'li',
						_defineProperty({ className: activeClass, key: id, onClick: self.onSelected.bind(self, id)
						}, 'onClick', self.onDbSelected.bind(self, id)),
						model.get('UserName')
					);
				})
			);
		}
	});

/***/ },
/* 75 */
/***/ function(module, exports, __webpack_require__) {

	var listCacheClear = __webpack_require__(488),
	    listCacheDelete = __webpack_require__(489),
	    listCacheGet = __webpack_require__(490),
	    listCacheHas = __webpack_require__(491),
	    listCacheSet = __webpack_require__(492);

	/**
	 * Creates an list cache object.
	 *
	 * @private
	 * @constructor
	 * @param {Array} [entries] The key-value pairs to cache.
	 */
	function ListCache(entries) {
	  var index = -1,
	      length = entries == null ? 0 : entries.length;

	  this.clear();
	  while (++index < length) {
	    var entry = entries[index];
	    this.set(entry[0], entry[1]);
	  }
	}

	// Add methods to `ListCache`.
	ListCache.prototype.clear = listCacheClear;
	ListCache.prototype['delete'] = listCacheDelete;
	ListCache.prototype.get = listCacheGet;
	ListCache.prototype.has = listCacheHas;
	ListCache.prototype.set = listCacheSet;

	module.exports = ListCache;


/***/ },
/* 76 */
/***/ function(module, exports) {

	/**
	 * Appends the elements of `values` to `array`.
	 *
	 * @private
	 * @param {Array} array The array to modify.
	 * @param {Array} values The values to append.
	 * @returns {Array} Returns `array`.
	 */
	function arrayPush(array, values) {
	  var index = -1,
	      length = values.length,
	      offset = array.length;

	  while (++index < length) {
	    array[offset + index] = values[index];
	  }
	  return array;
	}

	module.exports = arrayPush;


/***/ },
/* 77 */
/***/ function(module, exports, __webpack_require__) {

	var baseAssignValue = __webpack_require__(109),
	    eq = __webpack_require__(35);

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * Assigns `value` to `key` of `object` if the existing value is not equivalent
	 * using [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
	 * for equality comparisons.
	 *
	 * @private
	 * @param {Object} object The object to modify.
	 * @param {string} key The key of the property to assign.
	 * @param {*} value The value to assign.
	 */
	function assignValue(object, key, value) {
	  var objValue = object[key];
	  if (!(hasOwnProperty.call(object, key) && eq(objValue, value)) ||
	      (value === undefined && !(key in object))) {
	    baseAssignValue(object, key, value);
	  }
	}

	module.exports = assignValue;


/***/ },
/* 78 */
/***/ function(module, exports, __webpack_require__) {

	var eq = __webpack_require__(35);

	/**
	 * Gets the index at which the `key` is found in `array` of key-value pairs.
	 *
	 * @private
	 * @param {Array} array The array to inspect.
	 * @param {*} key The key to search for.
	 * @returns {number} Returns the index of the matched value, else `-1`.
	 */
	function assocIndexOf(array, key) {
	  var length = array.length;
	  while (length--) {
	    if (eq(array[length][0], key)) {
	      return length;
	    }
	  }
	  return -1;
	}

	module.exports = assocIndexOf;


/***/ },
/* 79 */
/***/ function(module, exports, __webpack_require__) {

	var castPath = __webpack_require__(45),
	    toKey = __webpack_require__(46);

	/**
	 * The base implementation of `_.get` without support for default values.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @param {Array|string} path The path of the property to get.
	 * @returns {*} Returns the resolved value.
	 */
	function baseGet(object, path) {
	  path = castPath(path, object);

	  var index = 0,
	      length = path.length;

	  while (object != null && index < length) {
	    object = object[toKey(path[index++])];
	  }
	  return (index && index == length) ? object : undefined;
	}

	module.exports = baseGet;


/***/ },
/* 80 */
/***/ function(module, exports, __webpack_require__) {

	var baseFindIndex = __webpack_require__(180),
	    baseIsNaN = __webpack_require__(432),
	    strictIndexOf = __webpack_require__(512);

	/**
	 * The base implementation of `_.indexOf` without `fromIndex` bounds checks.
	 *
	 * @private
	 * @param {Array} array The array to inspect.
	 * @param {*} value The value to search for.
	 * @param {number} fromIndex The index to search from.
	 * @returns {number} Returns the index of the matched value, else `-1`.
	 */
	function baseIndexOf(array, value, fromIndex) {
	  return value === value
	    ? strictIndexOf(array, value, fromIndex)
	    : baseFindIndex(array, baseIsNaN, fromIndex);
	}

	module.exports = baseIndexOf;


/***/ },
/* 81 */
/***/ function(module, exports) {

	/**
	 * The base implementation of `_.unary` without support for storing metadata.
	 *
	 * @private
	 * @param {Function} func The function to cap arguments for.
	 * @returns {Function} Returns the new capped function.
	 */
	function baseUnary(func) {
	  return function(value) {
	    return func(value);
	  };
	}

	module.exports = baseUnary;


/***/ },
/* 82 */
/***/ function(module, exports, __webpack_require__) {

	var isKeyable = __webpack_require__(486);

	/**
	 * Gets the data for `map`.
	 *
	 * @private
	 * @param {Object} map The map to query.
	 * @param {string} key The reference key.
	 * @returns {*} Returns the map data.
	 */
	function getMapData(map, key) {
	  var data = map.__data__;
	  return isKeyable(key)
	    ? data[typeof key == 'string' ? 'string' : 'hash']
	    : data.map;
	}

	module.exports = getMapData;


/***/ },
/* 83 */
/***/ function(module, exports) {

	/** Used as references for various `Number` constants. */
	var MAX_SAFE_INTEGER = 9007199254740991;

	/** Used to detect unsigned integer values. */
	var reIsUint = /^(?:0|[1-9]\d*)$/;

	/**
	 * Checks if `value` is a valid array-like index.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @param {number} [length=MAX_SAFE_INTEGER] The upper bounds of a valid index.
	 * @returns {boolean} Returns `true` if `value` is a valid index, else `false`.
	 */
	function isIndex(value, length) {
	  length = length == null ? MAX_SAFE_INTEGER : length;
	  return !!length &&
	    (typeof value == 'number' || reIsUint.test(value)) &&
	    (value > -1 && value % 1 == 0 && value < length);
	}

	module.exports = isIndex;


/***/ },
/* 84 */
/***/ function(module, exports, __webpack_require__) {

	var getNative = __webpack_require__(34);

	/* Built-in method references that are verified to be native. */
	var nativeCreate = getNative(Object, 'create');

	module.exports = nativeCreate;


/***/ },
/* 85 */
/***/ function(module, exports) {

	/**
	 * Converts `set` to an array of its values.
	 *
	 * @private
	 * @param {Object} set The set to convert.
	 * @returns {Array} Returns the values.
	 */
	function setToArray(set) {
	  var index = -1,
	      result = Array(set.size);

	  set.forEach(function(value) {
	    result[++index] = value;
	  });
	  return result;
	}

	module.exports = setToArray;


/***/ },
/* 86 */
/***/ function(module, exports, __webpack_require__) {

	var baseIsArguments = __webpack_require__(429),
	    isObjectLike = __webpack_require__(38);

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/** Built-in value references. */
	var propertyIsEnumerable = objectProto.propertyIsEnumerable;

	/**
	 * Checks if `value` is likely an `arguments` object.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is an `arguments` object,
	 *  else `false`.
	 * @example
	 *
	 * _.isArguments(function() { return arguments; }());
	 * // => true
	 *
	 * _.isArguments([1, 2, 3]);
	 * // => false
	 */
	var isArguments = baseIsArguments(function() { return arguments; }()) ? baseIsArguments : function(value) {
	  return isObjectLike(value) && hasOwnProperty.call(value, 'callee') &&
	    !propertyIsEnumerable.call(value, 'callee');
	};

	module.exports = isArguments;


/***/ },
/* 87 */
/***/ function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(module) {var root = __webpack_require__(22),
	    stubFalse = __webpack_require__(532);

	/** Detect free variable `exports`. */
	var freeExports = typeof exports == 'object' && exports && !exports.nodeType && exports;

	/** Detect free variable `module`. */
	var freeModule = freeExports && typeof module == 'object' && module && !module.nodeType && module;

	/** Detect the popular CommonJS extension `module.exports`. */
	var moduleExports = freeModule && freeModule.exports === freeExports;

	/** Built-in value references. */
	var Buffer = moduleExports ? root.Buffer : undefined;

	/* Built-in method references for those with the same name as other `lodash` methods. */
	var nativeIsBuffer = Buffer ? Buffer.isBuffer : undefined;

	/**
	 * Checks if `value` is a buffer.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.3.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is a buffer, else `false`.
	 * @example
	 *
	 * _.isBuffer(new Buffer(2));
	 * // => true
	 *
	 * _.isBuffer(new Uint8Array(2));
	 * // => false
	 */
	var isBuffer = nativeIsBuffer || stubFalse;

	module.exports = isBuffer;

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(68)(module)))

/***/ },
/* 88 */
/***/ function(module, exports, __webpack_require__) {

	var baseGetTag = __webpack_require__(44),
	    isObject = __webpack_require__(20);

	/** `Object#toString` result references. */
	var asyncTag = '[object AsyncFunction]',
	    funcTag = '[object Function]',
	    genTag = '[object GeneratorFunction]',
	    proxyTag = '[object Proxy]';

	/**
	 * Checks if `value` is classified as a `Function` object.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is a function, else `false`.
	 * @example
	 *
	 * _.isFunction(_);
	 * // => true
	 *
	 * _.isFunction(/abc/);
	 * // => false
	 */
	function isFunction(value) {
	  if (!isObject(value)) {
	    return false;
	  }
	  // The use of `Object#toString` avoids issues with the `typeof` operator
	  // in Safari 9 which returns 'object' for typed arrays and other constructors.
	  var tag = baseGetTag(value);
	  return tag == funcTag || tag == genTag || tag == asyncTag || tag == proxyTag;
	}

	module.exports = isFunction;


/***/ },
/* 89 */,
/* 90 */,
/* 91 */,
/* 92 */,
/* 93 */,
/* 94 */,
/* 95 */,
/* 96 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _forEach2 = __webpack_require__(59);

	var _forEach3 = _interopRequireDefault(_forEach2);

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	var _base = __webpack_require__(24);

	var _base2 = _interopRequireDefault(_base);

	var _footer = __webpack_require__(361);

	var _footer2 = _interopRequireDefault(_footer);

	var _cover = __webpack_require__(358);

	var _cover2 = _interopRequireDefault(_cover);

	var _campaign = __webpack_require__(357);

	var _campaign2 = _interopRequireDefault(_campaign);

	var _submap = __webpack_require__(362);

	var _submap2 = _interopRequireDefault(_submap);

	var _croute = __webpack_require__(359);

	var _croute2 = _interopRequireDefault(_croute);

	var _dmap = __webpack_require__(70);

	var _dmap2 = _interopRequireDefault(_dmap);

	var _distribution = __webpack_require__(360);

	var _distribution2 = _interopRequireDefault(_distribution);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	var SubmapCollection = _backbone2.default.Collection.extend({
	    model: _submap2.default
	});
	var DmapCollection = _backbone2.default.Collection.extend({
	    model: _dmap2.default
	});

	exports.default = _backbone2.default.Collection.extend({
	    model: _base2.default,
	    urlRoot: 'print',
	    dmaps: [],
	    campaignId: null,
	    fetchForCampaignMap: function fetchForCampaignMap(campaignId, opts) {
	        var collection = this,
	            options = {
	            url: collection.urlRoot + '/campaign/' + campaignId,
	            success: function success(result) {
	                var displayName = result.DisplayName,
	                    footerModel = new _footer2.default({
	                    DisplayName: result.DisplayName,
	                    Date: result.Date,
	                    ContactName: result.ContactName,
	                    CreatorName: result.CreatorName
	                });

	                collection.campaignId = campaignId;
	                collection.submaps = [];
	                collection.dmaps = [];

	                collection.add(new _cover2.default({
	                    type: 'Cover',
	                    key: 'cover',
	                    ClientName: result.ClientName,
	                    ContactName: result.ContactName,
	                    DisplayName: result.DisplayName,
	                    CreatorName: result.CreatorName,
	                    Date: result.Date,
	                    Logo: result.Logo,
	                    Footer: footerModel
	                }));

	                collection.add(new _campaign2.default({
	                    type: 'Campaign',
	                    key: 'campaign',
	                    CampaignId: campaignId,
	                    ClientName: result.ClientName,
	                    ContactName: result.ContactName,
	                    DisplayName: result.DisplayName,
	                    CreatorName: result.CreatorName,
	                    TotalHouseHold: result.TotalHouseHold,
	                    TargetHouseHold: result.TargetHouseHold,
	                    Penetration: result.Penetration,
	                    Date: result.Date,
	                    Logo: result.Logo,
	                    Footer: footerModel
	                }));
	                collection.add(new _campaign2.default({
	                    type: 'CampaignSummary',
	                    key: 'campaign-summary',
	                    CampaignId: campaignId,
	                    SubMaps: collection.submaps,
	                    Footer: footerModel
	                }));

	                (0, _forEach3.default)(result.SubMaps, function (submap) {
	                    collection.submaps.push({
	                        Name: submap.Name,
	                        OrderId: submap.OrderId,
	                        TotalHouseHold: submap.TotalHouseHold,
	                        TargetHouseHold: submap.TargetHouseHold,
	                        Penetration: submap.Penetration
	                    });
	                    collection.add(new _submap2.default({
	                        type: 'SubMap',
	                        key: campaignId + '-' + submap.Id,
	                        CampaignId: campaignId,
	                        SubMapId: submap.Id,
	                        Name: submap.Name,
	                        OrderId: submap.OrderId,
	                        TotalHouseHold: submap.TotalHouseHold,
	                        TargetHouseHold: submap.TargetHouseHold,
	                        Penetration: submap.Penetration,
	                        Footer: footerModel
	                    }));
	                    collection.add(new _croute2.default({
	                        type: 'SubMapDetail',
	                        key: 'detail-' + campaignId + '-' + submap.Id,
	                        CampaignId: campaignId,
	                        SubMapId: submap.Id,
	                        Name: submap.Name,
	                        OrderId: submap.OrderId,
	                        Footer: footerModel
	                    }));
	                });
	            }
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    },
	    fetchForDistributionMap: function fetchForDistributionMap(campaignId, opts) {
	        var collection = this,
	            options = {
	            url: collection.urlRoot + '/campaign/' + campaignId,
	            success: function success(result) {
	                collection.campaignId = campaignId;
	                collection.dmaps = [];
	                var displayName = result.DisplayName;
	                (0, _forEach3.default)(result.SubMaps, function (submap) {
	                    (0, _forEach3.default)(submap.DMaps, function (dmap) {
	                        collection.dmaps.push({
	                            Selected: false,
	                            Id: dmap.Id,
	                            Name: dmap.Name
	                        });
	                        collection.add(new _distribution2.default({
	                            key: campaignId + '-' + submap.Id + '-' + dmap.Id,
	                            CampaignId: campaignId,
	                            SubMapId: submap.Id,
	                            DMapId: dmap.Id,
	                            Name: dmap.Name,
	                            Total: dmap.Total,
	                            DisplayName: displayName,
	                            ImageStatus: 'waiting'
	                        }));
	                    });
	                });
	            }
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    },
	    fetchForReportMap: function fetchForReportMap(campaignId, opts) {
	        var collection = this,
	            options = {
	            url: collection.urlRoot + '/campaign/' + campaignId,
	            success: function success(result) {
	                var displayName = result.DisplayName,
	                    footerModel = new _footer2.default({
	                    DisplayName: result.DisplayName,
	                    Date: result.Date,
	                    ContactName: result.ContactName,
	                    CreatorName: result.CreatorName
	                });

	                collection.campaignId = campaignId;
	                collection.submaps = [];
	                collection.dmaps = [];

	                collection.add(new _cover2.default({
	                    type: 'Cover',
	                    key: 'cover',
	                    ClientName: result.ClientName,
	                    ContactName: result.ContactName,
	                    DisplayName: result.DisplayName,
	                    CreatorName: result.CreatorName,
	                    Date: result.Date,
	                    Logo: result.Logo,
	                    Footer: footerModel
	                }));

	                collection.add(new _campaign2.default({
	                    type: 'Campaign',
	                    key: 'campaign',
	                    CampaignId: campaignId,
	                    ClientName: result.ClientName,
	                    ContactName: result.ContactName,
	                    DisplayName: result.DisplayName,
	                    CreatorName: result.CreatorName,
	                    TotalHouseHold: result.TotalHouseHold,
	                    TargetHouseHold: result.TargetHouseHold,
	                    Penetration: result.Penetration,
	                    Date: result.Date,
	                    Logo: result.Logo,
	                    Footer: footerModel
	                }));
	                collection.add(new _campaign2.default({
	                    type: 'CampaignSummary',
	                    key: 'campaign-summary',
	                    CampaignId: campaignId,
	                    SubMaps: collection.submaps,
	                    Footer: footerModel
	                }));

	                (0, _forEach3.default)(result.SubMaps, function (submap) {
	                    collection.submaps.push({
	                        Name: submap.Name,
	                        OrderId: submap.OrderId,
	                        TotalHouseHold: submap.TotalHouseHold,
	                        TargetHouseHold: submap.TargetHouseHold,
	                        Penetration: submap.Penetration
	                    });
	                    collection.add(new _submap2.default({
	                        type: 'SubMap',
	                        key: campaignId + '-' + submap.Id,
	                        CampaignId: campaignId,
	                        SubMapId: submap.Id,
	                        Name: submap.Name,
	                        OrderId: submap.OrderId,
	                        TotalHouseHold: submap.TotalHouseHold,
	                        TargetHouseHold: submap.TargetHouseHold,
	                        Penetration: submap.Penetration,
	                        Footer: footerModel
	                    }));
	                    collection.add(new _croute2.default({
	                        type: 'SubMapDetail',
	                        key: 'detail-' + campaignId + '-' + submap.Id,
	                        CampaignId: campaignId,
	                        SubMapId: submap.Id,
	                        Name: submap.Name,
	                        OrderId: submap.OrderId,
	                        Footer: footerModel
	                    }));
	                });

	                (0, _forEach3.default)(result.SubMaps, function (submap) {
	                    (0, _forEach3.default)(submap.DMaps, function (dmap) {
	                        collection.dmaps.push({
	                            Selected: false,
	                            Id: dmap.Id,
	                            Name: dmap.Name
	                        });
	                        collection.add(new _dmap2.default({
	                            type: 'DMap',
	                            key: campaignId + '-' + submap.Id + '-' + dmap.Id,
	                            CampaignId: campaignId,
	                            SubMapId: submap.Id,
	                            DMapId: dmap.Id,
	                            Name: dmap.Name,
	                            Total: dmap.Total,
	                            DisplayName: displayName,
	                            Footer: footerModel
	                        }));
	                    });
	                });
	            }
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    },
	    getCampaignId: function getCampaignId() {
	        return this.campaignId;
	    },
	    getDMaps: function getDMaps() {
	        return new DmapCollection(this.dmaps);
	    },
	    exportPdf: function exportPdf(params, opts) {
	        var collection = this,
	            options = {
	            url: 'pdf/build/',
	            method: 'POST',
	            processData: true,
	            data: _jquery2.default.param({
	                'options': JSON.stringify(params)
	            }),
	            success: function success(result) {
	                //console.log(result, result.campaignId, result.sourceFile);
	                return result;
	            }
	        };
	        options = (0, _extend3.default)(opts, options);
	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    }
	});

/***/ },
/* 97 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _user = __webpack_require__(71);

	var _user2 = _interopRequireDefault(_user);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _backbone2.default.Collection.extend({
		model: _user2.default,
		url: 'user',
		fetchInGroup: function fetchInGroup(type, opts) {
			var collection = this,
			    url = collection.url + '/group/' + type,
			    options = {
				url: url,
				type: 'GET'
			};
			(0, _extend3.default)(options, opts);
			return this.fetch(options);
		},
		fetchForGtu: function fetchForGtu(opts) {
			var collection = this,
			    options = {
				url: collection.url + '/gtu/',
				type: 'GET'
			};
			(0, _extend3.default)(options, opts);

			return this.fetch(options);
		},
		fetchCompany: function fetchCompany(opts) {
			var collection = this,
			    options = {
				url: collection.url + '/company/',
				type: 'GET'
			};
			(0, _extend3.default)(options, opts);

			return this.fetch(options);
		}
	});

/***/ },
/* 98 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _d = __webpack_require__(160);

	var _d2 = _interopRequireDefault(_d);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	FastMarkerLayer.prototype = new google.maps.OverlayView();
	FastMarkerLayer.prototype.addMarker = function (marker) {
		this._markers.push(marker);
		this.draw();
	};
	FastMarkerLayer.prototype.removeMarker = function (marker) {
		var index = this._markers.indexOf(marker);
		if (index > -1) {
			if (this._markers.length == 1) {
				this._markers = [];
				console.log('FastMarker remove last one');
			} else {
				this._markers = this._markers.slice(index, index + 1);
			}
		}
		this.draw();
	};
	FastMarkerLayer.prototype.onAdd = function () {
		var map = this.getMap(),
		    panes = this.getPanes(),
		    projection = this.getProjection(),
		    targetLayer = panes.overlayLayer;
		map.overlayMapTypes.insertAt(0, this);
		if (projection && panes && targetLayer && (0, _jquery2.default)('.fastMarkerLayer', targetLayer).size() == 0) {
			var center = projection.fromLatLngToDivPixel(map.getCenter());
			this.width = center.x * 2;
			this.height = center.y * 2;
			var markerLayer = _d2.default.select(targetLayer).append("svg").attr('style', 'position:absolute; top: 0; left: 0;').attr('class', 'fastMarkerLayer');
			this.markerLayer = markerLayer;
		}
	};
	FastMarkerLayer.prototype.onRemove = function () {
		var map = this.getMap(),
		    panes = this.getPanes(),
		    targetLayer = panes.overlayLayer;
		(0, _jquery2.default)('.fastMarkerLayer', targetLayer).remove();
	};
	FastMarkerLayer.prototype.drawTimeout = null;
	FastMarkerLayer.prototype.draw = function () {
		window.clearTimeout(this.drawTimeout);
		this.drawTimeout = window.setTimeout(_jquery2.default.proxy(this._draw, this), 500);
	};
	FastMarkerLayer.prototype._draw = function () {
		console.log('FastMarker _draw');
		var projection = this.getProjection(),
		    layer = this.markerLayer,
		    topLeft = {
			x: null,
			y: null
		},
		    bottomRight = {
			x: null,
			y: null
		};
		if (!layer || !projection) {
			console.log('FastMarker not ready error.');
			return;
		}
		layer.selectAll('*').remove();
		console.log('FastMarker draw', this._markers.length);
		_jquery2.default.each(this._markers, function () {
			var position = projection.fromLatLngToDivPixel(this.options.position);
			topLeft.x = topLeft.x == null || topLeft.x > position.x ? position.x : topLeft.x;
			topLeft.y = topLeft.y == null || topLeft.y > position.y ? position.y : topLeft.y;
			bottomRight.x = bottomRight.x == null || bottomRight.x < position.x ? position.x : bottomRight.x;
			bottomRight.y = bottomRight.y == null || bottomRight.y < position.y ? position.y : bottomRight.y;
		});
		//Todo: we just extend boundary 50px should use marker max size to extend
		topLeft.x -= 50;
		topLeft.y -= 50;
		bottomRight.x += 50;
		bottomRight.y += 50;
		layer.attr('width', Math.abs(bottomRight.x - topLeft.x)).attr('height', Math.abs(bottomRight.y - topLeft.y)).attr('style', 'position:absolute; left: ' + topLeft.x + 'px; top: ' + topLeft.y + 'px;');

		_jquery2.default.each(this._markers, function () {
			var position = projection.fromLatLngToDivPixel(this.options.position);
			layer.append('path').attr('d', this.options.icon.path).attr('fill', this.options.icon.fillColor).style("opacity", this.options.icon.fillOpacity).attr('stroke', this.options.icon.strokeColor).style("stroke-opacity", this.options.icon.strokeOpacity).attr("transform", function (d) {
				return "translate(" + (position.x - topLeft.x) + ", " + (position.y - topLeft.y) + ")";
			});
		});
	};
	FastMarker.prototype.setMap = function (map) {
		if (map != null) {
			var layer;
			if (!hashMap.has(map)) {
				layer = new FastMarkerLayer(map);
				hashMap.set(map, layer);
			} else {
				layer = hashMap.get(map);
			}
			this.Layer = layer;
			this.Map = map;
			layer.addMarker.call(this.Layer, this);
		} else {
			if (this.Layer && this.Map) {
				this.Layer.removeMarker.call(this.Layer, this);
				this.Map = null;
			}
		}
	};

	var hashMap = {
		keys: [],
		values: {},
		set: function set(k, v) {
			var index = this.keys.indexOf(k);
			if (index > -1) {
				this.values[index + ''] = v;
			} else {
				this.keys.push(k);
				this.values[this.keys.length - 1 + ''] = v;
			}
		},
		get: function get(k) {
			var index = this.keys.indexOf(k);
			if (index > -1) {
				return this.values[index + ''];
			} else {
				return null;
			}
		},
		has: function has(k) {
			return this.keys.indexOf(k) > -1;
		},
		delete: function _delete(k) {
			var index = this.keys.indexOf(k);
			if (index > -1) {
				this.keys = this.keys.splice(index, 1);
				delete this.values[index + ''];
			}
		}
	};

	function FastMarkerLayer(map) {
		this.setMap(map);
		this._markers = [];
	}

	function FastMarker(options) {
		this.options = options;
		if (this.options && this.options.position) {
			this.options.position = new google.maps.LatLng(this.options.position.lat, this.options.position.lng);
		}
		if (options && options.map) {
			var map = options.map;
			this.setMap(map);
		}
	}

	exports.default = FastMarker;

/***/ },
/* 99 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	var _task = __webpack_require__(353);

	var _task2 = _interopRequireDefault(_task);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _backbone2.default.Model.extend({
	    urlRoot: 'campaign',
	    idAttribute: 'Id',
	    defaults: {
	        'AreaDescription': null,
	        'ClientCode': null,
	        'ClientName': null,
	        'ContactName': null,
	        'CreatorName': null,
	        'CustemerName': null,
	        'Date': null,
	        'Description': null,
	        'Id': null,
	        'Name': null,
	        'Sequence': 0,
	        'UserName': null
	    },
	    getDisplayName: function getDisplayName() {
	        return this.get('ClientCode') + '-' + this.get('CreatorName') + '-' + this.get('AreaDescription');
	    },
	    copy: function copy(opts) {
	        var model = this,
	            options = {
	            method: 'POST',
	            url: model.urlRoot + '/copy/' + model.get('Id')
	        };
	        (0, _extend3.default)(options, opts);

	        return (this.sync || _backbone2.default.sync).call(this, 'create', this, options);
	    },
	    publishToDMap: function publishToDMap(user, opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/publish/',
	            method: 'PUT',
	            processData: true,
	            data: _jquery2.default.param({
	                '': user.get('Id')
	            })
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    dismissToCampaign: function dismissToCampaign(user, opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/dismiss/',
	            method: 'PUT',
	            processData: true,
	            data: _jquery2.default.param({
	                '': user.get('Id')
	            })
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    publishToMonitor: function publishToMonitor(user, opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/monitor/',
	            method: 'PUT',
	            processData: true,
	            data: _jquery2.default.param({
	                '': user.get('Id')
	            })
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    dismissToDMap: function dismissToDMap(user, opts) {
	        var model = this,
	            options = {
	            url: model.urlRoot + '/' + model.get('Id') + '/dismiss/',
	            method: 'PUT',
	            processData: true,
	            data: _jquery2.default.param({
	                '': user.get('Id')
	            })
	        };
	        options = (0, _extend3.default)(opts, options);

	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    loadWithAllTask: function loadWithAllTask(campaignId) {
	        var self = this;
	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, {
	            url: 'campaign/' + campaignId + '/tasks/all'
	        }).then(function (result) {
	            var tasks = new _task2.default();
	            tasks.set(result.Tasks);
	            result.Tasks = tasks;
	            self.set(result);
	        });
	    }
	});

/***/ },
/* 100 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _base = __webpack_require__(24);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _base2.default.extend({
	    urlRoot: 'map',
	    defaults: {
	        suppressCover: true,

	        suppressCampaign: true,
	        suppressCampaignSummary: true,
	        suppressNDAInCampaign: true,

	        suppressSubMap: true,
	        suppressSubMapCountDetail: true,
	        suppressNDAInSubMap: true,

	        suppressDMap: false,
	        suppressGTU: true,
	        suppressNDAInDMap: true,

	        customSubMapPenetrationColors: false,
	        suppressLocations: true,
	        suppressRadii: true
	    }
	});

/***/ },
/* 101 */
/***/ function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(62), __webpack_require__(5)], __WEBPACK_AMD_DEFINE_RESULT__ = function (_, BaseView) {}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
	exports.default = (0, _extend3.default)({}, _base2.default, {
		onClose: function onClose() {
			this.publish("showDialog");
			if (this.props.needTrigger) {
				this.publish('print.map.options.changed', this.getModel());
			}
		},
		onApply: function onApply(e) {
			e.preventDefault();
			e.stopPropagation();
			this.publish('print.map.options.changed', this.getModel());
		},
		OnValueChanged: function OnValueChanged(e) {
			var model = this.getModel(),
			    ele = $(e.currentTarget),
			    name = ele.attr('name'),
			    value = ele.is('input:checkbox') ? ele.prop('checked') : ele.val();
			model.set(name, value);
		}
	});

/***/ },
/* 102 */,
/* 103 */,
/* 104 */,
/* 105 */
/***/ function(module, exports, __webpack_require__) {

	var getNative = __webpack_require__(34),
	    root = __webpack_require__(22);

	/* Built-in method references that are verified to be native. */
	var Map = getNative(root, 'Map');

	module.exports = Map;


/***/ },
/* 106 */
/***/ function(module, exports, __webpack_require__) {

	var mapCacheClear = __webpack_require__(493),
	    mapCacheDelete = __webpack_require__(494),
	    mapCacheGet = __webpack_require__(495),
	    mapCacheHas = __webpack_require__(496),
	    mapCacheSet = __webpack_require__(497);

	/**
	 * Creates a map cache object to store key-value pairs.
	 *
	 * @private
	 * @constructor
	 * @param {Array} [entries] The key-value pairs to cache.
	 */
	function MapCache(entries) {
	  var index = -1,
	      length = entries == null ? 0 : entries.length;

	  this.clear();
	  while (++index < length) {
	    var entry = entries[index];
	    this.set(entry[0], entry[1]);
	  }
	}

	// Add methods to `MapCache`.
	MapCache.prototype.clear = mapCacheClear;
	MapCache.prototype['delete'] = mapCacheDelete;
	MapCache.prototype.get = mapCacheGet;
	MapCache.prototype.has = mapCacheHas;
	MapCache.prototype.set = mapCacheSet;

	module.exports = MapCache;


/***/ },
/* 107 */
/***/ function(module, exports, __webpack_require__) {

	var MapCache = __webpack_require__(106),
	    setCacheAdd = __webpack_require__(504),
	    setCacheHas = __webpack_require__(505);

	/**
	 *
	 * Creates an array cache object to store unique values.
	 *
	 * @private
	 * @constructor
	 * @param {Array} [values] The values to cache.
	 */
	function SetCache(values) {
	  var index = -1,
	      length = values == null ? 0 : values.length;

	  this.__data__ = new MapCache;
	  while (++index < length) {
	    this.add(values[index]);
	  }
	}

	// Add methods to `SetCache`.
	SetCache.prototype.add = SetCache.prototype.push = setCacheAdd;
	SetCache.prototype.has = setCacheHas;

	module.exports = SetCache;


/***/ },
/* 108 */
/***/ function(module, exports, __webpack_require__) {

	var ListCache = __webpack_require__(75),
	    stackClear = __webpack_require__(507),
	    stackDelete = __webpack_require__(508),
	    stackGet = __webpack_require__(509),
	    stackHas = __webpack_require__(510),
	    stackSet = __webpack_require__(511);

	/**
	 * Creates a stack cache object to store key-value pairs.
	 *
	 * @private
	 * @constructor
	 * @param {Array} [entries] The key-value pairs to cache.
	 */
	function Stack(entries) {
	  var data = this.__data__ = new ListCache(entries);
	  this.size = data.size;
	}

	// Add methods to `Stack`.
	Stack.prototype.clear = stackClear;
	Stack.prototype['delete'] = stackDelete;
	Stack.prototype.get = stackGet;
	Stack.prototype.has = stackHas;
	Stack.prototype.set = stackSet;

	module.exports = Stack;


/***/ },
/* 109 */
/***/ function(module, exports, __webpack_require__) {

	var defineProperty = __webpack_require__(191);

	/**
	 * The base implementation of `assignValue` and `assignMergeValue` without
	 * value checks.
	 *
	 * @private
	 * @param {Object} object The object to modify.
	 * @param {string} key The key of the property to assign.
	 * @param {*} value The value to assign.
	 */
	function baseAssignValue(object, key, value) {
	  if (key == '__proto__' && defineProperty) {
	    defineProperty(object, key, {
	      'configurable': true,
	      'enumerable': true,
	      'value': value,
	      'writable': true
	    });
	  } else {
	    object[key] = value;
	  }
	}

	module.exports = baseAssignValue;


/***/ },
/* 110 */
/***/ function(module, exports, __webpack_require__) {

	var Stack = __webpack_require__(108),
	    arrayEach = __webpack_require__(172),
	    assignValue = __webpack_require__(77),
	    baseAssign = __webpack_require__(420),
	    baseAssignIn = __webpack_require__(421),
	    cloneBuffer = __webpack_require__(453),
	    copyArray = __webpack_require__(113),
	    copySymbols = __webpack_require__(462),
	    copySymbolsIn = __webpack_require__(463),
	    getAllKeys = __webpack_require__(472),
	    getAllKeysIn = __webpack_require__(195),
	    getTag = __webpack_require__(115),
	    initCloneArray = __webpack_require__(482),
	    initCloneByTag = __webpack_require__(483),
	    initCloneObject = __webpack_require__(484),
	    isArray = __webpack_require__(12),
	    isBuffer = __webpack_require__(87),
	    isObject = __webpack_require__(20),
	    keys = __webpack_require__(23);

	/** Used to compose bitmasks for cloning. */
	var CLONE_DEEP_FLAG = 1,
	    CLONE_FLAT_FLAG = 2,
	    CLONE_SYMBOLS_FLAG = 4;

	/** `Object#toString` result references. */
	var argsTag = '[object Arguments]',
	    arrayTag = '[object Array]',
	    boolTag = '[object Boolean]',
	    dateTag = '[object Date]',
	    errorTag = '[object Error]',
	    funcTag = '[object Function]',
	    genTag = '[object GeneratorFunction]',
	    mapTag = '[object Map]',
	    numberTag = '[object Number]',
	    objectTag = '[object Object]',
	    regexpTag = '[object RegExp]',
	    setTag = '[object Set]',
	    stringTag = '[object String]',
	    symbolTag = '[object Symbol]',
	    weakMapTag = '[object WeakMap]';

	var arrayBufferTag = '[object ArrayBuffer]',
	    dataViewTag = '[object DataView]',
	    float32Tag = '[object Float32Array]',
	    float64Tag = '[object Float64Array]',
	    int8Tag = '[object Int8Array]',
	    int16Tag = '[object Int16Array]',
	    int32Tag = '[object Int32Array]',
	    uint8Tag = '[object Uint8Array]',
	    uint8ClampedTag = '[object Uint8ClampedArray]',
	    uint16Tag = '[object Uint16Array]',
	    uint32Tag = '[object Uint32Array]';

	/** Used to identify `toStringTag` values supported by `_.clone`. */
	var cloneableTags = {};
	cloneableTags[argsTag] = cloneableTags[arrayTag] =
	cloneableTags[arrayBufferTag] = cloneableTags[dataViewTag] =
	cloneableTags[boolTag] = cloneableTags[dateTag] =
	cloneableTags[float32Tag] = cloneableTags[float64Tag] =
	cloneableTags[int8Tag] = cloneableTags[int16Tag] =
	cloneableTags[int32Tag] = cloneableTags[mapTag] =
	cloneableTags[numberTag] = cloneableTags[objectTag] =
	cloneableTags[regexpTag] = cloneableTags[setTag] =
	cloneableTags[stringTag] = cloneableTags[symbolTag] =
	cloneableTags[uint8Tag] = cloneableTags[uint8ClampedTag] =
	cloneableTags[uint16Tag] = cloneableTags[uint32Tag] = true;
	cloneableTags[errorTag] = cloneableTags[funcTag] =
	cloneableTags[weakMapTag] = false;

	/**
	 * The base implementation of `_.clone` and `_.cloneDeep` which tracks
	 * traversed objects.
	 *
	 * @private
	 * @param {*} value The value to clone.
	 * @param {boolean} bitmask The bitmask flags.
	 *  1 - Deep clone
	 *  2 - Flatten inherited properties
	 *  4 - Clone symbols
	 * @param {Function} [customizer] The function to customize cloning.
	 * @param {string} [key] The key of `value`.
	 * @param {Object} [object] The parent object of `value`.
	 * @param {Object} [stack] Tracks traversed objects and their clone counterparts.
	 * @returns {*} Returns the cloned value.
	 */
	function baseClone(value, bitmask, customizer, key, object, stack) {
	  var result,
	      isDeep = bitmask & CLONE_DEEP_FLAG,
	      isFlat = bitmask & CLONE_FLAT_FLAG,
	      isFull = bitmask & CLONE_SYMBOLS_FLAG;

	  if (customizer) {
	    result = object ? customizer(value, key, object, stack) : customizer(value);
	  }
	  if (result !== undefined) {
	    return result;
	  }
	  if (!isObject(value)) {
	    return value;
	  }
	  var isArr = isArray(value);
	  if (isArr) {
	    result = initCloneArray(value);
	    if (!isDeep) {
	      return copyArray(value, result);
	    }
	  } else {
	    var tag = getTag(value),
	        isFunc = tag == funcTag || tag == genTag;

	    if (isBuffer(value)) {
	      return cloneBuffer(value, isDeep);
	    }
	    if (tag == objectTag || tag == argsTag || (isFunc && !object)) {
	      result = (isFlat || isFunc) ? {} : initCloneObject(value);
	      if (!isDeep) {
	        return isFlat
	          ? copySymbolsIn(value, baseAssignIn(result, value))
	          : copySymbols(value, baseAssign(result, value));
	      }
	    } else {
	      if (!cloneableTags[tag]) {
	        return object ? value : {};
	      }
	      result = initCloneByTag(value, tag, baseClone, isDeep);
	    }
	  }
	  // Check for circular references and return its corresponding clone.
	  stack || (stack = new Stack);
	  var stacked = stack.get(value);
	  if (stacked) {
	    return stacked;
	  }
	  stack.set(value, result);

	  var keysFunc = isFull
	    ? (isFlat ? getAllKeysIn : getAllKeys)
	    : (isFlat ? keysIn : keys);

	  var props = isArr ? undefined : keysFunc(value);
	  arrayEach(props || value, function(subValue, key) {
	    if (props) {
	      key = subValue;
	      subValue = value[key];
	    }
	    // Recursively populate clone (susceptible to call stack limits).
	    assignValue(result, key, baseClone(subValue, bitmask, customizer, key, value, stack));
	  });
	  return result;
	}

	module.exports = baseClone;


/***/ },
/* 111 */
/***/ function(module, exports) {

	/**
	 * Checks if a `cache` value for `key` exists.
	 *
	 * @private
	 * @param {Object} cache The cache to query.
	 * @param {string} key The key of the entry to check.
	 * @returns {boolean} Returns `true` if an entry for `key` exists, else `false`.
	 */
	function cacheHas(cache, key) {
	  return cache.has(key);
	}

	module.exports = cacheHas;


/***/ },
/* 112 */
/***/ function(module, exports, __webpack_require__) {

	var Uint8Array = __webpack_require__(171);

	/**
	 * Creates a clone of `arrayBuffer`.
	 *
	 * @private
	 * @param {ArrayBuffer} arrayBuffer The array buffer to clone.
	 * @returns {ArrayBuffer} Returns the cloned array buffer.
	 */
	function cloneArrayBuffer(arrayBuffer) {
	  var result = new arrayBuffer.constructor(arrayBuffer.byteLength);
	  new Uint8Array(result).set(new Uint8Array(arrayBuffer));
	  return result;
	}

	module.exports = cloneArrayBuffer;


/***/ },
/* 113 */
/***/ function(module, exports) {

	/**
	 * Copies the values of `source` to `array`.
	 *
	 * @private
	 * @param {Array} source The array to copy values from.
	 * @param {Array} [array=[]] The array to copy values to.
	 * @returns {Array} Returns `array`.
	 */
	function copyArray(source, array) {
	  var index = -1,
	      length = source.length;

	  array || (array = Array(length));
	  while (++index < length) {
	    array[index] = source[index];
	  }
	  return array;
	}

	module.exports = copyArray;


/***/ },
/* 114 */
/***/ function(module, exports, __webpack_require__) {

	var overArg = __webpack_require__(118),
	    stubArray = __webpack_require__(211);

	/* Built-in method references for those with the same name as other `lodash` methods. */
	var nativeGetSymbols = Object.getOwnPropertySymbols;

	/**
	 * Creates an array of the own enumerable symbols of `object`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of symbols.
	 */
	var getSymbols = nativeGetSymbols ? overArg(nativeGetSymbols, Object) : stubArray;

	module.exports = getSymbols;


/***/ },
/* 115 */
/***/ function(module, exports, __webpack_require__) {

	var DataView = __webpack_require__(410),
	    Map = __webpack_require__(105),
	    Promise = __webpack_require__(412),
	    Set = __webpack_require__(170),
	    WeakMap = __webpack_require__(413),
	    baseGetTag = __webpack_require__(44),
	    toSource = __webpack_require__(204);

	/** `Object#toString` result references. */
	var mapTag = '[object Map]',
	    objectTag = '[object Object]',
	    promiseTag = '[object Promise]',
	    setTag = '[object Set]',
	    weakMapTag = '[object WeakMap]';

	var dataViewTag = '[object DataView]';

	/** Used to detect maps, sets, and weakmaps. */
	var dataViewCtorString = toSource(DataView),
	    mapCtorString = toSource(Map),
	    promiseCtorString = toSource(Promise),
	    setCtorString = toSource(Set),
	    weakMapCtorString = toSource(WeakMap);

	/**
	 * Gets the `toStringTag` of `value`.
	 *
	 * @private
	 * @param {*} value The value to query.
	 * @returns {string} Returns the `toStringTag`.
	 */
	var getTag = baseGetTag;

	// Fallback for data views, maps, sets, and weak maps in IE 11 and promises in Node.js < 6.
	if ((DataView && getTag(new DataView(new ArrayBuffer(1))) != dataViewTag) ||
	    (Map && getTag(new Map) != mapTag) ||
	    (Promise && getTag(Promise.resolve()) != promiseTag) ||
	    (Set && getTag(new Set) != setTag) ||
	    (WeakMap && getTag(new WeakMap) != weakMapTag)) {
	  getTag = function(value) {
	    var result = baseGetTag(value),
	        Ctor = result == objectTag ? value.constructor : undefined,
	        ctorString = Ctor ? toSource(Ctor) : '';

	    if (ctorString) {
	      switch (ctorString) {
	        case dataViewCtorString: return dataViewTag;
	        case mapCtorString: return mapTag;
	        case promiseCtorString: return promiseTag;
	        case setCtorString: return setTag;
	        case weakMapCtorString: return weakMapTag;
	      }
	    }
	    return result;
	  };
	}

	module.exports = getTag;


/***/ },
/* 116 */
/***/ function(module, exports, __webpack_require__) {

	var eq = __webpack_require__(35),
	    isArrayLike = __webpack_require__(25),
	    isIndex = __webpack_require__(83),
	    isObject = __webpack_require__(20);

	/**
	 * Checks if the given arguments are from an iteratee call.
	 *
	 * @private
	 * @param {*} value The potential iteratee value argument.
	 * @param {*} index The potential iteratee index or key argument.
	 * @param {*} object The potential iteratee object argument.
	 * @returns {boolean} Returns `true` if the arguments are from an iteratee call,
	 *  else `false`.
	 */
	function isIterateeCall(value, index, object) {
	  if (!isObject(object)) {
	    return false;
	  }
	  var type = typeof index;
	  if (type == 'number'
	        ? (isArrayLike(object) && isIndex(index, object.length))
	        : (type == 'string' && index in object)
	      ) {
	    return eq(object[index], value);
	  }
	  return false;
	}

	module.exports = isIterateeCall;


/***/ },
/* 117 */
/***/ function(module, exports, __webpack_require__) {

	var isArray = __webpack_require__(12),
	    isSymbol = __webpack_require__(61);

	/** Used to match property names within property paths. */
	var reIsDeepProp = /\.|\[(?:[^[\]]*|(["'])(?:(?!\1)[^\\]|\\.)*?\1)\]/,
	    reIsPlainProp = /^\w*$/;

	/**
	 * Checks if `value` is a property name and not a property path.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @param {Object} [object] The object to query keys on.
	 * @returns {boolean} Returns `true` if `value` is a property name, else `false`.
	 */
	function isKey(value, object) {
	  if (isArray(value)) {
	    return false;
	  }
	  var type = typeof value;
	  if (type == 'number' || type == 'symbol' || type == 'boolean' ||
	      value == null || isSymbol(value)) {
	    return true;
	  }
	  return reIsPlainProp.test(value) || !reIsDeepProp.test(value) ||
	    (object != null && value in Object(object));
	}

	module.exports = isKey;


/***/ },
/* 118 */
/***/ function(module, exports) {

	/**
	 * Creates a unary function that invokes `func` with its argument transformed.
	 *
	 * @private
	 * @param {Function} func The function to wrap.
	 * @param {Function} transform The argument transform.
	 * @returns {Function} Returns the new function.
	 */
	function overArg(func, transform) {
	  return function(arg) {
	    return func(transform(arg));
	  };
	}

	module.exports = overArg;


/***/ },
/* 119 */
/***/ function(module, exports, __webpack_require__) {

	var baseClone = __webpack_require__(110);

	/** Used to compose bitmasks for cloning. */
	var CLONE_DEEP_FLAG = 1,
	    CLONE_SYMBOLS_FLAG = 4;

	/**
	 * This method is like `_.clone` except that it recursively clones `value`.
	 *
	 * @static
	 * @memberOf _
	 * @since 1.0.0
	 * @category Lang
	 * @param {*} value The value to recursively clone.
	 * @returns {*} Returns the deep cloned value.
	 * @see _.clone
	 * @example
	 *
	 * var objects = [{ 'a': 1 }, { 'b': 2 }];
	 *
	 * var deep = _.cloneDeep(objects);
	 * console.log(deep[0] === objects[0]);
	 * // => false
	 */
	function cloneDeep(value) {
	  return baseClone(value, CLONE_DEEP_FLAG | CLONE_SYMBOLS_FLAG);
	}

	module.exports = cloneDeep;


/***/ },
/* 120 */
/***/ function(module, exports, __webpack_require__) {

	var arrayPush = __webpack_require__(76),
	    baseFlatten = __webpack_require__(55),
	    copyArray = __webpack_require__(113),
	    isArray = __webpack_require__(12);

	/**
	 * Creates a new array concatenating `array` with any additional arrays
	 * and/or values.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Array
	 * @param {Array} array The array to concatenate.
	 * @param {...*} [values] The values to concatenate.
	 * @returns {Array} Returns the new concatenated array.
	 * @example
	 *
	 * var array = [1];
	 * var other = _.concat(array, 2, [3], [[4]]);
	 *
	 * console.log(other);
	 * // => [1, 2, 3, [4]]
	 *
	 * console.log(array);
	 * // => [1]
	 */
	function concat() {
	  var length = arguments.length;
	  if (!length) {
	    return [];
	  }
	  var args = Array(length - 1),
	      array = arguments[0],
	      index = length;

	  while (index--) {
	    args[index - 1] = arguments[index];
	  }
	  return arrayPush(isArray(array) ? copyArray(array) : [array], baseFlatten(args, 1));
	}

	module.exports = concat;


/***/ },
/* 121 */
/***/ function(module, exports) {

	/** Used as references for various `Number` constants. */
	var MAX_SAFE_INTEGER = 9007199254740991;

	/**
	 * Checks if `value` is a valid array-like length.
	 *
	 * **Note:** This method is loosely based on
	 * [`ToLength`](http://ecma-international.org/ecma-262/7.0/#sec-tolength).
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is a valid length, else `false`.
	 * @example
	 *
	 * _.isLength(3);
	 * // => true
	 *
	 * _.isLength(Number.MIN_VALUE);
	 * // => false
	 *
	 * _.isLength(Infinity);
	 * // => false
	 *
	 * _.isLength('3');
	 * // => false
	 */
	function isLength(value) {
	  return typeof value == 'number' &&
	    value > -1 && value % 1 == 0 && value <= MAX_SAFE_INTEGER;
	}

	module.exports = isLength;


/***/ },
/* 122 */
/***/ function(module, exports, __webpack_require__) {

	var baseIsTypedArray = __webpack_require__(434),
	    baseUnary = __webpack_require__(81),
	    nodeUtil = __webpack_require__(501);

	/* Node.js helper references. */
	var nodeIsTypedArray = nodeUtil && nodeUtil.isTypedArray;

	/**
	 * Checks if `value` is classified as a typed array.
	 *
	 * @static
	 * @memberOf _
	 * @since 3.0.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is a typed array, else `false`.
	 * @example
	 *
	 * _.isTypedArray(new Uint8Array);
	 * // => true
	 *
	 * _.isTypedArray([]);
	 * // => false
	 */
	var isTypedArray = nodeIsTypedArray ? baseUnary(nodeIsTypedArray) : baseIsTypedArray;

	module.exports = isTypedArray;


/***/ },
/* 123 */
/***/ function(module, exports, __webpack_require__) {

	var arrayLikeKeys = __webpack_require__(176),
	    baseKeysIn = __webpack_require__(435),
	    isArrayLike = __webpack_require__(25);

	/**
	 * Creates an array of the own and inherited enumerable property names of `object`.
	 *
	 * **Note:** Non-object values are coerced to objects.
	 *
	 * @static
	 * @memberOf _
	 * @since 3.0.0
	 * @category Object
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of property names.
	 * @example
	 *
	 * function Foo() {
	 *   this.a = 1;
	 *   this.b = 2;
	 * }
	 *
	 * Foo.prototype.c = 3;
	 *
	 * _.keysIn(new Foo);
	 * // => ['a', 'b', 'c'] (iteration order is not guaranteed)
	 */
	function keysIn(object) {
	  return isArrayLike(object) ? arrayLikeKeys(object, true) : baseKeysIn(object);
	}

	module.exports = keysIn;


/***/ },
/* 124 */
/***/ function(module, exports, __webpack_require__) {

	var arrayMap = __webpack_require__(31),
	    baseClone = __webpack_require__(110),
	    baseUnset = __webpack_require__(189),
	    castPath = __webpack_require__(45),
	    copyObject = __webpack_require__(33),
	    flatRest = __webpack_require__(193),
	    getAllKeysIn = __webpack_require__(195);

	/** Used to compose bitmasks for cloning. */
	var CLONE_DEEP_FLAG = 1,
	    CLONE_FLAT_FLAG = 2,
	    CLONE_SYMBOLS_FLAG = 4;

	/**
	 * The opposite of `_.pick`; this method creates an object composed of the
	 * own and inherited enumerable property paths of `object` that are not omitted.
	 *
	 * **Note:** This method is considerably slower than `_.pick`.
	 *
	 * @static
	 * @since 0.1.0
	 * @memberOf _
	 * @category Object
	 * @param {Object} object The source object.
	 * @param {...(string|string[])} [paths] The property paths to omit.
	 * @returns {Object} Returns the new object.
	 * @example
	 *
	 * var object = { 'a': 1, 'b': '2', 'c': 3 };
	 *
	 * _.omit(object, ['a', 'c']);
	 * // => { 'b': '2' }
	 */
	var omit = flatRest(function(object, paths) {
	  var result = {};
	  if (object == null) {
	    return result;
	  }
	  var isDeep = false;
	  paths = arrayMap(paths, function(path) {
	    path = castPath(path, object);
	    isDeep || (isDeep = path.length > 1);
	    return path;
	  });
	  copyObject(object, getAllKeysIn(object), result);
	  if (isDeep) {
	    result = baseClone(result, CLONE_DEEP_FLAG | CLONE_FLAT_FLAG | CLONE_SYMBOLS_FLAG);
	  }
	  var length = paths.length;
	  while (length--) {
	    baseUnset(result, paths[length]);
	  }
	  return result;
	});

	module.exports = omit;


/***/ },
/* 125 */
/***/ function(module, exports, __webpack_require__) {

	var baseOrderBy = __webpack_require__(185),
	    isArray = __webpack_require__(12);

	/**
	 * This method is like `_.sortBy` except that it allows specifying the sort
	 * orders of the iteratees to sort by. If `orders` is unspecified, all values
	 * are sorted in ascending order. Otherwise, specify an order of "desc" for
	 * descending or "asc" for ascending sort order of corresponding values.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Collection
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Array[]|Function[]|Object[]|string[]} [iteratees=[_.identity]]
	 *  The iteratees to sort by.
	 * @param {string[]} [orders] The sort orders of `iteratees`.
	 * @param- {Object} [guard] Enables use as an iteratee for methods like `_.reduce`.
	 * @returns {Array} Returns the new sorted array.
	 * @example
	 *
	 * var users = [
	 *   { 'user': 'fred',   'age': 48 },
	 *   { 'user': 'barney', 'age': 34 },
	 *   { 'user': 'fred',   'age': 40 },
	 *   { 'user': 'barney', 'age': 36 }
	 * ];
	 *
	 * // Sort by `user` in ascending order and by `age` in descending order.
	 * _.orderBy(users, ['user', 'age'], ['asc', 'desc']);
	 * // => objects for [['barney', 36], ['barney', 34], ['fred', 48], ['fred', 40]]
	 */
	function orderBy(collection, iteratees, orders, guard) {
	  if (collection == null) {
	    return [];
	  }
	  if (!isArray(iteratees)) {
	    iteratees = iteratees == null ? [] : [iteratees];
	  }
	  orders = guard ? undefined : orders;
	  if (!isArray(orders)) {
	    orders = orders == null ? [] : [orders];
	  }
	  return baseOrderBy(collection, iteratees, orders);
	}

	module.exports = orderBy;


/***/ },
/* 126 */
/***/ function(module, exports, __webpack_require__) {

	var basePick = __webpack_require__(438),
	    flatRest = __webpack_require__(193);

	/**
	 * Creates an object composed of the picked `object` properties.
	 *
	 * @static
	 * @since 0.1.0
	 * @memberOf _
	 * @category Object
	 * @param {Object} object The source object.
	 * @param {...(string|string[])} [paths] The property paths to pick.
	 * @returns {Object} Returns the new object.
	 * @example
	 *
	 * var object = { 'a': 1, 'b': '2', 'c': 3 };
	 *
	 * _.pick(object, ['a', 'c']);
	 * // => { 'a': 1, 'c': 3 }
	 */
	var pick = flatRest(function(object, paths) {
	  return object == null ? {} : basePick(object, paths);
	});

	module.exports = pick;


/***/ },
/* 127 */
/***/ function(module, exports, __webpack_require__) {

	var toFinite = __webpack_require__(533);

	/**
	 * Converts `value` to an integer.
	 *
	 * **Note:** This method is loosely based on
	 * [`ToInteger`](http://www.ecma-international.org/ecma-262/7.0/#sec-tointeger).
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to convert.
	 * @returns {number} Returns the converted integer.
	 * @example
	 *
	 * _.toInteger(3.2);
	 * // => 3
	 *
	 * _.toInteger(Number.MIN_VALUE);
	 * // => 0
	 *
	 * _.toInteger(Infinity);
	 * // => 1.7976931348623157e+308
	 *
	 * _.toInteger('3.2');
	 * // => 3
	 */
	function toInteger(value) {
	  var result = toFinite(value),
	      remainder = result % 1;

	  return result === result ? (remainder ? result - remainder : result) : 0;
	}

	module.exports = toInteger;


/***/ },
/* 128 */
/***/ function(module, exports, __webpack_require__) {

	var baseUniq = __webpack_require__(188);

	/**
	 * Creates a duplicate-free version of an array, using
	 * [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
	 * for equality comparisons, in which only the first occurrence of each element
	 * is kept. The order of result values is determined by the order they occur
	 * in the array.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Array
	 * @param {Array} array The array to inspect.
	 * @returns {Array} Returns the new duplicate free array.
	 * @example
	 *
	 * _.uniq([2, 1, 2]);
	 * // => [2, 1]
	 */
	function uniq(array) {
	  return (array && array.length) ? baseUniq(array) : [];
	}

	module.exports = uniq;


/***/ },
/* 129 */
/***/ function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/**
	 * postal - Pub/Sub library providing wildcard subscriptions, complex message handling, etc.  Works server and client-side.
	 * Author: Jim Cowart (http://ifandelse.com)
	 * Version: v2.0.4
	 * Url: http://github.com/postaljs/postal.js
	 * License(s): MIT
	 */

	( function( root, factory ) {
		
		if ( true ) {
			// AMD. Register as an anonymous module.
			!(__WEBPACK_AMD_DEFINE_ARRAY__ = [ __webpack_require__(62) ], __WEBPACK_AMD_DEFINE_RESULT__ = function( _ ) {
				return factory( _, root );
			}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
		
		} else if ( typeof module === "object" && module.exports ) {
			// Node, or CommonJS-Like environments
			module.exports = factory( require( "lodash" ), this );
		} else {
			// Browser globals
			root.postal = factory( root._, root );
		}
	}( this, function( _, global, undefined ) {
		var prevPostal = global && global.postal;
		var prevLodash = global && global._;
		if ( prevLodash && prevLodash !== _ ) {
			_ = _.noConflict();
		}
		var _defaultConfig = {
			DEFAULT_CHANNEL: "/",
			SYSTEM_CHANNEL: "postal",
			enableSystemMessages: true,
			cacheKeyDelimiter: "|",
			autoCompactResolver: false
		};
		var postal = {
			configuration: _.extend( {}, _defaultConfig )
		};
		var _config = postal.configuration;

		

	var ChannelDefinition = function( channelName, bus ) {
		this.bus = bus;
		this.channel = channelName || _config.DEFAULT_CHANNEL;
	};

	ChannelDefinition.prototype.subscribe = function() {
		return this.bus.subscribe( {
			channel: this.channel,
			topic: ( arguments.length === 1 ? arguments[ 0 ].topic : arguments[ 0 ] ),
			callback: ( arguments.length === 1 ? arguments[ 0 ].callback : arguments[ 1 ] )
		} );
	};

	/*
	    publish( envelope [, callback ] );
	    publish( topic, data [, callback ] );
	*/
	ChannelDefinition.prototype.publish = function() {
		var envelope = {};
		var callback;
		if ( typeof arguments[ 0 ] === "string" ) {
			envelope.topic = arguments[ 0 ];
			envelope.data = arguments[ 1 ];
			callback = arguments[ 2 ];
		} else {
			envelope = arguments[ 0 ];
			callback = arguments[ 1 ];
		}
		if ( typeof envelope !== "object" ) {
			throw new Error( "The first argument to ChannelDefinition.publish should be either an envelope object or a string topic." );
		}
		envelope.channel = this.channel;
		this.bus.publish( envelope, callback );
	};

		
	var SubscriptionDefinition = function( channel, topic, callback ) {
		if ( arguments.length !== 3 ) {
			throw new Error( "You must provide a channel, topic and callback when creating a SubscriptionDefinition instance." );
		}
		if ( topic.length === 0 ) {
			throw new Error( "Topics cannot be empty" );
		}
		this.channel = channel;
		this.topic = topic;
		this.callback = callback;
		this.pipeline = [];
		this.cacheKeys = [];
		this._context = undefined;
	};

	var ConsecutiveDistinctPredicate = function() {
		var previous;
		return function( data ) {
			var eq = false;
			if ( typeof data === "string" ) {
				eq = data === previous;
				previous = data;
			} else {
				eq = _.isEqual( data, previous );
				previous = _.extend( {}, data );
			}
			return !eq;
		};
	};

	var DistinctPredicate = function DistinctPredicateFactory() {
		var previous = [];
		return function DistinctPredicate( data ) {
			var isDistinct = !_.some( previous, function( p ) {
				return _.isEqual( data, p );
			} );
			if ( isDistinct ) {
				previous.push( data );
			}
			return isDistinct;
		};
	};

	SubscriptionDefinition.prototype = {

		"catch": function( errorHandler ) {
			var original = this.callback;
			var safeCallback = function() {
				try {
					original.apply( this, arguments );
				} catch ( err ) {
					errorHandler( err, arguments[ 0 ] );
				}
			};
			this.callback = safeCallback;
			return this;
		},

		defer: function defer() {
			return this.delay( 0 );
		},

		disposeAfter: function disposeAfter( maxCalls ) {
			if ( typeof maxCalls !== "number" || maxCalls <= 0 ) {
				throw new Error( "The value provided to disposeAfter (maxCalls) must be a number greater than zero." );
			}
			var dispose = _.after( maxCalls, this.unsubscribe.bind( this ) );
			this.pipeline.push( function( data, env, next ) {
				next( data, env );
				dispose();
			} );
			return this;
		},

		distinct: function distinct() {
			return this.constraint( new DistinctPredicate() );
		},

		distinctUntilChanged: function distinctUntilChanged() {
			return this.constraint( new ConsecutiveDistinctPredicate() );
		},

		invokeSubscriber: function invokeSubscriber( data, env ) {
			if ( !this.inactive ) {
				var self = this;
				var pipeline = self.pipeline;
				var len = pipeline.length;
				var context = self._context;
				var idx = -1;
				var invoked = false;
				if ( !len ) {
					self.callback.call( context, data, env );
					invoked = true;
				} else {
					pipeline = pipeline.concat( [ self.callback ] );
					var step = function step( d, e ) {
						idx += 1;
						if ( idx < len ) {
							pipeline[ idx ].call( context, d, e, step );
						} else {
							self.callback.call( context, d, e );
							invoked = true;
						}
					};
					step( data, env, 0 );
				}
				return invoked;
			}
		},

		logError: function logError() {
			
			if ( console ) {
				var report;
				if ( console.warn ) {
					report = console.warn;
				} else {
					report = console.log;
				}
				this.catch( report );
			}
			return this;
		},

		once: function once() {
			return this.disposeAfter( 1 );
		},

		subscribe: function subscribe( callback ) {
			this.callback = callback;
			return this;
		},

		unsubscribe: function unsubscribe() {
			
			if ( !this.inactive ) {
				postal.unsubscribe( this );
			}
		},

		constraint: function constraint( predicate ) {
			if ( typeof predicate !== "function" ) {
				throw new Error( "Predicate constraint must be a function" );
			}
			this.pipeline.push( function( data, env, next ) {
				if ( predicate.call( this, data, env ) ) {
					next( data, env );
				}
			} );
			return this;
		},

		constraints: function constraints( predicates ) {
			var self = this;
			
			_.each( predicates, function( predicate ) {
				self.constraint( predicate );
			} );
			return self;
		},

		context: function contextSetter( context ) {
			this._context = context;
			return this;
		},

		debounce: function debounce( milliseconds, immediate ) {
			if ( typeof milliseconds !== "number" ) {
				throw new Error( "Milliseconds must be a number" );
			}

			var options = {};

			if ( !!immediate === true ) { 
				options.leading = true;
				options.trailing = false;
			}

			this.pipeline.push(
				_.debounce( function( data, env, next ) {
					next( data, env );
				},
					milliseconds,
					options
				)
			);
			return this;
		},

		delay: function delay( milliseconds ) {
			if ( typeof milliseconds !== "number" ) {
				throw new Error( "Milliseconds must be a number" );
			}
			var self = this;
			self.pipeline.push( function( data, env, next ) {
				setTimeout( function() {
					next( data, env );
				}, milliseconds );
			} );
			return this;
		},

		throttle: function throttle( milliseconds ) {
			if ( typeof milliseconds !== "number" ) {
				throw new Error( "Milliseconds must be a number" );
			}
			var fn = function( data, env, next ) {
				next( data, env );
			};
			this.pipeline.push( _.throttle( fn, milliseconds ) );
			return this;
		}
	};

		


	var bindingsResolver = _config.resolver = {
		cache: {},
		regex: {},
		enableCache: true,

		compare: function compare( binding, topic, headerOptions ) {
			var pattern;
			var rgx;
			var prevSegment;
			var cacheKey = topic + _config.cacheKeyDelimiter + binding;
			var result = ( this.cache[ cacheKey ] );
			var opt = headerOptions || {};
			var saveToCache = this.enableCache && !opt.resolverNoCache;
			// result is cached?
			if ( result === true ) {
				return result;
			}
			// plain string matching?
			if ( binding.indexOf( "#" ) === -1 && binding.indexOf( "*" ) === -1 ) {
				result = ( topic === binding );
				if ( saveToCache ) {
					this.cache[ cacheKey ] = result;
				}
				return result;
			}
			// ah, regex matching, then
			if ( !( rgx = this.regex[ binding ] ) ) {
				pattern = "^" + _.map( binding.split( "." ), function mapTopicBinding( segment ) {
						var res = "";
						if ( !!prevSegment ) {
							res = prevSegment !== "#" ? "\\.\\b" : "\\b";
						}
						if ( segment === "#" ) {
							res += "[\\s\\S]*";
						} else if ( segment === "*" ) {
							res += "[^.]+";
						} else {
							res += segment;
						}
						prevSegment = segment;
						return res;
					} ).join( "" ) + "$";
				rgx = this.regex[ binding ] = new RegExp( pattern );
			}
			result = rgx.test( topic );
			if ( saveToCache ) {
				this.cache[ cacheKey ] = result;
			}
			return result;
		},

		reset: function reset() {
			this.cache = {};
			this.regex = {};
		},

		purge: function( options ) {
			var self = this;
			var keyDelimiter = _config.cacheKeyDelimiter;
			var matchPredicate = function( val, key ) {
				var split = key.split( keyDelimiter );
				var topic = split[ 0 ];
				var binding = split[ 1 ];
				if ( ( typeof options.topic === "undefined" || options.topic === topic ) &&
						( typeof options.binding === "undefined" || options.binding === binding ) ) {
					delete self.cache[ key ];
				}
			};

			var compactPredicate = function( val, key ) {
				var split = key.split( keyDelimiter );
				if ( postal.getSubscribersFor( { topic: split[ 0 ] } ).length === 0 ) {
					delete self.cache[ key ];
				}
			};

			if ( typeof options === "undefined" ) {
				this.reset();
			} else {
				var handler = options.compact === true ? compactPredicate : matchPredicate;
				_.each( this.cache, handler );
			}
		}
	};

		


	var pubInProgress = 0;
	var unSubQueue = [];
	var autoCompactIndex = 0;

	function clearUnSubQueue() {
		while ( unSubQueue.length ) {
			postal.unsubscribe( unSubQueue.shift() );
		}
	}

	function getCachePurger( subDef, key, cache ) {
		return function( sub, i, list ) {
			if ( sub === subDef ) {
				list.splice( i, 1 );
			}
			if ( list.length === 0 ) {
				delete cache[ key ];
			}
		};
	}

	function getCacher( topic, pubCache, cacheKey, done, envelope ) {
		var headers = envelope && envelope.headers || {};
		return function( subDef ) {
			var cache;
			if ( _config.resolver.compare( subDef.topic, topic, headers ) ) {
				if ( !headers.resolverNoCache ) {
					cache = pubCache[ cacheKey ] = ( pubCache[ cacheKey ] || [] );
					cache.push( subDef );
				}
				subDef.cacheKeys.push( cacheKey );
				if ( done ) {
					done( subDef );
				}
			}
		};
	}

	function getSystemMessage( kind, subDef ) {
		return {
			channel: _config.SYSTEM_CHANNEL,
			topic: "subscription." + kind,
			data: {
				event: "subscription." + kind,
				channel: subDef.channel,
				topic: subDef.topic
			}
		};
	}

	var sysCreatedMessage = getSystemMessage.bind( undefined, "created" );
	var sysRemovedMessage = getSystemMessage.bind( undefined, "removed" );

	function getPredicate( options, resolver ) {
		if ( typeof options === "function" ) {
			return options;
		} else if ( !options ) {
			return function() {
				return true;
			};
		} else {
			return function( sub ) {
				var compared = 0;
				var matched = 0;
				_.each( options, function( val, prop ) {
					compared += 1;
					if (
					// We use the bindings resolver to compare the options.topic to subDef.topic
					( prop === "topic" && resolver.compare( sub.topic, options.topic, { resolverNoCache: true } ) ) ||
							( prop === "context" && options.context === sub._context ) ||
							// Any other potential prop/value matching outside topic & context...
							( sub[ prop ] === options[ prop ] ) ) {
						matched += 1;
					}
				} );
				return compared === matched;
			};
		}
	}

	_.extend( postal, {
		cache: {},
		subscriptions: {},
		wireTaps: [],

		ChannelDefinition: ChannelDefinition,
		SubscriptionDefinition: SubscriptionDefinition,

		channel: function channel( channelName ) {
			return new ChannelDefinition( channelName, this );
		},

		addWireTap: function addWireTap( callback ) {
			var self = this;
			self.wireTaps.push( callback );
			return function() {
				var idx = self.wireTaps.indexOf( callback );
				if ( idx !== -1 ) {
					self.wireTaps.splice( idx, 1 );
				}
			};
		},

		noConflict: function noConflict() {
			
			if ( typeof window === "undefined" || ( typeof window !== "undefined" && "function" === "function" && __webpack_require__(617) ) ) {
				throw new Error( "noConflict can only be used in browser clients which aren't using AMD modules" );
			}
			global.postal = prevPostal;
			return this;
		},

		getSubscribersFor: function getSubscribersFor( options ) {
			var result = [];
			var self = this;
			_.each( self.subscriptions, function( channel ) {
				_.each( channel, function( subList ) {
					result = result.concat( _.filter( subList, getPredicate( options, _config.resolver ) ) );
				} );
			} );
			return result;
		},

		publish: function publish( envelope, cb ) {
			++pubInProgress;
			var channel = envelope.channel = envelope.channel || _config.DEFAULT_CHANNEL;
			var topic = envelope.topic;
			envelope.timeStamp = new Date();
			if ( this.wireTaps.length ) {
				_.each( this.wireTaps, function( tap ) {
					tap( envelope.data, envelope, pubInProgress );
				} );
			}
			var cacheKey = channel + _config.cacheKeyDelimiter + topic;
			var cache = this.cache[ cacheKey ];
			var skipped = 0;
			var activated = 0;
			if ( !cache ) {
				var cacherFn = getCacher(
					topic,
					this.cache,
					cacheKey,
					function( candidate ) {
						if ( candidate.invokeSubscriber( envelope.data, envelope ) ) {
							activated++;
						} else {
							skipped++;
						}
					},
					envelope
				);
				_.each( this.subscriptions[ channel ], function( candidates ) {
					_.each( candidates, cacherFn );
				} );
			} else {
				_.each( cache, function( subDef ) {
					if ( subDef.invokeSubscriber( envelope.data, envelope ) ) {
						activated++;
					} else {
						skipped++;
					}
				} );
			}
			if ( --pubInProgress === 0 ) {
				clearUnSubQueue();
			}
			if ( cb ) {
				cb( {
					activated: activated,
					skipped: skipped
				} );
			}
		},

		reset: function reset() {
			this.unsubscribeFor();
			_config.resolver.reset();
			this.subscriptions = {};
			this.cache = {};
		},

		subscribe: function subscribe( options ) {
			var subscriptions = this.subscriptions;
			var subDef = new SubscriptionDefinition( options.channel || _config.DEFAULT_CHANNEL, options.topic, options.callback );
			var channel = subscriptions[ subDef.channel ];
			var channelLen = subDef.channel.length;
			var subs;
			if ( !channel ) {
				channel = subscriptions[ subDef.channel ] = {};
			}
			subs = subscriptions[ subDef.channel ][ subDef.topic ];
			if ( !subs ) {
				subs = subscriptions[ subDef.channel ][ subDef.topic ] = [];
			}
			// First, add the SubscriptionDefinition to the channel list
			subs.push( subDef );
			// Next, add the SubscriptionDefinition to any relevant existing cache(s)
			var cache = this.cache;
			_.each( _.keys( cache ), function( cacheKey ) {
				if ( cacheKey.substr( 0, channelLen ) === subDef.channel ) {
					getCacher(
						cacheKey.split( _config.cacheKeyDelimiter )[1],
						cache,
						cacheKey )( subDef );
				}
			} );
			
			if ( _config.enableSystemMessages ) {
				this.publish( sysCreatedMessage( subDef ) );
			}
			return subDef;
		},

		unsubscribe: function unsubscribe() {
			var unSubLen = arguments.length;
			var unSubIdx = 0;
			var subDef;
			var channelSubs;
			var topicSubs;
			var idx;
			for ( ; unSubIdx < unSubLen; unSubIdx++ ) {
				subDef = arguments[ unSubIdx ];
				subDef.inactive = true;
				if ( pubInProgress ) {
					unSubQueue.push( subDef );
					return;
				}
				channelSubs = this.subscriptions[ subDef.channel ];
				topicSubs = channelSubs && channelSubs[ subDef.topic ];
				
				if ( topicSubs ) {
					var len = topicSubs.length;
					idx = 0;
					// remove SubscriptionDefinition from channel list
					while ( idx < len ) {
						
						if ( topicSubs[ idx ] === subDef ) {
							topicSubs.splice( idx, 1 );
							break;
						}
						idx += 1;
					}
					if ( topicSubs.length === 0 ) {
						delete channelSubs[ subDef.topic ];
						if ( !_.keys( channelSubs ).length ) {
							delete this.subscriptions[ subDef.channel ];
						}
					}
					// remove SubscriptionDefinition from postal cache
					if ( subDef.cacheKeys && subDef.cacheKeys.length ) {
						var key;
						while ( key = subDef.cacheKeys.pop() ) {
							_.each( this.cache[ key ], getCachePurger( subDef, key, this.cache ) );
						}
					}
					if ( typeof _config.resolver.purge === "function" ) {
						// check to see if relevant resolver cache entries can be purged
						var autoCompact = _config.autoCompactResolver === true ?
							0 : typeof _config.autoCompactResolver === "number" ?
								( _config.autoCompactResolver - 1 ) : false;
						if ( autoCompact >= 0 && autoCompactIndex === autoCompact ) {
							_config.resolver.purge( { compact: true } );
							autoCompactIndex = 0;
						} else if ( autoCompact >= 0 && autoCompactIndex < autoCompact ) {
							autoCompactIndex += 1;
						}
					}
				}
				if ( _config.enableSystemMessages ) {
					this.publish( sysRemovedMessage( subDef ) );
				}
			}
		},

		unsubscribeFor: function unsubscribeFor( options ) {
			var toDispose = [];
			
			if ( this.subscriptions ) {
				toDispose = this.getSubscribersFor( options );
				this.unsubscribe.apply( this, toDispose );
			}
		}
	} );


		
		if ( global && Object.prototype.hasOwnProperty.call( global, "__postalReady__" ) && _.isArray( global.__postalReady__ ) ) {
			while ( global.__postalReady__.length ) {
				global.__postalReady__.shift().onReady( postal );
			}
		}
		

		return postal;
	} ) );


/***/ },
/* 130 */,
/* 131 */,
/* 132 */,
/* 133 */,
/* 134 */,
/* 135 */,
/* 136 */,
/* 137 */,
/* 138 */,
/* 139 */,
/* 140 */,
/* 141 */,
/* 142 */,
/* 143 */,
/* 144 */,
/* 145 */,
/* 146 */,
/* 147 */,
/* 148 */,
/* 149 */,
/* 150 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	__webpack_require__(104);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		componentDidMount: function componentDidMount() {
			var self = this,
			    model = this.getModel();

			if (!model.get('Date')) {
				model.set('Date', new Date());
			}

			$('.fdatepicker').fdatepicker({
				format: 'yyyy-mm-dd'
			}).on('changeDate', function (e) {
				self.getModel().set('Date', e.date);
			});
			$('form').foundation();
		},
		onSave: function onSave(e) {
			e.preventDefault();
			e.stopPropagation();
			var self = this;
			this.getModel().save(null, {
				success: function success(model, response) {
					if (response && response.success) {
						self.publish('camapign/refresh');
						self.onClose();
					} else {
						self.setState({
							error: "opps! something wrong. please contact us!"
						});
					}
				},
				error: function error() {
					self.setState({
						error: "opps! something wrong. please contact us!"
					});
				}
			});
		},
		onClose: function onClose() {
			$('.fdatepicker').off('changeDate').fdatepicker('remove');
			this.publish("showDialog");
		},
		onChange: function onChange(e) {
			this.getModel().set(e.currentTarget.name, e.currentTarget.value);
		},
		render: function render() {
			var model = this.getModel();
			var title = model.get('Id') ? 'Edit Campaign' : 'New Campaign';
			var AreaDescription = model.get('AreaDescription');
			var date = model.get('Date');
			var displayDate = date ? (0, _moment2.default)(date).format("YYYY-MM-DD") : '';
			var showError = this.state && this.state.error ? true : false;
			var errorMessage = showError ? this.state.error : "";
			return _react2.default.createElement(
				'form',
				{ 'data-abide': true, onSubmit: this.onSave },
				_react2.default.createElement(
					'h3',
					null,
					title
				),
				_react2.default.createElement(
					'div',
					{ 'data-abide-error': true, className: 'alert callout', style: { display: showError ? 'block' : 'none' } },
					_react2.default.createElement(
						'p',
						null,
						_react2.default.createElement('i', { className: 'fa fa-exclamation-circle' }),
						'\xA0',
						errorMessage
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Client Name',
							_react2.default.createElement('input', { onChange: this.onChange, name: 'ClientName', type: 'text', defaultValue: model.get('ClientName'), required: true }),
							_react2.default.createElement(
								'span',
								{ className: 'form-error' },
								'it is required.'
							)
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Contact Name',
							_react2.default.createElement('input', { onChange: this.onChange, name: 'ContactName', type: 'text', defaultValue: model.get('ContactName'), required: true })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Client Code',
							_react2.default.createElement('input', { onChange: this.onChange, name: 'ClientCode', type: 'text', defaultValue: model.get('ClientCode'), required: true })
						)
					),
					_react2.default.createElement(
						'fieldset',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Total Type'
						),
						_react2.default.createElement('input', { type: 'radio', onChange: this.onChange, name: 'AreaDescription', checked: "APT + HOME" == AreaDescription, value: 'APT + HOME', id: 'apthome' }),
						_react2.default.createElement(
							'label',
							{ htmlFor: 'apthome' },
							'APT + HOME'
						),
						_react2.default.createElement('input', { type: 'radio', onChange: this.onChange, name: 'AreaDescription', checked: "APT ONLY" == AreaDescription, value: 'APT ONLY', id: 'aptonly' }),
						_react2.default.createElement(
							'label',
							{ htmlFor: 'aptonly' },
							'APT ONLY'
						),
						_react2.default.createElement('input', { type: 'radio', onChange: this.onChange, name: 'AreaDescription', checked: "HOME ONLY" == AreaDescription, value: 'HOME ONLY', id: 'homeonly' }),
						_react2.default.createElement(
							'label',
							{ htmlFor: 'homeonly' },
							'HOME ONLY'
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 medium-12 large-4 columns end' },
						_react2.default.createElement(
							'label',
							null,
							'Date',
							_react2.default.createElement('input', { className: 'fdatepicker', onChange: this.onChange, name: 'Date', type: 'date', readOnly: true, value: displayDate, required: true })
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'button-group float-right' },
						_react2.default.createElement(
							'button',
							{ type: 'submit', className: 'success button' },
							'Save'
						),
						_react2.default.createElement(
							'a',
							{ href: 'javascript:;', className: 'button', onClick: this.onClose },
							'Cancel'
						)
					)
				),
				_react2.default.createElement(
					'button',
					{ onClick: this.onClose, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					_react2.default.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'\xD7'
					)
				)
			);
		}
	});

/***/ },
/* 151 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _groupBy2 = __webpack_require__(206);

	var _groupBy3 = _interopRequireDefault(_groupBy2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	__webpack_require__(149);

	__webpack_require__(352);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				editId: null
			};
		},
		componentDidMount: function componentDidMount() {
			var self = this,
			    model = this.getModel();

			(0, _jquery2.default)('form').foundation();
		},
		componentWillUpdate: function componentWillUpdate(newProps, newState) {
			if (this.state.editId != null && newState.editId != this.state.editId && this.refs['userSelector-' + this.state.editId]) {
				(0, _jquery2.default)(this.refs['userSelector-' + this.state.editId]).select2('destroy');
			}
		},
		componentDidUpdate: function componentDidUpdate(prevProps, prevState) {
			if (this.state.editId != null && this.state.editId != prevState.editId && this.refs['userSelector-' + this.state.editId]) {
				(0, _jquery2.default)(this.refs['userSelector-' + this.state.editId]).select2();
			}
		},
		onClose: function onClose() {
			this.publish("showDialog");
		},
		onAdd: function onAdd(gtuId) {
			this.setState({
				editId: gtuId
			});
		},
		onSave: function onSave(gtuId) {
			var taskId = this.props.taskId,
			    collection = this.getCollection(),
			    model = collection.get(gtuId),
			    color = (0, _jquery2.default)(this.refs['userColor']).val(),
			    user = (0, _jquery2.default)(this.refs['userSelector-' + gtuId]).val(),
			    self = this;
			model.assignGTUToTask({
				Id: gtuId,
				TaskId: taskId,
				UserColor: color,
				AuditorId: user
			}).done(function () {
				self.setState({
					editId: null
				});
			});
		},
		onCacnel: function onCacnel() {
			this.setState({
				editId: null
			});
		},
		onRemove: function onRemove(gtuId) {
			var _this = this;

			var msg = 'Are you sure you want to remove the assignment from GTU and Employee?';
			this.confirm(msg).then(function () {
				var taskId = _this.props.taskId,
				    collection = _this.getCollection(),
				    model = collection.get(gtuId);
				model.unassignGTUFromTask(taskId, gtuId);
			});
		},
		renderEditForm: function renderEditForm(gtu) {
			var self = this,
			    user = (0, _groupBy3.default)(this.props.user.models, function (item) {
				return item.get('CompanyId');
			});
			return _react2.default.createElement(
				'tr',
				{ key: gtu.get('Id') },
				_react2.default.createElement(
					'td',
					{ className: 'text-center' },
					gtu.get('BagId')
				),
				_react2.default.createElement(
					'td',
					{ className: 'text-center' },
					gtu.get('ShortUniqueID')
				),
				_react2.default.createElement(
					'td',
					{ className: 'text-center' },
					_react2.default.createElement('input', { ref: 'userColor', className: 'color-block', type: 'color', autocomplete: true, defaultValue: '#' + Math.floor(Math.random() * 16777215).toString(16) })
				),
				_react2.default.createElement(
					'td',
					{ colSpan: '2' },
					_react2.default.createElement(
						'select',
						{ ref: 'userSelector-' + gtu.get('Id') },
						(0, _map3.default)(user, function (v, k) {
							return _react2.default.createElement(
								'optgroup',
								{ key: k, label: v[0].get('CompanyName') },
								v.map(function (u) {
									return _react2.default.createElement(
										'option',
										{ key: u.get('UserId'), value: u.get('UserId') },
										u.get('UserName'),
										' - ',
										u.get('Role')
									);
								})
							);
						})
					)
				),
				_react2.default.createElement(
					'td',
					{ className: 'text-center' },
					gtu.get('IsOnline') ? 'Online' : 'Offline'
				),
				_react2.default.createElement(
					'td',
					{ className: 'text-center' },
					_react2.default.createElement(
						'button',
						{ className: 'button tiny', onClick: self.onSave.bind(self, gtu.get('Id')) },
						'Save'
					),
					_react2.default.createElement(
						'button',
						{ className: 'button tiny alert', onClick: self.onCacnel },
						'Cancel'
					)
				)
			);
		},
		render: function render() {
			var collection = this.getCollection().where(function (i) {
				return i.get('IsAssignedToOther') == false;
			}),
			    showError = false,
			    errorMessage = '',
			    self = this;

			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'h3',
					null,
					'Assign GTU'
				),
				_react2.default.createElement(
					'div',
					{ 'data-abide-error': true, className: 'alert callout', style: { display: showError ? 'block' : 'none' } },
					_react2.default.createElement(
						'p',
						null,
						_react2.default.createElement('i', { className: 'fa fa-exclamation-circle' }),
						'\xA0',
						errorMessage
					)
				),
				_react2.default.createElement(
					'table',
					null,
					_react2.default.createElement(
						'colgroup',
						null,
						_react2.default.createElement('col', { style: { "width": "80px" } }),
						_react2.default.createElement('col', { style: { "width": "160px" } }),
						_react2.default.createElement('col', { style: { "width": "160px" } }),
						_react2.default.createElement('col', null),
						_react2.default.createElement('col', { style: { "width": "160px" } }),
						_react2.default.createElement('col', { style: { "width": "160px" } }),
						_react2.default.createElement('col', { style: { "width": "150px" } })
					),
					_react2.default.createElement(
						'thead',
						null,
						_react2.default.createElement(
							'tr',
							null,
							_react2.default.createElement(
								'th',
								{ className: 'text-center' },
								'Bag#'
							),
							_react2.default.createElement(
								'th',
								{ className: 'text-center' },
								'GTU#'
							),
							_react2.default.createElement(
								'th',
								{ className: 'text-center' },
								'Color'
							),
							_react2.default.createElement(
								'th',
								{ className: 'text-center' },
								_react2.default.createElement(
									'div',
									{ className: 'row' },
									_react2.default.createElement(
										'div',
										{ className: 'small-6 column' },
										'Team'
									),
									_react2.default.createElement(
										'div',
										{ className: 'small-6 column' },
										'Auditor'
									)
								)
							),
							_react2.default.createElement(
								'th',
								{ className: 'text-center' },
								'Role'
							),
							_react2.default.createElement(
								'th',
								{ className: 'text-center' },
								'Status'
							),
							_react2.default.createElement('th', { className: 'text-center' })
						)
					),
					_react2.default.createElement(
						'tbody',
						null,
						collection.map(function (gtu) {
							var isAssign = gtu.get('IsAssign'),
							    gtuId = gtu.get('Id'),
							    assignButton = _react2.default.createElement(
								'button',
								{ className: 'button tiny', onClick: self.onAdd.bind(null, gtuId) },
								_react2.default.createElement('i', { className: 'fa fa-plus' }),
								'\xA0Assign'
							),
							    removeButton = _react2.default.createElement(
								'button',
								{ className: 'button alert tiny', onClick: self.onRemove.bind(null, gtuId) },
								_react2.default.createElement('i', { className: 'fa fa-remove' }),
								'\xA0Remove'
							),
							    colorInput = gtu.get('UserColor') ? _react2.default.createElement('div', { className: 'color-block', style: { background: gtu.get('UserColor') } }) : null;
							var actionButton = isAssign ? removeButton : assignButton;
							if (gtu.get('Id') == self.state.editId) {
								return self.renderEditForm(gtu);
							} else {
								return _react2.default.createElement(
									'tr',
									{ key: gtu.get('Id') },
									_react2.default.createElement(
										'td',
										{ className: 'text-center' },
										gtu.get('BagId')
									),
									_react2.default.createElement(
										'td',
										{ className: 'text-center' },
										gtu.get('ShortUniqueID')
									),
									_react2.default.createElement(
										'td',
										{ className: 'text-center' },
										colorInput
									),
									_react2.default.createElement(
										'td',
										null,
										_react2.default.createElement(
											'div',
											{ className: 'row' },
											_react2.default.createElement(
												'div',
												{ className: 'small-6 column' },
												gtu.get('Company')
											),
											_react2.default.createElement(
												'div',
												{ className: 'small-6 column' },
												gtu.get('Auditor')
											)
										)
									),
									_react2.default.createElement(
										'td',
										{ className: 'text-center' },
										gtu.get('Role')
									),
									_react2.default.createElement(
										'td',
										{ className: 'text-center' },
										gtu.get('IsOnline') ? 'Online' : 'Offline'
									),
									_react2.default.createElement(
										'td',
										{ className: 'text-center' },
										actionButton
									)
								);
							}
						})
					)
				),
				_react2.default.createElement(
					'button',
					{ onClick: this.onClose, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					_react2.default.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'\xD7'
					)
				)
			);
		}
	});

/***/ },
/* 152 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _isEmpty2 = __webpack_require__(37);

	var _isEmpty3 = _interopRequireDefault(_isEmpty2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _numeral = __webpack_require__(26);

	var _numeral2 = _interopRequireDefault(_numeral);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _loading = __webpack_require__(29);

	var _loading2 = _interopRequireDefault(_loading);

	var _footer = __webpack_require__(30);

	var _footer2 = _interopRequireDefault(_footer);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				imageLoaded: null,
				imageLoading: false
			};
		},
		componentDidMount: function componentDidMount() {
			this.publish('print.map.imageloaded');
		},
		scrollToPage: function scrollToPage() {
			var model = this.getModel(),
			    height = (0, _jquery2.default)(this.refs.page).position().top;
			(0, _jquery2.default)('.off-canvas-wrapper-inner').stop().animate({
				scrollTop: height
			}, 600);
		},
		preloadImage: function preloadImage(imageAddress) {
			var def = _jquery2.default.Deferred();
			(0, _jquery2.default)(new Image()).one('load', function () {
				def.resolve();
			}).attr('src', imageAddress);
			return def;
		},
		loadImage: function loadImage() {
			var model = this.getModel(),
			    key = model.get('key'),
			    self = this;
			this.setState({
				imageLoaded: null,
				imageLoading: true
			});
			this.scrollToPage();
			model.fetchMapImage(this.props.options).then(function () {
				var def = _jquery2.default.Deferred(),
				    mapImage = model.get('MapImage'),
				    ploygonImage = model.get('PolygonImage');
				if ((0, _isEmpty3.default)(mapImage) || (0, _isEmpty3.default)(ploygonImage)) {
					def.reject();
				} else {
					_jquery2.default.when(self.preloadImage(mapImage), self.preloadImage(ploygonImage)).done(function () {
						def.resolve();
					});
				}
				return def;
			}).always(function () {
				self.setState({
					imageLoaded: true,
					imageLoading: false
				});
				self.publish('print.map.imageloaded');
			});
		},
		onReloadImage: function onReloadImage() {
			this.setState({
				imageLoaded: null,
				imageLoading: false
			});
			this.publish('print.map.imageloaded');
		},
		/**
	  * template exmaple:
	  * <div class="tips">COLOR LEGEND</div>
	  * <div>
	  *     <i class="blue"></i>
	  *     <label>Blue(0%-20%)</label>
	  *     <i class="green"></i>
	  *     <label>Green(20%-40%)</label>
	  *     <i class="yellow"></i>
	  *     <label>Yellow(40%-60%)</label>
	  *     <i class="orange"></i>
	  *     <label>Orange(60%-80%)</label>
	  *     <i class="red"></i>
	  *     <label>Red(80%-100%)</label>
	  * </div>
	  */
		formatePenetrationColor: function formatePenetrationColor(values) {
			var penetrationColorGroup = ['Blue', 'Green', 'Yellow', 'Orange', 'Red'];

			var colors = [0].concat(values),
			    min = 0,
			    legend = [];
			colors.push(100);

			for (var i = 0; i < colors.length - 1; i++) {
				if (min >= colors[i + 1]) {
					continue;
				}
				legend.push(_react2.default.createElement('i', {
					key: 'i' + i,
					className: penetrationColorGroup[i].toLowerCase()
				}));
				legend.push(_react2.default.createElement('label', {
					key: 'l' + i
				}, penetrationColorGroup[i] + '(' + colors[i] + '%-' + colors[i + 1] + '%)'));
				min = colors[i + 1];
			}
			return _react2.default.createElement('div', {
				className: 'color-legend'
			}, _react2.default.createElement('div', {
				key: 'tips',
				className: 'tips'
			}, 'COLOR LEGEND'), _react2.default.createElement('div', {
				key: 'legend'
			}, legend.map(function (i) {
				return i;
			})));
		},
		getExportParamters: function getExportParamters() {
			var model = this.getModel(),
			    options = this.props && this.props.options || {};
			return {
				'type': 'campaign',
				'options': [{
					'title': 'Campaign Summary'
				}, {
					'list': [{
						'key': 'MASTER CAMPAIGN #:',
						'text': model.get('DisplayName')
					}, {
						'key': 'CLIENT NAME:',
						'text': model.get('ClientName')
					}, {
						'key': 'CONTACT NAME:',
						'text': model.get('ContactName')
					}, {
						'key': 'TARGETING METHOD:',
						'text': options.targetMethod || ''
					}, {
						'key': 'TOTAL HOUSEHOLDS:',
						'text': (0, _numeral2.default)(model.get('TotalHouseHold')).format('0,0')
					}, {
						'key': 'TARGET HOUSEHOLDS:',
						'text': (0, _numeral2.default)(model.get('TargetHouseHold')).format('0,0')
					}, {
						'key': 'PENETRATION:',
						'text': (0, _numeral2.default)(model.get('Penetration')).format('0.00%')
					}]
				}, {
					'title': 'Campaign Summary Map'
				}, {
					'map': model.get('PolygonImage'),
					'bg': model.get('MapImage'),
					'legend': 'true'
				}]
			};
		},
		render: function render() {
			var model = this.getModel(),
			    options = this.props && this.props.options ? this.props.options : {},
			    targetMethod = options.targetMethod || '',
			    displayTotalHouseHold = (0, _numeral2.default)(model.get('TotalHouseHold')).format('0,0'),
			    displayTargetHouseHold = (0, _numeral2.default)(model.get('TargetHouseHold')).format('0,0'),
			    displayPenetration = (0, _numeral2.default)(model.get('Penetration')).format('0.00%'),
			    mapImage = model.get('MapImage'),
			    polygonImage = model.get('PolygonImage');

			if (options.showPenetrationColors) {
				var colorLegend = this.formatePenetrationColor(options.penetrationColors);
			} else {
				var colorLegend = null;
			}
			if (this.state.imageLoaded) {
				if (mapImage && polygonImage) {
					var style = {
						'backgroundImage': 'url(' + mapImage + ')',
						'backgroundRepeat': 'no-repeat',
						'backgroundSize': '100% auto',
						'backgroundPosition': '0 0'
					};
					var mapImage = _react2.default.createElement(
						'div',
						{ style: style },
						_react2.default.createElement('img', { src: polygonImage })
					);
				} else {
					var mapImage = _react2.default.createElement(
						'button',
						{ className: 'button reload', onClick: this.onReloadImage },
						_react2.default.createElement('i', { className: 'fa fa-2x fa-refresh' })
					);
				}
			} else {
				var mapImage = _react2.default.createElement(_loading2.default, { text: this.state.imageLoading ? 'LOADING' : 'WAITING' });
			}

			return _react2.default.createElement(
				'div',
				{ className: 'page campaign', ref: 'page' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						model.get('ClientName')
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row list', role: 'list' },
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'MASTER CAMPAIGN #:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('DisplayName')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'CLIENT NAME:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('ClientName')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'CONTACT NAME:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('ContactName')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TARGETING METHOD:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						targetMethod
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TOTAL HOUSEHOLDS:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						displayTotalHouseHold
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TARGET HOUSEHOLDS:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						displayTargetHouseHold
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'PENETRATION:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						displayPenetration
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'Campaign Summary Map'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'map-container', ref: model.get('key') },
							mapImage
						),
						_react2.default.createElement(
							'div',
							{ className: 'small-12 columns' },
							colorLegend,
							_react2.default.createElement('div', { className: 'direction-legend' })
						)
					)
				),
				_react2.default.createElement(_footer2.default, { model: model.get('Footer') })
			);
		}
	});

/***/ },
/* 153 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _numeral = __webpack_require__(26);

	var _numeral2 = _interopRequireDefault(_numeral);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _footer = __webpack_require__(30);

	var _footer2 = _interopRequireDefault(_footer);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getExportParamters: function getExportParamters() {
			return {
				"type": "campaign-summary",
				"options": [{
					"title": "Summary of Sub Maps"
				}, {
					"table": "submap-list"
				}]
			};
		},
		render: function render() {
			var model = this.getModel(),
			    submaps = model.get('SubMaps');

			return _react2.default.createElement(
				'div',
				{ className: 'page campaign-summary' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'Summary of Sub Maps'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'table',
							null,
							_react2.default.createElement(
								'thead',
								null,
								_react2.default.createElement(
									'tr',
									null,
									_react2.default.createElement(
										'th',
										null,
										'#'
									),
									_react2.default.createElement(
										'th',
										null,
										'SUB MAP NAME'
									),
									_react2.default.createElement(
										'th',
										null,
										'TOTAL H/H'
									),
									_react2.default.createElement(
										'th',
										null,
										'TARGET H/H'
									),
									_react2.default.createElement(
										'th',
										null,
										'PENETRATION'
									)
								)
							),
							_react2.default.createElement(
								'tbody',
								null,
								submaps.map(function (item, index) {
									var totalHouseHold = item.TotalHouseHold,
									    targetHouseHold = item.TargetHouseHold,
									    penetration = item.Penetration,
									    displayTotalHouseHold = totalHouseHold ? (0, _numeral2.default)(totalHouseHold).format('0,0') : '',
									    displayTargetHouseHold = targetHouseHold ? (0, _numeral2.default)(targetHouseHold).format('0,0') : '',
									    displayPenetration = penetration ? (0, _numeral2.default)(penetration).format('0.00%') : '';
									return _react2.default.createElement(
										'tr',
										{ key: item.OrderId },
										_react2.default.createElement(
											'td',
											null,
											item.OrderId
										),
										_react2.default.createElement(
											'td',
											null,
											item.Name
										),
										_react2.default.createElement(
											'td',
											null,
											displayTotalHouseHold
										),
										_react2.default.createElement(
											'td',
											null,
											displayTargetHouseHold
										),
										_react2.default.createElement(
											'td',
											null,
											displayPenetration
										)
									);
								})
							)
						)
					)
				),
				_react2.default.createElement(_footer2.default, { model: model.get('Footer') })
			);
		}
	});

/***/ },
/* 154 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _footer = __webpack_require__(30);

	var _footer2 = _interopRequireDefault(_footer);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getExportParamters: function getExportParamters() {
			var model = this.getModel();
			return {
				'type': 'cover',
				'options': [{
					'title': 'This Custom Campaign is Presented to:'
				}, {
					'key': '',
					'image': model.get('Logo') || '',
					'height': '80',
					'align': 'center',
					'top': '40',
					'bottom': '40'
				}, {
					'key': 'Client Name:',
					'text': model.get('ClientName')
				}, {
					'key': 'Created for:',
					'text': model.get('ContactName')
				}, {
					'key': 'Created on:',
					'text': model.get('Date') ? (0, _moment2.default)(model.get('Date')).format("MMM DD, YYYY") : ''
				}, {
					'key': ' ',
					'text': ' '
				}, {
					'key': 'Presented by:',
					'image': 'images/vargainc-logo.png',
					'height': '40',
					'align': 'center',
					'top': '10',
					'bottom': '10'
				}, {
					'key': 'Master Campaign #:',
					'text': model.get('DisplayName')
				}, {
					'key': 'Created by:',
					'text': model.get('CreatorName')
				}]

			};
		},
		render: function render() {
			var model = this.getModel(),
			    displayDate = model.get('Date') ? (0, _moment2.default)(model.get('Date')).format("MMM DD, YYYY") : '';
			if (model.get('Logo')) {
				var clientLog = _react2.default.createElement('img', { className: 'client-logo', src: model.get('Logo') });
			} else {
				var clientLog = _react2.default.createElement('div', { className: 'client-logo' });
			}

			return _react2.default.createElement(
				'div',
				{ className: 'page cover' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						_react2.default.createElement(
							'span',
							{ className: 'editable', role: 'title' },
							'This Custom Campaign is Presented to:'
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						clientLog
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Client Name:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						model.get('ClientName')
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Created for:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						model.get('ContactName')
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Created on:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						displayDate
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'\xA0'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'\xA0'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Presented by:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						_react2.default.createElement('div', { className: 'vargainc-log' })
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Master Campaign #:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						model.get('DisplayName')
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Created by:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						model.get('CreatorName')
					)
				),
				_react2.default.createElement(_footer2.default, { model: model.get('Footer') })
			);
		}
	});

/***/ },
/* 155 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		onSelectAll: function onSelectAll(e) {
			var collection = this.getCollection(),
			    value = e.currentTarget.checked;
			collection.forEach(function (item) {
				item.set('Selected', value);
			});
			this.forceUpdate();
		},
		onItemSelect: function onItemSelect(model, e) {
			model.set('Selected', e.currentTarget.checked);
			this.forceUpdate();
		},
		render: function render() {
			var dmaps = this.getCollection(),
			    btnSelectedAllStatus = dmaps.every({
				Selected: true
			}),
			    self = this;
			return _react2.default.createElement(
				'div',
				{ className: 'panel callout primary' },
				_react2.default.createElement('input', { id: 'btnCheckAllDMap', className: 'btnCheckAllDMap', type: 'checkbox',
					checked: btnSelectedAllStatus,
					onChange: this.onSelectAll }),
				_react2.default.createElement(
					'label',
					{ htmlFor: 'btnCheckAllDMap' },
					'Suppress All Distribute Maps'
				),
				_react2.default.createElement(
					'div',
					{ className: 'row small-up-1 medium-up-2 large-up-4 collapse' },
					dmaps.map(function (map) {
						return _react2.default.createElement(
							'div',
							{ className: 'column', key: 'optionDmap-' + map.get('Id') },
							_react2.default.createElement('input', { id: 'optionDmap-' + map.get('Id'), name: map.get('Id'), type: 'checkbox',
								checked: map.get('Selected'),
								onChange: self.onItemSelect.bind(null, map) }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'optionDmap-' + map.get('Id') },
								map.get('Name')
							)
						);
					})
				)
			);
		}
	});

/***/ },
/* 156 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	__webpack_require__(165);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getDefaultProps: function getDefaultProps() {
			return {
				colors: [20, 40, 60, 80]
			};
		},
		componentWillUnmount: function componentWillUnmount() {
			(0, _jquery2.default)(this.refs.colorSlider).slider("destroy");
		},
		componentDidMount: function componentDidMount() {
			var self = this;
			(0, _jquery2.default)(this.refs.colorSlider).slider({
				step: 1,
				min: 0,
				max: 100,
				values: this.props.colors,
				slide: function slide(event, ui) {
					var colorGroup = ['Blue', 'Green', 'Yellow', 'Orange', 'Red'],
					    colors = [0].concat(ui.values),
					    result = [],
					    min = 0;
					colors.push(100);
					for (var i = 0; i < colors.length - 1; i++) {
						if (min >= colors[i + 1]) {
							continue;
						}
						min = colors[i + 1];
						result.push(colorGroup[i] + ' (' + colors[i] + '% - ' + colors[i + 1] + '%) ');
					}
					(0, _jquery2.default)(this).parents('label').find('span').eq(0).text(result.join('  '));
					self.props.onChange && self.props.onChange(ui.values);
				}
			});
		},
		onReset: function onReset() {
			(0, _jquery2.default)(this.refs.colorSlider).slider('values', [20, 40, 60, 80]);
			self.props.onChange && self.props.onChange([20, 40, 60, 80]);
		},
		render: function render() {
			return _react2.default.createElement(
				'div',
				{ className: 'column' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'custom-colors small-10 columns' },
						_react2.default.createElement(
							'label',
							null,
							_react2.default.createElement(
								'span',
								null,
								'Blue (0% - 20%) Green (20% - 40%) Yellow (40% - 60%) Orange (60% - 80%) Red (80% - 100%) '
							),
							_react2.default.createElement('div', { className: 'slider', ref: 'colorSlider' })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-2 columns' },
						_react2.default.createElement(
							'button',
							{ className: 'button tiny alert', onClick: this.onReset },
							'Reset'
						)
					)
				)
			);
		}
	});

/***/ },
/* 157 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _numeral = __webpack_require__(26);

	var _numeral2 = _interopRequireDefault(_numeral);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _loading = __webpack_require__(29);

	var _loading2 = _interopRequireDefault(_loading);

	var _footer = __webpack_require__(30);

	var _footer2 = _interopRequireDefault(_footer);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				imageLoaded: null,
				imageLoading: false
			};
		},
		componentDidMount: function componentDidMount() {
			this.publish('print.map.imageloaded');
		},
		scrollToPage: function scrollToPage() {
			var model = this.getModel(),
			    height = (0, _jquery2.default)(this.refs.page).position().top;
			(0, _jquery2.default)('.off-canvas-wrapper-inner').stop().animate({
				scrollTop: height
			}, 600);
		},
		preloadImage: function preloadImage(imageAddress) {
			var def = _jquery2.default.Deferred();
			(0, _jquery2.default)(new Image()).one('load', function () {
				def.resolve();
			}).attr('src', imageAddress);
			return def;
		},
		loadImage: function loadImage() {
			var model = this.getModel(),
			    key = model.get('key'),
			    self = this;
			this.setState({
				imageLoaded: null,
				imageLoading: true
			});
			this.scrollToPage();
			model.fetchMapImage(this.props.options).then(function () {
				var def = _jquery2.default.Deferred(),
				    mapImage = model.get('MapImage'),
				    ploygonImage = model.get('PolygonImage');
				if (_.isEmpty(mapImage) || _.isEmpty(ploygonImage)) {
					def.reject();
				} else {
					_jquery2.default.when(self.preloadImage(mapImage), self.preloadImage(ploygonImage)).done(function () {
						def.resolve();
					});
				}
				return def;
			}).always(function () {
				self.setState({
					imageLoaded: true,
					imageLoading: false
				});
				self.publish('print.map.imageloaded');
			});
		},
		onReloadImage: function onReloadImage() {
			this.setState({
				imageLoaded: null,
				imageLoading: false
			});
			this.publish('print.map.imageloaded');
		},
		/**
	  * template exmaple:
	  * <div class="tips">COLOR LEGEND</div>
	  * <div>
	  *     <i class="blue"></i>
	  *     <label>Blue(0%-20%)</label>
	  *     <i class="green"></i>
	  *     <label>Green(20%-40%)</label>
	  *     <i class="yellow"></i>
	  *     <label>Yellow(40%-60%)</label>
	  *     <i class="orange"></i>
	  *     <label>Orange(60%-80%)</label>
	  *     <i class="red"></i>
	  *     <label>Red(80%-100%)</label>
	  * </div>
	  */
		formatePenetrationColor: function formatePenetrationColor(values) {
			var penetrationColorGroup = ['Blue', 'Green', 'Yellow', 'Orange', 'Red'];

			var colors = [0].concat(values),
			    min = 0,
			    legend = [];
			colors.push(100);

			for (var i = 0; i < colors.length - 1; i++) {
				if (min >= colors[i + 1]) {
					continue;
				}
				legend.push(_react2.default.createElement('i', {
					key: 'i' + i,
					className: penetrationColorGroup[i].toLowerCase()
				}));
				legend.push(_react2.default.createElement('label', {
					key: 'l' + i
				}, penetrationColorGroup[i] + '(' + colors[i] + '%-' + colors[i + 1] + '%)'));
				min = colors[i + 1];
			}
			return _react2.default.createElement('div', {
				className: 'color-legend'
			}, _react2.default.createElement('div', {
				key: 'tips',
				className: 'tips'
			}, 'COLOR LEGEND'), _react2.default.createElement('div', {
				key: 'legend'
			}, legend.map(function (i) {
				return i;
			})));
		},
		getExportParamters: function getExportParamters() {
			var model = this.getModel(),
			    options = this.props && this.props.options || {};
			return {
				'type': 'submap',
				'options': [{
					'title': 'SUB MAP ' + model.get('OrderId') + '(' + model.get('Name') + ')'
				}, {
					'list': [{
						'key': 'SUB MAP #:',
						'text': model.get('OrderId')
					}, {
						'key': 'SUB MAP NAME:',
						'text': model.get('Name')
					}, {
						'key': 'TARGETING METHOD:',
						'text': options.targetMethod || ''
					}, {
						'key': 'TOTAL HOUSEHOLDS:',
						'text': (0, _numeral2.default)(model.get('TotalHouseHold')).format('0,0')
					}, {
						'key': 'TARGET HOUSEHOLDS:',
						'text': (0, _numeral2.default)(model.get('TargetHouseHold')).format('0,0')
					}, {
						'key': 'PENETRATION:',
						'text': (0, _numeral2.default)(model.get('Penetration')).format('0.00%')
					}]
				}, {
					'title': 'Map'
				}, {
					'map': model.get('PolygonImage'),
					'bg': model.get('MapImage'),
					"legend": "true",
					"color": !options.showPenetrationColors ? null : options.penetrationColors
				}]
			};
		},
		render: function render() {
			var model = this.getModel(),
			    options = this.props && this.props.options ? this.props.options : {},
			    targetMethod = options.targetMethod || '',
			    displayTotalHouseHold = (0, _numeral2.default)(model.get('TotalHouseHold')).format('0,0'),
			    displayTargetHouseHold = (0, _numeral2.default)(model.get('TargetHouseHold')).format('0,0'),
			    displayPenetration = (0, _numeral2.default)(model.get('Penetration')).format('0.00%'),
			    mapImage = model.get('MapImage'),
			    polygonImage = model.get('PolygonImage');

			if (options.showPenetrationColors) {
				var colorLegend = this.formatePenetrationColor(options.penetrationColors);
			} else {
				var colorLegend = null;
			}

			if (this.state.imageLoaded) {
				if (mapImage && polygonImage) {
					var style = {
						'backgroundImage': 'url(' + mapImage + ')',
						'backgroundRepeat': 'no-repeat',
						'backgroundSize': '100% auto',
						'backgroundPosition': '0 0'
					};
					var mapImage = _react2.default.createElement(
						'div',
						{ style: style },
						_react2.default.createElement('img', { src: polygonImage })
					);
				} else {
					var mapImage = _react2.default.createElement(
						'button',
						{ className: 'button reload', onClick: this.onReloadImage },
						_react2.default.createElement('i', { className: 'fa fa-2x fa-refresh' })
					);
				}
			} else {
				var mapImage = _react2.default.createElement(_loading2.default, { text: this.state.imageLoading ? 'LOADING' : 'WAITING' });
			}

			return _react2.default.createElement(
				'div',
				{ className: 'page submap', ref: 'page' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'SUB MAP ',
						model.get('OrderId'),
						'(',
						model.get('Name'),
						')'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row list', role: 'list' },
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'SUB MAP #:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('OrderId')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'SUB MAP NAME:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('Name')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TARGETING METHOD:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						targetMethod
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TOTAL HOUSEHOLDS:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						displayTotalHouseHold
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TARGET HOUSEHOLDS:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						displayTargetHouseHold
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'PENETRATION:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						displayPenetration
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'Map'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'map-container', ref: model.get('key') },
							mapImage
						),
						_react2.default.createElement(
							'div',
							{ className: 'small-12 columns' },
							colorLegend,
							_react2.default.createElement('div', { className: 'direction-legend' })
						)
					)
				),
				_react2.default.createElement(_footer2.default, { model: model.get('Footer') })
			);
		}
	});

/***/ },
/* 158 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _numeral = __webpack_require__(26);

	var _numeral2 = _interopRequireDefault(_numeral);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _footer = __webpack_require__(30);

	var _footer2 = _interopRequireDefault(_footer);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		componentDidMount: function componentDidMount() {
			this.getModel().fetchBySubMap({
				quite: true
			});
		},
		getExportParamters: function getExportParamters() {
			var model = this.getModel();
			return {
				"type": "submapDetail",
				"options": [{
					"title": 'CARRIER ROUTES CONTAINED IN SUB-MAP ' + model.get('OrderId') + ' (' + model.get('Name') + ')'
				}, {
					"table": "submap-detail",
					"submapId": model.get('SubMapId')
				}]
			};
		},
		render: function render() {
			var model = this.getModel(),
			    croutes = model.get('CRoutes') ? model.get('CRoutes') : [];

			return _react2.default.createElement(
				'div',
				{ className: 'page submap-detail' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'CARRIER ROUTES CONTAINED IN SUB-MAP ',
						model.get('OrderId'),
						' (',
						model.get('Name'),
						')'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'table',
							null,
							_react2.default.createElement(
								'thead',
								null,
								_react2.default.createElement(
									'tr',
									null,
									_react2.default.createElement(
										'th',
										null,
										'#'
									),
									_react2.default.createElement(
										'th',
										null,
										'CARRIER ROUTE #'
									),
									_react2.default.createElement(
										'th',
										null,
										'TOTAL H/H'
									),
									_react2.default.createElement(
										'th',
										null,
										'TARGET H/H'
									),
									_react2.default.createElement(
										'th',
										null,
										'PENETRATION'
									)
								)
							),
							_react2.default.createElement(
								'tbody',
								null,
								croutes.map(function (item, index) {
									return _react2.default.createElement(
										'tr',
										{ key: item.Name },
										_react2.default.createElement(
											'td',
											null,
											index
										),
										_react2.default.createElement(
											'td',
											null,
											item.Name
										),
										_react2.default.createElement(
											'td',
											null,
											(0, _numeral2.default)(item.TotalHouseHold).format('0,0')
										),
										_react2.default.createElement(
											'td',
											null,
											(0, _numeral2.default)(item.TargetHouseHold).format('0,0')
										),
										_react2.default.createElement(
											'td',
											null,
											(0, _numeral2.default)(item.Penetration).format('0.00%')
										)
									);
								})
							)
						)
					)
				),
				_react2.default.createElement(_footer2.default, { model: model.get('Footer') })
			);
		}
	});

/***/ },
/* 159 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		componentDidMount: function componentDidMount() {
			var self = this,
			    model = this.getModel();

			$('#birthdayDatePicker').fdatepicker({
				format: 'yyyy-mm-dd'
			}).on('changeDate', function (e) {
				self.getModel().set('DateOfBirth', e.date);
			});

			$(this.refs.companySelector).select2();

			$('form').foundation();
		},
		componentWillUnmount: function componentWillUnmount() {
			$(companySelector).select2('destroy');
			$('#birthdayDatePicker').off('changeDate').fdatepicker('remove');
		},
		onSave: function onSave(e) {
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel(),
			    file = this.refs.employeePicture.files.length > 0 ? this.refs.employeePicture.files[0] : null,
			    self = this;
			model.set('CompanyId', $(this.refs.companySelector).val());
			model.addEmployee(file).done(function () {
				self.publish("showDialog");
			});
		},
		onClose: function onClose() {
			this.publish("showDialog");
		},
		onChange: function onChange(e) {
			this.getModel().set(e.currentTarget.name, e.currentTarget.value);
		},
		render: function render() {
			var showError = this.state && this.state.error ? true : false;
			var errorMessage = showError ? this.state.error : "";
			return _react2.default.createElement(
				'form',
				{ 'data-abide': true, onSubmit: this.onSave },
				_react2.default.createElement(
					'h3',
					null,
					'Add Employee'
				),
				_react2.default.createElement(
					'div',
					{ 'data-abide-error': true, className: 'alert callout', style: { display: showError ? 'block' : 'none' } },
					_react2.default.createElement(
						'p',
						null,
						_react2.default.createElement('i', { className: 'fa fa-exclamation-circle' }),
						'\xA0',
						errorMessage
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Distributor'
						),
						_react2.default.createElement(
							'select',
							{ ref: 'companySelector' },
							this.props.company.models.map(function (item) {
								return _react2.default.createElement(
									'option',
									{ key: item.get('Id'), value: item.get('Id') },
									item.get('Name')
								);
							})
						),
						_react2.default.createElement(
							'span',
							{ className: 'form-error' },
							'it is required.'
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Full Name',
							_react2.default.createElement('input', { onChange: this.onChange, name: 'FullName', type: 'text', required: true })
						)
					),
					_react2.default.createElement(
						'fieldset',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Role'
						),
						_react2.default.createElement('input', { type: 'radio', onChange: this.onChange, name: 'Role', value: 'Walker', id: 'walker' }),
						_react2.default.createElement(
							'label',
							{ htmlFor: 'walker' },
							'Walker'
						),
						_react2.default.createElement('input', { type: 'radio', onChange: this.onChange, name: 'Role', value: 'Driver', id: 'driver' }),
						_react2.default.createElement(
							'label',
							{ htmlFor: 'driver' },
							'Driver'
						),
						_react2.default.createElement('input', { type: 'radio', onChange: this.onChange, name: 'Role', value: 'Auditor', id: 'autitor' }),
						_react2.default.createElement(
							'label',
							{ htmlFor: 'autitor' },
							'Auditor'
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Cell Phone',
							_react2.default.createElement('input', { onChange: this.onChange, name: 'CellPhone', type: 'text' })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 medium-12 large-4 columns end' },
						_react2.default.createElement(
							'label',
							null,
							'Birthday',
							_react2.default.createElement('input', { id: 'birthdayDatePicker', className: 'fdatepicker', onChange: this.onChange, name: 'DateOfBirth', type: 'date', readOnly: true })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Photo',
							_react2.default.createElement('input', { ref: 'employeePicture', name: 'picture', type: 'file' })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Notes',
							_react2.default.createElement('textarea', { onChange: this.onChange, name: 'Notes' })
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'button-group float-right' },
						_react2.default.createElement(
							'button',
							{ type: 'submit', className: 'success button' },
							'Save'
						),
						_react2.default.createElement(
							'a',
							{ href: 'javascript:;', className: 'button', onClick: this.onClose },
							'Cancel'
						)
					)
				),
				_react2.default.createElement(
					'button',
					{ onClick: this.onClose, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					_react2.default.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'\xD7'
					)
				)
			);
		}
	});

/***/ },
/* 160 */,
/* 161 */,
/* 162 */,
/* 163 */,
/* 164 */,
/* 165 */,
/* 166 */,
/* 167 */,
/* 168 */,
/* 169 */,
/* 170 */
/***/ function(module, exports, __webpack_require__) {

	var getNative = __webpack_require__(34),
	    root = __webpack_require__(22);

	/* Built-in method references that are verified to be native. */
	var Set = getNative(root, 'Set');

	module.exports = Set;


/***/ },
/* 171 */
/***/ function(module, exports, __webpack_require__) {

	var root = __webpack_require__(22);

	/** Built-in value references. */
	var Uint8Array = root.Uint8Array;

	module.exports = Uint8Array;


/***/ },
/* 172 */
/***/ function(module, exports) {

	/**
	 * A specialized version of `_.forEach` for arrays without support for
	 * iteratee shorthands.
	 *
	 * @private
	 * @param {Array} [array] The array to iterate over.
	 * @param {Function} iteratee The function invoked per iteration.
	 * @returns {Array} Returns `array`.
	 */
	function arrayEach(array, iteratee) {
	  var index = -1,
	      length = array == null ? 0 : array.length;

	  while (++index < length) {
	    if (iteratee(array[index], index, array) === false) {
	      break;
	    }
	  }
	  return array;
	}

	module.exports = arrayEach;


/***/ },
/* 173 */
/***/ function(module, exports) {

	/**
	 * A specialized version of `_.filter` for arrays without support for
	 * iteratee shorthands.
	 *
	 * @private
	 * @param {Array} [array] The array to iterate over.
	 * @param {Function} predicate The function invoked per iteration.
	 * @returns {Array} Returns the new filtered array.
	 */
	function arrayFilter(array, predicate) {
	  var index = -1,
	      length = array == null ? 0 : array.length,
	      resIndex = 0,
	      result = [];

	  while (++index < length) {
	    var value = array[index];
	    if (predicate(value, index, array)) {
	      result[resIndex++] = value;
	    }
	  }
	  return result;
	}

	module.exports = arrayFilter;


/***/ },
/* 174 */
/***/ function(module, exports, __webpack_require__) {

	var baseIndexOf = __webpack_require__(80);

	/**
	 * A specialized version of `_.includes` for arrays without support for
	 * specifying an index to search from.
	 *
	 * @private
	 * @param {Array} [array] The array to inspect.
	 * @param {*} target The value to search for.
	 * @returns {boolean} Returns `true` if `target` is found, else `false`.
	 */
	function arrayIncludes(array, value) {
	  var length = array == null ? 0 : array.length;
	  return !!length && baseIndexOf(array, value, 0) > -1;
	}

	module.exports = arrayIncludes;


/***/ },
/* 175 */
/***/ function(module, exports) {

	/**
	 * This function is like `arrayIncludes` except that it accepts a comparator.
	 *
	 * @private
	 * @param {Array} [array] The array to inspect.
	 * @param {*} target The value to search for.
	 * @param {Function} comparator The comparator invoked per element.
	 * @returns {boolean} Returns `true` if `target` is found, else `false`.
	 */
	function arrayIncludesWith(array, value, comparator) {
	  var index = -1,
	      length = array == null ? 0 : array.length;

	  while (++index < length) {
	    if (comparator(value, array[index])) {
	      return true;
	    }
	  }
	  return false;
	}

	module.exports = arrayIncludesWith;


/***/ },
/* 176 */
/***/ function(module, exports, __webpack_require__) {

	var baseTimes = __webpack_require__(447),
	    isArguments = __webpack_require__(86),
	    isArray = __webpack_require__(12),
	    isBuffer = __webpack_require__(87),
	    isIndex = __webpack_require__(83),
	    isTypedArray = __webpack_require__(122);

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * Creates an array of the enumerable property names of the array-like `value`.
	 *
	 * @private
	 * @param {*} value The value to query.
	 * @param {boolean} inherited Specify returning inherited property names.
	 * @returns {Array} Returns the array of property names.
	 */
	function arrayLikeKeys(value, inherited) {
	  var isArr = isArray(value),
	      isArg = !isArr && isArguments(value),
	      isBuff = !isArr && !isArg && isBuffer(value),
	      isType = !isArr && !isArg && !isBuff && isTypedArray(value),
	      skipIndexes = isArr || isArg || isBuff || isType,
	      result = skipIndexes ? baseTimes(value.length, String) : [],
	      length = result.length;

	  for (var key in value) {
	    if ((inherited || hasOwnProperty.call(value, key)) &&
	        !(skipIndexes && (
	           // Safari 9 has enumerable `arguments.length` in strict mode.
	           key == 'length' ||
	           // Node.js 0.10 has enumerable non-index properties on buffers.
	           (isBuff && (key == 'offset' || key == 'parent')) ||
	           // PhantomJS 2 has enumerable non-index properties on typed arrays.
	           (isType && (key == 'buffer' || key == 'byteLength' || key == 'byteOffset')) ||
	           // Skip index properties.
	           isIndex(key, length)
	        ))) {
	      result.push(key);
	    }
	  }
	  return result;
	}

	module.exports = arrayLikeKeys;


/***/ },
/* 177 */
/***/ function(module, exports) {

	/**
	 * A specialized version of `_.reduce` for arrays without support for
	 * iteratee shorthands.
	 *
	 * @private
	 * @param {Array} [array] The array to iterate over.
	 * @param {Function} iteratee The function invoked per iteration.
	 * @param {*} [accumulator] The initial value.
	 * @param {boolean} [initAccum] Specify using the first element of `array` as
	 *  the initial value.
	 * @returns {*} Returns the accumulated value.
	 */
	function arrayReduce(array, iteratee, accumulator, initAccum) {
	  var index = -1,
	      length = array == null ? 0 : array.length;

	  if (initAccum && length) {
	    accumulator = array[++index];
	  }
	  while (++index < length) {
	    accumulator = iteratee(accumulator, array[index], index, array);
	  }
	  return accumulator;
	}

	module.exports = arrayReduce;


/***/ },
/* 178 */
/***/ function(module, exports) {

	/**
	 * A specialized version of `_.some` for arrays without support for iteratee
	 * shorthands.
	 *
	 * @private
	 * @param {Array} [array] The array to iterate over.
	 * @param {Function} predicate The function invoked per iteration.
	 * @returns {boolean} Returns `true` if any element passes the predicate check,
	 *  else `false`.
	 */
	function arraySome(array, predicate) {
	  var index = -1,
	      length = array == null ? 0 : array.length;

	  while (++index < length) {
	    if (predicate(array[index], index, array)) {
	      return true;
	    }
	  }
	  return false;
	}

	module.exports = arraySome;


/***/ },
/* 179 */
/***/ function(module, exports, __webpack_require__) {

	var SetCache = __webpack_require__(107),
	    arrayIncludes = __webpack_require__(174),
	    arrayIncludesWith = __webpack_require__(175),
	    arrayMap = __webpack_require__(31),
	    baseUnary = __webpack_require__(81),
	    cacheHas = __webpack_require__(111);

	/** Used as the size to enable large array optimizations. */
	var LARGE_ARRAY_SIZE = 200;

	/**
	 * The base implementation of methods like `_.difference` without support
	 * for excluding multiple arrays or iteratee shorthands.
	 *
	 * @private
	 * @param {Array} array The array to inspect.
	 * @param {Array} values The values to exclude.
	 * @param {Function} [iteratee] The iteratee invoked per element.
	 * @param {Function} [comparator] The comparator invoked per element.
	 * @returns {Array} Returns the new array of filtered values.
	 */
	function baseDifference(array, values, iteratee, comparator) {
	  var index = -1,
	      includes = arrayIncludes,
	      isCommon = true,
	      length = array.length,
	      result = [],
	      valuesLength = values.length;

	  if (!length) {
	    return result;
	  }
	  if (iteratee) {
	    values = arrayMap(values, baseUnary(iteratee));
	  }
	  if (comparator) {
	    includes = arrayIncludesWith;
	    isCommon = false;
	  }
	  else if (values.length >= LARGE_ARRAY_SIZE) {
	    includes = cacheHas;
	    isCommon = false;
	    values = new SetCache(values);
	  }
	  outer:
	  while (++index < length) {
	    var value = array[index],
	        computed = iteratee == null ? value : iteratee(value);

	    value = (comparator || value !== 0) ? value : 0;
	    if (isCommon && computed === computed) {
	      var valuesIndex = valuesLength;
	      while (valuesIndex--) {
	        if (values[valuesIndex] === computed) {
	          continue outer;
	        }
	      }
	      result.push(value);
	    }
	    else if (!includes(values, computed, comparator)) {
	      result.push(value);
	    }
	  }
	  return result;
	}

	module.exports = baseDifference;


/***/ },
/* 180 */
/***/ function(module, exports) {

	/**
	 * The base implementation of `_.findIndex` and `_.findLastIndex` without
	 * support for iteratee shorthands.
	 *
	 * @private
	 * @param {Array} array The array to inspect.
	 * @param {Function} predicate The function invoked per iteration.
	 * @param {number} fromIndex The index to search from.
	 * @param {boolean} [fromRight] Specify iterating from right to left.
	 * @returns {number} Returns the index of the matched value, else `-1`.
	 */
	function baseFindIndex(array, predicate, fromIndex, fromRight) {
	  var length = array.length,
	      index = fromIndex + (fromRight ? 1 : -1);

	  while ((fromRight ? index-- : ++index < length)) {
	    if (predicate(array[index], index, array)) {
	      return index;
	    }
	  }
	  return -1;
	}

	module.exports = baseFindIndex;


/***/ },
/* 181 */
/***/ function(module, exports, __webpack_require__) {

	var arrayPush = __webpack_require__(76),
	    isArray = __webpack_require__(12);

	/**
	 * The base implementation of `getAllKeys` and `getAllKeysIn` which uses
	 * `keysFunc` and `symbolsFunc` to get the enumerable property names and
	 * symbols of `object`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @param {Function} keysFunc The function to get the keys of `object`.
	 * @param {Function} symbolsFunc The function to get the symbols of `object`.
	 * @returns {Array} Returns the array of property names and symbols.
	 */
	function baseGetAllKeys(object, keysFunc, symbolsFunc) {
	  var result = keysFunc(object);
	  return isArray(object) ? result : arrayPush(result, symbolsFunc(object));
	}

	module.exports = baseGetAllKeys;


/***/ },
/* 182 */
/***/ function(module, exports, __webpack_require__) {

	var baseIsEqualDeep = __webpack_require__(430),
	    isObject = __webpack_require__(20),
	    isObjectLike = __webpack_require__(38);

	/**
	 * The base implementation of `_.isEqual` which supports partial comparisons
	 * and tracks traversed objects.
	 *
	 * @private
	 * @param {*} value The value to compare.
	 * @param {*} other The other value to compare.
	 * @param {boolean} bitmask The bitmask flags.
	 *  1 - Unordered comparison
	 *  2 - Partial comparison
	 * @param {Function} [customizer] The function to customize comparisons.
	 * @param {Object} [stack] Tracks traversed `value` and `other` objects.
	 * @returns {boolean} Returns `true` if the values are equivalent, else `false`.
	 */
	function baseIsEqual(value, other, bitmask, customizer, stack) {
	  if (value === other) {
	    return true;
	  }
	  if (value == null || other == null || (!isObject(value) && !isObjectLike(other))) {
	    return value !== value && other !== other;
	  }
	  return baseIsEqualDeep(value, other, bitmask, customizer, baseIsEqual, stack);
	}

	module.exports = baseIsEqual;


/***/ },
/* 183 */
/***/ function(module, exports, __webpack_require__) {

	var isPrototype = __webpack_require__(57),
	    nativeKeys = __webpack_require__(499);

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * The base implementation of `_.keys` which doesn't treat sparse arrays as dense.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of property names.
	 */
	function baseKeys(object) {
	  if (!isPrototype(object)) {
	    return nativeKeys(object);
	  }
	  var result = [];
	  for (var key in Object(object)) {
	    if (hasOwnProperty.call(object, key) && key != 'constructor') {
	      result.push(key);
	    }
	  }
	  return result;
	}

	module.exports = baseKeys;


/***/ },
/* 184 */
/***/ function(module, exports, __webpack_require__) {

	var baseEach = __webpack_require__(54),
	    isArrayLike = __webpack_require__(25);

	/**
	 * The base implementation of `_.map` without support for iteratee shorthands.
	 *
	 * @private
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} iteratee The function invoked per iteration.
	 * @returns {Array} Returns the new mapped array.
	 */
	function baseMap(collection, iteratee) {
	  var index = -1,
	      result = isArrayLike(collection) ? Array(collection.length) : [];

	  baseEach(collection, function(value, key, collection) {
	    result[++index] = iteratee(value, key, collection);
	  });
	  return result;
	}

	module.exports = baseMap;


/***/ },
/* 185 */
/***/ function(module, exports, __webpack_require__) {

	var arrayMap = __webpack_require__(31),
	    baseIteratee = __webpack_require__(32),
	    baseMap = __webpack_require__(184),
	    baseSortBy = __webpack_require__(446),
	    baseUnary = __webpack_require__(81),
	    compareMultiple = __webpack_require__(461),
	    identity = __webpack_require__(60);

	/**
	 * The base implementation of `_.orderBy` without param guards.
	 *
	 * @private
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function[]|Object[]|string[]} iteratees The iteratees to sort by.
	 * @param {string[]} orders The sort orders of `iteratees`.
	 * @returns {Array} Returns the new sorted array.
	 */
	function baseOrderBy(collection, iteratees, orders) {
	  var index = -1;
	  iteratees = arrayMap(iteratees.length ? iteratees : [identity], baseUnary(baseIteratee));

	  var result = baseMap(collection, function(value, key, collection) {
	    var criteria = arrayMap(iteratees, function(iteratee) {
	      return iteratee(value);
	    });
	    return { 'criteria': criteria, 'index': ++index, 'value': value };
	  });

	  return baseSortBy(result, function(object, other) {
	    return compareMultiple(object, other, orders);
	  });
	}

	module.exports = baseOrderBy;


/***/ },
/* 186 */
/***/ function(module, exports) {

	/**
	 * The base implementation of `_.slice` without an iteratee call guard.
	 *
	 * @private
	 * @param {Array} array The array to slice.
	 * @param {number} [start=0] The start position.
	 * @param {number} [end=array.length] The end position.
	 * @returns {Array} Returns the slice of `array`.
	 */
	function baseSlice(array, start, end) {
	  var index = -1,
	      length = array.length;

	  if (start < 0) {
	    start = -start > length ? 0 : (length + start);
	  }
	  end = end > length ? length : end;
	  if (end < 0) {
	    end += length;
	  }
	  length = start > end ? 0 : ((end - start) >>> 0);
	  start >>>= 0;

	  var result = Array(length);
	  while (++index < length) {
	    result[index] = array[index + start];
	  }
	  return result;
	}

	module.exports = baseSlice;


/***/ },
/* 187 */
/***/ function(module, exports, __webpack_require__) {

	var Symbol = __webpack_require__(43),
	    arrayMap = __webpack_require__(31),
	    isArray = __webpack_require__(12),
	    isSymbol = __webpack_require__(61);

	/** Used as references for various `Number` constants. */
	var INFINITY = 1 / 0;

	/** Used to convert symbols to primitives and strings. */
	var symbolProto = Symbol ? Symbol.prototype : undefined,
	    symbolToString = symbolProto ? symbolProto.toString : undefined;

	/**
	 * The base implementation of `_.toString` which doesn't convert nullish
	 * values to empty strings.
	 *
	 * @private
	 * @param {*} value The value to process.
	 * @returns {string} Returns the string.
	 */
	function baseToString(value) {
	  // Exit early for strings to avoid a performance hit in some environments.
	  if (typeof value == 'string') {
	    return value;
	  }
	  if (isArray(value)) {
	    // Recursively convert values (susceptible to call stack limits).
	    return arrayMap(value, baseToString) + '';
	  }
	  if (isSymbol(value)) {
	    return symbolToString ? symbolToString.call(value) : '';
	  }
	  var result = (value + '');
	  return (result == '0' && (1 / value) == -INFINITY) ? '-0' : result;
	}

	module.exports = baseToString;


/***/ },
/* 188 */
/***/ function(module, exports, __webpack_require__) {

	var SetCache = __webpack_require__(107),
	    arrayIncludes = __webpack_require__(174),
	    arrayIncludesWith = __webpack_require__(175),
	    cacheHas = __webpack_require__(111),
	    createSet = __webpack_require__(469),
	    setToArray = __webpack_require__(85);

	/** Used as the size to enable large array optimizations. */
	var LARGE_ARRAY_SIZE = 200;

	/**
	 * The base implementation of `_.uniqBy` without support for iteratee shorthands.
	 *
	 * @private
	 * @param {Array} array The array to inspect.
	 * @param {Function} [iteratee] The iteratee invoked per element.
	 * @param {Function} [comparator] The comparator invoked per element.
	 * @returns {Array} Returns the new duplicate free array.
	 */
	function baseUniq(array, iteratee, comparator) {
	  var index = -1,
	      includes = arrayIncludes,
	      length = array.length,
	      isCommon = true,
	      result = [],
	      seen = result;

	  if (comparator) {
	    isCommon = false;
	    includes = arrayIncludesWith;
	  }
	  else if (length >= LARGE_ARRAY_SIZE) {
	    var set = iteratee ? null : createSet(array);
	    if (set) {
	      return setToArray(set);
	    }
	    isCommon = false;
	    includes = cacheHas;
	    seen = new SetCache;
	  }
	  else {
	    seen = iteratee ? [] : result;
	  }
	  outer:
	  while (++index < length) {
	    var value = array[index],
	        computed = iteratee ? iteratee(value) : value;

	    value = (comparator || value !== 0) ? value : 0;
	    if (isCommon && computed === computed) {
	      var seenIndex = seen.length;
	      while (seenIndex--) {
	        if (seen[seenIndex] === computed) {
	          continue outer;
	        }
	      }
	      if (iteratee) {
	        seen.push(computed);
	      }
	      result.push(value);
	    }
	    else if (!includes(seen, computed, comparator)) {
	      if (seen !== result) {
	        seen.push(computed);
	      }
	      result.push(value);
	    }
	  }
	  return result;
	}

	module.exports = baseUniq;


/***/ },
/* 189 */
/***/ function(module, exports, __webpack_require__) {

	var castPath = __webpack_require__(45),
	    last = __webpack_require__(525),
	    parent = __webpack_require__(503),
	    toKey = __webpack_require__(46);

	/**
	 * The base implementation of `_.unset`.
	 *
	 * @private
	 * @param {Object} object The object to modify.
	 * @param {Array|string} path The property path to unset.
	 * @returns {boolean} Returns `true` if the property is deleted, else `false`.
	 */
	function baseUnset(object, path) {
	  path = castPath(path, object);
	  object = parent(object, path);
	  return object == null || delete object[toKey(last(path))];
	}

	module.exports = baseUnset;


/***/ },
/* 190 */
/***/ function(module, exports, __webpack_require__) {

	var baseRest = __webpack_require__(56),
	    isIterateeCall = __webpack_require__(116);

	/**
	 * Creates a function like `_.assign`.
	 *
	 * @private
	 * @param {Function} assigner The function to assign values.
	 * @returns {Function} Returns the new assigner function.
	 */
	function createAssigner(assigner) {
	  return baseRest(function(object, sources) {
	    var index = -1,
	        length = sources.length,
	        customizer = length > 1 ? sources[length - 1] : undefined,
	        guard = length > 2 ? sources[2] : undefined;

	    customizer = (assigner.length > 3 && typeof customizer == 'function')
	      ? (length--, customizer)
	      : undefined;

	    if (guard && isIterateeCall(sources[0], sources[1], guard)) {
	      customizer = length < 3 ? undefined : customizer;
	      length = 1;
	    }
	    object = Object(object);
	    while (++index < length) {
	      var source = sources[index];
	      if (source) {
	        assigner(object, source, index, customizer);
	      }
	    }
	    return object;
	  });
	}

	module.exports = createAssigner;


/***/ },
/* 191 */
/***/ function(module, exports, __webpack_require__) {

	var getNative = __webpack_require__(34);

	var defineProperty = (function() {
	  try {
	    var func = getNative(Object, 'defineProperty');
	    func({}, '', {});
	    return func;
	  } catch (e) {}
	}());

	module.exports = defineProperty;


/***/ },
/* 192 */
/***/ function(module, exports, __webpack_require__) {

	var SetCache = __webpack_require__(107),
	    arraySome = __webpack_require__(178),
	    cacheHas = __webpack_require__(111);

	/** Used to compose bitmasks for value comparisons. */
	var COMPARE_PARTIAL_FLAG = 1,
	    COMPARE_UNORDERED_FLAG = 2;

	/**
	 * A specialized version of `baseIsEqualDeep` for arrays with support for
	 * partial deep comparisons.
	 *
	 * @private
	 * @param {Array} array The array to compare.
	 * @param {Array} other The other array to compare.
	 * @param {number} bitmask The bitmask flags. See `baseIsEqual` for more details.
	 * @param {Function} customizer The function to customize comparisons.
	 * @param {Function} equalFunc The function to determine equivalents of values.
	 * @param {Object} stack Tracks traversed `array` and `other` objects.
	 * @returns {boolean} Returns `true` if the arrays are equivalent, else `false`.
	 */
	function equalArrays(array, other, bitmask, customizer, equalFunc, stack) {
	  var isPartial = bitmask & COMPARE_PARTIAL_FLAG,
	      arrLength = array.length,
	      othLength = other.length;

	  if (arrLength != othLength && !(isPartial && othLength > arrLength)) {
	    return false;
	  }
	  // Assume cyclic values are equal.
	  var stacked = stack.get(array);
	  if (stacked && stack.get(other)) {
	    return stacked == other;
	  }
	  var index = -1,
	      result = true,
	      seen = (bitmask & COMPARE_UNORDERED_FLAG) ? new SetCache : undefined;

	  stack.set(array, other);
	  stack.set(other, array);

	  // Ignore non-index properties.
	  while (++index < arrLength) {
	    var arrValue = array[index],
	        othValue = other[index];

	    if (customizer) {
	      var compared = isPartial
	        ? customizer(othValue, arrValue, index, other, array, stack)
	        : customizer(arrValue, othValue, index, array, other, stack);
	    }
	    if (compared !== undefined) {
	      if (compared) {
	        continue;
	      }
	      result = false;
	      break;
	    }
	    // Recursively compare arrays (susceptible to call stack limits).
	    if (seen) {
	      if (!arraySome(other, function(othValue, othIndex) {
	            if (!cacheHas(seen, othIndex) &&
	                (arrValue === othValue || equalFunc(arrValue, othValue, bitmask, customizer, stack))) {
	              return seen.push(othIndex);
	            }
	          })) {
	        result = false;
	        break;
	      }
	    } else if (!(
	          arrValue === othValue ||
	            equalFunc(arrValue, othValue, bitmask, customizer, stack)
	        )) {
	      result = false;
	      break;
	    }
	  }
	  stack['delete'](array);
	  stack['delete'](other);
	  return result;
	}

	module.exports = equalArrays;


/***/ },
/* 193 */
/***/ function(module, exports, __webpack_require__) {

	var flatten = __webpack_require__(522),
	    overRest = __webpack_require__(202),
	    setToString = __webpack_require__(203);

	/**
	 * A specialized version of `baseRest` which flattens the rest array.
	 *
	 * @private
	 * @param {Function} func The function to apply a rest parameter to.
	 * @returns {Function} Returns the new function.
	 */
	function flatRest(func) {
	  return setToString(overRest(func, undefined, flatten), func + '');
	}

	module.exports = flatRest;


/***/ },
/* 194 */
/***/ function(module, exports) {

	/* WEBPACK VAR INJECTION */(function(global) {/** Detect free variable `global` from Node.js. */
	var freeGlobal = typeof global == 'object' && global && global.Object === Object && global;

	module.exports = freeGlobal;

	/* WEBPACK VAR INJECTION */}.call(exports, (function() { return this; }())))

/***/ },
/* 195 */
/***/ function(module, exports, __webpack_require__) {

	var baseGetAllKeys = __webpack_require__(181),
	    getSymbolsIn = __webpack_require__(197),
	    keysIn = __webpack_require__(123);

	/**
	 * Creates an array of own and inherited enumerable property names and
	 * symbols of `object`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of property names and symbols.
	 */
	function getAllKeysIn(object) {
	  return baseGetAllKeys(object, keysIn, getSymbolsIn);
	}

	module.exports = getAllKeysIn;


/***/ },
/* 196 */
/***/ function(module, exports, __webpack_require__) {

	var overArg = __webpack_require__(118);

	/** Built-in value references. */
	var getPrototype = overArg(Object.getPrototypeOf, Object);

	module.exports = getPrototype;


/***/ },
/* 197 */
/***/ function(module, exports, __webpack_require__) {

	var arrayPush = __webpack_require__(76),
	    getPrototype = __webpack_require__(196),
	    getSymbols = __webpack_require__(114),
	    stubArray = __webpack_require__(211);

	/* Built-in method references for those with the same name as other `lodash` methods. */
	var nativeGetSymbols = Object.getOwnPropertySymbols;

	/**
	 * Creates an array of the own and inherited enumerable symbols of `object`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of symbols.
	 */
	var getSymbolsIn = !nativeGetSymbols ? stubArray : function(object) {
	  var result = [];
	  while (object) {
	    arrayPush(result, getSymbols(object));
	    object = getPrototype(object);
	  }
	  return result;
	};

	module.exports = getSymbolsIn;


/***/ },
/* 198 */
/***/ function(module, exports, __webpack_require__) {

	var castPath = __webpack_require__(45),
	    isArguments = __webpack_require__(86),
	    isArray = __webpack_require__(12),
	    isIndex = __webpack_require__(83),
	    isLength = __webpack_require__(121),
	    toKey = __webpack_require__(46);

	/**
	 * Checks if `path` exists on `object`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @param {Array|string} path The path to check.
	 * @param {Function} hasFunc The function to check properties.
	 * @returns {boolean} Returns `true` if `path` exists, else `false`.
	 */
	function hasPath(object, path, hasFunc) {
	  path = castPath(path, object);

	  var index = -1,
	      length = path.length,
	      result = false;

	  while (++index < length) {
	    var key = toKey(path[index]);
	    if (!(result = object != null && hasFunc(object, key))) {
	      break;
	    }
	    object = object[key];
	  }
	  if (result || ++index != length) {
	    return result;
	  }
	  length = object == null ? 0 : object.length;
	  return !!length && isLength(length) && isIndex(key, length) &&
	    (isArray(object) || isArguments(object));
	}

	module.exports = hasPath;


/***/ },
/* 199 */
/***/ function(module, exports, __webpack_require__) {

	var isObject = __webpack_require__(20);

	/**
	 * Checks if `value` is suitable for strict equality comparisons, i.e. `===`.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` if suitable for strict
	 *  equality comparisons, else `false`.
	 */
	function isStrictComparable(value) {
	  return value === value && !isObject(value);
	}

	module.exports = isStrictComparable;


/***/ },
/* 200 */
/***/ function(module, exports) {

	/**
	 * Converts `map` to its key-value pairs.
	 *
	 * @private
	 * @param {Object} map The map to convert.
	 * @returns {Array} Returns the key-value pairs.
	 */
	function mapToArray(map) {
	  var index = -1,
	      result = Array(map.size);

	  map.forEach(function(value, key) {
	    result[++index] = [key, value];
	  });
	  return result;
	}

	module.exports = mapToArray;


/***/ },
/* 201 */
/***/ function(module, exports) {

	/**
	 * A specialized version of `matchesProperty` for source values suitable
	 * for strict equality comparisons, i.e. `===`.
	 *
	 * @private
	 * @param {string} key The key of the property to get.
	 * @param {*} srcValue The value to match.
	 * @returns {Function} Returns the new spec function.
	 */
	function matchesStrictComparable(key, srcValue) {
	  return function(object) {
	    if (object == null) {
	      return false;
	    }
	    return object[key] === srcValue &&
	      (srcValue !== undefined || (key in Object(object)));
	  };
	}

	module.exports = matchesStrictComparable;


/***/ },
/* 202 */
/***/ function(module, exports, __webpack_require__) {

	var apply = __webpack_require__(416);

	/* Built-in method references for those with the same name as other `lodash` methods. */
	var nativeMax = Math.max;

	/**
	 * A specialized version of `baseRest` which transforms the rest array.
	 *
	 * @private
	 * @param {Function} func The function to apply a rest parameter to.
	 * @param {number} [start=func.length-1] The start position of the rest parameter.
	 * @param {Function} transform The rest array transform.
	 * @returns {Function} Returns the new function.
	 */
	function overRest(func, start, transform) {
	  start = nativeMax(start === undefined ? (func.length - 1) : start, 0);
	  return function() {
	    var args = arguments,
	        index = -1,
	        length = nativeMax(args.length - start, 0),
	        array = Array(length);

	    while (++index < length) {
	      array[index] = args[start + index];
	    }
	    index = -1;
	    var otherArgs = Array(start + 1);
	    while (++index < start) {
	      otherArgs[index] = args[index];
	    }
	    otherArgs[start] = transform(array);
	    return apply(func, this, otherArgs);
	  };
	}

	module.exports = overRest;


/***/ },
/* 203 */
/***/ function(module, exports, __webpack_require__) {

	var baseSetToString = __webpack_require__(444),
	    shortOut = __webpack_require__(506);

	/**
	 * Sets the `toString` method of `func` to return `string`.
	 *
	 * @private
	 * @param {Function} func The function to modify.
	 * @param {Function} string The `toString` result.
	 * @returns {Function} Returns `func`.
	 */
	var setToString = shortOut(baseSetToString);

	module.exports = setToString;


/***/ },
/* 204 */
/***/ function(module, exports) {

	/** Used for built-in method references. */
	var funcProto = Function.prototype;

	/** Used to resolve the decompiled source of functions. */
	var funcToString = funcProto.toString;

	/**
	 * Converts `func` to its source code.
	 *
	 * @private
	 * @param {Function} func The function to convert.
	 * @returns {string} Returns the source code.
	 */
	function toSource(func) {
	  if (func != null) {
	    try {
	      return funcToString.call(func);
	    } catch (e) {}
	    try {
	      return (func + '');
	    } catch (e) {}
	  }
	  return '';
	}

	module.exports = toSource;


/***/ },
/* 205 */
/***/ function(module, exports, __webpack_require__) {

	var createFind = __webpack_require__(468),
	    findIndex = __webpack_require__(521);

	/**
	 * Iterates over elements of `collection`, returning the first element
	 * `predicate` returns truthy for. The predicate is invoked with three
	 * arguments: (value, index|key, collection).
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Collection
	 * @param {Array|Object} collection The collection to inspect.
	 * @param {Function} [predicate=_.identity] The function invoked per iteration.
	 * @param {number} [fromIndex=0] The index to search from.
	 * @returns {*} Returns the matched element, else `undefined`.
	 * @example
	 *
	 * var users = [
	 *   { 'user': 'barney',  'age': 36, 'active': true },
	 *   { 'user': 'fred',    'age': 40, 'active': false },
	 *   { 'user': 'pebbles', 'age': 1,  'active': true }
	 * ];
	 *
	 * _.find(users, function(o) { return o.age < 40; });
	 * // => object for 'barney'
	 *
	 * // The `_.matches` iteratee shorthand.
	 * _.find(users, { 'age': 1, 'active': true });
	 * // => object for 'pebbles'
	 *
	 * // The `_.matchesProperty` iteratee shorthand.
	 * _.find(users, ['active', false]);
	 * // => object for 'fred'
	 *
	 * // The `_.property` iteratee shorthand.
	 * _.find(users, 'active');
	 * // => object for 'barney'
	 */
	var find = createFind(findIndex);

	module.exports = find;


/***/ },
/* 206 */
/***/ function(module, exports, __webpack_require__) {

	var baseAssignValue = __webpack_require__(109),
	    createAggregator = __webpack_require__(465);

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * Creates an object composed of keys generated from the results of running
	 * each element of `collection` thru `iteratee`. The order of grouped values
	 * is determined by the order they occur in `collection`. The corresponding
	 * value of each key is an array of elements responsible for generating the
	 * key. The iteratee is invoked with one argument: (value).
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Collection
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} [iteratee=_.identity] The iteratee to transform keys.
	 * @returns {Object} Returns the composed aggregate object.
	 * @example
	 *
	 * _.groupBy([6.1, 4.2, 6.3], Math.floor);
	 * // => { '4': [4.2], '6': [6.1, 6.3] }
	 *
	 * // The `_.property` iteratee shorthand.
	 * _.groupBy(['one', 'two', 'three'], 'length');
	 * // => { '3': ['one', 'two'], '5': ['three'] }
	 */
	var groupBy = createAggregator(function(result, value, key) {
	  if (hasOwnProperty.call(result, key)) {
	    result[key].push(value);
	  } else {
	    baseAssignValue(result, key, [value]);
	  }
	});

	module.exports = groupBy;


/***/ },
/* 207 */
/***/ function(module, exports, __webpack_require__) {

	var baseHas = __webpack_require__(426),
	    hasPath = __webpack_require__(198);

	/**
	 * Checks if `path` is a direct property of `object`.
	 *
	 * @static
	 * @since 0.1.0
	 * @memberOf _
	 * @category Object
	 * @param {Object} object The object to query.
	 * @param {Array|string} path The path to check.
	 * @returns {boolean} Returns `true` if `path` exists, else `false`.
	 * @example
	 *
	 * var object = { 'a': { 'b': 2 } };
	 * var other = _.create({ 'a': _.create({ 'b': 2 }) });
	 *
	 * _.has(object, 'a');
	 * // => true
	 *
	 * _.has(object, 'a.b');
	 * // => true
	 *
	 * _.has(object, ['a', 'b']);
	 * // => true
	 *
	 * _.has(other, 'a');
	 * // => false
	 */
	function has(object, path) {
	  return object != null && hasPath(object, path, baseHas);
	}

	module.exports = has;


/***/ },
/* 208 */
/***/ function(module, exports, __webpack_require__) {

	var baseHasIn = __webpack_require__(427),
	    hasPath = __webpack_require__(198);

	/**
	 * Checks if `path` is a direct or inherited property of `object`.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Object
	 * @param {Object} object The object to query.
	 * @param {Array|string} path The path to check.
	 * @returns {boolean} Returns `true` if `path` exists, else `false`.
	 * @example
	 *
	 * var object = _.create({ 'a': _.create({ 'b': 2 }) });
	 *
	 * _.hasIn(object, 'a');
	 * // => true
	 *
	 * _.hasIn(object, 'a.b');
	 * // => true
	 *
	 * _.hasIn(object, ['a', 'b']);
	 * // => true
	 *
	 * _.hasIn(object, 'b');
	 * // => false
	 */
	function hasIn(object, path) {
	  return object != null && hasPath(object, path, baseHasIn);
	}

	module.exports = hasIn;


/***/ },
/* 209 */
/***/ function(module, exports, __webpack_require__) {

	var isArrayLike = __webpack_require__(25),
	    isObjectLike = __webpack_require__(38);

	/**
	 * This method is like `_.isArrayLike` except that it also checks if `value`
	 * is an object.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is an array-like object,
	 *  else `false`.
	 * @example
	 *
	 * _.isArrayLikeObject([1, 2, 3]);
	 * // => true
	 *
	 * _.isArrayLikeObject(document.body.children);
	 * // => true
	 *
	 * _.isArrayLikeObject('abc');
	 * // => false
	 *
	 * _.isArrayLikeObject(_.noop);
	 * // => false
	 */
	function isArrayLikeObject(value) {
	  return isObjectLike(value) && isArrayLike(value);
	}

	module.exports = isArrayLikeObject;


/***/ },
/* 210 */
/***/ function(module, exports, __webpack_require__) {

	var baseGetTag = __webpack_require__(44),
	    isArray = __webpack_require__(12),
	    isObjectLike = __webpack_require__(38);

	/** `Object#toString` result references. */
	var stringTag = '[object String]';

	/**
	 * Checks if `value` is classified as a `String` primitive or object.
	 *
	 * @static
	 * @since 0.1.0
	 * @memberOf _
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is a string, else `false`.
	 * @example
	 *
	 * _.isString('abc');
	 * // => true
	 *
	 * _.isString(1);
	 * // => false
	 */
	function isString(value) {
	  return typeof value == 'string' ||
	    (!isArray(value) && isObjectLike(value) && baseGetTag(value) == stringTag);
	}

	module.exports = isString;


/***/ },
/* 211 */
/***/ function(module, exports) {

	/**
	 * This method returns a new empty array.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.13.0
	 * @category Util
	 * @returns {Array} Returns the new empty array.
	 * @example
	 *
	 * var arrays = _.times(2, _.stubArray);
	 *
	 * console.log(arrays);
	 * // => [[], []]
	 *
	 * console.log(arrays[0] === arrays[1]);
	 * // => false
	 */
	function stubArray() {
	  return [];
	}

	module.exports = stubArray;


/***/ },
/* 212 */,
/* 213 */,
/* 214 */,
/* 215 */,
/* 216 */,
/* 217 */,
/* 218 */,
/* 219 */,
/* 220 */,
/* 221 */,
/* 222 */,
/* 223 */,
/* 224 */,
/* 225 */,
/* 226 */,
/* 227 */,
/* 228 */,
/* 229 */,
/* 230 */,
/* 231 */,
/* 232 */,
/* 233 */,
/* 234 */,
/* 235 */,
/* 236 */,
/* 237 */,
/* 238 */,
/* 239 */,
/* 240 */,
/* 241 */,
/* 242 */,
/* 243 */,
/* 244 */,
/* 245 */,
/* 246 */,
/* 247 */,
/* 248 */,
/* 249 */,
/* 250 */,
/* 251 */,
/* 252 */,
/* 253 */,
/* 254 */,
/* 255 */,
/* 256 */,
/* 257 */,
/* 258 */,
/* 259 */,
/* 260 */,
/* 261 */,
/* 262 */,
/* 263 */,
/* 264 */,
/* 265 */,
/* 266 */,
/* 267 */,
/* 268 */,
/* 269 */,
/* 270 */,
/* 271 */,
/* 272 */,
/* 273 */,
/* 274 */,
/* 275 */,
/* 276 */,
/* 277 */,
/* 278 */,
/* 279 */,
/* 280 */,
/* 281 */,
/* 282 */,
/* 283 */,
/* 284 */,
/* 285 */,
/* 286 */,
/* 287 */,
/* 288 */,
/* 289 */,
/* 290 */,
/* 291 */,
/* 292 */,
/* 293 */,
/* 294 */,
/* 295 */,
/* 296 */,
/* 297 */,
/* 298 */,
/* 299 */,
/* 300 */,
/* 301 */,
/* 302 */,
/* 303 */,
/* 304 */,
/* 305 */,
/* 306 */,
/* 307 */,
/* 308 */,
/* 309 */,
/* 310 */,
/* 311 */,
/* 312 */,
/* 313 */,
/* 314 */,
/* 315 */,
/* 316 */,
/* 317 */,
/* 318 */,
/* 319 */,
/* 320 */,
/* 321 */,
/* 322 */,
/* 323 */,
/* 324 */,
/* 325 */,
/* 326 */,
/* 327 */,
/* 328 */,
/* 329 */,
/* 330 */,
/* 331 */,
/* 332 */,
/* 333 */,
/* 334 */,
/* 335 */,
/* 336 */,
/* 337 */,
/* 338 */,
/* 339 */,
/* 340 */,
/* 341 */,
/* 342 */,
/* 343 */,
/* 344 */,
/* 345 */,
/* 346 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	module.exports = __webpack_require__(563);


/***/ },
/* 347 */,
/* 348 */,
/* 349 */,
/* 350 */,
/* 351 */,
/* 352 */,
/* 353 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _task = __webpack_require__(53);

	var _task2 = _interopRequireDefault(_task);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _backbone2.default.Collection.extend({
		model: _task2.default
	});

/***/ },
/* 354 */
/***/ function(module, exports) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	  value: true
	});

	var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

	// ==ClosureCompiler==
	// @compilation_level ADVANCED_OPTIMIZATIONS
	// @externs_url https://raw.githubusercontent.com/google/closure-compiler/master/contrib/externs/maps/google_maps_api_v3.js
	// ==/ClosureCompiler==

	/**
	 * @name MarkerClusterer for Google Maps v3
	 * @version version 1.0
	 * @author Luke Mahe
	 * @fileoverview
	 * The library creates and manages per-zoom-level clusters for large amounts of
	 * markers.
	 * <br/>
	 * This is a v3 implementation of the
	 * <a href="http://gmaps-utility-library-dev.googlecode.com/svn/tags/markerclusterer/"
	 * >v2 MarkerClusterer</a>.
	 */

	/**
	 * @license
	 * Copyright 2010 Google Inc. All Rights Reserved.
	 *
	 * Licensed under the Apache License, Version 2.0 (the "License");
	 * you may not use this file except in compliance with the License.
	 * You may obtain a copy of the License at
	 *
	 *     http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 */

	/**
	 * A Marker Clusterer that clusters markers.
	 *
	 * @param {google.maps.Map} map The Google map to attach to.
	 * @param {Array.<google.maps.Marker>=} opt_markers Optional markers to add to
	 *   the cluster.
	 * @param {Object=} opt_options support the following options:
	 *     'gridSize': (number) The grid size of a cluster in pixels.
	 *     'maxZoom': (number) The maximum zoom level that a marker can be part of a
	 *                cluster.
	 *     'zoomOnClick': (boolean) Whether the default behaviour of clicking on a
	 *                    cluster is to zoom into it.
	 *     'averageCenter': (boolean) Whether the center of each cluster should be
	 *                      the average of all markers in the cluster.
	 *     'minimumClusterSize': (number) The minimum number of markers to be in a
	 *                           cluster before the markers are hidden and a count
	 *                           is shown.
	 *     'styles': (object) An object that has style properties:
	 *       'url': (string) The image url.
	 *       'height': (number) The image height.
	 *       'width': (number) The image width.
	 *       'anchor': (Array) The anchor position of the label text.
	 *       'textColor': (string) The text color.
	 *       'textSize': (number) The text size.
	 *       'backgroundPosition': (string) The position of the backgound x, y.
	 *       'iconAnchor': (Array) The anchor position of the icon x, y.
	 * @constructor
	 * @extends google.maps.OverlayView
	 */
	function MarkerClusterer(map, opt_markers, opt_options) {
	  // MarkerClusterer implements google.maps.OverlayView interface. We use the
	  // extend function to extend MarkerClusterer with google.maps.OverlayView
	  // because it might not always be available when the code is defined so we
	  // look for it at the last possible moment. If it doesn't exist now then
	  // there is no point going ahead :)
	  this.extend(MarkerClusterer, google.maps.OverlayView);
	  this.map_ = map;

	  /**
	   * @type {Array.<google.maps.Marker>}
	   * @private
	   */
	  this.markers_ = [];

	  /**
	   *  @type {Array.<Cluster>}
	   */
	  this.clusters_ = [];

	  this.sizes = [53, 56, 66, 78, 90];

	  /**
	   * @private
	   */
	  this.styles_ = [];

	  /**
	   * @type {boolean}
	   * @private
	   */
	  this.ready_ = false;

	  var options = opt_options || {};

	  /**
	   * @type {number}
	   * @private
	   */
	  this.gridSize_ = options['gridSize'] || 60;

	  /**
	   * @private
	   */
	  this.minClusterSize_ = options['minimumClusterSize'] || 2;

	  /**
	   * @type {?number}
	   * @private
	   */
	  this.maxZoom_ = options['maxZoom'] || null;

	  this.styles_ = options['styles'] || [];

	  /**
	   * @type {string}
	   * @private
	   */
	  this.imagePath_ = options['imagePath'] || this.MARKER_CLUSTER_IMAGE_PATH_;

	  /**
	   * @type {string}
	   * @private
	   */
	  this.imageExtension_ = options['imageExtension'] || this.MARKER_CLUSTER_IMAGE_EXTENSION_;

	  /**
	   * @type {boolean}
	   * @private
	   */
	  this.zoomOnClick_ = true;

	  if (options['zoomOnClick'] != undefined) {
	    this.zoomOnClick_ = options['zoomOnClick'];
	  }

	  /**
	   * @type {boolean}
	   * @private
	   */
	  this.averageCenter_ = false;

	  if (options['averageCenter'] != undefined) {
	    this.averageCenter_ = options['averageCenter'];
	  }

	  this.setupStyles_();

	  this.setMap(map);

	  /**
	   * @type {number}
	   * @private
	   */
	  this.prevZoom_ = this.map_.getZoom();

	  // Add the map event listeners
	  var that = this;
	  google.maps.event.addListener(this.map_, 'zoom_changed', function () {
	    var zoom = that.map_.getZoom();

	    if (that.prevZoom_ != zoom) {
	      that.prevZoom_ = zoom;
	      that.resetViewport();
	    }
	  });

	  google.maps.event.addListener(this.map_, 'idle', function () {
	    that.redraw();
	  });

	  // Finally, add the markers
	  if (opt_markers && opt_markers.length) {
	    this.addMarkers(opt_markers, false);
	  }
	}

	/**
	 * The marker cluster image path.
	 *
	 * @type {string}
	 * @private
	 */
	MarkerClusterer.prototype.MARKER_CLUSTER_IMAGE_PATH_ = '../images/m';

	/**
	 * The marker cluster image path.
	 *
	 * @type {string}
	 * @private
	 */
	MarkerClusterer.prototype.MARKER_CLUSTER_IMAGE_EXTENSION_ = 'png';

	/**
	 * Extends a objects prototype by anothers.
	 *
	 * @param {Object} obj1 The object to be extended.
	 * @param {Object} obj2 The object to extend with.
	 * @return {Object} The new extended object.
	 * @ignore
	 */
	MarkerClusterer.prototype.extend = function (obj1, obj2) {
	  return function (object) {
	    for (var property in object.prototype) {
	      this.prototype[property] = object.prototype[property];
	    }
	    return this;
	  }.apply(obj1, [obj2]);
	};

	/**
	 * Implementaion of the interface method.
	 * @ignore
	 */
	MarkerClusterer.prototype.onAdd = function () {
	  this.setReady_(true);
	};

	/**
	 * Implementaion of the interface method.
	 * @ignore
	 */
	MarkerClusterer.prototype.draw = function () {};

	/**
	 * Sets up the styles object.
	 *
	 * @private
	 */
	MarkerClusterer.prototype.setupStyles_ = function () {
	  if (this.styles_.length) {
	    return;
	  }

	  for (var i = 0, size; size = this.sizes[i]; i++) {
	    this.styles_.push({
	      url: this.imagePath_ + (i + 1) + '.' + this.imageExtension_,
	      height: size,
	      width: size
	    });
	  }
	};

	/**
	 *  Fit the map to the bounds of the markers in the clusterer.
	 */
	MarkerClusterer.prototype.fitMapToMarkers = function () {
	  var markers = this.getMarkers();
	  var bounds = new google.maps.LatLngBounds();
	  for (var i = 0, marker; marker = markers[i]; i++) {
	    bounds.extend(marker.getPosition());
	  }

	  this.map_.fitBounds(bounds);
	};

	/**
	 *  Sets the styles.
	 *
	 *  @param {Object} styles The style to set.
	 */
	MarkerClusterer.prototype.setStyles = function (styles) {
	  this.styles_ = styles;
	};

	/**
	 *  Gets the styles.
	 *
	 *  @return {Object} The styles object.
	 */
	MarkerClusterer.prototype.getStyles = function () {
	  return this.styles_;
	};

	/**
	 * Whether zoom on click is set.
	 *
	 * @return {boolean} True if zoomOnClick_ is set.
	 */
	MarkerClusterer.prototype.isZoomOnClick = function () {
	  return this.zoomOnClick_;
	};

	/**
	 * Whether average center is set.
	 *
	 * @return {boolean} True if averageCenter_ is set.
	 */
	MarkerClusterer.prototype.isAverageCenter = function () {
	  return this.averageCenter_;
	};

	/**
	 *  Returns the array of markers in the clusterer.
	 *
	 *  @return {Array.<google.maps.Marker>} The markers.
	 */
	MarkerClusterer.prototype.getMarkers = function () {
	  return this.markers_;
	};

	/**
	 *  Returns the number of markers in the clusterer
	 *
	 *  @return {Number} The number of markers.
	 */
	MarkerClusterer.prototype.getTotalMarkers = function () {
	  return this.markers_.length;
	};

	/**
	 *  Sets the max zoom for the clusterer.
	 *
	 *  @param {number} maxZoom The max zoom level.
	 */
	MarkerClusterer.prototype.setMaxZoom = function (maxZoom) {
	  this.maxZoom_ = maxZoom;
	};

	/**
	 *  Gets the max zoom for the clusterer.
	 *
	 *  @return {number} The max zoom level.
	 */
	MarkerClusterer.prototype.getMaxZoom = function () {
	  return this.maxZoom_;
	};

	/**
	 *  The function for calculating the cluster icon image.
	 *
	 *  @param {Array.<google.maps.Marker>} markers The markers in the clusterer.
	 *  @param {number} numStyles The number of styles available.
	 *  @return {Object} A object properties: 'text' (string) and 'index' (number).
	 *  @private
	 */
	MarkerClusterer.prototype.calculator_ = function (markers, numStyles) {
	  var index = 0;
	  var count = markers.length;
	  var dv = count;
	  while (dv !== 0) {
	    dv = parseInt(dv / 10, 10);
	    index++;
	  }

	  index = Math.min(index, numStyles);
	  return {
	    text: count,
	    index: index
	  };
	};

	/**
	 * Set the calculator function.
	 *
	 * @param {function(Array, number)} calculator The function to set as the
	 *     calculator. The function should return a object properties:
	 *     'text' (string) and 'index' (number).
	 *
	 */
	MarkerClusterer.prototype.setCalculator = function (calculator) {
	  this.calculator_ = calculator;
	};

	/**
	 * Get the calculator function.
	 *
	 * @return {function(Array, number)} the calculator function.
	 */
	MarkerClusterer.prototype.getCalculator = function () {
	  return this.calculator_;
	};

	/**
	 * Add an array of markers to the clusterer.
	 *
	 * @param {Array.<google.maps.Marker>} markers The markers to add.
	 * @param {boolean=} opt_nodraw Whether to redraw the clusters.
	 */
	MarkerClusterer.prototype.addMarkers = function (markers, opt_nodraw) {
	  for (var i = 0, marker; marker = markers[i]; i++) {
	    this.pushMarkerTo_(marker);
	  }
	  if (!opt_nodraw) {
	    this.redraw();
	  }
	};

	/**
	 * Pushes a marker to the clusterer.
	 *
	 * @param {google.maps.Marker} marker The marker to add.
	 * @private
	 */
	MarkerClusterer.prototype.pushMarkerTo_ = function (marker) {
	  marker.isAdded = false;
	  if (marker['draggable']) {
	    // If the marker is draggable add a listener so we update the clusters on
	    // the drag end.
	    var that = this;
	    google.maps.event.addListener(marker, 'dragend', function () {
	      marker.isAdded = false;
	      that.repaint();
	    });
	  }
	  this.markers_.push(marker);
	};

	/**
	 * Adds a marker to the clusterer and redraws if needed.
	 *
	 * @param {google.maps.Marker} marker The marker to add.
	 * @param {boolean=} opt_nodraw Whether to redraw the clusters.
	 */
	MarkerClusterer.prototype.addMarker = function (marker, opt_nodraw) {
	  this.pushMarkerTo_(marker);
	  if (!opt_nodraw) {
	    this.redraw();
	  }
	};

	/**
	 * Removes a marker and returns true if removed, false if not
	 *
	 * @param {google.maps.Marker} marker The marker to remove
	 * @return {boolean} Whether the marker was removed or not
	 * @private
	 */
	MarkerClusterer.prototype.removeMarker_ = function (marker) {
	  var index = -1;
	  if (this.markers_.indexOf) {
	    index = this.markers_.indexOf(marker);
	  } else {
	    for (var i = 0, m; m = this.markers_[i]; i++) {
	      if (m == marker) {
	        index = i;
	        break;
	      }
	    }
	  }

	  if (index == -1) {
	    // Marker is not in our list of markers.
	    return false;
	  }

	  marker.setMap(null);

	  this.markers_.splice(index, 1);

	  return true;
	};

	/**
	 * Remove a marker from the cluster.
	 *
	 * @param {google.maps.Marker} marker The marker to remove.
	 * @param {boolean=} opt_nodraw Optional boolean to force no redraw.
	 * @return {boolean} True if the marker was removed.
	 */
	MarkerClusterer.prototype.removeMarker = function (marker, opt_nodraw) {
	  var removed = this.removeMarker_(marker);

	  if (!opt_nodraw && removed) {
	    this.resetViewport();
	    this.redraw();
	    return true;
	  } else {
	    return false;
	  }
	};

	/**
	 * Removes an array of markers from the cluster.
	 *
	 * @param {Array.<google.maps.Marker>} markers The markers to remove.
	 * @param {boolean=} opt_nodraw Optional boolean to force no redraw.
	 */
	MarkerClusterer.prototype.removeMarkers = function (markers, opt_nodraw) {
	  var removed = false;

	  for (var i = 0, marker; marker = markers[i]; i++) {
	    var r = this.removeMarker_(marker);
	    removed = removed || r;
	  }

	  if (!opt_nodraw && removed) {
	    this.resetViewport();
	    this.redraw();
	    return true;
	  }
	};

	/**
	 * Sets the clusterer's ready state.
	 *
	 * @param {boolean} ready The state.
	 * @private
	 */
	MarkerClusterer.prototype.setReady_ = function (ready) {
	  if (!this.ready_) {
	    this.ready_ = ready;
	    this.createClusters_();
	  }
	};

	/**
	 * Returns the number of clusters in the clusterer.
	 *
	 * @return {number} The number of clusters.
	 */
	MarkerClusterer.prototype.getTotalClusters = function () {
	  return this.clusters_.length;
	};

	/**
	 * Returns the google map that the clusterer is associated with.
	 *
	 * @return {google.maps.Map} The map.
	 */
	MarkerClusterer.prototype.getMap = function () {
	  return this.map_;
	};

	/**
	 * Sets the google map that the clusterer is associated with.
	 *
	 * @param {google.maps.Map} map The map.
	 */
	MarkerClusterer.prototype.setMap = function (map) {
	  this.map_ = map;
	};

	/**
	 * Returns the size of the grid.
	 *
	 * @return {number} The grid size.
	 */
	MarkerClusterer.prototype.getGridSize = function () {
	  return this.gridSize_;
	};

	/**
	 * Sets the size of the grid.
	 *
	 * @param {number} size The grid size.
	 */
	MarkerClusterer.prototype.setGridSize = function (size) {
	  this.gridSize_ = size;
	};

	/**
	 * Returns the min cluster size.
	 *
	 * @return {number} The grid size.
	 */
	MarkerClusterer.prototype.getMinClusterSize = function () {
	  return this.minClusterSize_;
	};

	/**
	 * Sets the min cluster size.
	 *
	 * @param {number} size The grid size.
	 */
	MarkerClusterer.prototype.setMinClusterSize = function (size) {
	  this.minClusterSize_ = size;
	};

	/**
	 * Extends a bounds object by the grid size.
	 *
	 * @param {google.maps.LatLngBounds} bounds The bounds to extend.
	 * @return {google.maps.LatLngBounds} The extended bounds.
	 */
	MarkerClusterer.prototype.getExtendedBounds = function (bounds) {
	  var projection = this.getProjection();

	  // Turn the bounds into latlng.
	  var tr = new google.maps.LatLng(bounds.getNorthEast().lat(), bounds.getNorthEast().lng());
	  var bl = new google.maps.LatLng(bounds.getSouthWest().lat(), bounds.getSouthWest().lng());

	  // Convert the points to pixels and the extend out by the grid size.
	  var trPix = projection.fromLatLngToDivPixel(tr);
	  trPix.x += this.gridSize_;
	  trPix.y -= this.gridSize_;

	  var blPix = projection.fromLatLngToDivPixel(bl);
	  blPix.x -= this.gridSize_;
	  blPix.y += this.gridSize_;

	  // Convert the pixel points back to LatLng
	  var ne = projection.fromDivPixelToLatLng(trPix);
	  var sw = projection.fromDivPixelToLatLng(blPix);

	  // Extend the bounds to contain the new bounds.
	  bounds.extend(ne);
	  bounds.extend(sw);

	  return bounds;
	};

	/**
	 * Determins if a marker is contained in a bounds.
	 *
	 * @param {google.maps.Marker} marker The marker to check.
	 * @param {google.maps.LatLngBounds} bounds The bounds to check against.
	 * @return {boolean} True if the marker is in the bounds.
	 * @private
	 */
	MarkerClusterer.prototype.isMarkerInBounds_ = function (marker, bounds) {
	  return bounds.contains(marker.getPosition());
	};

	/**
	 * Clears all clusters and markers from the clusterer.
	 */
	MarkerClusterer.prototype.clearMarkers = function () {
	  this.resetViewport(true);

	  // Set the markers a empty array.
	  this.markers_ = [];
	};

	/**
	 * Clears all existing clusters and recreates them.
	 * @param {boolean} opt_hide To also hide the marker.
	 */
	MarkerClusterer.prototype.resetViewport = function (opt_hide) {
	  // Remove all the clusters
	  for (var i = 0, cluster; cluster = this.clusters_[i]; i++) {
	    cluster.remove();
	  }

	  // Reset the markers to not be added and to be invisible.
	  for (var i = 0, marker; marker = this.markers_[i]; i++) {
	    marker.isAdded = false;
	    if (opt_hide) {
	      marker.setMap(null);
	    }
	  }

	  this.clusters_ = [];
	};

	/**
	 *
	 */
	MarkerClusterer.prototype.repaint = function () {
	  var oldClusters = this.clusters_.slice();
	  this.clusters_.length = 0;
	  this.resetViewport();
	  this.redraw();

	  // Remove the old clusters.
	  // Do it in a timeout so the other clusters have been drawn first.
	  window.setTimeout(function () {
	    for (var i = 0, cluster; cluster = oldClusters[i]; i++) {
	      cluster.remove();
	    }
	  }, 0);
	};

	/**
	 * Redraws the clusters.
	 */
	MarkerClusterer.prototype.redraw = function () {
	  this.createClusters_();
	};

	/**
	 * Calculates the distance between two latlng locations in km.
	 * @see http://www.movable-type.co.uk/scripts/latlong.html
	 *
	 * @param {google.maps.LatLng} p1 The first lat lng point.
	 * @param {google.maps.LatLng} p2 The second lat lng point.
	 * @return {number} The distance between the two points in km.
	 * @private
	*/
	MarkerClusterer.prototype.distanceBetweenPoints_ = function (p1, p2) {
	  if (!p1 || !p2) {
	    return 0;
	  }

	  var R = 6371; // Radius of the Earth in km
	  var dLat = (p2.lat() - p1.lat()) * Math.PI / 180;
	  var dLon = (p2.lng() - p1.lng()) * Math.PI / 180;
	  var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos(p1.lat() * Math.PI / 180) * Math.cos(p2.lat() * Math.PI / 180) * Math.sin(dLon / 2) * Math.sin(dLon / 2);
	  var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
	  var d = R * c;
	  return d;
	};

	/**
	 * Add a marker to a cluster, or creates a new cluster.
	 *
	 * @param {google.maps.Marker} marker The marker to add.
	 * @private
	 */
	MarkerClusterer.prototype.addToClosestCluster_ = function (marker) {
	  var distance = 40000; // Some large number
	  var clusterToAddTo = null;
	  var pos = marker.getPosition();
	  for (var i = 0, cluster; cluster = this.clusters_[i]; i++) {
	    var center = cluster.getCenter();
	    if (center) {
	      var d = this.distanceBetweenPoints_(center, marker.getPosition());
	      if (d < distance) {
	        distance = d;
	        clusterToAddTo = cluster;
	      }
	    }
	  }

	  if (clusterToAddTo && clusterToAddTo.isMarkerInClusterBounds(marker)) {
	    clusterToAddTo.addMarker(marker);
	  } else {
	    var cluster = new Cluster(this);
	    cluster.addMarker(marker);
	    this.clusters_.push(cluster);
	  }
	};

	/**
	 * Creates the clusters.
	 *
	 * @private
	 */
	MarkerClusterer.prototype.createClusters_ = function () {
	  if (!this.ready_) {
	    return;
	  }

	  // Get our current map view bounds.
	  // Create a new bounds object so we don't affect the map.
	  var mapBounds = new google.maps.LatLngBounds(this.map_.getBounds().getSouthWest(), this.map_.getBounds().getNorthEast());
	  var bounds = this.getExtendedBounds(mapBounds);

	  for (var i = 0, marker; marker = this.markers_[i]; i++) {
	    if (!marker.isAdded && this.isMarkerInBounds_(marker, bounds)) {
	      this.addToClosestCluster_(marker);
	    }
	  }
	};

	/**
	 * A cluster that contains markers.
	 *
	 * @param {MarkerClusterer} markerClusterer The markerclusterer that this
	 *     cluster is associated with.
	 * @constructor
	 * @ignore
	 */
	function Cluster(markerClusterer) {
	  this.markerClusterer_ = markerClusterer;
	  this.map_ = markerClusterer.getMap();
	  this.gridSize_ = markerClusterer.getGridSize();
	  this.minClusterSize_ = markerClusterer.getMinClusterSize();
	  this.averageCenter_ = markerClusterer.isAverageCenter();
	  this.center_ = null;
	  this.markers_ = [];
	  this.bounds_ = null;
	  this.clusterIcon_ = new ClusterIcon(this, markerClusterer.getStyles(), markerClusterer.getGridSize());
	}

	/**
	 * Determins if a marker is already added to the cluster.
	 *
	 * @param {google.maps.Marker} marker The marker to check.
	 * @return {boolean} True if the marker is already added.
	 */
	Cluster.prototype.isMarkerAlreadyAdded = function (marker) {
	  if (this.markers_.indexOf) {
	    return this.markers_.indexOf(marker) != -1;
	  } else {
	    for (var i = 0, m; m = this.markers_[i]; i++) {
	      if (m == marker) {
	        return true;
	      }
	    }
	  }
	  return false;
	};

	/**
	 * Add a marker the cluster.
	 *
	 * @param {google.maps.Marker} marker The marker to add.
	 * @return {boolean} True if the marker was added.
	 */
	Cluster.prototype.addMarker = function (marker) {
	  if (this.isMarkerAlreadyAdded(marker)) {
	    return false;
	  }

	  if (!this.center_) {
	    this.center_ = marker.getPosition();
	    this.calculateBounds_();
	  } else {
	    if (this.averageCenter_) {
	      var l = this.markers_.length + 1;
	      var lat = (this.center_.lat() * (l - 1) + marker.getPosition().lat()) / l;
	      var lng = (this.center_.lng() * (l - 1) + marker.getPosition().lng()) / l;
	      this.center_ = new google.maps.LatLng(lat, lng);
	      this.calculateBounds_();
	    }
	  }

	  marker.isAdded = true;
	  this.markers_.push(marker);

	  var len = this.markers_.length;
	  if (len < this.minClusterSize_ && marker.getMap() != this.map_) {
	    // Min cluster size not reached so show the marker.
	    marker.setMap(this.map_);
	  }

	  if (len == this.minClusterSize_) {
	    // Hide the markers that were showing.
	    for (var i = 0; i < len; i++) {
	      this.markers_[i].setMap(null);
	    }
	  }

	  if (len >= this.minClusterSize_) {
	    marker.setMap(null);
	  }

	  this.updateIcon();
	  return true;
	};

	/**
	 * Returns the marker clusterer that the cluster is associated with.
	 *
	 * @return {MarkerClusterer} The associated marker clusterer.
	 */
	Cluster.prototype.getMarkerClusterer = function () {
	  return this.markerClusterer_;
	};

	/**
	 * Returns the bounds of the cluster.
	 *
	 * @return {google.maps.LatLngBounds} the cluster bounds.
	 */
	Cluster.prototype.getBounds = function () {
	  var bounds = new google.maps.LatLngBounds(this.center_, this.center_);
	  var markers = this.getMarkers();
	  for (var i = 0, marker; marker = markers[i]; i++) {
	    bounds.extend(marker.getPosition());
	  }
	  return bounds;
	};

	/**
	 * Removes the cluster
	 */
	Cluster.prototype.remove = function () {
	  this.clusterIcon_.remove();
	  this.markers_.length = 0;
	  delete this.markers_;
	};

	/**
	 * Returns the center of the cluster.
	 *
	 * @return {number} The cluster center.
	 */
	Cluster.prototype.getSize = function () {
	  return this.markers_.length;
	};

	/**
	 * Returns the center of the cluster.
	 *
	 * @return {Array.<google.maps.Marker>} The cluster center.
	 */
	Cluster.prototype.getMarkers = function () {
	  return this.markers_;
	};

	/**
	 * Returns the center of the cluster.
	 *
	 * @return {google.maps.LatLng} The cluster center.
	 */
	Cluster.prototype.getCenter = function () {
	  return this.center_;
	};

	/**
	 * Calculated the extended bounds of the cluster with the grid.
	 *
	 * @private
	 */
	Cluster.prototype.calculateBounds_ = function () {
	  var bounds = new google.maps.LatLngBounds(this.center_, this.center_);
	  this.bounds_ = this.markerClusterer_.getExtendedBounds(bounds);
	};

	/**
	 * Determines if a marker lies in the clusters bounds.
	 *
	 * @param {google.maps.Marker} marker The marker to check.
	 * @return {boolean} True if the marker lies in the bounds.
	 */
	Cluster.prototype.isMarkerInClusterBounds = function (marker) {
	  return this.bounds_.contains(marker.getPosition());
	};

	/**
	 * Returns the map that the cluster is associated with.
	 *
	 * @return {google.maps.Map} The map.
	 */
	Cluster.prototype.getMap = function () {
	  return this.map_;
	};

	/**
	 * Updates the cluster icon
	 */
	Cluster.prototype.updateIcon = function () {
	  var zoom = this.map_.getZoom();
	  var mz = this.markerClusterer_.getMaxZoom();

	  if (mz && zoom > mz) {
	    // The zoom is greater than our max zoom so show all the markers in cluster.
	    for (var i = 0, marker; marker = this.markers_[i]; i++) {
	      marker.setMap(this.map_);
	    }
	    return;
	  }

	  if (this.markers_.length < this.minClusterSize_) {
	    // Min cluster size not yet reached.
	    this.clusterIcon_.hide();
	    return;
	  }

	  var numStyles = this.markerClusterer_.getStyles().length;
	  var sums = this.markerClusterer_.getCalculator()(this.markers_, numStyles);
	  this.clusterIcon_.setCenter(this.center_);
	  this.clusterIcon_.setSums(sums);
	  this.clusterIcon_.show();
	};

	/**
	 * A cluster icon
	 *
	 * @param {Cluster} cluster The cluster to be associated with.
	 * @param {Object} styles An object that has style properties:
	 *     'url': (string) The image url.
	 *     'height': (number) The image height.
	 *     'width': (number) The image width.
	 *     'anchor': (Array) The anchor position of the label text.
	 *     'textColor': (string) The text color.
	 *     'textSize': (number) The text size.
	 *     'backgroundPosition: (string) The background postition x, y.
	 * @param {number=} opt_padding Optional padding to apply to the cluster icon.
	 * @constructor
	 * @extends google.maps.OverlayView
	 * @ignore
	 */
	function ClusterIcon(cluster, styles, opt_padding) {
	  cluster.getMarkerClusterer().extend(ClusterIcon, google.maps.OverlayView);

	  this.styles_ = styles;
	  this.padding_ = opt_padding || 0;
	  this.cluster_ = cluster;
	  this.center_ = null;
	  this.map_ = cluster.getMap();
	  this.div_ = null;
	  this.sums_ = null;
	  this.visible_ = false;

	  this.setMap(this.map_);
	}

	/**
	 * Triggers the clusterclick event and zoom's if the option is set.
	 *
	 * @param {google.maps.MouseEvent} event The event to propagate
	 */
	ClusterIcon.prototype.triggerClusterClick = function (event) {
	  var markerClusterer = this.cluster_.getMarkerClusterer();

	  // Trigger the clusterclick event.
	  google.maps.event.trigger(markerClusterer, 'clusterclick', this.cluster_, event);

	  if (markerClusterer.isZoomOnClick()) {
	    // Zoom into the cluster.
	    this.map_.fitBounds(this.cluster_.getBounds());
	  }
	};

	/**
	 * Adding the cluster icon to the dom.
	 * @ignore
	 */
	ClusterIcon.prototype.onAdd = function () {
	  this.div_ = document.createElement('DIV');
	  if (this.visible_) {
	    var pos = this.getPosFromLatLng_(this.center_);
	    this.div_.style.cssText = this.createCss(pos);
	    this.div_.innerHTML = this.sums_.text;
	  }

	  var panes = this.getPanes();
	  panes.overlayMouseTarget.appendChild(this.div_);

	  var that = this;
	  var isDragging = false;
	  google.maps.event.addDomListener(this.div_, 'click', function (event) {
	    // Only perform click when not preceded by a drag
	    if (!isDragging) {
	      that.triggerClusterClick(event);
	    }
	  });
	  google.maps.event.addDomListener(this.div_, 'mousedown', function () {
	    isDragging = false;
	  });
	  google.maps.event.addDomListener(this.div_, 'mousemove', function () {
	    isDragging = true;
	  });
	};

	/**
	 * Returns the position to place the div dending on the latlng.
	 *
	 * @param {google.maps.LatLng} latlng The position in latlng.
	 * @return {google.maps.Point} The position in pixels.
	 * @private
	 */
	ClusterIcon.prototype.getPosFromLatLng_ = function (latlng) {
	  var pos = this.getProjection().fromLatLngToDivPixel(latlng);

	  if (_typeof(this.iconAnchor_) === 'object' && this.iconAnchor_.length === 2) {
	    pos.x -= this.iconAnchor_[0];
	    pos.y -= this.iconAnchor_[1];
	  } else {
	    pos.x -= parseInt(this.width_ / 2, 10);
	    pos.y -= parseInt(this.height_ / 2, 10);
	  }
	  return pos;
	};

	/**
	 * Draw the icon.
	 * @ignore
	 */
	ClusterIcon.prototype.draw = function () {
	  if (this.visible_) {
	    var pos = this.getPosFromLatLng_(this.center_);
	    this.div_.style.top = pos.y + 'px';
	    this.div_.style.left = pos.x + 'px';
	  }
	};

	/**
	 * Hide the icon.
	 */
	ClusterIcon.prototype.hide = function () {
	  if (this.div_) {
	    this.div_.style.display = 'none';
	  }
	  this.visible_ = false;
	};

	/**
	 * Position and show the icon.
	 */
	ClusterIcon.prototype.show = function () {
	  if (this.div_) {
	    var pos = this.getPosFromLatLng_(this.center_);
	    this.div_.style.cssText = this.createCss(pos);
	    this.div_.style.display = '';
	  }
	  this.visible_ = true;
	};

	/**
	 * Remove the icon from the map
	 */
	ClusterIcon.prototype.remove = function () {
	  this.setMap(null);
	};

	/**
	 * Implementation of the onRemove interface.
	 * @ignore
	 */
	ClusterIcon.prototype.onRemove = function () {
	  if (this.div_ && this.div_.parentNode) {
	    this.hide();
	    this.div_.parentNode.removeChild(this.div_);
	    this.div_ = null;
	  }
	};

	/**
	 * Set the sums of the icon.
	 *
	 * @param {Object} sums The sums containing:
	 *   'text': (string) The text to display in the icon.
	 *   'index': (number) The style index of the icon.
	 */
	ClusterIcon.prototype.setSums = function (sums) {
	  this.sums_ = sums;
	  this.text_ = sums.text;
	  this.index_ = sums.index;
	  if (this.div_) {
	    this.div_.innerHTML = sums.text;
	  }

	  this.useStyle();
	};

	/**
	 * Sets the icon to the the styles.
	 */
	ClusterIcon.prototype.useStyle = function () {
	  var index = Math.max(0, this.sums_.index - 1);
	  index = Math.min(this.styles_.length - 1, index);
	  var style = this.styles_[index];
	  this.url_ = style['url'];
	  this.height_ = style['height'];
	  this.width_ = style['width'];
	  this.textColor_ = style['textColor'];
	  this.anchor_ = style['anchor'];
	  this.textSize_ = style['textSize'];
	  this.backgroundPosition_ = style['backgroundPosition'];
	  this.iconAnchor_ = style['iconAnchor'];
	};

	/**
	 * Sets the center of the icon.
	 *
	 * @param {google.maps.LatLng} center The latlng to set as the center.
	 */
	ClusterIcon.prototype.setCenter = function (center) {
	  this.center_ = center;
	};

	/**
	 * Create the css text based on the position of the icon.
	 *
	 * @param {google.maps.Point} pos The position.
	 * @return {string} The css style text.
	 */
	ClusterIcon.prototype.createCss = function (pos) {
	  var style = [];
	  style.push('background-image:url(' + this.url_ + ');');
	  var backgroundPosition = this.backgroundPosition_ ? this.backgroundPosition_ : '0 0';
	  style.push('background-position:' + backgroundPosition + ';');

	  if (_typeof(this.anchor_) === 'object') {
	    if (typeof this.anchor_[0] === 'number' && this.anchor_[0] > 0 && this.anchor_[0] < this.height_) {
	      style.push('height:' + (this.height_ - this.anchor_[0]) + 'px; padding-top:' + this.anchor_[0] + 'px;');
	    } else if (typeof this.anchor_[0] === 'number' && this.anchor_[0] < 0 && -this.anchor_[0] < this.height_) {
	      style.push('height:' + this.height_ + 'px; line-height:' + (this.height_ + this.anchor_[0]) + 'px;');
	    } else {
	      style.push('height:' + this.height_ + 'px; line-height:' + this.height_ + 'px;');
	    }
	    if (typeof this.anchor_[1] === 'number' && this.anchor_[1] > 0 && this.anchor_[1] < this.width_) {
	      style.push('width:' + (this.width_ - this.anchor_[1]) + 'px; padding-left:' + this.anchor_[1] + 'px;');
	    } else {
	      style.push('width:' + this.width_ + 'px; text-align:center;');
	    }
	  } else {
	    style.push('height:' + this.height_ + 'px; line-height:' + this.height_ + 'px; width:' + this.width_ + 'px; text-align:center;');
	  }

	  var txtColor = this.textColor_ ? this.textColor_ : 'black';
	  var txtSize = this.textSize_ ? this.textSize_ : 11;

	  style.push('cursor:pointer; top:' + pos.y + 'px; left:' + pos.x + 'px; color:' + txtColor + '; position:absolute; font-size:' + txtSize + 'px; font-family:Arial,sans-serif; font-weight:bold');
	  return style.join('');
	};

	// Export Symbols for Closure
	// If you are not going to compile with closure then you can remove the
	// code below.
	window['MarkerClusterer'] = MarkerClusterer;
	MarkerClusterer.prototype['addMarker'] = MarkerClusterer.prototype.addMarker;
	MarkerClusterer.prototype['addMarkers'] = MarkerClusterer.prototype.addMarkers;
	MarkerClusterer.prototype['clearMarkers'] = MarkerClusterer.prototype.clearMarkers;
	MarkerClusterer.prototype['fitMapToMarkers'] = MarkerClusterer.prototype.fitMapToMarkers;
	MarkerClusterer.prototype['getCalculator'] = MarkerClusterer.prototype.getCalculator;
	MarkerClusterer.prototype['getGridSize'] = MarkerClusterer.prototype.getGridSize;
	MarkerClusterer.prototype['getExtendedBounds'] = MarkerClusterer.prototype.getExtendedBounds;
	MarkerClusterer.prototype['getMap'] = MarkerClusterer.prototype.getMap;
	MarkerClusterer.prototype['getMarkers'] = MarkerClusterer.prototype.getMarkers;
	MarkerClusterer.prototype['getMaxZoom'] = MarkerClusterer.prototype.getMaxZoom;
	MarkerClusterer.prototype['getStyles'] = MarkerClusterer.prototype.getStyles;
	MarkerClusterer.prototype['getTotalClusters'] = MarkerClusterer.prototype.getTotalClusters;
	MarkerClusterer.prototype['getTotalMarkers'] = MarkerClusterer.prototype.getTotalMarkers;
	MarkerClusterer.prototype['redraw'] = MarkerClusterer.prototype.redraw;
	MarkerClusterer.prototype['removeMarker'] = MarkerClusterer.prototype.removeMarker;
	MarkerClusterer.prototype['removeMarkers'] = MarkerClusterer.prototype.removeMarkers;
	MarkerClusterer.prototype['resetViewport'] = MarkerClusterer.prototype.resetViewport;
	MarkerClusterer.prototype['repaint'] = MarkerClusterer.prototype.repaint;
	MarkerClusterer.prototype['setCalculator'] = MarkerClusterer.prototype.setCalculator;
	MarkerClusterer.prototype['setGridSize'] = MarkerClusterer.prototype.setGridSize;
	MarkerClusterer.prototype['setMaxZoom'] = MarkerClusterer.prototype.setMaxZoom;
	MarkerClusterer.prototype['onAdd'] = MarkerClusterer.prototype.onAdd;
	MarkerClusterer.prototype['draw'] = MarkerClusterer.prototype.draw;

	Cluster.prototype['getCenter'] = Cluster.prototype.getCenter;
	Cluster.prototype['getSize'] = Cluster.prototype.getSize;
	Cluster.prototype['getMarkers'] = Cluster.prototype.getMarkers;

	ClusterIcon.prototype['onAdd'] = ClusterIcon.prototype.onAdd;
	ClusterIcon.prototype['draw'] = ClusterIcon.prototype.draw;
	ClusterIcon.prototype['onRemove'] = ClusterIcon.prototype.onRemove;

	exports.default = MarkerClusterer;

/***/ },
/* 355 */
/***/ function(module, exports) {

	'use strict';

	L.GoogleTile = L.Layer.extend({

		// @section
		// @aka ImageOverlay options
		options: {
			pane: 'tilePane',
			interactive: false
		},

		initialize: function initialize(options) {
			L.setOptions(this, options);
		},
		onAdd: function onAdd() {
			if (!this._googleMap) {
				this._initMap();
			}

			this.getPane().appendChild(this._container);

			this._reset();
		},
		onRemove: function onRemove() {
			L.DomUtil.remove(this._container);
			//need destory google map
		},
		getElement: function getElement() {
			return this._container;
		},
		isLoading: function isLoading() {
			return this._loading || false;
		},
		bringToFront: function bringToFront() {
			if (this._map) {
				L.DomUtil.toFront(this._container);
			}
			return this;
		},
		bringToBack: function bringToBack() {
			if (this._map) {
				L.DomUtil.toBack(this._container);
			}
			return this;
		},
		getEvents: function getEvents() {
			return {
				zoom: this._zoom,
				viewreset: this._update,
				moveend: this._update,
				resize: this._resize,
				move: this._update
			};
		},
		_initMap: function _initMap() {
			if (!this._container) {
				var container = this._container = L.DomUtil.create('div', 'leaflet-google-layer ');
			} else {
				var container = this._container;
			}

			this._googleMap = new google.maps.Map(container, {
				disableDefaultUI: true,
				disableDoubleClickZoom: true,
				draggable: false,
				keyboardShortcuts: false,
				scrollwheel: false,
				animatedZoom: false,
				animated: false,
				animate: false
			});
		},
		_resize: function _resize() {
			var size = this._map.getSize(),
			    container = this._container;
			container.style.width = size.x * 1 + 'px';
			container.style.height = size.y * 1 + 'px';
		},
		_zoom: function _zoom() {
			var zoom = this._map.getZoom();
			this._googleMap.setZoom(zoom);
		},
		_reset: function _reset() {
			var map = this._map;
			if (!map) {
				return;
			}
			this._resize();
			this._zoom();
			this._update();
			// let zoom = map.getZoom(),
			// 	center = map.getCenter(),
			// 	size = this._map.getSize(),
			// 	container = this._container;

			// this._googleMap.setZoom(zoom);
			// this._googleMap.setCenter(center);

			// container.style.width = `${size.x * 1}px`;
			// container.style.height = `${size.y * 1}px`;

			// L.DomUtil.setPosition(this._container, this._map.containerPointToLayerPoint([0, 0]), false);
		},
		_update: function _update(e) {
			var center = this._map.getCenter();
			this._googleMap.setCenter(center);
			L.DomUtil.setPosition(this._container, this._map.containerPointToLayerPoint([0, 0]), false);
		}
	});

	// @factory L.imageOverlay(imageUrl: String, bounds: LatLngBounds, options?: ImageOverlay options)
	// Instantiates an image overlay object given the URL of the image and the
	// geographical bounds it is tied to.
	L.googleTile = function (options) {
		return new L.GoogleTile(options);
	};

/***/ },
/* 356 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _concat2 = __webpack_require__(120);

	var _concat3 = _interopRequireDefault(_concat2);

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _backbone2.default.Model.extend({
		urlRoot: 'gtu',
		idAttribute: 'Id',
		defaults: {
			Id: null,
			ShortUniqueID: null,
			UserColor: null
		},
		assignGTUToTask: function assignGTUToTask(postData, opts) {
			var model = this,
			    options = {
				url: model.urlRoot + '/task/assign/',
				method: 'PUT',
				data: postData,
				processData: true,
				success: function success(result) {
					if (result && result.success) {
						model.set(result.data);
					}
				}
			};
			options = (0, _extend3.default)(opts, options);

			return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
		},
		unassignGTUFromTask: function unassignGTUFromTask(taskId, opts) {
			var model = this,
			    options = {
				url: model.urlRoot + '/task/' + taskId + '/unassign/' + model.get('Id'),
				method: 'DELETE',
				success: function success(result) {
					if (result && result.success) {
						model.set({
							IsAssign: false,
							UserColor: null,
							Company: null,
							Auditor: null,
							Role: null,
							TaskId: null,
							AuditorId: null
						});
					}
				}
			};
			options = (0, _extend3.default)(opts, options);

			return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
		},
		getTrack: function getTrack(taskId, opts) {
			var model = this,
			    lastTime = model.get('lastUpdateTime'),
			    url = model.urlRoot + '/task/' + taskId + '/track/' + model.get('Id');
			if (lastTime) {
				url += '/' + lastTime;
			}
			var options = {
				url: url,
				method: 'GET',
				success: function success(result) {
					if (result) {
						model.set('lastUpdateTime', result.lastUpdateTime);
						var existTrack = model.get('track') || [];
						model.set('track', (0, _concat3.default)(existTrack, (0, _map3.default)(result.data, function (i) {
							return {
								lat: parseFloat(i.lat),
								lng: parseFloat(i.lng)
							};
						})));
					}
				}
			};
			options = (0, _extend3.default)(opts, options);
			return (this.sync || _backbone2.default.sync).call(this, '', this, options);
		}
	});

/***/ },
/* 357 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _base = __webpack_require__(24);

	var _base2 = _interopRequireDefault(_base);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _base2.default.extend({
	    urlRoot: 'map',
	    idAttribute: 'key',
	    defaults: {
	        'Id': null,
	        'Name': null,
	        'Total': null,
	        'DisplayName': null,
	        'MapImage': null,
	        'PolygonImage': null,
	        'ImageStatus': 'waiting'
	    },
	    fetchMapImage: function fetchMapImage(mapOption) {
	        var model = this,
	            params = _jquery2.default.extend({
	            mapType: 'ROADMAP'
	        }, mapOption, {
	            campaignId: model.get('CampaignId')
	        }),
	            options = {
	            quite: true,
	            url: model.urlRoot + '/campaign/',
	            method: 'POST',
	            processData: true,
	            data: _jquery2.default.param(params),
	            success: function success(result) {
	                var mapImage = null,
	                    polygonImage = null;
	                if (result && result.success) {
	                    mapImage = result.tiles;
	                    polygonImage = result.geometry;
	                }
	                model.set('MapImage', mapImage, {
	                    silent: true
	                });
	                model.set('PolygonImage', polygonImage, {
	                    silent: true
	                });
	            }
	        };
	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    }
	});

/***/ },
/* 358 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _base = __webpack_require__(24);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _base2.default.extend({
	    idAttribute: 'key',
	    defaults: {
	        'Id': null,
	        'ClientName': null,
	        'ContactName': null,
	        'DisplayName': null,
	        'CreatorName': null,
	        'Date': null,
	        'Logo': null
	    }
	});

/***/ },
/* 359 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _base = __webpack_require__(24);

	var _base2 = _interopRequireDefault(_base);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _base2.default.extend({
	    urlRoot: 'print',
	    defaults: {
	        'Name': null,
	        'TotalHouseHold': null,
	        'TargetHouseHold': null,
	        'Penetration': null
	    },
	    fetchBySubMap: function fetchBySubMap(opts) {
	        var model = this,
	            campaignId = model.get('CampaignId'),
	            submapId = model.get('SubMapId'),
	            url = model.urlRoot + '/campaign/' + campaignId + '/submap/' + submapId + '/record',
	            options = {
	            url: url,
	            success: function success(result) {
	                model.set('CRoutes', result.record);
	            }
	        };
	        (0, _extend3.default)(options, opts);

	        return (this.sync || _backbone2.default.sync).call(this, 'read', this, options);
	    }
	});

/***/ },
/* 360 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _base = __webpack_require__(24);

	var _base2 = _interopRequireDefault(_base);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _base2.default.extend({
	    urlRoot: 'map',
	    idAttribute: 'key',
	    defaults: {
	        'Id': null,
	        'Name': null,
	        'Total': null,
	        'DisplayName': null,
	        'MapImage': null,
	        'PolygonImage': null,
	        'ImageStatus': 'waiting'
	    },
	    fetchMapImage: function fetchMapImage(mapOption) {
	        var model = this,
	            params = _jquery2.default.extend({
	            mapType: 'HYBRID'
	        }, mapOption, {
	            campaignId: model.get('CampaignId'),
	            submapId: model.get('SubMapId'),
	            dmapId: model.get('DMapId')
	        }),
	            options = {
	            quite: true,
	            url: model.urlRoot + '/distribution/',
	            method: 'POST',
	            processData: true,
	            data: _jquery2.default.param(params),
	            success: function success(result) {
	                var mapImage = null,
	                    polygonImage = null;
	                if (result && result.success) {
	                    mapImage = result.tiles;
	                    polygonImage = result.geometry;
	                }
	                model.set('MapImage', mapImage, {
	                    silent: true
	                });
	                model.set('PolygonImage', polygonImage, {
	                    silent: true
	                });
	            }
	        };
	        if (model.get('TopRight') && model.get('BottomLeft')) {
	            options.data = _jquery2.default.param(_jquery2.default.extend(params, {
	                topRightLat: model.get('TopRight').lat,
	                topRightLng: model.get('TopRight').lng,
	                bottomLeftLat: model.get('BottomLeft').lat,
	                bottomLeftLng: model.get('BottomLeft').lng
	            }));
	        }
	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    },
	    fetchBoundary: function fetchBoundary(opts) {
	        var model = this,
	            options = {
	            url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/boundary/',
	            method: 'GET',
	            success: function success(result) {
	                model.set({
	                    'Boundary': result.boundary,
	                    'Color': result.color
	                });
	            }
	        };
	        (0, _extend3.default)(options, opts);
	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    }
	});

/***/ },
/* 361 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _base = __webpack_require__(24);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _base2.default.extend({
	    defaults: {
	        'DisplayName': null,
	        'Date': null,
	        'ContactName': null,
	        'CreatorName': null
	    }
	});

/***/ },
/* 362 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
	    value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _base = __webpack_require__(24);

	var _base2 = _interopRequireDefault(_base);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _base2.default.extend({
	    urlRoot: 'map',
	    idAttribute: 'key',
	    defaults: {
	        'Id': null,
	        'Name': null,
	        'Total': null,
	        'DisplayName': null,
	        'MapImage': null,
	        'PolygonImage': null,
	        'ImageStatus': 'waiting'
	    },
	    fetchMapImage: function fetchMapImage(mapOption) {
	        var model = this,
	            params = _jquery2.default.extend({
	            mapType: 'HYBRID'
	        }, mapOption, {
	            campaignId: model.get('CampaignId'),
	            submapId: model.get('SubMapId')
	        }),
	            options = {
	            quite: true,
	            url: model.urlRoot + '/submap/',
	            method: 'POST',
	            processData: true,
	            data: _jquery2.default.param(params),
	            success: function success(result) {
	                var mapImage = null,
	                    polygonImage = null;
	                if (result && result.success) {
	                    mapImage = result.tiles;
	                    polygonImage = result.geometry;
	                }
	                model.set('MapImage', mapImage, {
	                    silent: true
	                });
	                model.set('PolygonImage', polygonImage, {
	                    silent: true
	                });
	            }
	        };
	        return (this.sync || _backbone2.default.sync).call(this, 'update', this, options);
	    }
	});

/***/ },
/* 363 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	var _postal = __webpack_require__(129);

	var _postal2 = _interopRequireDefault(_postal);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _backbone2.default.Router.extend({
		routes: {
			'campaign': 'defaultAction',
			'distribution': 'distributionAction',
			'monitor': 'monitorAction',
			'campaign/:campaignId/monitor': 'campaignMonitorAction',
			'report': 'reportAction',
			'report/:taskId': 'reportAction',
			'admin': 'adminAction',
			'admin/gtu': 'availableGTUAction',
			'print/:campaignId/:printType': 'printAction',
			'campaign/:campaignId/:taskName/:taskId/edit': 'gtuEditAction',
			'campaign/:campaignId/:taskName/:taskId/monitor': 'gtuMonitorAction',
			'campaign/import': 'importCampaign',
			'frame/:page': 'frameAction',
			'frame/*page?*queryString': 'frameAction',
			'*actions': 'defaultAction'
		},
		defaultAction: function defaultAction() {
			var Collection = __webpack_require__(52).default;
			var View = __webpack_require__(367).default;
			var campaignlist = new Collection();
			campaignlist.fetch().then(function () {
				_postal2.default.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							collection: campaignlist
						}
					}
				});
			});
		},
		frameAction: function frameAction(page, query) {
			var url = "../" + page;
			if (query) {
				url += "?" + query;
			}
			window.open(url, '_blank', 'resizable=yes,status=yes,toolbar=no,scrollbars=yes,menubar=no,location=no');
			_backbone2.default.history.history.back(-2);
		},
		distributionAction: function distributionAction() {
			var Collection = __webpack_require__(52).default;
			var View = __webpack_require__(371).default;
			var campaignlist = new Collection();
			campaignlist.fetchForDistribution().then(function () {
				_postal2.default.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							collection: campaignlist
						}
					}
				});
			});
		},
		monitorAction: function monitorAction() {
			var Collection = __webpack_require__(52).default;
			var View = __webpack_require__(382).default;
			var campaignlist = new Collection();
			campaignlist.fetchForTask().then(function () {
				_postal2.default.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							collection: campaignlist
						}
					}
				});
			});
		},
		gtuMonitorAction: function gtuMonitorAction(campaignId, taskName, taskId) {
			var Task = __webpack_require__(53).default;
			var DMap = __webpack_require__(70).default;
			var Gtu = __webpack_require__(69).default;
			var View = __webpack_require__(376).default;

			var gtu = new Gtu(),
			    task = new Task({
				Id: taskId
			});
			task.fetch().then(function () {
				var dmap = new DMap({
					CampaignId: task.get('CampaignId'),
					SubMapId: task.get('SubMapId'),
					DMapId: task.get('DMapId')
				});

				_bluebird2.default.all([dmap.fetchBoundary(), dmap.fetchAllGtu(), gtu.fetchGtuWithStatusByTask(taskId)]).then(function () {
					var pageTitle = 'GTU Monitor - ' + task.get('ClientName') + ', ' + task.get('ClientCode') + ': ' + task.get('Name');
					_postal2.default.publish({
						channel: 'View',
						topic: 'loadView',
						data: {
							view: View,
							params: {
								dmap: dmap,
								gtu: gtu,
								task: task
							}
						},
						options: {
							showMenu: false,
							showUser: false,
							showSearch: false,
							pageTitle: pageTitle
						}
					});
				});
			});
		},
		campaignMonitorAction: function campaignMonitorAction(campaignId) {
			var View = __webpack_require__(374).default;
			var Model = __webpack_require__(99).default;
			var campaignWithTaskModel = new Model();
			campaignWithTaskModel.loadWithAllTask(campaignId).then(function () {
				_postal2.default.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						options: {
							showMenu: false
						},
						params: {
							model: campaignWithTaskModel
						}
					}
				});
			});
		},
		reportAction: function reportAction(taskId) {
			var Model = __webpack_require__(99).default;
			var Collection = __webpack_require__(52).default;
			var View = __webpack_require__(394).default;
			var campaignlist = new Collection();
			campaignlist.fetchForReport().then(function () {
				_postal2.default.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							collection: campaignlist,
							taskId: taskId
						}
					}
				});
			});
		},
		adminAction: function adminAction() {
			var View = __webpack_require__(365).default;
			_postal2.default.publish({
				channel: 'View',
				topic: 'loadView',
				data: {
					view: View
				}
			});
		},
		importCampaign: function importCampaign() {
			var Collection = __webpack_require__(52).default;
			var View = __webpack_require__(366).default;
			_postal2.default.publish({
				channel: 'View',
				topic: 'loadView',
				data: {
					view: View,
					params: {
						collection: new Collection()
					}
				}
			});
		},
		printAction: function printAction(campaignId, printType) {
			switch (printType) {
				case 'campaign':
					var Collection = __webpack_require__(96).default;
					var View = __webpack_require__(384).default;
					var print = new Collection();
					print.fetchForCampaignMap(campaignId).then(function () {
						_postal2.default.publish({
							channel: 'View',
							topic: 'loadView',
							data: {
								view: View,
								params: {
									collection: print
								},
								options: {
									showMenu: false,
									showUser: false,
									showSearch: false,
									pageTitle: 'Timm Print Preview'
								}
							}
						});
					});
					break;
				case 'distribution':
					var Collection = __webpack_require__(96).default;
					var View = __webpack_require__(385).default;
					var print = new Collection();
					print.fetchForDistributionMap(campaignId).then(function () {
						_postal2.default.publish({
							channel: 'View',
							topic: 'loadView',
							data: {
								view: View,
								params: {
									collection: print
								},
								options: {
									showMenu: false,
									showUser: false,
									showSearch: false,
									pageTitle: 'Timm Print Preview'
								}
							}
						});
					});
					break;
				case 'report':
					var Collection = __webpack_require__(96).default;
					var View = __webpack_require__(386).default;
					var print = new Collection();
					print.fetchForReportMap(campaignId).then(function () {
						_postal2.default.publish({
							channel: 'View',
							topic: 'loadView',
							data: {
								view: View,
								params: {
									collection: print
								},
								options: {
									showMenu: false,
									showUser: false,
									showSearch: false,
									pageTitle: 'Timm Print Preview'
								}
							}
						});
					});
					break;
				default:

					break;
			}
		},
		gtuEditAction: function gtuEditAction(campaignId, taskName, taskId) {
			var Task = __webpack_require__(53).default;
			var DMap = __webpack_require__(70).default;
			var Gtu = __webpack_require__(69).default;
			var View = __webpack_require__(375).default;

			var gtu = new Gtu(),
			    task = new Task({
				Id: taskId
			});
			task.fetch().then(function () {
				var dmap = new DMap({
					CampaignId: task.get('CampaignId'),
					SubMapId: task.get('SubMapId'),
					DMapId: task.get('DMapId')
				});

				_bluebird2.default.all([dmap.fetchBoundary(), dmap.fetchGtuForEdit(), gtu.fetchByTask(taskId)]).then(function () {
					_postal2.default.publish({
						channel: 'View',
						topic: 'loadView',
						data: {
							view: View,
							params: {
								dmap: dmap,
								gtu: gtu,
								task: task
							}
						}
					});
				});
			});
		},

		availableGTUAction: function availableGTUAction() {
			var Collection = __webpack_require__(69).default;
			var View = __webpack_require__(364).default;
			var gtuList = new Collection();
			gtuList.fetch().then(function () {
				_postal2.default.publish({
					channel: 'View',
					topic: 'loadView',
					data: {
						view: View,
						params: {
							collection: gtuList
						},
						options: {
							showSearch: false
						}
					}
				});
			});
		}
	});

/***/ },
/* 364 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _forEach2 = __webpack_require__(59);

	var _forEach3 = _interopRequireDefault(_forEach2);

	var _concat2 = __webpack_require__(120);

	var _concat3 = _interopRequireDefault(_concat2);

	var _sortBy2 = __webpack_require__(531);

	var _sortBy3 = _interopRequireDefault(_sortBy2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _server = __webpack_require__(346);

	var _server2 = _interopRequireDefault(_server);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	__webpack_require__(149);

	var _jsMarkerClusterer = __webpack_require__(354);

	var _jsMarkerClusterer2 = _interopRequireDefault(_jsMarkerClusterer);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _mapBase = __webpack_require__(72);

	var _mapBase2 = _interopRequireDefault(_mapBase);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	var reloadTimeout = null,
	    gtuManager = null,
	    refreshTimeout = null,
	    infowindow;

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default, _mapBase2.default],
		getInitialState: function getInitialState() {
			return {
				disableDefaultUI: false,
				scrollwheel: true,
				disableDoubleClickZoom: false
			};
		},
		onWindowResize: function onWindowResize() {
			var pageLeftHeight = (0, _jquery2.default)(window).height() - (0, _jquery2.default)(this.refs.mapArea).position().top;
			(0, _jquery2.default)('#google-map').css({
				'position': 'relative',
				'top': 0,
				'left': 0,
				'right': 'auto',
				'bottom': 'auto',
				'width': '100%',
				'height': pageLeftHeight
			});
			google.maps.event.trigger(this.getGoogleMap(), "resize");
		},
		componentDidMount: function componentDidMount() {
			/**
	   * position google map to main area
	   */
			(0, _jquery2.default)(window).on('resize', _jquery2.default.proxy(this.onWindowResize, this));
			this.onWindowResize();
			this.drawGtu();
			var collection = this.getCollection();
			var sorted = (0, _sortBy3.default)(collection.models, function (m) {
				var seed = 0;
				if (!m.get('HaveLocation')) {
					seed += 10000000;
				}
				if (m.get('IsAssign')) {
					seed += 1000000;
				}
				seed + parseInt(m.get('Id'));
				return seed;
			});
			var sourceData = (0, _map3.default)(sorted, function (model) {
				return {
					id: model.get('Id'),
					text: model.get('Code')
				};
			});
			sourceData = (0, _concat3.default)({
				id: -1,
				text: 'Please select GTU to show on map'
			}, sourceData);
			(0, _jquery2.default)('#gtuFilter').select2({
				placeholder: 'Please input the GTU code to search',
				tags: false,
				multiple: false,
				data: sourceData
			});
			var googleMap = this.getGoogleMap();
			(0, _jquery2.default)('#gtuFilter').on('change', _jquery2.default.proxy(function () {
				var selectedGTU = (0, _jquery2.default)('#gtuFilter').val();
				if (selectedGTU > 0) {
					this.gotoMarker(parseInt(selectedGTU));
				}
			}, this));
			infowindow = new google.maps.InfoWindow();
		},
		showGTUInfo: function showGTUInfo(marker) {
			var googleMap = this.getGoogleMap();
			infowindow.setContent(this.buildContent(marker.gtuId));
			infowindow.open(googleMap, marker);
		},
		gotoMarker: function gotoMarker(showGtu) {
			var googleMap = this.getGoogleMap(),
			    allMarkers = gtuManager.getMarkers(),
			    bounds = new google.maps.LatLngBounds(),
			    marker = find(allMarkers, function (m) {
				return m.gtuId == showGtu;
			}),
			    self = this;
			console.log('search', showGtu, marker.gtuId, marker.getPosition().lat(), marker.getPosition().lng());
			if (marker) {
				googleMap.setCenter(marker.getPosition());
				googleMap.setZoom(22);
				setTimeout(function () {
					self.showGTUInfo(marker);
				}, 1000);
			} else {
				this.publish('showDialog', "You searched GTU do not find the last position");
			}
		},
		componentWillUnmount: function componentWillUnmount() {
			try {
				this.clearMap();
				(0, _forEach3.default)(gtuLocation, function (item) {
					item.setMap(null);
				});
				google.maps.event.clearInstanceListeners(googleMap);
			} catch (ex) {
				console.log('google map clear error', ex);
			}
			(0, _jquery2.default)('#google-map').css({
				'visibility': 'hidden'
			});
		},
		buildContent: function buildContent(gtuId) {
			var gtu = this.getCollection().get(gtuId);
			var template = _react2.default.createElement(
				'ul',
				{ className: 'menu vertical' },
				_react2.default.createElement(
					'li',
					null,
					_react2.default.createElement(
						'h5',
						null,
						gtu.get('Code')
					)
				),
				_react2.default.createElement(
					'li',
					null,
					'IsAssign: ',
					gtu.get('IsAssign') ? 'YES' : 'NO'
				),
				_react2.default.createElement(
					'li',
					null,
					'IsOnline: ',
					gtu.get('IsOnline') ? 'YES' : 'NO'
				),
				_react2.default.createElement(
					'li',
					{ style: { display: gtu.get('IsAssign') ? 'inherit' : 'none' } },
					'Team: ',
					gtu.get('Company')
				),
				_react2.default.createElement(
					'li',
					{ style: { display: gtu.get('IsAssign') ? 'inherit' : 'none' } },
					'Auditor: ',
					gtu.get('Auditor')
				),
				_react2.default.createElement(
					'li',
					{ style: { display: gtu.get('IsAssign') ? 'inherit' : 'none' } },
					'Role: ',
					gtu.get('Role')
				)
			);
			return _server2.default.renderToStaticMarkup(template);
		},
		drawGtu: function drawGtu() {
			var googleMap = this.getGoogleMap(),
			    gtu = this.getCollection(),
			    bounds = new google.maps.LatLngBounds(),
			    markers = [],
			    self = this;

			(0, _forEach3.default)(gtu.models, function (i) {
				if (i.get('HaveLocation')) {
					var latlng = new google.maps.LatLng(i.get('Latitude'), i.get('Longitude'));
					bounds.extend(latlng);
					var marker = new google.maps.Marker({
						'position': latlng
					});
					marker.gtuId = i.get('Id');
					marker.addListener('click', function () {
						self.showGTUInfo(marker);
					});
					markers.push(marker);
				}
			});
			gtuManager = new _jsMarkerClusterer2.default(googleMap, markers);
			googleMap.fitBounds(bounds);
		},
		render: function render() {
			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'div',
					{ className: 'section row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'section-header' },
							_react2.default.createElement(
								'div',
								{ className: 'small-10 small-centered columns' },
								_react2.default.createElement('select', { id: 'gtuFilter' })
							)
						)
					)
				),
				_react2.default.createElement('div', { ref: 'mapArea' })
			);
		}
	});

/***/ },
/* 365 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createClass({
		displayName: "dashboard",

		render: function render() {
			return _react2.default.createElement(
				"div",
				{ className: "section row" },
				_react2.default.createElement(
					"div",
					{ className: "small-12 columns" },
					_react2.default.createElement(
						"div",
						{ className: "section-header" },
						_react2.default.createElement(
							"div",
							{ className: "row" },
							_react2.default.createElement(
								"div",
								{ className: "small-12 column" },
								_react2.default.createElement(
									"h5",
									null,
									"Administration"
								)
							),
							_react2.default.createElement(
								"div",
								{ className: "small-12 column" },
								_react2.default.createElement(
									"nav",
									{ "aria-label": "You are here:", role: "navigation" },
									_react2.default.createElement(
										"ul",
										{ className: "breadcrumbs" },
										_react2.default.createElement(
											"li",
											null,
											_react2.default.createElement(
												"a",
												{ href: "#" },
												"Control Center"
											)
										),
										_react2.default.createElement(
											"li",
											null,
											_react2.default.createElement(
												"span",
												{ className: "show-for-sr" },
												"Current: "
											),
											" Administration"
										)
									)
								)
							)
						)
					)
				),
				_react2.default.createElement(
					"div",
					{ className: "small-12 columns" },
					_react2.default.createElement(
						"div",
						{ className: "row", style: { 'marginTop': '120px' } },
						_react2.default.createElement(
							"div",
							{ className: "small-7 small-centered" },
							_react2.default.createElement(
								"div",
								{ className: "row" },
								_react2.default.createElement(
									"div",
									{ className: "small-12 column" },
									_react2.default.createElement(
										"div",
										{ className: "callout secondary" },
										_react2.default.createElement(
											"a",
											{ href: "#frame/Users.aspx" },
											_react2.default.createElement(
												"span",
												null,
												"User Management"
											)
										)
									)
								),
								_react2.default.createElement(
									"div",
									{ className: "small-12 column" },
									_react2.default.createElement(
										"div",
										{ className: "callout secondary" },
										_react2.default.createElement(
											"a",
											{ href: "#campaign/import" },
											_react2.default.createElement(
												"span",
												null,
												"Import Campaign"
											)
										)
									)
								)
							),
							_react2.default.createElement(
								"div",
								{ className: "row small-up-2 medium-up-2 large-up-2" },
								_react2.default.createElement(
									"div",
									{ className: "column" },
									_react2.default.createElement(
										"div",
										{ className: "callout secondary" },
										_react2.default.createElement(
											"a",
											{ href: "#frame/NonDeliverables.aspx" },
											_react2.default.createElement(
												"span",
												null,
												"Non-Deliverables"
											)
										)
									)
								),
								_react2.default.createElement(
									"div",
									{ className: "column" },
									_react2.default.createElement(
										"div",
										{ className: "callout secondary" },
										_react2.default.createElement(
											"a",
											{ href: "#frame/GtuAdmin.aspx?AssignNameToGTUFlag=true" },
											_react2.default.createElement(
												"span",
												null,
												"GTU Management"
											)
										)
									)
								),
								_react2.default.createElement(
									"div",
									{ className: "column" },
									_react2.default.createElement(
										"div",
										{ className: "callout secondary" },
										_react2.default.createElement(
											"a",
											{ href: "#admin/gtu" },
											_react2.default.createElement(
												"span",
												null,
												"GTU Available List"
											)
										)
									)
								),
								_react2.default.createElement(
									"div",
									{ className: "column" },
									_react2.default.createElement(
										"div",
										{ className: "callout secondary" },
										_react2.default.createElement(
											"a",
											{ href: "#frame/AdminGtuToBag.aspx" },
											_react2.default.createElement(
												"span",
												null,
												"GTU bag Management "
											)
										)
									)
								),
								_react2.default.createElement(
									"div",
									{ className: "column" },
									_react2.default.createElement(
										"div",
										{ className: "callout secondary" },
										_react2.default.createElement(
											"a",
											{ href: "#frame/AdminGtuBagToAuditor.aspx" },
											_react2.default.createElement(
												"span",
												null,
												"Assign GTU-Bags to Auditors"
											)
										)
									)
								),
								_react2.default.createElement(
									"div",
									{ className: "column" },
									_react2.default.createElement(
										"div",
										{ className: "callout secondary" },
										_react2.default.createElement(
											"a",
											{ href: "#frame/AdminDistributorCompany.aspx" },
											_react2.default.createElement(
												"span",
												null,
												"Distributor Management"
											)
										)
									)
								)
							)
						)
					)
				)
			);
		}
	});

/***/ },
/* 366 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _trimEnd2 = __webpack_require__(535);

	var _trimEnd3 = _interopRequireDefault(_trimEnd2);

	var _isEmpty2 = __webpack_require__(37);

	var _isEmpty3 = _interopRequireDefault(_isEmpty2);

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		onSearch: function onSearch() {
			var url = 'http://timm.vargainc.com/' + this.refs.sourceUrl.value + '/api/campaign';
			if (_.isEmpty(url)) {
				this.publish('showDialog', 'Please input source url');
				return;
			}
			var self = this,
			    collection = this.getCollection();
			collection.reset();
			_jquery2.default.ajax({
				url: url,
				method: 'GET',
				dataType: "json",
				cache: false,
				success: function success(result) {
					var data = (0, _map3.default)(result, function (i) {
						return {
							Id: i.Id,
							ClientName: i.ClientName,
							ClientCode: i.ClientCode,
							Date: i.Date,
							AreaDescription: i.AreaDescription
						};
					});
					if ((0, _isEmpty3.default)(data)) {
						collection.add(data);
						self.setState({
							srcUrl: url
						});
					}
				}
			});
		},
		onImportFailed: function onImportFailed() {
			this.publish('showDialog', 'copy campaign failed. please contact us!');
		},
		onImport: function onImport(campaignId) {
			console.log(campaignId);
			var exportUrl = (0, _trimEnd3.default)(this.state.srcUrl, '/') + '/' + campaignId + '/export',
			    importUrl = '../api/campaign/import',
			    self = this;
			_jquery2.default.getJSON(exportUrl).then(function (campaign) {
				return _jquery2.default.post(importUrl, campaign).then(function (response) {
					if (response && response.success) {
						self.publish('showDialog', 'copy success. please refresh control center!');
						return Promise.resolve();
					}
					return Promise.reject(new Error('server method failed'));
				});
			}).catch(function () {
				self.onImportFailed();
			});
		},
		render: function render() {
			var self = this;
			return _react2.default.createElement(
				'div',
				{ className: 'section row' },
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'section-header' },
						_react2.default.createElement(
							'div',
							{ className: 'row', 'data-equalizer': true },
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'h5',
									null,
									'Import Campaign'
								)
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-8 column' },
								_react2.default.createElement(
									'nav',
									{ 'aria-label': 'You are here:', role: 'navigation' },
									_react2.default.createElement(
										'ul',
										{ className: 'breadcrumbs' },
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'a',
												{ href: '#' },
												'Control Center'
											)
										),
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'span',
												{ className: 'show-for-sr' },
												'Current: '
											),
											' Import Campaign'
										)
									)
								)
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'div',
									{ className: 'input-group' },
									_react2.default.createElement('input', { ref: 'sourceUrl', className: 'input-group-field', type: 'text', placeholder: 'Please input server address and query campaign from this server.' }),
									_react2.default.createElement(
										'div',
										{ className: 'input-group-button' },
										_react2.default.createElement('input', { onClick: this.onSearch, type: 'button', className: 'button', value: 'Query' })
									)
								)
							)
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'scroll-list-section-body' },
						_react2.default.createElement(
							'div',
							{ className: 'row scroll-list-header' },
							_react2.default.createElement(
								'div',
								{ className: 'small-2 columns' },
								'ClientName'
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-4 columns' },
								'ClientCode'
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-2 columns' },
								'Date'
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-2 columns' },
								'AreaDescription'
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-2 columns' },
								'Action'
							)
						),
						this.getCollection().map(function (item) {
							return _react2.default.createElement(
								'div',
								{ key: item.get('Id'), className: 'row scroll-list-item' },
								_react2.default.createElement(
									'div',
									{ className: 'small-2 columns' },
									item.get('ClientName')
								),
								_react2.default.createElement(
									'div',
									{ className: 'small-4 columns' },
									item.get('ClientCode')
								),
								_react2.default.createElement(
									'div',
									{ className: 'small-2 columns' },
									moment(item.get('Date'), moment.ISO_8601).format("MMM DD, YYYY")
								),
								_react2.default.createElement(
									'div',
									{ className: 'small-2 columns' },
									item.get('AreaDescription')
								),
								_react2.default.createElement(
									'div',
									{ className: 'small-2 columns' },
									_react2.default.createElement(
										'button',
										{ 'class': 'button', onClick: self.onImport.bind(self, item.get('Id')) },
										'Import'
									)
								)
							);
						})
					)
				)
			);
		}
	});

/***/ },
/* 367 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _indexOf2 = __webpack_require__(36);

	var _indexOf3 = _interopRequireDefault(_indexOf2);

	var _toString2 = __webpack_require__(63);

	var _toString3 = _interopRequireDefault(_toString2);

	var _some2 = __webpack_require__(47);

	var _some3 = _interopRequireDefault(_some2);

	var _filter2 = __webpack_require__(58);

	var _filter3 = _interopRequireDefault(_filter2);

	var _uniq2 = __webpack_require__(128);

	var _uniq3 = _interopRequireDefault(_uniq2);

	var _orderBy2 = __webpack_require__(125);

	var _orderBy3 = _interopRequireDefault(_orderBy2);

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _row = __webpack_require__(369);

	var _row2 = _interopRequireDefault(_row);

	var _edit = __webpack_require__(150);

	var _edit2 = _interopRequireDefault(_edit);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				orderByFiled: null,
				orderByAsc: false,
				search: null,
				filterField: null,
				filterValues: []
			};
		},
		componentDidMount: function componentDidMount() {
			var self = this;
			this.subscribe('camapign/refresh', function () {
				self.getCollection().fetch();
			});
			this.subscribe('search', function (words) {
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			(0, _jquery2.default)("#campaign-filter-ddl-ClientName, #campaign-filter-ddl-ClientCode, #campaign-filter-ddl-Date, #campaign-filter-ddl-AreaDescription").foundation();
		},
		onNew: function onNew() {
			this.publish('showDialog', _edit2.default, {
				model: new Model()
			});
		},
		onOrderBy: function onOrderBy(field, reactObj, reactId, e) {
			e.preventDefault();
			e.stopPropagation();
			if (this.state.orderByFiled == field) {
				this.setState({
					orderByAsc: !this.state.orderByAsc
				});
			} else {
				this.setState({
					orderByFiled: field,
					orderByAsc: true
				});
			}
		},
		onFilter: function onFilter(field, e) {
			e.preventDefault();
			e.stopPropagation();
			var els = (0, _jquery2.default)(":checked", e.currentTarget),
			    values = (0, _map3.default)(els, function (item) {
				return (0, _jquery2.default)(item).val();
			});

			this.setState({
				filterField: values.length > 0 ? field : null,
				filterValues: values.length > 0 ? values : [],
				search: null
			});
			(0, _jquery2.default)('#campaign-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function onClearFilter(field, e) {
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			(0, _jquery2.default)('#campaign-filter-ddl-' + field).foundation('close');
			(0, _jquery2.default)(e.currentTarget).closest('form').get(0).reset();
		},
		renderHeader: function renderHeader(field, displayName) {
			var dataSource = this.getCollection(),
			    sortIcon = null,
			    filterIcon = null,
			    filterMenu = null;
			dataSource = dataSource ? dataSource.toArray() : [];

			if (this.state.orderByFiled == field) {
				if (this.state.orderByAsc) {
					sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort-up active' });
				} else {
					sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort-down active' });
				}
			} else {
				sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort' });
			}
			sortIcon = _react2.default.createElement(
				'a',
				{ onClick: this.onOrderBy.bind(null, field) },
				'\xA0',
				sortIcon
			);

			if (this.state.filterField && this.state.filterField == field) {
				filterIcon = _react2.default.createElement(
					'a',
					{ 'data-toggle': "campaign-filter-ddl-" + field },
					'\xA0',
					_react2.default.createElement('i', { className: 'fa fa-filter active' })
				);
			}
			if (dataSource) {
				var fieldValues = (0, _map3.default)(dataSource, function (i) {
					var fieldValue = i.get(field);
					var dateCheck = (0, _moment2.default)(fieldValue, _moment2.default.ISO_8601);
					if (dateCheck.isValid()) {
						return dateCheck.format("MMM DD, YYYY");
					}
					return fieldValue;
				});
				var menuItems = (0, _uniq3.default)(fieldValues).sort();
				filterMenu = _react2.default.createElement(
					'div',
					{ id: "campaign-filter-ddl-" + field,
						className: 'dropdown-pane bottom',
						style: { 'width': 'auto' },
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false' },
					_react2.default.createElement(
						'form',
						{ onSubmit: this.onFilter.bind(this, field) },
						_react2.default.createElement(
							'ul',
							{ className: 'vertical menu' },
							menuItems.map(function (item, index) {
								return _react2.default.createElement(
									'li',
									{ key: field + index },
									_react2.default.createElement('input', { id: field + index, name: field, type: 'checkbox', value: item }),
									_react2.default.createElement(
										'label',
										{ htmlFor: field + index },
										item
									)
								);
							})
						),
						_react2.default.createElement(
							'button',
							{ className: 'button tiny success', type: 'submit' },
							'Filter'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button tiny warning', type: 'reset', onClick: this.onClearFilter.bind(this, field) },
							'Clear'
						)
					)
				);
			}

			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'a',
					{ 'data-toggle': "campaign-filter-ddl-" + field },
					displayName
				),
				sortIcon,
				filterIcon,
				filterMenu
			);
		},
		getDataSource: function getDataSource() {
			var dataSource = this.getCollection();
			dataSource = dataSource ? dataSource.toArray() : [];
			if (this.state.orderByFiled) {
				dataSource = (0, _orderBy3.default)(dataSource, ['attributes.' + this.state.orderByFiled], [this.state.orderByAsc ? 'asc' : 'desc']);
			}
			if (this.state.search) {
				var keyword = this.state.search.toLowerCase(),
				    values = null;
				dataSource = (0, _filter3.default)(dataSource, function (i) {
					values = values(i.attributes);
					return (0, _some3.default)(values, function (i) {
						var dateCheck = (0, _moment2.default)(i, _moment2.default.ISO_8601);
						if (dateCheck.isValid()) {
							return dateCheck.format("MMM DD YYYY MMM DD, YYYY YYYY-MM-DD MM/DD/YYYY YYYY MM MMM DD").toLowerCase().indexOf(keyword) > -1;
						}
						return (0, _toString3.default)(i).toLowerCase().indexOf(keyword) > -1;
					});
				});
			} else if (this.state.filterField && this.state.filterValues) {
				var filterField = this.state.filterField,
				    filterValues = this.state.filterValues;
				dataSource = (0, _filter3.default)(dataSource, function (i) {
					var fieldValue = i.get(filterField);
					var dateCheck = (0, _moment2.default)(fieldValue, _moment2.default.ISO_8601);
					if (dateCheck.isValid()) {
						return (0, _indexOf3.default)(filterValues, dateCheck.format("MMM DD, YYYY")) > -1;
					}
					return (0, _indexOf3.default)(filterValues, fieldValue) > -1;
				});
			}
			return dataSource;
		},
		render: function render() {
			var list = this.getDataSource();
			return _react2.default.createElement(
				'div',
				{ className: 'section row' },
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'section-header' },
						_react2.default.createElement(
							'div',
							{ className: 'row', 'data-equalizer': true },
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'h5',
									null,
									'Campaign'
								)
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-8 column' },
								_react2.default.createElement(
									'nav',
									{ 'aria-label': 'You are here:', role: 'navigation' },
									_react2.default.createElement(
										'ul',
										{ className: 'breadcrumbs' },
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'a',
												{ href: '#' },
												'Control Center'
											)
										),
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'span',
												{ className: 'show-for-sr' },
												'Current: '
											),
											' Campaign'
										)
									)
								)
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-4 column' },
								_react2.default.createElement(
									'button',
									{ onClick: this.onNew, className: 'float-right section-button' },
									_react2.default.createElement('i', { className: 'fa fa-plus' }),
									_react2.default.createElement(
										'span',
										null,
										'New Campaign'
									)
								)
							)
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'scroll-list-section-body' },
						_react2.default.createElement(
							'div',
							{ className: 'row scroll-list-header' },
							_react2.default.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('ClientName', 'Client Name')
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-5 columns' },
								this.renderHeader('ClientCode', 'Client Code')
							),
							_react2.default.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('Date', 'Date')
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-3 columns' },
								_react2.default.createElement(
									'span',
									{ className: 'show-for-large' },
									this.renderHeader('AreaDescription', 'Type')
								)
							)
						),
						list.map(function (item) {
							return _react2.default.createElement(_row2.default, { key: item.get('Id'), model: item });
						})
					)
				)
			);
		}
	});

/***/ },
/* 368 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _adminList = __webpack_require__(74);

	var _adminList2 = _interopRequireDefault(_adminList);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		onUserSelected: function onUserSelected(user) {
			this.setState({
				selectedUser: user
			});
		},
		onDbUserSelected: function onDbUserSelected(user) {
			this.setState({
				selectedUser: user
			});
			this.publish('campaign/publish', user);
		},
		onClose: function onClose() {
			this.publish("showDialog");
		},
		onProcess: function onProcess() {
			if (this.state && this.state.selectedUser) {
				this.publish('campaign/publish', this.state.selectedUser);
			}
		},
		render: function render() {
			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'h5',
					null,
					'Campaign Publish'
				),
				_react2.default.createElement(
					'span',
					null,
					'Assign to'
				),
				_react2.default.createElement(_adminList2.default, { onSelect: this.onUserSelected, onDbSelect: this.onDbUserSelected, group: 'distribution' }),
				_react2.default.createElement(
					'div',
					{ className: 'float-right' },
					_react2.default.createElement(
						'button',
						{ className: 'success button', onClick: this.onProcess },
						'Okay'
					),
					_react2.default.createElement(
						'a',
						{ href: 'javascript:;', className: 'button', onClick: this.onClose },
						'Cancel'
					)
				)
			);
		}
	});

/***/ },
/* 369 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _edit = __webpack_require__(150);

	var _edit2 = _interopRequireDefault(_edit);

	var _publish = __webpack_require__(368);

	var _publish2 = _interopRequireDefault(_publish);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		menuKey: 'campaign-menu-ddl-',
		getDefaultProps: function getDefaultProps() {
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		componentDidMount: function componentDidMount() {
			(0, _jquery2.default)("#" + this.menuKey + this.getModel().get('Id')).foundation();
		},
		onCopy: function onCopy(e) {
			e.preventDefault();
			e.stopPropagation();
			(0, _jquery2.default)(e.currentTarget).closest('.dropdown-pane').foundation('close');
			var model = this.getModel(),
			    self = this;
			model.copy().then(function (response) {
				if (response && response.success) {
					var copiedModel = new Model(response.data);
					model.collection.add(copiedModel, {
						at: 0
					});
				}
			});
		},
		onEdit: function onEdit(e) {
			e.preventDefault();
			e.stopPropagation();
			(0, _jquery2.default)(e.currentTarget).closest('.dropdown-pane').foundation('close');
			var model = this.getModel().clone();
			var view = _react2.default.createElement(_edit2.default, { model: model });
			this.publish('showDialog', {
				view: view
			});
		},
		onDelete: function onDelete(e) {
			e.preventDefault();
			e.stopPropagation();
			(0, _jquery2.default)(e.currentTarget).closest('.dropdown-pane').foundation('close');
			var self = this;
			this.confirm('are you sure want delete this campaign?').then(function () {
				var model = self.getModel();
				model.destroy({
					wait: true,
					success: function success() {
						self.publish('camapign/refresh');
					}
				});
			});
		},
		onPublishToDMap: function onPublishToDMap(e) {
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel(),
			    self = this;

			this.publish('showDialog', _react2.default.createElement(_publish2.default, null));

			this.unsubscribe('campaign/publish');
			this.subscribe('campaign/publish', function (user) {
				model.publishToDMap(user).then(function (result) {
					self.publish('showDialog');
					if (result && result.success) {
						self.publish('camapign/refresh');
					} else {
						self.alert(result.error);
					}
				});
			});
		},
		onOpenMoreMenu: function onOpenMoreMenu(e) {
			e.preventDefault();
			e.stopPropagation();
		},
		onCloseMoreMenu: function onCloseMoreMenu(key, e) {
			e.preventDefault();
			e.stopPropagation();
			(0, _jquery2.default)("#" + this.menuKey + key).foundation('close');
		},
		renderMoreMenu: function renderMoreMenu(key) {
			var id = this.menuKey + key;
			return _react2.default.createElement(
				'span',
				null,
				_react2.default.createElement(
					'button',
					{ className: 'button cirle', 'data-toggle': id, onClick: this.onOpenMoreMenu },
					_react2.default.createElement('i', { className: 'fa fa-ellipsis-h' })
				),
				_react2.default.createElement(
					'div',
					{ id: id, className: 'dropdown-pane bottom',
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false',
						onClick: this.onCloseMoreMenu.bind(null, key) },
					_react2.default.createElement(
						'ul',
						{ className: 'vertical menu' },
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onEdit },
								_react2.default.createElement('i', { className: 'fa fa-edit' }),
								_react2.default.createElement(
									'span',
									null,
									'Edit'
								)
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onCopy },
								_react2.default.createElement('i', { className: 'fa fa-copy' }),
								_react2.default.createElement(
									'span',
									null,
									'Copy'
								)
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onDelete },
								_react2.default.createElement('i', { className: 'fa fa-trash' }),
								_react2.default.createElement(
									'span',
									null,
									'Delete'
								)
							)
						)
					)
				)
			);
		},
		onGotoCMap: function onGotoCMap(id) {
			window.location.hash = 'frame/campaign.aspx?cid=' + id;
		},
		render: function render() {
			var model = this.getModel();
			var date = model.get('Date');
			var displayDate = date ? (0, _moment2.default)(date).format("MMM DD, YYYY") : '';
			return _react2.default.createElement(
				'div',
				{ className: 'row scroll-list-item', onClick: this.onGotoCMap.bind(null, model.get('Id')) },
				_react2.default.createElement(
					'div',
					{ className: 'hide-for-small-only medium-2 columns' },
					model.get('ClientName')
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 medium-5 columns' },
					model.get('ClientCode')
				),
				_react2.default.createElement(
					'div',
					{ className: 'hide-for-small-only medium-2 columns' },
					displayDate
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 medium-3 columns' },
					_react2.default.createElement(
						'span',
						{ className: 'show-for-large' },
						model.get('AreaDescription')
					),
					_react2.default.createElement(
						'div',
						{ className: 'float-right tool-bar' },
						_react2.default.createElement(
							'button',
							{ onClick: this.onPublishToDMap, className: 'button' },
							_react2.default.createElement('i', { className: 'fa fa-upload' }),
							_react2.default.createElement(
								'small',
								null,
								'Publish'
							)
						),
						this.renderMoreMenu(model.get('Id'))
					)
				)
			);
		}
	});

/***/ },
/* 370 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _adminList = __webpack_require__(74);

	var _adminList2 = _interopRequireDefault(_adminList);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		componentWillMount: function componentWillMount() {},
		onUserSelected: function onUserSelected(user) {
			this.setState({
				selectedUser: user
			});
		},
		onDbUserSelected: function onDbUserSelected(user) {
			this.setState({
				selectedUser: user
			});
			this.publish('distribution/dismiss', user);
		},
		onClose: function onClose() {
			this.publish("showDialog");
		},
		onProcess: function onProcess() {
			if (this.state && this.state.selectedUser) {
				this.publish('distribution/dismiss', this.state.selectedUser);
			}
		},
		render: function render() {
			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'h5',
					null,
					'Campaign Publish'
				),
				_react2.default.createElement(
					'span',
					null,
					'Assign to'
				),
				_react2.default.createElement(_adminList2.default, { onSelect: this.onUserSelected, onDbSelect: this.onDbUserSelected, group: 'campaign' }),
				_react2.default.createElement(
					'div',
					{ className: 'float-right' },
					_react2.default.createElement(
						'button',
						{ className: 'success button', onClick: this.onProcess },
						'Okay'
					),
					_react2.default.createElement(
						'a',
						{ href: 'javascript:;', className: 'button', onClick: this.onClose },
						'Cancel'
					)
				)
			);
		}
	});

/***/ },
/* 371 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _indexOf2 = __webpack_require__(36);

	var _indexOf3 = _interopRequireDefault(_indexOf2);

	var _filter2 = __webpack_require__(58);

	var _filter3 = _interopRequireDefault(_filter2);

	var _toString2 = __webpack_require__(63);

	var _toString3 = _interopRequireDefault(_toString2);

	var _some2 = __webpack_require__(47);

	var _some3 = _interopRequireDefault(_some2);

	var _orderBy2 = __webpack_require__(125);

	var _orderBy3 = _interopRequireDefault(_orderBy2);

	var _uniq2 = __webpack_require__(128);

	var _uniq3 = _interopRequireDefault(_uniq2);

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _row = __webpack_require__(373);

	var _row2 = _interopRequireDefault(_row);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				orderByFiled: null,
				orderByAsc: false,
				search: null,
				filterField: null,
				filterValues: []
			};
		},
		componentDidMount: function componentDidMount() {
			var self = this;
			this.subscribe('distribution/refresh', function () {
				self.getCollection().fetchForDistribution();
			});
			this.subscribe('search', function (words) {
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			(0, _jquery2.default)("#distribution-filter-ddl-ClientName, #distribution-filter-ddl-ClientCode, #distribution-filter-ddl-Date, #distribution-filter-ddl-AreaDescription").foundation();
		},
		onOrderBy: function onOrderBy(field, reactObj, reactId, e) {
			e.preventDefault();
			e.stopPropagation();
			if (this.state.orderByFiled == field) {
				this.setState({
					orderByAsc: !this.state.orderByAsc
				});
			} else {
				this.setState({
					orderByFiled: field,
					orderByAsc: true
				});
			}
		},
		onFilter: function onFilter(field, e) {
			e.preventDefault();
			e.stopPropagation();
			var els = (0, _jquery2.default)(":checked", e.currentTarget),
			    values = (0, _map3.default)(els, function (item) {
				return (0, _jquery2.default)(item).val();
			});

			this.setState({
				filterField: values.length > 0 ? field : null,
				filterValues: values.length > 0 ? values : [],
				search: null
			});
			(0, _jquery2.default)('#distribution-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function onClearFilter(field, e) {
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			(0, _jquery2.default)('#distribution-filter-ddl-' + field).foundation('close');
			(0, _jquery2.default)(e.currentTarget).closest('form').get(0).reset();
		},
		renderHeader: function renderHeader(field, displayName) {
			var dataSource = this.getCollection(),
			    sortIcon = null,
			    filterIcon = null,
			    filterMenu = null;
			dataSource = dataSource ? dataSource.toArray() : [];

			if (this.state.orderByFiled == field) {
				if (this.state.orderByAsc) {
					sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort-up active' });
				} else {
					sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort-down active' });
				}
			} else {
				sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort' });
			}
			sortIcon = _react2.default.createElement(
				'a',
				{ onClick: this.onOrderBy.bind(null, field) },
				'\xA0',
				sortIcon
			);

			if (this.state.filterField && this.state.filterField == field) {
				filterIcon = _react2.default.createElement(
					'a',
					{ 'data-toggle': "distribution-filter-ddl-" + field },
					'\xA0',
					_react2.default.createElement('i', { className: 'fa fa-filter active' })
				);
			}
			if (dataSource) {
				var fieldValues = (0, _map3.default)(dataSource, function (i) {
					var fieldValue = i.get(field);
					var dateCheck = (0, _moment2.default)(fieldValue, _moment2.default.ISO_8601);
					if (dateCheck.isValid()) {
						return dateCheck.format("MMM DD, YYYY");
					}
					return fieldValue;
				});
				var menuItems = (0, _uniq3.default)(fieldValues).sort();
				filterMenu = _react2.default.createElement(
					'div',
					{ id: "distribution-filter-ddl-" + field,
						className: 'dropdown-pane bottom',
						style: { width: 'auto' },
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false' },
					_react2.default.createElement(
						'form',
						{ onSubmit: this.onFilter.bind(this, field) },
						_react2.default.createElement(
							'ul',
							{ className: 'vertical menu' },
							menuItems.map(function (item, index) {
								return _react2.default.createElement(
									'li',
									{ key: field + index },
									_react2.default.createElement('input', { id: field + index, name: field, type: 'checkbox', value: item }),
									_react2.default.createElement(
										'label',
										{ htmlFor: field + index },
										item
									)
								);
							})
						),
						_react2.default.createElement(
							'button',
							{ className: 'button tiny success', type: 'submit' },
							'Filter'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button tiny warning', type: 'reset', onClick: this.onClearFilter.bind(this, field) },
							'Clear'
						)
					)
				);
			}

			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'a',
					{ 'data-toggle': "distribution-filter-ddl-" + field },
					displayName
				),
				sortIcon,
				filterIcon,
				filterMenu
			);
		},
		getDataSource: function getDataSource() {
			var dataSource = this.getCollection();
			dataSource = dataSource ? dataSource.toArray() : [];
			if (this.state.orderByFiled) {
				dataSource = (0, _orderBy3.default)(dataSource, ['attributes.' + this.state.orderByFiled], [this.state.orderByAsc ? 'asc' : 'desc']);
			}
			if (this.state.search) {
				var keyword = this.state.search.toLowerCase(),
				    values = null;
				dataSource = (0, _filter3.default)(dataSource, function (i) {
					values = values(i.attributes);
					return (0, _some3.default)(values, function (i) {
						var dateCheck = (0, _moment2.default)(i, _moment2.default.ISO_8601);
						if (dateCheck.isValid()) {
							return dateCheck.format("MMM DD YYYY MMM DD, YYYY YYYY-MM-DD MM/DD/YYYY YYYY MM MMM DD").toLowerCase().indexOf(keyword) > -1;
						}
						return (0, _toString3.default)(i).toLowerCase().indexOf(keyword) > -1;
					});
				});
			} else if (this.state.filterField && this.state.filterValues) {
				var filterField = this.state.filterField,
				    filterValues = this.state.filterValues;
				dataSource = (0, _filter3.default)(dataSource, function (i) {
					var fieldValue = i.get(filterField);
					var dateCheck = (0, _moment2.default)(fieldValue, _moment2.default.ISO_8601);
					if (dateCheck.isValid()) {
						return (0, _indexOf3.default)(filterValues, dateCheck.format("MMM DD, YYYY")) > -1;
					}
					return (0, _indexOf3.default)(filterValues, fieldValue) > -1;
				});
			}
			return dataSource;
		},
		render: function render() {
			var list = this.getDataSource();
			return _react2.default.createElement(
				'div',
				{ className: 'section row' },
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'section-header' },
						_react2.default.createElement(
							'div',
							{ className: 'row' },
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'h5',
									null,
									'Distribution Maps'
								)
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'nav',
									{ 'aria-label': 'You are here:', role: 'navigation' },
									_react2.default.createElement(
										'ul',
										{ className: 'breadcrumbs' },
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'a',
												{ href: '#' },
												'Control Center'
											)
										),
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'span',
												{ className: 'show-for-sr' },
												'Current: '
											),
											' Distribution Maps'
										)
									)
								)
							)
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'scroll-list-section-body' },
						_react2.default.createElement(
							'div',
							{ className: 'row scroll-list-header' },
							_react2.default.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('ClientName', 'Client Name')
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-5 columns' },
								this.renderHeader('ClientCode', 'Client Code')
							),
							_react2.default.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('Date', 'Date')
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-3 columns' },
								_react2.default.createElement(
									'span',
									{ className: 'show-for-large' },
									this.renderHeader('AreaDescription', 'Type')
								)
							)
						),
						list.map(function (item) {
							return _react2.default.createElement(_row2.default, { key: item.get('Id'), model: item });
						})
					)
				)
			);
		}
	});

/***/ },
/* 372 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _adminList = __webpack_require__(74);

	var _adminList2 = _interopRequireDefault(_adminList);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		componentWillMount: function componentWillMount() {},
		onUserSelected: function onUserSelected(user) {
			this.setState({
				selectedUser: user
			});
		},
		onDbUserSelected: function onDbUserSelected(user) {
			this.setState({
				selectedUser: user
			});
			this.publish('distribution/publish', user);
		},
		onClose: function onClose() {
			this.publish("showDialog");
		},
		onProcess: function onProcess() {
			if (this.state && this.state.selectedUser) {
				this.publish('distribution/publish', this.state.selectedUser);
			}
		},
		render: function render() {
			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'h5',
					null,
					'Campaign Publish'
				),
				_react2.default.createElement(
					'span',
					null,
					'Assign to'
				),
				_react2.default.createElement(_adminList2.default, { onSelect: this.onUserSelected, onDbSelect: this.onDbUserSelected, group: 'monitor' }),
				_react2.default.createElement(
					'div',
					{ className: 'float-right' },
					_react2.default.createElement(
						'button',
						{ className: 'success button', onClick: this.onProcess },
						'Okay'
					),
					_react2.default.createElement(
						'a',
						{ href: 'javascript:;', className: 'button', onClick: this.onClose },
						'Cancel'
					)
				)
			);
		}
	});

/***/ },
/* 373 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _publish = __webpack_require__(372);

	var _publish2 = _interopRequireDefault(_publish);

	var _dismiss = __webpack_require__(370);

	var _dismiss2 = _interopRequireDefault(_dismiss);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getDefaultProps: function getDefaultProps() {
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		componentDidMount: function componentDidMount() {
			(0, _jquery2.default)('.has-tip').foundation();
		},
		componentDidUpdate: function componentDidUpdate() {},
		onDismiss: function onDismiss(e) {
			e.preventDefault();
			e.stopPropagation();

			var self = this,
			    model = this.getModel(),
			    msg = 'Are you sure you would like to move \r\n' + model.getDisplayName() + '\r\nto Campaigns? Any changes that were made will be lost.';

			this.confirm(msg).then(function () {
				self.publish('showDialog', {
					view: _dismiss2.default
				});
				self.unsubscribe('distribution/dismiss');
				self.subscribe('distribution/dismiss', function (user) {
					model.dismissToCampaign(user, {
						success: function success(result) {
							self.publish('showDialog');
							if (result && result.success) {
								self.publish('distribution/refresh');
							} else {
								alert("something wrong");
							}
						}
					});
				});
			});
		},
		onPublishToMonitors: function onPublishToMonitors(e) {
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel(),
			    self = this;
			this.publish('showDialog', {
				view: _publish2.default
			});
			this.unsubscribe('distribution/publish');
			this.subscribe('distribution/publish', function (user) {
				model.publishToMonitor(user, {
					success: function success(result) {
						self.publish('showDialog');
						if (result && result.success) {
							self.publish('distribution/refresh');
						} else {
							alert(result.error);
						}
					}
				});
			});
		},
		onGotoDMap: function onGotoDMap(id) {
			window.location.hash = 'frame/DistributionMap.aspx?cid=' + id;
		},
		render: function render() {
			var model = this.getModel();
			var date = model.get('Date');
			var displayDate = date ? (0, _moment2.default)(date).format("MMM DD, YYYY") : '';
			return _react2.default.createElement(
				'div',
				{ className: 'row scroll-list-item', onClick: this.onGotoDMap.bind(null, model.get('Id')) },
				_react2.default.createElement(
					'div',
					{ className: 'hide-for-small-only medium-2 columns' },
					model.get('ClientName')
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 medium-5 columns' },
					model.get('ClientCode')
				),
				_react2.default.createElement(
					'div',
					{ className: 'hide-for-small-only medium-2 columns' },
					displayDate
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 medium-3 columns' },
					_react2.default.createElement(
						'span',
						{ className: 'show-for-large' },
						model.get('AreaDescription')
					),
					_react2.default.createElement(
						'div',
						{ className: 'float-right tool-bar' },
						_react2.default.createElement(
							'button',
							{ onClick: this.onPublishToMonitors, className: 'button' },
							_react2.default.createElement('i', { className: 'fa fa-upload' }),
							_react2.default.createElement(
								'small',
								null,
								'Publish'
							)
						),
						_react2.default.createElement(
							'button',
							{ onClick: this.onDismiss, className: 'button has-tip top', title: 'dismiss',
								'data-tooltip': true, 'aria-haspopup': 'true',
								'data-disable-hover': 'false', tabIndex: '1' },
							_react2.default.createElement('i', { className: 'fa fa-reply' })
						)
					)
				)
			);
		}
	});

/***/ },
/* 374 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _filter2 = __webpack_require__(58);

	var _filter3 = _interopRequireDefault(_filter2);

	var _find2 = __webpack_require__(205);

	var _find3 = _interopRequireDefault(_find2);

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _groupBy2 = __webpack_require__(206);

	var _groupBy3 = _interopRequireDefault(_groupBy2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	var _server = __webpack_require__(346);

	var _server2 = _interopRequireDefault(_server);

	__webpack_require__(6);

	var _bluebird = __webpack_require__(13);

	var _bluebird2 = _interopRequireDefault(_bluebird);

	var _leaflet = __webpack_require__(95);

	var _leaflet2 = _interopRequireDefault(_leaflet);

	__webpack_require__(355);

	__webpack_require__(168);

	__webpack_require__(169);

	__webpack_require__(167);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _classnames = __webpack_require__(102);

	var _classnames2 = _interopRequireDefault(_classnames);

	var _jsts = __webpack_require__(166);

	var _gtu = __webpack_require__(69);

	var _gtu2 = _interopRequireDefault(_gtu);

	var _dmap = __webpack_require__(70);

	var _dmap2 = _interopRequireDefault(_dmap);

	var _user = __webpack_require__(97);

	var _user2 = _interopRequireDefault(_user);

	var _user3 = __webpack_require__(71);

	var _user4 = _interopRequireDefault(_user3);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _assign = __webpack_require__(151);

	var _assign2 = _interopRequireDefault(_assign);

	var _employee = __webpack_require__(159);

	var _employee2 = _interopRequireDefault(_employee);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

	var MapContainer = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		shouldComponentUpdate: function shouldComponentUpdate() {
			return false;
		},
		getInitialState: function getInitialState() {
			return {
				gtuMarkerLayer: {},
				gtuSnakeTrackLayerGroup: null,
				gtuAntTrackLayerGroup: null,
				showOutOfBoundaryGtu: false,
				drawMode: 'marker'
			};
		},
		onInit: function onInit(mapContainer) {
			var self = this;
			var monitorMap = _leaflet2.default.map(mapContainer, {
				center: {
					lat: 41.850033,
					lng: -87.6500523
				},
				zoom: 13,
				preferCanvas: false,
				animate: false,
				scrollWheelZoom: false
			});
			new _leaflet2.default.googleTile({
				mapTypeId: google.maps.MapTypeId.HYBRID
			}).addTo(monitorMap);

			/**
	   * prepare track pane
	   */
			monitorMap.createPane('GtuAntTrackPane', monitorMap.getPane('overlayPane'));
			monitorMap.createPane('GtuSnakeTrackPane', monitorMap.getPane('overlayPane'));
			monitorMap.createPane('GtuMarkerPane', monitorMap.getPane('markerPane'));

			this.setState({
				map: monitorMap
			}, function () {
				self.drawBoundary(monitorMap);
			});
			this.subscribe('Campaign.Monitor.ZoomToTask', function (taskId) {
				self.flyToTask(taskId);
			});
			this.subscribe('Campaign.Monitor.DrawGtu', function (task) {
				self.setState({
					task: task
				}, function () {
					self.drawGtu();
					window.clearInterval(self.state.reloadTimeout);
					self.setState({
						reloadTimeout: window.setInterval(self.reload, 30 * 1000)
					});
				});
			});
			this.subscribe('Campaign.Monitor.DrawMode', function (mode) {
				self.setState({
					drawMode: mode
				}, function () {
					self.drawGtu();
				});
			});
			this.subscribe('Campaign.Monitor.ShowOutOfBoundaryGtu', function (boolean) {
				self.setState({
					showOutOfBoundaryGtu: boolean
				}, function () {
					self.drawGtu();
				});
			});
		},
		drawBoundary: function drawBoundary(monitorMap) {
			var self = this,
			    model = this.getModel(),
			    campaignId = model.get('Id'),
			    tasks = model.get('Tasks'),
			    submapTasks = (0, _groupBy3.default)(tasks.models, function (t) {
				return t.get('SubMapId');
			}),
			    promiseSubmapArray = [],
			    promiseTaskArray = [],
			    taskBoundaryLayerGroup = _leaflet2.default.layerGroup(),
			    boundaryBounds = _leaflet2.default.latLngBounds();

			(0, _each3.default)(submapTasks, function (groupTasks, submapId) {
				promiseSubmapArray.push(_jquery2.default.getJSON('../api/print/campaign/' + campaignId + '/submap/' + submapId + '/boundary'));
			});
			return _bluebird2.default.all(promiseSubmapArray).then(function (data) {
				return _bluebird2.default.each(data, function (result) {
					var latlngs = (0, _map3.default)(result.boundary, function (i) {
						boundaryBounds.extend(i);
						return [i.lat, i.lng];
					});
					_leaflet2.default.polyline(latlngs, {
						color: 'rgb(' + result.color.r + ', ' + result.color.g + ', ' + result.color.b + ')',
						weight: 3,
						noClip: true,
						dropShadow: true
					}).addTo(monitorMap);
					return _bluebird2.default.resolve();
				});
			}).then(function () {
				(0, _each3.default)(submapTasks, function (groupTasks) {
					(0, _each3.default)(groupTasks, function (task) {
						promiseTaskArray.push(new _bluebird2.default(function (resolve, reject) {
							var url = '../api/print/campaign/' + task.get('CampaignId') + '/submap/' + task.get('SubMapId') + '/dmap/' + task.get('DMapId') + '/boundary';
							_jquery2.default.getJSON(url).then(function (result) {
								var latlngArray = []; //for draw map polygon
								var coordinateList = []; //for use jsts get polygon center
								(0, _each3.default)(result.boundary, function (i) {
									latlngArray.push([i.lat, i.lng]);
									coordinateList.push(new _jsts.geom.Coordinate(i.lng, i.lat));
								});
								var factory = new _jsts.geom.GeometryFactory();
								var boundaryLineRing = factory.createLinearRing(coordinateList);
								var polygon = factory.createPolygon(boundaryLineRing);
								var center = polygon.getCentroid();
								var color = 'rgb(' + result.color.r + ', ' + result.color.g + ', ' + result.color.b + ')';
								var opt = {
									taskId: task.get('Id'),
									center: {
										lat: center.getY(),
										lng: center.getX()
									},
									text: {
										text: '' + task.get('Name')
									},
									weight: 1,
									color: color,
									opacity: 0.75,
									fill: true,
									fillColor: !task.get('IsFinished') ? color : '#000',
									fillOpacity: !task.get('IsFinished') ? 0.1 : 0.75,
									noClip: true,
									clickable: !task.get('IsFinished'),
									dropShadow: !task.get('IsFinished'),
									fillPattern: !task.get('IsFinished') ? null : {
										url: 'data:image/svg+xml;base64,PHN2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHdpZHRoPSc4JyBoZWlnaHQ9JzgnPgogIDxyZWN0IHdpZHRoPSc4JyBoZWlnaHQ9JzgnIGZpbGw9JyNmZmYnLz4KICA8cGF0aCBkPSdNMCAwTDggOFpNOCAwTDAgOFonIHN0cm9rZS13aWR0aD0nMC41JyBzdHJva2U9JyNhYWEnLz4KPC9zdmc+Cg==',
										pattern: {
											width: '8px',
											height: '8px',
											patternUnits: 'userSpaceOnUse',
											patternContentUnits: 'Default'
										},
										image: {
											width: '8px',
											height: '8px'
										}
									}
								};
								var boundary = _leaflet2.default.polyline(latlngArray, opt).on('click', self.onTaskAreaClickHandler, self);
								boundary.getCenter = function () {
									return {
										lat: center.getY(),
										lng: center.getX()
									};
								};
								taskBoundaryLayerGroup.addLayer(boundary);
								resolve();
							});
						}));
					});
				});

				return _bluebird2.default.all(promiseTaskArray);
			}).then(function () {
				taskBoundaryLayerGroup.addTo(monitorMap);
				monitorMap.flyToBounds(boundaryBounds);
				self.setState({
					taskBoundaryLayerGroup: taskBoundaryLayerGroup
				});
			});
		},
		buildTaskPopup: function buildTaskPopup(task) {
			var self = this;
			var taskStatus = null;
			if (task.get('IsFinished')) {
				taskStatus = [_react2.default.createElement(
					'span',
					{ key: 'finished' },
					'FINISHED'
				)];
			} else {
				switch (task.get('Status')) {
					case 0:
						//started
						taskStatus = [_react2.default.createElement(
							'span',
							{ key: 'started' },
							'STARTED'
						)];
						break;
					case 1:
						//stoped
						taskStatus = [_react2.default.createElement(
							'span',
							{ key: 'stopped' },
							'STOPPED'
						)];
						break;
					case 2:
						//peased
						taskStatus = [_react2.default.createElement(
							'span',
							{ key: 'peased' },
							'PEASED'
						)];
						break;
					default:
						taskStatus = [_react2.default.createElement(
							'span',
							{ key: 'started' },
							'NOT STARTED'
						)];
						break;
				}
			}
			var gtuList = [];
			var templat = _react2.default.createElement(
				'div',
				{ className: 'row section', style: { 'min-width': '320px;' } },
				_react2.default.createElement(
					'div',
					{ className: 'small-7 columns' },
					task.get('Name')
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-5 columns text-right' },
					taskStatus
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'row small-up-4' },
						(0, _map3.default)(gtuList, function (gtu) {
							return self.buildGtuInPopup(task, gtu);
						})
					)
				)
			);

			return _server2.default.renderToStaticMarkup(templat);
		},
		onTaskAreaClickHandler: function onTaskAreaClickHandler(evt) {
			console.log(this);
			var self = this;
			var taskAreaCenter = evt.target.options.center,
			    taskId = evt.target.options.taskId,
			    allTask = this.getModel().get('Tasks'),
			    task = (0, _find3.default)(allTask.models, function (i) {
				return i.get('Id') == taskId;
			}),
			    popContent = self.buildTaskPopup(task);
			_leaflet2.default.popup({
				maxWidth: 640
			}).setLatLng(taskAreaCenter).setContent(popContent).openOn(this.state.map);
		},
		reload: function reload() {
			if (!this.state.task) {
				return;
			}
			var self = this,
			    task = this.state.task,
			    dmap = task.get('dmap'),
			    gtus = task.get('gtuList');

			return _bluebird2.default.all([dmap.updateGtuAfterTime(null, {
				quite: true
			}), gtus.fetchGtuLocation(task.get('Id'), {
				quite: true
			})]).then(function () {
				self.drawGtu();
			});
		},
		drawGtu: function drawGtu() {
			var map = this.state.map;
			var task = this.state.task;
			if (!map || !task) {
				return;
			}
			var gtus = task.get('gtuTrack');
			this.drawGtuMarker(gtus);
			this.drawGtuTrackSnake(gtus);
			this.drawGtuTrackAnt(gtus);
		},
		drawGtuMarker: function drawGtuMarker(gtus) {
			var _this = this;

			var self = this;
			var monitorMap = this.state.map;
			if (!monitorMap) {
				return;
			}
			if (this.state.drawMode != 'marker') {
				(0, _each3.default)(this.state.gtuMarkerLayer, function (layer) {
					layer.remove();
				});
				return;
			}
			(0, _each3.default)(gtus, function (data) {
				if (data.points && data.points.length > 0) {
					var gtuId = data.points[0].Id;
					var dataLayer = _this.state.gtuMarkerLayer[gtuId];
					if (!dataLayer) {
						var _layerOptions;

						var gtuRadiusFunction = new _leaflet2.default.LinearFunction(new _leaflet2.default.Point(0, 3), new _leaflet2.default.Point(1, 10));
						var dataLayer = new _leaflet2.default.MapMarkerDataLayer(data, {
							recordsField: 'points',
							locationMode: _leaflet2.default.LocationModes.LATLNG,
							latitudeField: 'lat',
							longitudeField: 'lng',
							tooltipOptions: null,
							getMarker: function getMarker(location, options, record) {
								return new _leaflet2.default.RegularPolygonMarker(location, options);
							},
							filter: function filter(value) {
								return self.state.showOutOfBoundaryGtu ? true : value.out == false;
							},
							layerOptions: (_layerOptions = {
								weight: 0.1,
								opacity: 0.7,
								fillColor: data.color,
								fillOpacity: 1.0,
								fill: true,
								stroke: true,
								numberOfSides: 50
							}, _defineProperty(_layerOptions, 'stroke', false), _defineProperty(_layerOptions, 'fillOpacity', 0.75), _defineProperty(_layerOptions, 'dropShadow', true), _defineProperty(_layerOptions, 'radius', 5), _defineProperty(_layerOptions, 'clickable', false), _defineProperty(_layerOptions, 'noClip', false), _defineProperty(_layerOptions, 'showLegendTooltips', false), _defineProperty(_layerOptions, 'pane', 'GtuMarkerPane'), _defineProperty(_layerOptions, 'gradient', function gradient() {
								return {
									gradientType: 'radial',
									stops: [{
										offset: '0%',
										style: {
											color: data.color,
											opacity: 1
										}
									}, {
										offset: '30%',
										style: {
											color: data.color,
											opacity: 0.5
										}
									}, {
										offset: '100%',
										style: {
											color: data.color,
											opacity: 0
										}
									}]
								};
							}), _layerOptions)
						});
						self.state.gtuMarkerLayer[gtuId] = dataLayer;
					} else {
						dataLayer.setData(data);
					}
					dataLayer.addTo(monitorMap);
				}
			});
		},
		drawGtuTrackSnake: function drawGtuTrackSnake(gtus) {
			var monitorMap = this.state.map;
			if (!monitorMap) {
				return;
			}
			var trackLayerGroup = this.state.gtuSnakeTrackLayerGroup;
			if (this.state.drawMode != 'snakeTrack') {
				trackLayerGroup && trackLayerGroup.remove();
				return;
			}

			if (!trackLayerGroup) {
				trackLayerGroup = _leaflet2.default.layerGroup();
				this.setState({
					gtuSnakeTrackLayerGroup: trackLayerGroup
				});
			}
			(0, _each3.default)(gtus, function (data) {
				if (data.points && data.points.length > 0) {
					var gtuId = data.points[0].Id;
					var gtuLayer = null;
					trackLayerGroup.eachLayer(function (layer) {
						if (layer.options.gtuId == gtuId) {
							gtuLayer = layer;
						}
					});
					if (!gtuLayer) {
						var latlngs = (0, _map3.default)(data.points, function (p) {
							return {
								lat: p.lat,
								lng: p.lng
							};
						});
						_leaflet2.default.polyline(latlngs, {
							gtuId: gtuId,
							weight: 2,
							color: data.color,
							opacity: 0.75,
							noClip: false,
							dropShadow: true,
							snakingSpeed: 200,
							pane: 'GtuSnakeTrackPane'
						}).addTo(trackLayerGroup);
					}
				}
			});

			trackLayerGroup.addTo(monitorMap).snakeIn();
		},
		drawGtuTrackAnt: function drawGtuTrackAnt(gtus) {
			var monitorMap = this.state.map;
			if (!monitorMap) {
				return;
			}
			var trackLayerGroup = this.state.gtuAntTrackLayerGroup;
			if (this.state.drawMode != 'antTrack') {
				trackLayerGroup && trackLayerGroup.remove();
				return;
			}

			if (!trackLayerGroup) {
				trackLayerGroup = _leaflet2.default.layerGroup();
				this.setState({
					gtuAntTrackLayerGroup: trackLayerGroup
				});
			}
			(0, _each3.default)(gtus, function (data) {
				if (data.points && data.points.length > 0) {
					var gtuId = data.points[0].Id;
					var gtuLayer = null;
					trackLayerGroup.eachLayer(function (layer) {
						if (layer.options.gtuId == gtuId) {
							gtuLayer = layer;
						}
					});
					if (!gtuLayer) {
						var latlngs = (0, _map3.default)(data.points, function (p) {
							return {
								lat: p.lat,
								lng: p.lng
							};
						});
						new _leaflet2.default.Polyline.AntPath(latlngs, {
							gtuId: gtuId,
							weight: 2,
							color: data.color,
							opacity: 0.75,
							noClip: false,
							dropShadow: true,
							delay: 15 * 1000,
							dashArray: [5, 40],
							pane: 'GtuAntTrackPane'
						}).addTo(trackLayerGroup);
					}
				}
			});

			trackLayerGroup.addTo(monitorMap);
		},
		flyToTask: function flyToTask(taskId) {
			var map = this.state.map;
			if (!map) {
				return;
			}
			this.state.taskBoundaryLayerGroup.eachLayer(function (layer) {
				if (layer.options.taskId == taskId) {
					var taskBounds = layer.getBounds();
					map.flyToBounds(taskBounds);
				}
			});
		},
		render: function render() {
			return _react2.default.createElement(
				'div',
				{ className: 'row' },
				_react2.default.createElement(
					'div',
					{ className: 'small-12' },
					_react2.default.createElement('div', { className: 'leaflet-map-container', ref: this.onInit, style: { 'minHeight': '640px' } })
				)
			);
		}
	});

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				activeTask: null
			};
		},
		componentDidMount: function componentDidMount() {
			var self = this;
			this.subscribe('Global.Window.Click', function () {
				self.onCloseDropDown();
				self.onCloseMoreMenu();
			});
		},
		showTask: function showTask(task) {
			this.publish('Campaign.Monitor.ZoomToTask', task);
		},
		onSwitchActiveTask: function onSwitchActiveTask(task) {
			var self = this;
			var taskDMap = new _dmap2.default({
				CampaignId: task.get('CampaignId'),
				SubMapId: task.get('SubMapId'),
				DMapId: task.get('DMapId'),
				Gtu: []
			});
			var trackQuery = taskDMap.updateGtuAfterTime(null, {
				quite: true
			});

			var gtu = new _gtu2.default();
			var locationQuery = gtu.fetchGtuWithStatusByTask(task.get('Id'), {
				quite: true
			}).then(function () {
				return gtu.fetchGtuLocation(task.get('Id'), {
					quite: true
				});
			});

			return _bluebird2.default.all([trackQuery, locationQuery]).then(function () {
				task.set('dmap', taskDMap);
				task.set('gtuList', gtu);
				self.setState({
					activeTask: task
				}, function () {
					self.publish('Campaign.Monitor.ZoomToTask', task.get('Id'));
					self.publish('Campaign.Monitor.DrawGtu', task);
				});
			});
		},
		onOpenDropDown: function onOpenDropDown(evt) {
			evt && evt.preventDefault();
			evt && evt.stopPropagation();
			this.setState({
				taskDropdownActive: true
			});
		},
		onCloseDropDown: function onCloseDropDown(evt) {
			evt && evt.preventDefault();
			evt && evt.stopPropagation();
			this.setState({
				taskDropdownActive: false
			});
		},
		onOpenMoreMenu: function onOpenMoreMenu(evt) {
			evt && evt.preventDefault();
			evt && evt.stopPropagation();
			this.setState({
				taskMoreMenuActive: true
			});
		},
		onCloseMoreMenu: function onCloseMoreMenu(evt) {
			evt && evt.preventDefault();
			evt && evt.stopPropagation();
			this.setState({
				taskMoreMenuActive: false
			});
		},
		onStart: function onStart(task) {
			task.setStart();
		},
		onStop: function onStop(task) {
			task.setStop();
		},
		onPause: function onPause(task) {
			task.setPause();
		},
		onSwitchDisplayMode: function onSwitchDisplayMode(mode) {
			var self = this;
			this.setState({
				displayMode: mode
			}, function () {
				self.publish('Campaign.Monitor.DrawMode', mode);
			});
		},
		onSwitchShowOutOfBoundaryGtu: function onSwitchShowOutOfBoundaryGtu() {
			var showOutOfBoundaryGtu = !this.state.ShowOutOfBoundaryGtu;
			this.setState({
				ShowOutOfBoundaryGtu: showOutOfBoundaryGtu
			}, function () {
				self.publish('Campaign.Monitor.ShowOutOfBoundaryGtu', showOutOfBoundaryGtu);
			});
		},
		onAddEmployee: function onAddEmployee() {
			var user = new _user2.default(),
			    self = this;
			user.fetchCompany().done(function () {
				self.publish('showDialog', _employee2.default, {
					model: new _user4.default(),
					company: user
				});
			});
		},
		onAssign: function onAssign() {
			var gtu = this.state.activeTask.get('gtu'),
			    taskId = this.state.activeTask.get('Id'),
			    user = new _user2.default(),
			    self = this;
			user.fetchForGtu().done(function () {
				self.publish('showDialog', {
					view: _assign2.default,
					params: {
						collection: gtu,
						user: user,
						taskId: taskId
					},
					options: {
						size: 'full'
					}
				});
			});
		},
		onReCenter: function onReCenter() {
			this.publish('', this.state.activeTask.get('Id'));
		},
		onReload: function onReload() {
			this.onSwitchActiveTask(this.state.activeTask);
		},
		onGotoGTU: function onGotoGTU() {},
		onSelectedGTU: function onSelectedGTU() {},
		renderTaskDropdown: function renderTaskDropdown() {
			var self = this,
			    model = this.getModel(),
			    tasks = model && model.get('Tasks') ? model.get('Tasks').models : [];

			tasks = (0, _filter3.default)(tasks, function (t) {
				return !t.get('IsFinished');
			});
			var parentClass = (0, _classnames2.default)({
				'is-dropdown-submenu-parent': true,
				'opens-left': true,
				'is-active': this.state.taskDropdownActive
			});
			var menuClass = (0, _classnames2.default)({
				'menu': true,
				'submenu': true,
				'is-dropdown-submenu': true,
				'first-sub': true,
				'vertical': true,
				'js-dropdown-active': this.state.taskDropdownActive
			});
			if (tasks.length > 10) {
				menuClass = (0, _classnames2.default)('section row collapse small-up-2 medium-up-3 large-up-4', {
					'menu': true,
					'submenu': true,
					'is-dropdown-submenu': true,
					'first-sub': true,
					'vertical': true,
					'js-dropdown-active': this.state.taskDropdownActive
				});
				return _react2.default.createElement(
					'ul',
					{ className: 'dropdown menu float-right' },
					_react2.default.createElement(
						'li',
						{ className: parentClass },
						_react2.default.createElement(
							'a',
							{ href: 'javascript:;', onClick: this.onOpenDropDown },
							'Switch Active Task'
						),
						_react2.default.createElement(
							'div',
							{ style: { 'min-width': '768px' }, className: menuClass, onClick: this.onCloseDropDown },
							(0, _map3.default)(tasks, function (t) {
								return _react2.default.createElement(
									'div',
									{ className: 'column', key: t.get('Id') },
									_react2.default.createElement(
										'a',
										{ href: 'javascript:;', style: { width: '100%' }, className: 'button row-button text-left', onClick: self.onSwitchActiveTask.bind(self, t) },
										t.get('Name')
									)
								);
							})
						)
					)
				);
			}
			return _react2.default.createElement(
				'ul',
				{ className: 'dropdown menu float-right' },
				_react2.default.createElement(
					'li',
					{ className: parentClass },
					_react2.default.createElement(
						'a',
						{ href: 'javascript:;', onClick: this.onOpenDropDown },
						'Switch Active Task'
					),
					_react2.default.createElement(
						'ul',
						{ className: menuClass, onClick: this.onCloseDropDown },
						(0, _map3.default)(tasks, function (t) {
							return _react2.default.createElement(
								'li',
								{ key: t.get('Id') },
								_react2.default.createElement(
									'a',
									{ href: 'javascript:;', onClick: self.onSwitchActiveTask.bind(self, t) },
									t.get('Name')
								)
							);
						})
					)
				)
			);
		},
		renderActiveTask: function renderActiveTask() {
			if (!this.state.activeTask) {
				return null;
			}
			var self = this;
			var activeTask = this.state.activeTask;
			var taskIsFinished = activeTask.get('IsFinished');
			var gtuList = activeTask.get('gtuList') || [];
			if (gtuList.where) {
				gtuList = gtuList.where(function (i) {
					if (taskIsFinished) {
						return i.get('WithData');
					} else {
						return i.get('IsAssign') || i.get('WithData');
					}
				});
			}

			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'div',
					{ className: 'section row gtu-monitor' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'row' },
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-5 large-3 columns' },
								this.renderTaskController(activeTask)
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-7 large-9 columns' },
								this.renderTaskMoreMenu(activeTask),
								_react2.default.createElement(
									'button',
									{ className: 'button float-right', onClick: this.onReCenter },
									_react2.default.createElement('i', { className: 'fa fa-crosshairs' }),
									_react2.default.createElement(
										'span',
										null,
										'Center'
									)
								),
								_react2.default.createElement(
									'button',
									{ className: 'button float-right', onClick: this.onReload },
									_react2.default.createElement('i', { className: 'fa fa-refresh' }),
									_react2.default.createElement(
										'span',
										null,
										'Refresh'
									)
								)
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row gtu' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						(0, _map3.default)(gtuList, function (gtu) {
							return self.renderGtu(gtu);
						})
					)
				)
			);
		},
		renderGtu: function renderGtu(gtu) {
			var typeIcon = null,
			    alertIcon = null,
			    deleteIcon = null,
			    buttonClass = 'button text-left',
			    taskIsStopped = this.state.activeTask.get('Status') == 1,

			// isActive = indexOf(this.state.activeGtu, gtu.get('Id')) > -1,
			isActive = true,
			    gtuIcon = null;

			if (taskIsStopped) {
				gtuIcon = _react2.default.createElement('i', { className: 'fa fa-stop' });
			} else {
				switch (gtu.get('Role')) {
					case 'Driver':
						gtuIcon = _react2.default.createElement('i', { className: 'fa fa-truck' });
						break;
					case 'Walker':
						gtuIcon = _react2.default.createElement('i', { className: 'fa fa-street-view' });
						break;
					default:
						gtuIcon = null;
						break;
				}
			}

			if (isActive) {
				buttonClass += ' active';
			}
			if (!taskIsStopped && !gtu.get('IsOnline')) {
				buttonClass += ' offline';
			}
			if (!taskIsStopped && gtu.get('IsOnline') && gtu.get('OutOfBoundary')) {
				alertIcon = _react2.default.createElement('i', { className: 'fa fa-bell faa-ring animated alert' });
			}
			if (!taskIsStopped && gtu.get('WithData')) {
				deleteIcon = _react2.default.createElement('i', { className: 'fa fa-warning alert' });
			}
			return _react2.default.createElement(
				'span',
				{ className: 'group', key: gtu.get('Id') },
				_react2.default.createElement(
					'button',
					{ className: buttonClass, style: { 'backgroundColor': isActive ? gtu.get('UserColor') : 'transparent' }, onClick: this.onSelectedGTU.bind(null, gtu.get('Id')) },
					deleteIcon,
					gtuIcon,
					'\xA0\xA0',
					_react2.default.createElement(
						'span',
						null,
						gtu.get('ShortUniqueID')
					),
					'\xA0\xA0',
					alertIcon
				),
				_react2.default.createElement(
					'button',
					{ className: 'button location', onClick: this.onGotoGTU.bind(null, gtu.get('Id')) },
					_react2.default.createElement('i', { className: 'location fa fa-crosshairs', style: { color: 'black' } })
				)
			);
		},
		renderTaskController: function renderTaskController(task) {
			switch (task.get('Status')) {
				case 0:
					//started
					return _react2.default.createElement(
						'div',
						null,
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onPause.bind(this, task) },
							_react2.default.createElement('i', { className: 'fa fa-pause' }),
							'Pause'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onStop.bind(this, task) },
							_react2.default.createElement('i', { className: 'fa fa-stop' }),
							'Stop'
						)
					);
					break;
				case 1:
					//stoped
					return _react2.default.createElement(
						'h5',
						null,
						'STOPPED'
					);
					break;
				case 2:
					//peased
					return _react2.default.createElement(
						'div',
						null,
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onStart.bind(this, task) },
							_react2.default.createElement('i', { className: 'fa fa-play' }),
							'Start'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onStop.bind(this, task) },
							_react2.default.createElement('i', { className: 'fa fa-stop' }),
							'Stop'
						)
					);
					break;
				default:
					return _react2.default.createElement(
						'div',
						null,
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onStart.bind(this, task) },
							_react2.default.createElement('i', { className: 'fa fa-play' }),
							'Start'
						)
					);
					break;
			}
		},
		renderTaskMoreMenu: function renderTaskMoreMenu(task) {
			if (task.get('Status') == 1) {
				var assignButton = null;
			} else {
				var assignButton = _react2.default.createElement(
					'li',
					null,
					_react2.default.createElement(
						'a',
						{ href: 'javascript:;', onClick: this.onAssign.bind(this, task) },
						_react2.default.createElement('i', { className: 'fa fa-users' }),
						'\xA0',
						_react2.default.createElement(
							'span',
							null,
							'Assign GTU'
						)
					)
				);
			}
			var parentClass = (0, _classnames2.default)({
				'is-dropdown-submenu-parent': true,
				'opens-left': true,
				'is-active': this.state.taskMoreMenuActive
			});
			var menuClass = (0, _classnames2.default)({
				'menu': true,
				'submenu': true,
				'is-dropdown-submenu': true,
				'first-sub': true,
				'vertical': true,
				'js-dropdown-active': this.state.taskMoreMenuActive
			});
			return _react2.default.createElement(
				'ul',
				{ className: 'dropdown menu float-right' },
				_react2.default.createElement(
					'li',
					{ className: parentClass },
					_react2.default.createElement(
						'button',
						{ className: 'button cirle', 'data-toggle': 'monitor-more-menu', onClick: this.onOpenMoreMenu },
						_react2.default.createElement('i', { className: 'fa fa-ellipsis-h' })
					),
					_react2.default.createElement(
						'ul',
						{ className: menuClass, onClick: this.onCloseMoreMenu },
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onAddEmployee },
								_react2.default.createElement('i', { className: 'fa fa-user-plus' }),
								'\xA0',
								_react2.default.createElement(
									'span',
									null,
									'New Employee'
								)
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onSwitchDisplayMode.bind(this, 'marker') },
								'Show Coverage'
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onSwitchDisplayMode.bind(this, 'snakeTrack') },
								'Track Path (Snake Style)'
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onSwitchDisplayMode.bind(this, 'antTrack') },
								'Track Path (Ant Style)'
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onSwitchShowOutOfBoundaryGtu },
								_react2.default.createElement('i', { className: !this.state.ShowOutOfBoundaryGtu ? 'fa fa-compress' : 'fa fa-expand' }),
								'\xA0',
								_react2.default.createElement(
									'span',
									null,
									!this.state.ShowOutOfBoundaryGtu ? 'Show Out of Bounds' : 'Hide Out of Bounds'
								)
							)
						)
					)
				)
			);
		},
		render: function render() {
			var self = this,
			    model = this.getModel();
			return _react2.default.createElement(
				'div',
				{ className: 'campaign-monitor' },
				_react2.default.createElement(
					'div',
					{ className: 'section row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'section-header' },
							_react2.default.createElement(
								'div',
								{ className: 'row' },
								_react2.default.createElement(
									'div',
									{ className: 'small-10 column' },
									_react2.default.createElement(
										'blockquote',
										null,
										_react2.default.createElement(
											'h5',
											null,
											model.getDisplayName()
										),
										_react2.default.createElement(
											'cite',
											null,
											this.state.activeTask ? this.state.activeTask.get('Name') : 'NO ACTIVE TASK'
										)
									)
								),
								_react2.default.createElement(
									'div',
									{ className: 'small-2 column' },
									this.renderTaskDropdown()
								)
							)
						)
					)
				),
				this.renderActiveTask(),
				_react2.default.createElement(MapContainer, { model: this.getModel() })
			);
		}
	});

/***/ },
/* 375 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _fastMarker = __webpack_require__(98);

	var _fastMarker2 = _interopRequireDefault(_fastMarker);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _mapBase = __webpack_require__(72);

	var _mapBase2 = _interopRequireDefault(_mapBase);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	var dmapPolygon = null,
	    dmapBounds = null,
	    gtuData = null,
	    gtuPoints = [];
	var TAG = '[GTU-EDIT]';
	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default, _mapBase2.default],
		getInitialState: function getInitialState() {
			return {
				disableDefaultUI: false,
				scrollwheel: false,
				disableDoubleClickZoom: true,
				activeGtu: null,
				customPoints: [],
				maxDisplayCount: 2000
			};
		},
		onWindowResize: function onWindowResize() {
			if (this.refs.mapArea) {
				var pageLeftHeight = (0, _jquery2.default)(window).height() - (0, _jquery2.default)(this.refs.mapArea).position().top;
				this.setMapHeight(pageLeftHeight);
			}
		},
		componentDidMount: function componentDidMount() {
			this.publish('showLoading');
			/**
	   * position google map to main area
	   */
			(0, _jquery2.default)(window).on('resize.gtu-edit-view', _jquery2.default.proxy(this.onWindowResize, this));
			this.onWindowResize();

			this.setState({
				activeGtu: this.props.gtu.at(0)
			});

			var self = this,
			    googleMap = this.getGoogleMap();
			this.drawDmapBoundary().then(this.filterGtu).then(this.drawGtu).done(function () {
				dmapPolygon.addListener('click', _jquery2.default.proxy(self.onNewGtu, self));
				// googleMap.addListener('zoom_changed', $.proxy(self.drawGtu, self));
				// googleMap.addListener('dragend', $.proxy(self.drawGtu, self));
				self.publish('hideLoading');
			});
		},
		componentWillUnmount: function componentWillUnmount() {
			var googleMap = this.getGoogleMap();
			try {
				(0, _each3.default)(gtuPoints, function (item) {
					item.setMap(null);
				});
				dmapPolygon.setMap(null);
				google.maps.event.clearInstanceListeners(googleMap);
				(0, _jquery2.default)(document).off('resize.gtu-edit-view');
			} catch (ex) {
				console.log(TAG + ' google map clear error', ex);
			}
		},
		setActiveGtu: function setActiveGtu(gtuId) {
			var activeGut = this.props.gtu.get(gtuId);
			this.setState({
				activeGtu: activeGut
			});
		},
		getCirclePath: function getCirclePath(size) {
			return 'M-' + size + ',0a' + size + ',' + size + ' 0 1,0 ' + size * 2 + ',0a' + size + ',' + size + ' 0 1,0 -' + size * 2 + ',0';
		},
		drawDmapBoundary: function drawDmapBoundary() {
			var def = _jquery2.default.Deferred(),
			    boundary = this.props.dmap.get('Boundary'),
			    fillColor = this.props.dmap.get('Color'),
			    googleMap = this.getGoogleMap(),
			    timeout = null;

			dmapBounds = new google.maps.LatLngBounds();
			dmapPolygon = new google.maps.Polygon({
				paths: boundary,
				strokeColor: '#000',
				strokeOpacity: 1,
				strokeWeight: 6,
				fillColor: fillColor,
				fillOpacity: 0.1,
				map: googleMap
			});

			(0, _each3.default)(boundary, function (i) {
				var point = new google.maps.LatLng(i.lat, i.lng);
				dmapBounds.extend(point);
			});
			google.maps.event.addListenerOnce(googleMap, 'tilesloaded', function () {
				window.clearTimeout(timeout);
				def.resolve();
			});
			googleMap.fitBounds(dmapBounds);
			timeout = window.setTimeout(function () {
				def.resolve();
			}, 5 * 60 * 1000);
			return def;
		},
		filterGtu: function filterGtu(fnFilter) {
			var def = _jquery2.default.Deferred(),
			    dots = this.props.dmap.get('Gtu') || [],
			    result = [];
			(0, _each3.default)(dots, function (gtu) {
				var filterGtu = groupBy(gtu.points, fnFilter);
				(0, _each3.default)(filterGtu, function (v, k) {
					var latlng = k.split(':');
					result.push({
						lat: parseFloat(latlng[0]),
						lng: parseFloat(latlng[1]),
						color: gtu.color
					});
				});
			});
			return result;
		},
		prepareGtu: function prepareGtu() {
			var gtu3 = this.filterGtu(function (latlng) {
				return Math.round(latlng.lat * 1000) / 1000 + ':' + Math.round(latlng.lng * 1000) / 1000;
			}),
			    gtu4 = this.filterGtu(function (latlng) {
				return Math.round(latlng.lat * 10000) / 10000 + ':' + Math.round(latlng.lng * 10000) / 10000;
			}),
			    gtu5 = this.filterGtu(function (latlng) {
				return Math.round(latlng.lat * 100000) / 100000 + ':' + Math.round(latlng.lng * 100000) / 100000;
			});
			gtuData = [gtu3, gtu4, gtu5];

			console.log(TAG + ' precision 1000   gut:' + gtu3.length);
			console.log(TAG + ' precision 10000  gut:' + gtu4.length);
			console.log(TAG + ' precision 100000 gut:' + gtu5.length);
		},
		drawGtu: function drawGtu() {
			var gtus = this.props.dmap.get('Gtu') || [],
			    googleMap = this.getGoogleMap(),
			    self = this;
			(0, _each3.default)(gtus, function (colorGtu) {
				var color = colorGtu.color;
				(0, _each3.default)(colorGtu.points, function (gtu) {
					if (gtu.out) {
						return true;
					}
					if (gtu.cellId == 1) {
						var editMarker = new google.maps.Marker({
							position: {
								lat: gtu.lat,
								lng: gtu.lng
							},
							icon: {
								path: self.getCirclePath(5),
								fillColor: color,
								fillOpacity: 1,
								strokeOpacity: 1,
								strokeWeight: 2,
								strokeColor: '#fff'
							},
							draggable: false,
							map: googleMap
						});
						editMarker.gtuInfoId = gtu.gtuInfoId;
						editMarker.status = 'added';
						self.state.customPoints.push(editMarker);
						editMarker.addListener('click', function () {
							self.onRemoveGtu(editMarker);
						});
					} else {
						new _fastMarker2.default({
							position: {
								lat: gtu.lat,
								lng: gtu.lng
							},
							icon: {
								path: self.getCirclePath(5),
								fillColor: color,
								fillOpacity: 1,
								strokeOpacity: 1,
								strokeWeight: 1,
								strokeColor: '#000'
							},
							draggable: false,
							map: googleMap
						});
					}
				});
			});
		},
		onNewGtu: function onNewGtu(e) {
			var googleMap = this.getGoogleMap(),
			    self = this,
			    newMarker = new google.maps.Marker({
				position: e.latLng,
				icon: {
					path: this.getCirclePath(5),
					fillColor: this.state.activeGtu.get('UserColor'),
					fillOpacity: 1,
					strokeOpacity: 1,
					strokeWeight: 2,
					strokeColor: '#fff'
				},
				draggable: false,
				map: googleMap
			});

			newMarker.GtuId = this.state.activeGtu.get('Id');
			newMarker.date = (0, _moment2.default)().format('YYYY-MM-DD HH:mm:ss');
			newMarker.status = 'new';
			this.state.customPoints.push(newMarker);
			newMarker.addListener('click', function () {
				self.onRemoveGtu(newMarker);
			});
			this.forceUpdate();
		},
		onRemoveGtu: function onRemoveGtu(gtu) {
			var index = indexOf(this.state.customPoints, gtu);
			if (gtu.status === 'new') {
				remove(this.state.customPoints, gtu);
			} else {
				gtu.status = 'deleted';
			}
			gtu.setMap(null);
			this.forceUpdate();
		},
		onUnderGtu: function onUnderGtu() {
			var googleMap = this.getGoogleMap(),
			    customPoints = this.state.customPoints,
			    canUndoPoints = filter(customPoints, function (i) {
				return i.status == 'new' || i.status == 'deleted';
			});
			if (canUndoPoints && canUndoPoints.length > 0) {
				var lastPoint = last(canUndoPoints);
				if (lastPoint.status == 'new') {
					lastPoint.setMap(null);
					remove(customPoints, lastPoint);
				} else {
					lastPoint.setMap(googleMap);
					lastPoint.status = 'added';
				}

				this.setState({
					customPoints: customPoints
				});
			}
		},
		onSaveGtu: function onSaveGtu() {
			var self = this,
			    googleMap = this.getGoogleMap(),
			    addedGtu = filter(this.state.customPoints, function (i) {
				return i.status == 'new';
			}),
			    postData = map(addedGtu, function (item) {
				return {
					GtuId: item.GtuId,
					Date: item.date,
					Location: {
						Latitude: item.getPosition().lat(),
						Longitude: item.getPosition().lng()
					}
				};
			}),
			    putData = map(this.state.customPoints, function (i) {
				return i.status == 'deleted' ? i.gtuInfoId : null;
			});

			_jquery2.default.when([this.props.task.addGtuDots(postData), this.props.task.removeGtuDots(putData)]).done(function () {
				(0, _each3.default)(self.state.customPoints, function (point) {
					if (point && point.status == 'deleted') {
						remove(self.state.customPoints, point);
					} else if (point && point.status == 'new') {
						point.status = 'added';
					}
				});

				self.forceUpdate();
			});
		},
		onSetMaxDisplayDots: function onSetMaxDisplayDots() {
			this.setState({
				maxDisplayCount: this.refs.txtMaxCount.value
			});
			setTimeout(this.drawGtu, 500);
		},
		onReCenter: function onReCenter() {
			var googleMap = this.getGoogleMap();
			googleMap.setCenter(dmapBounds.getCenter());
			this.drawGtu();
		},
		render: function render() {
			var self = this,
			    gtuList = this.props.gtu.models || [],
			    newAddedPoints = filter(this.state.customPoints, function (i) {
				return i.status == 'new' || i.status == 'deleted';
			});
			canUndoSave = isEmpty(newAddedPoints) ? false : true;
			if (!canUndoSave) {
				var undoButton = _react2.default.createElement(
					'button',
					{ className: 'button float-right', disabled: true, onClick: this.onUnderGtu },
					_react2.default.createElement('i', { className: 'fa fa-undo' }),
					'Undo'
				);
				var saveButton = _react2.default.createElement(
					'button',
					{ className: 'button float-right', disabled: true, onClick: this.onSaveGtu },
					_react2.default.createElement('i', { className: 'fa fa-save' }),
					'Save'
				);
			} else {
				var undoButton = _react2.default.createElement(
					'button',
					{ className: 'button float-right', onClick: this.onUnderGtu },
					_react2.default.createElement('i', { className: 'fa fa-undo' }),
					'Undo'
				);
				var saveButton = _react2.default.createElement(
					'button',
					{ className: 'button float-right', onClick: this.onSaveGtu },
					_react2.default.createElement('i', { className: 'fa fa-save' }),
					'Save'
				);
			}

			return _react2.default.createElement(
				'div',
				{ className: 'section row' },
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'section-header' },
						_react2.default.createElement(
							'div',
							{ className: 'row' },
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'h5',
									null,
									'Edit GTU - ',
									this.props.task.get('Name')
								)
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'nav',
									{ 'aria-label': 'You are here:', role: 'navigation' },
									_react2.default.createElement(
										'ul',
										{ className: 'breadcrumbs' },
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'a',
												{ href: '#' },
												'Control Center'
											)
										),
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'a',
												{ href: '#report/' + this.props.task.get('Id') },
												'Report'
											)
										),
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'span',
												{ className: 'show-for-sr' },
												'Current: '
											),
											' Edit GTU'
										)
									)
								)
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 medium-7 large-9 columns' },
					gtuList.map(function (gtu) {
						return _react2.default.createElement(
							'button',
							{ key: gtu.get('Id'), className: self.state.activeGtu == gtu ? " button" : "button", onClick: self.setActiveGtu.bind(null, gtu.get('Id')) },
							_react2.default.createElement('i', { className: self.state.activeGtu == gtu ? "fa fa-map-marker" : "fa fa-stop", style: { color: gtu.get('UserColor') } }),
							gtu.get('ShortUniqueID')
						);
					})
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 medium-5 large-3 columns' },
					saveButton,
					undoButton,
					_react2.default.createElement(
						'button',
						{ className: 'button float-right', onClick: this.onReCenter },
						_react2.default.createElement('i', { className: 'fa fa-refresh' }),
						'ReCenter'
					)
				),
				_react2.default.createElement('div', { className: 'small-12 columns', ref: 'mapArea' })
			);
		}
	});

/***/ },
/* 376 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _isInteger2 = __webpack_require__(524);

	var _isInteger3 = _interopRequireDefault(_isInteger2);

	var _pull2 = __webpack_require__(529);

	var _pull3 = _interopRequireDefault(_pull2);

	var _clone2 = __webpack_require__(518);

	var _clone3 = _interopRequireDefault(_clone2);

	var _difference2 = __webpack_require__(520);

	var _difference3 = _interopRequireDefault(_difference2);

	var _keys2 = __webpack_require__(23);

	var _keys3 = _interopRequireDefault(_keys2);

	var _filter2 = __webpack_require__(58);

	var _filter3 = _interopRequireDefault(_filter2);

	var _indexOf2 = __webpack_require__(36);

	var _indexOf3 = _interopRequireDefault(_indexOf2);

	var _isEmpty2 = __webpack_require__(37);

	var _isEmpty3 = _interopRequireDefault(_isEmpty2);

	var _xor2 = __webpack_require__(538);

	var _xor3 = _interopRequireDefault(_xor2);

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _fastMarker = __webpack_require__(98);

	var _fastMarker2 = _interopRequireDefault(_fastMarker);

	var _user = __webpack_require__(97);

	var _user2 = _interopRequireDefault(_user);

	var _user3 = __webpack_require__(71);

	var _user4 = _interopRequireDefault(_user3);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _mapBase = __webpack_require__(72);

	var _mapBase2 = _interopRequireDefault(_mapBase);

	var _assign = __webpack_require__(151);

	var _assign2 = _interopRequireDefault(_assign);

	var _employee = __webpack_require__(159);

	var _employee2 = _interopRequireDefault(_employee);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	var reloadTimeout = null,
	    backgroundIntervalReload = null,
	    trackAnimationFrame = null,
	    dmapPolygon = null,
	    dmapBounds = null,
	    gtuPoints = [],
	    //{key: 'lat:lng', point: google.map.marker}
	gtuLocation = {},
	    gtuTrack = {},
	    //{key: 'gtu id', {track: google.map.polyline, startPoint: google.map.marker}
	animateIndex = 0,
	    animateSerial = 0,
	    infowindow = null,
	    startPointIcon = 'M1.8-25.1c-0.3-0.3-0.7-0.5-1.2-0.5c-0.4,0-0.8,0.2-1.2,0.5C-1-24.8-1-24.4-1-23.9c0,0.4,0.1,0.8,0.4,1c0.3,0.3,0.4,0.6,0.4,1v21.4c0,0.1,0,0.2,0.1,0.3C0,0,0.1,0,0.2,0H1c0.1,0,0.2,0,0.3-0.1s0.1-0.2,0.1-0.3v-21.4c0-0.4,0.2-0.7,0.4-1s0.4-0.6,0.4-1C2.3-24.4,2.1-24.8,1.8-25.1z M16.8-23.7c-0.2-0.2-0.4-0.2-0.6-0.2c-0.1,0-0.3,0.1-0.7,0.3c-0.4,0.2-0.6,0.4-1,0.5c-0.1,0,0.8,0.1,0.7,0.1c-0.4,0.1-0.8,0.3-1.3,0.5s-1,0.3-1.5,0.3c-0.4,0-0.8-0.1-1.1-0.2c-1.1-0.5-1-0.9-1.8-1.1c-0.8-0.3-1.7-0.4-2.6-0.4c-1.6,0-1.4,0.5-3.4,1.5c-0.5,0.2-0.8,0.4-1,0.5c-0.3,0.2-0.4,0.4-0.4,0.7v7.4c0,0.2,0.1,0.4,0.2,0.6S2.8-13,3-13c0.1,0,0.3,0,0.4-0.1C5.7-14.3,5.7-15,7.3-15c0.6,0,1.2,0.1,1.8,0.3s1.1,0.4,1.5,0.6c0.4,0.2-0.1,0.4,0.4,0.6c0.5,0.2,1.1,0.3,1.6,0.3c1.3,0,1.9-0.5,3.7-1.5c0.2-0.1,0.4-0.2,0.5-0.4c0.1-0.1,0.2-0.3,0.2-0.5v-7.5C17-23.4,16.9-23.5,16.8-23.7z',
	    walkerIcon = 'M-9-0.2c0,0.5,0.3,1,0.8,1.4S-7,2-6.1,2.2c0.9,0.3,1.8,0.4,2.8,0.6C-2.3,2.9-1.2,3-0.1,3S2,2.9,3.1,2.8c1-0.1,2-0.3,2.8-0.6c0.9-0.3,1.5-0.6,2-1s0.8-0.9,0.8-1.4c0-0.4-0.1-0.8-0.4-1.1C8-1.6,7.6-1.9,7.2-2.1c-0.5-0.2-1-0.4-1.5-0.6C5.2-2.8,4.7-3,4.1-3.1c-0.2,0-0.4,0-0.6,0.1C3.3-2.9,3.2-2.7,3.2-2.5s0,0.4,0.1,0.6s0.3,0.3,0.5,0.3c0.5,0.1,0.9,0.2,1.3,0.3c0.4,0.1,0.7,0.2,1,0.3c0.2,0.1,0.4,0.2,0.6,0.3C6.9-0.6,7-0.5,7-0.5c0.1,0.1,0.1,0.1,0.1,0.1c0,0.1-0.1,0.2-0.3,0.3C6.6,0,6.3,0.2,5.9,0.3S5,0.6,4.5,0.7S3.3,0.9,2.5,1C1.7,1.1,0.9,1.1,0,1.1s-1.7,0-2.5-0.1s-1.5-0.2-2-0.3s-1-0.3-1.4-0.4C-6.3,0.2-6.6,0-6.8-0.1s-0.3-0.2-0.3-0.3c0,0,0-0.1,0.1-0.1c0.1-0.1,0.2-0.1,0.3-0.2S-6.3-0.9-6.1-1c0.2-0.1,0.6-0.2,1-0.3c0.4-0.1,0.8-0.2,1.3-0.3c0.2,0,0.4-0.1,0.5-0.3c0.1-0.2,0.2-0.4,0.1-0.6c0-0.2-0.1-0.4-0.3-0.5c-0.2-0.1-0.4-0.2-0.6-0.1C-4.7-3-5.2-2.9-5.7-2.7s-1,0.3-1.5,0.6c-0.5,0.2-0.9,0.5-1.1,0.8S-9-0.6-9-0.2z M-4.2-11.4v4.8C-4.2-6.4-4.1-6.2-4-6c0.2,0.2,0.3,0.2,0.6,0.2h0.8V-1c0,0.2,0.1,0.4,0.2,0.6c0.2,0.2,0.3,0.2,0.6,0.2h3.2c0.2,0,0.4-0.1,0.6-0.2C2.2-0.6,2.2-0.7,2.2-1v-4.8H3c0.2,0,0.4-0.1,0.6-0.2c0.2-0.2,0.2-0.3,0.2-0.6v-4.8c0-0.4-0.2-0.8-0.5-1.1S2.6-13,2.2-13h-4.8c-0.4,0-0.8,0.2-1.1,0.5S-4.2-11.8-4.2-11.4z M-3-16.2c0,0.8,0.3,1.4,0.8,2s1.2,0.8,2,0.8s1.4-0.3,2-0.8s0.8-1.2,0.8-2c0-0.8-0.3-1.4-0.8-2c-0.5-0.6-1.2-0.8-2-0.8s-1.4,0.3-2,0.8S-3-17-3-16.2z',
	    driverIcon = 'M-10.8,1.3c-0.3,0.3-0.7,0.5-1.1,0.5c-0.4,0-0.8-0.2-1.1-0.5s-0.5-0.7-0.5-1.1s0.2-0.8,0.5-1.1s0.7-0.5,1.1-0.5c0.4,0,0.8,0.2,1.1,0.5c0.3,0.3,0.5,0.7,0.5,1.1C-10.3,0.7-10.4,1.1-10.8,1.3z M-15.2-6.6c0-0.1,0-0.2,0.1-0.3l2.5-2.5c0.1,0,0.2-0.1,0.3-0.1h2v3.3h-4.9V-6.6z M0.7,1.3C0.4,1.6-0.1,1.8-0.4,1.8s-0.8-0.2-1.1-0.5s-0.5-0.7-0.5-1.1s0.2-0.8,0.5-1.1c0.3-0.3,0.7-0.5,1.1-0.5s0.8,0.2,1.1,0.5c0.3,0.3,0.5,0.7,0.5,1.1C1.1,0.7,1,1.1,0.7,1.3z M4.2-14.1c-0.2-0.2-0.4-0.3-0.6-0.3h-13c-0.3,0-0.5,0.1-0.6,0.3c-0.1,0.2-0.3,0.3-0.3,0.6v2.4h-2c-0.2,0-0.5,0.1-0.7,0.2c-0.3,0.1-0.5,0.2-0.7,0.4l-2.5,2.5c-0.1,0.1-0.2,0.2-0.3,0.4c-0.1,0.1-0.1,0.2-0.2,0.4c0,0.1-0.1,0.3-0.1,0.5s0,0.3,0,0.4c0,0.1,0,0.3,0,0.5s0,0.4,0,0.4v4.1c-0.2,0-0.4,0.1-0.6,0.2s-0.2,0.4-0.2,0.6c0,0.1,0,0.2,0.1,0.3c0,0.1,0.1,0.2,0.2,0.2s0.2,0.1,0.2,0.1s0.2,0.1,0.3,0.1s0.2,0,0.3,0c0.1,0,0.2,0,0.3,0s0.3,0,0.3,0h0.8c0,0.9,0.3,1.7,1,2.3s1.4,1,2.3,1s1.7-0.3,2.3-1c0.6-0.6,1-1.4,1-2.3h4.9c0,0.9,0.3,1.7,1,2.3s1.4,1,2.3,1s1.7-0.3,2.3-1c0.6-0.6,1-1.4,1-2.3c0,0,0.1,0,0.3,0c0.2,0,0.3,0,0.3,0s0.1,0,0.3,0c0.1,0,0.2,0,0.3-0.1c0.1,0,0.1-0.1,0.2-0.1s0.1-0.1,0.2-0.2c0-0.1,0.1-0.2,0.1-0.3v-13C4.4-13.9,4.3-14,4.2-14.1z',
	    angleIcon = 'M0.9,0C0.9,0,0.9,0,0.9,0l-1.4-1.5c0,0,0,0-0.1,0c0,0,0,0-0.1,0l-0.2,0.2c0,0,0,0,0,0.1s0,0,0,0.1L0.3,0l-1.2,1.2c0,0,0,0,0,0.1c0,0,0,0,0,0.1l0.2,0.2c0,0,0,0,0.1,0c0,0,0,0,0.1,0L0.9,0C0.9,0,0.9,0,0.9,0z';

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default, _mapBase2.default],
		getInitialState: function getInitialState() {
			return {
				disableDefaultUI: false,
				scrollwheel: false,
				disableDoubleClickZoom: true,
				activeGtu: [],
				ShowOutOfBoundary: false,
				displayMode: 'cover', //cover: show gtu dots, track: show gtu path
				customPoints: [],
				maxDisplayCount: 1000,
				canDrawMap: false,
				lastDisplayGtuIndex: null
			};
		},
		onWindowResize: function onWindowResize() {
			var pageLeftHeight = (0, _jquery2.default)(window).height() - (0, _jquery2.default)(this.refs.mapArea).position().top;
			this.setMapHeight(pageLeftHeight);
		},
		componentDidMount: function componentDidMount() {
			this.publish('showLoading');
			/**
	   * position google map to main area
	   */
			(0, _jquery2.default)(window).on('resize.gtu-monitor-view', _jquery2.default.proxy(this.onWindowResize, this));
			this.onWindowResize();

			var self = this,
			    googleMap = this.getGoogleMap();

			this.drawDmapBoundary().done(function () {
				self.drawGtu();
				self.drawLastLocation();
				var allAssignedGtu = (0, _map3.default)(self.props.gtu.where(function (i) {
					return i.get('IsAssign') || i.get('WithData');
				}), function (gtu) {
					return gtu.get('Id');
				});
				self.setState({
					activeGtu: allAssignedGtu,
					canDrawMap: true
				});
				self.publish('hideLoading');
				infowindow = new google.maps.InfoWindow();
				window.clearInterval(backgroundIntervalReload);
				backgroundIntervalReload = window.setInterval(self.reload, 30 * 1000);
			});

			(0, _jquery2.default)("#monitor-more-menu").foundation();
		},
		componentWillUnmount: function componentWillUnmount() {
			var googleMap = this.getGoogleMap();
			try {
				this.clearMap();
				(0, _each3.default)(gtuLocation, function (item) {
					item.setMap(null);
				});
				dmapPolygon.setMap(null);
				google.maps.event.clearInstanceListeners(googleMap);
				(0, _jquery2.default)(document).off('resize.gtu-monitor-view');
			} catch (ex) {}
		},
		shouldComponentUpdate: function shouldComponentUpdate(nextProps, nextState) {
			var oldActiveGtu = nextState.activeGtu,
			    newActiveGtu = this.state.activeGtu,
			    difference = (0, _xor3.default)(oldActiveGtu, newActiveGtu);

			if (nextState.displayMode == 'track') {
				window.clearInterval(backgroundIntervalReload);
			} else {
				window.clearInterval(backgroundIntervalReload);
				backgroundIntervalReload = window.setInterval(this.reload, 30 * 1000);
			}

			if (this.state.ShowOutOfBoundary != nextState.ShowOutOfBoundary) {
				this.reload();
			} else if (!(0, _isEmpty3.default)(difference)) {
				this.reload();
			} else if (this.state.displayMode != nextState.displayMode) {
				this.clearMap();
				this.reload();
			}

			return true;
		},
		getCirclePath: function getCirclePath(size) {
			return 'M-' + size + ',0a' + size + ',' + size + ' 0 1,0 ' + size * 2 + ',0a' + size + ',' + size + ' 0 1,0 -' + size * 2 + ',0';
		},
		drawDmapBoundary: function drawDmapBoundary() {
			var def = _jquery2.default.Deferred(),
			    boundary = this.props.dmap.get('Boundary'),
			    fillColor = this.props.dmap.get('Color'),
			    googleMap = this.getGoogleMap(),
			    timeout = null;

			dmapBounds = new google.maps.LatLngBounds();
			dmapPolygon = new google.maps.Polygon({
				paths: boundary,
				strokeColor: fillColor,
				strokeOpacity: 1,
				strokeWeight: 6,
				fillOpacity: 0,
				map: googleMap
			});

			(0, _each3.default)(boundary, function (i) {
				var point = new google.maps.LatLng(i.lat, i.lng);
				dmapBounds.extend(point);
			});
			google.maps.event.addListenerOnce(googleMap, 'tilesloaded', function () {
				window.clearTimeout(timeout);
				def.resolve();
			});
			googleMap.fitBounds(dmapBounds);
			def.resolve();
			return def;
		},
		drawGtu: function drawGtu() {
			if (this.state.displayMode != 'cover') {
				return;
			}
			console.log('begin draw');
			var self = this,
			    googleMap = this.getGoogleMap(),
			    needFilterOutOfBoundary = !this.state.ShowOutOfBoundary,
			    dots = this.props.dmap.get('Gtu') || [],
			    activeGtu = this.state.activeGtu;
			console.log('already draw gtu', gtuPoints.length);
			(0, _each3.default)(gtuPoints, function (p) {
				p.setMap(null);
			});
			gtuPoints = [];

			(0, _each3.default)(dots, function (gtu) {
				if ((0, _indexOf3.default)(activeGtu, gtu.gtuId) == -1) {
					return true;
				}
				if (needFilterOutOfBoundary) {
					var points = (0, _filter3.default)(gtu.points, {
						out: false
					});
				} else {
					var points = gtu.points;
				}
				var color = gtu.color;
				console.log('draw color: ' + color + ' gtu count:' + points.length);
				(0, _each3.default)(points, function (latlng) {
					if (latlng && latlng.lat && latlng.lng) {
						gtuPoints.push(new _fastMarker2.default({
							position: {
								lat: latlng.lat,
								lng: latlng.lng
							},
							icon: {
								path: self.getCirclePath(5),
								fillColor: color,
								fillOpacity: 0.8,
								strokeWeight: 1,
								strokeOpacity: 0.9,
								strokeColor: '#000'
							},
							draggable: false,
							map: googleMap
						}));
					}
				});
			});
		},
		prepareGTUTrack: function prepareGTUTrack() {
			var def = _jquery2.default.Deferred(),
			    taskId = this.props.task.get('Id'),
			    activeGtu = this.state.activeGtu;
			if (activeGtu && activeGtu.length == 1) {
				var currentGtu = this.props.gtu.get(activeGtu[0]),
				    track = currentGtu.get('track');
				if (!(0, _isEmpty3.default)(track)) {
					def.resolve();
				} else {
					def = currentGtu.getTrack(taskId);
				}
			} else {
				def.resolve();
			}
			return def;
		},
		drawGTUTrack: function drawGTUTrack() {
			if (this.state.displayMode != 'track') {
				window.cancelAnimationFrame(trackAnimationFrame);
				return;
			}
			var googleMap = this.getGoogleMap(),
			    activeGtu = this.state.activeGtu,
			    gtus = this.props.gtu,
			    self = this;
			this.prepareGTUTrack().then(function () {
				self.clearMap();
				if (activeGtu && activeGtu.length == 1) {
					animateIndex = 0;
					var gtu = gtus.get(activeGtu[0]);
					self.changeToBestTrackView(gtu);
					self.animateDrawTrack(gtu);
				}
			});
		},
		changeToBestTrackView: function changeToBestTrackView(gtu) {
			var path = gtu.get('track'),
			    googleMap = this.getGoogleMap(),
			    bounds = new google.maps.LatLngBounds();
			(0, _each3.default)(path, function (point) {
				bounds.extend(new google.maps.LatLng(point.lat, point.lng));
			});
			googleMap.fitBounds(bounds);
		},
		animateDrawTrack: function animateDrawTrack(gtu) {
			if (this.state.displayMode != 'track') {
				return;
			}
			if (animateSerial % 10 == 0) {
				animateSerial = 1;
			} else {
				animateSerial++;
				trackAnimationFrame = window.requestAnimationFrame(_jquery2.default.proxy(function () {
					this.animateDrawTrack(gtu);
				}, this));
				return;
			}
			var path = gtu.get('track');
			if (!path || animateIndex >= path.length) {
				return;
			}
			var gtuId = gtu.get('Id'),
			    googleMap = this.getGoogleMap();
			if (!gtuTrack[gtuId]) {
				gtuTrack[gtuId] = [];
			}
			var trackPoint = new google.maps.Marker({
				position: path[animateIndex++],
				icon: {
					path: this.getCirclePath(6),
					fillColor: gtu.get('UserColor'),
					fillOpacity: 1,
					strokeOpacity: 1,
					strokeColor: '#000'
				},
				draggable: false,
				map: googleMap
			});
			gtuTrack[gtuId].push(trackPoint);
			trackAnimationFrame = window.requestAnimationFrame(_jquery2.default.proxy(function () {
				this.animateDrawTrack(gtu);
			}, this));
		},
		drawLastLocation: function drawLastLocation() {
			var googleMap = this.getGoogleMap(),
			    point,
			    taskIsStopped = this.props.task.get('Status') == 1,
			    gtuList = this.props.gtu.where(function (i) {
				if (taskIsStopped) {
					return i.get('WithData');
				} else {
					return i.get('IsAssign') || i.get('WithData');
				}
			}),
			    willDrawGtu = (0, _map3.default)(gtuList, function (gtu) {
				return gtu.get('Id');
			}),
			    drawedGtu = (0, _keys3.default)(gtuLocation) || [],
			    needDeleteGtu = (0, _difference3.default)(drawedGtu, willDrawGtu);

			(0, _each3.default)(needDeleteGtu, function (id) {
				gtuLocation[id].setMap(null);
				delete gtuLocation[id];
			});

			(0, _each3.default)(gtuList, function (gtu) {

				var gtuId = gtu.get('Id'),
				    location = gtu.get('Location');
				if (!location) {
					delete gtuLocation[gtuId];
					return true;
				}
				if (gtuLocation[gtuId]) {
					gtuLocation[gtuId].setPosition({
						lat: location.lat,
						lng: location.lng
					});
				} else {
					gtuLocation[gtuId] = new google.maps.Marker({
						position: {
							lat: location.lat,
							lng: location.lng
						},
						icon: {
							path: gtu.get('Role') == 'Driver' ? driverIcon : walkerIcon,
							fillColor: gtu.get('UserColor'),
							fillOpacity: 1,
							strokeOpacity: 1,
							strokeWeight: 1,
							strokeColor: '#fff'
						},
						draggable: false,
						map: googleMap
					});
				}
			});
		},
		clearMap: function clearMap() {
			try {
				(0, _each3.default)(gtuPoints, function (item) {
					item && item.setMap && item.setMap(null);
				});
				window.window.cancelAnimationFrame(trackAnimationFrame);
				animateIndex = 0;
				(0, _each3.default)(gtuTrack, function (item) {
					item && (0, _each3.default)(item, function (point) {
						point && point.setMap && point.setMap(null);
					});
				});
			} catch (ex) {}
		},
		reload: function reload() {
			console.log('reload');
			window.clearTimeout(reloadTimeout);
			reloadTimeout = window.setTimeout(_jquery2.default.proxy(this._reload, this), 0.1 * 1000);
		},
		_reload: function _reload() {
			console.log('_reload');
			var dmap = this.props.dmap,
			    gtu = this.props.gtu,
			    taskId = this.props.task.get('Id'),
			    self = this;
			if (this.state.displayMode == 'cover') {
				_jquery2.default.when([dmap.updateGtuAfterTime(null, {
					quite: true
				}).promise, gtu.fetchGtuLocation(taskId, {
					quite: true
				}).promise]).done(function () {
					self.drawGtu();
					self.drawLastLocation();
				});
			} else {
				self.drawLastLocation();
				self.drawGTUTrack();
			}
		},
		onReCenter: function onReCenter() {
			var googleMap = this.getGoogleMap();
			googleMap.setCenter(dmapBounds.getCenter());
		},
		onSelectedGTU: function onSelectedGTU(gtuId) {
			var activeGtu = (0, _clone3.default)(this.state.activeGtu);
			if (this.state.displayMode == 'cover') {
				if ((0, _indexOf3.default)(activeGtu, gtuId) > -1) {
					(0, _pull3.default)(activeGtu, gtuId);
				} else {
					activeGtu.push(gtuId);
				}
			} else {
				activeGtu = [gtuId];
			}
			this.setState({
				activeGtu: activeGtu
			});
		},
		onGotoGTU: function onGotoGTU(gtuId, e) {
			e.preventDefault();
			e.stopPropagation();
			var googleMap = this.getGoogleMap(),
			    gtu = this.props.gtu.get(gtuId),
			    location = gtu.get('Location'),
			    marker = gtuLocation[gtu.get('Id')];
			if (location) {
				googleMap.setCenter(gtu.get('Location'));
				infowindow.setContent(gtu.get('ShortUniqueID'));
				infowindow.open(googleMap, marker);
			} else {
				this.publish('showDialog', 'No Data');
			}
		},
		onAssign: function onAssign() {
			var gtu = this.props.gtu,
			    taskId = this.props.task.get('Id'),
			    user = new _user2.default(),
			    self = this;
			user.fetchForGtu().done(function () {
				self.publish('showDialog', {
					view: _assign2.default,
					params: {
						collection: gtu,
						user: user,
						taskId: taskId
					},
					options: {
						size: 'full'
					}
				});
			});
		},
		onAddEmployee: function onAddEmployee() {
			var user = new _user2.default(),
			    self = this;
			user.fetchCompany().done(function () {
				self.publish('showDialog', _employee2.default, {
					model: new _user4.default(),
					company: user
				});
			});
		},
		onStart: function onStart() {
			var model = this.props.task;
			model.setStart();
		},
		onStop: function onStop() {
			var model = this.props.task;
			model.setStop();
		},
		onPause: function onPause() {
			var model = this.props.task;
			model.setPause();
		},
		onSwitchDisplayMode: function onSwitchDisplayMode() {
			var activeGtu = (0, _clone3.default)(this.state.activeGtu),
			    singleActiveGtu = [];
			if (activeGtu && activeGtu.length > 0) {
				singleActiveGtu = [activeGtu[0]];
			}
			this.setState({
				activeGtu: this.state.displayMode == 'cover' ? singleActiveGtu : activeGtu,
				displayMode: this.state.displayMode == 'cover' ? 'track' : 'cover',
				lastDisplayGtuIndex: null
			});
		},
		onSwitchShowOutOfBoundaryPoints: function onSwitchShowOutOfBoundaryPoints() {
			this.setState({
				ShowOutOfBoundary: !this.state.ShowOutOfBoundary,
				lastViewArea: null
			});
		},
		onCopyShareLink: function onCopyShareLink() {
			var location = window.location,
			    path = location.pathname.substr(1);
			firstPath = location.pathname.substr(0, path.indexOf('/') + 1), task = this.props.task.get('PublicUrl'), address = location.protocol + '//' + window.location.host + firstPath + '/monitor/#' + task;
			this.publish('showDialog', address);
		},
		onOpenMoreMenu: function onOpenMoreMenu(e) {
			e.preventDefault();
			e.stopPropagation();
		},
		onCloseMoreMenu: function onCloseMoreMenu() {
			(0, _jquery2.default)('#monitor-more-menu').foundation('close');
		},
		renderMoreMenu: function renderMoreMenu() {
			if (this.props.task.get('Status') == 1) {
				var assignButton = null;
			} else {
				var assignButton = _react2.default.createElement(
					'li',
					null,
					_react2.default.createElement(
						'a',
						{ href: 'javascript:;', onClick: this.onAssign },
						_react2.default.createElement('i', { className: 'fa fa-users' }),
						'\xA0',
						_react2.default.createElement(
							'span',
							null,
							'Assign GTU'
						)
					)
				);
			}
			//for for Distribution drivers 
			if (!(0, _isInteger3.default)(this.props.task.get('Id'))) {
				return _react2.default.createElement(
					'span',
					{ className: 'float-right' },
					_react2.default.createElement(
						'button',
						{ className: 'button cirle', 'data-toggle': 'monitor-more-menu', onClick: this.onOpenMoreMenu },
						_react2.default.createElement('i', { className: 'fa fa-ellipsis-h' })
					),
					_react2.default.createElement(
						'div',
						{ id: 'monitor-more-menu', className: 'dropdown-pane bottom',
							'data-dropdown': true,
							'data-close-on-click': 'true',
							'data-auto-focus': 'false',
							onClick: this.onCloseMoreMenu },
						_react2.default.createElement(
							'ul',
							{ className: 'vertical menu' },
							_react2.default.createElement(
								'li',
								null,
								_react2.default.createElement(
									'a',
									{ href: 'javascript:;', onClick: this.onSwitchDisplayMode },
									_react2.default.createElement('i', { className: this.state.displayMode == 'cover' ? 'fa fa-map' : 'fa fa-map-o' }),
									'\xA0',
									_react2.default.createElement(
										'span',
										null,
										this.state.displayMode == 'cover' ? 'Track Path' : 'Show Coverage'
									)
								)
							),
							_react2.default.createElement(
								'li',
								null,
								_react2.default.createElement(
									'a',
									{ href: 'javascript:;', onClick: this.onSwitchShowOutOfBoundaryPoints },
									_react2.default.createElement('i', { className: !this.state.ShowOutOfBoundary ? 'fa fa-compress' : 'fa fa-expand' }),
									'\xA0',
									_react2.default.createElement(
										'span',
										null,
										!this.state.ShowOutOfBoundary ? 'Show Out of Bounds' : 'Hide Out of Bounds'
									)
								)
							)
						)
					)
				);
			}
			return _react2.default.createElement(
				'span',
				{ className: 'float-right' },
				_react2.default.createElement(
					'button',
					{ className: 'button cirle', 'data-toggle': 'monitor-more-menu', onClick: this.onOpenMoreMenu },
					_react2.default.createElement('i', { className: 'fa fa-ellipsis-h' })
				),
				_react2.default.createElement(
					'div',
					{ id: 'monitor-more-menu', className: 'dropdown-pane bottom',
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false',
						onClick: this.onCloseMoreMenu },
					_react2.default.createElement(
						'ul',
						{ className: 'vertical menu' },
						assignButton,
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onAddEmployee },
								_react2.default.createElement('i', { className: 'fa fa-user-plus' }),
								'\xA0',
								_react2.default.createElement(
									'span',
									null,
									'New Employee'
								)
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onSwitchDisplayMode },
								_react2.default.createElement('i', { className: this.state.displayMode == 'cover' ? 'fa fa-map' : 'fa fa-map-o' }),
								'\xA0',
								_react2.default.createElement(
									'span',
									null,
									this.state.displayMode == 'cover' ? 'Track Path' : 'Show Coverage'
								)
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onSwitchShowOutOfBoundaryPoints },
								_react2.default.createElement('i', { className: !this.state.ShowOutOfBoundary ? 'fa fa-compress' : 'fa fa-expand' }),
								'\xA0',
								_react2.default.createElement(
									'span',
									null,
									!this.state.ShowOutOfBoundary ? 'Show Out of Bounds' : 'Hide Out of Bounds'
								)
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onCopyShareLink },
								_react2.default.createElement('i', { className: 'fa fa-link' }),
								'\xA0',
								_react2.default.createElement(
									'span',
									null,
									'URL for Distribution drivers'
								)
							)
						)
					)
				)
			);
		},
		renderGtu: function renderGtu(gtu) {
			var typeIcon = null,
			    alertIcon = null,
			    deleteIcon = null,
			    buttonClass = 'button text-left',
			    taskIsStopped = this.props.task.get('Status') == 1,
			    isActive = (0, _indexOf3.default)(this.state.activeGtu, gtu.get('Id')) > -1,
			    gtuIcon = null;

			if (taskIsStopped) {
				//gtuIcon = <i className="fa fa-stop" style={{color: gtu.get('UserColor')}}></i>
				gtuIcon = _react2.default.createElement('i', { className: 'fa fa-stop' });
			} else {
				switch (gtu.get('Role')) {
					case 'Driver':
						//gtuIcon = <i className="fa fa-truck" style={{color: gtu.get('UserColor')}}></i>
						gtuIcon = _react2.default.createElement('i', { className: 'fa fa-truck' });
						break;
					case 'Walker':
						//gtuIcon = <i className="fa fa-street-view" style={{color: gtu.get('UserColor')}}></i>
						gtuIcon = _react2.default.createElement('i', { className: 'fa fa-street-view' });
						break;
					default:
						gtuIcon = null;
						break;
				}
			}

			if (isActive) {
				buttonClass += ' active';
			}
			if (!taskIsStopped && !gtu.get('IsOnline')) {
				buttonClass += ' offline';
			}
			if (!taskIsStopped && gtu.get('IsOnline') && gtu.get('OutOfBoundary')) {
				alertIcon = _react2.default.createElement('i', { className: 'fa fa-bell faa-ring animated alert' });
			}
			if (!taskIsStopped && gtu.get('WithData')) {
				deleteIcon = _react2.default.createElement('i', { className: 'fa fa-warning alert' });
			}
			return _react2.default.createElement(
				'span',
				{ className: 'group', key: gtu.get('Id') },
				_react2.default.createElement(
					'button',
					{ className: buttonClass, style: { 'backgroundColor': isActive ? gtu.get('UserColor') : 'transparent' }, onClick: this.onSelectedGTU.bind(null, gtu.get('Id')) },
					deleteIcon,
					gtuIcon,
					'\xA0\xA0',
					_react2.default.createElement(
						'span',
						null,
						gtu.get('ShortUniqueID')
					),
					'\xA0\xA0',
					alertIcon
				),
				_react2.default.createElement(
					'button',
					{ className: 'button location', onClick: this.onGotoGTU.bind(null, gtu.get('Id')) },
					_react2.default.createElement('i', { className: 'location fa fa-crosshairs', style: { color: 'black' } })
				)
			);
		},
		renderController: function renderController() {
			var task = this.props.task;
			//for for Distribution drivers 
			if (!(0, _isInteger3.default)(this.props.task.get('Id'))) {
				switch (task.get('Status')) {
					case 0:
						//started
						return _react2.default.createElement(
							'h5',
							null,
							'STARTED'
						);
						break;
					case 1:
						//stoped
						return _react2.default.createElement(
							'h5',
							null,
							'STOPPED'
						);
						break;
					case 2:
						//peased
						return _react2.default.createElement(
							'h5',
							null,
							'PEASED'
						);
						break;
					default:
						return null;
						break;
				}
			}
			switch (task.get('Status')) {
				case 0:
					//started
					return _react2.default.createElement(
						'div',
						null,
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onPause },
							_react2.default.createElement('i', { className: 'fa fa-pause' }),
							'Pause'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onStop },
							_react2.default.createElement('i', { className: 'fa fa-stop' }),
							'Stop'
						)
					);
					break;
				case 1:
					//stoped
					return _react2.default.createElement(
						'h5',
						null,
						'STOPPED'
					);
					break;
				case 2:
					//peased
					return _react2.default.createElement(
						'div',
						null,
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onStart },
							_react2.default.createElement('i', { className: 'fa fa-play' }),
							'Start'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onStop },
							_react2.default.createElement('i', { className: 'fa fa-stop' }),
							'Stop'
						)
					);
					break;
				default:
					return _react2.default.createElement(
						'div',
						null,
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onStart },
							_react2.default.createElement('i', { className: 'fa fa-play' }),
							'Start'
						)
					);
					break;
			}
		},
		render: function render() {
			var self = this,
			    taskIsStopped = this.props.task.get('Status') == 1,
			    gtuList = this.props.gtu.where(function (i) {
				if (taskIsStopped) {
					return i.get('WithData');
				} else {
					return i.get('IsAssign') || i.get('WithData');
				}
			});

			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'div',
					{ className: 'section row gtu-monitor' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'section-header' },
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-5 large-3 columns' },
								this.renderController()
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-7 large-9 columns' },
								this.renderMoreMenu(),
								_react2.default.createElement(
									'button',
									{ className: 'button float-right', onClick: this.onReCenter },
									_react2.default.createElement('i', { className: 'fa fa-crosshairs' }),
									_react2.default.createElement(
										'span',
										null,
										'Center'
									)
								),
								_react2.default.createElement(
									'button',
									{ className: 'button float-right', onClick: this.reload },
									_react2.default.createElement('i', { className: 'fa fa-refresh' }),
									_react2.default.createElement(
										'span',
										null,
										'Refresh'
									)
								)
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row gtu' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						gtuList.map(function (gtu) {
							return self.renderGtu(gtu);
						})
					)
				),
				_react2.default.createElement('div', { ref: 'mapArea' })
			);
		}
	});

/***/ },
/* 377 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _has2 = __webpack_require__(207);

	var _has3 = _interopRequireDefault(_has2);

	var _isString2 = __webpack_require__(210);

	var _isString3 = _interopRequireDefault(_isString2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _menu = __webpack_require__(378);

	var _menu2 = _interopRequireDefault(_menu);

	var _user = __webpack_require__(379);

	var _user2 = _interopRequireDefault(_user);

	var _loading = __webpack_require__(29);

	var _loading2 = _interopRequireDefault(_loading);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default, _react2.default.BackboneMixin("user", "change:FullName")],
		getInitialState: function getInitialState() {
			return {
				mainView: null,
				mainParams: null,
				dialogView: null,
				dialogParams: null,
				dialogSize: 'small',
				dialogCustomClass: '',
				loading: false,
				pageTitle: 'TIMM System',
				showMenu: null,
				showSearch: null,
				showUser: null
			};
		},
		componentDidMount: function componentDidMount() {
			var self = this;
			/**
	   * set main view
	   * @param  {React} view
	   * @param  {Backbone.Collection} or Backbone.Model} params
	   * @param  {showMenu: {boolean}
	   */
			this.subscribe('loadView', function (data) {
				self.setState({
					mainView: data.view,
					mainParams: data.params
				});
				var options = data.options;
				self.setState({
					pageTitle: (0, _has3.default)(options, 'pageTitle') ? options.pageTitle : 'TIMM System',
					showMenu: (0, _has3.default)(options, 'showMenu') ? options.showMenu : true,
					showSearch: (0, _has3.default)(options, 'showSearch') ? options.showSearch : true,
					showUser: (0, _has3.default)(options, 'showUser') ? options.showUser : true
				});
			});
			/**
	   * show a dialog
	   * @param  {React} view
	   * @param  {Backbone.Collection} or Backbone.Model} params
	   * @param  {size: {String} size Foundation Reveal Size Value: tiny, small, large, full} options
	   */
			this.subscribe('showDialog', function (data) {
				if (data) {
					self.setState({
						dialogView: data.view,
						dialogParams: data.params
					});
					var options = data.options;
					self.setState({
						dialogSize: (0, _has3.default)(options, 'size') ? options.size : 'small',
						dialogCustomClass: (0, _has3.default)(options, 'customClass') ? options.customClass : ''
					});
				} else {
					self.setState({
						dialogView: null,
						dialogParams: null,
						dialogSize: null,
						dialogCustomClass: null
					});
				}
			});

			/**
	   * loading control
	   */
			var loadingCount = 0,
			    loadingDelay = 500,
			    loadingTimeout = null;
			this.subscribe('showLoading', function () {
				loadingCount++;
				window.clearTimeout(loadingTimeout);
				loadingTimeout = window.setTimeout(function () {
					self.setState({
						loading: true
					});
				}, loadingDelay);
			});
			this.subscribe('hideLoading', function () {
				loadingCount--;
				window.setTimeout(function () {
					if (loadingCount <= 0) {
						window.clearTimeout(loadingTimeout);
						self.setState({
							loading: false
						});
						loadingCount = 0;
					}
				}, 300);
			});
			/**
	   * fix main view size
	   */
			(0, _jquery2.default)(window).on('resize', function () {
				(0, _jquery2.default)(".off-canvas-wrapper-inner").height((0, _jquery2.default)(window).height());
			});
			(0, _jquery2.default)(window).trigger('resize');

			(0, _jquery2.default)(window).on('click', function () {
				self.publish('Global.Window.Click');
			});
		},
		componentDidUpdate: function componentDidUpdate(prevProps, prevState) {
			if (this.state.dialogView && Foundation) {
				(0, _jquery2.default)('.reveal').foundation();
				var dialogSize = this.state.dialogSize;
				// $(document).off('open.zf.reveal.mainView');
				// $(document).one('open.zf.reveal.mainView', function () {
				// 	console.log('open.zf.reveal.mainView');
				// 	$('.reveal-overlay').css({
				// 		display: dialogSize == 'full' ? 'none' : 'block'
				// 	});
				// });
				(0, _jquery2.default)('.reveal').foundation('open');
			} else {
				(0, _jquery2.default)('.reveal').foundation();
				(0, _jquery2.default)('.reveal').foundation('close');
			}
			(0, _jquery2.default)('iframe').height((0, _jquery2.default)(window).height());
		},
		fullTextSearch: function fullTextSearch(e) {
			this.publish('search', e.currentTarget.value);
		},
		menuSwitch: function menuSwitch() {
			this.refs.sideMenu && this.refs.sideMenu.switch();
		},
		/**
	  * build main view
	  */
		getMainView: function getMainView() {
			if (this.state.mainView) {
				if (_react2.default.isValidElement(this.state.mainView)) {
					return this.state.mainView;
				} else {
					var MainView = _react2.default.createFactory(this.state.mainView);
					return MainView(this.state.mainParams);
				}
			}
			return null;
		},
		onCloseDialog: function onCloseDialog() {
			this.publish('showDialog');
		},
		/**
	  * build dialog view
	  */
		getDialogView: function getDialogView() {
			if (this.state.dialogView) {
				if ((0, _isString3.default)(this.state.dialogView)) {
					var content = this.state.dialogView;
					return _react2.default.createElement(
						'div',
						{ className: 'row' },
						_react2.default.createElement(
							'div',
							{ className: 'small-12 columns' },
							_react2.default.createElement(
								'p',
								null,
								'\xA0'
							),
							_react2.default.createElement(
								'h5',
								null,
								content
							),
							_react2.default.createElement(
								'p',
								null,
								'\xA0'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'small-12 columns' },
							_react2.default.createElement(
								'div',
								{ className: 'button-group float-right' },
								_react2.default.createElement(
									'a',
									{ href: 'javascript:;', className: 'button tiny', onClick: this.onCloseDialog },
									'Okay'
								)
							)
						),
						_react2.default.createElement(
							'button',
							{ onClick: this.onCloseDialog, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
							_react2.default.createElement(
								'span',
								{ 'aria-hidden': 'true' },
								'\xD7'
							)
						)
					);
				} else if (_react2.default.isValidElement(this.state.dialogView)) {
					return this.state.dialogView;
				} else {
					var DialogView = _react2.default.createFactory(this.state.dialogView),
					    params = (0, _extend3.default)(this.state.dialogParams, {
						ref: "DialogView"
					});
					return DialogView(params);
				}
			}
			return null;
		},
		render: function render() {
			var model = this.getModel(),
			    mainView = this.getMainView(),
			    dialogView = this.getDialogView();

			if (this.state.showMenu === true) {
				var mainMenuClassName = 'left-menu';
				var menu = _react2.default.createElement(_menu2.default, { ref: 'sideMenu' });
			} else {
				var mainMenuClassName = '';
				var menu = null;
			}
			if (this.state.showSearch === true) {
				var search = _react2.default.createElement(
					'span',
					{ className: 'title-bar-center' },
					_react2.default.createElement(
						'div',
						{ className: 'topSearchBar hide-for-small-only' },
						_react2.default.createElement('input', { type: 'text', placeholder: 'Search', onChange: this.fullTextSearch })
					)
				);
			} else {
				var search = null;
			}
			if (this.state.showUser === true) {
				var user = _react2.default.createElement(_user2.default, { model: this.props.user });
			} else {
				var user = null;
			}
			var loadingDisplay = this.state.loading ? 'block' : 'none';
			return _react2.default.createElement(
				'div',
				null,
				menu,
				_react2.default.createElement(
					'div',
					{ className: "main off-canvas-content " + mainMenuClassName, 'data-off-canvas-content': true },
					_react2.default.createElement(
						'div',
						{ className: 'title-bar' },
						_react2.default.createElement(
							'div',
							{ className: 'title-bar-left' },
							_react2.default.createElement('button', { 'aria-expanded': 'true', className: 'menu-icon', type: 'button', onClick: this.menuSwitch }),
							_react2.default.createElement(
								'span',
								{ className: 'title-text' },
								this.state.pageTitle
							)
						),
						user,
						search
					),
					mainView,
					_react2.default.createElement(
						'div',
						{ id: 'google-map-wrapper' },
						_react2.default.createElement('div', { id: 'google-map', className: 'google-map' })
					)
				),
				_react2.default.createElement(
					'div',
					{ key: 'dialog', className: 'reveal ' + this.state.dialogSize + ' ' + this.state.dialogCustomClass, 'data-reveal': true, 'data-options': 'closeOnClick: false; closeOnEsc: false;' },
					dialogView
				),
				_react2.default.createElement(
					'div',
					{ className: 'overlayer', style: { 'display': loadingDisplay } },
					_react2.default.createElement(_loading2.default, null)
				)
			);
		}
	});

/***/ },
/* 378 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});
	exports.MenuItem = undefined;

	var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _classnames = __webpack_require__(102);

	var _classnames2 = _interopRequireDefault(_classnames);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	var MenuItem = exports.MenuItem = _react2.default.createClass({
		displayName: 'MenuItem',

		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				active: false
			};
		},
		getDefaultProps: function getDefaultProps() {
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		render: function render() {
			return _react2.default.createElement(
				'li',
				null,
				_react2.default.createElement(
					'a',
					{ className: this.state.active ? 'active' : '', href: "#" + this.props.address },
					_react2.default.createElement('i', { className: this.props.icon }),
					_react2.default.createElement(
						'span',
						{ className: 'menu-text' },
						'\xA0',
						this.props.name
					)
				)
			);
		}
	});

	exports.default = _react2.default.createClass({
		displayName: 'menu',

		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				open: false
			};
		},
		getDefaultProps: function getDefaultProps() {
			return {
				menu: [{
					key: 'campaign',
					icon: 'fa fa-trophy',
					address: 'campaign',
					name: 'Campaign'
				}, {
					key: 'distribution',
					icon: 'fa fa-map',
					address: 'distribution',
					name: 'Distribution Maps'
				}, {
					key: 'monitor',
					icon: 'fa fa-truck',
					address: 'monitor',
					name: 'GPS Monitor'
				}, {
					key: 'report',
					icon: 'fa fa-file-pdf-o',
					address: 'report',
					name: 'Reports'
				}]
			};
		},
		closeMenu: function closeMenu() {
			this.setState({
				open: false
			});
		},
		switch: function _switch() {
			this.setState({
				open: !this.state.open
			});
		},
		render: function render() {
			var menuClass = (0, _classnames2.default)({
				'sidebar': true,
				'off-canvas': true,
				'position-left': true,
				'is-open': this.state.open
			});
			return _react2.default.createElement(
				'div',
				{ className: menuClass, 'data-off-canvas': true, 'data-position': 'left' },
				_react2.default.createElement(
					'ul',
					{ className: 'menu vertical', onClick: this.closeMenu },
					this.props.menu.map(function (item) {
						return _react2.default.createElement(MenuItem, _extends({ key: item.key }, item));
					}),
					_react2.default.createElement(
						'li',
						null,
						_react2.default.createElement(
							'a',
							{ href: '#admin' },
							_react2.default.createElement('i', { className: 'fa fa-gear' }),
							_react2.default.createElement(
								'span',
								{ className: 'menu-text' },
								'\xA0 Administration'
							)
						),
						_react2.default.createElement(
							'ul',
							{ className: 'submenu menu vertical' },
							_react2.default.createElement(
								'li',
								null,
								_react2.default.createElement(
									'a',
									{ href: '#frame/Users.aspx' },
									_react2.default.createElement(
										'span',
										{ className: 'menu-text' },
										'User Management'
									)
								)
							),
							_react2.default.createElement(
								'li',
								null,
								_react2.default.createElement(
									'a',
									{ href: '#frame/NonDeliverables.aspx' },
									_react2.default.createElement(
										'span',
										{ className: 'menu-text' },
										'Non-Deliverables'
									)
								)
							),
							_react2.default.createElement(
								'li',
								null,
								_react2.default.createElement(
									'a',
									{ href: '#frame/GtuAdmin.aspx?AssignNameToGTUFlag=true' },
									_react2.default.createElement(
										'span',
										{ className: 'menu-text' },
										'GTU Management'
									)
								)
							),
							_react2.default.createElement(
								'li',
								null,
								_react2.default.createElement(
									'a',
									{ href: '#frame/AvailableGTUList.aspx' },
									_react2.default.createElement(
										'span',
										{ className: 'menu-text' },
										'GTU Available List'
									)
								)
							),
							_react2.default.createElement(
								'li',
								null,
								_react2.default.createElement(
									'a',
									{ href: '#frame/AdminGtuToBag.aspx' },
									_react2.default.createElement(
										'span',
										{ className: 'menu-text' },
										'GTU bag Management '
									)
								)
							),
							_react2.default.createElement(
								'li',
								null,
								_react2.default.createElement(
									'a',
									{ href: '#frame/AdminGtuBagToAuditor.aspx' },
									_react2.default.createElement(
										'span',
										{ className: 'menu-text' },
										'Assign GTU-Bags to Auditors'
									)
								)
							),
							_react2.default.createElement(
								'li',
								null,
								_react2.default.createElement(
									'a',
									{ href: '#frame/AdminDistributorCompany.aspx' },
									_react2.default.createElement(
										'span',
										{ className: 'menu-text' },
										'Distributor Management'
									)
								)
							)
						)
					)
				)
			);
		}
	});

/***/ },
/* 379 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		componentDidMount: function componentDidMount() {
			$("#userMenu").foundation();
		},
		onLogout: function onLogout() {
			this.getModel().logout().always(function () {
				window.location.reload(true);
			});
		},
		render: function render() {
			var model = this.getModel();
			return _react2.default.createElement(
				'div',
				{ className: 'title-bar-right' },
				_react2.default.createElement(
					'a',
					{ href: 'javascript:;', onClick: this.onLogout },
					_react2.default.createElement(
						'span',
						{ className: 'hide-for-small-only' },
						_react2.default.createElement(
							'small',
							null,
							'Welcome,\xA0',
							this.getModel().get('FullName'),
							'\xA0\xA0'
						)
					),
					_react2.default.createElement('i', { className: 'fa fa-sign-out', style: { 'position': 'relative', 'top': '2px' } })
				)
			);
		}
	});

/***/ },
/* 380 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _adminList = __webpack_require__(74);

	var _adminList2 = _interopRequireDefault(_adminList);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		componentWillMount: function componentWillMount() {},
		onUserSelected: function onUserSelected(user) {
			this.setState({
				selectedUser: user
			});
		},
		onDbUserSelected: function onDbUserSelected(user) {
			this.setState({
				selectedUser: user
			});
			this.publish('monitor/dismiss', user);
		},
		onClose: function onClose() {
			this.publish("showDialog");
		},
		onProcess: function onProcess() {
			if (this.state && this.state.selectedUser) {
				this.publish('monitor/dismiss', this.state.selectedUser);
			}
		},
		render: function render() {
			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'h5',
					null,
					'Dismiss back to Distribution Map'
				),
				_react2.default.createElement(
					'span',
					null,
					'Assign to'
				),
				_react2.default.createElement(_adminList2.default, { onSelect: this.onUserSelected, onDbSelect: this.onDbUserSelected, group: 'distribution' }),
				_react2.default.createElement(
					'div',
					{ className: 'float-right' },
					_react2.default.createElement(
						'button',
						{ className: 'success button', onClick: this.onProcess },
						'Okay'
					),
					_react2.default.createElement(
						'a',
						{ href: 'javascript:;', className: 'button', onClick: this.onClose },
						'Cancel'
					)
				)
			);
		}
	});

/***/ },
/* 381 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	__webpack_require__(104);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		componentDidMount: function componentDidMount() {
			var self = this,
			    model = this.getModel();

			$('.fdatepicker').fdatepicker({
				format: 'yyyy-mm-dd'
			}).on('changeDate', function (e) {
				self.getModel().set('Date', e.date);
			});
			$('form').foundation();
		},
		onSave: function onSave(e) {
			var self = this;
			this.getModel().save(null, {
				success: function success(model, response) {
					if (response && response.success) {
						self.onClose();
					} else {
						self.setState({
							error: "opps! something wrong. please contact us!"
						});
					}
				},
				error: function error() {
					self.setState({
						error: "opps! something wrong. please contact us!"
					});
				}
			});
			e.preventDefault();
			e.stopPropagation();
		},
		onClose: function onClose() {
			$('.fdatepicker').off('changeDate').fdatepicker('remove');
			this.publish("showDialog");
		},
		onChange: function onChange(e) {
			this.getModel().set(e.currentTarget.name, e.currentTarget.value);
		},
		render: function render() {
			var model = this.getModel();
			var date = model.get('Date');
			var displayDate = date ? (0, _moment2.default)(date).format("YYYY-MM-DD") : '';
			var tel = model.get('Telephone'),
			    telphone,
			    operator;
			if (tel) {
				var telArray = tel.split('@');
				telphone = telArray[0];
				operator = '@' + telArray[1];
			}
			var showError = this.state && this.state.error ? true : false;
			var errorMessage = showError ? this.state.error : "";
			return _react2.default.createElement(
				'form',
				{ 'data-abide': true, onSubmit: this.onSave },
				_react2.default.createElement(
					'h3',
					null,
					'Edit Task'
				),
				_react2.default.createElement(
					'div',
					{ 'data-abide-error': true, className: 'alert callout', style: { display: showError ? 'block' : 'none' } },
					_react2.default.createElement(
						'p',
						null,
						_react2.default.createElement('i', { className: 'fa fa-exclamation-circle' }),
						'\xA0',
						errorMessage
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Name',
							_react2.default.createElement('input', { type: 'text', value: model.get('Name'), readOnly: true })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns end' },
						_react2.default.createElement(
							'label',
							null,
							'Distribution Date',
							_react2.default.createElement('input', { className: 'fdatepicker', onChange: this.onChange, name: 'Date', type: 'date', readOnly: true, value: displayDate })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Select Auditor',
							_react2.default.createElement('input', { type: 'text', defaultValue: model.get('AuditorName'), readOnly: true })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Email',
							_react2.default.createElement('input', { onChange: this.onChange, name: 'Email', type: 'text', defaultValue: model.get('Email') })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Telephone',
							_react2.default.createElement('input', { onChange: this.onChange, name: 'Telephone', type: 'text', defaultValue: telphone })
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Telecommunications Operator',
							_react2.default.createElement(
								'select',
								{ defaultValue: operator, onChange: this.onChange, name: 'Operator' },
								_react2.default.createElement(
									'option',
									{ value: '@message.alltel.com' },
									'Alltel'
								),
								_react2.default.createElement(
									'option',
									{ value: '@txt.att.net' },
									'AT&T'
								),
								_react2.default.createElement(
									'option',
									{ value: '@messaging.nextel.com' },
									'Nextel'
								),
								_react2.default.createElement(
									'option',
									{ value: '@messaging.sprintpcs.com' },
									'Sprint'
								),
								_react2.default.createElement(
									'option',
									{ value: '@tms.suncom.com' },
									'SunCom'
								),
								_react2.default.createElement(
									'option',
									{ value: '@tmomail.net' },
									'T-mobile'
								),
								_react2.default.createElement(
									'option',
									{ value: '@voicestream.net' },
									'VoiceStream'
								),
								_react2.default.createElement(
									'option',
									{ value: '@vtext.com' },
									'Verizon(text only)'
								)
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'button-group float-right' },
						_react2.default.createElement(
							'button',
							{ type: 'submit', className: 'success button' },
							'Save'
						),
						_react2.default.createElement(
							'a',
							{ href: 'javascript:;', className: 'button', onClick: this.onClose },
							'Cancel'
						)
					)
				),
				_react2.default.createElement(
					'button',
					{ onClick: this.onClose, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					_react2.default.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'\xD7'
					)
				)
			);
		}
	});

/***/ },
/* 382 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _row = __webpack_require__(383);

	var _row2 = _interopRequireDefault(_row);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				orderByFiled: null,
				orderByAsc: false,
				search: null,
				filterField: null,
				filterValues: []
			};
		},
		componentDidMount: function componentDidMount() {
			var self = this;
			this.subscribe('monitor/refresh', function () {
				self.getCollection().fetchForTask();
			});
			this.subscribe('search', function (words) {
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			(0, _jquery2.default)("#monitor-filter-ddl-ClientName, #monitor-filter-ddl-ClientCode, #monitor-filter-ddl-Date, #monitor-filter-ddl-AreaDescription").foundation();
		},
		onOrderBy: function onOrderBy(field, reactObj, reactId, e) {
			e.preventDefault();
			e.stopPropagation();

			if (this.state.orderByFiled == field) {
				this.setState({
					orderByAsc: !this.state.orderByAsc
				});
			} else {
				this.setState({
					orderByFiled: field,
					orderByAsc: true
				});
			}
		},
		onFilter: function onFilter(field, e) {
			e.preventDefault();
			e.stopPropagation();

			var els = (0, _jquery2.default)(":checked", e.currentTarget),
			    values = _.map(els, function (item) {
				return (0, _jquery2.default)(item).val();
			});

			this.setState({
				filterField: values.length > 0 ? field : null,
				filterValues: values.length > 0 ? values : [],
				search: null
			});
			(0, _jquery2.default)('#monitor-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function onClearFilter(field, e) {
			e.preventDefault();
			e.stopPropagation();
			(0, _jquery2.default)('#monitor-filter-ddl-' + field).foundation('close');
			(0, _jquery2.default)(e.currentTarget).closest('form').get(0).reset();

			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
		},
		renderHeader: function renderHeader(field, displayName) {
			var dataSource = this.getCollection(),
			    sortIcon = null,
			    filterIcon = null,
			    filterMenu = null;
			dataSource = dataSource ? dataSource.toArray() : [];

			if (this.state.orderByFiled == field) {
				if (this.state.orderByAsc) {
					sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort-up active' });
				} else {
					sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort-down active' });
				}
			} else {
				sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort' });
			}
			sortIcon = _react2.default.createElement(
				'a',
				{ onClick: this.onOrderBy.bind(null, field) },
				'\xA0',
				sortIcon
			);

			if (this.state.filterField && this.state.filterField == field) {
				filterIcon = _react2.default.createElement(
					'a',
					{ 'data-toggle': "monitor-filter-ddl-" + field },
					'\xA0',
					_react2.default.createElement('i', { className: 'fa fa-filter active' })
				);
			}
			if (dataSource) {
				var fieldValues = _.map(dataSource, function (i) {
					var fieldValue = i.get(field);
					var dateCheck = (0, _moment2.default)(fieldValue, _moment2.default.ISO_8601);
					if (dateCheck.isValid()) {
						return dateCheck.format("MMM DD, YYYY");
					}
					return fieldValue;
				});
				var menuItems = _.uniq(fieldValues).sort();
				filterMenu = _react2.default.createElement(
					'div',
					{ id: "monitor-filter-ddl-" + field,
						className: 'dropdown-pane bottom',
						style: { width: 'auto' },
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false' },
					_react2.default.createElement(
						'form',
						{ onSubmit: this.onFilter.bind(this, field) },
						_react2.default.createElement(
							'ul',
							{ className: 'vertical menu' },
							menuItems.map(function (item, index) {
								return _react2.default.createElement(
									'li',
									{ key: field + index },
									_react2.default.createElement('input', { id: field + index, name: field, type: 'checkbox', value: item }),
									_react2.default.createElement(
										'label',
										{ htmlFor: field + index },
										item
									)
								);
							})
						),
						_react2.default.createElement(
							'button',
							{ className: 'button tiny success', type: 'submit' },
							'Filter'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button tiny warning', type: 'reset', onClick: this.onClearFilter.bind(this, field) },
							'Clear'
						)
					)
				);
			}

			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'a',
					{ 'data-toggle': "monitor-filter-ddl-" + field },
					displayName
				),
				sortIcon,
				filterIcon,
				filterMenu
			);
		},
		getDataSource: function getDataSource() {
			var dataSource = this.getCollection();
			dataSource = dataSource ? dataSource.toArray() : [];
			if (this.state.orderByFiled) {
				dataSource = _.orderBy(dataSource, ['attributes.' + this.state.orderByFiled], [this.state.orderByAsc ? 'asc' : 'desc']);
			}
			if (this.state.search) {
				var keyword = this.state.search.toLowerCase(),
				    campaignValues = null,
				    campaignSearch = null,
				    taskValues = null,
				    taskSearch = null;
				dataSource = _.filter(dataSource, function (i) {
					campaignValues = _.values(i.attributes);
					campaignSearch = _.some(campaignValues, function (i) {
						var dateCheck = (0, _moment2.default)(i, _moment2.default.ISO_8601);
						if (dateCheck.isValid()) {
							return dateCheck.format("MMM DD YYYY MMM DD, YYYY YYYY-MM-DD MM/DD/YYYY YYYY MM MMM DD").toLowerCase().indexOf(keyword) > -1;
						}
						return _.toString(i).toLowerCase().indexOf(keyword) > -1;
					});
					/**
	     * update task visiable logical.
	     * if campaign in search keyward. show all task
	     * otherwise only show task name in search word.
	     * if there is no task need show hide this campaign.
	     */
					taskValues = _.values(i.attributes.Tasks);
					_.forEach(taskValues, function (i) {
						i.visiable = campaignSearch || i.Name.toLowerCase().indexOf(keyword) > -1;
					});
					return campaignSearch || _.some(taskValues, {
						visiable: true
					});
				});
			} else if (this.state.filterField && this.state.filterValues) {
				var filterField = this.state.filterField,
				    filterValues = this.state.filterValues;
				dataSource = _.filter(dataSource, function (i) {
					var fieldValue = i.get(filterField);
					var dateCheck = (0, _moment2.default)(fieldValue, _moment2.default.ISO_8601);
					if (dateCheck.isValid()) {
						return _.indexOf(filterValues, dateCheck.format("MMM DD, YYYY")) > -1;
					}
					return _.indexOf(filterValues, fieldValue) > -1;
				});
			}
			return dataSource;
		},
		render: function render() {
			var list = this.getDataSource();
			return _react2.default.createElement(
				'div',
				{ className: 'section row' },
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'section-header' },
						_react2.default.createElement(
							'div',
							{ className: 'row' },
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'h5',
									null,
									'GPS Monitor'
								)
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'nav',
									{ 'aria-label': 'You are here:', role: 'navigation' },
									_react2.default.createElement(
										'ul',
										{ className: 'breadcrumbs' },
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'a',
												{ href: '#' },
												'Control Center'
											)
										),
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'span',
												{ className: 'show-for-sr' },
												'Current: '
											),
											' GPS Monitor'
										)
									)
								)
							)
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'scroll-list-section-body' },
						_react2.default.createElement(
							'div',
							{ className: 'row scroll-list-header' },
							_react2.default.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('ClientName', 'Client Name')
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-5 columns' },
								this.renderHeader('ClientCode', 'Client Code')
							),
							_react2.default.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('Date', 'Date')
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 medium-3 columns' },
								_react2.default.createElement(
									'span',
									{ className: 'show-for-large' },
									this.renderHeader('AreaDescription', 'Type')
								)
							)
						),
						list.map(function (item) {
							return _react2.default.createElement(_row2.default, { key: item.get('Id'), model: item });
						})
					)
				)
			);
		}
	});

/***/ },
/* 383 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _task = __webpack_require__(53);

	var _task2 = _interopRequireDefault(_task);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _dismiss = __webpack_require__(380);

	var _dismiss2 = _interopRequireDefault(_dismiss);

	var _edit = __webpack_require__(381);

	var _edit2 = _interopRequireDefault(_edit);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		menuKey: 'monitor-menu-ddl-',
		getDefaultProps: function getDefaultProps() {
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		componentDidMount: function componentDidMount() {
			var menuKey = this.menuKey;
			(0, _each3.default)(this.getModel().get('Tasks'), function (task) {
				(0, _jquery2.default)("#" + menuKey + task.Id).foundation();
			});
		},
		onDismiss: function onDismiss(e) {
			e.preventDefault();
			e.stopPropagation();
			var self = this,
			    model = this.getModel();
			this.confirm('Are you sure you want to delete all selected Tasks?').then(function () {
				self.publish('showDialog', _dismiss2.default);
				self.unsubscribe('monitor/dismiss');
				self.subscribe('monitor/dismiss', function (user) {
					model.dismissToDMap(user, {
						success: function success(result) {
							if (result && result.success) {
								self.publish('monitor/refresh');
							} else {
								self.alert("something wrong");
							}
						},
						complete: function complete() {
							self.publish('showDialog');
						}
					});
				});
			});
		},
		onFinished: function onFinished(taskId, reactEvent, reactNumber, evt) {
			evt.preventDefault();
			evt.stopPropagation();

			var self = this,
			    model = new _task2.default({
				Id: taskId
			});
			model.markFinished({
				success: function success(result) {
					if (result && result.success) {
						self.publish('monitor/refresh');
					} else {
						self.alert(result.error);
					}
				}
			});
		},
		onOpenUploadFile: function onOpenUploadFile(taskId) {
			(0, _jquery2.default)("#upload-file-" + taskId).click();
		},
		onImport: function onImport(taskId, e) {
			(0, _jquery2.default)(e.currentTarget).closest('.dropdown-pane').foundation('close');
			e.bubbles = false;

			var uploadFile = e.currentTarget.files[0];
			if (uploadFile.size == 0) {
				alert('please select an not empty file!');
				return;
			}

			var model = new _task2.default({
				Id: taskId
			}),
			    self = this;

			model.importGtu(uploadFile, {
				success: function success(result) {
					(0, _jquery2.default)("#upload-file-" + taskId).val('');
					if (result && result.success) {
						self.publish('monitor/refresh');
					}
					if (result && result.error && result.error.length > 0) {
						self.alert(result.error.join('\r\n'));
					}
				}
			});
		},
		onEdit: function onEdit(taskId, e) {
			e.preventDefault();
			e.stopPropagation();
			(0, _jquery2.default)(e.currentTarget).closest('.dropdown-pane').foundation('close');

			var model = new _task2.default({
				Id: taskId
			}),
			    self = this;

			model.fetch().then(function () {
				self.publish('showDialog', _edit2.default, model);
			});
		},
		onGotoMonitor: function onGotoMonitor(campaignId, taskName, taskId) {
			window.open('./#campaign/' + campaignId + '/' + taskName + '/' + taskId + '/monitor');
		},
		onOpenMoreMenu: function onOpenMoreMenu(e) {
			e.preventDefault();
			e.stopPropagation();
		},
		onCloseMoreMenu: function onCloseMoreMenu(key) {
			(0, _jquery2.default)('#' + this.menuKey + key).foundation('close');
		},
		renderMoreMenu: function renderMoreMenu(key) {
			var id = this.menuKey + key;
			return _react2.default.createElement(
				'span',
				null,
				_react2.default.createElement(
					'button',
					{ className: 'button cirle', 'data-toggle': id, onClick: this.onOpenMoreMenu },
					_react2.default.createElement('i', { className: 'fa fa-ellipsis-h' })
				),
				_react2.default.createElement(
					'div',
					{ id: id, className: 'dropdown-pane bottom',
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false',
						onClick: this.onCloseMoreMenu.bind(null, key) },
					_react2.default.createElement(
						'ul',
						{ className: 'vertical menu' },
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onEdit.bind(null, key) },
								_react2.default.createElement('i', { className: 'fa fa-edit' }),
								_react2.default.createElement(
									'span',
									null,
									'Edit'
								)
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onOpenUploadFile.bind(null, key) },
								_react2.default.createElement('i', { className: 'fa fa-cloud-upload' }),
								_react2.default.createElement(
									'span',
									null,
									'Import'
								)
							)
						),
						_react2.default.createElement('input', { type: 'file', id: 'upload-file-' + key, multiple: true, style: { 'display': 'none' }, onChange: this.onImport.bind(null, key) })
					)
				)
			);
		},
		onCompanyMonitor: function onCompanyMonitor(campaignId) {
			window.open('./#campaign/' + campaignId + '/monitor');
		},
		render: function render() {
			var model = this.getModel(),
			    date = model.get('Date'),
			    displayDate = date ? (0, _moment2.default)(date).format("MMM DD, YYYY") : '',
			    self = this;
			return _react2.default.createElement(
				'div',
				{ className: 'row scroll-list-item' },
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns', onClick: this.onCompanyMonitor.bind(this, model.get('Id')) },
					_react2.default.createElement(
						'div',
						{ className: 'hide-for-small-only medium-2 columns' },
						model.get('ClientName')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 medium-5 columns' },
						model.get('ClientCode')
					),
					_react2.default.createElement(
						'div',
						{ className: 'hide-for-small-only medium-2 columns' },
						displayDate
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-12 medium-3 columns' },
						_react2.default.createElement(
							'span',
							{ className: 'show-for-large' },
							model.get('AreaDescription')
						),
						_react2.default.createElement(
							'div',
							{ className: 'float-right tool-bar' },
							_react2.default.createElement(
								'a',
								{ onClick: this.onDismiss, className: 'button row-button' },
								_react2.default.createElement('i', { className: 'fa fa-reply' }),
								_react2.default.createElement(
									'small',
									null,
									'Dismiss'
								)
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'table',
						{ className: 'hover' },
						_react2.default.createElement(
							'colgroup',
							null,
							_react2.default.createElement('col', null),
							_react2.default.createElement('col', { style: { 'width': "160px" } })
						),
						_react2.default.createElement(
							'tbody',
							null,
							model.get('Tasks').map(function (task) {
								if (task.visiable === false) {
									return null;
								}
								var campaignId = model.get('Id'),
								    taskName = task.Name;
								return _react2.default.createElement(
									'tr',
									{ key: task.Id },
									_react2.default.createElement(
										'td',
										{ onClick: self.onGotoMonitor.bind(null, campaignId, taskName, task.Id) },
										task.Name
									),
									_react2.default.createElement(
										'td',
										null,
										_react2.default.createElement(
											'div',
											{ className: 'float-right tool-bar' },
											_react2.default.createElement(
												'button',
												{ onClick: self.onFinished.bind(null, task.Id), className: 'button' },
												_react2.default.createElement('i', { className: 'fa fa-check' }),
												_react2.default.createElement(
													'small',
													null,
													'Finish'
												)
											),
											self.renderMoreMenu(task.Id)
										)
									)
								);
							})
						)
					)
				)
			);
		}
	});

/***/ },
/* 384 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _indexOf2 = __webpack_require__(36);

	var _indexOf3 = _interopRequireDefault(_indexOf2);

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _omit2 = __webpack_require__(124);

	var _omit3 = _interopRequireDefault(_omit2);

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _eq2 = __webpack_require__(35);

	var _eq3 = _interopRequireDefault(_eq2);

	var _pick2 = __webpack_require__(126);

	var _pick3 = _interopRequireDefault(_pick2);

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _cloneDeep2 = __webpack_require__(119);

	var _cloneDeep3 = _interopRequireDefault(_cloneDeep2);

	var _some2 = __webpack_require__(47);

	var _some3 = _interopRequireDefault(_some2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _options = __webpack_require__(100);

	var _options2 = _interopRequireDefault(_options);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _campaignOptions = __webpack_require__(387);

	var _campaignOptions2 = _interopRequireDefault(_campaignOptions);

	var _cover = __webpack_require__(154);

	var _cover2 = _interopRequireDefault(_cover);

	var _campaign = __webpack_require__(152);

	var _campaign2 = _interopRequireDefault(_campaign);

	var _campaignSummary = __webpack_require__(153);

	var _campaignSummary2 = _interopRequireDefault(_campaignSummary);

	var _submap = __webpack_require__(157);

	var _submap2 = _interopRequireDefault(_submap);

	var _submapDetail = __webpack_require__(158);

	var _submapDetail2 = _interopRequireDefault(_submapDetail);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			var options = new _options2.default({
				suppressCover: false,
				suppressCampaign: false,
				suppressCampaignSummary: false,
				suppressNDAInCampaign: false,
				suppressSubMap: false,
				suppressSubMapCountDetail: false,
				suppressNDAInSubMap: false,
				suppressDMap: false,
				suppressGTU: false,
				suppressNDAInDMap: true,
				showPenetrationColors: true,
				penetrationColors: [20, 40, 60, 80],
				suppressLocations: false,
				suppressRadii: false
			});
			return {
				options: options
			};
		},
		componentDidMount: function componentDidMount() {
			var collecton = this.getCollection(),
			    self = this;
			this.subscribe('print.map.imageloaded', function () {
				(0, _some3.default)(collecton.models, function (page) {
					var currentPage = self.refs[page.get('key')];
					currentPage && currentPage.state && !currentPage.state.imageLoaded && currentPage.state.imageLoading === false && currentPage.loadImage && currentPage.loadImage();
					return currentPage && currentPage.loadImage ? !currentPage.state.imageLoaded : false;
				});
			});
			this.subscribe('print.map.options.changed', this.onApplyOptions);
			this.onOpenOptions({
				needTrigger: true
			});
		},
		onOpenOptions: function onOpenOptions(opts) {
			var options = this.state.options;
			if (!options.get('DMaps')) {
				options.attributes['DMaps'] = this.getCollection().getDMaps();
			}
			var model = (0, _cloneDeep3.default)(options);
			var params = (0, _extend3.default)(opts, {
				model: model
			});
			this.publish('showDialog', _campaignOptions2.default, params, {
				size: 'large'
			});
		},
		onApplyOptions: function onApplyOptions(options) {
			//check need reload images
			var compareProperty = ['suppressNDAInCampaign', 'suppressNDAInSubMap', 'suppressNDAInDMap', 'showPenetrationColors', 'penetrationColors', 'suppressLocations', 'suppressRadii'];
			var oldOptions = (0, _pick3.default)(this.state.options.attributes, compareProperty),
			    newOptions = (0, _pick3.default)(options.attributes, compareProperty);

			if (!(0, _eq3.default)(oldOptions, newOptions)) {
				(0, _each3.default)(this.refs, function (page) {
					page.setState({
						imageLoaded: null,
						imageLoading: false
					});
				});
			}
			this.setState({
				options: options
			});
			this.publish("showDialog");
			this.publish('print.map.imageloaded');
		},
		onPrint: function onPrint() {
			var collecton = this.getCollection(),
			    campaignId = collecton.getCampaignId(),
			    postData = {
				campaignId: campaignId,
				size: 'A4',
				needFooter: 'true',
				options: []
			},
			    self = this;
			(0, _each3.default)(collecton.models, function (page) {
				if (self.refs[page.get('key')]) {
					postData.options.push(self.refs[page.get('key')].getExportParamters());
				}
			});

			collecton.exportPdf(postData).then(function (response) {
				var downloadUrl = '../api/pdf/download/' + campaignId + '/' + response.sourceFile;
				if ((0, _jquery2.default)('#downloadForm').size() == 0) {
					(0, _jquery2.default)('<form id="downloadForm" action="' + downloadUrl + '" method="GET"></form>').appendTo('body').get(0).submit();
				} else {
					(0, _jquery2.default)('#downloadForm').attr('action', downloadUrl).get(0).submit();
				}
			});
		},
		render: function render() {
			var pages = this.getCollection(),
			    printOptions = this.state.options,
			    options = (0, _omit3.default)(printOptions.attributes, ['DMaps']),
			    dmaps = printOptions.get('DMaps') || [],
			    hideDMaps = (0, _map3.default)(dmaps.models, function (item) {
				if (item.get('Selected') === true) {
					return item.get('Id');
				}
				return null;
			});
			return _react2.default.createElement(
				'div',
				{ className: 'section' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						_react2.default.createElement(
							'div',
							{ className: 'button-group print-toolbar' },
							_react2.default.createElement(
								'button',
								{ onClick: this.onOpenOptions },
								_react2.default.createElement('i', { className: 'fa fa-cog' }),
								'Options'
							),
							_react2.default.createElement(
								'button',
								{ onClick: this.onPrint },
								_react2.default.createElement('i', { className: 'fa fa-print' }),
								'Print'
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'page-container A4' },
					pages.map(function (page) {
						var dmapId = page.get('DMapId');
						if (dmapId && (0, _indexOf3.default)(hideDMaps, dmapId) > -1) {
							return null;
						}
						switch (page.get('type')) {
							case 'Cover':
								return options.suppressCover ? null : _react2.default.createElement(_cover2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							case 'Campaign':
								return options.suppressCampaign ? null : _react2.default.createElement(_campaign2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							case 'CampaignSummary':
								return options.suppressCampaign || options.suppressCampaignSummary ? null : _react2.default.createElement(_campaignSummary2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							case 'SubMap':
								return options.suppressSubMap ? null : _react2.default.createElement(_submap2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
								break;
							case 'SubMapDetail':
								return options.suppressSubMap || options.suppressSubMapCountDetail ? null : _react2.default.createElement(_submapDetail2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
								break;
							default:
								return null;
								break;
						}
					})
				)
			);
		}
	});

/***/ },
/* 385 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _indexOf2 = __webpack_require__(36);

	var _indexOf3 = _interopRequireDefault(_indexOf2);

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _omit2 = __webpack_require__(124);

	var _omit3 = _interopRequireDefault(_omit2);

	var _eq2 = __webpack_require__(35);

	var _eq3 = _interopRequireDefault(_eq2);

	var _pick2 = __webpack_require__(126);

	var _pick3 = _interopRequireDefault(_pick2);

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _cloneDeep2 = __webpack_require__(119);

	var _cloneDeep3 = _interopRequireDefault(_cloneDeep2);

	var _some2 = __webpack_require__(47);

	var _some3 = _interopRequireDefault(_some2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _options = __webpack_require__(100);

	var _options2 = _interopRequireDefault(_options);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _distributionOptions = __webpack_require__(390);

	var _distributionOptions2 = _interopRequireDefault(_distributionOptions);

	var _distributionMap = __webpack_require__(389);

	var _distributionMap2 = _interopRequireDefault(_distributionMap);

	var _distributionDetailMap = __webpack_require__(388);

	var _distributionDetailMap2 = _interopRequireDefault(_distributionDetailMap);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			var options = new _options2.default({
				suppressCover: true,
				suppressCampaign: true,
				suppressCampaignSummary: true,
				suppressNDAInCampaign: true,
				suppressSubMap: true,
				suppressSubMapCountDetail: true,
				suppressNDAInSubMap: true,
				suppressDMap: false,
				suppressGTU: true,
				suppressNDAInDMap: true,
				suppressLocations: true,
				suppressRadii: true
			});
			return {
				options: options
			};
		},
		componentDidMount: function componentDidMount() {
			var collecton = this.getCollection(),
			    self = this;
			this.subscribe('print.map.imageloaded', function () {
				(0, _some3.default)(collecton.models, function (page) {
					var currentPage = self.refs[page.get('key')];
					currentPage && currentPage.state && !currentPage.state.imageLoaded && currentPage.state.imageLoading == false && currentPage.loadImage();
					return currentPage && currentPage.loadImage ? !currentPage.state.imageLoaded : false;
				});
			});
			this.subscribe('print.map.options.changed', this.onApplyOptions);
			this.onOpenOptions({
				needTrigger: true
			});
		},
		onOpenOptions: function onOpenOptions(opts) {
			var options = this.state.options;
			if (!options.get('DMaps')) {
				options.attributes['DMaps'] = this.getCollection().getDMaps();
			}
			var model = (0, _cloneDeep3.default)(options);
			var params = (0, _extend3.default)(opts, {
				model: model
			});
			this.publish('showDialog', _distributionOptions2.default, params, {
				size: 'large'
			});
		},
		onApplyOptions: function onApplyOptions(options) {
			//check need reload images
			var compareProperty = ['suppressDMap', 'suppressNDAInDMap'],
			    oldOptions = (0, _pick3.default)(this.state.options.attributes, compareProperty),
			    newOptions = (0, _pick3.default)(options.attributes, compareProperty);
			if (!(0, _eq3.default)(oldOptions, newOptions)) {
				forEach(this.refs, function (page) {
					page.setState({
						imageLoaded: null
					});
				});
			}
			this.setState({
				options: options
			});
			this.publish("showDialog");
			this.publish('print.map.imageloaded');
		},
		onPrint: function onPrint() {
			var collecton = this.getCollection(),
			    campaignId = collecton.getCampaignId(),
			    postData = {
				campaignId: campaignId,
				size: 'Distribute',
				needFooter: 'false',
				options: []
			},
			    self = this;
			forEach(collecton.models, function (page) {
				if (self.refs[page.get('key')]) {
					postData.options.push(self.refs[page.get('key')].getExportParamters());
				}
			});

			collecton.exportPdf(postData).then(function (response) {
				var downloadUrl = '../api/pdf/download/' + campaignId + '/' + response.sourceFile;
				if ((0, _jquery2.default)('#downloadForm').size() == 0) {
					(0, _jquery2.default)('<form id="downloadForm" action="' + downloadUrl + '" method="GET"></form>').appendTo('body').get(0).submit();
				} else {
					(0, _jquery2.default)('#downloadForm').attr('action', downloadUrl).get(0).submit();
				}
			});
		},
		render: function render() {
			var pages = this.getCollection(),
			    printOptions = this.state.options,
			    options = (0, _omit3.default)(printOptions.attributes, ['DMaps']),
			    dmaps = printOptions.get('DMaps') || [],
			    hideDMaps = (0, _map3.default)(dmaps.models, function (item) {
				if (item.get('Selected') === true) {
					return item.get('Id');
				}
				return null;
			});
			return _react2.default.createElement(
				'div',
				{ className: 'section' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						_react2.default.createElement(
							'div',
							{ className: 'button-group print-toolbar' },
							_react2.default.createElement(
								'button',
								{ onClick: this.onOpenOptions },
								_react2.default.createElement('i', { className: 'fa fa-cog' }),
								'Options'
							),
							_react2.default.createElement(
								'button',
								{ onClick: this.onPrint },
								_react2.default.createElement('i', { className: 'fa fa-print' }),
								'Print'
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'page-container distribution-print' },
					pages.map(function (page) {
						var dmapId = page.get('DMapId');
						if (dmapId && (0, _indexOf3.default)(hideDMaps, dmapId) > -1) {
							return null;
						}
						switch (page.get('type')) {
							case 'DistributionDetail':
								return _react2.default.createElement(_distributionDetailMap2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							default:
								return _react2.default.createElement(_distributionMap2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
						}
					})
				)
			);
		}
	});

/***/ },
/* 386 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _indexOf2 = __webpack_require__(36);

	var _indexOf3 = _interopRequireDefault(_indexOf2);

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _omit2 = __webpack_require__(124);

	var _omit3 = _interopRequireDefault(_omit2);

	var _eq2 = __webpack_require__(35);

	var _eq3 = _interopRequireDefault(_eq2);

	var _pick2 = __webpack_require__(126);

	var _pick3 = _interopRequireDefault(_pick2);

	var _extend2 = __webpack_require__(14);

	var _extend3 = _interopRequireDefault(_extend2);

	var _cloneDeep2 = __webpack_require__(119);

	var _cloneDeep3 = _interopRequireDefault(_cloneDeep2);

	var _some2 = __webpack_require__(47);

	var _some3 = _interopRequireDefault(_some2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _options = __webpack_require__(100);

	var _options2 = _interopRequireDefault(_options);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _reportOptions = __webpack_require__(393);

	var _reportOptions2 = _interopRequireDefault(_reportOptions);

	var _cover = __webpack_require__(154);

	var _cover2 = _interopRequireDefault(_cover);

	var _campaign = __webpack_require__(152);

	var _campaign2 = _interopRequireDefault(_campaign);

	var _campaignSummary = __webpack_require__(153);

	var _campaignSummary2 = _interopRequireDefault(_campaignSummary);

	var _submap = __webpack_require__(157);

	var _submap2 = _interopRequireDefault(_submap);

	var _submapDetail = __webpack_require__(158);

	var _submapDetail2 = _interopRequireDefault(_submapDetail);

	var _dmap = __webpack_require__(391);

	var _dmap2 = _interopRequireDefault(_dmap);

	var _dmapDetailMap = __webpack_require__(392);

	var _dmapDetailMap2 = _interopRequireDefault(_dmapDetailMap);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			var options = new _options2.default({
				suppressCover: false,
				suppressCampaign: false,
				suppressCampaignSummary: false,
				suppressNDAInCampaign: false,
				suppressSubMap: false,
				suppressSubMapCountDetail: false,
				suppressNDAInSubMap: false,
				suppressDMap: false,
				suppressGTU: false,
				suppressNDAInDMap: true,
				showPenetrationColors: true,
				penetrationColors: [20, 40, 60, 80],
				suppressLocations: false,
				suppressRadii: false
			});
			return {
				options: options
			};
		},
		componentDidMount: function componentDidMount() {
			var collecton = this.getCollection(),
			    self = this;
			this.subscribe('print.map.imageloaded', function () {
				(0, _some3.default)(collecton.models, function (page) {
					var currentPage = self.refs[page.get('key')];
					currentPage && currentPage.state && !currentPage.state.imageLoaded && currentPage.state.imageLoading === false && currentPage.loadImage && currentPage.loadImage();
					return currentPage && currentPage.loadImage ? !currentPage.state.imageLoaded : false;
				});
			});
			this.subscribe('print.map.options.changed', this.onApplyOptions);
			this.onOpenOptions({
				needTrigger: true
			});
		},
		onOpenOptions: function onOpenOptions(opts) {
			var options = this.state.options;
			if (!options.get('DMaps')) {
				options.attributes['DMaps'] = this.getCollection().getDMaps();
			}
			var model = (0, _cloneDeep3.default)(options);
			var params = (0, _extend3.default)(opts, {
				model: model
			});
			this.publish('showDialog', _reportOptions2.default, params, {
				size: 'large'
			});
		},
		onApplyOptions: function onApplyOptions(options) {
			//check need reload images
			var compareProperty = ['suppressNDAInCampaign', 'suppressNDAInSubMap', 'suppressNDAInDMap', 'showPenetrationColors', 'penetrationColors', 'suppressLocations', 'suppressRadii'],
			    oldOptions = (0, _pick3.default)(this.state.options.attributes, compareProperty),
			    newOptions = (0, _pick3.default)(options.attributes, compareProperty);
			if (!(0, _eq3.default)(oldOptions, newOptions)) {
				forEach(this.refs, function (page) {
					page.setState({
						imageLoaded: null
					});
				});
			}
			this.setState({
				options: options
			});
			this.publish("showDialog");
			this.publish('print.map.imageloaded');
		},
		onPrint: function onPrint() {
			var collecton = this.getCollection(),
			    campaignId = collecton.getCampaignId(),
			    postData = {
				campaignId: campaignId,
				size: 'A4',
				needFooter: 'true',
				options: []
			},
			    self = this;
			forEach(collecton.models, function (page) {
				if (self.refs[page.get('key')]) {
					postData.options.push(self.refs[page.get('key')].getExportParamters());
				}
			});

			collecton.exportPdf(postData).then(function (response) {
				var downloadUrl = '../api/pdf/download/' + campaignId + '/' + response.sourceFile;
				if ((0, _jquery2.default)('#downloadForm').size() == 0) {
					(0, _jquery2.default)('<form id="downloadForm" action="' + downloadUrl + '" method="GET"></form>').appendTo('body').get(0).submit();
				} else {
					(0, _jquery2.default)('#downloadForm').attr('action', downloadUrl).get(0).submit();
				}
			});
		},
		render: function render() {
			var self = this,
			    pages = this.getCollection(),
			    printOptions = this.state.options,
			    options = (0, _omit3.default)(printOptions.attributes, ['DMaps']),
			    dmaps = printOptions.get('DMaps') || [];
			var hideDMaps = (0, _map3.default)(dmaps.models, function (item) {
				if (item.get('Selected') === true) {
					return item.get('Id');
				}
				return null;
			});
			return _react2.default.createElement(
				'div',
				{ className: 'section' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						_react2.default.createElement(
							'div',
							{ className: 'button-group print-toolbar' },
							_react2.default.createElement(
								'button',
								{ onClick: this.onOpenOptions },
								_react2.default.createElement('i', { className: 'fa fa-cog' }),
								'Options'
							),
							_react2.default.createElement(
								'button',
								{ onClick: this.onPrint },
								_react2.default.createElement('i', { className: 'fa fa-print' }),
								'Print'
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'page-container A4' },
					pages.map(function (page) {
						switch (page.get('type')) {
							case 'Cover':
								return options.suppressCover ? null : _react2.default.createElement(_cover2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							case 'Campaign':
								return options.suppressCampaign ? null : _react2.default.createElement(_campaign2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							case 'CampaignSummary':
								return options.suppressCampaign || options.suppressCampaignSummary ? null : _react2.default.createElement(_campaignSummary2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							case 'SubMap':
								return options.suppressSubMap ? null : _react2.default.createElement(_submap2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
								break;
							case 'SubMapDetail':
								return options.suppressSubMap || options.suppressSubMapCountDetail ? null : _react2.default.createElement(_submapDetail2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
								break;
							case 'DMap':
								return options.suppressDMap || (0, _indexOf3.default)(hideDMaps, page.get('DMapId')) > -1 ? null : _react2.default.createElement(_dmap2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
								break;
							case 'DMapDetailMap':
								return options.suppressDMap || (0, _indexOf3.default)(hideDMaps, page.get('DMapId')) > -1 ? null : _react2.default.createElement(_dmapDetailMap2.default, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
								break;
							default:
								return null;
								break;
						}
					})
				)
			);
		}
	});

/***/ },
/* 387 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _options = __webpack_require__(101);

	var _options2 = _interopRequireDefault(_options);

	var _penetrationColor = __webpack_require__(156);

	var _penetrationColor2 = _interopRequireDefault(_penetrationColor);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_options2.default],
		onPenetrationColorsChanged: function onPenetrationColorsChanged(values) {
			var model = this.getModel();
			model.set('penetrationColors', values, {
				silent: true
			});
		},
		render: function render() {
			var model = this.getModel();
			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'h3',
					null,
					'Print Options'
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Target Method:',
							_react2.default.createElement('input', { type: 'text', name: 'targetMethod', defaultValue: model.get('targetMethod'), onChange: this.OnValueChanged })
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'panel callout secondary' },
					_react2.default.createElement(
						'h6',
						null,
						'Campaign Maps'
					),
					_react2.default.createElement(
						'div',
						{ className: 'row medium-up-2 large-up-2 collapse' },
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressCover', name: 'suppressCover', type: 'checkbox', checked: model.get('suppressCover'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressCover' },
								'Suppress Cover'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressCampaign', name: 'suppressCampaign', type: 'checkbox', checked: model.get('suppressCampaign'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressCampaign' },
								'Suppress Campaign Page'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressSubMap', name: 'suppressSubMap', type: 'checkbox', checked: model.get('suppressSubMap'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressSubMap' },
								'Suppress Sub Maps'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressCampaignSummary', name: 'suppressCampaignSummary', type: 'checkbox', checked: model.get('suppressCampaignSummary'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressCampaignSummary' },
								'Suppress Sub Map Summary'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressSubMapCountDetail', name: 'suppressSubMapCountDetail', type: 'checkbox', checked: model.get('suppressSubMapCountDetail'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressSubMapCountDetail' },
								'Suppress Sub Map Croute Counts'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressNDAInCampaign', name: 'suppressNDAInCampaign', type: 'checkbox', checked: model.get('suppressNDAInCampaign'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressNDAInCampaign' },
								'Suppress non-deliverables for campaign map'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressNDAInSubMap', name: 'suppressNDAInSubMap', type: 'checkbox', checked: model.get('suppressNDAInSubMap'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressNDAInSubMap' },
								'Suppress non-deliverables for sub map'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressLocations', name: 'suppressLocations', type: 'checkbox', checked: model.get('suppressLocations'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressLocations' },
								'Suppress Locations'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressRadii', name: 'suppressRadii', type: 'checkbox', checked: model.get('suppressRadii'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressRadii' },
								'Suppress Radii'
							)
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'row small-up-1 collapse' },
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'showPenetrationColors', name: 'showPenetrationColors', type: 'checkbox', checked: model.get('showPenetrationColors'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'showPenetrationColors' },
								'Show Penetration Colors:'
							)
						),
						_react2.default.createElement(_penetrationColor2.default, { colors: model.get('penetrationColors'), onChange: this.onPenetrationColorsChanged })
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'button-group float-right' },
						_react2.default.createElement(
							'button',
							{ className: 'success button', onClick: this.onApply },
							'Apply'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onClose },
							'Cancel'
						)
					)
				),
				_react2.default.createElement(
					'button',
					{ onClick: this.onClose, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					_react2.default.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'\xD7'
					)
				)
			);
		}
	});

/***/ },
/* 388 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _isEmpty2 = __webpack_require__(37);

	var _isEmpty3 = _interopRequireDefault(_isEmpty2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _numeral = __webpack_require__(26);

	var _numeral2 = _interopRequireDefault(_numeral);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _loading = __webpack_require__(29);

	var _loading2 = _interopRequireDefault(_loading);

	var _mapZoom = __webpack_require__(73);

	var _mapZoom2 = _interopRequireDefault(_mapZoom);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				imageLoaded: null,
				imageLoading: false
			};
		},
		preloadImage: function preloadImage(imageAddress) {
			var def = _jquery2.default.Deferred();
			(0, _jquery2.default)(new Image()).one('load', function () {
				def.resolve();
			}).attr('src', imageAddress);
			return def;
		},
		scrollToPage: function scrollToPage() {
			var model = this.getModel(),
			    height = (0, _jquery2.default)(this.refs.page).position().top;
			(0, _jquery2.default)('.off-canvas-wrapper-inner').stop().animate({
				scrollTop: height
			}, 600);
		},
		loadImage: function loadImage() {
			var model = this.getModel(),
			    key = model.get('key'),
			    self = this;

			this.setState({
				imageLoaded: null,
				imageLoading: true
			});
			this.scrollToPage();

			model.fetchMapImage(this.props.options).then(function () {
				var def = _jquery2.default.Deferred(),
				    mapImage = model.get('MapImage'),
				    ploygonImage = model.get('PolygonImage');
				if ((0, _isEmpty3.default)(mapImage) || (0, _isEmpty3.default)(ploygonImage)) {
					def.reject();
				} else {
					_jquery2.default.when(self.preloadImage(mapImage), self.preloadImage(ploygonImage)).done(function () {
						def.resolve();
					});
				}
				return def;
			}).always(function () {
				self.setState({
					imageLoaded: true,
					imageLoading: false
				});
				self.publish('print.map.imageloaded');
			});
		},
		onReloadImage: function onReloadImage() {
			this.setState({
				imageLoaded: null
			});
			this.publish('print.map.imageloaded');
		},
		onRemove: function onRemove() {
			var model = this.getModel(),
			    dmapId = model.get('DMapId'),
			    serial = model.get('Serial'),
			    collection = model.collection;
			collection.remove(model);
			(0, _each3.default)(collection.models, function (item) {
				var currentDmapId = item.get('DMapId'),
				    currentSerial = item.get('Serial');
				currentDmapId == dmapId && currentSerial > serial && item.set('Serial', currentSerial - 1);
			});
		},
		getExportParamters: function getExportParamters() {
			var model = this.getModel();
			return {
				'type': 'dmap',
				'options': [{
					'title': 'DM MAP ' + model.get('DMapId') + '(' + model.get('Name') + ') -- ' + model.get('Serial')
				}, {
					'list': [{
						'key': 'DM MAP #',
						'text': model.get('DMapId') + ''
					}, {
						'key': 'DISTRIBUTION MAP NAME',
						'text': model.get('Name')
					}, {
						'key': 'TOTAL',
						'text': (0, _numeral2.default)(model.get('Total')).format('0,0')
					}]
				}, {
					'title': model.get('DisplayName') + ' - ' + model.get('Name')
				}, {
					'map': model.get('PolygonImage'),
					'bg': model.get('MapImage')
				}]
			};
		},
		render: function render() {
			var model = this.getModel(),
			    total = (0, _numeral2.default)(model.get('Total')).format('0,0'),
			    mapImage = model.get('MapImage'),
			    polygonImage = model.get('PolygonImage');

			if (this.state.imageLoaded) {
				if (mapImage && polygonImage) {
					var style = {
						'backgroundImage': 'url(' + mapImage + ')',
						'backgroundRepeat': 'no-repeat',
						'backgroundSize': '100% auto',
						'backgroundPosition': '0 0'
					};
					var mapImage = _react2.default.createElement(
						'div',
						{ style: style },
						_react2.default.createElement('img', { src: polygonImage })
					);
				} else {
					var mapImage = _react2.default.createElement(
						'button',
						{ className: 'button reload', onClick: this.onReloadImage },
						_react2.default.createElement('i', { className: 'fa fa-2x fa-refresh' })
					);
				}
			} else {
				var mapImage = _react2.default.createElement(_loading2.default, { text: this.state.imageLoading ? 'LOADING' : 'WAITING' });
			}

			return _react2.default.createElement(
				'div',
				{ className: 'page', ref: 'page' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'DM MAP ',
						model.get('DMapId'),
						'(',
						model.get('Name'),
						') -- ',
						model.get('Serial'),
						_react2.default.createElement(
							'button',
							{ className: 'button float-right', onClick: this.onRemove },
							_react2.default.createElement('i', { className: 'fa fa-delete' }),
							'Remove'
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row list', role: 'list' },
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DM MAP #:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('DMapId')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DISTRIBUTION MAP NAME:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('Name')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TOTAL:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						total
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title mapTitle' },
						model.get('DisplayName'),
						' - ',
						model.get('Name')
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'map-container' },
							mapImage
						)
					)
				)
			);
		}
	});

/***/ },
/* 389 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _isEmpty2 = __webpack_require__(37);

	var _isEmpty3 = _interopRequireDefault(_isEmpty2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _numeral = __webpack_require__(26);

	var _numeral2 = _interopRequireDefault(_numeral);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _loading = __webpack_require__(29);

	var _loading2 = _interopRequireDefault(_loading);

	var _mapZoom = __webpack_require__(73);

	var _mapZoom2 = _interopRequireDefault(_mapZoom);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				imageLoaded: null,
				imageLoading: false
			};
		},
		componentDidMount: function componentDidMount() {
			this.publish('print.map.imageloaded');
		},
		scrollToPage: function scrollToPage() {
			var model = this.getModel(),
			    height = (0, _jquery2.default)(this.refs.page).position().top;
			(0, _jquery2.default)('.off-canvas-wrapper-inner').stop().animate({
				scrollTop: height
			}, 600);
		},
		preloadImage: function preloadImage(imageAddress) {
			var def = _jquery2.default.Deferred();
			(0, _jquery2.default)(new Image()).one('load', function () {
				def.resolve();
			}).attr('src', imageAddress);
			return def;
		},
		loadImage: function loadImage() {
			var model = this.getModel(),
			    key = model.get('key'),
			    self = this;
			this.setState({
				imageLoaded: null,
				imageLoading: true
			});
			this.scrollToPage();
			model.fetchMapImage(this.props.options).then(function () {
				var def = _jquery2.default.Deferred(),
				    mapImage = model.get('MapImage'),
				    ploygonImage = model.get('PolygonImage');
				if ((0, _isEmpty3.default)(mapImage) || (0, _isEmpty3.default)(ploygonImage)) {
					def.reject();
				} else {
					_jquery2.default.when(self.preloadImage(mapImage), self.preloadImage(ploygonImage)).done(function () {
						def.resolve();
					});
				}
				return def;
			}).always(function () {
				self.setState({
					imageLoaded: true,
					imageLoading: false
				});
				self.publish('print.map.imageloaded');
			});
		},
		onReloadImage: function onReloadImage(e) {
			e.stopPropagation();
			this.setState({
				imageLoaded: null,
				imageLoading: false
			});
			this.publish('print.map.imageloaded');
		},
		onCreateDetailMap: function onCreateDetailMap(rectBounds) {
			var model = this.getModel(),
			    collection = model.collection,
			    detailModel = model.clone(),
			    dmapId = detailModel.get('DMapId'),
			    topRight = rectBounds.getNorthEast(),
			    bottomLeft = rectBounds.getSouthWest(),
			    modelIndex = 0,
			    serial = 0,
			    key = model.get('key') + '-' + topRight.lat() + '-' + topRight.lng() + '-' + bottomLeft.lat() + '-' + bottomLeft.lng();

			(0, _each3.default)(collection.models, function (item, index) {
				var currentDMapId = item.get('DMapId');
				if (currentDMapId == dmapId) {
					serial++;
					modelIndex = index;
				}
			});
			detailModel.set({
				'key': key,
				'type': 'DistributionDetail',
				'TopRight': {
					lat: topRight.lat(),
					lng: topRight.lng()
				},
				'BottomLeft': {
					lat: bottomLeft.lat(),
					lng: bottomLeft.lng()
				},
				'Boundary': null,
				'ImageStatus': 'waiting',
				'MapImage': null,
				'PolygonImage': null,
				'Serial': serial
			});
			collection.add(detailModel, {
				at: modelIndex + 1
			});
			this.publish('showDialog');
			this.publish('print.map.imageloaded');
		},
		onShowEditDialog: function onShowEditDialog() {
			var model = this.getModel(),
			    self = this,
			    def = _jquery2.default.Deferred();
			if (!this.state.imageLoaded || !model.get('MapImage') || !model.get('PolygonImage')) {
				return;
			}
			this.publish('showLoading');
			if (model.get('Boundary')) {
				def.resolve();
			} else {
				def = model.fetchBoundary({
					quite: true
				});
			}
			def.done(function () {
				var color = model.get('Color'),
				    key = 'distribution-' + model.get('DMapId');
				self.unsubscribe('print.mapzoom@' + key);
				self.subscribe('print.mapzoom@' + key, _jquery2.default.proxy(self.onCreateDetailMap, self));
				self.publish('showDialog', _mapZoom2.default, {
					sourceKey: key,
					boundary: model.get('Boundary'),
					color: 'rgb(' + color.r + ',' + color.g + ',' + color.b + ')'
				}, {
					size: 'full',
					customClass: 'google-map-pop'
				});
			});
		},
		getExportParamters: function getExportParamters() {
			var model = this.getModel();
			return {
				'type': 'dmap',
				'options': [{
					'title': 'DM MAP ' + model.get('DMapId') + '(' + model.get('Name') + ')'
				}, {
					'list': [{
						'key': 'DM MAP #:',
						'text': model.get('DMapId') + ''
					}, {
						'key': 'DISTRIBUTION MAP NAME:',
						'text': model.get('Name')
					}, {
						'key': 'TOTAL:',
						'text': (0, _numeral2.default)(model.get('Total')).format('0,0')
					}]
				}, {
					'title': model.get('DisplayName') + ' - ' + model.get('Name')
				}, {
					'map': model.get('PolygonImage'),
					'bg': model.get('MapImage')
				}]
			};
		},
		render: function render() {
			var model = this.getModel(),
			    total = (0, _numeral2.default)(model.get('Total')).format('0,0'),
			    mapImage = model.get('MapImage'),
			    polygonImage = model.get('PolygonImage');

			if (this.state.imageLoaded) {
				if (mapImage && polygonImage) {
					var style = {
						'backgroundImage': 'url(' + mapImage + ')',
						'backgroundRepeat': 'no-repeat',
						'backgroundSize': '100% auto',
						'backgroundPosition': '0 0'
					};
					var mapImage = _react2.default.createElement(
						'div',
						{ style: style },
						_react2.default.createElement('img', { src: polygonImage })
					);
				} else {
					var mapImage = _react2.default.createElement(
						'button',
						{ className: 'button reload', onClick: this.onReloadImage },
						_react2.default.createElement('i', { className: 'fa fa-2x fa-refresh' })
					);
				}
			} else {
				var mapImage = _react2.default.createElement(_loading2.default, { text: this.state.imageLoading ? 'LOADING' : 'WAITING' });
			}
			return _react2.default.createElement(
				'div',
				{ className: 'page', ref: 'page' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'DM MAP ',
						model.get('DMapId'),
						'(',
						model.get('Name'),
						')'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row list', role: 'list' },
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DM MAP #:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('DMapId')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DISTRIBUTION MAP NAME:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('Name')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TOTAL:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						total
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title mapTitle' },
						model.get('DisplayName'),
						' - ',
						model.get('Name')
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'map-container', onClick: this.onShowEditDialog },
							mapImage
						)
					)
				)
			);
		}
	});

/***/ },
/* 390 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _options = __webpack_require__(101);

	var _options2 = _interopRequireDefault(_options);

	var _optionsDMapSelector = __webpack_require__(155);

	var _optionsDMapSelector2 = _interopRequireDefault(_optionsDMapSelector);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_options2.default],
		render: function render() {
			var model = this.getModel();
			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'h3',
					null,
					'Print Options'
				),
				_react2.default.createElement(
					'div',
					{ className: 'panel callout secondary' },
					_react2.default.createElement(
						'h6',
						null,
						'Distribution Maps'
					),
					_react2.default.createElement(
						'div',
						{ className: 'row medium-up-1 large-up-2 collapse' },
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressDMap', name: 'suppressDMap', type: 'checkbox', checked: model.get('suppressDMap'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressDMap' },
								'Suppress Distribution Maps'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressNDAInDMap', name: 'suppressNDAInDMap', type: 'checkbox', checked: model.get('suppressNDAInDMap'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressNDAInDMap' },
								'Suppress non-deliverables for distribution map'
							)
						)
					)
				),
				_react2.default.createElement(_optionsDMapSelector2.default, { collection: model.get('DMaps') }),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'button-group float-right' },
						_react2.default.createElement(
							'button',
							{ className: 'success button', onClick: this.onApply },
							'Apply'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onClose },
							'Cancel'
						)
					)
				),
				_react2.default.createElement(
					'button',
					{ onClick: this.onClose, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					_react2.default.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'\xD7'
					)
				)
			);
		}
	});

/***/ },
/* 391 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _isEmpty2 = __webpack_require__(37);

	var _isEmpty3 = _interopRequireDefault(_isEmpty2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _numeral = __webpack_require__(26);

	var _numeral2 = _interopRequireDefault(_numeral);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _loading = __webpack_require__(29);

	var _loading2 = _interopRequireDefault(_loading);

	var _footer = __webpack_require__(30);

	var _footer2 = _interopRequireDefault(_footer);

	var _mapZoom = __webpack_require__(73);

	var _mapZoom2 = _interopRequireDefault(_mapZoom);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				imageLoaded: null,
				imageLoading: false
			};
		},
		componentDidMount: function componentDidMount() {
			this.publish('print.map.imageloaded');
		},
		scrollToPage: function scrollToPage() {
			var model = this.getModel(),
			    height = (0, _jquery2.default)(this.refs.page).position().top;
			(0, _jquery2.default)('.off-canvas-wrapper-inner').stop().animate({
				scrollTop: height
			}, 600);
		},
		preloadImage: function preloadImage(imageAddress) {
			var def = _jquery2.default.Deferred();
			(0, _jquery2.default)(new Image()).one('load', function () {
				def.resolve();
			}).attr('src', imageAddress);
			return def;
		},
		loadImage: function loadImage() {
			var model = this.getModel(),
			    key = model.get('key'),
			    self = this;
			this.setState({
				imageLoaded: null,
				imageLoading: true
			});
			this.scrollToPage();
			model.fetchMapImage(this.props.options).then(function () {
				var def = _jquery2.default.Deferred(),
				    mapImage = model.get('MapImage'),
				    ploygonImage = model.get('PolygonImage');
				if ((0, _isEmpty3.default)(mapImage) || (0, _isEmpty3.default)(ploygonImage)) {
					def.reject();
				} else {
					_jquery2.default.when(self.preloadImage(mapImage), self.preloadImage(ploygonImage)).done(function () {
						def.resolve();
					});
				}
				return def;
			}).always(function () {
				self.setState({
					imageLoaded: true,
					imageLoading: false
				});
				self.publish('print.map.imageloaded');
			});
		},
		onReloadImage: function onReloadImage(e) {
			e.stopPropagation();
			this.setState({
				imageLoaded: null,
				imageLoading: false
			});
			this.publish('print.map.imageloaded');
		},
		onShowEditDialog: function onShowEditDialog() {
			var model = this.getModel(),
			    self = this,
			    def = _jquery2.default.Deferred();
			if (!this.state.imageLoaded || !model.get('MapImage') || !model.get('PolygonImage')) {
				return;
			}
			this.publish('showLoading');
			if (model.get('Boundary')) {
				def.resolve();
			} else {
				def = model.fetchBoundary({
					quite: true
				});
			}
			def.done(function () {
				var color = model.get('Color'),
				    key = 'dmap-' + model.get('DMapId');
				self.unsubscribe('print.mapzoom@' + key);
				self.subscribe('print.mapzoom@' + key, _jquery2.default.proxy(self.onCreateDetailMap, self));
				self.publish('showDialog', _mapZoom2.default, {
					sourceKey: key,
					boundary: model.get('Boundary'),
					color: 'rgb(' + color.r + ',' + color.g + ',' + color.b + ')'
				}, {
					size: 'full',
					customClass: 'google-map-pop'
				});
			});
		},
		onCreateDetailMap: function onCreateDetailMap(rectBounds) {
			var model = this.getModel(),
			    collection = model.collection,
			    detailModel = model.clone(),
			    dmapId = detailModel.get('DMapId'),
			    topRight = rectBounds.getNorthEast(),
			    bottomLeft = rectBounds.getSouthWest(),
			    modelIndex = 0,
			    serial = 0,
			    key = model.get('key') + '-' + topRight.lat() + '-' + topRight.lng() + '-' + bottomLeft.lat() + '-' + bottomLeft.lng();

			(0, _each3.default)(collection.models, function (item, index) {
				var currentDMapId = item.get('DMapId');
				if (currentDMapId == dmapId) {
					serial++;
					modelIndex = index;
				}
			});
			detailModel.set({
				'key': key,
				'type': 'DMapDetailMap',
				'TopRight': {
					lat: topRight.lat(),
					lng: topRight.lng()
				},
				'BottomLeft': {
					lat: bottomLeft.lat(),
					lng: bottomLeft.lng()
				},
				'ImageStatus': 'waiting',
				'MapImage': null,
				'PolygonImage': null,
				'Serial': serial
			});
			collection.add(detailModel, {
				at: modelIndex + 1
			});
			this.publish('showDialog');
			this.publish('print.map.imageloaded');
		},
		getExportParamters: function getExportParamters() {
			var model = this.getModel();
			return {
				'type': 'dmap',
				'options': [{
					'title': 'DM MAP ' + model.get('DMapId') + '(' + model.get('Name') + ')'
				}, {
					'list': [{
						'key': 'DM MAP #:',
						'text': model.get('DMapId') + ''
					}, {
						'key': 'DISTRIBUTION MAP NAME:',
						'text': model.get('Name')
					}, {
						'key': 'TOTAL:',
						'text': (0, _numeral2.default)(model.get('Total')).format('0,0')
					}]
				}, {
					'title': model.get('DisplayName') + ' - ' + model.get('Name')
				}, {
					'map': model.get('PolygonImage'),
					'bg': model.get('MapImage'),
					'legend': true
				}]
			};
		},
		render: function render() {
			var model = this.getModel(),
			    displayTotal = (0, _numeral2.default)(model.get('Total')).format('0,0'),
			    mapImage = model.get('MapImage'),
			    polygonImage = model.get('PolygonImage');

			if (this.state.imageLoaded) {
				if (mapImage && polygonImage) {
					var style = {
						'backgroundImage': 'url(' + mapImage + ')',
						'backgroundRepeat': 'no-repeat',
						'backgroundSize': '100% auto',
						'backgroundPosition': '0 0'
					};
					var mapImage = _react2.default.createElement(
						'div',
						{ style: style },
						_react2.default.createElement('img', { src: polygonImage })
					);
				} else {
					var mapImage = _react2.default.createElement(
						'button',
						{ className: 'button reload', onClick: this.onReloadImage },
						_react2.default.createElement('i', { className: 'fa fa-2x fa-refresh' })
					);
				}
			} else {
				var mapImage = _react2.default.createElement(_loading2.default, { text: this.state.imageLoading ? 'LOADING' : 'WAITING' });
			}

			return _react2.default.createElement(
				'div',
				{ className: 'page', ref: 'page' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'DM MAP ',
						model.get('DMapId'),
						'(',
						model.get('Name'),
						')'
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row list', role: 'list' },
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DM MAP #:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('DMapId')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DISTRIBUTION MAP NAME:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('Name')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TOTAL:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						displayTotal
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						model.get('DisplayName'),
						' - ',
						model.get('Name')
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'map-container', onClick: this.onShowEditDialog },
							mapImage
						),
						_react2.default.createElement(
							'div',
							{ className: 'small-12 columns' },
							_react2.default.createElement('div', { className: 'color-legend' }),
							_react2.default.createElement('div', { className: 'direction-legend' })
						)
					)
				),
				_react2.default.createElement(_footer2.default, { model: model.get('Footer') })
			);
		}
	});

/***/ },
/* 392 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _isEmpty2 = __webpack_require__(37);

	var _isEmpty3 = _interopRequireDefault(_isEmpty2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _numeral = __webpack_require__(26);

	var _numeral2 = _interopRequireDefault(_numeral);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _loading = __webpack_require__(29);

	var _loading2 = _interopRequireDefault(_loading);

	var _footer = __webpack_require__(30);

	var _footer2 = _interopRequireDefault(_footer);

	var _mapZoom = __webpack_require__(73);

	var _mapZoom2 = _interopRequireDefault(_mapZoom);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				imageLoaded: null,
				imageLoading: false
			};
		},
		componentDidMount: function componentDidMount() {
			this.publish('print.map.imageloaded');
		},
		scrollToPage: function scrollToPage() {
			var model = this.getModel(),
			    height = (0, _jquery2.default)(this.refs.page).position().top;
			(0, _jquery2.default)('.off-canvas-wrapper-inner').stop().animate({
				scrollTop: height
			}, 600);
		},
		preloadImage: function preloadImage(imageAddress) {
			var def = _jquery2.default.Deferred();
			(0, _jquery2.default)(new Image()).one('load', function () {
				def.resolve();
			}).attr('src', imageAddress);
			return def;
		},
		loadImage: function loadImage() {
			var model = this.getModel(),
			    key = model.get('key'),
			    self = this;
			this.setState({
				imageLoaded: null,
				imageLoading: true
			});
			this.scrollToPage();
			model.fetchMapImage(this.props.options).then(function () {
				var def = _jquery2.default.Deferred(),
				    mapImage = model.get('MapImage'),
				    ploygonImage = model.get('PolygonImage');
				if ((0, _isEmpty3.default)(mapImage) || (0, _isEmpty3.default)(ploygonImage)) {
					def.reject();
				} else {
					_jquery2.default.when(self.preloadImage(mapImage), self.preloadImage(ploygonImage)).done(function () {
						def.resolve();
					});
				}
				return def;
			}).always(function () {
				self.setState({
					imageLoaded: true,
					imageLoading: false
				});
				self.publish('print.map.imageloaded');
			});
		},
		onReloadImage: function onReloadImage() {
			this.setState({
				imageLoaded: null,
				imageLoading: false
			});
			this.publish('print.map.imageloaded');
		},
		onRemove: function onRemove() {
			var model = this.getModel(),
			    dmapId = model.get('DMapId'),
			    serial = model.get('Serial'),
			    collection = model.collection;
			collection.remove(model);
			(0, _each3.default)(collection.models, function (item) {
				var currentDmapId = item.get('DMapId'),
				    currentSerial = item.get('Serial');
				currentDmapId == dmapId && currentSerial > serial && item.set('Serial', currentSerial - 1);
			});
		},
		getExportParamters: function getExportParamters() {
			var model = this.getModel();
			return {
				'type': 'dmap',
				'options': [{
					'title': 'DM MAP ' + model.get('DMapId') + '(' + model.get('Name') + ') -- ' + model.get('Serial')
				}, {
					'list': [{
						'key': 'DM MAP #:',
						'text': model.get('DMapId') + ''
					}, {
						'key': 'DISTRIBUTION MAP NAME:',
						'text': model.get('Name')
					}, {
						'key': 'TOTAL:',
						'text': (0, _numeral2.default)(model.get('Total')).format('0,0')
					}]
				}, {
					'title': model.get('DisplayName') + ' - ' + model.get('Name')
				}, {
					'map': model.get('PolygonImage'),
					'bg': model.get('MapImage'),
					'legend': true
				}]
			};
		},
		render: function render() {
			var model = this.getModel(),
			    displayTotal = (0, _numeral2.default)(model.get('Total')).format('0,0'),
			    mapImage = model.get('MapImage'),
			    polygonImage = model.get('PolygonImage');

			if (this.state.imageLoaded) {
				if (mapImage && polygonImage) {
					var style = {
						'backgroundImage': 'url(' + mapImage + ')',
						'backgroundRepeat': 'no-repeat',
						'backgroundSize': '100% auto',
						'backgroundPosition': '0 0'
					};
					var mapImage = _react2.default.createElement(
						'div',
						{ style: style },
						_react2.default.createElement('img', { src: polygonImage })
					);
				} else {
					var mapImage = _react2.default.createElement(
						'button',
						{ className: 'button reload', onClick: this.onReloadImage },
						_react2.default.createElement('i', { className: 'fa fa-2x fa-refresh' })
					);
				}
			} else {
				var mapImage = _react2.default.createElement(_loading2.default, { text: this.state.imageLoading ? 'LOADING' : 'WAITING' });
			}

			return _react2.default.createElement(
				'div',
				{ className: 'page', ref: 'page' },
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'DM MAP ',
						model.get('DMapId'),
						'(',
						model.get('Name'),
						') -- ',
						model.get('Serial'),
						_react2.default.createElement(
							'button',
							{ className: 'button float-right', onClick: this.onRemove },
							_react2.default.createElement('i', { className: 'fa fa-delete' }),
							'Remove'
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row list', role: 'list' },
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DM MAP #:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('DMapId')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'DISTRIBUTION MAP NAME:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						model.get('Name')
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-4 columns' },
						'TOTAL:'
					),
					_react2.default.createElement(
						'div',
						{ className: 'small-8 columns' },
						'\xA0',
						displayTotal
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						model.get('DisplayName'),
						' - ',
						model.get('Name')
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'div',
							{ className: 'map-container' },
							mapImage
						),
						_react2.default.createElement(
							'div',
							{ className: 'small-12 columns' },
							_react2.default.createElement('div', { className: 'color-legend' }),
							_react2.default.createElement('div', { className: 'direction-legend' })
						)
					)
				),
				_react2.default.createElement(_footer2.default, { model: model.get('Footer') })
			);
		}
	});

/***/ },
/* 393 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _options = __webpack_require__(101);

	var _options2 = _interopRequireDefault(_options);

	var _penetrationColor = __webpack_require__(156);

	var _penetrationColor2 = _interopRequireDefault(_penetrationColor);

	var _optionsDMapSelector = __webpack_require__(155);

	var _optionsDMapSelector2 = _interopRequireDefault(_optionsDMapSelector);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_options2.default],
		onPenetrationColorsChanged: function onPenetrationColorsChanged(values) {
			var model = this.getModel();
			model.set('penetrationColors', values, {
				silent: true
			});
		},
		render: function render() {
			var model = this.getModel();

			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'h3',
					null,
					'Print Options'
				),
				_react2.default.createElement(
					'div',
					{ className: 'row collapse' },
					_react2.default.createElement(
						'div',
						{ className: 'small-12 columns' },
						_react2.default.createElement(
							'label',
							null,
							'Target Method:',
							_react2.default.createElement('input', { type: 'text', name: 'targetMethod', defaultValue: model.get('targetMethod'), onChange: this.OnValueChanged })
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'panel callout secondary' },
					_react2.default.createElement(
						'h6',
						null,
						'Campaign Maps'
					),
					_react2.default.createElement(
						'div',
						{ className: 'row medium-up-2 large-up-2 collapse' },
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressCover', name: 'suppressCover', type: 'checkbox', checked: model.get('suppressCover'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressCover' },
								'Suppress Cover'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressCampaign', name: 'suppressCampaign', type: 'checkbox', checked: model.get('suppressCampaign'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressCampaign' },
								'Suppress Campaign Page'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressSubMap', name: 'suppressSubMap', type: 'checkbox', checked: model.get('suppressSubMap'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressSubMap' },
								'Suppress Sub Maps'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressCampaignSummary', name: 'suppressCampaignSummary', type: 'checkbox', checked: model.get('suppressCampaignSummary'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressCampaignSummary' },
								'Suppress Sub Map Summary'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressSubMapCountDetail', name: 'suppressSubMapCountDetail', type: 'checkbox', checked: model.get('suppressSubMapCountDetail'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressSubMapCountDetail' },
								'Suppress Sub Map Croute Counts'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressNDAInCampaign', name: 'suppressNDAInCampaign', type: 'checkbox', checked: model.get('suppressNDAInCampaign'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressNDAInCampaign' },
								'Suppress non-deliverables for campaign map'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressNDAInSubMap', name: 'suppressNDAInSubMap', type: 'checkbox', checked: model.get('suppressNDAInSubMap'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressNDAInSubMap' },
								'Suppress non-deliverables for sub map'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressLocations', name: 'suppressLocations', type: 'checkbox', checked: model.get('suppressLocations'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressLocations' },
								'Suppress Locations'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressRadii', name: 'suppressRadii', type: 'checkbox', checked: model.get('suppressRadii'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressRadii' },
								'Suppress Radii'
							)
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'row small-up-1 collapse' },
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'showPenetrationColors', name: 'showPenetrationColors', type: 'checkbox', checked: model.get('showPenetrationColors'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'showPenetrationColors' },
								'Show Penetration Colors:'
							)
						),
						_react2.default.createElement(_penetrationColor2.default, { colors: model.get('penetrationColors'), onChange: this.onPenetrationColorsChanged })
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'panel callout secondary' },
					_react2.default.createElement(
						'h6',
						null,
						'GTU Reports'
					),
					_react2.default.createElement(
						'div',
						{ className: 'row medium-up-1 large-up-2 collapse' },
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressGTU', name: 'suppressGTU', type: 'checkbox', checked: model.get('suppressGTU'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressGTU' },
								'Suppress GTU Tracking'
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'panel callout secondary' },
					_react2.default.createElement(
						'h6',
						null,
						'Distribution Maps'
					),
					_react2.default.createElement(
						'div',
						{ className: 'row medium-up-1 large-up-2 collapse' },
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressDMap', name: 'suppressDMap', type: 'checkbox', checked: model.get('suppressDMap'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressDMap' },
								'Suppress Distribution Maps'
							)
						),
						_react2.default.createElement(
							'div',
							{ className: 'column' },
							_react2.default.createElement('input', { id: 'suppressNDAInDMap', name: 'suppressNDAInDMap', type: 'checkbox', checked: model.get('suppressNDAInDMap'), onChange: this.OnValueChanged }),
							_react2.default.createElement(
								'label',
								{ htmlFor: 'suppressNDAInDMap' },
								'Suppress non-deliverables for distribution map'
							)
						)
					)
				),
				_react2.default.createElement(_optionsDMapSelector2.default, { collection: model.get('DMaps') }),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'button-group float-right' },
						_react2.default.createElement(
							'button',
							{ className: 'success button', onClick: this.onApply },
							'Apply'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button', onClick: this.onClose },
							'Cancel'
						)
					)
				),
				_react2.default.createElement(
					'button',
					{ onClick: this.onClose, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					_react2.default.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'\xD7'
					)
				)
			);
		}
	});

/***/ },
/* 394 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _indexOf2 = __webpack_require__(36);

	var _indexOf3 = _interopRequireDefault(_indexOf2);

	var _toString2 = __webpack_require__(63);

	var _toString3 = _interopRequireDefault(_toString2);

	var _some2 = __webpack_require__(47);

	var _some3 = _interopRequireDefault(_some2);

	var _values2 = __webpack_require__(537);

	var _values3 = _interopRequireDefault(_values2);

	var _filter2 = __webpack_require__(58);

	var _filter3 = _interopRequireDefault(_filter2);

	var _uniq2 = __webpack_require__(128);

	var _uniq3 = _interopRequireDefault(_uniq2);

	var _orderBy2 = __webpack_require__(125);

	var _orderBy3 = _interopRequireDefault(_orderBy2);

	var _map2 = __webpack_require__(17);

	var _map3 = _interopRequireDefault(_map2);

	var _each2 = __webpack_require__(16);

	var _each3 = _interopRequireDefault(_each2);

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	var _row = __webpack_require__(395);

	var _row2 = _interopRequireDefault(_row);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		getInitialState: function getInitialState() {
			return {
				orderByFiled: null,
				orderByAsc: false,
				search: null,
				filterField: null,
				filterValues: []
			};
		},
		componentDidMount: function componentDidMount() {
			var self = this;

			this.subscribe('report/refresh', function () {
				self.getCollection().fetchForReport();
			});
			this.subscribe('search', function (words) {
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			(0, _jquery2.default)("#report-filter-ddl-ClientName, #report-filter-ddl-ClientCode, #report-filter-ddl-Date, #report-filter-ddl-AreaDescription").foundation();
		},
		onOrderBy: function onOrderBy(field, reactObj, reactId, e) {
			e.preventDefault();
			e.stopPropagation();
			if (this.state.orderByFiled == field) {
				this.setState({
					orderByAsc: !this.state.orderByAsc
				});
			} else {
				this.setState({
					orderByFiled: field,
					orderByAsc: true
				});
			}
		},
		onFilter: function onFilter(field, e) {
			e.preventDefault();
			e.stopPropagation();
			var els = (0, _jquery2.default)(":checked", e.currentTarget),
			    values = (0, _map3.default)(els, function (item) {
				return (0, _jquery2.default)(item).val();
			});

			this.setState({
				filterField: values.length > 0 ? field : null,
				filterValues: values.length > 0 ? values : [],
				search: null
			});
			(0, _jquery2.default)('#report-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function onClearFilter(field, e) {
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			(0, _jquery2.default)('#report-filter-ddl-' + field).foundation('close');
			(0, _jquery2.default)(e.currentTarget).closest('form').get(0).reset();
		},
		renderHeader: function renderHeader(field, displayName) {
			var dataSource = this.getCollection(),
			    sortIcon = null,
			    filterIcon = null,
			    filterMenu = null;
			dataSource = dataSource ? dataSource.toArray() : [];

			if (this.state.orderByFiled == field) {
				if (this.state.orderByAsc) {
					sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort-up active' });
				} else {
					sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort-down active' });
				}
			} else {
				sortIcon = _react2.default.createElement('i', { className: 'fa fa-sort' });
			}
			sortIcon = _react2.default.createElement(
				'a',
				{ onClick: this.onOrderBy.bind(null, field) },
				'\xA0',
				sortIcon
			);

			if (this.state.filterField && this.state.filterField == field) {
				filterIcon = _react2.default.createElement(
					'a',
					{ 'data-toggle': "report-filter-ddl-" + field },
					'\xA0',
					_react2.default.createElement('i', { className: 'fa fa-filter active' })
				);
			}
			if (dataSource) {
				var fieldValues = (0, _map3.default)(dataSource, function (i) {
					var fieldValue = i.get(field);
					var dateCheck = (0, _moment2.default)(fieldValue, _moment2.default.ISO_8601);
					if (dateCheck.isValid()) {
						return dateCheck.format("MMM DD, YYYY");
					}
					return fieldValue;
				});
				var menuItems = (0, _uniq3.default)(fieldValues).sort();
				filterMenu = _react2.default.createElement(
					'div',
					{ id: "report-filter-ddl-" + field,
						className: 'dropdown-pane bottom',
						style: { width: 'auto' },
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false' },
					_react2.default.createElement(
						'form',
						{ onSubmit: this.onFilter.bind(this, field) },
						_react2.default.createElement(
							'ul',
							{ className: 'vertical menu' },
							menuItems.map(function (item, index) {
								return _react2.default.createElement(
									'li',
									{ key: field + index },
									_react2.default.createElement('input', { id: field + index, name: field, type: 'checkbox', value: item }),
									_react2.default.createElement(
										'label',
										{ htmlFor: field + index },
										item
									)
								);
							})
						),
						_react2.default.createElement(
							'button',
							{ className: 'button tiny success', type: 'submit' },
							'Filter'
						),
						_react2.default.createElement(
							'button',
							{ className: 'button tiny warning', type: 'reset', onClick: this.onClearFilter.bind(this, field) },
							'Clear'
						)
					)
				);
			}

			return _react2.default.createElement(
				'div',
				null,
				_react2.default.createElement(
					'a',
					{ 'data-toggle': "report-filter-ddl-" + field },
					displayName
				),
				sortIcon,
				filterIcon,
				filterMenu
			);
		},
		getDataSource: function getDataSource() {
			var dataSource = this.getCollection();
			dataSource = dataSource ? dataSource.toArray() : [];
			if (this.state.orderByFiled) {
				dataSource = (0, _orderBy3.default)(dataSource, ['attributes.' + this.state.orderByFiled], [this.state.orderByAsc ? 'asc' : 'desc']);
			}
			if (this.state.search) {
				var keyword = this.state.search.toLowerCase(),
				    campaignValues = null,
				    campaignSearch = null,
				    taskValues = null,
				    taskSearch = null;
				dataSource = (0, _filter3.default)(dataSource, function (i) {
					campaignValues = (0, _values3.default)(i.attributes);
					campaignSearch = (0, _some3.default)(campaignValues, function (i) {
						var dateCheck = (0, _moment2.default)(i, _moment2.default.ISO_8601);
						if (dateCheck.isValid()) {
							return dateCheck.format("MMM DD YYYY MMM DD, YYYY YYYY-MM-DD MM/DD/YYYY YYYY MM MMM DD").toLowerCase().indexOf(keyword) > -1;
						}
						return (0, _toString3.default)(i).toLowerCase().indexOf(keyword) > -1;
					});
					/**
	     * update task visiable logical.
	     * if campaign in search keyward. show all task
	     * otherwise only show task name in search word.
	     * if there is no task need show hide this campaign.
	     */
					taskValues = (0, _values3.default)(i.attributes.Tasks);
					(0, _each3.default)(taskValues, function (i) {
						i.visiable = campaignSearch || i.Name.toLowerCase().indexOf(keyword) > -1;
					});
					return campaignSearch || (0, _some3.default)(taskValues, {
						visiable: true
					});
				});
			} else if (this.state.filterField && this.state.filterValues) {
				var filterField = this.state.filterField,
				    filterValues = this.state.filterValues;
				dataSource = (0, _filter3.default)(dataSource, function (i) {
					var fieldValue = i.get(filterField);
					var dateCheck = (0, _moment2.default)(fieldValue, _moment2.default.ISO_8601);
					if (dateCheck.isValid()) {
						return (0, _indexOf3.default)(filterValues, dateCheck.format("MMM DD, YYYY")) > -1;
					}
					return (0, _indexOf3.default)(filterValues, fieldValue) > -1;
				});
			}
			return dataSource;
		},
		render: function render() {
			var list = this.getDataSource(),
			    scrollToTaskId = this.props.taskId;
			return _react2.default.createElement(
				'div',
				{ className: 'section row' },
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'div',
						{ className: 'section-header' },
						_react2.default.createElement(
							'div',
							{ className: 'row' },
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'h5',
									null,
									'Reports'
								)
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-12 column' },
								_react2.default.createElement(
									'nav',
									{ 'aria-label': 'You are here:', role: 'navigation' },
									_react2.default.createElement(
										'ul',
										{ className: 'breadcrumbs' },
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'a',
												{ href: '#' },
												'Control Center'
											)
										),
										_react2.default.createElement(
											'li',
											null,
											_react2.default.createElement(
												'span',
												{ className: 'show-for-sr' },
												'Current: '
											),
											' Reports'
										)
									)
								)
							)
						)
					),
					_react2.default.createElement(
						'div',
						{ className: 'scroll-list-section-body' },
						_react2.default.createElement(
							'div',
							{ className: 'row scroll-list-header' },
							_react2.default.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('ClientName', 'Client Name')
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-10 medium-5 columns' },
								this.renderHeader('ClientCode', 'Client Code')
							),
							_react2.default.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('Date', 'Date')
							),
							_react2.default.createElement(
								'div',
								{ className: 'small-2 medium-3 columns' },
								_react2.default.createElement(
									'span',
									{ className: 'show-for-large' },
									this.renderHeader('AreaDescription', 'Type')
								)
							)
						),
						list.map(function (item) {
							return _react2.default.createElement(_row2.default, { key: item.get('Id'), model: item, scrollToTaskId: scrollToTaskId });
						})
					)
				)
			);
		}
	});

/***/ },
/* 395 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';

	Object.defineProperty(exports, "__esModule", {
		value: true
	});

	var _backbone = __webpack_require__(2);

	var _backbone2 = _interopRequireDefault(_backbone);

	var _react = __webpack_require__(3);

	var _react2 = _interopRequireDefault(_react);

	__webpack_require__(6);

	var _moment = __webpack_require__(1);

	var _moment2 = _interopRequireDefault(_moment);

	var _jquery = __webpack_require__(4);

	var _jquery2 = _interopRequireDefault(_jquery);

	var _task = __webpack_require__(53);

	var _task2 = _interopRequireDefault(_task);

	var _base = __webpack_require__(5);

	var _base2 = _interopRequireDefault(_base);

	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

	exports.default = _react2.default.createBackboneClass({
		mixins: [_base2.default],
		menuKey: 'report-menu-ddl-',
		getDefaultProps: function getDefaultProps() {
			return {
				address: null,
				icon: null,
				name: null,
				scrollToTaskId: null
			};
		},
		componentDidMount: function componentDidMount() {
			(0, _jquery2.default)('.has-tip').foundation();
			var scrollToTaskId = this.props.scrollToTaskId;
			if (scrollToTaskId && this.refs[scrollToTaskId]) {
				this.scrollTop(this.refs[scrollToTaskId]);
				this.props.scrollToTaskId = null;
			}
		},
		onReOpenTask: function onReOpenTask(taskId) {
			var self = this;
			this.confirm('Do you really want to move report back to GPS Montor?').then(function () {
				var model = new _task2.default({
					Id: taskId
				});
				model.reOpen({
					success: function success(result) {
						if (result && result.success) {
							self.publish('report/refresh');
						} else {
							self.alert(result.error);
						}
					}
				});
			});
		},
		onGotoReport: function onGotoReport(taskId) {
			window.location.hash = 'frame/ReportsTask.aspx?tid=' + taskId;
		},
		onGotoReview: function onGotoReview(campaignId, taskName, taskId) {
			window.open('index.html#campaign/' + campaignId + '/' + taskName + '/' + taskId + '/edit');
		},
		onCloseMoreMenu: function onCloseMoreMenu(key) {
			(0, _jquery2.default)("#" + this.menuKey + key).foundation('close');
		},
		renderMoreMenu: function renderMoreMenu(key) {
			var id = this.menuKey + key;
			return _react2.default.createElement(
				'span',
				null,
				_react2.default.createElement(
					'button',
					{ className: 'button cirle', 'data-toggle': id },
					_react2.default.createElement('i', { className: 'fa fa-ellipsis-h' })
				),
				_react2.default.createElement(
					'div',
					{ id: id, className: 'dropdown-pane bottom',
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false',
						onClick: this.onCloseMoreMenu.bind(null, key) },
					_react2.default.createElement(
						'ul',
						{ className: 'vertical menu' },
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onEdit },
								_react2.default.createElement('i', { className: 'fa fa-edit' }),
								_react2.default.createElement(
									'span',
									null,
									'Edit'
								)
							)
						),
						_react2.default.createElement(
							'li',
							null,
							_react2.default.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onOpenUploadFile.bind(null, key) },
								_react2.default.createElement('i', { className: 'fa fa-cloud-upload' }),
								_react2.default.createElement(
									'span',
									null,
									'Import'
								)
							)
						),
						_react2.default.createElement('input', { type: 'file', id: 'upload-file-' + key, multiple: true, style: { 'display': 'none' }, onChange: this.onImport.bind(null, key) })
					)
				)
			);
		},
		render: function render() {
			var self = this,
			    model = this.getModel(),
			    date = model.get('Date'),
			    displayDate = date ? (0, _moment2.default)(date).format("MMM DD, YYYY") : '';

			return _react2.default.createElement(
				'div',
				{ className: 'row scroll-list-item' },
				_react2.default.createElement(
					'div',
					{ className: 'hide-for-small-only medium-2 columns' },
					model.get('ClientName')
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-10 medium-5 columns' },
					model.get('ClientCode')
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-2 medium-2 columns' },
					displayDate
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-2 medium-3 columns' },
					_react2.default.createElement(
						'span',
						{ className: 'show-for-large' },
						model.get('AreaDescription')
					),
					_react2.default.createElement(
						'div',
						{ className: 'float-right tool-bar' },
						_react2.default.createElement(
							'a',
							{ className: 'button row-button', href: "#frame/handler/PhantomjsPrintHandler.ashx?campaignId=" + model.get('Id') + "&type=print" },
							_react2.default.createElement('i', { className: 'fa fa-print' }),
							_react2.default.createElement(
								'small',
								null,
								'Print'
							)
						)
					)
				),
				_react2.default.createElement(
					'div',
					{ className: 'small-12 columns' },
					_react2.default.createElement(
						'table',
						{ className: 'hover' },
						_react2.default.createElement(
							'colgroup',
							null,
							_react2.default.createElement('col', null),
							_react2.default.createElement('col', { style: { 'width': "160px" } })
						),
						_react2.default.createElement(
							'tbody',
							null,
							model.get('Tasks').map(function (task) {
								if (task.visiable === false) {
									return null;
								}
								var campaignId = model.get('Id'),
								    taskName = task.Name;
								return _react2.default.createElement(
									'tr',
									{ key: task.Id, ref: task.Id },
									_react2.default.createElement(
										'td',
										{ onClick: self.onGotoReview.bind(null, campaignId, taskName, task.Id) },
										task.Name
									),
									_react2.default.createElement(
										'td',
										null,
										_react2.default.createElement(
											'div',
											{ className: 'float-right tool-bar' },
											_react2.default.createElement(
												'a',
												{ className: 'button row-button', onClick: self.onGotoReport.bind(null, task.Id) },
												_react2.default.createElement('i', { className: 'fa fa-file-text-o' }),
												_react2.default.createElement(
													'small',
													null,
													'Report'
												)
											),
											_react2.default.createElement(
												'button',
												{ onClick: self.onReOpenTask.bind(null, task.Id),
													className: 'button has-tip top', title: 'dismiss',
													'data-tooltip': true, 'aria-haspopup': 'true',
													'data-disable-hover': 'false', tabIndex: '1' },
												_react2.default.createElement('i', { className: 'fa fa-reply' })
											)
										)
									)
								);
							})
						)
					)
				)
			);
		}
	});

/***/ },
/* 396 */,
/* 397 */,
/* 398 */,
/* 399 */,
/* 400 */,
/* 401 */,
/* 402 */,
/* 403 */,
/* 404 */,
/* 405 */,
/* 406 */,
/* 407 */,
/* 408 */,
/* 409 */,
/* 410 */
/***/ function(module, exports, __webpack_require__) {

	var getNative = __webpack_require__(34),
	    root = __webpack_require__(22);

	/* Built-in method references that are verified to be native. */
	var DataView = getNative(root, 'DataView');

	module.exports = DataView;


/***/ },
/* 411 */
/***/ function(module, exports, __webpack_require__) {

	var hashClear = __webpack_require__(477),
	    hashDelete = __webpack_require__(478),
	    hashGet = __webpack_require__(479),
	    hashHas = __webpack_require__(480),
	    hashSet = __webpack_require__(481);

	/**
	 * Creates a hash object.
	 *
	 * @private
	 * @constructor
	 * @param {Array} [entries] The key-value pairs to cache.
	 */
	function Hash(entries) {
	  var index = -1,
	      length = entries == null ? 0 : entries.length;

	  this.clear();
	  while (++index < length) {
	    var entry = entries[index];
	    this.set(entry[0], entry[1]);
	  }
	}

	// Add methods to `Hash`.
	Hash.prototype.clear = hashClear;
	Hash.prototype['delete'] = hashDelete;
	Hash.prototype.get = hashGet;
	Hash.prototype.has = hashHas;
	Hash.prototype.set = hashSet;

	module.exports = Hash;


/***/ },
/* 412 */
/***/ function(module, exports, __webpack_require__) {

	var getNative = __webpack_require__(34),
	    root = __webpack_require__(22);

	/* Built-in method references that are verified to be native. */
	var Promise = getNative(root, 'Promise');

	module.exports = Promise;


/***/ },
/* 413 */
/***/ function(module, exports, __webpack_require__) {

	var getNative = __webpack_require__(34),
	    root = __webpack_require__(22);

	/* Built-in method references that are verified to be native. */
	var WeakMap = getNative(root, 'WeakMap');

	module.exports = WeakMap;


/***/ },
/* 414 */
/***/ function(module, exports) {

	/**
	 * Adds the key-value `pair` to `map`.
	 *
	 * @private
	 * @param {Object} map The map to modify.
	 * @param {Array} pair The key-value pair to add.
	 * @returns {Object} Returns `map`.
	 */
	function addMapEntry(map, pair) {
	  // Don't return `map.set` because it's not chainable in IE 11.
	  map.set(pair[0], pair[1]);
	  return map;
	}

	module.exports = addMapEntry;


/***/ },
/* 415 */
/***/ function(module, exports) {

	/**
	 * Adds `value` to `set`.
	 *
	 * @private
	 * @param {Object} set The set to modify.
	 * @param {*} value The value to add.
	 * @returns {Object} Returns `set`.
	 */
	function addSetEntry(set, value) {
	  // Don't return `set.add` because it's not chainable in IE 11.
	  set.add(value);
	  return set;
	}

	module.exports = addSetEntry;


/***/ },
/* 416 */
/***/ function(module, exports) {

	/**
	 * A faster alternative to `Function#apply`, this function invokes `func`
	 * with the `this` binding of `thisArg` and the arguments of `args`.
	 *
	 * @private
	 * @param {Function} func The function to invoke.
	 * @param {*} thisArg The `this` binding of `func`.
	 * @param {Array} args The arguments to invoke `func` with.
	 * @returns {*} Returns the result of `func`.
	 */
	function apply(func, thisArg, args) {
	  switch (args.length) {
	    case 0: return func.call(thisArg);
	    case 1: return func.call(thisArg, args[0]);
	    case 2: return func.call(thisArg, args[0], args[1]);
	    case 3: return func.call(thisArg, args[0], args[1], args[2]);
	  }
	  return func.apply(thisArg, args);
	}

	module.exports = apply;


/***/ },
/* 417 */
/***/ function(module, exports) {

	/**
	 * A specialized version of `baseAggregator` for arrays.
	 *
	 * @private
	 * @param {Array} [array] The array to iterate over.
	 * @param {Function} setter The function to set `accumulator` values.
	 * @param {Function} iteratee The iteratee to transform keys.
	 * @param {Object} accumulator The initial aggregated object.
	 * @returns {Function} Returns `accumulator`.
	 */
	function arrayAggregator(array, setter, iteratee, accumulator) {
	  var index = -1,
	      length = array == null ? 0 : array.length;

	  while (++index < length) {
	    var value = array[index];
	    setter(accumulator, value, iteratee(value), array);
	  }
	  return accumulator;
	}

	module.exports = arrayAggregator;


/***/ },
/* 418 */
/***/ function(module, exports) {

	/**
	 * Converts an ASCII `string` to an array.
	 *
	 * @private
	 * @param {string} string The string to convert.
	 * @returns {Array} Returns the converted array.
	 */
	function asciiToArray(string) {
	  return string.split('');
	}

	module.exports = asciiToArray;


/***/ },
/* 419 */
/***/ function(module, exports, __webpack_require__) {

	var baseEach = __webpack_require__(54);

	/**
	 * Aggregates elements of `collection` on `accumulator` with keys transformed
	 * by `iteratee` and values set by `setter`.
	 *
	 * @private
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} setter The function to set `accumulator` values.
	 * @param {Function} iteratee The iteratee to transform keys.
	 * @param {Object} accumulator The initial aggregated object.
	 * @returns {Function} Returns `accumulator`.
	 */
	function baseAggregator(collection, setter, iteratee, accumulator) {
	  baseEach(collection, function(value, key, collection) {
	    setter(accumulator, value, iteratee(value), collection);
	  });
	  return accumulator;
	}

	module.exports = baseAggregator;


/***/ },
/* 420 */
/***/ function(module, exports, __webpack_require__) {

	var copyObject = __webpack_require__(33),
	    keys = __webpack_require__(23);

	/**
	 * The base implementation of `_.assign` without support for multiple sources
	 * or `customizer` functions.
	 *
	 * @private
	 * @param {Object} object The destination object.
	 * @param {Object} source The source object.
	 * @returns {Object} Returns `object`.
	 */
	function baseAssign(object, source) {
	  return object && copyObject(source, keys(source), object);
	}

	module.exports = baseAssign;


/***/ },
/* 421 */
/***/ function(module, exports, __webpack_require__) {

	var copyObject = __webpack_require__(33),
	    keysIn = __webpack_require__(123);

	/**
	 * The base implementation of `_.assignIn` without support for multiple sources
	 * or `customizer` functions.
	 *
	 * @private
	 * @param {Object} object The destination object.
	 * @param {Object} source The source object.
	 * @returns {Object} Returns `object`.
	 */
	function baseAssignIn(object, source) {
	  return object && copyObject(source, keysIn(source), object);
	}

	module.exports = baseAssignIn;


/***/ },
/* 422 */
/***/ function(module, exports, __webpack_require__) {

	var isObject = __webpack_require__(20);

	/** Built-in value references. */
	var objectCreate = Object.create;

	/**
	 * The base implementation of `_.create` without support for assigning
	 * properties to the created object.
	 *
	 * @private
	 * @param {Object} proto The object to inherit from.
	 * @returns {Object} Returns the new object.
	 */
	var baseCreate = (function() {
	  function object() {}
	  return function(proto) {
	    if (!isObject(proto)) {
	      return {};
	    }
	    if (objectCreate) {
	      return objectCreate(proto);
	    }
	    object.prototype = proto;
	    var result = new object;
	    object.prototype = undefined;
	    return result;
	  };
	}());

	module.exports = baseCreate;


/***/ },
/* 423 */
/***/ function(module, exports, __webpack_require__) {

	var baseEach = __webpack_require__(54);

	/**
	 * The base implementation of `_.filter` without support for iteratee shorthands.
	 *
	 * @private
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} predicate The function invoked per iteration.
	 * @returns {Array} Returns the new filtered array.
	 */
	function baseFilter(collection, predicate) {
	  var result = [];
	  baseEach(collection, function(value, index, collection) {
	    if (predicate(value, index, collection)) {
	      result.push(value);
	    }
	  });
	  return result;
	}

	module.exports = baseFilter;


/***/ },
/* 424 */
/***/ function(module, exports, __webpack_require__) {

	var createBaseFor = __webpack_require__(467);

	/**
	 * The base implementation of `baseForOwn` which iterates over `object`
	 * properties returned by `keysFunc` and invokes `iteratee` for each property.
	 * Iteratee functions may exit iteration early by explicitly returning `false`.
	 *
	 * @private
	 * @param {Object} object The object to iterate over.
	 * @param {Function} iteratee The function invoked per iteration.
	 * @param {Function} keysFunc The function to get the keys of `object`.
	 * @returns {Object} Returns `object`.
	 */
	var baseFor = createBaseFor();

	module.exports = baseFor;


/***/ },
/* 425 */
/***/ function(module, exports, __webpack_require__) {

	var baseFor = __webpack_require__(424),
	    keys = __webpack_require__(23);

	/**
	 * The base implementation of `_.forOwn` without support for iteratee shorthands.
	 *
	 * @private
	 * @param {Object} object The object to iterate over.
	 * @param {Function} iteratee The function invoked per iteration.
	 * @returns {Object} Returns `object`.
	 */
	function baseForOwn(object, iteratee) {
	  return object && baseFor(object, iteratee, keys);
	}

	module.exports = baseForOwn;


/***/ },
/* 426 */
/***/ function(module, exports) {

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * The base implementation of `_.has` without support for deep paths.
	 *
	 * @private
	 * @param {Object} [object] The object to query.
	 * @param {Array|string} key The key to check.
	 * @returns {boolean} Returns `true` if `key` exists, else `false`.
	 */
	function baseHas(object, key) {
	  return object != null && hasOwnProperty.call(object, key);
	}

	module.exports = baseHas;


/***/ },
/* 427 */
/***/ function(module, exports) {

	/**
	 * The base implementation of `_.hasIn` without support for deep paths.
	 *
	 * @private
	 * @param {Object} [object] The object to query.
	 * @param {Array|string} key The key to check.
	 * @returns {boolean} Returns `true` if `key` exists, else `false`.
	 */
	function baseHasIn(object, key) {
	  return object != null && key in Object(object);
	}

	module.exports = baseHasIn;


/***/ },
/* 428 */
/***/ function(module, exports) {

	/**
	 * This function is like `baseIndexOf` except that it accepts a comparator.
	 *
	 * @private
	 * @param {Array} array The array to inspect.
	 * @param {*} value The value to search for.
	 * @param {number} fromIndex The index to search from.
	 * @param {Function} comparator The comparator invoked per element.
	 * @returns {number} Returns the index of the matched value, else `-1`.
	 */
	function baseIndexOfWith(array, value, fromIndex, comparator) {
	  var index = fromIndex - 1,
	      length = array.length;

	  while (++index < length) {
	    if (comparator(array[index], value)) {
	      return index;
	    }
	  }
	  return -1;
	}

	module.exports = baseIndexOfWith;


/***/ },
/* 429 */
/***/ function(module, exports, __webpack_require__) {

	var baseGetTag = __webpack_require__(44),
	    isObjectLike = __webpack_require__(38);

	/** `Object#toString` result references. */
	var argsTag = '[object Arguments]';

	/**
	 * The base implementation of `_.isArguments`.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is an `arguments` object,
	 */
	function baseIsArguments(value) {
	  return isObjectLike(value) && baseGetTag(value) == argsTag;
	}

	module.exports = baseIsArguments;


/***/ },
/* 430 */
/***/ function(module, exports, __webpack_require__) {

	var Stack = __webpack_require__(108),
	    equalArrays = __webpack_require__(192),
	    equalByTag = __webpack_require__(470),
	    equalObjects = __webpack_require__(471),
	    getTag = __webpack_require__(115),
	    isArray = __webpack_require__(12),
	    isBuffer = __webpack_require__(87),
	    isTypedArray = __webpack_require__(122);

	/** Used to compose bitmasks for value comparisons. */
	var COMPARE_PARTIAL_FLAG = 1;

	/** `Object#toString` result references. */
	var argsTag = '[object Arguments]',
	    arrayTag = '[object Array]',
	    objectTag = '[object Object]';

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * A specialized version of `baseIsEqual` for arrays and objects which performs
	 * deep comparisons and tracks traversed objects enabling objects with circular
	 * references to be compared.
	 *
	 * @private
	 * @param {Object} object The object to compare.
	 * @param {Object} other The other object to compare.
	 * @param {number} bitmask The bitmask flags. See `baseIsEqual` for more details.
	 * @param {Function} customizer The function to customize comparisons.
	 * @param {Function} equalFunc The function to determine equivalents of values.
	 * @param {Object} [stack] Tracks traversed `object` and `other` objects.
	 * @returns {boolean} Returns `true` if the objects are equivalent, else `false`.
	 */
	function baseIsEqualDeep(object, other, bitmask, customizer, equalFunc, stack) {
	  var objIsArr = isArray(object),
	      othIsArr = isArray(other),
	      objTag = arrayTag,
	      othTag = arrayTag;

	  if (!objIsArr) {
	    objTag = getTag(object);
	    objTag = objTag == argsTag ? objectTag : objTag;
	  }
	  if (!othIsArr) {
	    othTag = getTag(other);
	    othTag = othTag == argsTag ? objectTag : othTag;
	  }
	  var objIsObj = objTag == objectTag,
	      othIsObj = othTag == objectTag,
	      isSameTag = objTag == othTag;

	  if (isSameTag && isBuffer(object)) {
	    if (!isBuffer(other)) {
	      return false;
	    }
	    objIsArr = true;
	    objIsObj = false;
	  }
	  if (isSameTag && !objIsObj) {
	    stack || (stack = new Stack);
	    return (objIsArr || isTypedArray(object))
	      ? equalArrays(object, other, bitmask, customizer, equalFunc, stack)
	      : equalByTag(object, other, objTag, bitmask, customizer, equalFunc, stack);
	  }
	  if (!(bitmask & COMPARE_PARTIAL_FLAG)) {
	    var objIsWrapped = objIsObj && hasOwnProperty.call(object, '__wrapped__'),
	        othIsWrapped = othIsObj && hasOwnProperty.call(other, '__wrapped__');

	    if (objIsWrapped || othIsWrapped) {
	      var objUnwrapped = objIsWrapped ? object.value() : object,
	          othUnwrapped = othIsWrapped ? other.value() : other;

	      stack || (stack = new Stack);
	      return equalFunc(objUnwrapped, othUnwrapped, bitmask, customizer, stack);
	    }
	  }
	  if (!isSameTag) {
	    return false;
	  }
	  stack || (stack = new Stack);
	  return equalObjects(object, other, bitmask, customizer, equalFunc, stack);
	}

	module.exports = baseIsEqualDeep;


/***/ },
/* 431 */
/***/ function(module, exports, __webpack_require__) {

	var Stack = __webpack_require__(108),
	    baseIsEqual = __webpack_require__(182);

	/** Used to compose bitmasks for value comparisons. */
	var COMPARE_PARTIAL_FLAG = 1,
	    COMPARE_UNORDERED_FLAG = 2;

	/**
	 * The base implementation of `_.isMatch` without support for iteratee shorthands.
	 *
	 * @private
	 * @param {Object} object The object to inspect.
	 * @param {Object} source The object of property values to match.
	 * @param {Array} matchData The property names, values, and compare flags to match.
	 * @param {Function} [customizer] The function to customize comparisons.
	 * @returns {boolean} Returns `true` if `object` is a match, else `false`.
	 */
	function baseIsMatch(object, source, matchData, customizer) {
	  var index = matchData.length,
	      length = index,
	      noCustomizer = !customizer;

	  if (object == null) {
	    return !length;
	  }
	  object = Object(object);
	  while (index--) {
	    var data = matchData[index];
	    if ((noCustomizer && data[2])
	          ? data[1] !== object[data[0]]
	          : !(data[0] in object)
	        ) {
	      return false;
	    }
	  }
	  while (++index < length) {
	    data = matchData[index];
	    var key = data[0],
	        objValue = object[key],
	        srcValue = data[1];

	    if (noCustomizer && data[2]) {
	      if (objValue === undefined && !(key in object)) {
	        return false;
	      }
	    } else {
	      var stack = new Stack;
	      if (customizer) {
	        var result = customizer(objValue, srcValue, key, object, source, stack);
	      }
	      if (!(result === undefined
	            ? baseIsEqual(srcValue, objValue, COMPARE_PARTIAL_FLAG | COMPARE_UNORDERED_FLAG, customizer, stack)
	            : result
	          )) {
	        return false;
	      }
	    }
	  }
	  return true;
	}

	module.exports = baseIsMatch;


/***/ },
/* 432 */
/***/ function(module, exports) {

	/**
	 * The base implementation of `_.isNaN` without support for number objects.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is `NaN`, else `false`.
	 */
	function baseIsNaN(value) {
	  return value !== value;
	}

	module.exports = baseIsNaN;


/***/ },
/* 433 */
/***/ function(module, exports, __webpack_require__) {

	var isFunction = __webpack_require__(88),
	    isMasked = __webpack_require__(487),
	    isObject = __webpack_require__(20),
	    toSource = __webpack_require__(204);

	/**
	 * Used to match `RegExp`
	 * [syntax characters](http://ecma-international.org/ecma-262/7.0/#sec-patterns).
	 */
	var reRegExpChar = /[\\^$.*+?()[\]{}|]/g;

	/** Used to detect host constructors (Safari). */
	var reIsHostCtor = /^\[object .+?Constructor\]$/;

	/** Used for built-in method references. */
	var funcProto = Function.prototype,
	    objectProto = Object.prototype;

	/** Used to resolve the decompiled source of functions. */
	var funcToString = funcProto.toString;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/** Used to detect if a method is native. */
	var reIsNative = RegExp('^' +
	  funcToString.call(hasOwnProperty).replace(reRegExpChar, '\\$&')
	  .replace(/hasOwnProperty|(function).*?(?=\\\()| for .+?(?=\\\])/g, '$1.*?') + '$'
	);

	/**
	 * The base implementation of `_.isNative` without bad shim checks.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is a native function,
	 *  else `false`.
	 */
	function baseIsNative(value) {
	  if (!isObject(value) || isMasked(value)) {
	    return false;
	  }
	  var pattern = isFunction(value) ? reIsNative : reIsHostCtor;
	  return pattern.test(toSource(value));
	}

	module.exports = baseIsNative;


/***/ },
/* 434 */
/***/ function(module, exports, __webpack_require__) {

	var baseGetTag = __webpack_require__(44),
	    isLength = __webpack_require__(121),
	    isObjectLike = __webpack_require__(38);

	/** `Object#toString` result references. */
	var argsTag = '[object Arguments]',
	    arrayTag = '[object Array]',
	    boolTag = '[object Boolean]',
	    dateTag = '[object Date]',
	    errorTag = '[object Error]',
	    funcTag = '[object Function]',
	    mapTag = '[object Map]',
	    numberTag = '[object Number]',
	    objectTag = '[object Object]',
	    regexpTag = '[object RegExp]',
	    setTag = '[object Set]',
	    stringTag = '[object String]',
	    weakMapTag = '[object WeakMap]';

	var arrayBufferTag = '[object ArrayBuffer]',
	    dataViewTag = '[object DataView]',
	    float32Tag = '[object Float32Array]',
	    float64Tag = '[object Float64Array]',
	    int8Tag = '[object Int8Array]',
	    int16Tag = '[object Int16Array]',
	    int32Tag = '[object Int32Array]',
	    uint8Tag = '[object Uint8Array]',
	    uint8ClampedTag = '[object Uint8ClampedArray]',
	    uint16Tag = '[object Uint16Array]',
	    uint32Tag = '[object Uint32Array]';

	/** Used to identify `toStringTag` values of typed arrays. */
	var typedArrayTags = {};
	typedArrayTags[float32Tag] = typedArrayTags[float64Tag] =
	typedArrayTags[int8Tag] = typedArrayTags[int16Tag] =
	typedArrayTags[int32Tag] = typedArrayTags[uint8Tag] =
	typedArrayTags[uint8ClampedTag] = typedArrayTags[uint16Tag] =
	typedArrayTags[uint32Tag] = true;
	typedArrayTags[argsTag] = typedArrayTags[arrayTag] =
	typedArrayTags[arrayBufferTag] = typedArrayTags[boolTag] =
	typedArrayTags[dataViewTag] = typedArrayTags[dateTag] =
	typedArrayTags[errorTag] = typedArrayTags[funcTag] =
	typedArrayTags[mapTag] = typedArrayTags[numberTag] =
	typedArrayTags[objectTag] = typedArrayTags[regexpTag] =
	typedArrayTags[setTag] = typedArrayTags[stringTag] =
	typedArrayTags[weakMapTag] = false;

	/**
	 * The base implementation of `_.isTypedArray` without Node.js optimizations.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is a typed array, else `false`.
	 */
	function baseIsTypedArray(value) {
	  return isObjectLike(value) &&
	    isLength(value.length) && !!typedArrayTags[baseGetTag(value)];
	}

	module.exports = baseIsTypedArray;


/***/ },
/* 435 */
/***/ function(module, exports, __webpack_require__) {

	var isObject = __webpack_require__(20),
	    isPrototype = __webpack_require__(57),
	    nativeKeysIn = __webpack_require__(500);

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * The base implementation of `_.keysIn` which doesn't treat sparse arrays as dense.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of property names.
	 */
	function baseKeysIn(object) {
	  if (!isObject(object)) {
	    return nativeKeysIn(object);
	  }
	  var isProto = isPrototype(object),
	      result = [];

	  for (var key in object) {
	    if (!(key == 'constructor' && (isProto || !hasOwnProperty.call(object, key)))) {
	      result.push(key);
	    }
	  }
	  return result;
	}

	module.exports = baseKeysIn;


/***/ },
/* 436 */
/***/ function(module, exports, __webpack_require__) {

	var baseIsMatch = __webpack_require__(431),
	    getMatchData = __webpack_require__(473),
	    matchesStrictComparable = __webpack_require__(201);

	/**
	 * The base implementation of `_.matches` which doesn't clone `source`.
	 *
	 * @private
	 * @param {Object} source The object of property values to match.
	 * @returns {Function} Returns the new spec function.
	 */
	function baseMatches(source) {
	  var matchData = getMatchData(source);
	  if (matchData.length == 1 && matchData[0][2]) {
	    return matchesStrictComparable(matchData[0][0], matchData[0][1]);
	  }
	  return function(object) {
	    return object === source || baseIsMatch(object, source, matchData);
	  };
	}

	module.exports = baseMatches;


/***/ },
/* 437 */
/***/ function(module, exports, __webpack_require__) {

	var baseIsEqual = __webpack_require__(182),
	    get = __webpack_require__(523),
	    hasIn = __webpack_require__(208),
	    isKey = __webpack_require__(117),
	    isStrictComparable = __webpack_require__(199),
	    matchesStrictComparable = __webpack_require__(201),
	    toKey = __webpack_require__(46);

	/** Used to compose bitmasks for value comparisons. */
	var COMPARE_PARTIAL_FLAG = 1,
	    COMPARE_UNORDERED_FLAG = 2;

	/**
	 * The base implementation of `_.matchesProperty` which doesn't clone `srcValue`.
	 *
	 * @private
	 * @param {string} path The path of the property to get.
	 * @param {*} srcValue The value to match.
	 * @returns {Function} Returns the new spec function.
	 */
	function baseMatchesProperty(path, srcValue) {
	  if (isKey(path) && isStrictComparable(srcValue)) {
	    return matchesStrictComparable(toKey(path), srcValue);
	  }
	  return function(object) {
	    var objValue = get(object, path);
	    return (objValue === undefined && objValue === srcValue)
	      ? hasIn(object, path)
	      : baseIsEqual(srcValue, objValue, COMPARE_PARTIAL_FLAG | COMPARE_UNORDERED_FLAG);
	  };
	}

	module.exports = baseMatchesProperty;


/***/ },
/* 438 */
/***/ function(module, exports, __webpack_require__) {

	var basePickBy = __webpack_require__(439),
	    hasIn = __webpack_require__(208);

	/**
	 * The base implementation of `_.pick` without support for individual
	 * property identifiers.
	 *
	 * @private
	 * @param {Object} object The source object.
	 * @param {string[]} paths The property paths to pick.
	 * @returns {Object} Returns the new object.
	 */
	function basePick(object, paths) {
	  object = Object(object);
	  return basePickBy(object, paths, function(value, path) {
	    return hasIn(object, path);
	  });
	}

	module.exports = basePick;


/***/ },
/* 439 */
/***/ function(module, exports, __webpack_require__) {

	var baseGet = __webpack_require__(79),
	    baseSet = __webpack_require__(443),
	    castPath = __webpack_require__(45);

	/**
	 * The base implementation of  `_.pickBy` without support for iteratee shorthands.
	 *
	 * @private
	 * @param {Object} object The source object.
	 * @param {string[]} paths The property paths to pick.
	 * @param {Function} predicate The function invoked per property.
	 * @returns {Object} Returns the new object.
	 */
	function basePickBy(object, paths, predicate) {
	  var index = -1,
	      length = paths.length,
	      result = {};

	  while (++index < length) {
	    var path = paths[index],
	        value = baseGet(object, path);

	    if (predicate(value, path)) {
	      baseSet(result, castPath(path, object), value);
	    }
	  }
	  return result;
	}

	module.exports = basePickBy;


/***/ },
/* 440 */
/***/ function(module, exports) {

	/**
	 * The base implementation of `_.property` without support for deep paths.
	 *
	 * @private
	 * @param {string} key The key of the property to get.
	 * @returns {Function} Returns the new accessor function.
	 */
	function baseProperty(key) {
	  return function(object) {
	    return object == null ? undefined : object[key];
	  };
	}

	module.exports = baseProperty;


/***/ },
/* 441 */
/***/ function(module, exports, __webpack_require__) {

	var baseGet = __webpack_require__(79);

	/**
	 * A specialized version of `baseProperty` which supports deep paths.
	 *
	 * @private
	 * @param {Array|string} path The path of the property to get.
	 * @returns {Function} Returns the new accessor function.
	 */
	function basePropertyDeep(path) {
	  return function(object) {
	    return baseGet(object, path);
	  };
	}

	module.exports = basePropertyDeep;


/***/ },
/* 442 */
/***/ function(module, exports, __webpack_require__) {

	var arrayMap = __webpack_require__(31),
	    baseIndexOf = __webpack_require__(80),
	    baseIndexOfWith = __webpack_require__(428),
	    baseUnary = __webpack_require__(81),
	    copyArray = __webpack_require__(113);

	/** Used for built-in method references. */
	var arrayProto = Array.prototype;

	/** Built-in value references. */
	var splice = arrayProto.splice;

	/**
	 * The base implementation of `_.pullAllBy` without support for iteratee
	 * shorthands.
	 *
	 * @private
	 * @param {Array} array The array to modify.
	 * @param {Array} values The values to remove.
	 * @param {Function} [iteratee] The iteratee invoked per element.
	 * @param {Function} [comparator] The comparator invoked per element.
	 * @returns {Array} Returns `array`.
	 */
	function basePullAll(array, values, iteratee, comparator) {
	  var indexOf = comparator ? baseIndexOfWith : baseIndexOf,
	      index = -1,
	      length = values.length,
	      seen = array;

	  if (array === values) {
	    values = copyArray(values);
	  }
	  if (iteratee) {
	    seen = arrayMap(array, baseUnary(iteratee));
	  }
	  while (++index < length) {
	    var fromIndex = 0,
	        value = values[index],
	        computed = iteratee ? iteratee(value) : value;

	    while ((fromIndex = indexOf(seen, computed, fromIndex, comparator)) > -1) {
	      if (seen !== array) {
	        splice.call(seen, fromIndex, 1);
	      }
	      splice.call(array, fromIndex, 1);
	    }
	  }
	  return array;
	}

	module.exports = basePullAll;


/***/ },
/* 443 */
/***/ function(module, exports, __webpack_require__) {

	var assignValue = __webpack_require__(77),
	    castPath = __webpack_require__(45),
	    isIndex = __webpack_require__(83),
	    isObject = __webpack_require__(20),
	    toKey = __webpack_require__(46);

	/**
	 * The base implementation of `_.set`.
	 *
	 * @private
	 * @param {Object} object The object to modify.
	 * @param {Array|string} path The path of the property to set.
	 * @param {*} value The value to set.
	 * @param {Function} [customizer] The function to customize path creation.
	 * @returns {Object} Returns `object`.
	 */
	function baseSet(object, path, value, customizer) {
	  if (!isObject(object)) {
	    return object;
	  }
	  path = castPath(path, object);

	  var index = -1,
	      length = path.length,
	      lastIndex = length - 1,
	      nested = object;

	  while (nested != null && ++index < length) {
	    var key = toKey(path[index]),
	        newValue = value;

	    if (index != lastIndex) {
	      var objValue = nested[key];
	      newValue = customizer ? customizer(objValue, key, nested) : undefined;
	      if (newValue === undefined) {
	        newValue = isObject(objValue)
	          ? objValue
	          : (isIndex(path[index + 1]) ? [] : {});
	      }
	    }
	    assignValue(nested, key, newValue);
	    nested = nested[key];
	  }
	  return object;
	}

	module.exports = baseSet;


/***/ },
/* 444 */
/***/ function(module, exports, __webpack_require__) {

	var constant = __webpack_require__(519),
	    defineProperty = __webpack_require__(191),
	    identity = __webpack_require__(60);

	/**
	 * The base implementation of `setToString` without support for hot loop shorting.
	 *
	 * @private
	 * @param {Function} func The function to modify.
	 * @param {Function} string The `toString` result.
	 * @returns {Function} Returns `func`.
	 */
	var baseSetToString = !defineProperty ? identity : function(func, string) {
	  return defineProperty(func, 'toString', {
	    'configurable': true,
	    'enumerable': false,
	    'value': constant(string),
	    'writable': true
	  });
	};

	module.exports = baseSetToString;


/***/ },
/* 445 */
/***/ function(module, exports, __webpack_require__) {

	var baseEach = __webpack_require__(54);

	/**
	 * The base implementation of `_.some` without support for iteratee shorthands.
	 *
	 * @private
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {Function} predicate The function invoked per iteration.
	 * @returns {boolean} Returns `true` if any element passes the predicate check,
	 *  else `false`.
	 */
	function baseSome(collection, predicate) {
	  var result;

	  baseEach(collection, function(value, index, collection) {
	    result = predicate(value, index, collection);
	    return !result;
	  });
	  return !!result;
	}

	module.exports = baseSome;


/***/ },
/* 446 */
/***/ function(module, exports) {

	/**
	 * The base implementation of `_.sortBy` which uses `comparer` to define the
	 * sort order of `array` and replaces criteria objects with their corresponding
	 * values.
	 *
	 * @private
	 * @param {Array} array The array to sort.
	 * @param {Function} comparer The function to define sort order.
	 * @returns {Array} Returns `array`.
	 */
	function baseSortBy(array, comparer) {
	  var length = array.length;

	  array.sort(comparer);
	  while (length--) {
	    array[length] = array[length].value;
	  }
	  return array;
	}

	module.exports = baseSortBy;


/***/ },
/* 447 */
/***/ function(module, exports) {

	/**
	 * The base implementation of `_.times` without support for iteratee shorthands
	 * or max array length checks.
	 *
	 * @private
	 * @param {number} n The number of times to invoke `iteratee`.
	 * @param {Function} iteratee The function invoked per iteration.
	 * @returns {Array} Returns the array of results.
	 */
	function baseTimes(n, iteratee) {
	  var index = -1,
	      result = Array(n);

	  while (++index < n) {
	    result[index] = iteratee(index);
	  }
	  return result;
	}

	module.exports = baseTimes;


/***/ },
/* 448 */
/***/ function(module, exports, __webpack_require__) {

	var arrayMap = __webpack_require__(31);

	/**
	 * The base implementation of `_.values` and `_.valuesIn` which creates an
	 * array of `object` property values corresponding to the property names
	 * of `props`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @param {Array} props The property names to get values for.
	 * @returns {Object} Returns the array of property values.
	 */
	function baseValues(object, props) {
	  return arrayMap(props, function(key) {
	    return object[key];
	  });
	}

	module.exports = baseValues;


/***/ },
/* 449 */
/***/ function(module, exports, __webpack_require__) {

	var baseDifference = __webpack_require__(179),
	    baseFlatten = __webpack_require__(55),
	    baseUniq = __webpack_require__(188);

	/**
	 * The base implementation of methods like `_.xor`, without support for
	 * iteratee shorthands, that accepts an array of arrays to inspect.
	 *
	 * @private
	 * @param {Array} arrays The arrays to inspect.
	 * @param {Function} [iteratee] The iteratee invoked per element.
	 * @param {Function} [comparator] The comparator invoked per element.
	 * @returns {Array} Returns the new array of values.
	 */
	function baseXor(arrays, iteratee, comparator) {
	  var length = arrays.length;
	  if (length < 2) {
	    return length ? baseUniq(arrays[0]) : [];
	  }
	  var index = -1,
	      result = Array(length);

	  while (++index < length) {
	    var array = arrays[index],
	        othIndex = -1;

	    while (++othIndex < length) {
	      if (othIndex != index) {
	        result[index] = baseDifference(result[index] || array, arrays[othIndex], iteratee, comparator);
	      }
	    }
	  }
	  return baseUniq(baseFlatten(result, 1), iteratee, comparator);
	}

	module.exports = baseXor;


/***/ },
/* 450 */
/***/ function(module, exports, __webpack_require__) {

	var identity = __webpack_require__(60);

	/**
	 * Casts `value` to `identity` if it's not a function.
	 *
	 * @private
	 * @param {*} value The value to inspect.
	 * @returns {Function} Returns cast function.
	 */
	function castFunction(value) {
	  return typeof value == 'function' ? value : identity;
	}

	module.exports = castFunction;


/***/ },
/* 451 */
/***/ function(module, exports, __webpack_require__) {

	var baseSlice = __webpack_require__(186);

	/**
	 * Casts `array` to a slice if it's needed.
	 *
	 * @private
	 * @param {Array} array The array to inspect.
	 * @param {number} start The start position.
	 * @param {number} [end=array.length] The end position.
	 * @returns {Array} Returns the cast slice.
	 */
	function castSlice(array, start, end) {
	  var length = array.length;
	  end = end === undefined ? length : end;
	  return (!start && end >= length) ? array : baseSlice(array, start, end);
	}

	module.exports = castSlice;


/***/ },
/* 452 */
/***/ function(module, exports, __webpack_require__) {

	var baseIndexOf = __webpack_require__(80);

	/**
	 * Used by `_.trim` and `_.trimEnd` to get the index of the last string symbol
	 * that is not found in the character symbols.
	 *
	 * @private
	 * @param {Array} strSymbols The string symbols to inspect.
	 * @param {Array} chrSymbols The character symbols to find.
	 * @returns {number} Returns the index of the last unmatched string symbol.
	 */
	function charsEndIndex(strSymbols, chrSymbols) {
	  var index = strSymbols.length;

	  while (index-- && baseIndexOf(chrSymbols, strSymbols[index], 0) > -1) {}
	  return index;
	}

	module.exports = charsEndIndex;


/***/ },
/* 453 */
/***/ function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(module) {var root = __webpack_require__(22);

	/** Detect free variable `exports`. */
	var freeExports = typeof exports == 'object' && exports && !exports.nodeType && exports;

	/** Detect free variable `module`. */
	var freeModule = freeExports && typeof module == 'object' && module && !module.nodeType && module;

	/** Detect the popular CommonJS extension `module.exports`. */
	var moduleExports = freeModule && freeModule.exports === freeExports;

	/** Built-in value references. */
	var Buffer = moduleExports ? root.Buffer : undefined,
	    allocUnsafe = Buffer ? Buffer.allocUnsafe : undefined;

	/**
	 * Creates a clone of  `buffer`.
	 *
	 * @private
	 * @param {Buffer} buffer The buffer to clone.
	 * @param {boolean} [isDeep] Specify a deep clone.
	 * @returns {Buffer} Returns the cloned buffer.
	 */
	function cloneBuffer(buffer, isDeep) {
	  if (isDeep) {
	    return buffer.slice();
	  }
	  var length = buffer.length,
	      result = allocUnsafe ? allocUnsafe(length) : new buffer.constructor(length);

	  buffer.copy(result);
	  return result;
	}

	module.exports = cloneBuffer;

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(68)(module)))

/***/ },
/* 454 */
/***/ function(module, exports, __webpack_require__) {

	var cloneArrayBuffer = __webpack_require__(112);

	/**
	 * Creates a clone of `dataView`.
	 *
	 * @private
	 * @param {Object} dataView The data view to clone.
	 * @param {boolean} [isDeep] Specify a deep clone.
	 * @returns {Object} Returns the cloned data view.
	 */
	function cloneDataView(dataView, isDeep) {
	  var buffer = isDeep ? cloneArrayBuffer(dataView.buffer) : dataView.buffer;
	  return new dataView.constructor(buffer, dataView.byteOffset, dataView.byteLength);
	}

	module.exports = cloneDataView;


/***/ },
/* 455 */
/***/ function(module, exports, __webpack_require__) {

	var addMapEntry = __webpack_require__(414),
	    arrayReduce = __webpack_require__(177),
	    mapToArray = __webpack_require__(200);

	/** Used to compose bitmasks for cloning. */
	var CLONE_DEEP_FLAG = 1;

	/**
	 * Creates a clone of `map`.
	 *
	 * @private
	 * @param {Object} map The map to clone.
	 * @param {Function} cloneFunc The function to clone values.
	 * @param {boolean} [isDeep] Specify a deep clone.
	 * @returns {Object} Returns the cloned map.
	 */
	function cloneMap(map, isDeep, cloneFunc) {
	  var array = isDeep ? cloneFunc(mapToArray(map), CLONE_DEEP_FLAG) : mapToArray(map);
	  return arrayReduce(array, addMapEntry, new map.constructor);
	}

	module.exports = cloneMap;


/***/ },
/* 456 */
/***/ function(module, exports) {

	/** Used to match `RegExp` flags from their coerced string values. */
	var reFlags = /\w*$/;

	/**
	 * Creates a clone of `regexp`.
	 *
	 * @private
	 * @param {Object} regexp The regexp to clone.
	 * @returns {Object} Returns the cloned regexp.
	 */
	function cloneRegExp(regexp) {
	  var result = new regexp.constructor(regexp.source, reFlags.exec(regexp));
	  result.lastIndex = regexp.lastIndex;
	  return result;
	}

	module.exports = cloneRegExp;


/***/ },
/* 457 */
/***/ function(module, exports, __webpack_require__) {

	var addSetEntry = __webpack_require__(415),
	    arrayReduce = __webpack_require__(177),
	    setToArray = __webpack_require__(85);

	/** Used to compose bitmasks for cloning. */
	var CLONE_DEEP_FLAG = 1;

	/**
	 * Creates a clone of `set`.
	 *
	 * @private
	 * @param {Object} set The set to clone.
	 * @param {Function} cloneFunc The function to clone values.
	 * @param {boolean} [isDeep] Specify a deep clone.
	 * @returns {Object} Returns the cloned set.
	 */
	function cloneSet(set, isDeep, cloneFunc) {
	  var array = isDeep ? cloneFunc(setToArray(set), CLONE_DEEP_FLAG) : setToArray(set);
	  return arrayReduce(array, addSetEntry, new set.constructor);
	}

	module.exports = cloneSet;


/***/ },
/* 458 */
/***/ function(module, exports, __webpack_require__) {

	var Symbol = __webpack_require__(43);

	/** Used to convert symbols to primitives and strings. */
	var symbolProto = Symbol ? Symbol.prototype : undefined,
	    symbolValueOf = symbolProto ? symbolProto.valueOf : undefined;

	/**
	 * Creates a clone of the `symbol` object.
	 *
	 * @private
	 * @param {Object} symbol The symbol object to clone.
	 * @returns {Object} Returns the cloned symbol object.
	 */
	function cloneSymbol(symbol) {
	  return symbolValueOf ? Object(symbolValueOf.call(symbol)) : {};
	}

	module.exports = cloneSymbol;


/***/ },
/* 459 */
/***/ function(module, exports, __webpack_require__) {

	var cloneArrayBuffer = __webpack_require__(112);

	/**
	 * Creates a clone of `typedArray`.
	 *
	 * @private
	 * @param {Object} typedArray The typed array to clone.
	 * @param {boolean} [isDeep] Specify a deep clone.
	 * @returns {Object} Returns the cloned typed array.
	 */
	function cloneTypedArray(typedArray, isDeep) {
	  var buffer = isDeep ? cloneArrayBuffer(typedArray.buffer) : typedArray.buffer;
	  return new typedArray.constructor(buffer, typedArray.byteOffset, typedArray.length);
	}

	module.exports = cloneTypedArray;


/***/ },
/* 460 */
/***/ function(module, exports, __webpack_require__) {

	var isSymbol = __webpack_require__(61);

	/**
	 * Compares values to sort them in ascending order.
	 *
	 * @private
	 * @param {*} value The value to compare.
	 * @param {*} other The other value to compare.
	 * @returns {number} Returns the sort order indicator for `value`.
	 */
	function compareAscending(value, other) {
	  if (value !== other) {
	    var valIsDefined = value !== undefined,
	        valIsNull = value === null,
	        valIsReflexive = value === value,
	        valIsSymbol = isSymbol(value);

	    var othIsDefined = other !== undefined,
	        othIsNull = other === null,
	        othIsReflexive = other === other,
	        othIsSymbol = isSymbol(other);

	    if ((!othIsNull && !othIsSymbol && !valIsSymbol && value > other) ||
	        (valIsSymbol && othIsDefined && othIsReflexive && !othIsNull && !othIsSymbol) ||
	        (valIsNull && othIsDefined && othIsReflexive) ||
	        (!valIsDefined && othIsReflexive) ||
	        !valIsReflexive) {
	      return 1;
	    }
	    if ((!valIsNull && !valIsSymbol && !othIsSymbol && value < other) ||
	        (othIsSymbol && valIsDefined && valIsReflexive && !valIsNull && !valIsSymbol) ||
	        (othIsNull && valIsDefined && valIsReflexive) ||
	        (!othIsDefined && valIsReflexive) ||
	        !othIsReflexive) {
	      return -1;
	    }
	  }
	  return 0;
	}

	module.exports = compareAscending;


/***/ },
/* 461 */
/***/ function(module, exports, __webpack_require__) {

	var compareAscending = __webpack_require__(460);

	/**
	 * Used by `_.orderBy` to compare multiple properties of a value to another
	 * and stable sort them.
	 *
	 * If `orders` is unspecified, all values are sorted in ascending order. Otherwise,
	 * specify an order of "desc" for descending or "asc" for ascending sort order
	 * of corresponding values.
	 *
	 * @private
	 * @param {Object} object The object to compare.
	 * @param {Object} other The other object to compare.
	 * @param {boolean[]|string[]} orders The order to sort by for each property.
	 * @returns {number} Returns the sort order indicator for `object`.
	 */
	function compareMultiple(object, other, orders) {
	  var index = -1,
	      objCriteria = object.criteria,
	      othCriteria = other.criteria,
	      length = objCriteria.length,
	      ordersLength = orders.length;

	  while (++index < length) {
	    var result = compareAscending(objCriteria[index], othCriteria[index]);
	    if (result) {
	      if (index >= ordersLength) {
	        return result;
	      }
	      var order = orders[index];
	      return result * (order == 'desc' ? -1 : 1);
	    }
	  }
	  // Fixes an `Array#sort` bug in the JS engine embedded in Adobe applications
	  // that causes it, under certain circumstances, to provide the same value for
	  // `object` and `other`. See https://github.com/jashkenas/underscore/pull/1247
	  // for more details.
	  //
	  // This also ensures a stable sort in V8 and other engines.
	  // See https://bugs.chromium.org/p/v8/issues/detail?id=90 for more details.
	  return object.index - other.index;
	}

	module.exports = compareMultiple;


/***/ },
/* 462 */
/***/ function(module, exports, __webpack_require__) {

	var copyObject = __webpack_require__(33),
	    getSymbols = __webpack_require__(114);

	/**
	 * Copies own symbols of `source` to `object`.
	 *
	 * @private
	 * @param {Object} source The object to copy symbols from.
	 * @param {Object} [object={}] The object to copy symbols to.
	 * @returns {Object} Returns `object`.
	 */
	function copySymbols(source, object) {
	  return copyObject(source, getSymbols(source), object);
	}

	module.exports = copySymbols;


/***/ },
/* 463 */
/***/ function(module, exports, __webpack_require__) {

	var copyObject = __webpack_require__(33),
	    getSymbolsIn = __webpack_require__(197);

	/**
	 * Copies own and inherited symbols of `source` to `object`.
	 *
	 * @private
	 * @param {Object} source The object to copy symbols from.
	 * @param {Object} [object={}] The object to copy symbols to.
	 * @returns {Object} Returns `object`.
	 */
	function copySymbolsIn(source, object) {
	  return copyObject(source, getSymbolsIn(source), object);
	}

	module.exports = copySymbolsIn;


/***/ },
/* 464 */
/***/ function(module, exports, __webpack_require__) {

	var root = __webpack_require__(22);

	/** Used to detect overreaching core-js shims. */
	var coreJsData = root['__core-js_shared__'];

	module.exports = coreJsData;


/***/ },
/* 465 */
/***/ function(module, exports, __webpack_require__) {

	var arrayAggregator = __webpack_require__(417),
	    baseAggregator = __webpack_require__(419),
	    baseIteratee = __webpack_require__(32),
	    isArray = __webpack_require__(12);

	/**
	 * Creates a function like `_.groupBy`.
	 *
	 * @private
	 * @param {Function} setter The function to set accumulator values.
	 * @param {Function} [initializer] The accumulator object initializer.
	 * @returns {Function} Returns the new aggregator function.
	 */
	function createAggregator(setter, initializer) {
	  return function(collection, iteratee) {
	    var func = isArray(collection) ? arrayAggregator : baseAggregator,
	        accumulator = initializer ? initializer() : {};

	    return func(collection, setter, baseIteratee(iteratee, 2), accumulator);
	  };
	}

	module.exports = createAggregator;


/***/ },
/* 466 */
/***/ function(module, exports, __webpack_require__) {

	var isArrayLike = __webpack_require__(25);

	/**
	 * Creates a `baseEach` or `baseEachRight` function.
	 *
	 * @private
	 * @param {Function} eachFunc The function to iterate over a collection.
	 * @param {boolean} [fromRight] Specify iterating from right to left.
	 * @returns {Function} Returns the new base function.
	 */
	function createBaseEach(eachFunc, fromRight) {
	  return function(collection, iteratee) {
	    if (collection == null) {
	      return collection;
	    }
	    if (!isArrayLike(collection)) {
	      return eachFunc(collection, iteratee);
	    }
	    var length = collection.length,
	        index = fromRight ? length : -1,
	        iterable = Object(collection);

	    while ((fromRight ? index-- : ++index < length)) {
	      if (iteratee(iterable[index], index, iterable) === false) {
	        break;
	      }
	    }
	    return collection;
	  };
	}

	module.exports = createBaseEach;


/***/ },
/* 467 */
/***/ function(module, exports) {

	/**
	 * Creates a base function for methods like `_.forIn` and `_.forOwn`.
	 *
	 * @private
	 * @param {boolean} [fromRight] Specify iterating from right to left.
	 * @returns {Function} Returns the new base function.
	 */
	function createBaseFor(fromRight) {
	  return function(object, iteratee, keysFunc) {
	    var index = -1,
	        iterable = Object(object),
	        props = keysFunc(object),
	        length = props.length;

	    while (length--) {
	      var key = props[fromRight ? length : ++index];
	      if (iteratee(iterable[key], key, iterable) === false) {
	        break;
	      }
	    }
	    return object;
	  };
	}

	module.exports = createBaseFor;


/***/ },
/* 468 */
/***/ function(module, exports, __webpack_require__) {

	var baseIteratee = __webpack_require__(32),
	    isArrayLike = __webpack_require__(25),
	    keys = __webpack_require__(23);

	/**
	 * Creates a `_.find` or `_.findLast` function.
	 *
	 * @private
	 * @param {Function} findIndexFunc The function to find the collection index.
	 * @returns {Function} Returns the new find function.
	 */
	function createFind(findIndexFunc) {
	  return function(collection, predicate, fromIndex) {
	    var iterable = Object(collection);
	    if (!isArrayLike(collection)) {
	      var iteratee = baseIteratee(predicate, 3);
	      collection = keys(collection);
	      predicate = function(key) { return iteratee(iterable[key], key, iterable); };
	    }
	    var index = findIndexFunc(collection, predicate, fromIndex);
	    return index > -1 ? iterable[iteratee ? collection[index] : index] : undefined;
	  };
	}

	module.exports = createFind;


/***/ },
/* 469 */
/***/ function(module, exports, __webpack_require__) {

	var Set = __webpack_require__(170),
	    noop = __webpack_require__(527),
	    setToArray = __webpack_require__(85);

	/** Used as references for various `Number` constants. */
	var INFINITY = 1 / 0;

	/**
	 * Creates a set object of `values`.
	 *
	 * @private
	 * @param {Array} values The values to add to the set.
	 * @returns {Object} Returns the new set.
	 */
	var createSet = !(Set && (1 / setToArray(new Set([,-0]))[1]) == INFINITY) ? noop : function(values) {
	  return new Set(values);
	};

	module.exports = createSet;


/***/ },
/* 470 */
/***/ function(module, exports, __webpack_require__) {

	var Symbol = __webpack_require__(43),
	    Uint8Array = __webpack_require__(171),
	    eq = __webpack_require__(35),
	    equalArrays = __webpack_require__(192),
	    mapToArray = __webpack_require__(200),
	    setToArray = __webpack_require__(85);

	/** Used to compose bitmasks for value comparisons. */
	var COMPARE_PARTIAL_FLAG = 1,
	    COMPARE_UNORDERED_FLAG = 2;

	/** `Object#toString` result references. */
	var boolTag = '[object Boolean]',
	    dateTag = '[object Date]',
	    errorTag = '[object Error]',
	    mapTag = '[object Map]',
	    numberTag = '[object Number]',
	    regexpTag = '[object RegExp]',
	    setTag = '[object Set]',
	    stringTag = '[object String]',
	    symbolTag = '[object Symbol]';

	var arrayBufferTag = '[object ArrayBuffer]',
	    dataViewTag = '[object DataView]';

	/** Used to convert symbols to primitives and strings. */
	var symbolProto = Symbol ? Symbol.prototype : undefined,
	    symbolValueOf = symbolProto ? symbolProto.valueOf : undefined;

	/**
	 * A specialized version of `baseIsEqualDeep` for comparing objects of
	 * the same `toStringTag`.
	 *
	 * **Note:** This function only supports comparing values with tags of
	 * `Boolean`, `Date`, `Error`, `Number`, `RegExp`, or `String`.
	 *
	 * @private
	 * @param {Object} object The object to compare.
	 * @param {Object} other The other object to compare.
	 * @param {string} tag The `toStringTag` of the objects to compare.
	 * @param {number} bitmask The bitmask flags. See `baseIsEqual` for more details.
	 * @param {Function} customizer The function to customize comparisons.
	 * @param {Function} equalFunc The function to determine equivalents of values.
	 * @param {Object} stack Tracks traversed `object` and `other` objects.
	 * @returns {boolean} Returns `true` if the objects are equivalent, else `false`.
	 */
	function equalByTag(object, other, tag, bitmask, customizer, equalFunc, stack) {
	  switch (tag) {
	    case dataViewTag:
	      if ((object.byteLength != other.byteLength) ||
	          (object.byteOffset != other.byteOffset)) {
	        return false;
	      }
	      object = object.buffer;
	      other = other.buffer;

	    case arrayBufferTag:
	      if ((object.byteLength != other.byteLength) ||
	          !equalFunc(new Uint8Array(object), new Uint8Array(other))) {
	        return false;
	      }
	      return true;

	    case boolTag:
	    case dateTag:
	    case numberTag:
	      // Coerce booleans to `1` or `0` and dates to milliseconds.
	      // Invalid dates are coerced to `NaN`.
	      return eq(+object, +other);

	    case errorTag:
	      return object.name == other.name && object.message == other.message;

	    case regexpTag:
	    case stringTag:
	      // Coerce regexes to strings and treat strings, primitives and objects,
	      // as equal. See http://www.ecma-international.org/ecma-262/7.0/#sec-regexp.prototype.tostring
	      // for more details.
	      return object == (other + '');

	    case mapTag:
	      var convert = mapToArray;

	    case setTag:
	      var isPartial = bitmask & COMPARE_PARTIAL_FLAG;
	      convert || (convert = setToArray);

	      if (object.size != other.size && !isPartial) {
	        return false;
	      }
	      // Assume cyclic values are equal.
	      var stacked = stack.get(object);
	      if (stacked) {
	        return stacked == other;
	      }
	      bitmask |= COMPARE_UNORDERED_FLAG;

	      // Recursively compare objects (susceptible to call stack limits).
	      stack.set(object, other);
	      var result = equalArrays(convert(object), convert(other), bitmask, customizer, equalFunc, stack);
	      stack['delete'](object);
	      return result;

	    case symbolTag:
	      if (symbolValueOf) {
	        return symbolValueOf.call(object) == symbolValueOf.call(other);
	      }
	  }
	  return false;
	}

	module.exports = equalByTag;


/***/ },
/* 471 */
/***/ function(module, exports, __webpack_require__) {

	var keys = __webpack_require__(23);

	/** Used to compose bitmasks for value comparisons. */
	var COMPARE_PARTIAL_FLAG = 1;

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * A specialized version of `baseIsEqualDeep` for objects with support for
	 * partial deep comparisons.
	 *
	 * @private
	 * @param {Object} object The object to compare.
	 * @param {Object} other The other object to compare.
	 * @param {number} bitmask The bitmask flags. See `baseIsEqual` for more details.
	 * @param {Function} customizer The function to customize comparisons.
	 * @param {Function} equalFunc The function to determine equivalents of values.
	 * @param {Object} stack Tracks traversed `object` and `other` objects.
	 * @returns {boolean} Returns `true` if the objects are equivalent, else `false`.
	 */
	function equalObjects(object, other, bitmask, customizer, equalFunc, stack) {
	  var isPartial = bitmask & COMPARE_PARTIAL_FLAG,
	      objProps = keys(object),
	      objLength = objProps.length,
	      othProps = keys(other),
	      othLength = othProps.length;

	  if (objLength != othLength && !isPartial) {
	    return false;
	  }
	  var index = objLength;
	  while (index--) {
	    var key = objProps[index];
	    if (!(isPartial ? key in other : hasOwnProperty.call(other, key))) {
	      return false;
	    }
	  }
	  // Assume cyclic values are equal.
	  var stacked = stack.get(object);
	  if (stacked && stack.get(other)) {
	    return stacked == other;
	  }
	  var result = true;
	  stack.set(object, other);
	  stack.set(other, object);

	  var skipCtor = isPartial;
	  while (++index < objLength) {
	    key = objProps[index];
	    var objValue = object[key],
	        othValue = other[key];

	    if (customizer) {
	      var compared = isPartial
	        ? customizer(othValue, objValue, key, other, object, stack)
	        : customizer(objValue, othValue, key, object, other, stack);
	    }
	    // Recursively compare objects (susceptible to call stack limits).
	    if (!(compared === undefined
	          ? (objValue === othValue || equalFunc(objValue, othValue, bitmask, customizer, stack))
	          : compared
	        )) {
	      result = false;
	      break;
	    }
	    skipCtor || (skipCtor = key == 'constructor');
	  }
	  if (result && !skipCtor) {
	    var objCtor = object.constructor,
	        othCtor = other.constructor;

	    // Non `Object` object instances with different constructors are not equal.
	    if (objCtor != othCtor &&
	        ('constructor' in object && 'constructor' in other) &&
	        !(typeof objCtor == 'function' && objCtor instanceof objCtor &&
	          typeof othCtor == 'function' && othCtor instanceof othCtor)) {
	      result = false;
	    }
	  }
	  stack['delete'](object);
	  stack['delete'](other);
	  return result;
	}

	module.exports = equalObjects;


/***/ },
/* 472 */
/***/ function(module, exports, __webpack_require__) {

	var baseGetAllKeys = __webpack_require__(181),
	    getSymbols = __webpack_require__(114),
	    keys = __webpack_require__(23);

	/**
	 * Creates an array of own enumerable property names and symbols of `object`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of property names and symbols.
	 */
	function getAllKeys(object) {
	  return baseGetAllKeys(object, keys, getSymbols);
	}

	module.exports = getAllKeys;


/***/ },
/* 473 */
/***/ function(module, exports, __webpack_require__) {

	var isStrictComparable = __webpack_require__(199),
	    keys = __webpack_require__(23);

	/**
	 * Gets the property names, values, and compare flags of `object`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the match data of `object`.
	 */
	function getMatchData(object) {
	  var result = keys(object),
	      length = result.length;

	  while (length--) {
	    var key = result[length],
	        value = object[key];

	    result[length] = [key, value, isStrictComparable(value)];
	  }
	  return result;
	}

	module.exports = getMatchData;


/***/ },
/* 474 */
/***/ function(module, exports, __webpack_require__) {

	var Symbol = __webpack_require__(43);

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * Used to resolve the
	 * [`toStringTag`](http://ecma-international.org/ecma-262/7.0/#sec-object.prototype.tostring)
	 * of values.
	 */
	var nativeObjectToString = objectProto.toString;

	/** Built-in value references. */
	var symToStringTag = Symbol ? Symbol.toStringTag : undefined;

	/**
	 * A specialized version of `baseGetTag` which ignores `Symbol.toStringTag` values.
	 *
	 * @private
	 * @param {*} value The value to query.
	 * @returns {string} Returns the raw `toStringTag`.
	 */
	function getRawTag(value) {
	  var isOwn = hasOwnProperty.call(value, symToStringTag),
	      tag = value[symToStringTag];

	  try {
	    value[symToStringTag] = undefined;
	    var unmasked = true;
	  } catch (e) {}

	  var result = nativeObjectToString.call(value);
	  if (unmasked) {
	    if (isOwn) {
	      value[symToStringTag] = tag;
	    } else {
	      delete value[symToStringTag];
	    }
	  }
	  return result;
	}

	module.exports = getRawTag;


/***/ },
/* 475 */
/***/ function(module, exports) {

	/**
	 * Gets the value at `key` of `object`.
	 *
	 * @private
	 * @param {Object} [object] The object to query.
	 * @param {string} key The key of the property to get.
	 * @returns {*} Returns the property value.
	 */
	function getValue(object, key) {
	  return object == null ? undefined : object[key];
	}

	module.exports = getValue;


/***/ },
/* 476 */
/***/ function(module, exports) {

	/** Used to compose unicode character classes. */
	var rsAstralRange = '\\ud800-\\udfff',
	    rsComboMarksRange = '\\u0300-\\u036f',
	    reComboHalfMarksRange = '\\ufe20-\\ufe2f',
	    rsComboSymbolsRange = '\\u20d0-\\u20ff',
	    rsComboRange = rsComboMarksRange + reComboHalfMarksRange + rsComboSymbolsRange,
	    rsVarRange = '\\ufe0e\\ufe0f';

	/** Used to compose unicode capture groups. */
	var rsZWJ = '\\u200d';

	/** Used to detect strings with [zero-width joiners or code points from the astral planes](http://eev.ee/blog/2015/09/12/dark-corners-of-unicode/). */
	var reHasUnicode = RegExp('[' + rsZWJ + rsAstralRange  + rsComboRange + rsVarRange + ']');

	/**
	 * Checks if `string` contains Unicode symbols.
	 *
	 * @private
	 * @param {string} string The string to inspect.
	 * @returns {boolean} Returns `true` if a symbol is found, else `false`.
	 */
	function hasUnicode(string) {
	  return reHasUnicode.test(string);
	}

	module.exports = hasUnicode;


/***/ },
/* 477 */
/***/ function(module, exports, __webpack_require__) {

	var nativeCreate = __webpack_require__(84);

	/**
	 * Removes all key-value entries from the hash.
	 *
	 * @private
	 * @name clear
	 * @memberOf Hash
	 */
	function hashClear() {
	  this.__data__ = nativeCreate ? nativeCreate(null) : {};
	  this.size = 0;
	}

	module.exports = hashClear;


/***/ },
/* 478 */
/***/ function(module, exports) {

	/**
	 * Removes `key` and its value from the hash.
	 *
	 * @private
	 * @name delete
	 * @memberOf Hash
	 * @param {Object} hash The hash to modify.
	 * @param {string} key The key of the value to remove.
	 * @returns {boolean} Returns `true` if the entry was removed, else `false`.
	 */
	function hashDelete(key) {
	  var result = this.has(key) && delete this.__data__[key];
	  this.size -= result ? 1 : 0;
	  return result;
	}

	module.exports = hashDelete;


/***/ },
/* 479 */
/***/ function(module, exports, __webpack_require__) {

	var nativeCreate = __webpack_require__(84);

	/** Used to stand-in for `undefined` hash values. */
	var HASH_UNDEFINED = '__lodash_hash_undefined__';

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * Gets the hash value for `key`.
	 *
	 * @private
	 * @name get
	 * @memberOf Hash
	 * @param {string} key The key of the value to get.
	 * @returns {*} Returns the entry value.
	 */
	function hashGet(key) {
	  var data = this.__data__;
	  if (nativeCreate) {
	    var result = data[key];
	    return result === HASH_UNDEFINED ? undefined : result;
	  }
	  return hasOwnProperty.call(data, key) ? data[key] : undefined;
	}

	module.exports = hashGet;


/***/ },
/* 480 */
/***/ function(module, exports, __webpack_require__) {

	var nativeCreate = __webpack_require__(84);

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * Checks if a hash value for `key` exists.
	 *
	 * @private
	 * @name has
	 * @memberOf Hash
	 * @param {string} key The key of the entry to check.
	 * @returns {boolean} Returns `true` if an entry for `key` exists, else `false`.
	 */
	function hashHas(key) {
	  var data = this.__data__;
	  return nativeCreate ? data[key] !== undefined : hasOwnProperty.call(data, key);
	}

	module.exports = hashHas;


/***/ },
/* 481 */
/***/ function(module, exports, __webpack_require__) {

	var nativeCreate = __webpack_require__(84);

	/** Used to stand-in for `undefined` hash values. */
	var HASH_UNDEFINED = '__lodash_hash_undefined__';

	/**
	 * Sets the hash `key` to `value`.
	 *
	 * @private
	 * @name set
	 * @memberOf Hash
	 * @param {string} key The key of the value to set.
	 * @param {*} value The value to set.
	 * @returns {Object} Returns the hash instance.
	 */
	function hashSet(key, value) {
	  var data = this.__data__;
	  this.size += this.has(key) ? 0 : 1;
	  data[key] = (nativeCreate && value === undefined) ? HASH_UNDEFINED : value;
	  return this;
	}

	module.exports = hashSet;


/***/ },
/* 482 */
/***/ function(module, exports) {

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * Initializes an array clone.
	 *
	 * @private
	 * @param {Array} array The array to clone.
	 * @returns {Array} Returns the initialized clone.
	 */
	function initCloneArray(array) {
	  var length = array.length,
	      result = array.constructor(length);

	  // Add properties assigned by `RegExp#exec`.
	  if (length && typeof array[0] == 'string' && hasOwnProperty.call(array, 'index')) {
	    result.index = array.index;
	    result.input = array.input;
	  }
	  return result;
	}

	module.exports = initCloneArray;


/***/ },
/* 483 */
/***/ function(module, exports, __webpack_require__) {

	var cloneArrayBuffer = __webpack_require__(112),
	    cloneDataView = __webpack_require__(454),
	    cloneMap = __webpack_require__(455),
	    cloneRegExp = __webpack_require__(456),
	    cloneSet = __webpack_require__(457),
	    cloneSymbol = __webpack_require__(458),
	    cloneTypedArray = __webpack_require__(459);

	/** `Object#toString` result references. */
	var boolTag = '[object Boolean]',
	    dateTag = '[object Date]',
	    mapTag = '[object Map]',
	    numberTag = '[object Number]',
	    regexpTag = '[object RegExp]',
	    setTag = '[object Set]',
	    stringTag = '[object String]',
	    symbolTag = '[object Symbol]';

	var arrayBufferTag = '[object ArrayBuffer]',
	    dataViewTag = '[object DataView]',
	    float32Tag = '[object Float32Array]',
	    float64Tag = '[object Float64Array]',
	    int8Tag = '[object Int8Array]',
	    int16Tag = '[object Int16Array]',
	    int32Tag = '[object Int32Array]',
	    uint8Tag = '[object Uint8Array]',
	    uint8ClampedTag = '[object Uint8ClampedArray]',
	    uint16Tag = '[object Uint16Array]',
	    uint32Tag = '[object Uint32Array]';

	/**
	 * Initializes an object clone based on its `toStringTag`.
	 *
	 * **Note:** This function only supports cloning values with tags of
	 * `Boolean`, `Date`, `Error`, `Number`, `RegExp`, or `String`.
	 *
	 * @private
	 * @param {Object} object The object to clone.
	 * @param {string} tag The `toStringTag` of the object to clone.
	 * @param {Function} cloneFunc The function to clone values.
	 * @param {boolean} [isDeep] Specify a deep clone.
	 * @returns {Object} Returns the initialized clone.
	 */
	function initCloneByTag(object, tag, cloneFunc, isDeep) {
	  var Ctor = object.constructor;
	  switch (tag) {
	    case arrayBufferTag:
	      return cloneArrayBuffer(object);

	    case boolTag:
	    case dateTag:
	      return new Ctor(+object);

	    case dataViewTag:
	      return cloneDataView(object, isDeep);

	    case float32Tag: case float64Tag:
	    case int8Tag: case int16Tag: case int32Tag:
	    case uint8Tag: case uint8ClampedTag: case uint16Tag: case uint32Tag:
	      return cloneTypedArray(object, isDeep);

	    case mapTag:
	      return cloneMap(object, isDeep, cloneFunc);

	    case numberTag:
	    case stringTag:
	      return new Ctor(object);

	    case regexpTag:
	      return cloneRegExp(object);

	    case setTag:
	      return cloneSet(object, isDeep, cloneFunc);

	    case symbolTag:
	      return cloneSymbol(object);
	  }
	}

	module.exports = initCloneByTag;


/***/ },
/* 484 */
/***/ function(module, exports, __webpack_require__) {

	var baseCreate = __webpack_require__(422),
	    getPrototype = __webpack_require__(196),
	    isPrototype = __webpack_require__(57);

	/**
	 * Initializes an object clone.
	 *
	 * @private
	 * @param {Object} object The object to clone.
	 * @returns {Object} Returns the initialized clone.
	 */
	function initCloneObject(object) {
	  return (typeof object.constructor == 'function' && !isPrototype(object))
	    ? baseCreate(getPrototype(object))
	    : {};
	}

	module.exports = initCloneObject;


/***/ },
/* 485 */
/***/ function(module, exports, __webpack_require__) {

	var Symbol = __webpack_require__(43),
	    isArguments = __webpack_require__(86),
	    isArray = __webpack_require__(12);

	/** Built-in value references. */
	var spreadableSymbol = Symbol ? Symbol.isConcatSpreadable : undefined;

	/**
	 * Checks if `value` is a flattenable `arguments` object or array.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is flattenable, else `false`.
	 */
	function isFlattenable(value) {
	  return isArray(value) || isArguments(value) ||
	    !!(spreadableSymbol && value && value[spreadableSymbol]);
	}

	module.exports = isFlattenable;


/***/ },
/* 486 */
/***/ function(module, exports) {

	/**
	 * Checks if `value` is suitable for use as unique object key.
	 *
	 * @private
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is suitable, else `false`.
	 */
	function isKeyable(value) {
	  var type = typeof value;
	  return (type == 'string' || type == 'number' || type == 'symbol' || type == 'boolean')
	    ? (value !== '__proto__')
	    : (value === null);
	}

	module.exports = isKeyable;


/***/ },
/* 487 */
/***/ function(module, exports, __webpack_require__) {

	var coreJsData = __webpack_require__(464);

	/** Used to detect methods masquerading as native. */
	var maskSrcKey = (function() {
	  var uid = /[^.]+$/.exec(coreJsData && coreJsData.keys && coreJsData.keys.IE_PROTO || '');
	  return uid ? ('Symbol(src)_1.' + uid) : '';
	}());

	/**
	 * Checks if `func` has its source masked.
	 *
	 * @private
	 * @param {Function} func The function to check.
	 * @returns {boolean} Returns `true` if `func` is masked, else `false`.
	 */
	function isMasked(func) {
	  return !!maskSrcKey && (maskSrcKey in func);
	}

	module.exports = isMasked;


/***/ },
/* 488 */
/***/ function(module, exports) {

	/**
	 * Removes all key-value entries from the list cache.
	 *
	 * @private
	 * @name clear
	 * @memberOf ListCache
	 */
	function listCacheClear() {
	  this.__data__ = [];
	  this.size = 0;
	}

	module.exports = listCacheClear;


/***/ },
/* 489 */
/***/ function(module, exports, __webpack_require__) {

	var assocIndexOf = __webpack_require__(78);

	/** Used for built-in method references. */
	var arrayProto = Array.prototype;

	/** Built-in value references. */
	var splice = arrayProto.splice;

	/**
	 * Removes `key` and its value from the list cache.
	 *
	 * @private
	 * @name delete
	 * @memberOf ListCache
	 * @param {string} key The key of the value to remove.
	 * @returns {boolean} Returns `true` if the entry was removed, else `false`.
	 */
	function listCacheDelete(key) {
	  var data = this.__data__,
	      index = assocIndexOf(data, key);

	  if (index < 0) {
	    return false;
	  }
	  var lastIndex = data.length - 1;
	  if (index == lastIndex) {
	    data.pop();
	  } else {
	    splice.call(data, index, 1);
	  }
	  --this.size;
	  return true;
	}

	module.exports = listCacheDelete;


/***/ },
/* 490 */
/***/ function(module, exports, __webpack_require__) {

	var assocIndexOf = __webpack_require__(78);

	/**
	 * Gets the list cache value for `key`.
	 *
	 * @private
	 * @name get
	 * @memberOf ListCache
	 * @param {string} key The key of the value to get.
	 * @returns {*} Returns the entry value.
	 */
	function listCacheGet(key) {
	  var data = this.__data__,
	      index = assocIndexOf(data, key);

	  return index < 0 ? undefined : data[index][1];
	}

	module.exports = listCacheGet;


/***/ },
/* 491 */
/***/ function(module, exports, __webpack_require__) {

	var assocIndexOf = __webpack_require__(78);

	/**
	 * Checks if a list cache value for `key` exists.
	 *
	 * @private
	 * @name has
	 * @memberOf ListCache
	 * @param {string} key The key of the entry to check.
	 * @returns {boolean} Returns `true` if an entry for `key` exists, else `false`.
	 */
	function listCacheHas(key) {
	  return assocIndexOf(this.__data__, key) > -1;
	}

	module.exports = listCacheHas;


/***/ },
/* 492 */
/***/ function(module, exports, __webpack_require__) {

	var assocIndexOf = __webpack_require__(78);

	/**
	 * Sets the list cache `key` to `value`.
	 *
	 * @private
	 * @name set
	 * @memberOf ListCache
	 * @param {string} key The key of the value to set.
	 * @param {*} value The value to set.
	 * @returns {Object} Returns the list cache instance.
	 */
	function listCacheSet(key, value) {
	  var data = this.__data__,
	      index = assocIndexOf(data, key);

	  if (index < 0) {
	    ++this.size;
	    data.push([key, value]);
	  } else {
	    data[index][1] = value;
	  }
	  return this;
	}

	module.exports = listCacheSet;


/***/ },
/* 493 */
/***/ function(module, exports, __webpack_require__) {

	var Hash = __webpack_require__(411),
	    ListCache = __webpack_require__(75),
	    Map = __webpack_require__(105);

	/**
	 * Removes all key-value entries from the map.
	 *
	 * @private
	 * @name clear
	 * @memberOf MapCache
	 */
	function mapCacheClear() {
	  this.size = 0;
	  this.__data__ = {
	    'hash': new Hash,
	    'map': new (Map || ListCache),
	    'string': new Hash
	  };
	}

	module.exports = mapCacheClear;


/***/ },
/* 494 */
/***/ function(module, exports, __webpack_require__) {

	var getMapData = __webpack_require__(82);

	/**
	 * Removes `key` and its value from the map.
	 *
	 * @private
	 * @name delete
	 * @memberOf MapCache
	 * @param {string} key The key of the value to remove.
	 * @returns {boolean} Returns `true` if the entry was removed, else `false`.
	 */
	function mapCacheDelete(key) {
	  var result = getMapData(this, key)['delete'](key);
	  this.size -= result ? 1 : 0;
	  return result;
	}

	module.exports = mapCacheDelete;


/***/ },
/* 495 */
/***/ function(module, exports, __webpack_require__) {

	var getMapData = __webpack_require__(82);

	/**
	 * Gets the map value for `key`.
	 *
	 * @private
	 * @name get
	 * @memberOf MapCache
	 * @param {string} key The key of the value to get.
	 * @returns {*} Returns the entry value.
	 */
	function mapCacheGet(key) {
	  return getMapData(this, key).get(key);
	}

	module.exports = mapCacheGet;


/***/ },
/* 496 */
/***/ function(module, exports, __webpack_require__) {

	var getMapData = __webpack_require__(82);

	/**
	 * Checks if a map value for `key` exists.
	 *
	 * @private
	 * @name has
	 * @memberOf MapCache
	 * @param {string} key The key of the entry to check.
	 * @returns {boolean} Returns `true` if an entry for `key` exists, else `false`.
	 */
	function mapCacheHas(key) {
	  return getMapData(this, key).has(key);
	}

	module.exports = mapCacheHas;


/***/ },
/* 497 */
/***/ function(module, exports, __webpack_require__) {

	var getMapData = __webpack_require__(82);

	/**
	 * Sets the map `key` to `value`.
	 *
	 * @private
	 * @name set
	 * @memberOf MapCache
	 * @param {string} key The key of the value to set.
	 * @param {*} value The value to set.
	 * @returns {Object} Returns the map cache instance.
	 */
	function mapCacheSet(key, value) {
	  var data = getMapData(this, key),
	      size = data.size;

	  data.set(key, value);
	  this.size += data.size == size ? 0 : 1;
	  return this;
	}

	module.exports = mapCacheSet;


/***/ },
/* 498 */
/***/ function(module, exports, __webpack_require__) {

	var memoize = __webpack_require__(526);

	/** Used as the maximum memoize cache size. */
	var MAX_MEMOIZE_SIZE = 500;

	/**
	 * A specialized version of `_.memoize` which clears the memoized function's
	 * cache when it exceeds `MAX_MEMOIZE_SIZE`.
	 *
	 * @private
	 * @param {Function} func The function to have its output memoized.
	 * @returns {Function} Returns the new memoized function.
	 */
	function memoizeCapped(func) {
	  var result = memoize(func, function(key) {
	    if (cache.size === MAX_MEMOIZE_SIZE) {
	      cache.clear();
	    }
	    return key;
	  });

	  var cache = result.cache;
	  return result;
	}

	module.exports = memoizeCapped;


/***/ },
/* 499 */
/***/ function(module, exports, __webpack_require__) {

	var overArg = __webpack_require__(118);

	/* Built-in method references for those with the same name as other `lodash` methods. */
	var nativeKeys = overArg(Object.keys, Object);

	module.exports = nativeKeys;


/***/ },
/* 500 */
/***/ function(module, exports) {

	/**
	 * This function is like
	 * [`Object.keys`](http://ecma-international.org/ecma-262/7.0/#sec-object.keys)
	 * except that it includes inherited enumerable properties.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of property names.
	 */
	function nativeKeysIn(object) {
	  var result = [];
	  if (object != null) {
	    for (var key in Object(object)) {
	      result.push(key);
	    }
	  }
	  return result;
	}

	module.exports = nativeKeysIn;


/***/ },
/* 501 */
/***/ function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(module) {var freeGlobal = __webpack_require__(194);

	/** Detect free variable `exports`. */
	var freeExports = typeof exports == 'object' && exports && !exports.nodeType && exports;

	/** Detect free variable `module`. */
	var freeModule = freeExports && typeof module == 'object' && module && !module.nodeType && module;

	/** Detect the popular CommonJS extension `module.exports`. */
	var moduleExports = freeModule && freeModule.exports === freeExports;

	/** Detect free variable `process` from Node.js. */
	var freeProcess = moduleExports && freeGlobal.process;

	/** Used to access faster Node.js helpers. */
	var nodeUtil = (function() {
	  try {
	    return freeProcess && freeProcess.binding && freeProcess.binding('util');
	  } catch (e) {}
	}());

	module.exports = nodeUtil;

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(68)(module)))

/***/ },
/* 502 */
/***/ function(module, exports) {

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/**
	 * Used to resolve the
	 * [`toStringTag`](http://ecma-international.org/ecma-262/7.0/#sec-object.prototype.tostring)
	 * of values.
	 */
	var nativeObjectToString = objectProto.toString;

	/**
	 * Converts `value` to a string using `Object.prototype.toString`.
	 *
	 * @private
	 * @param {*} value The value to convert.
	 * @returns {string} Returns the converted string.
	 */
	function objectToString(value) {
	  return nativeObjectToString.call(value);
	}

	module.exports = objectToString;


/***/ },
/* 503 */
/***/ function(module, exports, __webpack_require__) {

	var baseGet = __webpack_require__(79),
	    baseSlice = __webpack_require__(186);

	/**
	 * Gets the parent value at `path` of `object`.
	 *
	 * @private
	 * @param {Object} object The object to query.
	 * @param {Array} path The path to get the parent value of.
	 * @returns {*} Returns the parent value.
	 */
	function parent(object, path) {
	  return path.length < 2 ? object : baseGet(object, baseSlice(path, 0, -1));
	}

	module.exports = parent;


/***/ },
/* 504 */
/***/ function(module, exports) {

	/** Used to stand-in for `undefined` hash values. */
	var HASH_UNDEFINED = '__lodash_hash_undefined__';

	/**
	 * Adds `value` to the array cache.
	 *
	 * @private
	 * @name add
	 * @memberOf SetCache
	 * @alias push
	 * @param {*} value The value to cache.
	 * @returns {Object} Returns the cache instance.
	 */
	function setCacheAdd(value) {
	  this.__data__.set(value, HASH_UNDEFINED);
	  return this;
	}

	module.exports = setCacheAdd;


/***/ },
/* 505 */
/***/ function(module, exports) {

	/**
	 * Checks if `value` is in the array cache.
	 *
	 * @private
	 * @name has
	 * @memberOf SetCache
	 * @param {*} value The value to search for.
	 * @returns {number} Returns `true` if `value` is found, else `false`.
	 */
	function setCacheHas(value) {
	  return this.__data__.has(value);
	}

	module.exports = setCacheHas;


/***/ },
/* 506 */
/***/ function(module, exports) {

	/** Used to detect hot functions by number of calls within a span of milliseconds. */
	var HOT_COUNT = 800,
	    HOT_SPAN = 16;

	/* Built-in method references for those with the same name as other `lodash` methods. */
	var nativeNow = Date.now;

	/**
	 * Creates a function that'll short out and invoke `identity` instead
	 * of `func` when it's called `HOT_COUNT` or more times in `HOT_SPAN`
	 * milliseconds.
	 *
	 * @private
	 * @param {Function} func The function to restrict.
	 * @returns {Function} Returns the new shortable function.
	 */
	function shortOut(func) {
	  var count = 0,
	      lastCalled = 0;

	  return function() {
	    var stamp = nativeNow(),
	        remaining = HOT_SPAN - (stamp - lastCalled);

	    lastCalled = stamp;
	    if (remaining > 0) {
	      if (++count >= HOT_COUNT) {
	        return arguments[0];
	      }
	    } else {
	      count = 0;
	    }
	    return func.apply(undefined, arguments);
	  };
	}

	module.exports = shortOut;


/***/ },
/* 507 */
/***/ function(module, exports, __webpack_require__) {

	var ListCache = __webpack_require__(75);

	/**
	 * Removes all key-value entries from the stack.
	 *
	 * @private
	 * @name clear
	 * @memberOf Stack
	 */
	function stackClear() {
	  this.__data__ = new ListCache;
	  this.size = 0;
	}

	module.exports = stackClear;


/***/ },
/* 508 */
/***/ function(module, exports) {

	/**
	 * Removes `key` and its value from the stack.
	 *
	 * @private
	 * @name delete
	 * @memberOf Stack
	 * @param {string} key The key of the value to remove.
	 * @returns {boolean} Returns `true` if the entry was removed, else `false`.
	 */
	function stackDelete(key) {
	  var data = this.__data__,
	      result = data['delete'](key);

	  this.size = data.size;
	  return result;
	}

	module.exports = stackDelete;


/***/ },
/* 509 */
/***/ function(module, exports) {

	/**
	 * Gets the stack value for `key`.
	 *
	 * @private
	 * @name get
	 * @memberOf Stack
	 * @param {string} key The key of the value to get.
	 * @returns {*} Returns the entry value.
	 */
	function stackGet(key) {
	  return this.__data__.get(key);
	}

	module.exports = stackGet;


/***/ },
/* 510 */
/***/ function(module, exports) {

	/**
	 * Checks if a stack value for `key` exists.
	 *
	 * @private
	 * @name has
	 * @memberOf Stack
	 * @param {string} key The key of the entry to check.
	 * @returns {boolean} Returns `true` if an entry for `key` exists, else `false`.
	 */
	function stackHas(key) {
	  return this.__data__.has(key);
	}

	module.exports = stackHas;


/***/ },
/* 511 */
/***/ function(module, exports, __webpack_require__) {

	var ListCache = __webpack_require__(75),
	    Map = __webpack_require__(105),
	    MapCache = __webpack_require__(106);

	/** Used as the size to enable large array optimizations. */
	var LARGE_ARRAY_SIZE = 200;

	/**
	 * Sets the stack `key` to `value`.
	 *
	 * @private
	 * @name set
	 * @memberOf Stack
	 * @param {string} key The key of the value to set.
	 * @param {*} value The value to set.
	 * @returns {Object} Returns the stack cache instance.
	 */
	function stackSet(key, value) {
	  var data = this.__data__;
	  if (data instanceof ListCache) {
	    var pairs = data.__data__;
	    if (!Map || (pairs.length < LARGE_ARRAY_SIZE - 1)) {
	      pairs.push([key, value]);
	      this.size = ++data.size;
	      return this;
	    }
	    data = this.__data__ = new MapCache(pairs);
	  }
	  data.set(key, value);
	  this.size = data.size;
	  return this;
	}

	module.exports = stackSet;


/***/ },
/* 512 */
/***/ function(module, exports) {

	/**
	 * A specialized version of `_.indexOf` which performs strict equality
	 * comparisons of values, i.e. `===`.
	 *
	 * @private
	 * @param {Array} array The array to inspect.
	 * @param {*} value The value to search for.
	 * @param {number} fromIndex The index to search from.
	 * @returns {number} Returns the index of the matched value, else `-1`.
	 */
	function strictIndexOf(array, value, fromIndex) {
	  var index = fromIndex - 1,
	      length = array.length;

	  while (++index < length) {
	    if (array[index] === value) {
	      return index;
	    }
	  }
	  return -1;
	}

	module.exports = strictIndexOf;


/***/ },
/* 513 */
/***/ function(module, exports, __webpack_require__) {

	var asciiToArray = __webpack_require__(418),
	    hasUnicode = __webpack_require__(476),
	    unicodeToArray = __webpack_require__(515);

	/**
	 * Converts `string` to an array.
	 *
	 * @private
	 * @param {string} string The string to convert.
	 * @returns {Array} Returns the converted array.
	 */
	function stringToArray(string) {
	  return hasUnicode(string)
	    ? unicodeToArray(string)
	    : asciiToArray(string);
	}

	module.exports = stringToArray;


/***/ },
/* 514 */
/***/ function(module, exports, __webpack_require__) {

	var memoizeCapped = __webpack_require__(498);

	/** Used to match property names within property paths. */
	var reLeadingDot = /^\./,
	    rePropName = /[^.[\]]+|\[(?:(-?\d+(?:\.\d+)?)|(["'])((?:(?!\2)[^\\]|\\.)*?)\2)\]|(?=(?:\.|\[\])(?:\.|\[\]|$))/g;

	/** Used to match backslashes in property paths. */
	var reEscapeChar = /\\(\\)?/g;

	/**
	 * Converts `string` to a property path array.
	 *
	 * @private
	 * @param {string} string The string to convert.
	 * @returns {Array} Returns the property path array.
	 */
	var stringToPath = memoizeCapped(function(string) {
	  var result = [];
	  if (reLeadingDot.test(string)) {
	    result.push('');
	  }
	  string.replace(rePropName, function(match, number, quote, string) {
	    result.push(quote ? string.replace(reEscapeChar, '$1') : (number || match));
	  });
	  return result;
	});

	module.exports = stringToPath;


/***/ },
/* 515 */
/***/ function(module, exports) {

	/** Used to compose unicode character classes. */
	var rsAstralRange = '\\ud800-\\udfff',
	    rsComboMarksRange = '\\u0300-\\u036f',
	    reComboHalfMarksRange = '\\ufe20-\\ufe2f',
	    rsComboSymbolsRange = '\\u20d0-\\u20ff',
	    rsComboRange = rsComboMarksRange + reComboHalfMarksRange + rsComboSymbolsRange,
	    rsVarRange = '\\ufe0e\\ufe0f';

	/** Used to compose unicode capture groups. */
	var rsAstral = '[' + rsAstralRange + ']',
	    rsCombo = '[' + rsComboRange + ']',
	    rsFitz = '\\ud83c[\\udffb-\\udfff]',
	    rsModifier = '(?:' + rsCombo + '|' + rsFitz + ')',
	    rsNonAstral = '[^' + rsAstralRange + ']',
	    rsRegional = '(?:\\ud83c[\\udde6-\\uddff]){2}',
	    rsSurrPair = '[\\ud800-\\udbff][\\udc00-\\udfff]',
	    rsZWJ = '\\u200d';

	/** Used to compose unicode regexes. */
	var reOptMod = rsModifier + '?',
	    rsOptVar = '[' + rsVarRange + ']?',
	    rsOptJoin = '(?:' + rsZWJ + '(?:' + [rsNonAstral, rsRegional, rsSurrPair].join('|') + ')' + rsOptVar + reOptMod + ')*',
	    rsSeq = rsOptVar + reOptMod + rsOptJoin,
	    rsSymbol = '(?:' + [rsNonAstral + rsCombo + '?', rsCombo, rsRegional, rsSurrPair, rsAstral].join('|') + ')';

	/** Used to match [string symbols](https://mathiasbynens.be/notes/javascript-unicode). */
	var reUnicode = RegExp(rsFitz + '(?=' + rsFitz + ')|' + rsSymbol + rsSeq, 'g');

	/**
	 * Converts a Unicode `string` to an array.
	 *
	 * @private
	 * @param {string} string The string to convert.
	 * @returns {Array} Returns the converted array.
	 */
	function unicodeToArray(string) {
	  return string.match(reUnicode) || [];
	}

	module.exports = unicodeToArray;


/***/ },
/* 516 */
/***/ function(module, exports, __webpack_require__) {

	var assignValue = __webpack_require__(77),
	    copyObject = __webpack_require__(33),
	    createAssigner = __webpack_require__(190),
	    isArrayLike = __webpack_require__(25),
	    isPrototype = __webpack_require__(57),
	    keys = __webpack_require__(23);

	/** Used for built-in method references. */
	var objectProto = Object.prototype;

	/** Used to check objects for own properties. */
	var hasOwnProperty = objectProto.hasOwnProperty;

	/**
	 * Assigns own enumerable string keyed properties of source objects to the
	 * destination object. Source objects are applied from left to right.
	 * Subsequent sources overwrite property assignments of previous sources.
	 *
	 * **Note:** This method mutates `object` and is loosely based on
	 * [`Object.assign`](https://mdn.io/Object/assign).
	 *
	 * @static
	 * @memberOf _
	 * @since 0.10.0
	 * @category Object
	 * @param {Object} object The destination object.
	 * @param {...Object} [sources] The source objects.
	 * @returns {Object} Returns `object`.
	 * @see _.assignIn
	 * @example
	 *
	 * function Foo() {
	 *   this.a = 1;
	 * }
	 *
	 * function Bar() {
	 *   this.c = 3;
	 * }
	 *
	 * Foo.prototype.b = 2;
	 * Bar.prototype.d = 4;
	 *
	 * _.assign({ 'a': 0 }, new Foo, new Bar);
	 * // => { 'a': 1, 'c': 3 }
	 */
	var assign = createAssigner(function(object, source) {
	  if (isPrototype(source) || isArrayLike(source)) {
	    copyObject(source, keys(source), object);
	    return;
	  }
	  for (var key in source) {
	    if (hasOwnProperty.call(source, key)) {
	      assignValue(object, key, source[key]);
	    }
	  }
	});

	module.exports = assign;


/***/ },
/* 517 */
/***/ function(module, exports, __webpack_require__) {

	var copyObject = __webpack_require__(33),
	    createAssigner = __webpack_require__(190),
	    keysIn = __webpack_require__(123);

	/**
	 * This method is like `_.assign` except that it iterates over own and
	 * inherited source properties.
	 *
	 * **Note:** This method mutates `object`.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @alias extend
	 * @category Object
	 * @param {Object} object The destination object.
	 * @param {...Object} [sources] The source objects.
	 * @returns {Object} Returns `object`.
	 * @see _.assign
	 * @example
	 *
	 * function Foo() {
	 *   this.a = 1;
	 * }
	 *
	 * function Bar() {
	 *   this.c = 3;
	 * }
	 *
	 * Foo.prototype.b = 2;
	 * Bar.prototype.d = 4;
	 *
	 * _.assignIn({ 'a': 0 }, new Foo, new Bar);
	 * // => { 'a': 1, 'b': 2, 'c': 3, 'd': 4 }
	 */
	var assignIn = createAssigner(function(object, source) {
	  copyObject(source, keysIn(source), object);
	});

	module.exports = assignIn;


/***/ },
/* 518 */
/***/ function(module, exports, __webpack_require__) {

	var baseClone = __webpack_require__(110);

	/** Used to compose bitmasks for cloning. */
	var CLONE_SYMBOLS_FLAG = 4;

	/**
	 * Creates a shallow clone of `value`.
	 *
	 * **Note:** This method is loosely based on the
	 * [structured clone algorithm](https://mdn.io/Structured_clone_algorithm)
	 * and supports cloning arrays, array buffers, booleans, date objects, maps,
	 * numbers, `Object` objects, regexes, sets, strings, symbols, and typed
	 * arrays. The own enumerable properties of `arguments` objects are cloned
	 * as plain objects. An empty object is returned for uncloneable values such
	 * as error objects, functions, DOM nodes, and WeakMaps.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Lang
	 * @param {*} value The value to clone.
	 * @returns {*} Returns the cloned value.
	 * @see _.cloneDeep
	 * @example
	 *
	 * var objects = [{ 'a': 1 }, { 'b': 2 }];
	 *
	 * var shallow = _.clone(objects);
	 * console.log(shallow[0] === objects[0]);
	 * // => true
	 */
	function clone(value) {
	  return baseClone(value, CLONE_SYMBOLS_FLAG);
	}

	module.exports = clone;


/***/ },
/* 519 */
/***/ function(module, exports) {

	/**
	 * Creates a function that returns `value`.
	 *
	 * @static
	 * @memberOf _
	 * @since 2.4.0
	 * @category Util
	 * @param {*} value The value to return from the new function.
	 * @returns {Function} Returns the new constant function.
	 * @example
	 *
	 * var objects = _.times(2, _.constant({ 'a': 1 }));
	 *
	 * console.log(objects);
	 * // => [{ 'a': 1 }, { 'a': 1 }]
	 *
	 * console.log(objects[0] === objects[1]);
	 * // => true
	 */
	function constant(value) {
	  return function() {
	    return value;
	  };
	}

	module.exports = constant;


/***/ },
/* 520 */
/***/ function(module, exports, __webpack_require__) {

	var baseDifference = __webpack_require__(179),
	    baseFlatten = __webpack_require__(55),
	    baseRest = __webpack_require__(56),
	    isArrayLikeObject = __webpack_require__(209);

	/**
	 * Creates an array of `array` values not included in the other given arrays
	 * using [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
	 * for equality comparisons. The order and references of result values are
	 * determined by the first array.
	 *
	 * **Note:** Unlike `_.pullAll`, this method returns a new array.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Array
	 * @param {Array} array The array to inspect.
	 * @param {...Array} [values] The values to exclude.
	 * @returns {Array} Returns the new array of filtered values.
	 * @see _.without, _.xor
	 * @example
	 *
	 * _.difference([2, 1], [2, 3]);
	 * // => [1]
	 */
	var difference = baseRest(function(array, values) {
	  return isArrayLikeObject(array)
	    ? baseDifference(array, baseFlatten(values, 1, isArrayLikeObject, true))
	    : [];
	});

	module.exports = difference;


/***/ },
/* 521 */
/***/ function(module, exports, __webpack_require__) {

	var baseFindIndex = __webpack_require__(180),
	    baseIteratee = __webpack_require__(32),
	    toInteger = __webpack_require__(127);

	/* Built-in method references for those with the same name as other `lodash` methods. */
	var nativeMax = Math.max;

	/**
	 * This method is like `_.find` except that it returns the index of the first
	 * element `predicate` returns truthy for instead of the element itself.
	 *
	 * @static
	 * @memberOf _
	 * @since 1.1.0
	 * @category Array
	 * @param {Array} array The array to inspect.
	 * @param {Function} [predicate=_.identity] The function invoked per iteration.
	 * @param {number} [fromIndex=0] The index to search from.
	 * @returns {number} Returns the index of the found element, else `-1`.
	 * @example
	 *
	 * var users = [
	 *   { 'user': 'barney',  'active': false },
	 *   { 'user': 'fred',    'active': false },
	 *   { 'user': 'pebbles', 'active': true }
	 * ];
	 *
	 * _.findIndex(users, function(o) { return o.user == 'barney'; });
	 * // => 0
	 *
	 * // The `_.matches` iteratee shorthand.
	 * _.findIndex(users, { 'user': 'fred', 'active': false });
	 * // => 1
	 *
	 * // The `_.matchesProperty` iteratee shorthand.
	 * _.findIndex(users, ['active', false]);
	 * // => 0
	 *
	 * // The `_.property` iteratee shorthand.
	 * _.findIndex(users, 'active');
	 * // => 2
	 */
	function findIndex(array, predicate, fromIndex) {
	  var length = array == null ? 0 : array.length;
	  if (!length) {
	    return -1;
	  }
	  var index = fromIndex == null ? 0 : toInteger(fromIndex);
	  if (index < 0) {
	    index = nativeMax(length + index, 0);
	  }
	  return baseFindIndex(array, baseIteratee(predicate, 3), index);
	}

	module.exports = findIndex;


/***/ },
/* 522 */
/***/ function(module, exports, __webpack_require__) {

	var baseFlatten = __webpack_require__(55);

	/**
	 * Flattens `array` a single level deep.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Array
	 * @param {Array} array The array to flatten.
	 * @returns {Array} Returns the new flattened array.
	 * @example
	 *
	 * _.flatten([1, [2, [3, [4]], 5]]);
	 * // => [1, 2, [3, [4]], 5]
	 */
	function flatten(array) {
	  var length = array == null ? 0 : array.length;
	  return length ? baseFlatten(array, 1) : [];
	}

	module.exports = flatten;


/***/ },
/* 523 */
/***/ function(module, exports, __webpack_require__) {

	var baseGet = __webpack_require__(79);

	/**
	 * Gets the value at `path` of `object`. If the resolved value is
	 * `undefined`, the `defaultValue` is returned in its place.
	 *
	 * @static
	 * @memberOf _
	 * @since 3.7.0
	 * @category Object
	 * @param {Object} object The object to query.
	 * @param {Array|string} path The path of the property to get.
	 * @param {*} [defaultValue] The value returned for `undefined` resolved values.
	 * @returns {*} Returns the resolved value.
	 * @example
	 *
	 * var object = { 'a': [{ 'b': { 'c': 3 } }] };
	 *
	 * _.get(object, 'a[0].b.c');
	 * // => 3
	 *
	 * _.get(object, ['a', '0', 'b', 'c']);
	 * // => 3
	 *
	 * _.get(object, 'a.b.c', 'default');
	 * // => 'default'
	 */
	function get(object, path, defaultValue) {
	  var result = object == null ? undefined : baseGet(object, path);
	  return result === undefined ? defaultValue : result;
	}

	module.exports = get;


/***/ },
/* 524 */
/***/ function(module, exports, __webpack_require__) {

	var toInteger = __webpack_require__(127);

	/**
	 * Checks if `value` is an integer.
	 *
	 * **Note:** This method is based on
	 * [`Number.isInteger`](https://mdn.io/Number/isInteger).
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to check.
	 * @returns {boolean} Returns `true` if `value` is an integer, else `false`.
	 * @example
	 *
	 * _.isInteger(3);
	 * // => true
	 *
	 * _.isInteger(Number.MIN_VALUE);
	 * // => false
	 *
	 * _.isInteger(Infinity);
	 * // => false
	 *
	 * _.isInteger('3');
	 * // => false
	 */
	function isInteger(value) {
	  return typeof value == 'number' && value == toInteger(value);
	}

	module.exports = isInteger;


/***/ },
/* 525 */
/***/ function(module, exports) {

	/**
	 * Gets the last element of `array`.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Array
	 * @param {Array} array The array to query.
	 * @returns {*} Returns the last element of `array`.
	 * @example
	 *
	 * _.last([1, 2, 3]);
	 * // => 3
	 */
	function last(array) {
	  var length = array == null ? 0 : array.length;
	  return length ? array[length - 1] : undefined;
	}

	module.exports = last;


/***/ },
/* 526 */
/***/ function(module, exports, __webpack_require__) {

	var MapCache = __webpack_require__(106);

	/** Error message constants. */
	var FUNC_ERROR_TEXT = 'Expected a function';

	/**
	 * Creates a function that memoizes the result of `func`. If `resolver` is
	 * provided, it determines the cache key for storing the result based on the
	 * arguments provided to the memoized function. By default, the first argument
	 * provided to the memoized function is used as the map cache key. The `func`
	 * is invoked with the `this` binding of the memoized function.
	 *
	 * **Note:** The cache is exposed as the `cache` property on the memoized
	 * function. Its creation may be customized by replacing the `_.memoize.Cache`
	 * constructor with one whose instances implement the
	 * [`Map`](http://ecma-international.org/ecma-262/7.0/#sec-properties-of-the-map-prototype-object)
	 * method interface of `clear`, `delete`, `get`, `has`, and `set`.
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Function
	 * @param {Function} func The function to have its output memoized.
	 * @param {Function} [resolver] The function to resolve the cache key.
	 * @returns {Function} Returns the new memoized function.
	 * @example
	 *
	 * var object = { 'a': 1, 'b': 2 };
	 * var other = { 'c': 3, 'd': 4 };
	 *
	 * var values = _.memoize(_.values);
	 * values(object);
	 * // => [1, 2]
	 *
	 * values(other);
	 * // => [3, 4]
	 *
	 * object.a = 2;
	 * values(object);
	 * // => [1, 2]
	 *
	 * // Modify the result cache.
	 * values.cache.set(object, ['a', 'b']);
	 * values(object);
	 * // => ['a', 'b']
	 *
	 * // Replace `_.memoize.Cache`.
	 * _.memoize.Cache = WeakMap;
	 */
	function memoize(func, resolver) {
	  if (typeof func != 'function' || (resolver != null && typeof resolver != 'function')) {
	    throw new TypeError(FUNC_ERROR_TEXT);
	  }
	  var memoized = function() {
	    var args = arguments,
	        key = resolver ? resolver.apply(this, args) : args[0],
	        cache = memoized.cache;

	    if (cache.has(key)) {
	      return cache.get(key);
	    }
	    var result = func.apply(this, args);
	    memoized.cache = cache.set(key, result) || cache;
	    return result;
	  };
	  memoized.cache = new (memoize.Cache || MapCache);
	  return memoized;
	}

	// Expose `MapCache`.
	memoize.Cache = MapCache;

	module.exports = memoize;


/***/ },
/* 527 */
/***/ function(module, exports) {

	/**
	 * This method returns `undefined`.
	 *
	 * @static
	 * @memberOf _
	 * @since 2.3.0
	 * @category Util
	 * @example
	 *
	 * _.times(2, _.noop);
	 * // => [undefined, undefined]
	 */
	function noop() {
	  // No operation performed.
	}

	module.exports = noop;


/***/ },
/* 528 */
/***/ function(module, exports, __webpack_require__) {

	var baseProperty = __webpack_require__(440),
	    basePropertyDeep = __webpack_require__(441),
	    isKey = __webpack_require__(117),
	    toKey = __webpack_require__(46);

	/**
	 * Creates a function that returns the value at `path` of a given object.
	 *
	 * @static
	 * @memberOf _
	 * @since 2.4.0
	 * @category Util
	 * @param {Array|string} path The path of the property to get.
	 * @returns {Function} Returns the new accessor function.
	 * @example
	 *
	 * var objects = [
	 *   { 'a': { 'b': 2 } },
	 *   { 'a': { 'b': 1 } }
	 * ];
	 *
	 * _.map(objects, _.property('a.b'));
	 * // => [2, 1]
	 *
	 * _.map(_.sortBy(objects, _.property(['a', 'b'])), 'a.b');
	 * // => [1, 2]
	 */
	function property(path) {
	  return isKey(path) ? baseProperty(toKey(path)) : basePropertyDeep(path);
	}

	module.exports = property;


/***/ },
/* 529 */
/***/ function(module, exports, __webpack_require__) {

	var baseRest = __webpack_require__(56),
	    pullAll = __webpack_require__(530);

	/**
	 * Removes all given values from `array` using
	 * [`SameValueZero`](http://ecma-international.org/ecma-262/7.0/#sec-samevaluezero)
	 * for equality comparisons.
	 *
	 * **Note:** Unlike `_.without`, this method mutates `array`. Use `_.remove`
	 * to remove elements from an array by predicate.
	 *
	 * @static
	 * @memberOf _
	 * @since 2.0.0
	 * @category Array
	 * @param {Array} array The array to modify.
	 * @param {...*} [values] The values to remove.
	 * @returns {Array} Returns `array`.
	 * @example
	 *
	 * var array = ['a', 'b', 'c', 'a', 'b', 'c'];
	 *
	 * _.pull(array, 'a', 'c');
	 * console.log(array);
	 * // => ['b', 'b']
	 */
	var pull = baseRest(pullAll);

	module.exports = pull;


/***/ },
/* 530 */
/***/ function(module, exports, __webpack_require__) {

	var basePullAll = __webpack_require__(442);

	/**
	 * This method is like `_.pull` except that it accepts an array of values to remove.
	 *
	 * **Note:** Unlike `_.difference`, this method mutates `array`.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Array
	 * @param {Array} array The array to modify.
	 * @param {Array} values The values to remove.
	 * @returns {Array} Returns `array`.
	 * @example
	 *
	 * var array = ['a', 'b', 'c', 'a', 'b', 'c'];
	 *
	 * _.pullAll(array, ['a', 'c']);
	 * console.log(array);
	 * // => ['b', 'b']
	 */
	function pullAll(array, values) {
	  return (array && array.length && values && values.length)
	    ? basePullAll(array, values)
	    : array;
	}

	module.exports = pullAll;


/***/ },
/* 531 */
/***/ function(module, exports, __webpack_require__) {

	var baseFlatten = __webpack_require__(55),
	    baseOrderBy = __webpack_require__(185),
	    baseRest = __webpack_require__(56),
	    isIterateeCall = __webpack_require__(116);

	/**
	 * Creates an array of elements, sorted in ascending order by the results of
	 * running each element in a collection thru each iteratee. This method
	 * performs a stable sort, that is, it preserves the original sort order of
	 * equal elements. The iteratees are invoked with one argument: (value).
	 *
	 * @static
	 * @memberOf _
	 * @since 0.1.0
	 * @category Collection
	 * @param {Array|Object} collection The collection to iterate over.
	 * @param {...(Function|Function[])} [iteratees=[_.identity]]
	 *  The iteratees to sort by.
	 * @returns {Array} Returns the new sorted array.
	 * @example
	 *
	 * var users = [
	 *   { 'user': 'fred',   'age': 48 },
	 *   { 'user': 'barney', 'age': 36 },
	 *   { 'user': 'fred',   'age': 40 },
	 *   { 'user': 'barney', 'age': 34 }
	 * ];
	 *
	 * _.sortBy(users, [function(o) { return o.user; }]);
	 * // => objects for [['barney', 36], ['barney', 34], ['fred', 48], ['fred', 40]]
	 *
	 * _.sortBy(users, ['user', 'age']);
	 * // => objects for [['barney', 34], ['barney', 36], ['fred', 40], ['fred', 48]]
	 */
	var sortBy = baseRest(function(collection, iteratees) {
	  if (collection == null) {
	    return [];
	  }
	  var length = iteratees.length;
	  if (length > 1 && isIterateeCall(collection, iteratees[0], iteratees[1])) {
	    iteratees = [];
	  } else if (length > 2 && isIterateeCall(iteratees[0], iteratees[1], iteratees[2])) {
	    iteratees = [iteratees[0]];
	  }
	  return baseOrderBy(collection, baseFlatten(iteratees, 1), []);
	});

	module.exports = sortBy;


/***/ },
/* 532 */
/***/ function(module, exports) {

	/**
	 * This method returns `false`.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.13.0
	 * @category Util
	 * @returns {boolean} Returns `false`.
	 * @example
	 *
	 * _.times(2, _.stubFalse);
	 * // => [false, false]
	 */
	function stubFalse() {
	  return false;
	}

	module.exports = stubFalse;


/***/ },
/* 533 */
/***/ function(module, exports, __webpack_require__) {

	var toNumber = __webpack_require__(534);

	/** Used as references for various `Number` constants. */
	var INFINITY = 1 / 0,
	    MAX_INTEGER = 1.7976931348623157e+308;

	/**
	 * Converts `value` to a finite number.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.12.0
	 * @category Lang
	 * @param {*} value The value to convert.
	 * @returns {number} Returns the converted number.
	 * @example
	 *
	 * _.toFinite(3.2);
	 * // => 3.2
	 *
	 * _.toFinite(Number.MIN_VALUE);
	 * // => 5e-324
	 *
	 * _.toFinite(Infinity);
	 * // => 1.7976931348623157e+308
	 *
	 * _.toFinite('3.2');
	 * // => 3.2
	 */
	function toFinite(value) {
	  if (!value) {
	    return value === 0 ? value : 0;
	  }
	  value = toNumber(value);
	  if (value === INFINITY || value === -INFINITY) {
	    var sign = (value < 0 ? -1 : 1);
	    return sign * MAX_INTEGER;
	  }
	  return value === value ? value : 0;
	}

	module.exports = toFinite;


/***/ },
/* 534 */
/***/ function(module, exports, __webpack_require__) {

	var isObject = __webpack_require__(20),
	    isSymbol = __webpack_require__(61);

	/** Used as references for various `Number` constants. */
	var NAN = 0 / 0;

	/** Used to match leading and trailing whitespace. */
	var reTrim = /^\s+|\s+$/g;

	/** Used to detect bad signed hexadecimal string values. */
	var reIsBadHex = /^[-+]0x[0-9a-f]+$/i;

	/** Used to detect binary string values. */
	var reIsBinary = /^0b[01]+$/i;

	/** Used to detect octal string values. */
	var reIsOctal = /^0o[0-7]+$/i;

	/** Built-in method references without a dependency on `root`. */
	var freeParseInt = parseInt;

	/**
	 * Converts `value` to a number.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Lang
	 * @param {*} value The value to process.
	 * @returns {number} Returns the number.
	 * @example
	 *
	 * _.toNumber(3.2);
	 * // => 3.2
	 *
	 * _.toNumber(Number.MIN_VALUE);
	 * // => 5e-324
	 *
	 * _.toNumber(Infinity);
	 * // => Infinity
	 *
	 * _.toNumber('3.2');
	 * // => 3.2
	 */
	function toNumber(value) {
	  if (typeof value == 'number') {
	    return value;
	  }
	  if (isSymbol(value)) {
	    return NAN;
	  }
	  if (isObject(value)) {
	    var other = typeof value.valueOf == 'function' ? value.valueOf() : value;
	    value = isObject(other) ? (other + '') : other;
	  }
	  if (typeof value != 'string') {
	    return value === 0 ? value : +value;
	  }
	  value = value.replace(reTrim, '');
	  var isBinary = reIsBinary.test(value);
	  return (isBinary || reIsOctal.test(value))
	    ? freeParseInt(value.slice(2), isBinary ? 2 : 8)
	    : (reIsBadHex.test(value) ? NAN : +value);
	}

	module.exports = toNumber;


/***/ },
/* 535 */
/***/ function(module, exports, __webpack_require__) {

	var baseToString = __webpack_require__(187),
	    castSlice = __webpack_require__(451),
	    charsEndIndex = __webpack_require__(452),
	    stringToArray = __webpack_require__(513),
	    toString = __webpack_require__(63);

	/** Used to match leading and trailing whitespace. */
	var reTrimEnd = /\s+$/;

	/**
	 * Removes trailing whitespace or specified characters from `string`.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category String
	 * @param {string} [string=''] The string to trim.
	 * @param {string} [chars=whitespace] The characters to trim.
	 * @param- {Object} [guard] Enables use as an iteratee for methods like `_.map`.
	 * @returns {string} Returns the trimmed string.
	 * @example
	 *
	 * _.trimEnd('  abc  ');
	 * // => '  abc'
	 *
	 * _.trimEnd('-_-abc-_-', '_-');
	 * // => '-_-abc'
	 */
	function trimEnd(string, chars, guard) {
	  string = toString(string);
	  if (string && (guard || chars === undefined)) {
	    return string.replace(reTrimEnd, '');
	  }
	  if (!string || !(chars = baseToString(chars))) {
	    return string;
	  }
	  var strSymbols = stringToArray(string),
	      end = charsEndIndex(strSymbols, stringToArray(chars)) + 1;

	  return castSlice(strSymbols, 0, end).join('');
	}

	module.exports = trimEnd;


/***/ },
/* 536 */
/***/ function(module, exports, __webpack_require__) {

	var baseUnset = __webpack_require__(189);

	/**
	 * Removes the property at `path` of `object`.
	 *
	 * **Note:** This method mutates `object`.
	 *
	 * @static
	 * @memberOf _
	 * @since 4.0.0
	 * @category Object
	 * @param {Object} object The object to modify.
	 * @param {Array|string} path The path of the property to unset.
	 * @returns {boolean} Returns `true` if the property is deleted, else `false`.
	 * @example
	 *
	 * var object = { 'a': [{ 'b': { 'c': 7 } }] };
	 * _.unset(object, 'a[0].b.c');
	 * // => true
	 *
	 * console.log(object);
	 * // => { 'a': [{ 'b': {} }] };
	 *
	 * _.unset(object, ['a', '0', 'b', 'c']);
	 * // => true
	 *
	 * console.log(object);
	 * // => { 'a': [{ 'b': {} }] };
	 */
	function unset(object, path) {
	  return object == null ? true : baseUnset(object, path);
	}

	module.exports = unset;


/***/ },
/* 537 */
/***/ function(module, exports, __webpack_require__) {

	var baseValues = __webpack_require__(448),
	    keys = __webpack_require__(23);

	/**
	 * Creates an array of the own enumerable string keyed property values of `object`.
	 *
	 * **Note:** Non-object values are coerced to objects.
	 *
	 * @static
	 * @since 0.1.0
	 * @memberOf _
	 * @category Object
	 * @param {Object} object The object to query.
	 * @returns {Array} Returns the array of property values.
	 * @example
	 *
	 * function Foo() {
	 *   this.a = 1;
	 *   this.b = 2;
	 * }
	 *
	 * Foo.prototype.c = 3;
	 *
	 * _.values(new Foo);
	 * // => [1, 2] (iteration order is not guaranteed)
	 *
	 * _.values('hi');
	 * // => ['h', 'i']
	 */
	function values(object) {
	  return object == null ? [] : baseValues(object, keys(object));
	}

	module.exports = values;


/***/ },
/* 538 */
/***/ function(module, exports, __webpack_require__) {

	var arrayFilter = __webpack_require__(173),
	    baseRest = __webpack_require__(56),
	    baseXor = __webpack_require__(449),
	    isArrayLikeObject = __webpack_require__(209);

	/**
	 * Creates an array of unique values that is the
	 * [symmetric difference](https://en.wikipedia.org/wiki/Symmetric_difference)
	 * of the given arrays. The order of result values is determined by the order
	 * they occur in the arrays.
	 *
	 * @static
	 * @memberOf _
	 * @since 2.4.0
	 * @category Array
	 * @param {...Array} [arrays] The arrays to inspect.
	 * @returns {Array} Returns the new array of filtered values.
	 * @see _.difference, _.without
	 * @example
	 *
	 * _.xor([2, 1], [2, 3]);
	 * // => [1, 3]
	 */
	var xor = baseRest(function(arrays) {
	  return baseXor(arrayFilter(arrays, isArrayLikeObject));
	});

	module.exports = xor;


/***/ },
/* 539 */,
/* 540 */,
/* 541 */,
/* 542 */,
/* 543 */,
/* 544 */,
/* 545 */,
/* 546 */,
/* 547 */,
/* 548 */,
/* 549 */,
/* 550 */,
/* 551 */,
/* 552 */,
/* 553 */,
/* 554 */,
/* 555 */,
/* 556 */,
/* 557 */,
/* 558 */,
/* 559 */,
/* 560 */,
/* 561 */,
/* 562 */,
/* 563 */
/***/ function(module, exports, __webpack_require__) {

	/**
	 * Copyright 2013-present, Facebook, Inc.
	 * All rights reserved.
	 *
	 * This source code is licensed under the BSD-style license found in the
	 * LICENSE file in the root directory of this source tree. An additional grant
	 * of patent rights can be found in the PATENTS file in the same directory.
	 *
	 */

	'use strict';

	var ReactDefaultInjection = __webpack_require__(328);
	var ReactServerRendering = __webpack_require__(577);
	var ReactVersion = __webpack_require__(337);

	ReactDefaultInjection.inject();

	var ReactDOMServer = {
	  renderToString: ReactServerRendering.renderToString,
	  renderToStaticMarkup: ReactServerRendering.renderToStaticMarkup,
	  version: ReactVersion
	};

	module.exports = ReactDOMServer;

/***/ },
/* 564 */,
/* 565 */,
/* 566 */,
/* 567 */,
/* 568 */,
/* 569 */,
/* 570 */,
/* 571 */,
/* 572 */,
/* 573 */,
/* 574 */,
/* 575 */,
/* 576 */
/***/ function(module, exports) {

	/**
	 * Copyright 2014-present, Facebook, Inc.
	 * All rights reserved.
	 *
	 * This source code is licensed under the BSD-style license found in the
	 * LICENSE file in the root directory of this source tree. An additional grant
	 * of patent rights can be found in the PATENTS file in the same directory.
	 *
	 */

	'use strict';

	var ReactServerBatchingStrategy = {
	  isBatchingUpdates: false,
	  batchedUpdates: function (callback) {
	    // Don't do anything here. During the server rendering we don't want to
	    // schedule any updates. We will simply ignore them.
	  }
	};

	module.exports = ReactServerBatchingStrategy;

/***/ },
/* 577 */
/***/ function(module, exports, __webpack_require__) {

	/**
	 * Copyright 2013-present, Facebook, Inc.
	 * All rights reserved.
	 *
	 * This source code is licensed under the BSD-style license found in the
	 * LICENSE file in the root directory of this source tree. An additional grant
	 * of patent rights can be found in the PATENTS file in the same directory.
	 *
	 */
	'use strict';

	var _prodInvariant = __webpack_require__(9);

	var React = __webpack_require__(41);
	var ReactDOMContainerInfo = __webpack_require__(325);
	var ReactDefaultBatchingStrategy = __webpack_require__(327);
	var ReactInstrumentation = __webpack_require__(18);
	var ReactMarkupChecksum = __webpack_require__(333);
	var ReactReconciler = __webpack_require__(40);
	var ReactServerBatchingStrategy = __webpack_require__(576);
	var ReactServerRenderingTransaction = __webpack_require__(336);
	var ReactUpdates = __webpack_require__(21);

	var emptyObject = __webpack_require__(42);
	var instantiateReactComponent = __webpack_require__(143);
	var invariant = __webpack_require__(7);

	var pendingTransactions = 0;

	/**
	 * @param {ReactElement} element
	 * @return {string} the HTML markup
	 */
	function renderToStringImpl(element, makeStaticMarkup) {
	  var transaction;
	  try {
	    ReactUpdates.injection.injectBatchingStrategy(ReactServerBatchingStrategy);

	    transaction = ReactServerRenderingTransaction.getPooled(makeStaticMarkup);

	    pendingTransactions++;

	    return transaction.perform(function () {
	      var componentInstance = instantiateReactComponent(element, true);
	      var markup = ReactReconciler.mountComponent(componentInstance, transaction, null, ReactDOMContainerInfo(), emptyObject, 0 /* parentDebugID */
	      );
	      if (false) {
	        ReactInstrumentation.debugTool.onUnmountComponent(componentInstance._debugID);
	      }
	      if (!makeStaticMarkup) {
	        markup = ReactMarkupChecksum.addChecksumToMarkup(markup);
	      }
	      return markup;
	    }, null);
	  } finally {
	    pendingTransactions--;
	    ReactServerRenderingTransaction.release(transaction);
	    // Revert to the DOM batching strategy since these two renderers
	    // currently share these stateful modules.
	    if (!pendingTransactions) {
	      ReactUpdates.injection.injectBatchingStrategy(ReactDefaultBatchingStrategy);
	    }
	  }
	}

	/**
	 * Render a ReactElement to its initial HTML. This should only be used on the
	 * server.
	 * See https://facebook.github.io/react/docs/top-level-api.html#reactdomserver.rendertostring
	 */
	function renderToString(element) {
	  !React.isValidElement(element) ?  false ? invariant(false, 'renderToString(): You must pass a valid ReactElement.') : _prodInvariant('46') : void 0;
	  return renderToStringImpl(element, false);
	}

	/**
	 * Similar to renderToString, except this doesn't create extra DOM attributes
	 * such as data-react-id that React uses internally.
	 * See https://facebook.github.io/react/docs/top-level-api.html#reactdomserver.rendertostaticmarkup
	 */
	function renderToStaticMarkup(element) {
	  !React.isValidElement(element) ?  false ? invariant(false, 'renderToStaticMarkup(): You must pass a valid ReactElement.') : _prodInvariant('47') : void 0;
	  return renderToStringImpl(element, true);
	}

	module.exports = {
	  renderToString: renderToString,
	  renderToStaticMarkup: renderToStaticMarkup
	};

/***/ },
/* 578 */,
/* 579 */,
/* 580 */,
/* 581 */,
/* 582 */,
/* 583 */,
/* 584 */,
/* 585 */,
/* 586 */,
/* 587 */,
/* 588 */,
/* 589 */,
/* 590 */,
/* 591 */,
/* 592 */,
/* 593 */,
/* 594 */,
/* 595 */,
/* 596 */,
/* 597 */,
/* 598 */,
/* 599 */,
/* 600 */,
/* 601 */,
/* 602 */,
/* 603 */,
/* 604 */,
/* 605 */,
/* 606 */,
/* 607 */,
/* 608 */,
/* 609 */,
/* 610 */,
/* 611 */,
/* 612 */,
/* 613 */,
/* 614 */,
/* 615 */,
/* 616 */,
/* 617 */
/***/ function(module, exports) {

	/* WEBPACK VAR INJECTION */(function(__webpack_amd_options__) {module.exports = __webpack_amd_options__;

	/* WEBPACK VAR INJECTION */}.call(exports, {}))

/***/ }
]);