(function() {
  'use strict';

  angular.module('app.util')

  .factory('groupby', [function() {
    return function groupby(arr, key) {
      var result = [];
      arr.forEach(function(element, index) {
        if (!result.some(function(e) {
            return e.key === element.key;
          })) {
          result.push({
            key: element.key,
            values: arr.filter(function(x) {
              return x.key === element.key;
            })
          });
        }
      });
      return result;
    };
  }]);

})();
