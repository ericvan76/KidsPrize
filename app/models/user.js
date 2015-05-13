'use strict';

var mongoose = require('mongoose'),
  Schema = mongoose.Schema;

var UserSchema = new Schema({
  email: {
    type: String,
    required: true,
    unique: true
  },
  displayName: {
    type: String
  },
  name: {
    familyName: String,
    givenName: String
  },
  roles: {
    type: Array,
    default: ['authenticated']
  },
  facebook: {},
  google: {}
});

module.exports = mongoose.model('User', UserSchema);
