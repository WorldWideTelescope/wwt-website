wwtng.controller('EditCommunity', [
    '$scope',
    'dataproxy',
    '$timeout',
    '$routeParams',
    'UIHelper',
    'FileUploader',
    function ($scope, dataproxy, $timeout, $routeParams, uiHelper, fileUploader) {
        $scope.isEdit = $routeParams && $routeParams.communityId && parseInt($routeParams.communityId) > 0;
        
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
            
            dataproxy.getAllTypes().then(function (types) {
                $scope.types = types;
                console.log(types);
                dataproxy.getUserCommunityList().then(function (data) {
                    
                    $.each(data, function (i, item) {
                        item.val = item.Value;
                    });
                    $scope.communities = data;
                    if (!data.length) {
                        location.href = '#/';
                        return;
                    }
                    if ($scope.isEdit) {
                        dataproxy.getEditCommunity($routeParams.communityId)
                            .then(function (community) {
                                $scope.content = community;
                                
                                setTimeout(function () { $('#lstCommunity').val(community.ParentID); }, 100);

                            });
                    } else {

                        $timeout(function () {
                            $scope.community = {
                                CategoryID: 9,
                                ParentID: '?',
                                AccessTypeID: 2,
                                IsOffensive: false,
                                IsLink: false,
                                CommunityType:'Community'
                            };
                            $scope.CategoryName = "General Interest";
                            $timeout(function () {
                                var opt = $('#lstCommunity option[label="None"]').first();
                                opt.prop('selected', true);
                                $('#lstCommunity').trigger('change');
                            }, 500);

                        }, 100);
                    }

                });

            });

        }

        $scope.saveEditedCommunity = function () {
            $scope.community.ParentID = $scope.community.ParentID.val;
            console.log($scope.community);

            dataproxy.saveEditedCommunity($scope.community)
                .then(function (response) {
                    console.log(this, arguments);
                    if (!response.error) {
                        location.href = '#/CommunityDetail/' + response.ID;
                    }
                });
        };

        $scope.createCommunity = function () {
            $scope.community.ParentID = $scope.community.ParentID.val;
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