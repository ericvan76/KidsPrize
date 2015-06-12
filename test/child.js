'use strict';

var assert = require('assert'),
  config = require('./config'),
  crud = require('../app/util/crud'),
  User = require('../app/user/user-model'),
  UserCtrl = require('../app/user/user-controller'),
  Child = require('../app/child/child-model'),
  ChildCtrl = require('../app/child/child-controller'),
  Score = require('../app/score/score-model');

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
        done();
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

    function addSomeScores() {
      //TODO: Add test scores for last week, this week and next week
    }

    function verifyScores() {
      //TODO: Verify no existing scores in this week or future for removed tasks.
    }

    function testFunc(testcase) {
      return function(done) {
        addSomeScores();
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
            verifyScores();
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
});
