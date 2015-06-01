(function() {
  'use strict';

  angular.module('child')

  .controller('EditChildCtrl', ['title', 'child', '$scope', '$modalInstance',
    function(title, child, $scope, $modalInstance) {

      $scope.title = title;
      $scope.child = child;

      $scope.save = function() {
        $modalInstance.close($scope.child);
      };

      $scope.cancel = function() {
        $modalInstance.dismiss();
      };
    }
  ]);

})();
