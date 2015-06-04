'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;

var PayoutSchema = new Schema({
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
  amount: {
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

// exports
module.exports = mongoose.model('Payout', PayoutSchema);
