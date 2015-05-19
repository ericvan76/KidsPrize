(function() {
  'use strict';

  angular.module('app.util', ['ui.bootstrap']);

  var app = angular.module('app', ['ngRoute', 'ngResource', 'app.templates', 'app.util', 'home']);

  // Configurations
  app.config(['$routeProvider', function($routeProvider) {
    // client routes
    $routeProvider
      .when('/', {
        templateUrl: 'home.html',
        resolve: {
          token: ['AuthSvc', function(auth) {
            return auth.requestToken();
          }],
        }
      })
      .when('/login', {
        templateUrl: 'login.html'
      })
      .when('/logout', {
        redirectTo: '/login',
        resolve: {
          revoke: ['AuthSvc', function(auth) {
            return auth.revokeToken();
          }],
          logout: ['AuthSvc', function(auth) {
            return auth.logout();
          }]
        }
      })
      .otherwise({
        redirectTo: '/'
      });
  }]);

  app.config(['$httpProvider', function($httpProvider) {

    $httpProvider.interceptors.push(['$q', '$location', '$injector',

      function($q, $location, $injector) {

        function isApi(url) {
          if (/^\/api\//i.test(url)) {
            return true;
          }
          return false;
        }

        return {
          request: function(config) {
            // add Authorization header for all calls startswith '/api/'
            if (isApi(config.url)) {
              var auth = $injector.get('AuthSvc');
              if (auth.token && auth.token.expires > new Date()) {
                config.headers.Authorization = 'Bearer ' + auth.token.access_token;
                return config;
              } else {
                var d = $q.defer();
                var promise = auth.requestToken();
                promise.then(function() {
                  config.headers.Authorization = 'Bearer ' + auth.token.access_token;
                  d.resolve(config);
                });
                return d.promise;
              }
            } else {
              return config;
            }
          },
          response: function(response) {
            return response;
          },
          responseError: function(response) {
            if (response.status === 401) {
              $location.url('/login');
            } else {
              if (isApi(response.config.url)) {
                var modal = $injector.get('$modal');
                var modalInstance = modal.open({
                  templateUrl: 'msgbox.html',
                  controller: 'MsgCtrl',
                  size: 'md',
                  backdrop: 'static',
                  keyboard: false,
                  animation: true,
                  resolve: {
                    info: function() {
                      return {
                        class: 'danger',
                        icon: 'exclamation',
                        commands: ['OK'],
                        title: 'Error',
                        content: [
                          response.config.method + ' ' + response.config.url,
                          response.status + ' ' + response.data
                        ]
                      };
                    }
                  }
                });
                modalInstance.result.then(function(result) {
                  return $scope.$apply();
                }, function() {
                  // $log.info('Modal dismissed at: ' + new Date());
                });
              }
            }
            return $q.reject(response);
          }
        };
      }
    ]);
  }]);

  app.config(['$locationProvider', function($locationProvider) {
    // html5 mode on
    $locationProvider.html5Mode(true);
  }]);

  app.config(['$resourceProvider', function($resourceProvider) {
    // Strip trailing slashes from calculated URLs
    $resourceProvider.defaults.stripTrailingSlashes = true;
  }]);


  // Run block
  app.run(['$injector', function($injector) {

  }]);

})();
