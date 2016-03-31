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
				js_dest: '<%= app %>/scripts/vendor/',
				scss_dest: '<%= app %>/scss/',
				css_dest: '<%= app %>/css/',
				fonts_dest: '<%= app %>/fonts/',
				images_dest: '<%= app %>/images/',
				options: {
					keepExpandedHierarchy: false,
					expand: false,
					ignorePackages: [
						'what-input'
					],
					packageSpecific: {
						'foundation-sites': {
							'files': [
								'js/foundation.core.js', 'js/foundation.util.*.js', 'js/foundation.reveal.js', 
								'js/foundation.dropdown.js', 'js/foundation.abide.js', 'js/foundation.tooltip.js'
							]
						},
						'motion-ui': {
							'files': ['dist/motion-ui.js']
						},
						'lodash': {
							'files': ['lodash.js']
						},
						'font-awesome': {
							'files': ['fonts/**']
						},
						'font-awesome-animation': {
							'files': ['dist/font-awesome-animation.css']
						},
						'foundation-datepicker': {
							'files': ['js/foundation-datepicker.js']
						},
						'react-select':{
							'files': ['dist/react-select.js', 'dist/react-select.css']
						},
						'classnames':{
							'files': ['dedupe.js']
						},
						'requirejs-plugins':{
							'files': ['src/async.js']
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
					sourcemap: 'none',
					style: 'expanded', // expanded or nested or compact or compressed
					loadPath: [
						'bower_components/foundation-sites/scss',
						'bower_components/motion-ui/src',
						'bower_components/font-awesome/scss',
						'bower_components/foundation-datepicker/css',
						'bower_components/select2/src/scss'
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
		babel: {
	        options: {
	            sourceMap: false,
	            presets: ['react'],
	            plugins: ['react-intl'],

	        },
	        dist: {
	            files: [{
	            	src: ['<%= app %>/scripts/views/layout/main.jsx'],
	            	dest: '<%= app %>/scripts/views/layout/main.js'
	            },{
	            	src: ['<%= app %>/scripts/views/layout/frame.jsx'],
	            	dest: '<%= app %>/scripts/views/layout/frame.js'
	            },{
	            	src: ['<%= app %>/scripts/views/layout/menu.jsx'],
	            	dest: '<%= app %>/scripts/views/layout/menu.js'
	            },{
	            	src: ['<%= app %>/scripts/views/layout/user.jsx'],
	            	dest: '<%= app %>/scripts/views/layout/user.js'
	            },{
	            	src: ['<%= app %>/scripts/views/layout/loading.jsx'],
	            	dest: '<%= app %>/scripts/views/layout/loading.js'
	            },{
	            	src: ['<%= app %>/scripts/views/campaign/list.jsx'],
	            	dest: '<%= app %>/scripts/views/campaign/list.js'
	            },{
	            	src: ['<%= app %>/scripts/views/campaign/edit.jsx'],
	            	dest: '<%= app %>/scripts/views/campaign/edit.js'
	            },{
	            	src: ['<%= app %>/scripts/views/campaign/publish.jsx'],
	            	dest: '<%= app %>/scripts/views/campaign/publish.js'
	            },{
	            	src: ['<%= app %>/scripts/views/user/adminList.jsx'],
	            	dest: '<%= app %>/scripts/views/user/adminList.js'
	            },{
	            	src: ['<%= app %>/scripts/views/user/employee.jsx'],
	            	dest: '<%= app %>/scripts/views/user/employee.js'
	            },{
	            	src: ['<%= app %>/scripts/views/distribution/list.jsx'],
	            	dest: '<%= app %>/scripts/views/distribution/list.js'
	            },{
	            	src: ['<%= app %>/scripts/views/distribution/publish.jsx'],
	            	dest: '<%= app %>/scripts/views/distribution/publish.js'
	            },{
	            	src: ['<%= app %>/scripts/views/distribution/dismiss.jsx'],
	            	dest: '<%= app %>/scripts/views/distribution/dismiss.js'
	            },{
	            	src: ['<%= app %>/scripts/views/monitor/list.jsx'],
	            	dest: '<%= app %>/scripts/views/monitor/list.js'
	            },{
	            	src: ['<%= app %>/scripts/views/monitor/edit.jsx'],
	            	dest: '<%= app %>/scripts/views/monitor/edit.js'
	            },{
	            	src: ['<%= app %>/scripts/views/monitor/dismiss.jsx'],
	            	dest: '<%= app %>/scripts/views/monitor/dismiss.js'
	            },{
	            	src: ['<%= app %>/scripts/views/report/list.jsx'],
	            	dest: '<%= app %>/scripts/views/report/list.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/cover.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/cover.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/footer.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/footer.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/campaign.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/campaign.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/campaignSummary.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/campaignSummary.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/submap.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/submap.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/submapDetail.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/submapDetail.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/dmap.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/dmap.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/dmapDetailMap.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/dmapDetailMap.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/distributionMap.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/distributionMap.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/distributionDetailMap.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/distributionDetailMap.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/mapZoom.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/mapZoom.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/options.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/options.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/penetrationColor.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/penetrationColor.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/optionsDMapSelector.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/optionsDMapSelector.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/campaignOptions.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/campaignOptions.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/distributionOptions.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/distributionOptions.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/shared/reportOptions.jsx'],
	            	dest: '<%= app %>/scripts/views/print/shared/reportOptions.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/campaign.jsx'],
	            	dest: '<%= app %>/scripts/views/print/campaign.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/distribution.jsx'],
	            	dest: '<%= app %>/scripts/views/print/distribution.js'
	            },{
	            	src: ['<%= app %>/scripts/views/print/report.jsx'],
	            	dest: '<%= app %>/scripts/views/print/report.js'
	            },{
	            	src: ['<%= app %>/scripts/views/gtu/edit.jsx'],
	            	dest: '<%= app %>/scripts/views/gtu/edit.js'
	            },{
	            	src: ['<%= app %>/scripts/views/gtu/monitor.jsx'],
	            	dest: '<%= app %>/scripts/views/gtu/monitor.js'
	            },{
	            	src: ['<%= app %>/scripts/views/gtu/assign.jsx'],
	            	dest: '<%= app %>/scripts/views/gtu/assign.js'
	            },{
	            	src: ['<%= app %>/scripts/views/admin/dashboard.jsx'],
	            	dest: '<%= app %>/scripts/views/admin/dashboard.js'
	            }]
	        }
	    },
		useminPrepare: {
			html: ['<%= app %>/index.html'],
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
				tasks: ['bower', 'sass', 'babel']
			},
			sass: {
				files: '<%= app %>/scss/**/*.scss',
				tasks: ['sass']
			},
			babel: {
	            files: '<%= app %>/scripts/**/*.jsx',
	            tasks: ['babel']
	        },
			livereload: {
				files: ['<%= app %>/**/*.html', '<%= app %>/scripts/**/*.js', '<%= app %>/css/**/*.css', '<%= app %>/images/**/*.{jpg,gif,svg,jpeg,png}'],
				options: {
					livereload: true
				}
			}
		},
	});

	grunt.registerTask('debug', ['bower', 'sass']);
	grunt.registerTask('default', ['debug']);

};