'use strict';

var Token = require('./token'),
  uuid = require('node-uuid'),
  base64 = require('js-base64').Base64,
  config = require('../../config');

exports.issueToken = function(userId, clientId, session, state, callback) {
  if (clientId === 'webapp' && (session === undefined || session === null)) {
    return callback(new Error('Unauthorised.'));
  }
  var expires_in_hours = 4;
  var access_token = base64.encode(uuid.v4());
  var now = new Date();
  var expire_at = new Date(now.getTime());
  expire_at.setHours(expire_at.getHours() + expires_in_hours);

  Token.findOneAndUpdate({
    _user: userId,
    client_id: clientId,
    session: session
  }, {
    $set: {
      access_token: access_token,
      expire_at: expire_at,
      refresh_at: now
    },
    $setOnInsert: {
      _user: userId,
      client_id: clientId,
      session: session,
      token_type: 'bearer',
      open_at: now
    }
  }, {
    new: true,
    upsert: true
  }, function(err, t) {
    if (err) {
      return callback(err);
    }
    if (!t) {
      return callback(null, false);
    }
    return callback(null, {
      token_type: t.token_type,
      access_token: t.access_token,
      expires_in: expires_in_hours * 3600,
      state: state
    });
  });
};

exports.validateToken = function(token, callback) {
  Token.findOne({
    access_token: token,
    expire_at: {
      $gt: new Date()
    }
  }, function(err, t) {
    if (err) {
      return callback(err);
    }
    if (!t) {
      return callback(null, false);
    }
    t.populate('_user', function(err, t) {
      if (err) {
        return callback(err);
      }
      if (!t._user) {
        return callback(null, false);
      }
      return callback(null, t._user);
    });
  });
};

exports.revokeToken = function(token, callback) {
  Token.remove({
    access_token: token
  }, callback);
};
