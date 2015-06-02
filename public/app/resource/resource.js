(function() {
  'use strict';

  angular.module('app.resource')

  .factory('resource', ['$resource', function($resource) {

    /** idea from
     * http://kirkbushell.me/angular-js-using-ng-resource-in-a-more-restful-manner/
     */
    return function(url, params, methods) {
      var defaults = {
        create: {
          method: 'POST'
        },
        patch: {
          method: 'PATCH'
        }
      };
      methods = angular.extend(defaults, methods);
      var r = $resource(url, params, methods);
      // override $save
      r.prototype.$save = function() {
        if (!this._id) {
          return this.$create.apply(this, arguments);
        } else {
          return this.$patch.apply(this, arguments);
        }
      };
      // override query
      r._query = r.query;
      r.query = function() {
        if (arguments.length > 0) {
          if (typeof arguments[0] === 'object' && arguments[0]) {
            arguments[0] = {
              q: Base64.encodeURI(JSON.stringify(arguments[0]))
            };
          }
        }
        return r._query.apply(r, arguments);
      };
      return r;
    };
  }])

  .factory('User', ['resource', function(resource) {
    return resource('/api/user', null, {
      savePreference: {
        method: 'POST',
        url: '/api/user/preference'
      }
    });
  }])

  .factory('Child', ['resource', function(resource) {
    return resource('/api/child/:id', {
      id: '@_id'
    }, {
      saveTasks: {
        method: 'POST',
        url: '/api/child/:id/tasks',
        isArray: true
      }
    });
  }])

  .factory('Score', ['resource', function(resource) {
    return resource('/api/score/:id', {
      id: '@_id'
    }, {
      total: {
        method: 'GET',
        url: '/api/score/total/:_child'
      }
    });
  }]);

})();
