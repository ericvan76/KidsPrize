(function () {
  'use strict';

  angular.module('app.util')

    .directive('kzSpinner', [function () {
      return {
        restrict: 'E',
        template: '<div class="spinner" ng-show="$root.loading" ng-click="$event.stopPropagation();""><i class="fa fa-spinner fa-3x fa-spin fa-pulse text-muted"></i></div>'
      };
    }]);

})();
