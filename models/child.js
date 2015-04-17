'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;
var ask;

var ChildSchema = new Schema({
  name: {
    type: String,
    required: true
  },
  tasks: [{
    name: {
      type: String,
      require: true
    },
    min_score: {
      type: Number,
      default: 0
    },
    max_score: {
      type: Number,
      default: 1
    }
  }],
  update_at: {
    type: Date,
    default: new Date()
  }
});

module.exports.Child = mongoose.model("Child", ChildSchema);
