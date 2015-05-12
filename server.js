'use strict';

var cluster = require('cluster'),
  config = require('./config');

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
  var app = require('./app/express.js');
  var port = process.env.PORT || 3000;
  app.set('port', port);

  var server = require('http').createServer(app);
  server.on('listening', function() {
    console.log('Listening on ' + port);
  });
  // listen
  server.listen(port);

}
