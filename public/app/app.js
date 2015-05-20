(function() {
  'use strict';

  angular.module('app.util', []);

  var app = angular.module('app', ['ngRoute', 'ngResource', 'ui.bootstrap', 'app.templates', 'app.util', 'home']);

  // Configurations
  app.config(['$routeProvider', function($routeProvider) {
    // client routes
    $routeProvider
      .when('/', {
        templateUrl: 'home.html',
        resolve: {
          token: ['AuthSvc', function(AuthSvc) {
            return AuthSvc.requestToken();
          }],
        }
      })
      .when('/login', {
        templateUrl: 'login.html'
      })
      .when('/logout', {
        redirectTo: '/login',
        resolve: {
          revoke: ['AuthSvc', function(AuthSvc) {
            return AuthSvc.revokeToken();
          }],
          logout: ['AuthSvc', function(AuthSvc) {
            return AuthSvc.logout();
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
              var AuthSvc = $injector.get('AuthSvc');
              if (AuthSvc.token && AuthSvc.token.expires > new Date()) {
                config.headers.Authorization = 'Bearer ' + AuthSvc.token.access_token;
                return config;
              } else {
                var d = $q.defer();
                var promise = AuthSvc.requestToken();
                promise.then(function() {
                  config.headers.Authorization = 'Bearer ' + AuthSvc.token.access_token;
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
                        type: 'error',
                        commands: ['OK'],
                        title: 'Http Error',
                        content: [
                          'Status: ' + response.status + ' - ' + response.data,
                          response.config.method + ' ' + response.config.url
                        ]
                      };
                    }
                  }
                });
                modalInstance.result.then(function(result) {
                  // return $scope.$apply();
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
