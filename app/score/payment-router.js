'use strict';

var HttpError = require('../util/http-error'),
  PaymentCtrl = require('./payment-controller'),
  router = require('../util/crud').Router(PaymentCtrl);

router.get('/payment/total/:childId', function(req, res, next) {
  if (!req.params.childId) {
    return next(new HttpError(400, 'Missing url parameter - childId'));
  }
  PaymentCtrl.total(req.user._id, req.params.childId, function(err, data) {
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
