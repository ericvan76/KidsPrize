(function() {
  'use strict';

  var log4js = require('log4js');

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
      filename: 'logs/file.log',
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
