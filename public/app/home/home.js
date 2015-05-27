(function() {
  'use strict';

  angular.module('home', ['child', 'weekview'])

  .controller('HomeCtrl', ['User', '$http', '$scope', '$rootScope', '$rootElement',
    function(User, $http, $scope, $rootScope, $rootElement) {

      User.get(function(user) {
        $rootScope.user = user;
        $scope.themes = [];
        $scope.currentTheme = user.preference.theme || 'Default';
        // load themes
        $http.get('http://api.bootswatch.com/3/').success(function(data) {
          var d = {
            name: 'Default',
            cssCdn: getBootstrapHeader().href
          };
          $scope.themes.push(d);
          $scope.themes = $scope.themes.concat(data.themes);
          // set user's theme
          var sel = $scope.themes.filter(function(theme) {
            return theme.name === $scope.currentTheme;
          });
          if (sel.length === 1) {
            $scope.changeTheme(sel[0]);
          }
        });
      });

      $scope.changeTheme = function(theme) {
        var bs = getBootstrapHeader();
        bs.href = theme.cssCdn;
        $scope.currentTheme = theme.name;
        User.savePreference({
          theme: theme.name
        }, function(preference) {
          $rootScope.user.preference = preference;
        });
      };

      function getBootstrapHeader() {
        var head = $rootElement[0].children[0];
        return head.getElementsByTagName('link')[0];
      }
    }
  ]);

})();
