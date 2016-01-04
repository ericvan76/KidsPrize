(function () {
  'use strict';

  angular.module('app.util')

    .factory('Themes', ['$q', '$http', '$rootElement',
      function ($q, $http, $rootElement) {

        var svc = {};

        /**
         * Loads themes
         * @return {Promise} [description]
         */
        svc.loadThemes = function () {
          // load themes
          var deferred = $q.defer();
          var themes = [{
            name: 'Default',
            cssCdn: bootstrapElem().href
          }];
          $http.get('http://bootswatch.com/api/3.json')
            .success(function (data) {
              themes = themes.concat(data.themes);
              deferred.resolve(themes);
            })
            .error(function (err) {
              deferred.resolve(themes);
            });
          return deferred.promise;
        };

        /**
         * Changes theme
         * @param  {String} cssCdn [description]
         */
        svc.changeTheme = function (cssCdn) {
          if (cssCdn) {
            bootstrapElem().href = cssCdn;
          }
        };

        /**
         * Gets bootstrap link element from html
         * @return {Element} [description]
         */
        function bootstrapElem() {
          return $rootElement[0].children[0].getElementsByTagName('link')[0];
        }

        return svc;

      }
    ]);

})();
