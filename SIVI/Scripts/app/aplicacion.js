(function () {

    var app = angular.module("comprobacionApp", [])
        .directive('loading', ['$http', function ($http) {
            return {
                restrict: 'A',
                template: '<div class="loading"><img src="' + raiz + 'Resources/tenor.gif" style="width:75%" /><br/><centre><h1>CARGANDO...</h1></centre></div>',
                link: function (scope, elm, attrs) {
                    scope.isLoading = function () {
                        return $http.pendingRequests > 0;
                    };
                    scope.$watch(scope.isLoading, function (v) {
                        if (v) {
                            elm.show;
                        } else {
                            elm.hide;
                        }
                    });
                }
            };
        }]);
}());