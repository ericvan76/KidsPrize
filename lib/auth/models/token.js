'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;

TokenSchema = new Schema({
  _user: {
    type: Schema.Types.ObjectId,
    ref: 'User',
    required: true
  },
  session_id: {
    type: String,
    unique: true,
    required: true
  },
  access_token: {
    type: String,
    unique: true,
    required: true
  },
  refresh_token: {
    type: String,
    unique: true,
    required: true,
  },
  expire_at: {
    type: Date,
    required: true
  },
  open_at: {
    type: Date,
    required: true
  },
  close_at: Date,
  refresh_at: Date,
});

function genToken() {
  var uuid = require('node-uuid'),
    base64 = require('base64').Base64;;
  return base64.encode(uuid.v4());
}

TokenSchema
