(function() {
  'use strict';

  angular.module('app.resource')

  .factory('Resource', ['$resource', function($resource) {
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
      var resource = $resource(url, params, methods);
      resource.prototype.$save = function() {
        if (!this.id) {
          return this.$create();
        } else {
          return this.$patch();
        }
      };
      return resource;
    };
  }])

  .factory('User', ['Resource', function($resource) {
    return $resource('/api/user', {});
  }])

  .factory('Score', ['Resource', function($resource) {
    return $resource('/api/score', {});
  }]);

})();
