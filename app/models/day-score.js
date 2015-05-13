'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;
// Day
var DayScoreSchema = new Schema({
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

// methods
DayScoreSchema.statics.listScore = function(childId, callback) {
  exports.find({
    _child: childId
  }, callback);
};

DayScoreSchema.statics.setScore = function(childId, date, task, score, callback) {
  exports.findOneAndUpdate({
      _child: childId,
      date: date
    }, {
      $setOnInsert: {
        date: date
      }
    }, {
      new: true,
      upsert: true
    },
    function(err, dayScore) {
      if (err) {
        callback(err);
      } else {
        try {
          // find existing task entry
          var existing = dayScore.scores.filter(function(s) {
            return s.task.toLowerCase() === task.toLowerCase();
          }).pop();

          if (existing) {
            if (existing.score !== score) {
              // update task's score
              mongoose.model('DayScore').update({
                _id: dayScore._id,
                __v: dayScore.__v,
                'scores.task': task
              }, {
                $set: {
                  'scores.$.score': score,
                  'scores.$.update_at': new Date(),
                  update_at: new Date()
                },
                $inc: {
                  __v: 1
                }
              }, function(err, n) {
                if (err) {
                  callback(err);
                } else {
                  mongoose.model('DayScore').findById(dayScore._id, callback);
                }
              });
            } else {
              callback(null, dayScore);
            }
          } else {
            // insert task's 1st score
            mongoose.model('DayScore').update({
              _id: dayScore._id,
              __v: dayScore.__v
            }, {
              $addToSet: {
                scores: {
                  task: task,
                  score: score
                }
              },
              $set: {
                update_at: new Date()
              },
              $inc: {
                __v: 1
              }
            }, function(err, n) {
              if (err) {
                callback(err);
              } else {
                mongoose.model('DayScore').findById(dayScore._id, callback);
              }
            });
          }
        } catch (e) {
          callback(e);
        }
      }
    }
  );
};

// exports
module.exports = mongoose.model('DayScore', DayScoreSchema);
