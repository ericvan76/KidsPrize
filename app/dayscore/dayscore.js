'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;

var DayScoreSchema = new Schema({
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
  scores: [{
    task: {
      type: String,
      required: true,
    },
    score: {
      type: Number,
      default: 0
    },
    update_at: {
      type: Date,
      default: new Date()
    }
  }],
  update_at: {
    type: Date,
    default: new Date()
  }
});

DayScoreSchema.index({
  _child: 1,
  date: 1
}, {
  unique: true
});

// total
DayScoreSchema.virtual('total').get(function() {
  return this.scores.reduce(function(prev, curr) {
    return {
      score: prev.score + curr.score
    };
  }).score;
});

// exports
module.exports = mongoose.model('DayScore', DayScoreSchema);
