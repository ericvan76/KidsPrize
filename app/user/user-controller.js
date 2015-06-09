'use strict';

var User = require('./user-model');

/**
 * Read a user by id
 * @param  {ObjectId} id
 * @param  {Function} callback
 */
exports.read = function(id, callback) {
  User.findById(id, callback);
};

/**
 * Save user's preference
 * @param  {ObjectId} id
 * @param  {Object}   preference
 * @param  {Function} callback
 */
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

/**
 * Will be called when passport got authorised.
 * @param  {String}   provider [description]
 * @param  {Object}   profile  [description]
 * @param  {Function} callback [description]
 */
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

/**
 * Returns a normalised user from social profile
 * @param  {String} provider facebook|google|etc
 * @param  {Object} profile  [description]
 * @return {Object} normalised user
 */
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
