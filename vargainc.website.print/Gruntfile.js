'use strict';

module.exports = function (grunt) {
	require('load-grunt-tasks')(grunt);
	grunt.verbose.writeln('begin task...');

	grunt.initConfig({
		pkg: grunt.file.readJSON('package.json'),
		app: 'app',
		dist: 'dist',
		bower: {
			all: {
				dest: '<%= app %>',
				js_dest: '.tmp/scripts/vendor/',
				scss_dest: '<%= app %>/scss/',
				css_dest: '<%= app %>/css/',
				fonts_dest: '<%= app %>/fonts/',
				images_dest: '<%= app %>/images/',
				options: {
					keepExpandedHierarchy: false,
					expand: true,
					ignorePackages: [],
					packageSpecific: {
						'handlebars': {
							files: [
								'handlebars.runtime.js'
							]
						},
						'handlebars-helper-intl': {
							files: [
								'dist/handlebars-intl.js',
							]
						},
						'font-awesome': {
							keepExpandedHierarchy: false,
							expand: false,
							files: [
								'fonts/**'
							]
						},
						'foundation':{
							files:['js/foundation.js']
						},
						'urijs': {
							files: [
								'src/punycode.js',
								'src/IPv6.js',
								'src/SecondLevelDomains.js',
								'src/URI.js'
							]
						},
						'intl': {
							files: [
								'dist/Intl.js'
							]
						},
						//css
						'alertify.js':{
							files: [
								'lib/alertify.js', 
								'themes/alertify.core.css',
								'themes/alertify.bootstrap.css'
							]
						},
						'css-spinners':{
							files: []
						},
						'jquery-ui':{
							files:[
								'jquery-ui.js',
								'themes/ui-lightness/jquery-ui.css',
								'themes/ui-lightness/images/*',
							]
						}
					}
				}
			}
		},
		clean: {
			dist: {
				src: ['<%= dist %>/*']
			},
		},
		copy: {
			dist: {
				files: [{
					expand: true,
					cwd: '<%= app %>/',
					src: ['fonts/**', '**/*.html', 'images/**', '!**/*.scss', '!**/*.hbs'],
					dest: '<%= dist %>/'
				}]
			},
			stage: {
				files: [{
					expand: true,
					cwd: '<%= app %>/',
					src: ['fonts/**', '**/*.html', 'scripts/**', 'images/**', '!**/*.scss', '!**/*.hbs'],
					dest: '<%= dist %>/'
				}]
			}
		},
		sass: {
			dist: {
				options: {
					style: 'expanded', // expanded or nested or compact or compressed
					loadPath: [
						'bower_components/foundation/scss',
						'bower_components/font-awesome/scss',
						'bower_components/css-spinners/sass',
						'bower_components'
					],
					compass: true,
					quiet: true
				},
				files: {
					'<%= app %>/css/library.css': '<%= app %>/scss/library.scss',
					'<%= app %>/css/app.css': '<%= app %>/scss/app.scss',
				}
			}
		},
		handlebars: {
			compile: {
				options: {
					amd: true,
					// configure a namespace for your templates
					// namespace: function (filename) {
					// 	var names = filename.replace(/app\/(.*)(\/\w+\.hbs)/, '$1');
					// 	grunt.verbose.writeln('template namespace:' + names);
					// 	return names.split('/').join('.');
					// },
					namespace: false,

					// convert file path into a function name
					// in this example, I convert grab just the filename without the extension 
					// processName: function (filePath) {
					// 	var pieces = filePath.split('/');
					// 	return pieces[pieces.length - 1].split('.')[0];
					// },

					compilerOptions: {
						knownHelpersOnly: false
					}
				},
				files: {}
			}
		},
		useminPrepare: {
			html: ['<%= app %>/print.html'],
			options: {
				dest: '<%= dist %>'
			}
		},
		uglify: {
			options: {
				preserveComments: 'some',
				mangle: false,
				compress: {
					global_defs: {
						DEBUG: true // That very variable
					}
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
					'<%= dist %>/scripts/**/*.js',
					'<%= dist %>/css/**/*.css',
					//'<%= dist %>/images/**/*.{jpg,jpeg,gif,png,ico}'
				]
			}
		},
		usemin: {
			html: ['<%= dist %>/**/*.html'],
			css: ['<%= dist %>/css/**/*.css'],
			options: {
				dirs: ['<%= dist %>']
			}
		},
		watch: {
			grunt: {
				files: ['Gruntfile.js'],
				tasks: ['sass', 'compile-handlebar']
			},
			sass: {
				files: '<%= app %>/scss/**/*.scss',
				tasks: ['sass']
			},
			handlebars: {
				files: '<%= app %>/templates/**/*.hbs',
				tasks: ['compile-handlebar']
			},
			livereload: {
				files: ['<%= app %>/**/*.html', '<%= app %>/scripts/**/*.js', '<%= app %>/css/**/*.css', '<%= app %>/images/**/*.{jpg,gif,svg,jpeg,png}'],
				options: {
					livereload: true
				}
			}
		},
	});
	/**
	 * copy bower and concat
	 */
	grunt.registerTask('fixBowerJsFile', 'copy bower library to scripts/vendor and concat to one file used library name', function () {
		var app = grunt.config.get('app'),
			concatOptions = {
				custom: {
					files: {}
				}
			},
			bowerOptions = grunt.config.get('bower'),
			files = {},
			vendorPath = '.tmp/scripts/vendor/',
			needRemoveFolder = {},
			lib,
			skip = false;

		grunt.file.recurse(vendorPath, function (abspath, rootdir, subdir, filename) {
			grunt.verbose.writeln(subdir);
			needRemoveFolder[subdir] = true;
			lib = app + '/scripts/vendor/' + subdir.replace('.js', '') + '.js';
			switch (subdir) {
				case 'requirejs-plugins':
				case 'urijs':
					lib = app + '/scripts/vendor/' + filename;
					break;
				case 'css-spinners':
					skip = true;
					break;
				default: 
					break;
			}
			if(!skip){
				if (typeof files[lib] === 'undefined') {
					files[lib] = [];
				}
				files[lib].push(abspath);			
			}
			skip = false;
		});
		for (var k in files) {
			concatOptions.custom.files[k] = files[k];
		}
		grunt.verbose.writeln(JSON.stringify(files));
		grunt.config.set('concat', concatOptions);
		grunt.task.run('concat');
	});
	/**
	 * add templates to grunt handlebar options
	 */
	grunt.registerTask('compile-handlebar', 'add all templates to handlebars compile files', function () {
		var app = grunt.config.get('app'),
			handlebarsOptions = grunt.config.get('handlebars'),
			matches = grunt.file.expand(app + '/templates/**/*.hbs'),
			target;
		if (matches.length > 0) {
			for (var x = 0; x < matches.length; x++) {
				target = matches[x].replace(app, app + '/scripts');
				target = target.replace('.hbs', '.js');
				handlebarsOptions.compile.files[target] = matches[x];
			}
		}
		grunt.verbose.writeln(handlebarsOptions);
		grunt.config.set('handlebars', handlebarsOptions);
		grunt.task.run('handlebars');
	});
	/**
	 * add bower managed library to dist scripts/vendor
	 */
	grunt.registerTask('prepareUglify', 'config uglify to minify all js file', function () {
		var app = grunt.config.get('app'),
			dist = grunt.config.get('dist');
		var matches = grunt.file.expand(app + '/scripts/**/*.js');
		var uglify = grunt.config.get("uglify");
		uglify.generated = {
			files: []
		};
		if (matches.length > 0) {
			for (var x = 0; x < matches.length; x++) {
				uglify.generated.files.push({
					dest: matches[x].replace(app, dist),
					src: [matches[x]]
				});
			}
		}
		grunt.config.set("uglify", uglify);
	});

	grunt.registerTask('initRequireConfig', 'add scripts/vendor library to config', function () {
		var app = grunt.config.get('app'),
			srcConfig = app + '/scripts/config.js',
			matches = grunt.file.expand(app + '/scripts/vendor/*.js'),
			fs = require('fs');
		/**
		 * read config.js
		 */
		grunt.verbose.writeln('begin read original requirejs config file');
		var configContents = fs.readFileSync(srcConfig, 'utf8'),
			index = configContents.lastIndexOf(');'),
			config = configContents.substr(15, index - 15);
		//remove \r\t
		config = config.replace(/\r|\n|\t/g, '').replace("'", '"');
		var requireJsConfig = JSON.parse(config);
		grunt.verbose.writeln('', JSON.stringify(requireJsConfig));
		/**
		 * build vendor library config paths
		 */
		for (var x = 0; x < matches.length; x++) {
			var model = matches[x].replace(app + '/scripts/', '').replace('.js', '').substr(7);
			var path = matches[x].replace(app + '/scripts/', '').replace('.js', '');
			requireJsConfig.paths[model] = path;
		}
		grunt.verbose.writeln('write new requirejs config');
		fs.unlinkSync(srcConfig);
		fs.writeFileSync(srcConfig, 'require.config(' + JSON.stringify(requireJsConfig) + ');');
	});

	grunt.registerTask('updateRequireConfig', 'read from filerev and update the config file', function () {
		var newPath = grunt.filerev && grunt.filerev.summary ? grunt.filerev.summary : [],
			app = grunt.config.get('app'),
			dist = grunt.config.get('dist'),
			srcConfig = app + '/scripts/config.js',
			newConfig = '',
			app = 'app',
			fs = require('fs');
		/**
		 * read config.js
		 */
		grunt.verbose.writeln('begin read original requirejs config file');
		var configContents = fs.readFileSync(srcConfig, 'utf8'),
			config = configContents.substr(15, configContents.length - 15 - 2);
		grunt.verbose.writeln('', config);
		//remove \r\t
		config = config.replace(/\r|\n|\t/g, '').replace("'", '"');
		grunt.verbose.writeln('', config);
		var requireJsConfig = JSON.parse(config);
		grunt.verbose.writeln(JSON.stringify(requireJsConfig));
		/**
		 * build new config paths
		 */
		for (var k in newPath) {
			if (!k.startsWith(dist + '/scripts')) {
				continue;
			}
			var model = k.replace(dist + '/scripts/', '').replace('.js', '');
			if (model.startsWith('vendor')) {
				model = model.substr(7);
			}
			var path = newPath[k].replace(dist + '/scripts/', '').replace('.js', '');
			if (model.startsWith('config')) {
				newConfig = newPath[k];
			} else {
				requireJsConfig.paths[model] = path;
			}
		}
		grunt.verbose.writeln('write new requirejs config');
		fs.unlinkSync(newConfig);
		fs.writeFileSync(newConfig, 'require.config(' + JSON.stringify(requireJsConfig) + ');');
	});

	grunt.registerTask('debug', ['bower', 'fixBowerJsFile', 'sass', 'compile-handlebar', 'initRequireConfig']);
	grunt.registerTask('stage', ['debug', 'clean:dist', 'useminPrepare', 'copy:stage', 'concat', 'cssmin', 'filerev', 'usemin', 'updateRequireConfig']);
	grunt.registerTask('release', ['debug', 'clean:dist', 'useminPrepare', 'prepareUglify', 'copy:dist', 'concat', 'cssmin', 'uglify', 'filerev', 'usemin', 'updateRequireConfig']);
	grunt.registerTask('default', ['debug']);

};