'use strict';

exports.index = function(req, res) {
  res.render('index', {
    user: req.user
  });
};

exports.partials = function(req, res) {
  var name = req.params.name;
  res.render('partials/' + name);
};

exports.auth = require('./auth');
exports.api = require('./api');

