'use strict';

var router = require('express').Router(),
  crud = require('../util/crud'),
  Child = require('./child-model'),
  ChildCtrl = require('./child-controller'),
  HttpError = require('../util/http-error');

crud(router, Child, {
  userRestrict: true,
  path: '/child',
  include: ['create', 'read', 'update', 'delete', 'query']
});

router.delete('/child/:id', function(req, res, next) {
  ChildCtrl.deleteChild(req.user._id, req.params.id, function(err, data) {
    if (err) {
      return next(err);
    }
    if (!data) {
      return next(new HttpError(404, 'Child not found.'));
    }
    return res.status(200).end();
  });
});

router.post('/child/:id/tasks', function(req, res, next) {
  ChildCtrl.updateTasks(req.user._id, req.params.id, req.body, function(err, data) {
    if (err) {
      return next(err);
    }
    if (!data) {
      return next(new HttpError(404, 'Child not found.'));
    }
    return res.json(data);
  });
});

module.exports = router;
