(function() {
  'use strict';

  angular.module('app.util')

  .controller('MsgCtrl', function($scope, $modalInstance, data) {

    switch (data.type) {
      case 'error':
        $scope.icon = 'exclamation-circle';
        data.commands = data.commands || ['OK'];
        break;
      case 'warning':
        $scope.icon = 'exclamation-triangle';
        data.commands = data.commands || ['Cancel' | 'OK'];
        break;
      case 'question':
        $scope.icon = 'question-circle';
        data.commands = data.commands || ['Yes' | 'No' | 'Cancel'];
        break;
      case 'info':
        $scope.icon = 'info-circle';
        data.commands = data.commands || ['OK'];
        break;
      case 'success':
        $scope.icon = 'check-circle';
        data.commands = data.commands || ['OK'];
        break;
      default:
        $scope.icon = '';
        data.commands = data.commands || ['OK'];
        break;
    }

    $scope.commands = data.commands.map(function(s) {
      return {
        name: s,
        icon: (function(s) {
          switch (s.toLowerCase()) {
            case 'ok':
            case 'yes':
              return 'check';
            case 'cancel':
            case 'no':
              return 'times';
            default:
              return '';
          }
        })(s),
        class: (function(s) {
          switch (s.toLowerCase()) {
            case 'ok':
            case 'yes':
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
  })

  .factory('msgBox', ['$modal', function($modal) {

    return function(data) {
      /** data: {
        type: 'error', // error|warning|question|info|success
        size: 'sm', // sm|md|lg
        commands: ['Cancel|'OK'],
        title: 'Http Error',
        content: ['line1', 'line2']  // lines
      } */
      return $modal.open({
        templateUrl: 'msgbox.html',
        controller: 'MsgCtrl',
        size: data.size || 'sm',
        backdrop: 'static',
        keyboard: false,
        animation: true,
        resolve: {
          data: function() {
            return data;
          }
        }
      });
    };

  }]);

})();
