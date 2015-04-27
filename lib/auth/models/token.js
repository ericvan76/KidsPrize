'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema,
  util = require('../../util'),
  config = require('../../config');

var TokenSchema = new Schema({
  _user: {
    type: Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  client_id: {
    type: String,
    required: true
  },
  token_type: {
    type: String,
    require: true
  },
  access_token: {
    type: String,
    unique: true,
    required: true
  },
  expire_at: {
    type: Date,
    required: true
  },
  refresh_token: {
    type: String
  },
  refresh_at: Date,
  open_at: {
    type: Date,
    required: true
  },
  close_at: Date
});

TokenSchema.statics.issueToken = function(userId, clientId, callback) {
  var expires_in_milliseconds = config.cookie.maxAge || 1440000;
  var dt = new Date();
  dt.setMilliseconds(dt.getMilliseconds() + expires_in_milliseconds);
  mongoose.model('Token').create({
    _user: userId,
    client_id: clientId,
    token_type: 'bearer',
    access_token: util.genToken(),
    expire_at: dt,
    open_at: new Date()
  }, callback);
};

TokenSchema.statics.validateToken = function(token, callback) {
  mongoose.model('Token').findOne({
    access_token: token,
    expire_at: {
      $gt: new Date()
    }
  }, function(err, token){
    if (err){
      callback(err);
    }
    else{
      token.populate('_user', callback);
    }
  });
};

TokenSchema.statics.revokeToken = function(token, callback) {
  mongoose.model('Token').remove({
    access_token: token
  }, callback);
};

module.exports = mongoose.model('Token', TokenSchema);
