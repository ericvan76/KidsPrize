'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;

var ScoreSchema = new Schema({
  _user: {
    type: Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  _child: {
    type: Schema.Types.ObjectId,
    ref: 'Child',
    required: true
  },
  date: {
    type: Date,
    required: true
  },
  task: {
    type: String,
    required: true,
  },
  value: {
    type: Number,
    default: 0
  },
  order: {
    type: Number,
    default: 0
  },
  update_at: {
    type: Date,
    required: true,
    default: Date.now
  }
});

ScoreSchema.index({
  _child: 1,
  date: 1,
  task: 1
}, {
    unique: true
  });

// exports
module.exports = mongoose.model('Score', ScoreSchema);
