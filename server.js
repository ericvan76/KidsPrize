'use strict';

var cluster = require('cluster'),
  config = require('./config');


if (cluster.isMaster) {

  var numCPUs = config.numCPUs || require('os').cpus().length;

  // Fork workers.
  for (var i = 0; i < numCPUs; i++) {
    cluster.fork();
  }

  cluster.on('exit', function (worker, code, signal) {
    console.log('worker %d died (%s). restarting...',
      worker.process.pid, signal || code);
    cluster.fork();
  });

} else {
 
  // connect to mongodb
  var mongoose = require('mongoose');
  mongoose.connect(config.mongoose.dbURL, config.mongoose.options || {});
  mongoose.set('debug', config.mongoose.debug || false);

  // create http server
  var app = require('./config/express');
  var port = process.env.PORT || 3000;
  app.set('port', port);

  var server = require('http').createServer(app);
  server.on('listening', function () {
    console.log('Listening on ' + port);
  });
  // listen
  server.listen(port);

}
