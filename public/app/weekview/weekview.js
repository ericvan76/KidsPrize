(function() {
  'use strict';

  angular.module('weekview', ['ngResource'])

  .directive('kzWeekview', ['AuthSvc', function(AuthSvc) {
    return {
      restrict: 'E',
      templateUrl: 'weekview.html',
      controller: ['$scope', '$rootScope', function($scope, $rootScope) {

        var today = new Date();
        today = new Date(today.getFullYear(), today.getMonth(), today.getDate());
        var thisWeek = getWeekStart(today, false);

        $scope.gotoThisWeek = function() {
          $rootScope.currentWeek = thisWeek;
          $scope.dates = getWeekDates($rootScope.currentWeek);
        };

        $scope.gotoPrevWeek = function() {
          $rootScope.currentWeek = addDays($rootScope.currentWeek, -7);
          $scope.dates = getWeekDates($rootScope.currentWeek);
        };

        $scope.gotoNextWeek = function() {
          $rootScope.currentWeek = addDays($rootScope.currentWeek, 7);
          $scope.dates = getWeekDates($rootScope.currentWeek);
        };

        $scope.isThisWeek = function() {
          return $rootScope.currentWeek.getTime() === thisWeek.getTime();
        };

        $scope.isToday = function(d) {
          return d.getTime() === today.getTime();
        };

      }]
    };
  }]);

  function getWeekStart(d, startFromMonday) {
    var offset = d.getDay();
    if (startFromMonday) {
      offset -= 1;
    }
    return addDays(d, -1 * offset);
  }

  function addDays(d, offset) {
    var nd = new Date(d.getTime());
    return new Date(nd.setDate(nd.getDate() + offset));
  }

  function getWeekDates(weekStart) {
    var dates = [];
    for (var i = 0; i < 7; i++) {
      dates.push(addDays(weekStart, i));
    }
    return dates;
  }

})();
