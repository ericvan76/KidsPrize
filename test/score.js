'use strict';

var assert = require('assert'),
  async = require('async'),
  config = require('./config'),
  crud = require('../app/util/crud'),
  User = require('../app/user/user-model'),
  UserCtrl = require('../app/user/user-controller'),
  Child = require('../app/child/child-model'),
  ChildCtrl = require('../app/child/child-controller'),
  Score = require('../app/score/score-model'),
  ScoreCtrl = require('../app/score/score-controller');

require('date-utils');

var user = null;
var child = null;

describe('ScoreController', function() {

  before(function(done) {
    config.db.connect();
    UserCtrl.resolveUser({
      email: 'test-user@domain.com'
    }, function(err, u) {
      assert.ifError(err);
      user = u;
      Child.create({
        _user: u._id,
        name: 'TestChild',
        tasks: ['task1', 'task2', 'task3']
      }, function(err, c) {
        assert.ifError(err);
        child = c;
        // generate scores
        var today = new Date(Date.today().toYMD());
        var scores = [];
        for (var i = -7; i <= 7; i += 7) {
          var day = today.clone().add({
            days: i
          });
          for (var j = 1; j <= 4; j++) {
            scores.push({
              date: day,
              task: 'task' + j,
              value: j
            });
          }
        }
        async.each(scores, function(item, cb) {
          Score.create({
            _user: user._id,
            _child: child._id,
            date: item.date,
            task: item.task,
            value: item.value
          }, function(err, r) {
            assert.ifError(err);
            cb(null);
          });
        }, function(err) {
          assert.ifError(err);
          done();
        });
      });
    });
  });

  after(function() {
    config.db.clean();
  });
  beforeEach(function() {});
  afterEach(function() {});

  describe('total', function() {
    it('calculate total', function(done) {
      ScoreCtrl.total(user._id, child._id, function(err, r) {
        assert.ifError(err);
        assert(r);
        assert.equal(r.total, 30);
        done();
      });
    });

    it('calculate total on non-existing child', function(done) {
      ScoreCtrl.total(user._id, user._id, function(err, r) {
        assert.ifError(err);
        assert.equal(r, false);
        done();
      });
    });
  });

  describe('cleanup', function() {
    it('clean up invalid scores then total', function(done) {
      ScoreCtrl.cleanup(user._id, child._id, new Date(Date.today().toYMD()), function(err, r) {
        assert.ifError(err);
        ScoreCtrl.total(user._id, child._id, function(err, r) {
          assert.ifError(err);
          assert(r);
          assert.equal(r.total, 22);
          done();
        });
      });
    });
  });
});
