(function() {
  'use strict';

  angular.module('child')

  .controller('ChildCtrl', ['Child', '$modal', '$scope', '$rootScope',
    function(Child, $modal, $scope, $rootScope) {

      $scope.children = [];
      $rootScope.currentChild = null;

      $scope.children = Child.query({}, function(data) {
        if (data.length > 0) {
          $rootScope.currentChild = data[0];
        }
      });

      $scope.setCurrent = function(child) {
        $rootScope.currentChild = child;
      };

      $scope.isCurrent = function(child) {
        return $rootScope.currentChild === child;
      };

      $scope.addChild = function() {

        var modalInstance = $modal.open({
          animation: true,
          templateUrl: 'add-child.html',
          controller: 'AddChildCtrl',
          size: 'sm'
        });

        modalInstance.result.then(function(newChild) {
          newChild.$save(function(child) {
            $scope.children.push(child);
            $rootScope.currentChild = child;
          });
        });

      };

    }
  ])

  .directive('kzChildren', function() {
    return {
      restrict: 'E',
      templateUrl: 'children.html'
    };
  });


})();
