var gulp = require('gulp'),
  ignore = require('gulp-ignore'),
  uglify = require('gulp-uglify'),
  ngAnnotate = require('gulp-ng-annotate'),
  minifyCss = require('gulp-minify-css'),
  image = require('gulp-image'),
  minifyHTML = require('gulp-minify-html'),
  sourcemaps = require('gulp-sourcemaps'),
  nodemon = require('gulp-nodemon'),
  watch = require('gulp-watch'),
  del = require('del'),
  path = require('path');

gulp.task('clean', function() {
  return del.sync('public/dist/**/*');
});

gulp.task('minify-css', function() {
  return gulp.src('public/css/**/*.css')
    .pipe(watch('public/css/**/*.css'))
    .pipe(sourcemaps.init())
    .pipe(minifyCss({
      compatibility: 'ie8'
    }))
    .pipe(sourcemaps.write())
    .pipe(gulp.dest('public/dist/css'));
});

gulp.task('minify-js', function() {
  return gulp.src('public/js/**/*.js')
    .pipe(watch('public/js/**/*.js'))
    .pipe(sourcemaps.init())
    .pipe(ngAnnotate())
    .pipe(uglify({
      mangle: false
    }))
    .pipe(sourcemaps.write())
    .pipe(gulp.dest('public/dist/js'));
});

gulp.task('minify-html', function() {
  return gulp.src(['public/**/*.html', '!public/dist/**/*'])
    .pipe(watch(['public/**/*.html', '!public/dist/**/*']))
    .pipe(sourcemaps.init())
    .pipe(minifyHTML({
      conditionals: true,
      spare: true
    }))
    .pipe(sourcemaps.write())
    .pipe(gulp.dest('public/dist'));
});

gulp.task('compress-images', function() {
  return gulp.src('public/img/**/*.*')
    .pipe(watch('public/img/**/*.*'))
    .pipe(image())
    .pipe(gulp.dest('public/dist/img'));
});

gulp.task('server', function() {
  nodemon({
    script: 'server.js',
    ext: 'js',
    ignore: ['public/**/*', 'node_modules/**/*', 'bower_components/**/*'],
    env: {
      'NODE_ENV': 'development'
    }
  });
});

gulp.task('build', ['minify-css', 'minify-js', 'minify-html', 'compress-images']);

gulp.task('default', ['clean'], function() {
  gulp.run('build', 'server');
});
