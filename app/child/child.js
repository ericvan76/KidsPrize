'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;

var ChildSchema = new Schema({
  _user: {
    type: Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  name: {
    type: String,
    required: true
  },
  gender: {
    type: String,
    enum: ['m', 'f', null]
  },
  tasks: [String],
  update_at: {
    type: Date,
    default: new Date()
  }
});

module.exports = mongoose.model("Child", ChildSchema);
