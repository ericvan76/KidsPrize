(function() {
  'use strict';

  angular.module('app.directives')
    .directive('weekhead', function() {
      return {
        restrict: 'A',
        templateUrl: '/partials/weekhead.html',
        controller: function() {
          this.days = [{
            date: '2015-01-01',
            day: 'Sun'
          }, {
            date: '2015-01-02',
            day: 'Mon'
          }, {
            date: '2015-01-03',
            day: 'Tue'
          }, {
            date: '2015-01-04',
            day: 'Wed'
          }, {
            date: '2015-01-05',
            day: 'Thu'
          }, {
            date: '2015-01-06',
            day: 'Fri'
          }, {
            date: '2015-01-07',
            day: 'Sat'
          }];
        },
        controllerAs: 'week'
      };
    });

})();
