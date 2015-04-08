/// <reference path="../TypeLite.Net4.d.ts" />
declare var ViewBag: any;

module Controllers
{
    export interface ISearchControllerScope extends IControllerScope
    {
        items: Array<any>;
        matchedIndex: number;
        searchBox: any;
        more: boolean;
        page: number;
        isAnyWordInSearch: Function;
        loading: boolean;
        noResults: boolean;
        triggerSearch: Function;
        loadMore: Function;
        triggerMatch:Function;
    }

    export class SearchController extends ControllerBase {

        static $inject = (()=> {
            var temp = ControllerBase.$inject.slice();
            return temp.concat(['$timeout', '$http',  '$location', '$window']);
        })();

        constructor(
            public $scope: ISearchControllerScope,
            public $q: IControllerQ, 
            public $sce: IControllerSce,
            public $timeout: IControllerTimeout,
            public $http: IControllerHttp,
            public $location: ILocationService,
            public $window: IWindowService
            ) {
            super($scope, $q, $sce);

            if ($location.absUrl().lastIndexOf('#') > 0)
            {
               if (!ViewBag.query)
                ViewBag.query = $location.search().q;
            }

            $scope.items = [];

            $scope.matchedIndex = -1;
            $scope.searchBox = ViewBag.query;
            $scope.more = false;
            $scope.page = 0;
       
            $scope.isAnyWordInSearch = ()=> {
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

            $scope.triggerMatch = (keyword, phrase, index)=> {
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

        public canceler: any;

        public search()
        {
            this.canceler.resolve();
            this.canceler = this.$q.defer();

            if (!this.$scope.isAnyWordInSearch())
            {
                return;
            }

            this.$scope.loading = true;

            var $scope = this.$scope;
            var $location = this.$location;
            this.$http
                .get(ViewBag.q,
                    {
                        timeout: this.canceler.promise,
                        params:
                        {
                            value: this.$scope.searchBox,
                            skip: !!this.$scope.page
                        }
                    })
                .success((response: Models.SearchResponseModel)=> {
                    $scope.loading = false;
                    if (response.Success)
                    {
                        if (response.Phrases == null || !response.Phrases.length)
                        {
                            $scope.items = [];
                            $scope.noResults = true;
                            return;
                        }

                        if (!$scope.page)
                        {
                            $scope.items = [];
                        }

                        if (response.Keyword != $scope.searchBox)
                        {
                            $scope.items = [];
                            return;
                        }

                        $scope.noResults = false;
                        var temp = [];
                        angular.forEach(response.Phrases, function(item : Models.PhraseModel)
                        {
                            this.push({ keyword: response.Keyword, comment: item.Comment, title: item.Right });
                        }, temp);
                        $scope.items = temp;
                        $scope.loading = false;
                        var current = $location.search();
                        if(current.q != response.Keyword)
                            $location.search('q', response.Keyword);
                    }
                })
                .error(()=> {
                    $scope.items = [];
                    $scope.loading = false;
            });
    }

        static isNullOrWhiteSpace(str)
        {
            return str === null || str === undefined || str.match(/^ *$/) !== null;
        }

    }
}