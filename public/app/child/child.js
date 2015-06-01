(function() {
  'use strict';

  angular.module('child')

  .controller('ChildCtrl', ['Child', 'msgBox', '$modal', '$scope', '$rootScope',
    function(Child, msgBox, $modal, $scope, $rootScope) {

      $scope.tabs = [];
      $rootScope.currentChild = null;

      Child.query({}, function(data) {
        $scope.tabs = data.map(function(e) {
          return {
            child: e,
            isopen: false
          };
        });
        if ($scope.tabs.length > 0) {
          $rootScope.currentChild = $scope.tabs[0].child;
        }
      });

      $scope.onClick = function($event, tab) {
        if ($scope.isSelected(tab)) {
          $event.preventDefault();
          $event.stopPropagation();
          tab.isopen = !tab.isopen;
        } else {
          $rootScope.currentChild = tab.child;
        }
      };

      $scope.isSelected = function(tab) {
        return $rootScope.currentChild === (tab.child || null);
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
            var tab = {
              child: child,
              isopen: false
            };
            $scope.tabs.push(tab);
            $rootScope.currentChild = child;
          });
        });

      };

      $scope.editChild = function(tab) {};

      $scope.deleteChild = function(tab) {
        var modalInstance = msgBox({
          type: 'question',
          size: 'md',
          commands: ['No', 'Yes'],
          title: 'Delete Child - ' + tab.child.name,
          content: [
            'This operation cannot be undone. Would you like to proceed?'
          ]
        });
        modalInstance.result.then(function(result) {
          var idx = $scope.tabs.indexOf(tab);
          if (result.toLowerCase() === 'yes') {
            tab.child.$delete(function() {
              $scope.tabs.splice(idx, 1);
              if (idx > 0) {
                idx -= 1;
              }
              if ($scope.tabs.length > 0) {
                $rootScope.currentChild = $scope.tabs[idx].child;
              } else {
                $rootScope.currentChild = null;
              }
            });
          }
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
