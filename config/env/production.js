'use strict';

module.exports = {
  app: {
    name: 'Kids-Reward'
  },
  // mongoose
  mongoose: {
    // dbURL: 'mongodb://{production-mongodb}/kids-reward',
    dbURL: 'mongodb://admin:tCXDPppESK65@ds029630.mongolab.com:29630/kids-reward', // #gitignore
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
    // secret: 'A secret to compute hash'
    secret: 'do NOT tell anybody', // #gitignore
    saveUninitialized: true,
    resave: false,
  },
  cookie: {
    httpOnly: true,
    maxAge: 3600000
  },
  // oauth setting
  facebook: {
    // clientID: 'DEFAULT_APP_ID',
    // clientSecret: 'APP_SECRET',
    // callbackURL: 'REDIRECT_URL'
    clientID: '487878201359953', // #gitignore
    clientSecret: '5ea75a640b79290dfdce7583892f20f2', // #gitignore
    callbackURL: 'http://127.0.0.1:3000/auth/facebook/callback' // #gitignore
  },
  google: {
    // clientID: 'DEFAULT_APP_ID',
    // clientSecret: 'APP_SECRET',
    // callbackURL: 'REDIRECT_URL'
    clientID: '224921903817-2bmsf33hgpbntiurp7ittihbf3nvkpog.apps.googleusercontent.com', // #gitignore
    clientSecret: 'GyJpyFspmrDoOME3Q452vZ1r', // #gitignore
    callbackURL: 'http://127.0.0.1:3000/auth/google/callback' // #gitignore
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
