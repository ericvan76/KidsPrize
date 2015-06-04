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

      $scope.onClick = function(tab, $event) {
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
        return $scope.editChild(null);
      };

      $scope.editChild = function(tab) {

        var modalInstance = $modal.open({
          animation: true,
          templateUrl: 'edit-child.html',
          controller: 'EditChildCtrl',
          size: 'sm',
          resolve: {
            child: function() {
              if (tab) {
                return angular.copy(tab.child);
              } else {
                return new Child({
                  name: null,
                  gender: null,
                  tasks: [
                    'Finish breakfast in 20 mins',
                    'Eating in good manners',
                    'Bath before 7pm'
                  ]
                });
              }
            }
          }
        });

        modalInstance.result.then(function(child) {
          if (tab) {
            tab.child = child;
            $rootScope.currentChild = child;
          } else {
            var newtab = {
              child: child,
              isopen: false
            };
            $scope.tabs.push(newtab);
            $rootScope.currentChild = child;
          }
        });
      };

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
