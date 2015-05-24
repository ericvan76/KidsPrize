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
      $scope.scores = [];

      $rootScope.$watch('currentChild', function() {
        if ($rootScope.currentChild && $scope.currentWeek) {
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
      };

      $scope.isThisWeek = function() {
        return $scope.currentWeek.getTime() === $scope.thisWeek.getTime();
      };

      $scope.isToday = function(d) {
        return d.getTime() === $scope.today.getTime();
      };

      $scope.updateWeekScores = function() {
        Score.query({
          _child: $rootScope.currentChild._id,
          date: {
            $gte: $scope.currentWeek,
            $lte: DateUtil.addDays($scope.currentWeek, 7)
          }
        }, function(data) {
          if ($scope.isThisWeek()) {
            $scope.tasks = $rootScope.currentChild.tasks || [];
          } else {
            $scope.tasks = groupby(data, 'task').map(function(g) {
              return {
                task: g.key,
                order: Math.max.apply(null, g.values.map(function(v) {
                  return v.order || -1;
                }))
              };
            }).sort(function(a, b) {
              return (a.order || -1) - (b.order || -1);
            }).map(function(x) {
              return x.task;
            });
          }
          $scope.scores = data;
        });
      };

    }
  ])

  .directive('kzWeekview', ['AuthSvc', function(AuthSvc) {
    return {
      restrict: 'E',
      templateUrl: 'weekview.html'
    };
  }]);



})();
