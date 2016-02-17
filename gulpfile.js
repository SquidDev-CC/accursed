var gulp = require('gulp');
var del = require('del'); // Used to clean files
var rename = require('gulp-rename'); // Used to rename *.* to *.min.*
var runSequence = require('run-sequence'); // Don't release files before cleaning

	// Javascript
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');

	// Stylesnp
var less = require('gulp-less');
var cssmin = require('gulp-cssmin');

var
	SRC = './assets/',
	DIST = './wwwroot/static/',
	BOWER = './bower_components/';

gulp.task('clean', function (cb) {
	del.sync([DIST + "**/*", "!" + DIST + "web.config"], cb);
});

gulp.task('copy:fonts', function() {
	return gulp.src([
		BOWER + "bootstrap/fonts/*.*",
		BOWER + "font-awesome/fonts/*.*"
	]).pipe(gulp.dest(DIST));
});

gulp.task('copy:metro', function() {
	return gulp.src([
		BOWER + "bootstrap/less/**/*",
	]).pipe(gulp.dest(BOWER + "metro-bootstrap/app/bootstrap"));
});

gulp.task('css:less', ['copy:metro'], function() {
	return gulp.src(SRC + 'css/main.less')
		.pipe(less({ paths: [ BOWER ] }))
		.pipe(gulp.dest(DIST));
});


gulp.task('css:minify', ['css:less'], function() {
	return gulp.src(DIST + 'main.css')
		.pipe(cssmin())
		.pipe(rename({ suffix: '.min' }))
		.pipe(gulp.dest(DIST));
});

gulp.task('js:combine', function() {
	return gulp.src([
		BOWER + "jquery/dist/jquery.js",
		BOWER + "bootstrap/dist/js/bootstrap.js",
	])
		.pipe(concat('main.js'))
		.pipe(gulp.dest(DIST));
});

gulp.task('js:minify', ['js:combine'], function() {
	return gulp.src([DIST + 'main.js',])
		.pipe(uglify())
		.pipe(rename({ suffix: '.min' }))
		.pipe(gulp.dest(DIST));
});


gulp.task('js', ['js:combine', 'js:minify']);
gulp.task('css', ['css:less', 'css:minify']);

gulp.task('build', function(callback) {
	// We should clean before anything else, everything else doesn't matter
	return runSequence('js', 'css', 'copy:fonts', callback);
});

// Just build it
gulp.task('default', ['build']);
