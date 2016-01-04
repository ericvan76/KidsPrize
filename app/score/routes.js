'use strict';

var router = require('express').Router();

router.use(require('./score-router'));
router.use(require('./payment-router'));

module.exports = router;
