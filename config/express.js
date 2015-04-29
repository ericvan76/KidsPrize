'use strict';

var express = require('express'),
  app = module.exports = express(),
  mongoose = require('mongoose'),
  passport = require('passport'),
  auth = require('./passport'),
  cookieParser = require('cookie-parser'),
  bodyParser = require('body-parser'),
  session = require('express-session'),
  MongoStore = require('connect-mongo')(session),
  path = require('path'),
  favicon = require('serve-favicon'),
  logger = require('morgan'),
  config = require('./');

// view engine setup
app.set('views', path.join(__dirname, '../app/views'));
app.set('view engine', 'jade');

// favicon & public
app.use(favicon(path.join(__dirname, '../public/assets/favicon.ico')));
app.use(express.static(path.join(__dirname, '../public/assets')));
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
app.use(passport.initialize());
app.use(passport.session());

// auth
app.use(require('../app/routes/auth'));

// rest api
app.all('/api/*', requiresToken);

// web app
app.all('/*', requiresLogin);
app.use('/', require('../app/routes'));

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


function requiresLogin(req, res, next) {
  if (!req.isAuthenticated()) {
    res.redirect('/auth/login');
  } else {
    next();
  }
}

function requiresToken(req, res, next) {
  passport.authenticate('bearer', {
    session: false
  }, function(err, user) {
    if (err || !user) {
      res.status(401).send('Unauthorised');
    } else {
      next();
    }
  });
}
