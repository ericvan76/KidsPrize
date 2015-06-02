'use strict';

require('date-utils');

var base64 = require('js-base64').Base64;

var Score = require('./score-model'),
  Child = require('../child/child-model'),
  crud = require('../crud');

var router = crud(Score, {
  userRestrict: true,
  path: '/score',
  include: ['create', 'read', 'patch', 'delete', 'query']
}).router;

router.get('/score/total/:childId', function(req, res, next) {
  if (!req.params.childId) {
    return res.status(400).end();
  }
  Child.findOne({
    _user: req.user._id,
    _id: req.params.childId
  }, function(err, child) {
    if (err) {
      return next(err);
    }
    if (!child) {
      return res.status(404).end();
    }
    var td = Date.UTCtoday();
    var thisWeek = td.addHours(-24 * td.getDay());
    Score.aggregate([{
        $match: {
          _user: req.user._id,
          _child: child._id,
          $or: [{
            date: {
              $lt: thisWeek
            }
          }, {
            task: {
              $in: child.tasks
            }
          }]
        }
      }, {
        $group: {
          _id: null,
          total: {
            $sum: '$value'
          }
        }
      }, {
        $project: {
          _id: 0,
          total: 1
        }
      }],
      function(err, data) {
        if (err) {
          return next(err);
        }
        console.log(data);
        return res.json(data[0] || {
          total: 0
        });
      });
  });
});

module.exports = router;
