(function() {
  'use strict';

  angular.module('home', ['child', 'weekview'])

  .controller('HomeCtrl', ['AuthSvc', '$scope', function(AuthSvc, $scope) {
    $scope.user = AuthSvc.getLoginUser();
  }])

  .controller('ThemeCtrl', ['$http', '$scope', '$rootElement', function($http, $scope, $rootElement) {
    $scope.currentTheme = 'Default';
    $scope.themes = [];

    $http.get('http://api.bootswatch.com/3/').success(function(data) {
      var d = {
        name: 'Default',
        cssCdn: getBootstrapHeader().href
      };
      $scope.themes.push(d);
      $scope.themes = $scope.themes.concat(data.themes);
    });

    $scope.changeTheme = function(theme) {
      var bs = getBootstrapHeader();
      bs.href = theme.cssCdn;
      $scope.currentTheme = theme.name;
    };

    function getBootstrapHeader() {
      var head = $rootElement[0].children[0];
      return head.getElementsByTagName('link')[0];
    }
  }]);

})();
