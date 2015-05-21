wwtng.controller('EditCommunity', [
    '$scope',
    'dataproxy',
    '$timeout',
    '$routeParams',
    'UIHelper',
    'FileUploader',
    '$q',
    function ($scope, dataproxy, $timeout, $routeParams, uiHelper, fileUploader,$q) {
        
        $scope.thumbuploader = new fileUploader({
            url: "/Entity/AddThumbnail/Community",
            autoUpload: true,
            onSuccessItem: successThumb,
            onErrorItem: log,
            alias: 'thumbnail'
        });
        
        function log() {
            console.log({ uploader: arguments });
        }

        function successThumb(xhr, response) {
            $timeout(function () {
                $scope.community.ThumbnailID = response.ThumbnailID;
            });
        }

        function init() {
            uiHelper.fixLinks('profileLink');
            $scope.isEdit = positiveIntParamExists('communityId');
            $scope.isChildCommunity = positiveIntParamExists('parentId');
            dataproxy.requireAuth().then(function(types) {
                $scope.types = types;
                dataproxy.getUserCommunityList().then(function (response) {
                    var communityList = response;
                    $.each(communityList, function (i, item) {
                        item.val = item.Value;
                    });
                    $scope.communities = communityList;
                    if (!communityList.length) {
                        location.href = '#/';
                        return;
                    }
                    var setAndDisableParentDropdown = function (parentId) {
                        $('#lstCommunity').val(parentId);
                        $('#lstCommunity').prop('disabled', true);
                    }
                    if ($scope.isEdit) {
                        dataproxy.getCommunityDetail($routeParams.communityId, true)
                            .then(function (response) {
                                var community = response.community;
                                $scope.permission = response.permission;
                                if (response.permission === null) {
                                    location.href = '#/CommunityDetail/' + $routeParams.communityId;
                                }
                                //community.CategoryID = community.Category;
                                //community.AccessTypeID = community.AccessType;
                                delete community.ParentList;
                                delete community.CategoryList;
                                $scope.community = community;

                                setTimeout(function () {
                                    setAndDisableParentDropdown(community.ParentID);
                                }, 100);

                            });
                    } else {

                        $timeout(function () {
                            $scope.community = {
                                CategoryID: 9,
                                ParentID: '?',
                                AccessTypeID: 2,
                                IsOffensive: false,
                                IsLink: false,
                                CommunityType: 'Community'
                            };
                            $scope.CategoryName = "General Interest";
                            if ($scope.isChildCommunity) {
                                setAndDisableParentDropdown($scope.parentId);
                            } else {
                                $timeout(function () {
                                    var opt = $('#lstCommunity option[label="None"]').first();
                                    opt.prop('selected', true);
                                    $('#lstCommunity').trigger('change');
                                }, 500);
                            }
                        }, 100);
                    }
                });
            });

        };

        var positiveIntParamExists = function(param) {
            var exists = $routeParams && $routeParams[param] && parseInt($routeParams[param]) > 0;
            if (exists) {
                $scope[param] = $routeParams[param];
            }
            return exists;
        };

        $scope.saveEditedCommunity = function () {
            if (!$scope.isEdit) {
                $scope.community.ParentID = positiveIntParamExists('parentId') ? $routeParams.parentId : $scope.community.ParentID.val;
            }
            console.log($scope.community);
            dataproxy.saveEditedCommunity($scope.community)
                .then(function (response) {
                    console.log(this, arguments);
                    if (!response.error && response.id) {
                        location.href = '#/CommunityDetail/' + response.id;
                    }
                    else {
                        bootbox.dialog("There was an error saving changes to this community.");
                    }
                });
        };

        $scope.createCommunity = function () {
            $scope.community.ParentID = $scope.isChildCommunity ? $routeParams.parentId : $scope.community.ParentID.val;
            console.log($scope.community);
            dataproxy.createCommunity($scope.community)
                .then(function (response) {
                    console.log(arguments);
                    if (response.ID) {
                        location.href = '#/CommunityDetail/' + response.ID;
                    } else {
                        bootbox.dialog("There was an error creating this community.");
                    }
                });
        };
        init();

    }]);