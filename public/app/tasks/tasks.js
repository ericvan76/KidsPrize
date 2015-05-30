(function() {
  'use strict';

  angular.module('tasks')

  .controller('EditTasksCtrl', ['$scope', '$modalInstance', 'childName', 'tasks',

    function($scope, $modalInstance, childName, tasks) {

      $scope.childName = childName;
      $scope.tasks = tasks.map(function(e) {
        return {
          name: e
        };
      });

      $scope.save = function() {
        $modalInstance.close($scope.tasks.map(function(t) {
          return t.name;
        }));
      };

      $scope.cancel = function() {
        $modalInstance.dismiss();
      };

      $scope.add = function() {
        $scope.tasks.push({
          name: ''
        });
      };

      $scope.remove = function(index) {
        $scope.tasks.splice(index, 1);
      };
    }
  ]);

})();
