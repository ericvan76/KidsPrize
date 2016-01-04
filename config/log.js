(function () {
  'use strict';

  var log4js = require('log4js'),
    config = require('./');

  log4js.configure({
    appenders: [{
      type: 'console',
      category: 'console',
      layout: {
        type: 'pattern',
        pattern: "%p - %m"
      }
    }, {
        type: 'dateFile',
        filename: config.log.filename,
        pattern: '-yyyy-MM-dd',
        alwaysIncludePattern: false,
        category: 'console',
        layout: {
          type: 'pattern',
          pattern: '%d %-5p - %m'
        }
      }],
    levels: {
      console: 'INFO'
    },
    replaceConsole: true
  });

})();
