'use strict';

var express = require('express'),
  app = express(),
  auth = require('../auth'),
  User = auth.User;

app.get('/', function(req, res, next) {
  User.find(function(err, users) {
    if (err) {
      next(err);
    } else {
      res.json(users);
    }
  });
});

module.exports = app;
