(function() {
  'use strict';

  angular.module('app.services').factory('AuthenticationService',

    function($q, $http, $location) {

      var service = {
        token: null
      };

      service.requestToken = function() {
        var deferred = $q.defer();
        var state = Math.floor((Math.random() * 10000) + 1).toString();
        $http.post('/auth/token', null, {
            params: {
              response_type: 'token',
              client_id: 'webapp',
              state: state,
            }
          })
          .success(function(token) {
            if (!token.state || token.state !== state) {
              return deferred.reject();
            }
            var expires = new Date();
            expires.setSeconds(expires.getSeconds() + token.expires_in - 300);
            token.expires = expires;
            service.token = token;
            deferred.resolve();
          })
          .error(function(err) {
            deferred.reject();
            $location.url('/login');
          });

        return deferred.promise;
      };

      service.revokeToken = function() {
        var deferred = $q.defer();
        $http.post('/auth/token/revoke', null, {
          params: {
            access_token: service.token.access_token
          }
        }).then(function() {
          service.token = null;
          deferred.resolve();
        });
        return deferred.promise;
      };

      service.logout = function() {
        var deferred = $q.defer();
        $http.get('/auth/logout').then(function() {
          deferred.resolve();
        });
        return deferred.promise;
      };

      return service;
    });

})();
