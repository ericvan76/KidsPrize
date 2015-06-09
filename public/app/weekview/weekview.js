(function() {
  'use strict';

  angular.module('weekview')

  .controller('WeekviewCtrl', ['Child', 'Score', 'Payment', 'groupby', '$scope', '$rootScope', '$modal',
    function(Child, Score, Payment, groupby, $scope, $rootScope, $modal) {

      $scope.today = null;
      $scope.thisWeek = null;
      $scope.currentWeek = null;
      $scope.dates = [];
      $scope.tasks = [];
      $scope.scores = {}; // hash set
      $scope.total = {
        _week: 0,
        _total: 0,
        _payments: 0,
        get week() {
          return Object.keys($scope.scores).map(function(k) {
            return $scope.scores[k].value || 0;
          }).reduce(function(a, b) {
            return a + b;
          }, 0);
        },
        get total() {
          return this._total + this.week - this._week;
        },
        get balance() {
          return this.total - this._payments;
        }
      };

      $rootScope.$watch('currentChild', function() {
        $scope.updateWeekScores();
        $scope.updatePayments();
        $scope.updateTotal();
      }, true);

      $scope.week = function(offset) {
        $scope.today = new Date(Date.today().toYMD());
        $scope.thisWeek = $scope.today.clone().add({
          days: -1 * $scope.today.getDay()
        });
        if (offset === 0 || $scope.currentWeek === null) {
          $scope.currentWeek = $scope.thisWeek;
        } else {
          $scope.currentWeek.add({
            weeks: offset
          });
        }
        $scope.dates = [0, 1, 2, 3, 4, 5, 6].map(function(i) {
          return $scope.currentWeek.clone().add({
            days: i
          });
        });
        $scope.updateWeekScores();
        $scope.updateTotal();
      };

      $scope.updateWeekScores = function() {
        if (!$rootScope.currentChild || !$scope.currentWeek) {
          $scope.tasks = [];
          $scope.scores = {};
          $scope.total._week = 0;
        } else {
          var q = {
            _child: $rootScope.currentChild._id,
            date: {
              $gte: $scope.currentWeek,
              $lt: $scope.currentWeek.clone().addWeeks(1)
            }
          };
          // opt-out removed tasks
          if ($scope.currentWeek.compareTo($scope.thisWeek) >= 0) {
            q.task = {
              $in: $rootScope.currentChild.tasks
            };
          }
          // query
          Score.query(q, function(data) {
            if ($scope.currentWeek.compareTo($scope.thisWeek) >= 0) {
              $scope.tasks = $rootScope.currentChild.tasks || [];
            } else {
              $scope.tasks = groupby(data, 'task').map(function(g) {
                return {
                  task: g.key,
                  order: g.values.reduce(function(a, b) {
                    return new Date(a.update_at).compareTo(new Date(b.update_at)) > 0 ? a : b;
                  }).order || -1
                };
              }).sort(function(a, b) {
                return (a.order || -1) - (b.order || -1);
              }).map(function(x) {
                return x.task;
              });
            }
            $scope.scores = {};
            data.forEach(function(e) {
              e.date = new Date(e.date);
              var key = e.date.toISOString() + '|' + e.task;
              $scope.scores[key] = e;
            });
            $scope.total._week = data.map(function(e) {
              return e.value || 0;
            }).reduce(function(a, b) {
              return a + b;
            }, 0);
          });
        }
      };

      $scope.updateTotal = function() {
        if (!$rootScope.currentChild) {
          $scope.total._total = 0;
        } else {
          Score.total({
            _child: $rootScope.currentChild._id
          }, function(d) {
            $scope.total._total = d.total;
          });
        }
      };

      $scope.updatePayments = function() {
        if (!$rootScope.currentChild) {
          $scope.total._payments = 0;
        } else {
          Payment.total({
            _child: $rootScope.currentChild._id
          }, function(d) {
            $scope.total._payments = d.total;
          });
        }
      };

      $scope.getScore = function(d, task) {
        var key = d.toISOString() + '|' + task;
        if (key in $scope.scores) {
          return $scope.scores[key].value || 0;
        }
        return 0;
      };

      $scope.toggleScore = function(d, task) {
        var key = d.toISOString() + '|' + task;
        if (key in $scope.scores) {
          if ($scope.scores[key].value === 1) {
            $scope.scores[key].value = 0;
          } else {
            $scope.scores[key].value = 1;
          }
        } else {
          $scope.scores[key] = new Score({
            _child: $rootScope.currentChild._id,
            date: d,
            task: task,
            value: 1
          });
        }
        $scope.scores[key].order = $scope.tasks.indexOf(task);
        $scope.scores[key].$save(
          function(s) {
            $scope.scores[key] = s;
          });
      };

      $scope.editTasks = function() {
        var modalInstance = $modal.open({
          animation: true,
          templateUrl: 'edit-tasks.html',
          controller: 'EditTasksCtrl',
          windowClass: 'edit-tasks',
          size: 'md',
          resolve: {
            child: function() {
              return $rootScope.currentChild;
            },
            thisWeek: function() {
              return $scope.thisWeek;
            }
          }
        });

        modalInstance.result.then(function(tasks) {
          $rootScope.currentChild.tasks = tasks;
        });
      };

      $scope.payment = function() {
        var modalInstance = $modal.open({
          animation: true,
          templateUrl: 'payment.html',
          controller: 'PaymentCtrl',
          windowClass: 'payment',
          size: 'md',
          resolve: {
            child: function() {
              return $rootScope.currentChild;
            },
            balance: function() {
              return $scope.total.balance;
            }
          }
        });

        modalInstance.result.then(function() {
          $scope.updatePayments();
        });

      };
    }
  ])

  .directive('kzWeekview', [function() {
    return {
      restrict: 'E',
      templateUrl: 'weekview.html'
    };
  }]);



})();
