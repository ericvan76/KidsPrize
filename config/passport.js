'use strict';

var passport = require('passport'),
  FacebookStrategy = require('passport-facebook').Strategy,
  GoogleStrategy = require('passport-google-oauth').OAuth2Strategy,
  BearerStrategy = require('passport-http-bearer').Strategy,
  tokens = require('../app/controllers/tokens'),
  users = require('../app/controllers/users'),
  config = require('./');

// Serialize and deserialize
passport.serializeUser(function(user, done) {
  done(null, {
    id: user.id,
    token: user.token
  });
});

passport.deserializeUser(function(str, done) {
  var o = str;
  if (!o || !o.id) {
    return done(null, false);
  }
  users.read(o.id, function(err, user) {
    if (err) {
      return done(err);
    }
    if (!user) {
      return done(null, false);
    }
    if (o.token) {
      user.token = o.token;
    }
    return done(null, user);
  });
});

// Strategies
passport.use(new FacebookStrategy(config.facebook,
  function(accessToken, refreshToken, profile, done) {
    users.postAuth('facebook', profile,
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

passport.use(new GoogleStrategy(config.google,
  function(accessToken, refreshToken, profile, done) {
    users.postAuth('google', profile,
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

passport.use(new BearerStrategy({},
  function(token, done) {
    tokens.validateToken(token,
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
