'use strict';

var express = require('express'),
  app = module.exports = express(),
  User = require('../auth/models/user'),
  auth = require('../auth/authorisation'),
  passport = require('../auth/passport'),
  config = require('../config');

app.get('/', passport.authenticate('bearer', {
  session: false
}), function(req, res, next) {
  User.find(function(err, users) {
    if (err) {
      next(err);
    } else {
      res.json(users);
    }
  })
});
