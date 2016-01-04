(function () {
  'use strict';

  var passport = require('passport'),
    FacebookStrategy = require('passport-facebook').Strategy,
    GoogleStrategy = require('passport-google-oauth').OAuth2Strategy,
    BearerStrategy = require('passport-http-bearer').Strategy,
    userCtrl = require('../app/user/user-controller'),
    tokenCtrl = require('../app/auth/token-controller'),
    config = require('../config');

  // Serialize and deserialize
  passport.serializeUser(function (user, done) {
    done(null, user.id);
  });

  passport.deserializeUser(function (id, done) {
    userCtrl.read(id, function (err, user) {
      done(err, user);
    });
  });

  // Strategies
  passport.use(new FacebookStrategy(config.facebook,
    function (accessToken, refreshToken, profile, done) {
      console.log(profile);
      var normUser = userCtrl.normaliseUser('facebook', profile);
      userCtrl.resolveUser(normUser, function (err, user) {
        done(err, user);
      });
    }));

  passport.use(new GoogleStrategy(config.google,
    function (accessToken, refreshToken, profile, done) {
      var normUser = userCtrl.normaliseUser('google', profile);
      userCtrl.resolveUser(normUser, function (err, user) {
        done(err, user);
      });
    }));

  passport.use(new BearerStrategy({},
    function (token, done) {
      tokenCtrl.validateToken(token, function (err, user) {
        done(err, user);
      });
    }));

})();
