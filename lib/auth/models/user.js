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

UserSchema.statics.postAuth = function(provider, profile, callback) {
  var normUser = normaliseUser(provider, profile);
  if (normUser instanceof Error) {
    callback(normUser);
  } else {
    mongoose.model('User').findOneAndUpdate({
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
        callback(err);
      } else {
        user[provider] = profile;
        if (user.isModified) {
          user.save(function(err, n) {
            if (err) {
              callback(err);
            } else {
              mongoose.model('User').findById(user._id, callback);
            }
          });
        } else {
          callback(null, user);
        }
      }
    });
  }
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

module.exports = mongoose.model('User', UserSchema);
