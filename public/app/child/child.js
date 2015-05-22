(function() {
  'use strict';

  angular.module('child', ['ngResource', 'app.util'])

  .controller('ChildCtrl', ['$http', '$modal', '$scope', '$rootScope', 'Child',
    function($http, $modal, $scope, $rootScope, Child) {

      $scope.children = [];
      $scope.currentChild = null;

      $scope.children = Child.query({}, function(data) {
        if (data.length > 0) {
          $scope.currentChild = data[0]._id;
        }
      });

      $scope.setCurrent = function(childId) {
        $scope.currentChild = childId;
      };

      $scope.isCurrent = function(childId) {
        return $scope.currentChild === childId;
      };

      $scope.addChild = function() {

        var modalInstance = $modal.open({
          animation: true,
          templateUrl: 'add-child.html',
          controller: 'AddChildCtrl',
          size: 'sm'
        });

        modalInstance.result.then(function(newChild) {
          var child = Child.save(newChild, function() {
            $scope.children.push(child);
            $scope.currentChild = child._id;
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

  .controller('AddChildCtrl', ['$scope', '$modalInstance', function($scope, $modalInstance) {

    $scope.child = {
      name: null,
      gender: null,
      tasks: [
        'Finish breakfast in 20 mins',
        'Eating in good manners',
        'Bath before 7pm'
      ]
    };

    $scope.add = function() {
      $modalInstance.close($scope.child);
    };

    $scope.cancel = function() {
      $modalInstance.dismiss();
    };
  }])

  .factory('Child', ['$resource', function($resource) {
    return $resource('/api/child/:id', {
      id: '@id'
    }, {
      'update': {
        method: 'PUT'
      }
    });
  }]);

})();
