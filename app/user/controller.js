'use strict';

var User = require('./user');

exports.read = function(userId, callback) {
  User.findById(userId, callback);
};

exports.postAuth = function(provider, profile, callback) {
  var u = normaliseUser(provider, profile);
  if (u instanceof Error) {
    return callback(u);
  }

  var updateSet = {
    displayName: u.displayName,
    name: u.name
  };
  updateSet[provider] = profile;

  User.findOneAndUpdate({
    email: {
      $regex: new RegExp(u.email, "i")
    }
  }, {
    $set: updateSet,
    $setOnInsert: {
      email: u.email
    }
  }, {
    new: true,
    upsert: true
  }, function(err, user) {
    if (err) {
      return callback(err);
    }
    return callback(null, user);
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