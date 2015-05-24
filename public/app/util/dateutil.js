(function() {
  'use strict';

  angular.module('app.util')

  .factory('DateUtil', [function() {

    return {

      getISODate: function(d) {
        return new Date((d || new Date()).toISOString().substr(0, 10));
      },

      addDays: function(d, offset) {
        var nd = new Date(d.getTime());
        return new Date(nd.setDate(nd.getDate() + offset));
      },

      getWeekStart: function(d, startFromMonday) {
        var offset = d.getDay();
        if (startFromMonday) {
          offset -= 1;
        }
        return this.addDays(d, -1 * offset);
      },

      getWeekDates: function(weekStart) {
        var dates = [];
        for (var i = 0; i < 7; i++) {
          dates.push(this.addDays(weekStart, i));
        }
        return dates;
      }

    };

  }]);

})();
