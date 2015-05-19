(function() {
  'use strict';

  exports.controller = function(model, option) {

    var opt = option || {};
    var exclude = opt.exclude || [];

    var ctrl = {
      _model: model
    };

    if (exclude.indexOf('create') === -1) {
      ctrl.create = function(userId, o, cb) {
        o._user = userId;
        model.create(o, cb);
      };
    }
    if (exclude.indexOf('read') === -1) {
      ctrl.read = function(userId, id, cb) {
        model.findOne({
          _user: userId,
          _id: id
        }, cb);
      };
    }
    if (exclude.indexOf('update') === -1) {
      ctrl.update = function(userId, id, o, cb) {
        o._user = userId;
        model.findOneAndUpdate({
          _user: userId,
          _id: id
        }, o, cb);
      };
    }
    if (exclude.indexOf('delete') === -1) {
      ctrl.delete = function(userId, id, cb) {
        model.findOneAndRemove({
          _user: userId,
          _id: id
        }, cb);
      };
    }
    if (exclude.indexOf('query') === -1) {
      ctrl.query = function(userId, q, cb) {
        q._user = userId;
        model.find(q, cb);
      };
    }
    return ctrl;
  };

  exports.router = function(ctrl, option) {

    var router = require('express').Router();

    var modelName = ctrl._model.modelName.toLowerCase();
    var basePath = '/' + modelName;

    var opt = option || {};
    var exclude = opt.exclude || [];

    if (typeof ctrl.create === 'function' && exclude.indexOf('create') === -1) {
      // create
      router.post(basePath, function(req, res, next) {
        ctrl.create(req.user._id, req.body, function(err, data) {
          if (err) {
            return next(err);
          }
          res.json(data);
        });
      });
    }
    if (typeof ctrl.read === 'function' && exclude.indexOf('read') === -1) {
      // read
      router.get(basePath + '/:id', function(req, res, next) {
        ctrl.read(req.user._id, req.param.id, function(err, data) {
          if (err) {
            return next(err);
          }
          res.json(data);
        });
      });
    }
    if (typeof ctrl.update === 'function' && exclude.indexOf('update') === -1) {
      // update
      router.put(basePath + '/:id', function(req, res, next) {
        ctrl.update(req.user._id, req.param.id, req.body, function(err, data) {
          if (err) {
            return next(err);
          }
          res.json(data);
        });
      });
    }
    if (typeof ctrl.delete === 'function' && exclude.indexOf('delete') === -1) {
      // delete
      router.delete(basePath + '/:id', function(req, res, next) {
        ctrl.delete(req.user._id, req.param.id, function(err) {
          if (err) {
            return next(err);
          }
          res.status(200).end();
        });
      });
    }
    if (typeof ctrl.query === 'function' && exclude.indexOf('query') === -1) {
      // query
      router.get(basePath, function(req, res, next) {
        var q = {};
        for (var k in req.query) {
          q[k] = req.query[k];
        }
        ctrl.query(req.user._id, q, function(err, data) {
          if (err) {
            return next(err);
          }
          res.json(data);
        });
      });
    }

    return router;
  };

  function getFuncArgs(f) {
    var funStr = f.toString();
    return funStr.slice(funStr.indexOf('(') + 1, funStr.indexOf(')')).match(/([^\s,]+)/g);
  }

})();
