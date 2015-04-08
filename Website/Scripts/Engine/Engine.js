var engine = angular.module('engineModule', []).config(function ($locationProvider) {
    $locationProvider.html5Mode(true);
});
engine.controller('ngSearchController', Controllers.SearchController);
engine.controller('ngWordOfDayController', Controllers.WordOfDayController);
