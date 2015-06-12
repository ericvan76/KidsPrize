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
 * @param  {Object}   user  [description]
 * @param  {Function} callback [description]
 */
exports.resolveUser = function(user, callback) {
  User.findOneAndUpdate({
      email: {
        $regex: new RegExp(user.email, 'i')
      }
    }, user, {
      new: true,
      upsert: true
    },
    function(err, u) {
      if (err) {
        return callback(err);
      }
      return callback(null, u);
    });
};

/**
 * Returns a normalised user from social profile
 * @param  {String} provider facebook|google|etc
 * @param  {Object} profile  [description]
 * @return {Object} normalised user
 */
exports.normaliseUser = function(provider, profile) {
  switch (provider) {
    case 'facebook':
      return {
        email: profile.emails[0].value.toLowerCase(),
        displayName: profile.displayName,
        name: {
          familyName: profile.name.familyName,
          givenName: profile.name.givenName,
        },
        facebook: profile
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
        },
        google: profile
      };
  }
  return new Error('Invalid autheticate provider.');
};
