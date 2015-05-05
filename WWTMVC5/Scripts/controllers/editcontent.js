wwtng.controller('EditContent', [
    '$scope', 'dataproxy', '$timeout', '$routeParams', '$http',
    function ($scope, dataproxy, $timeout, $routeParams, $http) {
        $scope.contentId = $routeParams.contentId;
        wwt.triggerResize();

    }]);