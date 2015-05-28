(function() {
  'use strict';

  angular.module('weekview', ['app.resource'])

  .controller('WeekviewCtrl', ['Score', 'DateUtil', 'groupby', '$scope', '$rootScope',
    function(Score, DateUtil, groupby, $scope, $rootScope) {

      $scope.today = null;
      $scope.thisWeek = null;
      $scope.currentWeek = null;
      $scope.dates = [];
      $scope.tasks = [];
      $scope.scores = {}; // hash set

      $rootScope.$watch('currentChild', function() {
        if ($rootScope.currentChild && $scope.currentWeek) {
          $scope.scores = {};
          $scope.updateWeekScores();
        }
      }, true);

      $scope.week = function(offset) {
        $scope.today = DateUtil.getISODate();
        $scope.thisWeek = DateUtil.getWeekStart($scope.today, false);
        if (offset === 0 || $scope.currentWeek === null) {
          $scope.currentWeek = $scope.thisWeek;
        } else {
          $scope.currentWeek = DateUtil.addDays($scope.currentWeek, offset);
        }
        $scope.dates = DateUtil.getWeekDates($scope.currentWeek);
        $scope.updateWeekScores();
      };

      $scope.isThisWeek = function() {
        return $scope.currentWeek.getTime() === $scope.thisWeek.getTime();
      };

      $scope.isToday = function(d) {
        return d.getTime() === $scope.today.getTime();
      };

      $scope.updateWeekScores = function() {
        if ($rootScope.currentChild && $scope.currentWeek) {
          Score.query({
            _child: $rootScope.currentChild._id,
            date: {
              $gte: $scope.currentWeek,
              $lt: DateUtil.addDays($scope.currentWeek, 7)
            }
          }, function(data) {
            if ($scope.currentWeek.getTime() >= $scope.thisWeek.getTime()) {
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
    }
  ])

  .directive('kzWeekview', [function() {
    return {
      restrict: 'E',
      templateUrl: 'weekview.html'
    };
  }]);



})();
