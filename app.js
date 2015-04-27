'use strict';

var express = require('express'),
  app = module.exports = express(),
  mongoose = require('mongoose'),
  passport = require('passport'),
  cookieParser = require('cookie-parser'),
  bodyParser = require('body-parser'),
  session = require('express-session'),
  MongoStore = require('connect-mongo')(session),
  path = require('path'),
  favicon = require('serve-favicon'),
  logger = require('morgan'),
  home = require('./lib/home'),
  auth = require('./lib/auth'),
  user = require('./lib/user'),
  config = require('./lib/config');

// connect to mongodb
mongoose.connect(config.mongoose.dbURL, config.mongoose.options || {});
mongoose.set('debug', config.mongoose.debug || false);

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

// session
app.use(session({
  secret: 'do NOT tell ANY body',
  saveUninitialized: true,
  resave: false,
  cookie: config.cookie,
  store: new MongoStore({
    mongooseConnection: mongoose.connection
  })
}));

// initialise passport
app.use(passport.initialize());
app.use(passport.session());

// auth
app.use('/auth', auth.app);

// rest api
app.all('/api/*', auth.requiresToken);
app.use('/api/user', user);

// web app
app.all('/*', auth.requiresLogin);
app.use('/', home);

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
