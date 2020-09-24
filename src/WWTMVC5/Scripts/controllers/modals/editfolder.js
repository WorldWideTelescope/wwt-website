wwtng.controller('EditFolderModal', [
    '$scope',
    'dataproxy',
    '$timeout',
    '$routeParams',
    'UIHelper',
    'FileUploader',
    '$http',
    '$q',
    function ($scope, dataproxy, $timeout, $routeParams, uiHelper, fileUploader, $http, $q) {

        $scope.editMode = $scope.folder != undefined && $scope.folder != null;

        function init() {
            if (!$scope.folder) {
                $scope.folder =  {
                    CategoryID: 9,
                    ParentID: $scope.communityId,
                    AccessTypeID: 1,
                    IsOffensive: false,
                    IsLink: false,
                    CommunityType: 'Community',
                    ThumbnailID: '136bdb25-92c6-4177-b088-6f77bdf9a5e4',
                    Description: '',
                    Name:''
                };
            }
        }

        $scope.createFolder = function () { }
        $scope.saveFolder = function () { }
        var isDeleteAction = false;
        $scope.removeTour = function (tour) {
            isDeleteAction = true;
            //bootbox.confirm("Remove this tour from " + $scope.folder.Name + "?", function (result) {
            //    if (result) {
                    var id = tour.Id.toString();
                    var tourIds = $scope.folder.Description.split(',');
                    tourIds.splice(tourIds.indexOf(id), 1);
                    $scope.folder.Description = tourIds.join(',');
                    saveCommunity();
            //    }
            //});
            setTimeout(function () { isDeleteAction = false; }, 100);
        }
        $scope.addTour = function (tour, index) {
            
            var id = tour.Id.toString();
            if ($scope.folder.Description && $scope.folder.Description.length) {
                $scope.folder.Description += ',';
            } else {
                $scope.folder.Description = '';
            }
            $scope.folder.Description += id;
            saveCommunity();
               
        }


        $scope.deleteCommunity = function() {
            bootbox.confirm("Delete this folder?", function (result) {

                if (result) {
                    $scope.savingWhat = 'Removing folder';
                    $scope.savingChanges = true;
                    dataproxy.deleteCommunity($scope.folder.Id, $scope.communityId).then(function () {
                        $scope.savingWhat = 'Generating new master tour file';
                        $http.get('/Community/Rebuild/Tours').success(function (result) {

                            console.log('new tour file at ' + result);
                            $scope.savingChanges = false;
                            $scope.refreshCommunityDetail();
                            $scope.$hide();

                        });
                    });

                }
            });
        }

        var saveCommunity = $scope.saveCommunity = function (hide) {
            $scope.savingWhat = 'Updating folder';
            $scope.savingChanges = true;
            if (!$scope.editMode) {
                $scope.savingWhat = 'Creating folder';
                dataproxy.createCommunity($scope.folder).then(function() {
                    $scope.savingChanges = false;
                    if (hide) $scope.$hide();
                    $scope.refreshCommunityDetail();
                });
                return;
            }
            $scope.folder.tours = $scope.getFolderTours($scope.folder);
            $scope.folder.notIncluded = $scope.getToursNotInFolder($scope.folder);
            dataproxy.getCommunityDetail($scope.folder.Id, true).then(function(response) {
                var community = response.community;
                try {
                    delete community.ParentList;
                    delete community.CategoryList;
                } catch (er) {
                    console.log('error in community:', community);
                }
                function eliminateDuplicates(arr) {
                    var result = [],
                    unique = {};

                    for (var i = 0; i < arr.length; i++) {
                        unique[arr[i]] = 0;
                    }
                    for (value in unique) {
                        if (unique.hasOwnProperty(value)) {
                            result.push(value);
                        }
                    }
                    return result;
                }

                community.Name = $scope.folder.Name;
                community.Description = eliminateDuplicates($scope.folder.Description.split(',')).join(',');
                dataproxy.saveEditedCommunity(community).then(function (result) {
                    $scope.savingChanges = false;
                    if (hide) $scope.$hide();
                });
            });
        }

        $scope.saveAll = function () {
            $scope.savingChanges = true;
            $scope.savingWhat = 'Generating new master tour file';
            $http.get('/Community/Rebuild/Tours').success(function (result) {
                console.log('new tour file at ' + result);
                $scope.savingChanges = false;
                $scope.$hide();
            });
        }

        $scope.editTour = function (tour) {
            if (!isDeleteAction) {
                $scope.$hide();
                $scope.showTourModal(tour);
            }
        }
        init();
    }]);