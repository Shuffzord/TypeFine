/// <reference path="../TypeLite.Net4.d.ts" />
declare var ViewBag: any;

module Controllers
{
    export interface IWordOfDayControllerScope extends IControllerScope
    {
        open: Function;
        close: Function;
        isNewDay: Function;
        isOpen: boolean;
        word: Models.WordOfDayModel;
        getWord: Function;
        cache: ng.ICacheObject;
    }

    export class WordOfDayController extends ControllerBase
    {
        static $inject = (() => {
            var temp = ControllerBase.$inject.slice();
            return temp.concat(['$http','$cacheFactory']);
        })();
        static cacheId = "wordOfDay";
        static lastWord = "lastWordOfDay";

        static CompareDates(date1: Date, date2: Date)
        {
            return (
                date1.getFullYear() == date2.getFullYear()
                && date1.getMonth() == date2.getMonth()
                && date1.getDay() == date2.getDay()
                );
        }

        constructor(
            public $scope: IWordOfDayControllerScope,
            public $q: IControllerQ,
            public $sce: IControllerSce,
            public $http: IControllerHttp,
            public $cacheFactory: ICacheFactoryService
        )
        {
            super($scope, $q,$sce);

            $scope.cache = $cacheFactory(WordOfDayController.cacheId);
         
            $scope.open = () =>
            {
                $scope.cache.put(WordOfDayController.lastWord, Date.now);

                if (!!$scope.word)
                    $scope.isOpen = true;
                else
                {
                    $scope.getWord();
                }
            };

            $scope.getWord = ()=> {
                $http.get(ViewBag.w, {
                        params: {}
                    })
                    .success((response: Models.WordOfDayModel)=>
                    {
                        if (response.Success)
                        {
                            $scope.word = response;
                            $scope.open();
                        }
                    })
                    .error(()=> {});
            };

            $scope.close = ()=> {
                $scope.isOpen = false;
            };

            $scope.isNewDay = ()=>
            {
                var dateInCache = $scope.cache.get(WordOfDayController.lastWord);
                if (!dateInCache) {
                    $scope.cache.put(WordOfDayController.lastWord, Date.now);
                }
                else
                {
                    var dateNow = new Date();
                    var dateFromCache = new Date(dateInCache);
                    return WordOfDayController.CompareDates(dateNow, dateFromCache);
                }
                return false;
            };
        }
    }
}