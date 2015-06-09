(function() {
  'use strict';

  angular.module('payment')

  .controller('PaymentCtrl', ['child', 'balance', 'Payment', '$scope', '$modalInstance',
    function(child, balance, Payment, $scope, $modalInstance) {

      $scope.child = child;
      $scope.balance = balance;
      $scope.payments = [];
      $scope.newPayment = createNew();

      function createNew() {
        return new Payment({
          _child: $scope.child._id,
          description: null,
          amount: 0
        });
      }

      $scope.query = function() {
        Payment.query({
          _child: $scope.child._id,
          update_at: {
            $gt: Date.today().addYears(-1)
          }
        }, function(data) {
          $scope.payments = data || [];
        });
      };

      $scope.submit = function() {
        $scope.newPayment.$save(function(data) {
          $scope.payments.push(data);
          $scope.balance -= data.amount;
          $scope.newPayment = createNew();
          $scope.form.$setPristine(true);
        });
      };

      $scope.close = function() {
        $modalInstance.close();
      };
    }
  ]);

})();
