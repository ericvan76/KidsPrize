(function() {
  'use strict';

  /**
   * Generates CURD controller
   * @param  {mongoose.Model} model     [description]
   * @return {express.Router}           [description]
   */
  function createController(model) {

    var controller = {
      _model: model
    };

    controller.create = function(uid, o, cb) {

      if (uid === null) {
        return cb(new Error('Unauthorised.'));
      }
      o._user = uid;
      model.create(o, cb);
    };
    controller.read = function(uid, id, cb) {
      if (uid === null) {
        return cb(new Error('Unauthorised.'));
      }
      model.findOne({
        _user: uid,
        _id: id
      }, function(err, r) {
        if (err) {
          return cb(err);
        }
        return cb(null, r);
      });
    };
    controller.update = function(uid, id, o, cb) {
      if (uid === null) {
        return cb(new Error('Unauthorised.'));
      }
      if (model.schema.path('update_at').instance === 'Date' &&
        model.schema.pathType('update_at') === 'real') {
        o.update_at = Date.now();
      }
      model.findOneAndUpdate({
        _user: uid,
        _id: id
      }, o, {
        new: true,
        upsert: false,
        overwrite: false // we will never overwrite a document, it won't be true
      }, function(err, r) {
        if (err) {
          return cb(err);
        }
        return cb(null, r);
      });
    };
    controller.delete = function(uid, id, cb) {
      if (uid === null) {
        return cb(new Error('Unauthorised.'));
      }
      model.findOneAndRemove({
        _user: uid,
        _id: id
      }, function(err, r) {
        if (err) {
          return cb(err);
        }
        return cb(null, r);
      });
    };
    controller.query = function(uid, q, cb) {
      if (uid === null) {
        return cb(new Error('Unauthorised.'));
      }
      q._user = uid;
      model.find(q, cb);
    };

    return controller;
  }


  /**
   * Generates CURD routes
   * @param  {Object}         controller  [description]
   * @param  {Object}         [option]    [description]
   * @return {express.Router}             [description]
   */
  function createRouter(controller, option) {

    if (!controller._model) {
      throw new Error('Invalid curd controller.');
    }
    var router = require('express').Router();
    var modelName = controller._model.modelName;
    var base64 = require('js-base64').Base64;
    var opt = option || {};
    var path = opt.path || '/' + modelName.toLowerCase();
    var include = opt.include || ['create', 'read', 'update', 'delete', 'query'];

    if (include.indexOf('create') !== -1) {
      // create
      router.post(path, function(req, res, next) {
        if (!req.user._id) {
          return next(new HttpError(401, 'Unauthorised.'));
        }
        if (req.body._id !== undefined) {
          return next(new HttpError(400, 'Field _id must not be set.'));
        }
        controller.create(req.user._id, req.body, function(err, data) {
          if (err) {
            return next(err);
          }
          if (!data) {
            return next(new HttpError(500, 'Failed to create ' + modelName));
          }
          res.json(data);
        });
      });
    }
    if (include.indexOf('read') !== -1) {
      // read
      router.get(path + '/:id', function(req, res, next) {
        if (!req.user._id) {
          return next(new HttpError(401, 'Unauthorised.'));
        }
        if (!req.params.id) {
          return next(new HttpError(400, 'Missing url parameter - id.'));
        }
        controller.read(req.user._id, req.params.id, function(err, data) {
          if (err) {
            return next(err);
          }
          if (!data) {
            return next(new HttpError(404, modelName + ' Not found.'));
          }
          res.json(data);
        });
      });
    }
    if (include.indexOf('update') !== -1) {
      // update
      router.put(path + '/:id', function(req, res, next) {
        if (!req.user._id) {
          return next(new HttpError(401, 'Unauthorised.'));
        }
        if (req.body._id !== undefined) {
          if (req.params.id !== req.body._id) {
            return next(new Error(400, 'Unmatched _id'));
          } else {
            delete req.body._id;
          }
        }
        controller.update(req.user._id, req.params.id, req.body, function(err, data) {
          if (err) {
            return next(err);
          }
          if (!data) {
            return next(new HttpError(404, modelName + ' Not found.'));
          }
          res.json(data);
        });
      });
    }
    if (include.indexOf('delete') !== -1) {
      // delete
      router.delete(path + '/:id', function(req, res, next) {
        if (!req.user._id) {
          return next(new HttpError(401, 'Unauthorised.'));
        }
        if (!req.params.id) {
          return next(new HttpError(400, 'Missing url parameter - id.'));
        }
        controller.delete(req.user._id, req.params.id, function(err, data) {
          if (err) {
            return next(err);
          }
          if (!data) {
            return next(new HttpError(404, modelName + ' Not found.'));
          }
          res.status(200).send('OK');
        });
      });
    }
    if (include.indexOf('query') !== -1) {
      // query
      router.get(path, function(req, res, next) {
        if (!req.user._id) {
          return next(new HttpError(401, 'Unauthorised.'));
        }
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
  }

  module.exports = {
    Controller: createController,
    Router: createRouter
  };

})();
