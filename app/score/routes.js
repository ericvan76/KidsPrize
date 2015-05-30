'use strict';

require('date-utils');

var Score = require('./score'),
  Child = require('../child/child'),
  crud = require('../crud');

var router = crud(Score, {
  userRestrict: true,
  path: '/score',
  include: ['create', 'read', 'update', 'patch', 'delete', 'query']
}).router;

router.post('/score/cleanup', function(req, res, next) {
  if (!req.body._child) {
    return res.status(400).send('_child is required.');
  }
  console.log(req.user._id);
  console.log(req.body._child);
  Child.findOne({
    _user: req.user._id,
    _id: req.body._child
  }, function(err, child) {
    if (err) {
      return next(err);
    }
    if (!child) {
      return res.status(404).send('Child Not Found');
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
    });
  });
});

module.exports = router;
