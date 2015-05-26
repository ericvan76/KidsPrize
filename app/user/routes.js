'use strict';

var router = require('express').Router();

router.get('/user', function(req, res, next) {
  return res.json(req.user);
});

module.exports = router;
