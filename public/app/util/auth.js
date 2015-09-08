(function () {
  'use strict';

  angular.module('app.auth')

    .factory('Auth', ['$q', '$http', '$location', 'User', function ($q, $http, $location, User) {

      var auth = {
        token: null,
        user: null
      };

      /**
       * Requests access token
       * @return {Promise} [description]
       */
      auth.requestToken = function () {
        var deferred = $q.defer();
        var state = Math.floor((Math.random() * 10000) + 1).toString();
        $http.post('/auth/token', null, {
          params: {
            response_type: 'token',
            client_id: 'webapp',
            state: state,
          }
        })
          .success(function (t) {
            if (!t.state || t.state !== state) {
              deferred.reject();
              return $location.url('/login');
            }
            var expires = new Date();
            expires.setSeconds(expires.getSeconds() + t.expires_in - 300);
            t.expires = expires;
            auth.token = t;
            deferred.resolve(t);
          })
          .error(function (err) {
            deferred.reject();
            return $location.url('/login');
          });
        return deferred.promise;
      };

      /**
       * Revokes access token
       * @return {Promise} [description]
       */
      auth.revokeToken = function () {
        var deferred = $q.defer();
        $http.post('/auth/token/revoke', null, {
          params: {
            access_token: auth.token.access_token
          }
        }).then(function () {
          auth.token = null;
          auth.user = null;
          deferred.resolve();
        });
        return deferred.promise;
      };

      /**
       * Logout
       * @return {Promise} [description]
       */
      auth.logout = function () {
        var deferred = $q.defer();
        $http.get('/auth/logout').then(function () {
          deferred.resolve();
        });
        return deferred.promise;
      };

      /**
       * Returns login user
       * @return {(Object|Promise)} Returns a user if it exists, otherwise returns a promise
       */
      auth.loginUser = function () {
        if (!auth.user) {
          var deferred = $q.defer();
          auth.user = User.get(function (u) {
            deferred.resolve(u);
          }, function () {
            deferred.reject();
            return $location.url('/login');
          });
          return deferred.promise;
        }
        return auth.user;
      };

      return auth;

    }]);

})();
