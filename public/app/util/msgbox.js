(function() {
  'use strict';

  angular.module('app.util')

  .controller('MsgCtrl', function($scope, $modalInstance, info) {

    switch (info.type) {
      case 'error':
        $scope.icon = 'exclamation-circle';
        $scope.class = 'danger';
        break;
      case 'warning':
        $scope.icon = 'exclamation-triangle';
        $scope.class = 'warning';
        break;
      case 'question':
        $scope.icon = 'question-circle';
        $scope.class = 'warning';
        break;
      case 'info':
        $scope.icon = 'info-circle';
        $scope.class = 'info';
        break;
      case 'success':
        $scope.icon = 'check-circle';
        $scope.class = 'success';
        break;
      default:
        $scope.icon = 'info';
        $scope.class = 'default';
        break;
    }

    $scope.commands = (info.commands || ['OK']).map(function(s) {
      return {
        name: s,
        icon: (function(s) {
          switch (s.toLowerCase()) {
            case 'ok':
            case 'yes':
            case 'confirm':
            case 'close':
              return 'check';
            case 'cancel':
            case 'no':
              return 'times';
            default:
              return 'invisible';
          }
        })(s),
        class: (function(s) {
          switch (s.toLowerCase()) {
            case 'ok':
            case 'yes':
            case 'confirm':
            case 'close':
              return 'primary';
            default:
              return 'default';
          }
        })(s)
      };
    });

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
