'use strict';

var Payout = require('./payout-model'),
  Child = require('../child/child-model'),
  HttpError = require('../util/http-error');

/**
 * Gets total payouts of given child
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
    Payout.aggregate([{
      $match: {
        _user: userId,
        _child: child._id
      }
    }, {
      $group: {
        _id: null,
        total: {
          $sum: '$amount'
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
