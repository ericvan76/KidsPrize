'use strict';

require('date-utils');

var base64 = require('js-base64').Base64;

var router = require('express').Router(),
  crud = require('../util/crud'),
  Score = require('./score-model'),
  ScoreCtrl = require('./score-controller'),
  Payout = require('./payout-model'),
  PayoutCtrl = require('./payout-controller');

// Scores
crud(router, Score, {
  userRestrict: true,
  path: '/score',
  include: ['create', 'read', 'patch', 'delete', 'query']
});


router.get('/score/total/:childId',
  function(req, res, next) {
    if (!req.params.childId) {
      return res.status(400).end();
    }
    ScoreCtrl.total(req.user._id, req.params.childId, function(err, data) {
      if (err) {
        console.log(err);
        return next(err);
      }
      return res.json(data);
    });
  });


// Payouts
crud(router, Payout, {
  userRestrict: true,
  path: '/payout',
  include: ['create', 'read', 'patch', 'delete', 'query']
});

router.get('/payout/total/:childId',
  function(req, res, next) {
    if (!req.params.childId) {
      return res.status(400).end();
    }
    PayoutCtrl.total(req.user._id, req.params.childId, function(err, data) {
      if (err) {
        console.log(err);
        return next(err);
      }
      return res.json(data);
    });
  });


module.exports = router;
