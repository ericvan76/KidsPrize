(function() {
  'use strict';

  angular.module('home', ['child', 'weekview'])

  .controller('HomeCtrl', ['$scope', 'User', 'Themes', 'user', 'themes',
    function($scope, User, Themes, user, themes) {

      $scope.changeTheme = function(theme) {
        if (theme.cssCdn) {
          Themes.changeTheme(theme.cssCdn);
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
