'use strict';

var configPath = process.cwd() + '/config/' + (process.env.NODE_ENV || 'development');
module.exports = require(configPath);
