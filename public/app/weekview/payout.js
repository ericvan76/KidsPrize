(function() {
  'use strict';

  angular.module('payout')

  .controller('PayoutCtrl', ['child', 'balance', 'Payout', '$scope', '$modalInstance',
    function(child, balance, Payout, $scope, $modalInstance) {

      $scope.child = child;
      $scope.balance = balance;
      $scope.payouts = [];
      $scope.newPayout = createNew();

      function createNew() {
        return new Payout({
          _child: $scope.child._id,
          description: null,
          amount: 0
        });
      }

      $scope.query = function() {
        Payout.query({
          _child: $scope.child._id,
          update_at: {
            $gt: Date.today().addYears(-1)
          }
        }, function(data) {
          $scope.payouts = data || [];
        });
      };

      $scope.submit = function() {
        $scope.newPayout.$save(function(data) {
          $scope.payouts.push(data);
          $scope.balance -= data.amount;
          $scope.newPayout = createNew();
          $scope.form.$setPristine(true);
        });
      };

      $scope.close = function() {
        $modalInstance.close();
      };
    }
  ]);

})();
