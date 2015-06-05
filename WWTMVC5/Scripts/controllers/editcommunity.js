wwtng.controller('EditCommunity', [
    '$scope',
    'dataproxy',
    '$timeout',
    '$routeParams',
    'UIHelper',
    'FileUploader',
    '$q',
    function ($scope, dataproxy, $timeout, $routeParams, uiHelper, fileUploader, $q) {
        
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
            $scope.editedThumb = true;
            $timeout(function () {
                $scope.community.ThumbnailID = response.ThumbnailID;
            });
        }

        function init() {
            uiHelper.fixLinks('profileLink');
            $scope.isEdit = uiHelper.positiveIntParamExists('communityId', $routeParams, $scope);
            $scope.isChildCommunity = uiHelper.positiveIntParamExists('parentId', $routeParams, $scope);
            $scope.cancelHref = $scope.isEdit ? '#/CommunityDetail/' + $scope.communityId : '#/MyProfile';
            dataproxy.requireAuth().then(function(types) {
                $scope.types = types;
                dataproxy.getUserCommunityList().then(function (response) {
                    var communityList = response;
                    $.each(communityList, function (i, item) {
                        if ($scope.isChildCommunity && item.Value === $scope.parentId) {
                            $scope.ParentCommunity = item.Text;
                        }
                        item.val = item.Value;
                    });
                    $scope.communities = communityList;
                    if (!communityList.length) {
                        location.href = '#/';
                        return;
                    }
                    var setAndDisableParentDropdown = function (parentId) {
                        $scope.parentId = parentId;
                        $('#lstCommunity').val(parentId);
                        //$('#lstCommunity').prop('disabled', true);
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

        

        $scope.saveEditedCommunity = function () {
            if (!$scope.isEdit) {
                $scope.community.ParentID = uiHelper.positiveIntParamExists('parentId', $routeParams, $scope) ?
                    $routeParams.parentId : isNaN($scope.community.ParentID) ? $scope.community.ParentID.val : $scope.community.ParentID;
            }
            
            dataproxy.saveEditedCommunity($scope.community)
                .then(function (response) {
                    console.log("savecommunityresponse", arguments);
                    if (!response.error && response.id) {
                        $scope.community = null;
                        location.href = '#/CommunityDetail/' + response.id;
                    }
                    else {
                        bootbox.dialog("There was an error saving changes to this community.");
                    }
                });
        };

        $scope.createCommunity = function () {
            $scope.community.ParentID = $scope.isChildCommunity ? $routeParams.parentId : $scope.community.ParentID.val;
            
            dataproxy.createCommunity($scope.community)
                .then(function (response) {
                    console.log("createcommunityresponse",arguments);
                    if (!response.error && response.ID) {
                        $scope.community = null;
                        location.href = '#/CommunityDetail/' + response.ID;
                    } else {
                        bootbox.dialog("There was an error creating this community.");
                    }
                });
        };
        init();

    }]);
