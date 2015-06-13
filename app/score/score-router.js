'use strict';

var HttpError = require('../util/http-error'),
  ScoreCtrl = require('./score-controller'),
  router = require('../util/crud').Router(ScoreCtrl);

router.get('/score/total/:childId', function(req, res, next) {
  if (!req.params.childId) {
    return next(new HttpError(400, 'Missing url parameter - childId'));
  }
  ScoreCtrl.total(req.user._id, req.params.childId, function(err, data) {
    if (err) {
      return next(err);
    }
    if (!data) {
      return next(new HttpError(404, 'Child not found.'));
    }
    return res.json(data);
  });
});

router.post('/score/cleanup', function(req, res, next) {
  if (!req.body.childId || !req.body.from) {
    return next(new HttpError(400, 'Bad request.'));
  }
  ScoreCtrl.cleanup(req.user._id, req.body.childId, req.body.from, function(err, data) {
    if (err) {
      return next(err);
    }
    if (!data) {
      return next(new HttpError(404, 'Child not found.'));
    }
    return res.status(200).end();
  });
});

module.exports = router;
