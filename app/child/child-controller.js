'use strict';

var Child = require('./child-model'),
  Score = require('../score/score-model'),
  Payment = require('../score/payment-model'),
  HttpError = require('../util/http-error');

/**
 * delete a child and its records
 * @param  {ObjectId}   userId   [description]
 * @param  {ObjectId}   childId  [description]
 * @param  {Function} callback [description]
 */
exports.deleteChild = function(userId, childId, callback) {
  Child.findOneAndRemove({
    _user: userId,
    _id: childId
  }, function(err) {
    if (err) {
      return callback(err);
    }
    Score.remove({
      _user: userId,
      _child: childId
    }, function(err) {});
    Payment.remove({
      _user: userId,
      _child: childId
    }, function(err) {});
    return callback(null);
  });
};

/**
 * update task list of child
 * @param  {ObjectId}   userId   [description]
 * @param  {ObjectId}   childId  [description]
 * @param  {Array}   tasks    [description]
 * @param  {Function} callback [description]
 */
exports.updateTasks = function(userId, childId, tasks, callback) {
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
      return callback(new HttpError(404, 'Child not found.'));
    }
    return callback(null, child.tasks);
  });
};
