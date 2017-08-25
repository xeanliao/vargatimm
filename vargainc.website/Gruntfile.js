'use strict';

// Hack for Ubuntu on Windows: interface enumeration fails with EINVAL, so return empty.
try {
	require('os').networkInterfaces()
} catch (e) {
	require('os').networkInterfaces = () => ({})
}

module.exports = function (grunt) {
	require('load-grunt-tasks')(grunt);
	grunt.verbose.writeln('begin task...');
	require('time-grunt')(grunt);
	var lodash = require('lodash');
	var webpack = require('webpack');
	var path = require('path');
	grunt.initConfig({
		pkg: grunt.file.readJSON('package.json'),
		app: 'app',
		dist: 'dist',
		bower: {
			style: {
				dest: '<%= app %>',
				js_dest: '<%= app %>/scripts/vendor',
				scss_dest: '<%= app %>/scss/',
				css_dest: '<%= app %>/css/',
				fonts_dest: '<%= app %>/fonts/',
				images_dest: '<%= app %>/images/',
				options: {
					keepExpandedHierarchy: false,
					expand: false,
					ignorePackages: [
						'bootstrap',
						'jquery',
						'tether',
						'SVG-Loaders'
					],
					packageSpecific: {
						'font-awesome': {
							'files': ['fonts/**', 'css/font-awesome.css']
						},
						'leaflet': {
							'files': ['dist/images/**', 'dist/leaflet.css']
						}
					}
				}
			}
		},
		sass: {
			options: {
				style: 'compact', // expanded or nested or compact or compressed
				sourcemap: 'none',
				loadPath: [
					'bower_components',
					'node_modules',
					'node_modules/sass-rem/',
					'node_modules/select2/src/scss',
					'node_modules/foundation-sites/scss',
					'node_modules/foundation-datepicker/scss',
					'node_modules/motion-ui/src/',
					'node_modules/select2/scss/',
				],
				compass: true,
				quiet: true
			},
			dist: {
				files: [{
					expand: true,
					cwd: '<%= app %>/scss',
					src: ['*.{scss,sass}'],
					dest: '<%= app %>/css',
					ext: '.css'
				}]
			}
		},
		webpack: {
			options: {
				cache: false,
				output: {
					path: '<%= app %>/js/',
					publicPath: '/',
					filename: '[name].js',
					pathinfo: false,
				},
				externals: {
					'jquery': 'jQuery',
					'js-marker-clusterer': 'MarkerClusterer'
				},
				resolve: {
					root: './',
					alias: {
						'underscore': 'lodash',
						'promise': 'bluebird'
					},
					extensions: ['', '.js', '.jsx'],
					modulesDirectories: [
						'app/scripts',
						'node_modules/foundation-datepicker/js',
						'node_modules/leaflet-dvf/dist',
						'node_modules/leaflet-ant-path/dist',
						'bower_components',
						'node_modules',
					]
				},
				module: {
					loaders: [{
						test: /\.(jsx?|es|es6)$/,
						exclude: /(node_modules|bower_components)/,
						loader: 'babel',
						query: {
							cacheDirectory: '.scripts_cache',
							plugins: ['lodash'],
							presets: ['latest', 'react']
						}
					}],
					noParse: [
						/localforage\/dist\/localforage.js/,
						/mapbox-gl/
					]
				},
				hot: false
			},
			'dist': {
				devtool: false,
				entry: {
					app: '<%= app %>/scripts/main',
					vendor: (function () {
						var pkg = grunt.file.readJSON('package.json');
						return lodash.keys(pkg.dependencies);
					})()
				},
				output: {
					filename: '[name].js',
				},
				plugins: [
					new webpack.optimize.CommonsChunkPlugin({
						name: 'vendor',
						filename: 'vendor.js'
					}),
					new webpack.optimize.OccurrenceOrderPlugin(true),
					new webpack.optimize.DedupePlugin(),
					new webpack.DefinePlugin({
						'process.env': {
							'NODE_ENV': JSON.stringify('production') // This has effect on the react lib size
						},
						DEBUG: JSON.stringify(false),
						MapboxToken: JSON.stringify("pk.eyJ1IjoiZ2hvc3R1eiIsImEiOiJjaXczc2tmc3cwMDEyMm9tb29pdDRwOXUzIn0.KPSiOO6DWTY59x1zHdvYSA"),
					})
				]
			}
		},
		clean: {
			dist: {
				src: [
					'<%= dist %>/*',
					'.tmp/**/*',
					'./*.tar.gz'
				]
			},
		},
		copy: {
			bower: {
				files: [{
					expand: true,
					cwd: 'bower_components/SVG-Loaders',
					dest: '<%= app %>/images/',
					src: [
						'**/*.svg',
					]
				}]
			},
			dist: {
				files: [{
					expand: true,
					dot: true,
					cwd: '<%= app %>',
					dest: '<%= dist %>',
					src: [
						'*.{ico,png,txt}',
						'images/{,*/}*.webp',
						'release.html',
						'fonts/{,*/}*.*'
					]
				}]
			},
			vendor: {
				files: [{
					expand: true,
					dot: true,
					cwd: './node_modules/js-marker-clusterer/src/',
					dest: '<%= app %>/js/',
					src: [
						'markerclusterer_compiled.js'
					]
				}]
			},
		},
		imagemin: {
			dist: {
				files: [{
					expand: true,
					cwd: '<%= app %>/images',
					src: '{,*/}*.{gif,jpeg,jpg,png}',
					dest: '<%= dist %>/images'
				}, {
					expand: true,
					cwd: '<%= app %>/img',
					src: '{,*/}*.{gif,jpeg,jpg,png}',
					dest: '<%= dist %>/img'
				}]
			}
		},
		svgmin: {
			dist: {
				files: [{
					expand: true,
					cwd: '<%= app %>/images',
					src: '{,*/}*.svg',
					dest: '<%= dist %>/images'
				}]
			}
		},
		uglify: {
			options: {
				mangle: true,
				preserveComments: false,
				report: 'min',
				// ASCIIOnly: true,
				compress: {
					sequences: true,
					dead_code: true,
					conditionals: true,
					booleans: true,
					unused: true,
					if_return: true,
					join_vars: true,
					drop_console: true,
				}
			}
		},
		filerev: {
			options: {
				algorithm: 'md5',
				length: 8
			},
			files: {
				src: [
					'<%= dist %>/js/{,*/}*.js',
					'<%= dist %>/css/{,*/}*.css',
					'<%= dist %>/images/{,*/}*.*',
					'<%= dist %>/img/{,*/}*.*',
					'<%= dist %>/fonts/{,*/}*.*',
					'<%= dist %>/*.{ico,png}'
				]
			}
		},
		useminPrepare: {
			html: ['<%= app %>/release.html'],
			options: {
				dest: '<%= dist %>',
			}
		},
		usemin: {
			options: {
				assetsDirs: [
					'<%= dist %>',
					'<%= dist %>/images',
					'<%= dist %>/css',
				]
			},
			html: ['<%= dist %>/{,*/}*.html'],
			css: ['<%= dist %>/css/{,*/}*.css']
		},
		rename: {
			release: {
				files: [{
					src: ['<%= dist %>/release.html'],
					dest: '<%= dist %>/index.html'
				}, ]
			}
		},
		concurrent: {
			dist: [
				'webpack',
				'sass',
				'imagemin',
				'svgmin'
			],
		},
		compress: {
			options: {
				archive: function () {
					let moment = require('moment');
					let date = moment().format('YYYYMMDDHHmmss');
					return `website.${date}.tar.gz`;
				},
				mode: 'tgz',
			},
			release: {
				files: [{
					src: ['<%= dist %>/**/*'],
					dest: './'
				}]
			}
		},
		shell: {
			options: {
				stderr: false
			},
			'release-folder': {
				command: 'mkdir -p /Volumes/RamDisk/Publish/'
			},
			'release-copy': {
				command: 'mv ./*.tar.gz /Volumes/RamDisk/Publish/'
			},
		},
		'webpack-dev-server': {
			options: {
				proxy: {//http://timm.vargainc.com/dev/api/user/login?username=admin&password=newpass
					'/api/**': {
						target: 'http://timm.vargainc.com',
						pathRewrite: {'^/api/' : '/dev/api/'}
					}
				},
				webpack: {
					cache: true,
					entry: {
						'app': [
							'./app/scripts/main',
							'webpack/hot/only-dev-server',
						],
						'style-app': ['./app/scss/app.scss', 'webpack/hot/only-dev-server'],
						'style-lib': ['./app/scss/library.scss', 'webpack/hot/only-dev-server'],
						'debug': 'webpack-dev-server/client?http://127.0.0.1:8082'
					},
					output: {
						path: '/',
						publicPath: '../',
						filename: '[name].js',
						chunkFilename: '[chunkhash].js',
					},
					externals: {
						'jquery': 'jQuery',
						'js-marker-clusterer': 'MarkerClusterer'
					},
					resolve: {
						root: './',
						alias: {
							'underscore': 'lodash',
							'promise': 'bluebird'
						},
						extensions: ['', '.js', '.jsx'],
						modulesDirectories: [
							'app/scripts',
							'node_modules/foundation-datepicker/js',
							'bower_components',
							'node_modules',
						]
					},
					module: {
						loaders: [{
							test: /\.scss$/,
							loader: (function () {
								var path = require('path');
								var loadPath = [
									'bower_components',
									'./bower_components/font-awesome/scss',
									'node_modules',
									'./node_modules/compass-mixins/lib',
									'./node_modules/sass-rem',
									'./node_modules/select2/src/scss',
									'./node_modules/foundation-sites/scss',
									'./node_modules/foundation-datepicker/scss',
									'./node_modules/motion-ui/src/',
									'node_modules/select2/scss/',
								];
								loadPath = lodash.map(loadPath, function (item) {
									return 'includePaths[]=' + path.resolve(item)
								});
								return 'style-loader!css-loader?sourceMap!sass-loader?sourceMap&' + loadPath.join('&');
							})()
						}, {
							test: /\.(jsx?|es|es6)$/,
							exclude: /(node_modules|bower_components)/,
							loaders: ['react-hot', 'babel'],
						}, {
							test: /\.(jsx?|es|es6)$/,
							exclude: /(node_modules|bower_components)/,
							loader: 'babel',
							query: {
								cacheDirectory: '.scripts_cache',
								plugins: ['lodash'],
								presets: ['es2015', 'react']
							}
						}, {
							test: /.(png|jpg|gif|svg)$/,
							loader: 'url-loader'
						}, {
							test: /\.json$/,
							loader: 'json-loader'
						}],
						noParse: [
							/localforage\/dist\/localforage.js/
						]
					},
					resolveUrlLoader: {
						root: 'app/'
					},
					plugins: [
						new webpack.HotModuleReplacementPlugin(),
						new webpack.DefinePlugin({
							DEBUG: JSON.stringify(true),
							MapboxToken: JSON.stringify("pk.eyJ1IjoiZ2hvc3R1eiIsImEiOiJjaXczc2tmc3cwMDEyMm9tb29pdDRwOXUzIn0.KPSiOO6DWTY59x1zHdvYSA"),
						}),
					],
					watch: true,
					keepalive: true,
					inline: true,
				},
				publicPath: '/',
			},
			start: {
				keepAlive: true,
				hot: true,
				webpack: {
					devtool: 'eval',
					debug: true
				},
				host: '0.0.0.0',
				port: 8082
			}
		},
	});

	grunt.registerTask('debug', ['bower', 'sass']);
	grunt.registerTask('default', ['debug']);
	grunt.registerTask('dev', ['webpack-dev-server']);
	grunt.registerTask('release', ['clean', 'bower', 'copy:bower', 'copy:dist', 'copy:vendor', 'useminPrepare', 'concurrent:dist',
		'concat', 'uglify', 'cssmin', 'filerev', 'usemin', 'rename:release', 'compress', 'shell'
	]);
};