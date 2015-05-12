wwtng.controller('CommunityDetail', [
    '$scope', 'dataproxy', '$timeout', '$routeParams', '$http',
    function ($scope, dataproxy, $timeout, $routeParams, $http) {
        $scope.communityId = $routeParams.communityId;
        wwt.triggerResize();
        var init = function() {
            dataproxy.getAllTypes().then(function(types) {
                $scope.types = types;
                console.log(types);
                dataproxy.getCommunityDetail($routeParams.communityId).then(function(response) {
                    $scope.community = response.community;
                    $scope.permission = response.permission;
                    console.log(response);
                    dataproxy.getCommunityContents($scope.communityId).then(function(response) {
                        $scope.community.contents = response.entities;
                        $scope.community.communities = response.childCommunities;
                        wwt.triggerResize();
                    });
                });
            });
        }

        $scope.options = {activeTab:'contents'}

        $scope.tabChange = function (tab) {
            $scope.options.activeTab = tab;
            wwt.triggerResize();
        }

        $scope.joinCommunity = function() {
            dataproxy.joinCommunity($routeParams.communityId, $('#lstRole').val());
        }

        init();
    }]);
