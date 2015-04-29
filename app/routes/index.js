'use strict';

var express = require('express'),
  router = module.exports = express.Router(),
  config = require('../../config');

/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', {
    title: config.app.name
  });
});
