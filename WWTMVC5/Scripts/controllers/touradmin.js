wwtng.controller('TourAdmin', [
    '$scope', 'dataproxy', '$timeout', '$routeParams', '$http', 'UIHelper','$modal',
    function ($scope, dataproxy, $timeout, $routeParams, $http, uiHelper, $modal) {
        var communityId = $scope.communityId = 596915;
        wwt.triggerResize();
        var init = function () {
            if (!$('#tourAdmin').length) {
                return notAuthorized();
            }
            uiHelper.fixLinks("tourAdmin");
            dataproxy.requireAuth().then(function (types) {
                $scope.types = types;
                if (!types.isAdmin) {
                    return notAuthorized();
                }
                getCommunityDetail();
            }, function (reason) {
                dataproxy.getAllTypes().then(function (types) {
                    $scope.types = types;
                    console.log(types);
                    getCommunityDetail();
                });
            });
        }

        function notAuthorized() {
            location.href = '#/';
            return false;
        }

        function getCommunityDetail() {
            dataproxy.getCommunityDetail(communityId).then(function (response) {
                $scope.community = response.community;
                dataproxy.getCommunityContents(communityId).then(function (response) {
                    $scope.community.contents = response.entities;
                    $scope.community.communities = response.childCommunities;
                    wwt.triggerResize();
                });
            });
        }

        $scope.options = { activeTab: 'contents' }

        $scope.tabChange = function (tab) {
            $scope.options.activeTab = tab;
            wwt.triggerResize();
        }

        var newTourModal = $modal({
            scope: $scope,
            contentTemplate: '/content/views/modals/edittour.html',
            show: false,
            title: 'Add New Tour'
        });
        var editTourModal = $modal({
            scope: $scope,
            contentTemplate: '/content/views/modals/edittour.html',
            show: false,
            title: 'Edit Tour'
        });
        // Show when some event occurs (use $promise property to ensure the template has been loaded)
        $scope.showTourModal = function (tour) {
            $scope.tour = tour;
            if (tour) {
                editTourModal.$promise.then(editTourModal.show);
            } else {
                newTourModal.$promise.then(newTourModal.show);
            }
        };
        init();
    }]);
