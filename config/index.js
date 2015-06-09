'use strict';

var merge = require('merge'),
  path = require('path'),
  defaultPath = path.join(__dirname, 'env/default'),
  configPath = path.join(__dirname,
    'env', (process.env.NODE_ENV || 'development'));

module.exports = merge.recursive(true, require(defaultPath), require(configPath));
