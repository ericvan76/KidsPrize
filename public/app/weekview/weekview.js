(function() {
  'use strict';

  angular.module('weekview')

  .controller('WeekviewCtrl', ['Child', 'Score', 'groupby', '$scope', '$rootScope', '$modal',
    function(Child, Score, groupby, $scope, $rootScope, $modal) {

      $scope.today = null;
      $scope.thisWeek = null;
      $scope.currentWeek = null;
      $scope.dates = [];
      $scope.tasks = [];
      $scope.scores = {}; // hash set

      $rootScope.$watch('currentChild', function() {
        $scope.updateWeekScores();
      }, true);

      $scope.week = function(offset) {
        $scope.today = Date.UTCtoday();
        $scope.thisWeek = $scope.today.clone().addHours(-24 * $scope.today.getDay());
        if (offset === 0 || $scope.currentWeek === null) {
          $scope.currentWeek = $scope.thisWeek;
        } else {
          $scope.currentWeek.addWeeks(offset);
        }
        $scope.dates = [0, 1, 2, 3, 4, 5, 6].map(function(i) {
          return $scope.currentWeek.clone().addHours(i * 24);
        });
        $scope.updateWeekScores();
      };

      $scope.updateWeekScores = function() {
        if (!$rootScope.currentChild || !$scope.currentWeek) {
          $scope.tasks = [];
          $scope.scores = {};
        } else {
          Score.query({
            _child: $rootScope.currentChild._id,
            date: {
              $gte: $scope.currentWeek,
              $lt: $scope.currentWeek.clone().addWeeks(1)
            }
          }, function(data) {
            if ($scope.currentWeek.compareTo($scope.thisWeek) >= 0) {
              $scope.tasks = $rootScope.currentChild.tasks || [];
            } else {
              $scope.tasks = groupby(data, 'task').map(function(g) {
                return {
                  task: g.key,
                  order: g.values.reduce(function(a, b) {
                    return new Date(a.update_at) > new Date(b.update_at) ? a : b;
                  }).order || -1
                };
              }).sort(function(a, b) {
                return (a.order || -1) - (b.order || -1);
              }).map(function(x) {
                return x.task;
              });
            }
            $scope.scores = {};
            data.forEach(function(e) {
              e.date = new Date(e.date);
              var key = e.date.toISOString() + '|' + e.task;
              $scope.scores[key] = e;
            });
          });
        }
      };

      $scope.getScore = function(d, task) {
        var key = d.toISOString() + '|' + task;
        if (key in $scope.scores) {
          return $scope.scores[key].value || 0;
        }
        return 0;
      };

      $scope.toggleScore = function(d, task) {
        var key = d.toISOString() + '|' + task;
        if (key in $scope.scores) {
          if ($scope.scores[key].value === 1) {
            $scope.scores[key].value = 0;
          } else {
            $scope.scores[key].value = 1;
          }
        } else {
          $scope.scores[key] = new Score({
            _child: $rootScope.currentChild._id,
            date: d,
            task: task,
            value: 1
          });
        }
        $scope.scores[key].order = $scope.tasks.indexOf(task);
        $scope.scores[key].$save(
          function(s) {
            $scope.scores[key] = s;
          });
      };

      $scope.editTasks = function() {

        var modalInstance = $modal.open({
          animation: true,
          templateUrl: 'edit-tasks.html',
          controller: 'EditTasksCtrl',
          windowClass: 'edit-tasks',
          //size: 'sm',
          resolve: {
            childName: function() {
              return $rootScope.currentChild.name;
            },
            tasks: function() {
              return [].concat($rootScope.currentChild.tasks);
            }
          }
        });

        modalInstance.result.then(function(tasks) {
          Child.saveTasks({
            id: $rootScope.currentChild._id
          }, tasks, function(data) {
            $rootScope.currentChild.tasks = data;
            $scope.tasks = data;
            $scope.updateWeekScores();
          });
        });
      };
    }
  ])

  .directive('kzWeekview', [function() {
    return {
      restrict: 'E',
      templateUrl: 'weekview.html'
    };
  }]);



})();
