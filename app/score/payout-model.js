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
  description: {
    type: String,
    required: true,
  },
  value: {
    type: Number,
    required: true,
    min: 0
  },
  update_at: {
    type: Date,
    required: true,
    default: Date.now
  }
});
