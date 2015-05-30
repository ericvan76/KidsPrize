(function() {
  'use strict';

  angular.module('app.auth', []);
  angular.module('app.util', []);
  angular.module('app.resource', []);

  angular.module('child', []);
  angular.module('weekview', []);
  angular.module('tasks', []);
  angular.module('home', ['child', 'weekview', 'tasks']);

  var app = angular.module('app', [
    'ngRoute', 'ngResource', 'ui.bootstrap', 'ui.sortable',
    'app.auth', 'app.util', 'app.resource', 'app.templates',
    'home'
  ]);

  // Configurations
  app.config(['$routeProvider', function($routeProvider) {
    // client routes
    $routeProvider
      .when('/', {
        controller: 'HomeCtrl',
        templateUrl: 'home.html',
        resolve: {
          user: ['Auth', function(Auth) {
            return Auth.loginUser();
          }],
          themes: ['Themes', function(Themes) {
            return Themes.loadThemes();
          }]
        }
      })
      .when('/login', {
        templateUrl: 'login.html'
      })
      .when('/logout', {
        redirectTo: '/login',
        resolve: {
          revoke: ['Auth', function(Auth) {
            return Auth.revokeToken();
          }],
          logout: ['Auth', function(Auth) {
            return Auth.logout();
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
              var Auth = $injector.get('Auth');
              if (Auth.token && Auth.token.expires > new Date()) {
                config.headers.Authorization = 'Bearer ' + Auth.token.access_token;
                return config;
              } else {
                var d = $q.defer();
                var promise = Auth.requestToken();
                promise.then(function() {
                  config.headers.Authorization = 'Bearer ' + Auth.token.access_token;
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
                    data: function() {
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
                  // TODO: refresh page?
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

  app.config(['$logProvider', function($logProvider) {
    $logProvider.debugEnabled(true);
  }]);

  // Run block
  app.run(['$injector', function($injector) {

  }]);

})();
