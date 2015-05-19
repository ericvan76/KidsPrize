'use strict';

var Child = require('./child'),
  crud = require('../crud');

var controller = crud.controller(Child, {
  exclude: ['create']
});

var router = crud.router(controller, {
  exclude: ['create']
});

module.exports = router;
