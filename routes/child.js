'use strict';

var express = require('express');
var router = express.Router();

var Child = require('../models/child').Child;
var DayScore = require('../models/day-score').DayScore;

router.get('/', function(req, res, next) {
  Child.find(function(err, children) {
    if (err) {
      next(err);
    } else {
      res.json(children);
    }
  });
});

router.get('/:id', function(req, res, next) {
  Child.findById(req.params.id, function(err, child) {
    if (err) {
      next(err);
    } else {
      if (!child) {
        res.status(404).end();
      } else {
        res.json(child);
      }
    }
  });
});

router.get('/:id/score', function(req, res, next) {
  DayScore.listScore(req.params.id, function(err, scores) {
    if (err) {
      next(err);
    } else {
      res.json(scores);
    }
  });
});

router.post('/:id/score', function(req, res, next) {
  if (!req.body || !req.body.date || !req.body.task || !req.body.score) {
    res.status(400).end();
  } else {
    DayScore.setScore(req.params.id, req.body.date, req.body.task, req.body.score, function(err, scores) {
      if (err) {
        next(err);
      } else {
        res.json(scores);
      }
    });
  }
});

module.exports = router;
