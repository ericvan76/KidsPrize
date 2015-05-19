(function() {
  'use strict';

  angular.module('user', ['ngResource'])
    .factory('User', ['$resource', function($resource) {
      return $resource('/api/user', {}, {
        'update': {
          method: 'PUT'
        }
      });
    }]);

})();
