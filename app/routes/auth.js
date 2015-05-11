'use strict';

var express = require('express'),
  router = express.Router(),
  passport = require('passport'),
  tokens = require('../controllers/tokens');

// facebook endpoints
router.get('/auth/facebook', passport.authenticate('facebook', {
  scope: ['public_profile', 'email']
}));

router.get('/auth/facebook/callback',
  passport.authenticate('facebook', {
    successRedirect: '/',
    failureRedirect: '/login'
  }));

// google endpoints
router.get('/auth/google', passport.authenticate('google', {
  scope: [
    'https://www.googleapis.com/auth/plus.login',
    'https://www.googleapis.com/auth/plus.profile.emails.read'
  ]
}));

router.get('/auth/google/callback',
  passport.authenticate('google', {
    successRedirect: '/',
    failureRedirect: '/login'
  }));

// request token
router.post('/auth/token', function(req, res, next) {
  if (!req.isAuthenticated()) {
    res.status(401).send('Unauthorised');
  } else {
    if (!req.query.response_type || req.query.response_type !== 'token') {
      return res.status(400).send('Invalid response_type');
    }
    if (!req.query.client_id || req.query.client_id !== 'webapp') {
      return res.status(400).send('Invalid client_id');
    }
    tokens.issueToken(req.user.id, req.query.client_id, req.query.state, function(err, token) {
      if (err || !token) {
        res.status(401).send('Unauthorised');
      } else {
        res.json(token);
      }
    });
  }
});

// revoke token
router.post('/auth/token/revoke', function(req, res, next) {
  var access_token = req.query.access_token;
  if (!access_token) {
    return res.status(400).send('Bad Request');
  }
  tokens.revokeToken(access_token, function(err) {
    if (err) {
      res.status(500).send('Unable to revoke token or token does not exists.');
    } else {
      res.status(200).end();
    }
  });
});

// logout
router.get('/auth/logout', function(req, res, next) {
  req.logout();
  res.redirect('/login');
});

// extract token from header
function extractToken(req) {
  if (req.headers && req.headers.authorization) {
    var parts = req.headers.authorization.split(' ');
    if (parts.length == 2) {
      var scheme = parts[0],
        credentials = parts[1];
      if (/^Bearer$/i.test(scheme)) {
        return credentials;
      }
    }
  }
  return null;
}

module.exports = router;
