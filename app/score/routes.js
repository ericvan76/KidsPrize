'use strict';

require('date-utils');

var base64 = require('js-base64').Base64;

var router = require('express').Router(),
  crud = require('../util/crud'),
  Score = require('./score-model'),
  ScoreCtrl = require('./score-controller'),
  Payment = require('./payment-model'),
  PaymentCtrl = require('./payment-controller');

// Scores
crud(router, Score, {
  userRestrict: true,
  path: '/score',
  include: ['create', 'read', 'patch', 'delete', 'query']
});


router.get('/score/total/:childId', function(req, res, next) {
  if (!req.params.childId) {
    return res.status(400).end();
  }
  ScoreCtrl.total(req.user._id, req.params.childId, function(err, data) {
    if (err) {
      return next(err);
    }
    return res.json(data);
  });
});

router.post('/score/cleanup', function(req, res, next) {
  if (!req.body.childId || !req.body.from) {
    return res.status(400).end();
  }
  ScoreCtrl.cleanup(req.user._id, req.body.childId, req.body.from, function(err) {
    if (err) {
      return next(err);
    }
    return res.status(200).end();
  });
});


// Payments
crud(router, Payment, {
  userRestrict: true,
  path: '/payment',
  include: ['create', 'read', 'patch', 'delete', 'query']
});

router.get('/payment/total/:childId', function(req, res, next) {
  if (!req.params.childId) {
    return res.status(400).end();
  }
  PaymentCtrl.total(req.user._id, req.params.childId, function(err, data) {
    if (err) {
      return next(err);
    }
    return res.json(data);
  });
});


module.exports = router;
