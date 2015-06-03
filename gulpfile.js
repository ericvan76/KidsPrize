'use strict';

var gulp = require('gulp'),
  ngAnnotate = require('gulp-ng-annotate'),
  ngTemplates = require('gulp-ng-templates'),
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
  del = require('del'),
  merge = require('merge-stream');

// styles
gulp.task('styles', function() {

  del.sync('public/dist/css/**/*');

  return merge(
    // concat vendors
    gulp.src([
      'bower_components/bootstrap-social/bootstrap-social.css',
      'bower_components/angular-bootstrap/ui-bootstrap-csp.css'
    ])
    .pipe(sourcemaps.init())
    .pipe(concat('vender.css'))
    .pipe(minifyCss())
    .pipe(sourcemaps.write('./'))
    .pipe(rename({
      suffix: '.min'
    }))
    .pipe(gulp.dest('public/dist/css')),

    // self
    gulp.src('public/css/style.scss')
    .pipe(sourcemaps.init())
    .pipe(sass.sync().on('error', sass.logError))
    .pipe(minifyCss())
    .pipe(rename({
      suffix: '.min'
    }))
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest('public/dist/css'))
  );

});


// scripts
gulp.task('scripts', function() {

  del.sync('public/dist/js/**/*');

  return merge(
    // concat vendors
    gulp.src([
      'bower_components/angular-bootstrap/ui-bootstrap.min.js',
      'bower_components/angular-bootstrap/ui-bootstrap-tpls.min.js',
      'bower_components/angular-ui-sortable/sortable.min.js',
      'bower_components/angular-ui-utils/validate.min.js',
      'bower_components/js-base64/base64.min.js',
      'bower_components/date-utils/lib/date-utils.min.js'
    ])
    .pipe(sourcemaps.init())
    .pipe(concat('vender.js'))
    .pipe(rename({
      suffix: '.min'
    }))
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest('public/dist/js')),

    merge(
      // ng templates
      gulp.src('public/app/**/*.html')
      .pipe(minifyHTML({
        conditionals: true,
        spare: true
      }))
      .pipe(rename({
        dirname: ''
      }))
      .pipe(ngTemplates('app.templates')),

      // ng app
      gulp.src('public/app/**/*.js')
    )
    .pipe(sourcemaps.init())
    .pipe(concat('app.js'))
    .pipe(ngAnnotate())
    .pipe(uglify({
      mangle: false
    }))
    .pipe(rename({
      suffix: '.min'
    }))
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest('public/dist/js'))

  );

});

// html & css/js injection
gulp.task('injection', ['styles', 'scripts'], function() {

  del.sync('public/dist/index.html');

  return gulp.src('public/index.html')
    .pipe(inject(gulp.src(['public/dist/css/**/*.css', 'public/dist/js/**/*.js'], {
      read: false
    }), {
      ignorePath: 'public/dist'
    }))
    .pipe(minifyHTML({
      conditionals: true,
      spare: true
    }))
    .pipe(gulp.dest('public/dist'));

});

// images
gulp.task('images', function() {

  del.sync('public/dist/img/**/*');

  return gulp.src('public/img/**/*')
    .pipe(image())
    .pipe(gulp.dest('public/dist/img'));

});

// build
gulp.task('build', ['injection', 'images']);

// watch
gulp.task('watch', ['build'], function() {
  gulp.watch(['public/app/**/*', 'public/css/**/*', 'public/index.html'], ['injection']);
  gulp.watch('public/img/**/*', ['images']);
});

// start server
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

// build & start server
gulp.task('default', ['watch', 'server']);
