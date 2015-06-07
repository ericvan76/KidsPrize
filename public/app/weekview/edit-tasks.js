(function() {
  'use strict';

  angular.module('tasks')

  .controller('EditTasksCtrl', ['Child', 'Score', '$scope', '$modalInstance', 'child', 'thisWeek',

    function(Child, Score, $scope, $modalInstance, child, thisWeek) {

      $scope.showWarning = true;
      $scope.child = child;
      $scope.tasks = child.tasks.map(function(e) {
        return {
          name: e
        };
      });

      $scope.save = function() {
        var newTasks = $scope.tasks.map(function(e) {
          return e.name;
        });
        Child.saveTasks({
          id: $scope.child._id
        }, newTasks, function(data) {
          Score.cleanup({
            childId: $scope.child._id,
            from: thisWeek
          }, function() {
            $modalInstance.close(data);
          });
        });
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

      $scope.isUnique = function(value, index) {
        return $scope.tasks.filter(function(e, i) {
          return e.name === value && i !== index;
        }).length === 0;
      };
    }
  ]);

})();
