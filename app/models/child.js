'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;

var ChildSchema = new Schema({
  name: {
    type: String,
    required: true
  },
  tasks: [{
    name: {
      type: String,
      require: true
    }
  }],
  update_at: {
    type: Date,
    default: new Date()
  }
});

module.exports = mongoose.model("Child", ChildSchema);
