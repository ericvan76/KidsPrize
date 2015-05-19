(function() {
  'use strict';

  angular.module('app.util')

  .controller('MsgCtrl', function($scope, $modalInstance, info) {

    $scope.icon = info.icon || 'info';
    $scope.class = info.class || 'primary';
    $scope.commands = info.commands || ['OK'];
    $scope.title = info.title || 'Infomation';
    if (typeof info.content === 'string') {
      $scope.content = [info.content];
    } else {
      $scope.content = info.content;
    }

    $scope.onclick = function(cmd) {
      $modalInstance.close(cmd.toLowerCase());
    };
  });

})();
