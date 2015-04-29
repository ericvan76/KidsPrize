'use strict';

var cluster = require('cluster'),
  debug = require('debug')('kids-reward:server');

if (cluster.isMaster) {
  // In real life, you'd probably use more than just 2 workers,
  // and perhaps not put the master and worker in the same file.
  cluster.fork();
  // cluster.fork();

  cluster.on('disconnect', function(worker) {
    console.error('disconnect!');
    cluster.fork();
  });

} else {
  // the worker
  var config = require('./config'),
    port = normalizePort(process.env.PORT || '3000');

  // create domain
  var domain = require('domain').create();
  var bind = typeof port === 'string' ? 'pipe ' + port : 'port ' + port;
  domain.on('error', function(err) {
    switch (err.code) {
      case 'EACCES':
        console.error(bind + ' requires elevated privileges');
        process.exit(1);
        break;
      case 'EADDRINUSE':
        console.error(bind + ' is already in use');
        process.exit(1);
        break;
      default:
        throw err;
    }
  });

  domain.run(function() {

    var mongoose = require('mongoose'),
      http = require('http'),
      app = require('./config/express'),
      config = require('./config');

    // connect to mongodb
    mongoose.connect(config.mongoose.dbURL, config.mongoose.options || {});
    mongoose.set('debug', config.mongoose.debug || false);

    // create server
    var server = http.createServer(app);
    server.on('listening', function() {
      var addr = server.address();
      var bind = typeof addr === 'string' ? 'pipe ' + addr : 'port ' + addr.port;
      console.log('Listening on ' + bind);
    });
    app.set('port', port);
    // listen
    server.listen(port);
  });
}

function normalizePort(val) {
  var port = parseInt(val, 10);
  if (isNaN(port)) {
    // named pipe
    return val;
  }
  if (port >= 0) {
    // port number
    return port;
  }
  return false;
}
