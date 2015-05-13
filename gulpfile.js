'use strict';

var gulp = require('gulp'),
  ngAnnotate = require('gulp-ng-annotate'),
  uglify = require('gulp-uglify'),
  image = require('gulp-image'),
  sass = require('gulp-sass'),
  minifyCss = require('gulp-minify-css'),
  minifyHTML = require('gulp-minify-html'),
  sourcemaps = require('gulp-sourcemaps'),
  nodemon = require('gulp-nodemon'),
  rename = require('gulp-rename'),
  inject = require('gulp-inject'),
  ignore = require('gulp-ignore'),
  concat = require('gulp-concat'),
  del = require('del');

gulp.task('styles', function() {
  del.sync('public/dist/css/**/*');
  return gulp.src('public/css/main.scss')
    .pipe(sourcemaps.init())
    .pipe(sass.sync().on('error', sass.logError))
    .pipe(minifyCss({
      compatibility: 'ie8'
    }))
    .pipe(rename({
      suffix: ".min"
    }))
    .pipe(sourcemaps.write())
    .pipe(gulp.dest('public/dist/css'));
});

gulp.task('scripts', function() {
  del.sync('public/dist/js/**/*');
  return gulp.src('public/js/**/*.js')
    .pipe(sourcemaps.init())
    .pipe(concat('all.js'))
    .pipe(ngAnnotate())
    .pipe(uglify({
      mangle: false
    }))
    .pipe(rename({
      suffix: ".min"
    }))
    .pipe(sourcemaps.write())
    .pipe(gulp.dest('public/dist/js'));
});

gulp.task('images', function() {
  del.sync('public/dist/img/**/*');
  return gulp.src('public/img/**/*')
    .pipe(image())
    .pipe(gulp.dest('public/dist/img'));
});

gulp.task('views', ['styles', 'scripts'], function() {
  del.sync('public/dist/**/*.html');
  return gulp.src(['public/**/*.html', '!public/dist/**/*'])
    .pipe(sourcemaps.init())
    .pipe(inject(gulp.src(['public/dist/css/**/*.css', 'public/dist/js/**/*.js'], {
      read: false
    }), {
      ignorePath: 'public/dist'
    }))
    .pipe(minifyHTML({
      conditionals: true,
      spare: true
    }))
    .pipe(sourcemaps.write())
    .pipe(gulp.dest('public/dist'));
});

gulp.task('build', ['views', 'images'], function() {
  gulp.watch(['public/css/**/*.*', 'public/js/**/*.*', 'public/**/*.html', '!public/dist/**/*'], ['views']);
  gulp.watch('public/img/**/*.*', ['images']);
});

gulp.task('server', function() {
  return nodemon({
    script: 'server.js',
    ext: 'js',
    ignore: ['public/**/*', 'node_modules/**/*', 'bower_components/**/*'],
    env: {
      'NODE_ENV': 'development'
    }
  });
});

gulp.task('default', ['build', 'server']);
