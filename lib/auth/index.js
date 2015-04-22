'use strict';

var express = require('express'),
  app = module.exports = express(),
  passport = require('./passport');

app.set('views', __dirname);
app.set('view engine', 'jade');

app.get('/facebook', passport.authenticate('facebook', {
  scope: ['public_profile', 'email']
}));

app.get('/facebook/callback',
  passport.authenticate('facebook', {
    successRedirect: '/',
    failureRedirect: app.mountpath + '/login'
  }));

app.get('/google', passport.authenticate('google', {
  scope: [
    'https://www.googleapis.com/auth/plus.login',
    'https://www.googleapis.com/auth/plus.profile.emails.read'
  ]
}));

app.get('/google/callback',
  passport.authenticate('google', {
    successRedirect: '/',
    failureRedirect: app.mountpath + '/login'
  }));

app.get('/login', function(req, res, next) {
  res.render('login', {
    mountpath: app.mountpath
  });
});

app.get('/logout', function(req, res, next) {
  req.logout();
  res.redirect(app.mountpath + '/login');
});
