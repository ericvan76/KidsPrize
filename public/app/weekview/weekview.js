(function() {
  'use strict';

  angular.module('weekview', ['ngResource'])

  .controller('WeekviewCtrl', ['$scope', function($scope) {

    var today = new Date();
    // date
    today = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    var thisWeek = getWeekStart(today, false);

    $scope.gotoThisWeek = function() {
      $scope.currentWeek = thisWeek;
      $scope.dates = getWeekDates($scope.currentWeek);
    };

    $scope.gotoPrevWeek = function() {
      $scope.currentWeek = addDays($scope.currentWeek, -7);
      $scope.dates = getWeekDates($scope.currentWeek);
    };

    $scope.gotoNextWeek = function() {
      $scope.currentWeek = addDays($scope.currentWeek, 7);
      $scope.dates = getWeekDates($scope.currentWeek);
    };

    $scope.isThisWeek = function() {
      return $scope.currentWeek.getTime() === thisWeek.getTime();
    };

    $scope.isToday = function(d) {
      return d.getTime() === today.getTime();
    };

  }])

  .directive('kzWeekview', ['AuthSvc', function(AuthSvc) {
    return {
      restrict: 'E',
      templateUrl: 'weekview.html'
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
