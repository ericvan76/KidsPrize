(function() {
  'use strict';

  angular.module('app.util')

  .factory('Themes', ['$q', '$http', '$rootElement',
    function($q, $http, $rootElement) {

      var svc = {};

      svc.loadThemes = function() {
        // load themes
        var deferred = $q.defer();
        var themes = [{
          name: 'Default',
          cssCdn: null
        }];
        $http.get('http://api.bootswatch.com/3/')
          .success(function(data) {
            themes = themes.concat(data.themes);
            deferred.resolve(themes);
          })
          .error(function(err) {
            deferred.resolve(themes);
          });
        return deferred.promise;
      };

      svc.changeTheme = function(cssCdn) {
        if (cssCdn) {
          var bs = $rootElement[0].children[0].getElementsByTagName('link')[0];
          bs.href = cssCdn;
        }
      };

      return svc;

    }
  ]);

})();
