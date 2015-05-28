'use strict';

var User = require('./user');

exports.read = function(id, callback) {
  User.findById(id, callback);
};

exports.savePreference = function(id, preference, callback) {
  var update = {};
  for (var prop in preference) {
    update['preference.' + prop] = preference[prop];
  }
  User.findByIdAndUpdate(id, {
    $set: update
  }, {
    new: true,
    upsert: false
  }, callback);
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
      email: u.email,
      preference: {}
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
