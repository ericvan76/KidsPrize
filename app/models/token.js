'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;

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
    required: true
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

module.exports = mongoose.model('Token', TokenSchema);
