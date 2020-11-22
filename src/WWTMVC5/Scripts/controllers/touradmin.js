wwtng.controller('TourAdmin', [
    '$scope', 'dataproxy', '$timeout', '$routeParams', '$http', 'UIHelper','$modal','$q',
    function ($scope, dataproxy, $timeout, $routeParams, $http, uiHelper, $modal,$q) {
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
                updateCommunityContents();
            });
        }

        function updateCommunityContents() {
            var d1 = new Date();
            dataproxy.getCommunityContents(communityId).then(function (response) {
                $scope.community.contents = response.entities;
                $scope.community.communities = response.childCommunities;
                wwt.triggerResize();
                console.log('update details took ' + (new Date().valueOf() - d1.valueOf()));
            });
        }

        $scope.refreshCommunityDetail = updateCommunityContents;

        $scope.options = { activeTab: 'communities' }

        $scope.tabChange = function (tab) {
            $scope.options.activeTab = tab;
            wwt.triggerResize();
        }

        $scope.getAzureGuidFromTourGuid = function (guid) {
            var tour = $scope.getTourFromGuid(guid);
            return tour ? tour.ContentAzureID : null;
        }

        $scope.getTourFromGuid = function (guid) {

            var tourResult = null;
            $.each($scope.community.contents, function (i, tour) {
                if (tour.extData.tourGuid.toLowerCase() === guid.toLowerCase()) {
                    tourResult = tour;
                }
            });
            return tourResult;
        }
        $scope.getTourById = function (id) {
            var tourResult = null;
            $.each($scope.community.contents, function (i, tour) {
                if (tour.Id == id) {
                    tourResult = tour;
                }
            });
            return tourResult;
        }
        $scope.getFolderTours = function (folder) {
            var collection = [];
            var tours = folder.Description ? folder.Description.split(',') : [];
            $.each(tours, function (i, tourId) {
                if (tourId && tourId.length > 3) {
                    var tour = $scope.getTourById(tourId);
                    if (tour) collection.push(tour);
                }
            });
            return collection;
        }
        $scope.getToursNotInFolder = function(folder) {
             var collection = [];
             var tourIds = folder.Description ? folder.Description.split(',') : [];
             $.each($scope.community.contents, function (i, t) {
                 var tourId = t.Id.toString();
                 if ($.inArray(tourId,tourIds) === -1) {
                     var tour = $scope.getTourById(tourId);
                     if (tour) collection.push(tour);
                 }
             });
             return collection;
         }

        $scope.getFolderId = function (node) {
            var parent = node.parent();
            return $scope.getCommunityByName(parent.attr('Name')).Id;
        }

        $scope.getCommunityByName = function (name) {
            var community = null;
            name = name.toLowerCase();
            $.each($scope.community.communities, function (i, com) {
                if ($.trim(com.Name).toLowerCase() == name) {
                    community = com;
                }
            });
            return community;
        }

        var newFolderModal = $modal({
            scope: $scope,
            contentTemplate: '/content/views/modals/editfolder.html',
            show: false,
            title: 'Add New Folder'
        });
        var editFolderModal = $modal({
            scope: $scope,
            contentTemplate: '/content/views/modals/editfolder.html',
            show: false,
            title: 'Edit Folder'
        });
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

        $scope.openFolder = function(community) {
            $scope.folder = community;
            
            if (community) {
                $scope.folder.tours = $scope.getFolderTours(community);
                $scope.folder.notIncluded = $scope.getToursNotInFolder($scope.folder);
                editFolderModal.$promise.then(editFolderModal.show);
            } else {
                newFolderModal.$promise.then(newFolderModal.show);
            }
        };

        $scope.buildXml = function () {
            var s = '<?xml version="1.0" encoding="UTF-8"?>\n<Folder>';
            var attr = function (attrName, value) {
                if (!value) {
                    value = '';
                }
                s += ' ' + attrName + '="' + value + '"';
            }
            $.each($scope.community.communities, function(i, c) {
                s += '\n  <Folder Name="' + c.Name + '" Group="Tour" Thumbnail="">';
                var tours = $scope.getFolderTours(c);
                $.each(tours, function(j,t) {
                    s += '\n      <Tour';
                    attr('Title', t.Name);
                    attr('ID', t.ContentAzureID);
                    attr('Description', t.Description.replace(/\\n/g,'\\n'));
                    attr('Classification', t.extData.classification);
                    attr('AuthorEmail', t.extData.authorEmail);
                    attr('Author', t.extData.author);
                    attr('AuthorUrl', '');
                    attr('AverageRating', t.Rating);
                    attr('LengthInSecs', t.TourLength);
                    attr('OrganizationUrl', t.extData.organizationUrl);
                    attr('OrganizationName', t.extData.organization);
                    attr('ITHList', t.extData.ithList);
                    attr('AstroObjectsList', '');
                    attr('Keywords', t.Tags.split(',').join(';').split(' ').join(''));
                    var related = t.extData.related;
                    if (typeof related != 'string') {
                        related = related.join(';');
                    }
                    attr('RelatedTours', related);
                    s += '/>';
                });
                s += '\n  </Folder>';
            });
            s += '\n</Folder>';
            console.log(s);
            $timeout(function() { $scope.tourXml = s; });
        }

        init();
    }]);
