'use strict';

var passport = module.exports = require('passport'),
  FacebookStrategy = require('passport-facebook').Strategy,
  GoogleStrategy = require('passport-google-oauth').OAuth2Strategy,
  BearerStrategy = require('passport-http-bearer').Strategy,
  User = require('./models/user'),
  config = require('../config');

// Serialize and deserialize
passport.serializeUser(function(user, done) {
  done(null, user.id);
});

passport.deserializeUser(function(id, done) {
  User.findById(id, function(err, user) {
    done(err, user);
  });
});

// Config Facebook
passport.use(new FacebookStrategy({
    clientID: config.facebook.clientID,
    clientSecret: config.facebook.clientSecret,
    callbackURL: config.facebook.callbackURL
  },
  function(accessToken, refreshToken, profile, done) {

    process.nextTick(function() {
      User.postAuth('facebook', profile,
        function(err, user) {
          if (err) {
            return done(err);
          }
          done(null, user);
        });
    });
  }));

// Config Google
passport.use(new GoogleStrategy({
    clientID: config.google.clientID,
    clientSecret: config.google.clientSecret,
    callbackURL: config.google.callbackURL,
  },
  function(accessToken, refreshToken, profile, done) {

    process.nextTick(function() {
      User.postAuth('google', profile,
        function(err, user) {
          if (err) {
            return done(err);
          }
          done(null, user);
        });
    });
  }));

// Config Bear Token
passport.use(new BearerStrategy({},
  function(token, done) {

    process.nextTick(function() {
      console.log('token: ', token);
      if (!token) {
        return done(new Error('Unauthorised'));
      } else {
        return done(null, {});
      }
      // validate token
      // findByToken(token, function(err, user) {
      //   if (err) {
      //     return done(err);
      //   }
      //   if (!user) {
      //     return done(null, false);
      //   }
      //   return done(null, user);
      // })
    });
  }
));
