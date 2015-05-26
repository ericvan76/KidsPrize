(function() {
  'use strict';

  angular.module('app.util')

  .controller('MsgCtrl', function($scope, $modalInstance, data) {

    switch (data.type) {
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

    $scope.commands = (data.commands || ['OK']).map(function(s) {
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

    $scope.title = data.title || 'Message';
    if (typeof data.content === 'string') {
      $scope.content = [data.content];
    } else {
      $scope.content = data.content;
    }

    $scope.onclick = function(cmd) {
      $modalInstance.close(cmd.toLowerCase());
    };
  });

})();
