var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};

var Controllers;
(function (Controllers) {
    var WordOfDayController = (function (_super) {
        __extends(WordOfDayController, _super);
        function WordOfDayController($scope, $q, $sce, $http, $cacheFactory) {
            _super.call(this, $scope, $q, $sce);
            this.$scope = $scope;
            this.$q = $q;
            this.$sce = $sce;
            this.$http = $http;
            this.$cacheFactory = $cacheFactory;

            $scope.cache = $cacheFactory(WordOfDayController.cacheId);

            $scope.open = function () {
                $scope.cache.put(WordOfDayController.lastWord, Date.now);

                if (!!$scope.word)
                    $scope.isOpen = true;
                else {
                    $scope.getWord();
                }
            };

            $scope.getWord = function () {
                $http.get(ViewBag.w, {
                    params: {}
                }).success(function (response) {
                    if (response.Success) {
                        $scope.word = response;
                        $scope.open();
                    }
                }).error(function () {
                });
            };

            $scope.close = function () {
                $scope.isOpen = false;
            };

            $scope.isNewDay = function () {
                var dateInCache = $scope.cache.get(WordOfDayController.lastWord);
                if (!dateInCache) {
                    $scope.cache.put(WordOfDayController.lastWord, Date.now);
                } else {
                    var dateNow = new Date();
                    var dateFromCache = new Date(dateInCache);
                    return WordOfDayController.CompareDates(dateNow, dateFromCache);
                }
                return false;
            };
        }
        WordOfDayController.CompareDates = function (date1, date2) {
            return (date1.getFullYear() == date2.getFullYear() && date1.getMonth() == date2.getMonth() && date1.getDay() == date2.getDay());
        };
        WordOfDayController.$inject = (function () {
            var temp = Controllers.ControllerBase.$inject.slice();
            return temp.concat(['$http', '$cacheFactory']);
        })();
        WordOfDayController.cacheId = "wordOfDay";
        WordOfDayController.lastWord = "lastWordOfDay";
        return WordOfDayController;
    })(Controllers.ControllerBase);
    Controllers.WordOfDayController = WordOfDayController;
})(Controllers || (Controllers = {}));
