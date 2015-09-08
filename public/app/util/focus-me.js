(function () {
  'use strict';

  angular.module('app.util')

    .directive('focusMe', ['$timeout', function ($timeout) {
      return {
        restrict: 'A',
        link: function (scope, element) {
          $timeout(function () {
            element[0].focus();
          });
        }
      };
    }]);

})();
