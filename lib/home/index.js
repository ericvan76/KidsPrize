'use strict';

var express = require('express'),
  app = module.exports = express(),
  config = require('../config'),
  auth = require('../auth/authorisation');

app.set('views', __dirname);
app.set('view engine', 'jade');

/* GET home page. */
app.get('/', auth.requiresLogin, function(req, res, next) {
  res.render('index', {
    title: config.app.name
  });
});
