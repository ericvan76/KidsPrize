'use strict';

var router = require('express').Router(),
  crud = require('../util/crud'),
  Child = require('./child-model'),
  ChildCtrl = require('./child-controller');

crud(router, Child, {
  userRestrict: true,
  path: '/child',
  include: ['create', 'read', 'patch', 'query']
});

router.delete('/child/:id', function(req, res, next) {
  ChildCtrl.deleteChild(req.user._id, req.params.id, function(err) {
    if (err) {
      return next(err);
    }
    return res.status(200).end();
  });
});

router.post('/child/:id/tasks', function(req, res, next) {
  ChildCtrl.updateTasks(req.user._id, req.params.id, req.body, function(err, data) {
    if (err) {
      return next(err);
    }
    return res.json(data);
  });
});

module.exports = router;
