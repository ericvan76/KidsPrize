'use strict';

var mongoose = require('mongoose'),
  dbURL = 'mongodb://127.0.0.1:27017/kids-prize-test';

module.exports = {
  db: {
    connect: function() {
      if (!mongoose.connection.readyState) {
        mongoose.connect(dbURL);
      }
    },
    clean: function() {
      mongoose.connection.db.dropDatabase();
    }
  }
};
