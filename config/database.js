(function () {
  'use strict';

  var mongoose = require('mongoose'),
    config = require('./');

  mongoose.connect(config.mongoose.dbURL, config.mongoose.options || {});
  mongoose.set('debug', config.mongoose.debug || false);

})();
