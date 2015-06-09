'use strict';

var cluster = require('cluster'),
  config = require('./config');

// configure log4js
require('./config/log');

var numCPUs = config.numCPUs || require('os').cpus().length;

if (numCPUs > 1 && cluster.isMaster) {

  // Fork workers.
  for (var i = 0; i < numCPUs; i++) {
    cluster.fork();
  }

  cluster.on('exit', function(worker, code, signal) {
    console.log('worker %d died (%s). restarting...',
      worker.process.pid, signal || code);
    cluster.fork();
  });

} else {

  // initialise database
  require('./config/database');

  // create http server
  var express = require('./app/express');
  var port = process.env.PORT || config.port || 3000;
  express.set('port', port);

  var server = require('http').createServer(express);
  server.on('listening', function() {
    console.log('Listening on ' + port);
  });
  // listen
  server.listen(port);

}
