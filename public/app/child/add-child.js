(function() {
  'use strict';

  angular.module('child')

  .controller('AddChildCtrl', ['Child', '$scope', '$modalInstance',
    function(Child, $scope, $modalInstance) {

      $scope.child = new Child({
        name: null,
        gender: null,
        tasks: [
          'Finish breakfast in 20 mins',
          'Eating in good manners',
          'Bath before 7pm'
        ]
      });

      $scope.add = function() {
        $modalInstance.close($scope.child);
      };

      $scope.cancel = function() {
        $modalInstance.dismiss();
      };
    }
  ]);

})();
