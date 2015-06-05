wwtng.controller('CommunityDetail', [
    '$scope', 'dataproxy', '$timeout', '$routeParams', '$http','UIHelper',
    function ($scope, dataproxy, $timeout, $routeParams, $http, uiHelper) {
        $scope.communityId = $routeParams.communityId;
        wwt.triggerResize();
        var init = function() {
            dataproxy.requireAuth().then(function(types) {
                $scope.types = types;
                console.log(types);
                getCommunityDetail();
            }, function(reason) {
                dataproxy.getAllTypes().then(function(types) {
                    $scope.types = types;
                    console.log(types);
                    getCommunityDetail();
                });
            });
        }



        function getCommunityDetail() {
            dataproxy.getCommunityDetail($routeParams.communityId).then(function (response) {
                $scope.community = response.community;
                $scope.permission = response.permission;
                $scope.userCanEdit = (response.permission && response.permission.CurrentUserPermission === 63) || $scope.types.currentUserId === response.community.ProducerId || $scope.types.isAdmin;
                $scope.loadingContent = true;
                uiHelper.imageHelper('.img-thumbnail');
                dataproxy.getCommunityContents($scope.communityId).then(function (response) {
                    $scope.loadingContent = false;
                    $scope.community.contents = response.entities;
                    $scope.community.communities = response.childCommunities;
                    wwt.triggerResize();
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

        $scope.confirmDelete = function() {
            bootbox.confirm("Delete this community and all its contents?", function (result) {
                if (result) {
                    dataproxy.deleteCommunity($routeParams.communityId, $scope.community.ParentId).then(function (success) {
                        if (success){location.href = '#/MyProfile';}
                    });
                }
            });
        }

        init();
    }]);
