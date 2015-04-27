'use strict';

var express = require('express'),
  app = express(),
  passport = require('passport'),
  BearerStrategy = require('passport-http-bearer').Strategy,
    querystring = require('querystring'),
  auth = require('./authorisation'),
  Token = require('./token'),
  config = require('../config');

// Config Bear Token
passport.use(new BearerStrategy({}, function(token, done) {
  Token.validateToken(token,
    function(err, user) {
      if (err) {
        return done(err);
      }
      if (!user) {
        return done(null, false);
      }
      return done(null, user);
    }
  );
}));


module.exports = {
  app: app,
  Token: Token
};
