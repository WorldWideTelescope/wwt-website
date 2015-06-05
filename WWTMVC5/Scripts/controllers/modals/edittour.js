wwtng.controller('EditTourModal', [
    '$scope',
    'dataproxy',
    '$timeout',
    '$routeParams',
    'UIHelper',
    'FileUploader',
    '$modal',
    function ($scope, dataproxy, $timeout, $routeParams, uiHelper, fileUploader,$modal) {
        $scope.isEdit = $scope.tour != null;
        $scope.touruploader = new fileUploader({
            url: "/Content/AddContent/1/true",
            autoUpload: true,
            onSuccessItem: successTour,
            onErrorItem: log,
            alias: 'contentFile'
        });
        $scope.tourthumbuploader = new fileUploader({
            url: "/Entity/AddThumbnail/Content",
            autoUpload: true,
            onSuccessItem: successThumb,
            onErrorItem: log,
            alias: 'thumbnail'
        });

        $scope.authorthumbuploader = new fileUploader({
            url: "/Content/Add/AssociatedContent",
            autoUpload: true,
            onSuccessItem: successAuthorThumb,
            onErrorItem: log,
            alias: 'associatedFile'
        });
        function log() {
            console.log({ uploader: arguments });
        }
        function successTour(xhr, response) {
            console.log(arguments, { tour: $scope.tour });
            var tour = response.contentData;
            $.each(tour, function (k, v) {
                $scope.tour[k] = v;
            });
            
            $scope.tour.FileName = $scope.tour.Name = tour.TourTitle;
            $scope.tour.Description = tour.TourDescription;
            $scope.extData = response.extendedData;
            $scope.extData.tags = response.extendedData.tags.split(';').join(',');
        };

        function successAuthorThumb(xhr, response) {
            if (!$scope.tour.PostedFileName) {
                $scope.tour.PostedFileName = [];
                $scope.tour.PostedFileDetail = [];
            }
            $scope.tour.PostedFileDetail = [response.fileDetailString];
            $scope.tour.PostedFileName = [response.fileName];
            $scope.AuthorThumbnailId = response.fileDetailString.split('~')[2];//".jpg~5226~bfa1553c-e7d3-4857-8a1e-56607bcc7543~image/jpeg~-1"
            $scope.editedAuthorThumb = true;
        };

        function successThumb(xhr, response) {
            $timeout(function () {
                $scope.tour.ThumbnailID = response.ThumbnailID;
                $scope.editedThumb = true;
                
            });
        }
        
       
        function init() {
            dataproxy.requireAuth().then(function (types) {
                $scope.types = types;
                    
                $('#tourFolder').multiselect({
                            includeSelectAllOption: false,
                            buttonContainer: '<div class="btn-group btn-group-sm"></div>'
                        });

                if (!$scope.tour) {
                    $scope.isEdit = false;
                    $timeout(function() {
                        $scope.tour = {
                            type: 'file',
                            CategoryID: 9,
                            ParentID: 596915,
                            AccessTypeID: 2,
                            IsOffensive: false,
                            IsLink: false
                        };
                        $scope.CategoryName = "General Interest";
                        
                    }, 100);
                } else {
                    $scope.isEdit = true;
                    dataproxy.getEditContent($scope.tour.Id)
                        .then(function (entity) {
                            var json = entity.Citation.split('json://')[1];
                            $scope.tour = entity;
                            $scope.extData = JSON.parse(json);
                            $scope.AuthorThumbnailId = $scope.tour.PostedFileDetail[0].split('~')[2];//".jpg~5226~bfa1553c-e7d3-4857-8a1e-56607bcc7543~image/jpeg~-1"
                            $.each($scope.extData.folders, function(i, folder) {
                                $.each($scope.community.communities, function(j, c) {
                                    if (c.Id === folder) {
                                        $('#tourFolder option[label="' + c.Name + '"').prop('selected', true);
                                    }
                                });
                            });
                            $('#tourFolder').multiselect('refresh');
                        });
                    
                    
                }


            }, function (reason) {
                //todo:handle reject case
            });
        }

        

        $scope.SaveTour = function () {
            pushExtendedProperties();
            $scope.tour.Name = $scope.tour.FileName;
            console.log($scope.tour);

            dataproxy.saveEditedContent($scope.tour)
                .then(function (response) {
                    console.log(this, arguments);
                    if (!response.error) {
                        //hide
                    }
                });
        };

        var pushExtendedProperties = function () {
            $scope.tour.DistributedBy = $scope.extData.author;
            $scope.tour.Tags = $scope.extData.tags;
            delete $scope.extData.searchCategories;
            
            var folders = [];
            $.each($scope.extData.folders, function (i, folder) {
                folders.push(folder);
            });
            $scope.extData.folders = folders;
            $scope.tour.Citation = "json://" + JSON.stringify($scope.extData);
        }

        $scope.AddTour = function () {
            pushExtendedProperties();
            console.log($scope.tour);
            dataproxy.publishContent($scope.tour)
                .then(function (response) {
                    console.log(arguments);
                    //hide
                });
        };
        init();

    }]);