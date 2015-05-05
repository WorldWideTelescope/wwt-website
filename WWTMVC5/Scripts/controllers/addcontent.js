wwtng.controller('AddContent', [
    '$scope',
    'dataproxy',
    '$timeout',
    '$routeParams',
    'UIHelper',
    'FileUploader',
    '$http',
    function ($scope, dataproxy, $timeout, $routeParams, uiHelper, fileUploader, $http) {
        $scope.isEdit = $routeParams && $routeParams.contentId && parseInt($routeParams.contentId) > 0;
    $scope.contentuploader = new fileUploader({
        url: "/Content/AddContent/1",
        autoUpload: true,
        onSuccessItem: successContent,
        onErrorItem: log,
        alias: 'contentFile'
    });
    $scope.thumbuploader = new fileUploader({
        url: "/Entity/AddThumbnail/Content",
        autoUpload: true,
        onSuccessItem: successThumb,
        onErrorItem: log,
        alias: 'thumbnail'
    });
    function log() {
        console.log({ uploader: arguments });
    }
    function successContent(xhr,response) {
        console.log(arguments, {content:$scope.content});
        $.each(response, function (k, v) {
            $scope.content[k] = v;
            
        });
        if (response.TourTitle) {
            $scope.content.FileName = response.TourTitle;
            $scope.content.Description = response.TourDescription;
        } else {
            var split = response.ContentFileName.split('.');
            split.pop();
            $scope.content.FileName = split.join('.');
        }
        $scope.content.Name = $scope.content.FileName;
        //$('#lstCommunity').val($scope.content.ParentID);//wth??
    };

    function successThumb(xhr, response) {
        $timeout(function () {
            $scope.content.ThumbnailID = response.ThumbnailID;
            //$('#lstCommunity').val($scope.content.ParentID);
        });
    }
    dataproxy.getAllTypes().then(function (types) {
        $scope.types = types;
        console.log(types);
        dataproxy.getUserCommunityList().then(function (data) {
            //$scope.content.ParentID = $('#lstCommunity option[label=None]').val();
            $scope.communities = data;
            if (!data.length) {
                location.href = '#/';
                return;
            }
            if ($scope.isEdit) {
                dataproxy.getEditContent($routeParams.contentId)
                   .then(function (content) {
                       $scope.content = content;

                    //content.ParentID = content.ParentID.toString();
                    //$('#lstCommunity').val(content.ParentID);
                });
            } else {
                $timeout(function () {
                    $scope.content = {
                        type: 'file',
                        CategoryID: 9,
                        ParentID: '?',
                        AccessTypeID: 2,
                        IsOffensive:false
                    };
                    $scope.CategoryName = "General Interest";
                    $timeout(function() {
                        //$('#lstCategory').val('9');
                        $scope.content.ParentID = $('#lstCommunity option[label="None"]').first().attr('Value');
                        $('#lstCommunity').val($scope.content.ParentID);
                    }, 500);

                },100);
            }
            
        });
        
    });


        $scope.saveEditedContent = function() {
            console.log($scope.content);
            dataproxy.saveEditedContent($scope.content)
                .then(function (response) {
                    console.log(this, arguments);
                if (!response.error) {
                    location.href = '#/ContentDetail/' + response.ID;
                }
            });
        };

    $scope.publishContent = function() {
        console.log($scope.content);
        dataproxy.publishContent($scope.content)
            .then(function (response) {
                console.log(arguments);
                location.href = '#/ContentDetail/' + response.ID;
        });
    };
    
    uiHelper.fixLinks('profileLink');
}]);