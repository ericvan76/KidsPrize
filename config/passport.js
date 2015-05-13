(function() {
  'use strict';

  var passport = require('passport'),
    FacebookStrategy = require('passport-facebook').Strategy,
    GoogleStrategy = require('passport-google-oauth').OAuth2Strategy,
    BearerStrategy = require('passport-http-bearer').Strategy,
    users = require('../app/controllers/users'),
    tokens = require('../app/controllers/tokens'),
    config = require('../config');

  // Serialize and deserialize
  passport.serializeUser(function(user, done) {
    done(null, user.id);
  });

  passport.deserializeUser(function(id, done) {
    users.read(id, function(err, user) {
      done(err, user);
    });
  });

  // Strategies
  passport.use(new FacebookStrategy(config.facebook,
    function(accessToken, refreshToken, profile, done) {
      users.postAuth('facebook', profile, function(err, user) {
        done(err, user);
      });
    }));

  passport.use(new GoogleStrategy(config.google,
    function(accessToken, refreshToken, profile, done) {
      users.postAuth('google', profile, function(err, user) {
        done(err, user);
      });
    }));

  passport.use(new BearerStrategy({},
    function(token, done) {
      tokens.validateToken(token, function(err, user) {
        done(err, user);
      });
    }));

})();
