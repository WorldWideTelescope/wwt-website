/// <binding ProjectOpened='watch' />

/**!
Gruntfile to perform wwt less compilation and script component updates
**/

module.exports = function(grunt) {
    'use strict';

    // Force use of Unix newlines
    grunt.util.linefeed = '\n';

    RegExp.quote = function(string) {
        return string.replace(/[-\\^$*+?.()|[\]{}]/g, '\\$&');
    };

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        less: {
            compileCore: {
                options: {
                    sourceMap: true,
                    outputSourceFiles: true,
                    sourceMapURL: 'Content/CSS/wwt.css.map',
                    sourceMapFilename: 'Content/CSS/wwt.css.map'
                },

                src: 'Content/CSS/wwt.less',
                dest: 'Content/CSS/wwt.css'
            }
        },

        autoprefixer: {
            options: {
                browsers: [
                    "Android 2.3",
                    "Android >= 4",
                    "Chrome >= 20",
                    "Firefox >= 24",
                    "Explorer >= 10",
                    "iOS >= 6",
                    "Opera >= 12",
                    "Safari >= 6"
                ]
            },
            core: {
                options: {
                    map: true
                },
                src: 'Content/CSS/wwt.css'
            }
        },

        cssmin: {
            options: {
                compatibility: 'ie10',
                keepSpecialComments: '*',
                noAdvanced: true
            },
            minifyCore: {
                src: 'Content/CSS/wwt.css',
                dest: 'Content/CSS/wwt.min.css'
            }
        },

        copy: {
            vendor: {
                files: [
                    {
                        src: 'bower_components/jquery/dist/jquery.js',
                        dest: 'Scripts/ext/',
                        expand: true,
                        flatten: true
                    }, {
                        src: 'bower_components/bootstrap/dist/js/bootstrap.js',
                        dest: 'Scripts/ext/',
                        expand: true,
                        flatten: true
                    }, {
                        src: 'bower_components/angular/angular.js',
                        dest: 'Scripts/ext/',
                        expand: true,
                        flatten: true
                    }, {
                        cwd: 'bower_components/bootstrap/less/',
                        src: '**/*',
                        dest: 'Content/bootstrap/',
                        expand: true
                    }, {
                        cwd: 'bower_components/bootstrap/fonts/',
                        src: '**/*',
                        dest: 'Content/fonts/',
                        expand: true
                    }
                ]
            }
        },

        watch: {
            vendor: { // will be triggered by 'bower install' when it finds updates
                files: [
                    'bower_components/jquery/dist/jquery.js',
                    'bower_components/bootstrap/dist/js/bootstrap.js',
                    'bower_components/angular/angular.js'
                ],
                tasks: ['vendor']
            },

            less: {
                files: 'Content/CSS/*.less',
                tasks: ['dist-css']
            }
        }
    });

    // Dependencies
    require('load-grunt-tasks')(grunt, { scope: 'devDependencies' });

    grunt.registerTask('dist-css', ['less:compileCore', 'autoprefixer:core', 'cssmin:minifyCore']);
    grunt.registerTask('vendor', ['copy:vendor']);
};
