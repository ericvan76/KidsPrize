(function() {
  'use strict';

  angular.module('child')

  .controller('EditChildCtrl', ['child', '$scope', '$modalInstance',
    function(child, $scope, $modalInstance) {

      $scope.child = child;
      $scope.title = child._id === undefined ? 'Add Child' : 'Edit Child';

      $scope.save = function() {
        $scope.child.$save(function(data) {
          $modalInstance.close(data);
        });
      };

      $scope.cancel = function() {
        $modalInstance.dismiss();
      };
    }
  ]);

})();
