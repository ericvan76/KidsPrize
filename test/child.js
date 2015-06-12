'use strict';

var assert = require('assert'),
  config = require('./config'),
  crud = require('../app/util/crud'),
  User = require('../app/user/user-model'),
  UserCtrl = require('../app/user/user-controller'),
  Child = require('../app/child/child-model'),
  ChildCtrl = require('../app/child/child-controller'),
  Score = require('../app/score/score-model'),
  Payment = require('../app/score/payment-model');

describe('ChildController', function() {

  var user = null;
  var child = null;

  before(function(done) {
    config.db.connect();
    UserCtrl.resolveUser({
      email: 'test-user@domain.com'
    }, function(err, u) {
      assert.ifError(err);
      user = u;
      Child.create({
        _user: u._id,
        name: 'TestChild'
      }, function(err, c) {
        assert.ifError(err);
        child = c;
        Score.create({
          _user: user._id,
          _child: child._id,
          date: Date.now(),
          task: 'task1',
          value: 1
        }, function(err, r) {
          assert.ifError(err);
          Payment.create({
            _user: user._id,
            _child: child._id,
            description: 'test payment',
            amount: 1
          }, function(err, r2) {
            assert.ifError(err);
            done();
          });
        });
      });
    });
  });
  after(function() {
    config.db.clean();
  });
  beforeEach(function() {});
  afterEach(function() {});

  describe('updateTasks', function() {
    var testcases = [
      ['task1', 'task2'],
      ['task1', 'task2', 'task3', 'task4'],
      ['task1', 'task4'],
      ['task1', 'task2', 'task3']
    ];

    function testFunc(testcase) {
      return function(done) {
        ChildCtrl.updateTasks(user._id, child._id, testcase, function(err, r) {
          assert.ifError(err);
          assert.equal(r.length, testcase.length);
          for (var i = 0; i < r.length; i++) {
            assert.equal(r[i], testcase[i]);
          }
          Child.findById(child._id, function(err, c) {
            assert.ifError(err);
            child = c;
            assert.equal(c.tasks.length, testcase.length);
            assert.equal(r.length, testcase.length);
            for (var i = 0; i < c.tasks.length; i++) {
              assert.equal(c.tasks[i], testcase[i]);
            }
            done();
          });
        });
      };
    }

    for (var i = 0; i < testcases.length; i++) {
      var tc = testcases[i];
      it('update tasks - [' + tc.toString() + ']', testFunc(tc));
    }
  });

  describe('deleteChild', function() {
    it('deleteChild', function(done) {
      ChildCtrl.deleteChild(user._id, child.id, function(err, r) {
        assert.ifError(err);
        assert.equal(r.id, child.id);
        assert.equal(r._user, user.id);
        Child.findById(child._id, function(err, c) {
          assert.ifError(err);
          assert.equal(c, null);
          Score.find({
            _child: child._id
          }, function(err, s) {
            assert.ifError(err);
            assert(s.length === 0);
            Payment.find({
              _child: child._id
            }, function(err, p) {
              assert.ifError(err);
              assert(p.length === 0);
              done();
            });
          });
        });
      });
    });
  });
});
