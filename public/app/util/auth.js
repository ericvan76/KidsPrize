(function() {

  // Global services & directives
  angular.module('app.util')

  .factory('AuthSvc', ['$q', '$http', '$location', 'User', function($q, $http, $location, User) {

    var auth = {
      token: null,
      user: null
    };

    auth.requestToken = function() {
      var deferred = $q.defer();
      var state = Math.floor((Math.random() * 10000) + 1).toString();
      $http.post('/auth/token', null, {
          params: {
            response_type: 'token',
            client_id: 'webapp',
            state: state,
          }
        })
        .success(function(t) {
          if (!t.state || t.state !== state) {
            deferred.reject();
            return $location.url('/login');
          }
          var expires = new Date();
          expires.setSeconds(expires.getSeconds() + t.expires_in - 300);
          t.expires = expires;
          auth.token = t;
          deferred.resolve();
        })
        .error(function(err) {
          deferred.reject();
          return $location.url('/login');
        });
      return deferred.promise;
    };

    auth.revokeToken = function() {
      var deferred = $q.defer();
      $http.post('/auth/token/revoke', null, {
        params: {
          access_token: auth.token.access_token
        }
      }).then(function() {
        auth.token = null;
        auth.user = null;
        deferred.resolve();
      });
      return deferred.promise;
    };

    auth.logout = function() {
      var deferred = $q.defer();
      $http.get('/auth/logout').then(function() {
        deferred.resolve();
      });
      return deferred.promise;
    };

    auth.getLoginUser = function() {
      if (!auth.user) {
        auth.user = User.get();
      }
      return auth.user;
    };

    return auth;

  }]);

})();