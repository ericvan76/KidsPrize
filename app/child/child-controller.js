'use strict';

var Child = require('./child-model'),
  Score = require('../score/score-model'),
  Payment = require('../score/payment-model'),
  controller = require('../util/crud').Controller(Child);

/**
 * delete a child and its records
 * @param  {ObjectId}   userId   [description]
 * @param  {ObjectId}   childId  [description]
 * @param  {Function} callback [description]
 */
controller.delete = function(userId, childId, callback) {
  if (userId === null) {
    return callback(new Error('Unauthorised.'));
  }
  Child.findOneAndRemove({
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
      _child: childId
    }, function(err) {
      if (err) {
        return callback(err);
      }
      Payment.remove({
        _user: userId,
        _child: childId
      }, function(err) {
        if (err) {
          return callback(err);
        }
        return callback(null, child);
      });
    });
  });
};

/**
 * update task list of child
 * @param  {ObjectId}   userId   [description]
 * @param  {ObjectId}   childId  [description]
 * @param  {Array}   tasks    [description]
 * @param  {Function} callback [description]
 */
controller.updateTasks = function(userId, childId, tasks, callback) {
  if (userId === null) {
    return callback(new Error('Unauthorised.'));
  }
  var uniqueTasks = tasks.filter(function(item, pos) {
    return tasks.indexOf(item) === pos;
  });
  Child.findOneAndUpdate({
    _user: userId,
    _id: childId
  }, {
    $set: {
      tasks: uniqueTasks
    }
  }, {
    new: true,
    upsert: false
  }, function(err, child) {
    if (err) {
      return callback(err);
    }
    if (!child) {
      return callback(null, false);
    }
    return callback(null, child.tasks);
  });
};

module.exports = controller;
