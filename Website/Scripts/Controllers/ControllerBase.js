/// <reference path="../TypeLite.Net4.d.ts" />
/// <reference path="../typings/angularjs/angular.d.ts" />
var Controllers;
(function (Controllers) {
    var ControllerBase = (function () {
        function ControllerBase($scope, $q, $sce) {
            this.$scope = $scope;
            this.$q = $q;
            this.$sce = $sce;
            $scope.to_trusted = function (code) {
                return $sce.trustAsHtml(code);
            };
        }
        ControllerBase.$inject = ['$scope', '$q', '$sce'];
        return ControllerBase;
    })();
    Controllers.ControllerBase = ControllerBase;
})(Controllers || (Controllers = {}));
//# sourceMappingURL=ControllerBase.js.map
