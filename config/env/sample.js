'use strict';

module.exports = {
  numCPUs: 1, // null to auto
  app: {
    name: 'Kids-Reward'
  },
  // mongoose
  mongoose: {
    dbURL: 'mongodb://127.0.0.1:27017/kids-reward',
    debug: false,
    options: {
      /**
       * Database options that will be passed directly to mongoose.connect
       * Below are some examples.
       * See http://mongodb.github.io/node-mongodb-native/driver-articles/mongoclient.html#mongoclient-connect-options
       * and http://mongoosejs.com/docs/connections.html for more information
       */
    }
  },
  session: {
    secret: 'A secret to compute hash',
    saveUninitialized: true,
    resave: false,
  },
  cookie: {
    httpOnly: true,
    maxAge: 3600000
  },
  // oauth setting
  facebook: {
    clientID: 'DEFAULT_APP_ID',
    clientSecret: 'APP_SECRET',
    callbackURL: 'REDIRECT_URL'
  },
  google: {
    clientID: 'DEFAULT_APP_ID',
    clientSecret: 'APP_SECRET',
    callbackURL: 'REDIRECT_URL'
  },
  // mailer settings
  emailFrom: 'SENDER EMAIL ADDRESS', // sender address like ABC <abc@example.com>
  mailer: {
    service: 'SERVICE_PROVIDER', // Gmail, SMTP
    auth: {
      user: 'EMAIL_ID',
      pass: 'PASSWORD'
    }
  }
};
