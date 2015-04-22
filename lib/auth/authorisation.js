'use strict';

var app = require('./');
var passport = require('./passport');

// auth functions
exports.requiresLogin = function(req, res, next) {
  if (!req.isAuthenticated()) {
    res.redirect(app.mountpath + '/login');
  } else {
    next();
  }
};

exports.requiresToken = function(req, res, next) {
  passport.authenticate('bearer', {
    session: false
  }, function(err, user) {
    if (err) {
      res.status(401).send('Unauthorised');
    } else {
      next();
    }
  });
};
