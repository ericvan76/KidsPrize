'use strict';

var express = require('express'),
  app = express(),
  passport = require('passport'),
  FacebookStrategy = require('passport-facebook').Strategy,
  GoogleStrategy = require('passport-google-oauth').OAuth2Strategy,
  BearerStrategy = require('passport-http-bearer').Strategy,
  querystring = require('querystring'),
  User = require('./models/user'),
  Token = require('./models/token'),
  util = require('../util'),
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
}, function(accessToken, refreshToken, profile, done) {
  User.postAuth('facebook', profile,
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

// Config Google
passport.use(new GoogleStrategy({
  clientID: config.google.clientID,
  clientSecret: config.google.clientSecret,
  callbackURL: config.google.callbackURL,
}, function(accessToken, refreshToken, profile, done) {
  User.postAuth('google', profile,
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

// app settings
app.set('views', __dirname);
app.set('view engine', 'jade');

// facebook endpoints
app.get('/facebook', passport.authenticate('facebook', {
  scope: ['public_profile', 'email']
}));

app.get('/facebook/callback',
  passport.authenticate('facebook', {
    successRedirect: '/',
    failureRedirect: app.mountpath + '/login'
  }));

// google endpoints
app.get('/google', passport.authenticate('google', {
  scope: [
    'https://www.googleapis.com/auth/userinfo.email',
    'https://www.googleapis.com/auth/userinfo.profile'
  ]
}));

app.get('/google/callback',
  passport.authenticate('google', {
    successRedirect: '/',
    failureRedirect: app.mountpath + '/login'
  }));

// login
app.get('/login', function(req, res, next) {
  res.render('login', {
    mountpath: app.mountpath
  });
});

// logout
app.get('/logout', function(req, res, next) {
  req.logout();
  // todo: revoke token
  if (req.session.token) {
    Token.revokeToken(req.session.token, function() {
      req.session.token = null;
      res.redirect(app.mountpath + '/login');
    });
  }
});

// bearer token callback
app.get('/token/callback', requiresLogin, function(req, res, next) {
  if (!req.query.access_token || req.query.token_type !== 'bearer' || !req.query.expires_in || !req.query.state) {
    return res.status(401).send('Unauthorised');
  }
  // todo: verify state
  req.session.token = req.query.access_token;
  res.status(200).end();
});

// issue bearer token, Implicity Grant
app.get('/token', requiresLogin, function(req, res, next) {
  if (!req.query.response_type || req.query.response_type !== 'token') {
    return res.status(400).send('Incorrect or missing response_type');
  }
  if (!req.query.client_id || req.query.client_id.trim() === '') {
    return res.status(400).send('Incorrect or missing client_id');
  }
  if (!req.query.redirect_uri || req.query.redirect_uri !== app.mountpath + '/token/callback') {
    return res.status(400).send('Incorrect or missing redirect_uri');
  }
  if (!req.query.state || req.query.state.trim() === '') {
    return res.status(400).send('Incorrect or missing state');
  }
  Token.issueToken(req.user.id, req.query.client_id, function(err, token) {
    if (err) {
      return res.status(401).send('Unauthorised');
    }
    var return_obj = {
      token_type: token.token_type,
      access_token: token.access_token,
      expires_in: Math.round((token.expire_at - new Date()) / 1000),
      refresh_token: token.refresh_token || null,
      state: req.query.state
    };
    res.redirect(req.query.redirect_uri + '?' + querystring.stringify(return_obj));
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

module.exports = {
  app: app,
  User: User,
  requiresLogin: requiresLogin,
  requiresToken: requiresToken
};
