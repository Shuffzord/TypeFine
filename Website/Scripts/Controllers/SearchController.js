var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};

var Controllers;
(function (Controllers) {
    var SearchController = (function (_super) {
        __extends(SearchController, _super);
        function SearchController($scope, $q, $sce, $timeout, $http, $location, $window) {
            _super.call(this, $scope, $q, $sce);
            this.$scope = $scope;
            this.$q = $q;
            this.$sce = $sce;
            this.$timeout = $timeout;
            this.$http = $http;
            this.$location = $location;
            this.$window = $window;

            if ($location.absUrl().lastIndexOf('#') > 0) {
                if (!ViewBag.query)
                    ViewBag.query = $location.search().q;
            }

            $scope.items = [];

            $scope.matchedIndex = -1;
            $scope.searchBox = ViewBag.query;
            $scope.more = false;
            $scope.page = 0;

            $scope.isAnyWordInSearch = function () {
                var str = $scope.searchBox;
                return !SearchController.isNullOrWhiteSpace(str);
            };

            this.canceler = $q.defer();
            var that = this;
            $scope.triggerSearch = function () {
                $scope.page = 0;
                that.search();
            };

            $scope.loadMore = function () {
                $scope.page = 1;
                that.search();
            };

            $scope.triggerMatch = function (keyword, phrase, index) {
                if ($scope.matchedIndex > 0)
                    return;

                $scope.matchedIndex = index;

                $http.get(ViewBag.m, { params: { keyword: keyword, phrase: phrase } });
            };

            $scope.loading = false;

            $scope.$watch('searchBox', function (val) {
                if (!val) {
                    $scope.items = [];
                    $scope.loading = false;
                    return;
                }
            });

            var filterTextTimeout;

            $scope.$watch('searchBox', function (val) {
                if (filterTextTimeout)
                    $timeout.cancel(filterTextTimeout);

                if (!val || SearchController.isNullOrWhiteSpace(val)) {
                    $scope.items = [];
                    $scope.loading = false;
                    $location.search('');
                    return;
                }

                filterTextTimeout = $timeout(function () {
                    $scope.triggerSearch();
                }, 300);
            });

            $scope.$watchCollection('items', function (newArr) {
                $scope.matchedIndex = -1;

                if ($scope.page > 0) {
                    $scope.more = false;
                    return;
                }

                if (newArr.length < 6 && newArr.length > 0) {
                    $scope.more = true;
                }
                if (newArr.length == 0 || newArr.length > 5) {
                    $scope.more = false;
                }
            });
        }
        SearchController.prototype.search = function () {
            this.canceler.resolve();
            this.canceler = this.$q.defer();

            if (!this.$scope.isAnyWordInSearch()) {
                return;
            }

            this.$scope.loading = true;

            var $scope = this.$scope;
            var $location = this.$location;
            this.$http.get(ViewBag.q, {
                timeout: this.canceler.promise,
                params: {
                    value: this.$scope.searchBox,
                    skip: !!this.$scope.page
                }
            }).success(function (response) {
                $scope.loading = false;
                if (response.Success) {
                    if (response.Phrases == null || !response.Phrases.length) {
                        $scope.items = [];
                        $scope.noResults = true;
                        return;
                    }

                    if (!$scope.page) {
                        $scope.items = [];
                    }

                    if (response.Keyword != $scope.searchBox) {
                        $scope.items = [];
                        return;
                    }

                    $scope.noResults = false;
                    var temp = [];
                    angular.forEach(response.Phrases, function (item) {
                        this.push({ keyword: response.Keyword, comment: item.Comment, title: item.Right });
                    }, temp);
                    $scope.items = temp;
                    $scope.loading = false;
                    var current = $location.search();
                    if (current.q != response.Keyword)
                        $location.search('q', response.Keyword);
                }
            }).error(function () {
                $scope.items = [];
                $scope.loading = false;
            });
        };

        SearchController.isNullOrWhiteSpace = function (str) {
            return str === null || str === undefined || str.match(/^ *$/) !== null;
        };
        SearchController.$inject = (function () {
            var temp = Controllers.ControllerBase.$inject.slice();
            return temp.concat(['$timeout', '$http', '$location', '$window']);
        })();
        return SearchController;
    })(Controllers.ControllerBase);
    Controllers.SearchController = SearchController;
})(Controllers || (Controllers = {}));
