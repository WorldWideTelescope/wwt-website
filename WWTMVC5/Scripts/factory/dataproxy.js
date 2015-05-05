wwtng.factory('dataproxy', [
    '$http', '$q','$timeout', function($http, $q,$timeout) {
        var api = {
            getEntities: getEntities,
            getEntityDetail: getEntityDetail,
            getEditContent: getEditContent,
            getAllTypes: getAllTypes,
            search: searchQuery,
            getMyProfile: getMyProfile,
            saveProfile: saveProfile,
            publishContent: publishContent,
            saveEditedContent: saveEditedContent,
            getUserEntities: getUserEntities,
            getUserRequests:getUserRequests,
            getUserCommunityList: getUserCommunityList,
            deleteContent:deleteContent,
            isAdmin: getIsAdmin,
            currentUserId: getCurrentUserId
        };

        //Enum object with friendly names and values
        // from getAllTypes - includes userid and isAdmin
        var types,
            currentUserId,
            isAdmin;

        //EntityController.cs
        //[Route("Entity/RenderJson/{highlightType}/{entityType}/{categoryType}/{contentType}/{page}/{pageSize}")]
        function getEntities(args) {
            var deferred = $q.defer();
            var url = '/Entity/RenderJson/' +
                args.highlightType + '/' +
                args.entityType + '/' +
                args.categoryType + '/' +
                args.contentType + '/' +
                args.currentPage + '/' +
                args.pageSize;
            $http.post(url, {}).
                success(function (data, status, headers, config) {
                    data.entities = dataHelper(data.entities);

                    deferred.resolve(data);
                }).
                error(function(data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });
            return deferred.promise;
        }

        //ContentController.cs: [Route("Content/RenderDetailJson/{Id}")]
        function getEntityDetail(args) {
            var deferred = $q.defer();
            getAllTypes().then(function() {
                $http.post('/Content/RenderDetailJson/' + args.entityId, {}).
                    success(function(data, status, headers, config) {
                        console.log(data);
                        data = dataHelper([data])[0];
                        deferred.resolve(data);
                    }).
                    error(function(data, status, headers, config) {
                        console.log({ data: data, status: status, headers: headers, config: config });
                        deferred.resolve({ error: true, status: 'error' });
                    });
            });
            
            return deferred.promise;
        }
        //ContentController.cs: [Route("Content/Edit/{id}")]
        function getEditContent(id) {
            var deferred = $q.defer();
            getAllTypes().then(function() {
                $http.post('/Content/Edit/' + id, {}).
                    success(function(data, status, headers, config) {
                        console.log(data);
                        //data = dataHelper([data])[0];
                        deferred.resolve(data);
                    }).
                    error(function(data, status, headers, config) {
                        console.log({ data: data, status: status, headers: headers, config: config });
                        deferred.resolve({ error: true, status: 'error' });
                    });
            });
            
            return deferred.promise;
        }
        //SearchController.cs: [Route("Search/RenderJson/{query}/{category}/{contentType}/{currentPage}/{pageSize}")]
        function searchQuery(args) {
            var deferred = $q.defer();
            var url = '/Search/RenderJson/' +
                args.query + '/' +
                args.categories + '/' +
                args.contentTypes + '/' +
                args.currentPage + '/' +
                args.pageSize;
            $http.post(url, {}).
                success(function (data, status, headers, config) {
                    console.log(data);
                    data.Data.searchResults = dataHelper(data.Data.searchResults);
                    deferred.resolve(data.Data);
                }).
                error(function(data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });
            return deferred.promise;
        }

        

        //EntityController.cs [Route("Entity/Types/GetAll")]
        function getAllTypes() {
            var deferred = $q.defer();
            if (types) {
                $timeout(function() {
                    deferred.resolve(types);
                });
            } else {
                var url = '/Entity/Types/GetAll';
                $http.post(url, {}).
                    success(function(data, status, headers, config) {
                        data.highlightValues = convertCamel(data.highlightValues);
                        data.categoryValues = convertCamel(data.categoryValues);
                        data.contentValues = getFriendlyTypes(data.contentValues);
                        currentUserId = data.currentUserId;
                        isAdmin = data.isAdmin;
                        types = data;
                        deferred.resolve(types);
                    }).
                    error(function(data, status, headers, config) {
                        console.log({ data: data, status: status, headers: headers, config: config });
                        deferred.resolve({ error: true, status: 'error' });
                    });
            }
            return deferred.promise;
        }
        //ProfileController.cs [Route("/Profile/MyProfile/Get")]
        function getMyProfile(id) {
            var deferred = $q.defer();

            $http.post('/Profile/MyProfile/Get', {}).
                success(function (data, status, headers, config) {
                    console.log(data);
                    if (typeof data == 'string' && data.indexOf('error') === 0) {
                        console.log('retrying profile get- server thinks user not logged in');
                        $timeout(function() {
                        $http.post('/Profile/MyProfile/Get', {}).
                            success(function (data, status, headers, config) {
                                console.log(data);
                                deferred.resolve(data);
                            }).
                            error(function (data, status, headers, config) {
                                console.log({ data: data, status: status, headers: headers, config: config });
                                deferred.resolve(data);
                            });
                        }, 500);
                    } else {
                        deferred.resolve(data);
                    }
                }).
                error(function(data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve(data);
                });
            
            
            return deferred.promise;
        }

        function saveProfile(profile) {
            //string affiliation, string aboutMe, bool isSubscribed, Guid? profileImageId, string profileName
            var deferred = $q.defer();

            $http.post('/Profile/Save/' + profile.profileId, profile).
                    success(function (data, status, headers, config) {
                        console.log(data);
                        deferred.resolve(data);
                    }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });

            return deferred.promise;
        }
        //ContentController.cs[Route("Content/New/{id}")]
        function publishContent(content) {
            var deferred = $q.defer();

            $http.post('/Content/Create/New', { contentInputViewModel: content }).
                    success(function (data, status, headers, config) {
                        console.log(data);
                        deferred.resolve(data);
                    }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });

            return deferred.promise;
        }
        //ContentController.cs [Route("Content/Save/Edits")]
        function saveEditedContent(content) {
            var deferred = $q.defer();
            $http.post('/Content/Save/Edits', { contentInputViewModel: JSON.stringify(content) }).
                success(function (data, status, headers, config) {
                    console.log(data);
                    deferred.resolve(data);
                }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });

            return deferred.promise;
        }
        //ProfileController.cs:[Route("Profile/Entities/{entityType}")]
        function getUserEntities(type, profileId) {
            var deferred = $q.defer();

            $http.post('/Profile/Entities/' + type + '/'+ 1 + '/' + 999, {profileId:profileId  }).
                    success(function (data, status, headers, config) {
                        data.entities = dataHelper(data.entities);
                        deferred.resolve(data);
                    }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });

            return deferred.promise;
        }
        //CommunityController.cs:[Route("Community/Permission/{permissionsTab}/{currentPage}")]
        function getUserRequests() {
            var deferred = $q.defer();

            $http.post('/Community/Permission/ProfileRequests/1', {}).
                    success(function (data, status, headers, config) {
                    console.log(data);
                        deferred.resolve(data);
                    }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });

            return deferred.promise;
        }
        //[Route("Content/User/CommunityList")]
        function getUserCommunityList() {
            var deferred = $q.defer();

            $http.post('/Content/User/CommunityList', {}).
                    success(function (data, status, headers, config) {
                        console.log(data);
                        deferred.resolve(data);
                    }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });

            return deferred.promise;
        }
        //[Route("Content/User/CommunityList")]
        function getUserCommunities() {
            var deferred = $q.defer();

            $http.post('/Content/User/CommunityList', {}).
                    success(function (data, status, headers, config) {
                        console.log(data);
                        deferred.resolve(data);
                    }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });

            return deferred.promise;
        }
        //ContentController.cs[Route("Content/Delete/{id}")]
        function deleteContent(contentId) {
            var deferred = $q.defer();

            $http.post('/Content/Delete/' + contentId, { }).
                    success(function (data, status, headers, config) {
                        console.log(data);
                        deferred.resolve(data);
                    }).
                error(function (data, status, headers, config) {
                    console.log({ data: data, status: status, headers: headers, config: config });
                    deferred.resolve({ error: true, status: 'error' });
                });

            return deferred.promise;
        }

        function getCurrentUserId() {
            var deferred = $q.defer();
            getAllTypes().then(function () {
                deferred.resolve(currentUserId);
            });
            return deferred.promise;
        }
        function getIsAdmin() {
            var deferred = $q.defer();
            getAllTypes().then(function () {
                deferred.resolve(isAdmin);
            });
            return deferred.promise;
        }

        //#region helpers
        var hasEmptyOrNullGuid = function (guid) {
            return !guid || guid.replace(/0/g, '') === '----';
        };

        function dataHelper(collection) {
            $.each(collection, function (i, item) {
                // add webclient url to tours and collections
                if (item.ContentType === 1) {
                    item.webclientUrl = 'http://' + location.host + '/webclient?tourUrl=' +
                        encodeURIComponent('http://' + location.host + '/file/download/' + item.ContentAzureID + '/' + item.FileName);
                } 
                else if (item.ContentType === 0) {//TODO: Find out why type can be "all"
                    item.ContentType = 7;
                }
                if (hasEmptyOrNullGuid(item.ThumbnailID)) {
                    item.ThumbnailUrl = '/Content/Images/default' + types.contentValues[item.ContentType].val + 'thumbnail.png';
                } else {
                    item.ThumbnailUrl = '/file/thumbnail/' + item.ThumbnailID;
                }

                if ($.trim(item.FileName).indexOf('http') === 0) {
                    item.LinkUrl = item.FileName;
                } else if (item.FileName) {
                    var fileSplit = item.FileName.split('.');
                    if (fileSplit.length === 1) {
                        console.log(fileSplit);
                    }
                    var ext = fileSplit.length > 1 ? fileSplit[fileSplit.length - 1] : null;
                    var file;
                    if (fileSplit.length > 2) {
                        fileSplit.pop();
                        file = fileSplit.join('_');
                    } else {
                        file = fileSplit[0];
                    }
                    item.DownloadUrl = '/file/Download/' + item.ContentAzureID + '/' + file;
                    if (ext) {
                        item.DownloadUrl += '/' + ext;
                    }
                } else {
                    item.DownloadUrl = '/file/Download/' + item.ContentAzureID + '/File.txt';
                }
                if (item.ContentType === 2) {
                    item.webclientUrl = 'http://' + location.host + '/webclient?wtml=' +
                        encodeURIComponent('http://' + location.host + item.DownloadUrl);
                }
                item.ContentTypeName = types.contentValues[item.ContentType].val;
            });
            return collection;
        }

        var getFriendlyTypes = function(array) {
            var friendly = [];
            var pushFriendly = function(v, f) {
                friendly.push({ val: v, name: f || v });
            }
            $.each(array, function(i, s) {
                switch (s) {
                    case 'All':
                        friendly.splice(0,0, {val:s,name:s});
                        break;
                    case 'Tours':
                        pushFriendly(s, 'Tour');
                        break;
                    case 'Wtml':
                        pushFriendly(s, 'Collection');
                        break;
                    case 'Excel':
                        pushFriendly(s, 'Spreadsheet');
                        break;
                    case 'Doc':
                        pushFriendly(s, 'Word Doc');
                        break;
                    case 'Ppt':
                        pushFriendly(s, 'PowerPoint');
                        break;
                    case 'Link':
                        pushFriendly(s);
                        break;
                    case 'Generic':
                        pushFriendly(s);
                        break;
                    case 'Wwtl':
                        pushFriendly(s, '3d Model');
                        break;
                    case 'Video':
                        pushFriendly(s);
                        break;

                }
            });
            return friendly;
        };
       
        var convertCamel = function(array) {
            var converted = [];
            $.each(array, function(i, s) {
                converted.push({
                    val: s,
                    name: $.trim(s.replace(/([A-Z]+)/g, "$1").replace(/([A-Z][a-z])/g, " $1")),
                    index:i
                });
            });
            return converted;
        }
        //#endregion

        return api;
    }
]);