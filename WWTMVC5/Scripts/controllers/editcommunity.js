wwtng.controller('EditCommunity', [
    '$scope', 'dataproxy', '$timeout', '$routeParams', '$http',
    function ($scope, dataproxy, $timeout, $routeParams, $http) {
        $scope.communityId = $routeParams.communityId;
        wwt.triggerResize();

    }]);