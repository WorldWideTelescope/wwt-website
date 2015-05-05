wwtng.controller('TestResources', [
    '$scope', 'dataproxy', '$timeout', '$routeParams', '$http',
    function ($scope, entities, $timeout, $routeParams, $http) {
        $scope.result = '';

        $scope.getMyCommunities = function () {
            var req = {
                method: 'GET',
                url: '/Resource/Service/Communities',
                headers: {
                    'LiveUserToken': wwt.user.get('accessToken')
                },
                data: {}
            }//"74864cff8a8e546d40cbcfeed2f119f3"
            //"fe22f7464528b7d3"
            $http(req).
                success(function (data, status, headers, config) {
                    console.log(data);
                    $scope.result = data;
                }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                });
        };

        var curToken = "EwB4Aq1DBAAUGCCXc8wU/zFu9QnLdZXy+YnElFkAAdrGnaz8vFRvYNraR5HbefJNXL+nB3CNyOprzJAULox3RYoHa2T2daFHqmqcy7Y+RZN4uhd604ez/5GKurAQpMh3Ui7KUQYMxhiFbuzjnEKBmqaS3qbCm9Cijp2YJcjhG5tJ4Hcws8A9H24MU7XY8Ptjk3RZj1ekuF9OxT+LumvtlQGSmGNnc2qURfaOO1jQhFKMJ1oc/HvIUjp1PCphZBmV64/K4oEjVLwPZIAY4Hl+JQ8o4ME70Jzp7vk3CIfgTCirmk1kBk1DD4FUqmz0Qu1XbWCCI0qGCTOHqjUnZdk1ACmyPagXNH23eDLp/GMTshngbEo4k0tGpSVgHg00bHMDZgAACFlGJnSoDYflSAF6RmtmoaY5c1iHa60HZeMa9vKFTsqSEpT1lLfsy/BpEHqwtlkVvMwWLuC0bJbblLVuLrdTXdby/zL3u7k2HZmhEyn2W7XnVDDqJseeOnmLk8lOyOZHC8PyHCrBdqkOCXFcfzEBJ5SQrU7sGfCmmnRneszkBhJ9pjJSpL4UUvNe6uOHuGnyrjfcGEfurpUiVpf9fkNZK9LIXAEkiqEUoNOmK090KD+gbBT8OqJSyYdHP/cryulIacukkXynSuqO/4x+sm2nzYMAXuNNgv37sSi9Z6gYWtabadZcTq6inb6KuuT1LKaXOtw50d4NXnhYnfY3lgo8hltnjTqiwXe/7mYrxNOQvjDv5CuEWJW5ilLp2aqClJlFTwc9Upk1hxsp8UGFZ2EnFi6dlKXZ8bttqwTn6QIlE/6zAOi6oeg+C9t0cn8mndPoTd7XZAE=";

        $scope.getMyCommunitiesExt = function () {
            var req = {
                method: 'GET',
                url: 'http://wwtstaging.azurewebsites.net/Resource/Service/Communities',
                headers: {
                    'LiveUserToken': curToken
                },
                data: {}
            }
            $http(req).
                success(function (data, status, headers, config) {
                    console.log(data);
                    $scope.result = data;
                }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                });
        };
        $scope.getUser = function () {
            var req = {
                method: 'GET',
                url: 'http://wwtstaging.azurewebsites.net/Resource/Service/User',
                headers: {
                    'LiveUserToken': curToken
                },
                data: {}
            }
            $http(req).
                success(function (data, status, headers, config) {
                    console.log(data);
                    $scope.result = data;
                }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                });
        };
        $scope.postUser = function () {
            var req = {
                method: 'POST',
                url: 'http://wwtstaging.azurewebsites.net/Resource/Service/User',
                headers: {
                    'LiveUserToken': curToken
                },
                data: {}
            }
            $http(req).
                success(function (data, status, headers, config) {
                    console.log(data);
                    $scope.result = data;
                }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                });
        };

}]);