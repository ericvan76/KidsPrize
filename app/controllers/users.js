'use strict';

var User = require('../models/user');

exports.read = function(userId, callback) {
  User.findById(userId, callback);
};

exports.postAuth = function(provider, profile, callback) {
  var normUser = normaliseUser(provider, profile);
  if (normUser instanceof Error) {
    return callback(normUser);
  }
  User.findOneAndUpdate({
    email: {
      $regex: new RegExp(normUser.email, "i")
    }
  }, {
    $setOnInsert: normUser
  }, {
    new: true,
    upsert: true
  }, function(err, user) {
    if (err) {
      return callback(err);
    }
    user[provider] = profile;
    user.save(function(err, n) {
      if (err) {
        return callback(err);
      }
      return callback(null, user);
    });
  });
};

function normaliseUser(provider, profile) {
  switch (provider) {
    case 'facebook':
      return {
        email: profile.emails[0].value.toLowerCase(),
        displayName: profile.displayName,
        name: {
          familyName: profile.name.familyName,
          givenName: profile.name.givenName,
        }
      };
    case 'google':
      return {
        email: profile.emails.filter(function(e) {
          return e.type === 'account';
        }).pop().value.toLowerCase(),
        displayName: profile.displayName,
        name: {
          familyName: profile.name.familyName,
          givenName: profile.name.givenName,
        }
      };
  }
  return new Error('Invalid autheticate provider.');
}
