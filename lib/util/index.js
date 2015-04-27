'use strict';

exports.genGuid = function() {
  var uuid = require('node-uuid');
  return uuid.v4();
};

exports.genToken = function() {
  var base64 = require('js-base64').Base64;
  var guid = exports.genGuid();
  return base64.encode(guid);
};

exports.hash = function(s, encoding) {
  encoding = encoding || 'base64';
  var crypto = require('crypto'),
    shasum = crypto.createHash('sha1');
  shasum.update(s);
  return shasum.digest(encoding);
};
