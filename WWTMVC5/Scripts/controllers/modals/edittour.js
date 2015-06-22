wwtng.controller('EditTourModal', [
    '$scope',
    'dataproxy',
    '$timeout',
    '$routeParams',
    'UIHelper',
    'FileUploader',
    '$http',
    '$q',
    function ($scope, dataproxy, $timeout, $routeParams, uiHelper, fileUploader, $http, $q) {
        var tourId = null;
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
            
            $.each(response.contentData, function (k, v) {
                $scope.tour[k] = v;
            });
            //$scope.tour.ContentData = response.contentData;
            $scope.tour.FileName = response.contentData.ContentFileName;
            $scope.tour.ContentTypeID = 1;
            $scope.tour.Name = response.contentData.TourTitle;
            $scope.tour.TourLength = response.contentData.TourLength;
            $scope.tour.Description = response.contentData.TourDescription;
            $scope.extData = response.extendedData;
            $scope.extData.tags = $scope.tour.Tags = response.extendedData.tags.split(';').join(',');
            findFolders();
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

        var communityTours = {};
        var addTourToCommunity = function(tourId, folderId) {
            folderId = 'f'+folderId.toString();
            if (communityTours[folderId]) {
                communityTours[folderId].push(tourId);
            } else {
                communityTours[folderId] = [tourId];
            }
        }
        var saveCommunities = function () {
            
            $.each(communityTours, function(k, v) {
                var communityId = parseInt(k.replace('f', ''));
                dataproxy.getCommunityDetail(communityId, true).then(function (response) {
                    var community = response.community;
                    try {
                        delete community.ParentList;
                        delete community.CategoryList;
                    } catch (er) {
                        console.log('error in community:', community);
                    }
                    community.Description = v.join(',');
                    dataproxy.saveEditedCommunity(community).then(function(response) {
                        console.log('saved community', communityId, v, response);
                    });
                });
            });

        }
        var curTourIndex = 0;
        var changeGuids = function () {
            var curTour = $scope.community.contents[curTourIndex];
            dataproxy.getEditContent(curTour.Id).then(function (tour) {
                var azureIds = [];
                try {
                    $.each(tour.extData.related, function(i, rel) {
                        var azureGuid = $scope.getAzureGuidFromTourGuid(rel);
                        if (azureGuid) {
                            azureIds.push(azureGuid);
                        }
                    });
                } catch (er) {
                }
                tour.extData.related = azureIds;
                tour.Citation = "json://" + JSON.stringify(tour.extData);
                tour.extData = null;
                dataproxy.saveEditedContent(tour)
                    .then(function (response) {
                        console.log('fixed tour props', response);
                        curTourIndex++;
                        if (curTourIndex < $scope.community.contents.length) {
                            changeGuids();
                        } else {
                            //$scope.buildXml();
                        }
                    });
            });
        };

        

        function init() {
            $scope.isEdit = $scope.tour != null;
            $scope.isInXml = true;

            $q.all([
                dataproxy.requireAuth(),
                $http({ method: 'GET', url: '/community/fetch/tours' })
            ]).then(function (results) {
                $scope.types = results[0];
                $scope.tourXml = $(results[1].data);
                
                $('#tourFolder').multiselect({
                    includeSelectAllOption: false,
                    buttonWidth: '100%',
                    maxHeight: 300,
                    enableCaseInsensitiveFiltering: true,
                    filterPlaceholder: 'Filter folders...',
                    buttonContainer: '<div class="btn-group btn-group-sm"></div>',
                    templates: {
                        filter: '<li class="multiselect-item filter"><div class="form-group form-group-sm"><input class="form-control multiselect-search" type="text"></div></li>',
                        filterClearBtn: ''
                    }
                });
                $('#relatedTours').multiselect({
                    includeSelectAllOption: false,
                    buttonWidth: '100%',
                    maxHeight: 300,
                    enableCaseInsensitiveFiltering: true,
                    filterPlaceholder: 'Filter tours...',
                    buttonContainer: '<div class="btn-group btn-group-sm"></div>',
                    templates: {
                        filter: '<li class="multiselect-item filter"><div class="form-group form-group-sm"><input class="form-control multiselect-search" type="text"></div></li>',
                        filterClearBtn: ''
                    }
                });
                
                if (!$scope.tour) {
                    $scope.isEdit = false;
                    $scope.tour = {
                        CategoryID: 9,
                        ParentID: 596915,
                        AccessTypeID: 2,
                        IsOffensive: false,
                        IsLink: false
                    };
                    $scope.tourLoaded = true;
                    
                } else {
                    $scope.isEdit = true;
                    tourId = $scope.tour.Id;
                    dataproxy.getEditContent(tourId).then(function (tour) {
                        if (tour.ContentFileDetail.indexOf('~')===0) {
                            tour.ContentFileDetail = ".wtt~" + $scope.tour.Size + "~" + tour.ContentDataID + "~application/x-wtt~" + tour.ID;
                            
                            if (tour.FileName.indexOf('.wtt') === -1) {
                                tour.FileName += '.wtt';
                            }
                            tour.Citation = "json://" + JSON.stringify(tour.extData);
                            dataproxy.saveEditedContent(tour)
                                .then(function(response) {
                                    console.log('fixed tour',response);
                                });
                        }
                            
                        $scope.tour = tour;
                        $scope.extData = tour.extData;
                        $scope.tourLoaded = true;

                        try {
                            $scope.AuthorThumbnailId = $scope.tour.PostedFileDetail[0].split('~')[2]; //".jpg~5226~bfa1553c-e7d3-4857-8a1e-56607bcc7543~image/jpeg~-1"
                        } catch (er) {
                            dataproxy.deleteContent(id).then($scope.$hide);
                            $scope.refreshCommunityDetail();
                        }

                        setTimeout(findFolders,10);
                            
                    });
                }

            }, function (reason) {
                //todo:handle reject case
                console.log('rejected', reason);
            });
        }

        var findFolders = function() {
            var ex = $scope.extData;

            var guid = ($scope.tour.ContentDataID).toLowerCase();
            var tours = $scope.tourXml.find('Tour[ID="' + guid + '"]');

            if (!tours.length) {
                $scope.isInXml = false;
                $scope.missingGuid = guid;
                tours = $scope.tourXml.find('Tour[Title="' + $scope.tour.Name + '"]');
            }
            
            
            var folders = ex.folders;
            if (folders && folders.length) {
                $.each(folders, function(i, fld) {
                    var opt = $('#tourFolder option[value="' + fld + '"');
                    opt.prop('selected', true);
                });
            } else {
                tours.each(function(i, tour) {
                    var folderName = $(tour).parent().attr('Name');
                    var opt = $('#tourFolder option[label="' + folderName + '"');
                    opt.prop('selected', true);
                    if (!parseInt(opt.val()))return;

                });
            }
            var related = ex.related && ex.related.length ?
                ex.related :
                tours.first().attr('RelatedTours') ?
                tours.first().attr('RelatedTours').split(';') : [];
            
            $.each(related, function(i, tguid) {
                tguid = tguid.toLowerCase();
                $('#relatedTours option[value="' + tguid + '"').prop('selected', true);
                       
            });
            $timeout(function () {
                $('#relatedTours').multiselect('refresh');
                $('#tourFolder').multiselect('refresh');
            });
        }

        var saveCommunityTours = function (hide, tour) {
            var communityPromises = [];
            $.each($scope.extData.folders, function (i, folder) {
                communityPromises.push(dataproxy.getCommunityDetail(folder, true));
            });
            var savePromises = [];
            $q.all(communityPromises).then(function (results) {
                $.each(results, function (i, response) {
                    var community = response.community;
                    try {
                        delete community.ParentList;
                        delete community.CategoryList;
                    } catch (er) {
                        console.log('error in community:', community);
                    }
                    //if (community.Description && community.Description.length) {
                    //    community.Description += ',';
                    //} else {
                    //    community.Description = '';
                    //}

                    var goodArray = [];
                    var tourArray = community.Description.split(',');
                    $.each(tourArray, function (i, id) {
                        id = id + ''; //ensure string;
                        if (!isNaN(parseInt(id)) && id.length > 3 && goodArray.indexOf(id) == -1 && id != tourId) {
                            goodArray.push(id);
                        }
                    });
                    goodArray.push(tourId);
                    function eliminateDuplicates(arr) {
                        var i,
                            len = arr.length,
                            out = [],
                            obj = {};

                        for (i = 0; i < len; i++) {
                            obj[arr[i]] = 0;
                        }
                        for (i in obj) {
                            out.push(i);
                        }
                        return out;
                    }
                    community.Description = eliminateDuplicates(goodArray).join(',');
                    
                    savePromises.push(dataproxy.saveEditedCommunity(community));

                });
                $q.all(savePromises).then(function (results) {
                    $.each(results, function (i, response) {
                        console.log('saved tour references for folder', response);
                    });
                    $scope.savingWhat = 'Rebuilding master tour file';
                    var rebuildUrl = '/Community/Rebuild/Tours';
                    
                    $http.get(rebuildUrl).success(function(result) {
                        console.log('new tour file at ' + result);
                    });
                    if ($scope.webclientChange) {
                        $scope.savingWhat = 'Web client state changed - rebuilding web client master tour file';
                        $http.get(rebuildUrl + '/true').success(function (result) {
                            if (hide) {
                                $scope.savingChanges = false;
                                $scope.$hide();
                            }
                            console.log('new webclient tour file at ' + result);
                        });
                    }
                    else if (hide) {
                        $scope.savingChanges = false;
                        $scope.$hide();
                    }
                });
            });

            
        }

        $scope.SaveTour = function () {
            $scope.savingWhat = 'Saving tour and folder references';
            $scope.savingChanges = true;
            pushExtendedProperties();
            console.log('save edits', $scope.tour);
            dataproxy.saveEditedContent($scope.tour).then(function (response) {
                console.log('saved edits', response);
                if (!response.error) {
                        
                    setTimeout(function () {
                        saveCommunityTours(true);//!$scope.ratingtotal);
                    }, 1000);
                }
            });
        };

        $scope.AddTour = function () {
            $scope.savingWhat = 'Saving tour and folder references';
            $scope.savingChanges = true;
            pushExtendedProperties();
            console.log('save new tour',$scope.tour);
            dataproxy.publishContent($scope.tour).then(function (response) {
                console.log('saved new tour response', response);
                if (!response.error) {
                    tourId = response.ID;
                    
                    setTimeout(function () {
                        saveCommunityTours(true); //!$scope.ratingtotal);
                    }, 1000);
                }
                //hide
            });
        };

        $scope.confirmDelete = function () {
            $scope.savingWhat = 'Removing tour and folder references';
            $scope.savingChanges = true;
            bootbox.confirm("Delete this Tour?", function (result) {
                if (result) {
                    dataproxy.deleteContent($scope.tour.ID).then($scope.$hide);
                    $scope.refreshCommunityDetail();
                }
            });
        };

        var pushExtendedProperties = function () {
            var ex = $scope.extData;
            //delete $scope.extData.searchCategories;
            
            var folders = [];
            var related = [];
            $('#relatedTours option:selected').each(function(i, opt) {
                related.push($(opt).val());
            });
            $('#tourFolder option:selected').each(function (i, opt) {
                folders.push($(opt).val());
            });
            ex.folders = folders;
            ex.related = related;

            
            $scope.tour.Citation = "json://" + JSON.stringify(ex);
        }

        

        //$.each(ratingData, function() {
            
        //    console.log(calcRatingCounts(this.count, this.total), this.avg);
        //});
        init();
    }]);