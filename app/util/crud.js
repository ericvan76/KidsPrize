(function() {
  'use strict';

  /**
   * Generates CURD routes
   * @param  {express.Router} router    [description]
   * @param  {mongoose.Model} model     [description]
   * @param  {Object}         [option]  [description]
   * @return {express.Router}           [description]
   */
  module.exports = function(router, model, option) {

    var base64 = require('js-base64').Base64;

    var opt = option || {};
    var userRestrict = opt.userRestrict !== false; // default=true
    var path = opt.path || '/' + model.modelName.toLowerCase();
    var include = opt.include || ['create', 'read', 'patch', 'delete', 'query'];

    var controller = {};

    controller.create = function(uid, o, cb) {
      if (userRestrict) {
        o._user = uid;
      }
      model.create(o, cb);
    };
    controller.read = function(uid, id, cb) {
      var q = {
        _id: id
      };
      if (userRestrict) {
        q._user = uid;
      }
      model.findOne(q, cb);
    };
    controller.patch = function(uid, id, o, cb) {
      var q = {
        _id: id
      };
      if (userRestrict) {
        q._user = uid;
      }
      delete o._id;
      if (model.schema.path('update_at').instance === 'Date' &&
        model.schema.pathType('update_at') === 'real') {
        o.update_at = Date.now();
      }
      model.findOneAndUpdate(q, o, {
        new: true,
        upsert: false
      }, cb);
    };
    controller.delete = function(uid, id, cb) {
      var q = {
        _id: id
      };
      if (userRestrict) {
        q._user = uid;
      }
      model.findOneAndRemove(q, cb);
    };
    controller.query = function(uid, q, cb) {
      if (userRestrict) {
        q._user = uid;
      }
      model.find(q, cb);
    };

    if (include.indexOf('create') !== -1) {
      // create
      router.post(path, function(req, res, next) {
        controller.create(req.user._id, req.body, function(err, data) {
          if (err) {
            return next(err);
          }
          res.json(data);
        });
      });
    }
    if (include.indexOf('read') !== -1) {
      // read
      router.get(path + '/:id', function(req, res, next) {
        controller.read(req.user._id, req.params.id, function(err, data) {
          if (err) {
            return next(err);
          }
          if (!data) {
            return res.status(404).send(model.modelName + ' Not Found');
          }
          res.json(data);
        });
      });
    }
    if (include.indexOf('patch') !== -1) {
      // patch
      router.patch(path + '/:id', function(req, res, next) {
        controller.patch(req.user._id, req.params.id, req.body, function(err, data) {
          if (err) {
            return next(err);
          }
          if (!data) {
            return res.status(404).send(model.modelName + ' not found.');
          }
          res.json(data);
        });
      });
    }
    if (include.indexOf('delete') !== -1) {
      // delete
      router.delete(path + '/:id', function(req, res, next) {
        controller.delete(req.user._id, req.params.id, function(err) {
          if (err) {
            return next(err);
          }
          res.status(200).send('OK');
        });
      });
    }
    if (include.indexOf('query') !== -1) {
      // query
      router.get(path, function(req, res, next) {
        var q = req.query.q || {};
        if (req.query.q) {
          q = JSON.parse(base64.decode(req.query.q));
        }
        console.log('Decoded query: ' + JSON.stringify(q));
        controller.query(req.user._id, q, function(err, data) {
          if (err) {
            return next(err);
          }
          res.json(data);
        });
      });
    }
    return router;
  };

})();
