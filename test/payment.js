'use strict';

var assert = require('assert'),
  async = require('async'),
  config = require('./config'),
  crud = require('../app/util/crud'),
  User = require('../app/user/user-model'),
  UserCtrl = require('../app/user/user-controller'),
  Child = require('../app/child/child-model'),
  ChildCtrl = require('../app/child/child-controller'),
  Payment = require('../app/score/payment-model'),
  PaymentCtrl = require('../app/score/payment-controller');

require('date-utils');

var user = null;
var child = null;

describe('PaymentController', function() {

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
        var payments = [];
        for (var i = 0; i < 10; i++) {
          payments.push({
            description: 'reward' + i,
            amount: 1
          });
        }
        async.each(payments, function(item, cb) {
          Payment.create({
            _user: user._id,
            _child: child._id,
            description: item.description,
            amount: item.amount
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
      PaymentCtrl.total(user._id, child._id, function(err, r) {
        assert.ifError(err);
        assert(r);
        assert.equal(r.total, 10);
        done();
      });
    });

    it('calculate total on non-existing child', function(done) {
      PaymentCtrl.total(user._id, user._id, function(err, r) {
        assert.ifError(err);
        assert.equal(r, false);
        done();
      });
    });

  });

});
