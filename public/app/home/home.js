(function() {
  'use strict';

  angular.module('home', ['child', 'weekview'])

  .controller('HomeCtrl', ['$scope', '$rootElement', 'User', 'user', 'themes',
    function($scope, $rootElement, User, user, themes) {

      $scope.changeTheme = function(theme) {
        if (theme.cssCdn) {
          var bs = $rootElement[0].children[0].getElementsByTagName('link')[0];
          bs.href = theme.cssCdn;
        }
        if ($scope.currentTheme !== theme.name) {
          $scope.currentTheme = theme.name;
          User.savePreference({
            theme: theme.name
          }, function(preference) {
            $scope.user.preference = preference;
          });
        }
      };

      $scope.user = user;
      $scope.themes = themes;
      $scope.currentTheme = 'Default';

      if ($scope.user.preference && $scope.user.preference.theme) {
        $scope.currentTheme = $scope.user.preference.theme;
      }

      var sel = $scope.themes.filter(function(theme) {
        return theme.name === $scope.currentTheme;
      });
      if (sel.length === 1) {
        $scope.changeTheme(sel[0]);
      }
    }

  ]);

})();
