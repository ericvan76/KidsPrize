'use strict';

var express = require('express'),
  router = module.exports = express.Router(),
  passport = require('passport'),
  tokens = require('../controllers/tokens');

// facebook endpoints
router.get('/auth/facebook', passport.authenticate('facebook', {
  scope: ['public_profile', 'email']
}));

router.get('/auth/facebook/callback',
  passport.authenticate('facebook', {
    successRedirect: '/',
    failureRedirect: '/auth/login'
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
    failureRedirect: '/auth/login'
  }));

// login
router.get('/auth/login', function(req, res, next) {
  res.render('login');
});

// logout
router.get('/auth/logout', function(req, res, next) {
  if (req.session.passport && req.session.passport.user &&
    req.session.passport.user.token) {
    tokens.revokeToken(req.session.passport.user.token, function(err) {});
  }
  req.logout();
  res.redirect('/auth/login');
});
