'use strict';

var router = require('express').Router(),
  crud = require('../util/crud'),
  Child = require('./child-model'),
  Score = require('../score/score-model');

crud(router, Child, {
  userRestrict: true,
  path: '/child',
  include: ['create', 'read', 'patch', 'query']
});

router.delete('/child/:id', function(req, res, next) {
  Child.findOneAndRemove({
    _user: req.user._id,
    _id: req.params.id
  }, function(err) {
    if (err) {
      return next(err);
    }
    Score.remove({
      _user: req.user._id,
      _child: req.params.id
    }, function(err) {
      if (err) {
        return next(err);
      }
      res.status(200).end();
    });
  });
});

router.post('/child/:id/tasks', function(req, res, next) {
  Child.findOneAndUpdate({
    _user: req.user._id,
    _id: req.params.id
  }, {
    $set: {
      tasks: req.body
    }
  }, {
    new: true,
    upsert: false
  }, function(err, child) {
    if (err) {
      return next(err);
    }
    if (!child) {
      return res.status(404).send('Child not found.');
    }
    var td = Date.UTCtoday();
    var thisWeek = td.addHours(-24 * td.getDay());
    Score.remove({
      _user: req.user._id,
      _child: child._id,
      date: {
        $gte: thisWeek
      },
      task: {
        $nin: child.tasks
      }
    }, function(err) {
      if (err) {
        return next(err);
      }
      return res.json(child.tasks);
    });

  });
});

module.exports = router;
