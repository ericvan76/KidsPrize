'use strict';

var HttpError = require('../util/http-error'),
  ChildCtrl = require('./child-controller'),
  router = require('../util/crud').Router(ChildCtrl);

router.post('/child/:id/tasks', function (req, res, next) {
  ChildCtrl.updateTasks(req.user._id, req.params.id, req.body, function (err, data) {
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
