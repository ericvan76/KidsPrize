'use strict';

var Payment = require('./payment-model'),
  Child = require('../child/child-model');

/**
 * Gets total payments of given child
 * @param  {ObjectId} userId
 * @param  {ObjectId} childId
 * @param  {Function} callback
 */
exports.total = function(userId, childId, callback) {
  if (userId === null) {
    return callback(new Error('Unauthorised.'));
  }
  Child.findOne({
    _user: userId,
    _id: childId
  }, function(err, child) {
    if (err) {
      return callback(err);
    }
    if (!child) {
      return callback(null, false);
    }
    Payment.aggregate([{
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
