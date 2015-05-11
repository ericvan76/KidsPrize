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

// token apis
router.post('/auth/token', function(req, res, next) {
  if (!req.isAuthenticated()) {
    res.status(401).send('Unauthorised');
  } else {
    tokens.issueToken(req.user.id, 'webapp', function(err, token) {
      if (err || !token) {
        res.status(401).send('Unauthorised');
      } else {
        res.json(token);
      }
    });
  }
});

router.post('/auth/token/revoke', function(req, res, next) {
  var token = null;
  // extract token from header
  if (req.headers && req.headers.authorization) {
    var parts = req.headers.authorization.split(' ');
    if (parts.length == 2) {
      var scheme = parts[0],
        credentials = parts[1];
      if (/^Bearer$/i.test(scheme)) {
        token = credentials;
      }
    }
  }
  if (!token) {
    return res.status(400).send('Bad Request');
  }
  tokens.revokeToken(token, function(err) {
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

module.exports = router;

