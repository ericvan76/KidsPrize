'use strict';

var Child = require('../child/child-model'),
  Score = require('./score-model'),
  controller = require('../util/crud').Controller(Score);

/**
 * Gets total scores of given child
 * @param  {ObjectId} userId
 * @param  {ObjectId} childId
 * @param  {Function} callback
 */
controller.total = function(userId, childId, callback) {
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
    Score.aggregate([{
      $match: {
        _user: userId,
        _child: child._id
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


/**
 * cleanup inactive scores after tasks updating.
 * @param  {ObjectId}   userId   [description]
 * @param  {ObjectId}   childId  [description]
 * @param  {Date}   from     [description]
 * @param  {Function} callback [description]
 */
controller.cleanup = function(userId, childId, from, callback) {
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
    Score.remove({
      _user: userId,
      _child: child._id,
      date: {
        $gte: from
      },
      task: {
        $nin: child.tasks
      }
    }, function(err) {
      if (err) {
        return callback(err);
      }
      return callback(null, true);
    });
  });

};

module.exports = controller;
