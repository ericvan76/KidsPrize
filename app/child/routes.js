'use strict';

var Child = require('./child'),
  crud = require('../crud');

var router = crud(Child, {
  userRestrict: true,
  path: '/child',
  include: ['create', 'read', 'update', 'patch', 'delete', 'query']
}).router;

router.post('/child/:id/tasks', function(req, res, next) {
  Child.findOneAndUpdate({
    _user: req.user._id,
    _id: req.params.id
  }, {
    $set: {
      tasks: req.body
    }
  }, {
    new: true,
    upsert: false
  }, function(err, child) {
    if (err) {
      return next(err);
    }
    if (!child) {
      return res.status(404).send('Child not found.');
    }
    return res.json(child.tasks);
  });
});

module.exports = router;
