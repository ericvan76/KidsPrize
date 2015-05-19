'use static';

var Token = require('./token'),
  uuid = require('node-uuid'),
  base64 = require('js-base64').Base64,
  config = require('../../config');

exports.issueToken = function(userId, clientId, state, callback) {
  var expires_in_seconds = Math.round((config.cookie.maxAge || 1440000) / 1000);
  var dt = new Date();
  dt.setSeconds(dt.getSeconds() + expires_in_seconds);
  Token.create({
    _user: userId,
    client_id: clientId,
    token_type: 'bearer',
    access_token: base64.encode(uuid.v4()),
    expire_at: dt,
    open_at: new Date()
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
      expires_in: expires_in_seconds,
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
