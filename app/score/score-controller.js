'use strict';

var Score = require('./score-model'),
  Child = require('../child/child-model'),
  HttpError = require('../util/http-error');

/**
 * Gets total scores of given child
 * @param  {ObjectId} userId
 * @param  {ObjectId} childId
 * @param  {Function} callback
 */
exports.total = function(userId, childId, callback) {
  Child.findOne({
    _user: userId,
    _id: childId
  }, function(err, child) {
    if (err) {
      return callback(err);
    }
    if (!child) {
      return callback(new HttpError(404, 'Child not found.'));
    }
    var td = Date.UTCtoday();
    var thisWeek = td.addHours(-24 * td.getDay());
    Score.aggregate([{
      $match: {
        _user: userId,
        _child: child._id,
        $or: [{
          date: {
            $lt: thisWeek
          }
        }, {
          task: {
            $in: child.tasks
          }
        }]
      }
    }, {
      $group: {
        _id: null,
        total: {
          $sum: '$value'
        }
      }
    }, {
      $project: {
        _id: 0,
        total: 1
      }
    }], function(err, data) {
      if (err) {
        return callback(err);
      }
      return callback(null, data[0] || {
        total: 0
      });
    });
  });
};
