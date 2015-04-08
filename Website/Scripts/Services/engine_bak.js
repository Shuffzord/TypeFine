var engine = angular.module('engineModule', []);

engine.controller('searchController', [
    '$scope', '$timeout', '$http', '$sce', '$q', function($scope, $timeout, $http, $sce, $q)
    {
        var q = ViewBag.q;
        var m = ViewBag.m;

        $scope.to_trusted = function(code)
        {
            return $sce.trustAsHtml(code);
        };
        $scope.items = [];
        $scope.matchedIndex = -1;
        $scope.searchBox = ViewBag.query;
        $scope.more = false;
        $scope.page = 0;
        $scope.isAnyWordInSearch = function()
        {
            var str = $scope.searchBox;
            return !isNullOrWhiteSpace(str);
        };

        function isNullOrWhiteSpace(str)
        {
            return str === null || str.match(/^ *$/) !== null;
        }

        var canceler = $q.defer();

        function search()
        {
            canceler.resolve();
            canceler = $q.defer();

            if (!$scope.isAnyWordInSearch())
            {
                return;
            }

            $scope.loading = true;
            $http
                .get(q, { timeout: canceler.promise, params: { value: $scope.searchBox, skip: !!$scope.page } })
                .success(function(response)
                {
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
                        angular.forEach(response.Phrases, function(value, key)
                        {
                            this.push({ keyword: value.Keyword, comment: value.Comment, title: value.Right });
                        }, temp);
                        $scope.items = temp;
                        $scope.loading = false;
                    }
                })
                .error(function(response)
                {
                    $scope.items = [];
                    $scope.loading = false;
                });
        }

        $scope.triggerSearch = function()
        {
            $scope.page = 0;
            search();
        };

        $scope.loadMore = function()
        {
            $scope.page = 1;
            search();
        };

        $scope.triggerMatch = function(keyword, phrase, index)
        {
            if ($scope.matchedIndex > 0)
                return;

            $scope.matchedIndex = index;

            $http.get(m, { params: { keyword: keyword, phrase: phrase } });
        };

        $scope.loading = false;

        $scope.$watch('searchBox', function(val)
        {
            if (!val)
            {
                $scope.items = [];
                $scope.loading = false;
                return;
            }
        });

        var filterTextTimeout;
        $scope.$watch('searchBox', function(val)
        {
            if (filterTextTimeout)
                $timeout.cancel(filterTextTimeout);

            if (!val || isNullOrWhiteSpace(val))
            {
                $scope.items = [];
                $scope.loading = false;
                return;
            }

            filterTextTimeout = $timeout(function()
            {
                $scope.triggerSearch();
            }, 300);
        });

        $scope.$watchCollection('items', function(newArr)
        {
            $scope.matchedIndex = -1;

            if ($scope.page > 0)
            {
                $scope.more = false;
                return;
            }

            if (newArr.length < 6 && newArr.length > 0)
            {
                $scope.more = true;
            }
            if (newArr.length == 0 || newArr.length > 5)
            {
                $scope.more = false;
            }
        });
    }]);