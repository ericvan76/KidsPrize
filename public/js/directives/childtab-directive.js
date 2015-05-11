(function() {
  'use strict';

  angular.module('app.directives')
    .directive('childtab', ['$http', function($http) {

      return {
        restrict: 'E',
        templateUrl: '/partials/childtab',
        controller: ['$scope', '$rootScope', function($scope, $rootScope) {

          $scope.children = [];
          
          $http.get('/api/child')
            .success(function(data, status) {
              $scope.children = data;
              $scope.selectedChild = data[0].id;
            })
            .error(function(data, status) {
              console.log(status);
            });

          $scope.setChild = function(childId) {
            $scope.selectedChild = childId;
          };

        }],
        controllerAs: 'childTabCtrl'
      };
    }]);
})();
