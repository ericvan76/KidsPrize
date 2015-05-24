(function() {
  'use strict';

  angular.module('app.resource')

  .factory('resource', ['$resource', function($resource) {

    /**
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
      r.save = function(a, y, m, x) {
        if (!a.id) {
          return r.create(a, y, m, x);
        } else {
          return r.patch(a, y, m, x);
        }
      };
      r.prototype.$save = function(y, m, x) {
        return r.save(this, y, m, x);
      };
      // override $query
      r._query = r.query;
      r.query = function(a, y, m, x) {
        if (typeof a === 'object' && a) {
          a = {
            q: Base64.encodeURI(JSON.stringify(a))
          };
        }
        return r._query(a, y, m, x);
      };
      r.prototype.$query = function(y, m, x) {
        return r.query(this, y, m, x);
      };
      return r;
    };
  }])

  .factory('User', ['resource', function(resource) {
    return resource('/api/user', {});
  }])


  .factory('Child', ['resource', function(resource) {
    return resource('/api/child/:id', {
      id: '@id'
    });
  }])

  .factory('Score', ['resource', function(resource) {
    return resource('/api/score/:id', {
      id: '@id'
    });
  }]);

})();
