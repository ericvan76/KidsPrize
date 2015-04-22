'use strict';

var express = require('express'),
  app = module.exports = express(),
  path = require('path'),
  favicon = require('serve-favicon'),
  logger = require('morgan'),
  cookieParser = require('cookie-parser'),
  bodyParser = require('body-parser'),
  session = require('express-session'),
  passport = require('./lib/auth/passport'),
  home = require('./lib/home'),
  auth = require('./lib/auth'),
  user = require('./lib/user'),
  config = require('./lib/config');

// view engine setup
app.set('views', path.join(__dirname, 'lib/views'));
app.set('view engine', 'jade');

// uncomment after placing your favicon in /public
// app.use(favicon(__dirname + '/public/favicon.ico'));
app.use(express.static(path.join(__dirname, 'public')));

app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({
  extended: false
}));
app.use(cookieParser());
app.use(session(config.session));

// initialise passport
app.use(passport.initialize());
app.use(passport.session());

// plug your sub apps here
app.use(home);
app.use('/auth', auth);
app.use('/user', user);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handlers
app.use(function(err, req, res, next) {
  var stacktrace = {};
  if (process.env.NODE_ENV === 'development') {
    // will print stack trace to user
    stacktrace = err;
  }
  res.status(err.status || 500);
  res.render('error', {
    title: 'Error',
    message: err.message,
    error: stacktrace
  });
});
