'use strict';

var Score = require('./score'),
  crud = require('../crud');

var router = crud(Score, {
  userRestrict: true,
  path: '/score',
  include: ['create', 'read', 'update', 'patch', 'delete', 'query']
}).router;

module.exports = router;
