'use strict';

var express = require('express'),
  app = module.exports = express(),
  config = require('../config'),
  auth = require('../auth');

app.set('views', __dirname);
app.set('view engine', 'jade');

/* GET home page. */
app.get('/', function(req, res, next) {
  res.render('index', {
    title: config.app.name
  });
});
