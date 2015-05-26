'use strict';

var Child = require('./child'),
  crud = require('../crud');

var childCrud = crud(Child, {
  userRestrict: true,
  path: '/child',
  include: ['create', 'read', 'patch', 'delete', 'query']
});

module.exports = childCrud.router;
