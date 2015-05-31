'use strict';

var router = require('express').Router(),
  ctrl = require('./controller');

router.get('/user', function(req, res, next) {
  return res.json(req.user);
});

router.post('/user/preference', function(req, res, next) {
  return ctrl.savePreference(req.user._id, req.body, function(err, user) {
    if (err) {
      return next(err);
    }
    if (!user) {
      return res.status(404).send('User not found.');
    }
    return res.json(user.preference);

  });
});

module.exports = router;
