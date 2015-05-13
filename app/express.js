'use strict';

var express = require('express'),
  errorhandler = require('errorhandler'),
  mongoose = require('mongoose'),
  passport = require('passport'),
  cookieParser = require('cookie-parser'),
  bodyParser = require('body-parser'),
  session = require('express-session'),
  MongoStore = require('connect-mongo')(session),
  path = require('path'),
  favicon = require('serve-favicon'),
  logger = require('morgan'),
  routes = require('./routes'),
  config = require('../config');

var app = express();
// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

// error handling
var nodeEnv = process.env.NODE_ENV || 'development';
if (nodeEnv === 'development') {
  app.use(errorhandler());
} else {
  // production only
}

var publicFolder = '../public/dist';

// favicon & public
app.use(favicon(path.join(__dirname, publicFolder, 'img/favicon.ico')));
app.use(express.static(path.join(__dirname, publicFolder)));
app.use('/css/bootstrap-social',
  express.static(path.join(__dirname, '../bower_components/bootstrap-social')));

app.use(logger('dev'));

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({
  extended: false
}));
app.use(cookieParser());

// session
app.use(session({
  secret: config.session.secret,
  saveUninitialized: config.session.saveUninitialized,
  resave: config.session.resave,
  cookie: config.cookie,
  store: new MongoStore({
    mongooseConnection: mongoose.connection
  })
}));

// initialise passport
require('../config/passport');
app.use(passport.initialize());
app.use(passport.session());

// auth
app.use(routes.auth);

// api
app.use('/api', requiresToken, routes.api);

// angular catch-all
app.get('*', function(req, res) {
  res.sendFile(path.join(__dirname, publicFolder, 'index.html'));
});

// authentication functions
function requiresLogin(req, res, next) {
  if (!req.isAuthenticated()) {
    res.redirect('/login');
  } else {
    next();
  }
}

function requiresToken(req, res, next) {
  passport.authenticate('bearer', {
    session: false
  })(req, res, next);
}

module.exports = app;
