(function() {
  'use strict';

  angular.module('app.services').factory('AuthenticationService',

    function($q, $http, $location, $rootScope) {

      function requestToken() {
        var deferred = $q.defer();
        $http.post('/auth/token')
          .success(function(token) {
            var expires = new Date();
            expires.setSeconds(expires.getSeconds() + token.expires_in - 300);
            token.expires = expires;
            $rootScope.token = token;
            deferred.resolve();
          })
          .error(function(err) {
            deferred.reject();
            $location.url('/login');
          });
        return deferred.promise;
      }

      function revokeToken() {
        var deferred = $q.defer();
        $http.post('/auth/token/revoke').then(function() {
          $rootScope.token = null;
          deferred.resolve();
        });
        return deferred.promise;
      }

      function logout() {
        var deferred = $q.defer();
        $http.get('/auth/logout').then(function() {
          deferred.resolve();
        });
        return deferred.promise;
      }

      return {
        requestToken: requestToken,
        revokeToken: revokeToken,
        logout: logout
      };
    });
})();
