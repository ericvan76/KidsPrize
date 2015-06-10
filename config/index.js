'use strict';

var merge = require('merge'),
  path = require('path'),
  cfg = require('./env/default'),
  cfg2 = require(path.join(__dirname, 'env', (process.env.NODE_ENV || 'development')));

module.exports = merge.recursive(true, cfg, cfg2);
