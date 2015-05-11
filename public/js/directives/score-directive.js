(function() {

  'use strict';

  angular.module('app.directives', []).directive('score', function() {
    return {
      restrict: 'E',
      templateUrl: '/particals/scores',
      controller: function() {},
      controllerAs: 'dayScore'
    };
  });

})();

