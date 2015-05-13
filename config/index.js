'use strict';

var path = require('path'),
  configPath = path.join(__dirname,
    'env', (process.env.NODE_ENV || 'development'));

module.exports = require(configPath);
