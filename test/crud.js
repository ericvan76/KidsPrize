'use strict';

var assert = require('assert'),
  config = require('../config/env/test'),
  mongoose = require('mongoose'),
  Schema = mongoose.Schema,
  router = require('express').Router(),
  crud = require('../app/util/crud'),
  UserCtrl = require('../app/user/user-controller');

var TestModel = mongoose.model('TestModel', new Schema({
  _user: {
    type: Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  strField: String,
  boolField: Boolean,
  numField: Number,
  dateField: Date,
  objField: {},
  arrField: [String],
  update_at: {
    type: Date,
    required: true,
    default: Date.now
  }
}));

crud(router, TestModel, {
  userRestrict: true,
  path: '/test-model',
  include: ['create', 'read', 'patch', 'delete', 'query']
});

var Ctrl = router.__crud_controller;

describe('Crud', function() {

  var user = null;
  var user2 = null;
  var created = null;
  var updated = null;

  var testModel = {
    strField: 'abc',
    numField: 1,
    boolField: true,
    dateField: Date.now(),
    objField: {
      x: 1,
      y: 2,
      z: 3
    },
    arrField: ['a', 'b', 'c']
  };

  before(function(done) {
    if (!mongoose.connection.readyState) {
      mongoose.connect(config.mongoose.dbURL);
    }
    UserCtrl.resolveUser({
      email: 'test-user@domain.com'
    }, function(err, u) {
      user = u;
      UserCtrl.resolveUser({
        email: 'test-user2@domain.com'
      }, function(err, u2) {
        user2 = u2;
        done();
      });
    });
  });

  after(function() {
    mongoose.connection.db.dropDatabase();
  });

  beforeEach(function() {});
  afterEach(function() {});


  describe('#create()', function() {
    it('create test model', function(done) {
      Ctrl.create(user._id, testModel, function(err, d) {
        assert.ifError(err);
        created = d;
        assert(d._id);
        assert.equal(d._user, user.id);
        assert(d.update_at);
        done();
      });
    });
  });

  describe('#read()', function() {
    it('read test model', function(done) {
      Ctrl.read(user._id, created.id, function(err, d) {
        assert.ifError(err);
        assert.equal(d.id, created.id);
        assert.equal(d._user, user.id);
        assert.equal(d.strField, created.strField);
        assert.equal(d.numField, created.numField);
        assert.equal(d.boolField, created.boolField);
        assert.equal(d.arrField.length, 3);
        assert.equal(d.dateField.getTime(), created.dateField.getTime());
        done();
      });
    });

    it('read test model as someone else should return 404', function(done) {
      Ctrl.read(user2._id, created.id, function(err, d) {
        assert(err);
        assert.equal(err.status, 404);
        done();
      });
    });

    it('read test model as no one should return 401', function(done) {
      Ctrl.read(null, created.id, function(err, d) {
        assert(err);
        assert.equal(err.status, 401);
        done();
      });
    });
  });

  describe('#patch()', function() {
    it('patch test model', function(done) {
      var m = {
        _id: created.id,
        strField: 'cde',
        numField: 2,
        objField: {
          x: 4,
          y: 5
        },
        arrField: [1, 2]
      };
      Ctrl.patch(user.id, created.id, m, function(err, d) {
        assert.ifError(err);
        updated = d;
        assert.equal(d.id, created.id);
        assert.equal(d.strField, m.strField);
        assert.equal(d.numField, m.numField);
        assert.equal(d.boolField, created.boolField);
        assert.equal(d.arrField.length, m.arrField.length);
        assert.equal(d.objField.x, m.objField.x);
        assert.equal(d.objField.y, m.objField.y);
        assert.equal(d.objField.z, m.objField.z);
        assert(d.update_at.getTime() > created.update_at.getTime());
        done();
      });
    });

    it('patch test model as someone else should return 404', function(done) {
      Ctrl.patch(user2._id, created.id, {}, function(err, d) {
        assert(err);
        assert.equal(err.status, 404);
        done();
      });
    });

    it('patch test model as no one should return 401', function(done) {
      Ctrl.patch(null, created.id, {}, function(err, d) {
        assert(err);
        assert.equal(err.status, 401);
        done();
      });
    });
  });

  describe('#query()', function() {
    it('query test model', function(done) {
      Ctrl.query(user._id, {}, function(err, d) {
        assert.ifError(err);
        assert(d.length > 0);
        assert.equal(d[0].id, created.id);
        done();
      });
    });

    it('query test model as someone else should return []', function(done) {
      Ctrl.query(user2._id, {}, function(err, d) {
        assert.ifError(err);
        assert(d);
        assert.equal(d.length, 0);
        done();
      });
    });

    it('query test model as no one should return 401', function(done) {
      Ctrl.query(null, {}, function(err, d) {
        assert(err);
        assert.equal(err.status, 401);
        done();
      });
    });
  });

  describe('#delete()', function() {
    it('delete test model', function(done) {
      Ctrl.delete(user._id, created.id, function(err, d) {
        assert.ifError(err);
        assert.equal(d.id, created.id);
        assert.equal(d._user, user.id);
        Ctrl.read(user._id, created.id, function(err, d) {
          assert(err);
          assert.equal(err.status, 404);
          done();
        });
      });
    });

    it('delete test model as someone else should return 404', function(done) {
      Ctrl.delete(user2._id, created.id, function(err, d) {
        assert(err);
        assert.equal(err.status, 404);
        done();
      });
    });

    it('delete test model as no one should return 401', function(done) {
      Ctrl.delete(null, created.id, function(err, d) {
        assert(err);
        assert.equal(err.status, 401);
        done();
      });
    });
  });
});
