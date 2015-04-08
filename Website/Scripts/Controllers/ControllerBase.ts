/// <reference path="../TypeLite.Net4.d.ts" />
/// <reference path="../typings/angularjs/angular.d.ts" />

module Controllers
{
    export class ControllerBase
    {
        constructor(
            public $scope: IControllerScope,
            public $q: IControllerQ,
            public $sce: IControllerSce
        )
        {
            $scope.to_trusted = code => $sce.trustAsHtml(code);
        }
        static $inject = ['$scope', '$q', '$sce'];
    }
}