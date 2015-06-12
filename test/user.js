'use strict';

var assert = require('assert'),
  config = require('./config'),
  User = require('../app/user/user-model'),
  UserCtrl = require('../app/user/user-controller');

describe('UserController', function() {
  before(function() {
    config.db.connect();
  });
  after(function() {
    config.db.clean();
  });
  beforeEach(function() {});
  afterEach(function() {});

  describe('normaliseUser()', function() {
    it('normalise facebook profile', function() {
      var profile = {
        emails: [{
          value: 'Test-User@domain.com'
        }],
        name: {
          familyName: 'Test',
          givenName: 'User'
        },
        displayName: 'Test User'
      };
      var normUser = UserCtrl.normaliseUser('facebook', profile);
      assert.equal(normUser.email, 'test-user@domain.com');
      assert.equal(normUser.displayName, profile.displayName);
      assert.equal(normUser.name.familyName, profile.name.familyName);
      assert.equal(normUser.name.givenName, profile.name.givenName);
      assert.deepEqual(normUser.facebook, profile);
    });
    it('normalise google profile', function() {
      var profile = {
        emails: [{
          type: 'account',
          value: 'Test-User@domain.com'
        }],
        name: {
          familyName: 'Test',
          givenName: 'User'
        },
        displayName: 'Test User'
      };
      var normUser = UserCtrl.normaliseUser('google', profile);
      assert.equal(normUser.email, 'test-user@domain.com');
      assert.equal(normUser.displayName, profile.displayName);
      assert.equal(normUser.name.familyName, profile.name.familyName);
      assert.equal(normUser.name.givenName, profile.name.givenName);
      assert.deepEqual(normUser.google, profile);
    });
  });

  describe('resolveUser()', function() {
    var user = {
      email: 'test-user@domain.com',
      name: {
        familyName: 'Test',
        givenName: 'User'
      },
      displayName: 'Test User',
      facebook: {
        'testField': 'A'
      }
    };
    var userId = null;
    it('new user', function(done) {
      UserCtrl.resolveUser(user, function(err, u) {
        assert.ifError(err);
        assert(u._id);
        userId = u._id;
        assert.equal(u.email, user.email);
        assert.equal(u.displayName, user.displayName);
        assert.equal(u.name.familyName, user.name.familyName);
        assert.equal(u.name.givenName, user.name.givenName);
        assert.equal(u.facebook.testField, user.facebook.testField);
        done();
      });
    });
    it('existing user', function(done) {
      user.name.givenName = 'User2';
      user.displayName = 'Test User2';
      user.facebook.testField = 'B';
      UserCtrl.resolveUser(user, function(err, u) {
        assert.ifError(err);
        assert(u._id);
        assert.equal(u._id.toString(), userId.toString());
        assert.equal(u.email, user.email);
        assert.equal(u.displayName, user.displayName);
        assert.equal(u.name.familyName, user.name.familyName);
        assert.equal(u.name.givenName, user.name.givenName);
        assert.equal(u.facebook.testField, user.facebook.testField);
        done();
      });
    });
  });

  describe('savePreference()', function() {
    var user = {
      email: 'test-user3@domain.com'
    };
    it('save new preferences', function(done) {
      UserCtrl.resolveUser(user, function(err, u) {
        assert.ifError(err);
        UserCtrl.savePreference(u.id, {
          preference1: 'abc',
          preference2: true
        }, function(err, u2) {
          assert.ifError(err);
          UserCtrl.resolveUser(user, function(err, u3) {
            assert.ifError(err);
            assert.equal(u3.preference.preference1, 'abc');
            assert.equal(u3.preference.preference2, true);
            done();
          });
        });
      });
    });
    it('update existing preferences', function(done) {
      UserCtrl.resolveUser(user, function(err, u) {
        assert.ifError(err);
        UserCtrl.savePreference(u.id, {
          preference1: 'cde',
          preference2: false,
        }, function(err, u2) {
          assert.ifError(err);
          UserCtrl.resolveUser(user, function(err, u3) {
            assert.ifError(err);
            assert.equal(u3.preference.preference1, 'cde');
            assert.equal(u3.preference.preference2, false);
            done();
          });
        });
      });
    });
    it('add & update preferences', function(done) {
      UserCtrl.resolveUser(user, function(err, u) {
        assert.ifError(err);
        UserCtrl.savePreference(u.id, {
          preference1: 'fgh',
          preference3: 99
        }, function(err, u2) {
          assert.ifError(err);
          UserCtrl.resolveUser(user, function(err, u3) {
            assert.ifError(err);
            assert.equal(u3.preference.preference1, 'fgh');
            assert.equal(u3.preference.preference3, 99);
            done();
          });
        });
      });
    });

    it('save empty shouldn\'t affect existing ones', function(done) {
      UserCtrl.resolveUser(user, function(err, u) {
        assert.ifError(err);
        UserCtrl.savePreference(u.id, {}, function(err, u2) {
          assert.ifError(err);
          UserCtrl.resolveUser(user, function(err, u3) {
            assert.ifError(err);
            assert.equal(u3.preference.preference1, 'fgh');
            assert.equal(u3.preference.preference2, false);
            assert.equal(u3.preference.preference3, 99);
            done();
          });
        });
      });
    });
  });

});
