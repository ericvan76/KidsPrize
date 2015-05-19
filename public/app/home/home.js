(function() {
  'use strict';

  angular.module('home', ['ngResource', 'ui.bootstrap', 'user', 'child', 'weekview'])

  .controller('HomeCtrl', ['User', '$scope', function(User, $scope) {
    $scope.user = User.get();
  }]);

})();
