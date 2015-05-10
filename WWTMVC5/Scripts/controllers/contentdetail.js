wwtng.controller('ContentDetail', ['$scope', 'dataproxy', '$timeout', '$routeParams','UIHelper', function ($scope, dataproxy, $timeout, $routeParams, uiHelper) {
    $scope.test = 'test';
    console.log($routeParams);
    dataproxy.getAllTypes().then(function(types) {
        $scope.types = types;
        console.log(types);
        dataproxy.getEntityDetail({ entityId: $routeParams.contentId })
            .then(function (entity) {
                
                $scope.entity = entity;
            });
    });

    $scope.confirmDelete = function() {
        bootbox.confirm("Delete this content?", function(result) {
            if (result) {
                dataproxy.deleteContent($scope.entity.Id).then(function() {
                    window.history.back();
                });
            }
        });
    };


   
    wwt.triggerResize();

}]);