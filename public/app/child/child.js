(function() {
  'use strict';

  angular.module('child', ['app.resource', 'app.util'])

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
        }, function() {
          // $log.info('Modal dismissed at: ' + new Date());
        });

      };

    }
  ])

  .directive('kzChildren', function() {
    return {
      restrict: 'E',
      templateUrl: 'children.html'
    };
  })

  .controller('AddChildCtrl', ['Child', '$scope', '$modalInstance',
    function(Child, $scope, $modalInstance) {

      $scope.child = new Child({
        name: null,
        gender: null,
        tasks: [
          'Finish breakfast in 20 mins',
          'Eating in good manners',
          'Bath before 7pm'
        ]
      });

      $scope.add = function() {
        $modalInstance.close($scope.child);
      };

      $scope.cancel = function() {
        $modalInstance.dismiss();
      };
    }
  ]);


})();
