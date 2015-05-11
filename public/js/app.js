(function() {

  'use strict';

  angular.module('app.services', []);

  angular.module('app.controllers', []);

  angular.module('app.directives', ['app.services']);

  var app = angular.module('app', ['ngRoute', 'app.controllers', 'app.directives', 'app.services']);

  app.config(['$routeProvider', '$locationProvider', '$httpProvider',

    function($routeProvider, $locationProvider, $httpProvider) {

      // config route
      $routeProvider
        .when('/', {
          templateUrl: '/partials/index.html',
          resolve: {
            token: ['AuthenticationService', function(auth) {
              auth.requestToken();
            }],
          },
        })
        .when('/login', {
          templateUrl: '/partials/login.html'
        })
        .when('/logout', {
          redirectTo: '/login',
          resolve: {
            revoke: ['AuthenticationService', function(auth) {
              auth.revokeToken();
            }],
            logout: ['AuthenticationService', function(auth) {
              auth.logout();
            }]
          }
        })
        .otherwise({
          redirectTo: '/'
        });

      // html5
      $locationProvider.html5Mode(true);

      // config $http
      $httpProvider.interceptors.push(['$q', '$location', '$injector',

        function($q, $location, $injector) {

          return {
            request: function(config) {
              // add Authorization header for all calls startswith '/api/'
              if (/^\/api\//i.test(config.url)) {
                var auth = $injector.get('AuthenticationService');
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
              }
              return $q.reject(response);
            }
          };
        }
      ]);
    }
  ]);

})();
