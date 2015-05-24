'use strict';

var Score = require('./score'),
  crud = require('../crud');

var router = crud(Score, {
  userRestrict: true,
  path: '/score',
  include: ['read', 'query']
}).router;

// create or update
router.post('/score', function(req, res, next) {
  var sc = req.body;
  if (!sc._child || !sc.date || !sc.task) {
    return res.status(400).send('Bad Request');
  }
  Score.findOneAndUpdate({
    _user: req.user._id,
    _child: sc._child,
    date: sc.date,
    task: sc.task
  }, {
    $set: {
      value: sc.value,
      update_at: Date.now()
    },
    $setOnInsert: {
      _user: req.user._id,
      _child: sc._child,
      date: sc.date,
      task: sc.task
    }
  }, {
    new: true,
    upsert: true
  }, next);
});

module.exports = router;
