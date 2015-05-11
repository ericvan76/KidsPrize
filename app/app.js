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
  routes = require('./routes'),
  config = require('../config');

// view engine setup
app.set('views', path.join(__dirname, '../views'));
app.set('view engine', 'jade');

// error handling
if (process.env.NODE_ENV === 'development') {
  app.use(express.errorHandler());
} else {
  // production only
}

// favicon & public
app.use(favicon(path.join(__dirname, '../public/favicon.ico')));
app.use(express.static(path.join(__dirname, '../public')));
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
require('./passport');
app.use(passport.initialize());
app.use(passport.session());

// auth
app.use(routes.auth);

// api
app.use('/api', requiresToken, routes.api);

// index & partials
app.get('/', routes.index);
app.get('/partials/:name', routes.partials);

// angular catch-all
app.get('*', routes.index);

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
