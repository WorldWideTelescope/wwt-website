wwtng.factory('dataproxy', [
    '$http', '$q', '$timeout', 'UIHelper', '$rootScope', function($http, $q, $timeout, uiHelper, $rootScope) {
        var api = {
            getEntities: getEntities,
            getEntityDetail: getEntityDetail,
            incrementDownloadCount:incrementDownloadCount,
            getEditContent: getEditContent,
            getAllTypes: getAllTypes,
            search: searchQuery,
            getMyProfile: getMyProfile,
            saveProfile: saveProfile,
            publishContent: publishContent,
            saveEditedContent: saveEditedContent,
            getUserEntities: getUserEntities,
            getUserRequests: getUserRequests,
            getUserCommunityList: getUserCommunityList,
            deleteContent: deleteContent,
            isAdmin: getIsAdmin,
            currentUserId: getCurrentUserId,
            createCommunity: createCommunity,
            getCommunityDetail: getCommunityDetail,
            getCommunityContents: getCommunityContents,
            saveEditedCommunity: saveEditedCommunity,
            joinCommunity: joinCommunity,
            deleteCommunity: deleteCommunity,
            requestResponse: requestResponse,
            requireAuth:requireAuth
        };

        //Enum object with friendly names and values
        // from getAllTypes - includes userid and isAdmin
        var types,
            currentUserId,
            isAdmin,
            sessionStart;

        //EntityController.cs [Route("Entity/Types/GetAll")]
        function getAllTypes() {
            var deferred = $q.defer();
            if (types) {
                $timeout(function () {
                    deferred.resolve(types);
                });
            } else {
                var url = '/Entity/Types/GetAll';
                $http.get(url, {}).
                    success(function (data, status, headers, config) {
                        data.highlightValues = convertCamel(data.highlightValues);
                        data.categoryValues = convertCamel(data.categoryValues);
                        data.contentValues = getFriendlyTypes(data.contentValues);
                        currentUserId = data.currentUserId;
                        if (currentUserId != 0 && $('#signin').text() === 'Sign in') {
                            try {
                                wwt.viewMaster.signIn();
                            } catch (er) {
                            }
                        }
                        isAdmin = data.isAdmin;
                        types = data;
                        deferred.resolve(types);
                    }).
                    error(function (data, status, headers, config) {
                        handleError(data, status, headers, config, deferred);
                    });
            }
            return deferred.promise;
        }

        //EntityController.cs
        //[Route("Entity/Browse/{highlightType}/{entityType}/{categoryType}/{contentType}/{page}/{pageSize}")]
        function getEntities(args) {
            var deferred = $q.defer();
            var url = '/Entity/Browse/' +
                args.highlightType + '/' +
                args.entityType + '/' +
                args.categoryType + '/' +
                args.contentType + '/' +
                args.currentPage + '/' +
                args.pageSize;
            
            ajaxProxy({
                url: url,
                method:'GET',
                deferred: deferred,
                processData: true,
                member: 'entities'
            });
            return deferred.promise;
        }
        //ContentController.cs: [Route("Content/Downloads/Increment/{id}")]
        function incrementDownloadCount(id) {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Content/Downloads/Increment/' + id,
                deferred: deferred
            });
            return deferred.promise;
        }
        //ContentController.cs: [Route("Content/Detail/{Id}")]
        function getEntityDetail(args) {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Content/Detail/' + args.entityId,
                method: 'GET',
                deferred: deferred, 
                processData: true, 
                processSingle: true
            });
            return deferred.promise;
        }

        //CommunityController.cs: [Route("Community/Detail/{id}")]
        function getCommunityDetail(id, isEdit) {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Community/Detail/' + id + '/' + (isEdit === true),
                method:'GET',
                deferred: deferred,
                processData: true,
                processSingle: true,
                member: 'community'
            });
            return deferred.promise;
        }
        //CommunityController.cs: [Route("Community/Contents/{communityId}")]
        function getCommunityContents(id) {
            var deferred = $q.defer();
            $http.get('/Community/Contents/' + id, {}).
                success(function (data, status, headers, config) {
                    data.entities = dataHelper(data.entities);
                    data.childCommunities = dataHelper(data.childCommunities);
                    deferred.resolve(data);
                }).
                error(function (data, status, headers, config) {
                    handleError(data, status, headers, config, deferred);
                });
            return deferred.promise;
        }
        //CommunityController.cs: [Route("Community/Join/{communityId}/{userRole}")]
        function joinCommunity(id, role) {
            var deferred = $q.defer();
            
            ajaxProxy({
                url: '/Community/Join/' + id + '/' + role,
                requestParams: { comments: " " },
                deferred: deferred
            });

            return deferred.promise;
        }
        //CommunityController.cs: [Route("Community/Delete/{Id}/{parentId}")]
        function deleteCommunity(id, parentId) {
            var deferred = $q.defer();
            ajaxProxy({
                url:'/Community/Delete/' + id + '/' + parentId, 
                deferred: deferred//,
                //method:'DELETE'
            });
            return deferred.promise;
        }

        //ContentController.cs: [Route("Content/Edit/{id}")]
        function getEditContent(id) {
            var deferred = $q.defer();
            getAllTypes().then(function () {
                ajaxProxy({
                    url: '/Content/Edit/' + id,
                    method:'GET',
                    deferred:deferred, 
                    processData:true, 
                    processSingle:true
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
            ajaxProxy({
                url:url,  
                deferred:deferred, 
                processData:true, 
                member:'searchResults'
            }); 
            return deferred.promise;
        }
        
        //ProfileController.cs [Route("/Profile/MyProfile/Get")]
        function getMyProfile() {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Profile/MyProfile/Get',
                method:'GET',
                deferred: deferred
            });
            return deferred.promise;
        }

        //ProfileController.cs [Route("Profile/Save/{profileId}")]
        //profile obj = (string affiliation, string aboutMe, bool isSubscribed, Guid? profileImageId, string profileName)
        function saveProfile(profile) {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Profile/Save/' + profile.profileId,
                //method: 'PUT',
                requestParams:profile,  
                deferred: deferred
            });
            return deferred.promise;
        }

        //ContentController.cs[Route("Content/New/{id}")]
        function publishContent(content) {
            var deferred = $q.defer();
            ajaxProxy({
                url:'/Content/Create/New', 
                requestParams:{ contentInputViewModel: content }, 
                deferred: deferred
            });
            return deferred.promise;
        }

        //ContentController.cs [Route("Content/Save/Edits")]
        function saveEditedContent(content) {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Content/Save/Edits',
                //method:'PUT',
                requestParams:{ contentInputViewModel: JSON.stringify(content) }, 
                deferred: deferred
            });
            return deferred.promise;
        }

        //ProfileController.cs:[Route("Profile/Entities/{entityType}")]
        function getUserEntities(type, profileId) {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Profile/Entities/' + type + '/' + 1 + '/' + 999,
                requestParams: { profileId: profileId },
                deferred: deferred,
                processData: true,
                method:'GET',
                member: 'entities'
            });
            return deferred.promise;
        }

        //CommunityController.cs:[Route("Community/Permission/{permissionsTab}/{currentPage}")]
        function getUserRequests() {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Community/Permission/ProfileRequests/1',
                method:'GET',
                deferred: deferred
            });
            return deferred.promise;
        }
        //CommunityController.cs:[Route("Community/Request/Reponse")]
        function requestResponse(args) {//args: long entityId, long requestorId, UserRole userRole, PermissionsTab permissionsTab, bool approve
            var deferred = $q.defer();
            ajaxProxy({
                url:'/Community/Request/Reponse', 
                requestParams:args,
                deferred: deferred
            });
            return deferred.promise;
        }
        //[Route("Content/User/CommunityList")]
        function getUserCommunityList() {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Content/User/CommunityList',
                method:'GET',
                deferred: deferred
            });
            return deferred.promise;
        }

        //CommunityController.cs:[Route("Community/Edit/Save")]
        function saveEditedCommunity(community) {
            var deferred = $q.defer();
            ajaxProxy({url:'/Community/Edit/Save', 
                requestParams: community,
                //method:'PUT',
                deferred: deferred
            });
            return deferred.promise;
        }
        //ContentController.cs[Route("Content/Delete/{id}")]
        function deleteContent(contentId) {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Content/Delete/' + contentId,
               // method:'DELETE',
                deferred: deferred
            });
            return deferred.promise;
        }

        //CommunityController.cs[Route("Community/Create/New")]
        function createCommunity(community) {
            var deferred = $q.defer();
            ajaxProxy({
                url: '/Community/Create/New',
                requestParams: { communityJson: community },
                deferred: deferred
            });
            return deferred.promise;
        }

        function requireAuth() {
            var deferred = $q.defer();
            var refreshTypes = function() {
                types = null;// this will update user info
                getAllTypes().then(function() {
                    if (currentUserId > 0) {
                        deferred.resolve(types);
                    } else {
                        deferred.reject('unknown');
                    }
                });
            };
            if (currentUserId && currentUserId > 0) {
                $timeout(function() {
                    deferred.resolve(types);
                });
            } else {
                getAllTypes().then(function() {
                    if (currentUserId && currentUserId > 0) {
                        deferred.resolve(types);
                    } else if ($('#signinContainer').attr('loggedIn') === 'true') {
                        refreshTypes();
                    } else {
                        wwt.viewMaster.signIn();
                        $(window).on('login', refreshTypes); // XXX nothing ever triggers this, I think
                    }
                });
            }
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
        
        //helper function that actually performs most posts / dataprocessing
        var ajaxProxy = function (args) {
            //args: url, requestParams, deferred, processData, processSingle, member,method
            if (new Date().valueOf() - sessionStart.valueOf() > 3600000) {//session expiry after an hour
                location.reload();
                return;
            }
            
            var httpAction = function () {
                var req = {
                    method: args.method || 'POST',
                    url: args.url,
                    data:args.requestParams || {}
                }
                $http(req)
                    .success(successCallback)
                    .error(errorCallback);
            }
            if (args.processData) {
                getAllTypes().then(httpAction);
            } else {
                httpAction();
            }
            function successCallback(data) {
                if (typeof data === 'string' && data.indexOf('error:') === 0 && data.indexOf('user not logged in') !== -1) {
                    location.reload();
                    return;
                }
                if (data['Data']) {
                    data = data.Data;
                }
                if (args.processData) {
                    if (args.processSingle && !args.member) {
                        data = dataHelper([data])[0];
                    } else if (args.member) {
                        if ($.isArray(args.member)) {
                            $.each(args.member, function () {
                                data[this] = dataHelper(data[this]);
                            });
                        } else if (args.processSingle) {
                            data[args.member] = dataHelper([data[args.member]])[0];
                        } else {
                            data[args.member] = dataHelper(data[args.member]);
                        }
                    } else {
                        data = dataHelper(data);
                    }
                }
                console.log(data);
                if (typeof data == 'string' && data.indexOf('error') === 0) {
                    handleError(data, status, headers, config, args.deferred);
                } else {
                    args.deferred.resolve(data);
                }
                setTimeout(wwt.triggerResize, 333);
            }
            function errorCallback(data, status, headers, config) {
                handleError(data, status, headers, config, args.deferred);
                wwt.triggerResize();
            }
        }

        //#region helpers
        var hasEmptyOrNullGuid = function (guid) {
            return !guid || guid.replace(/0/g, '') === '----';
        };

        function dataHelper(collection) {
            try {
                $.each(collection, function(i, item) {
                    // add webclient url to tours and collections
                    var isCommunity = item.MemberCount != undefined || item.CommunityType != undefined;
                    if (hasEmptyOrNullGuid(item.ThumbnailID)) {
                        if (isCommunity) {
                            item.ThumbnailUrl = $rootScope.contentRoot + '/images/defaultcommunitythumbnail.png';
                        } else {
                            item.ThumbnailUrl = $rootScope.contentRoot + '/images/default' + $.trim(types.contentValues.getTypeName(item.ContentType)).toLowerCase() + 'thumbnail.png';
                        }
                        item.ThumbnailIsNull = true;
                    } else {

                        item.ThumbnailUrl = '/file/thumbnail/' + item.ThumbnailID;
                    }

                    if (!isCommunity) {
                        var itemLink = uiHelper.getDownloadUrl(item.FileName, (item.ContentAzureID || item.ContentDataID), item.ContentType);
                        if (itemLink.DownloadUrl) {
                            item.DownloadUrl = itemLink.DownloadUrl;
                        } else if (itemLink.LinkUrl) {
                            item.LinkUrl = itemLink.LinkUrl;
                        }

                        item.FileSize = !item.Size ? 'n/a' : uiHelper.getFileSizeString(item.Size);
                        if (item.ContentType === 1) {
                            item.webclientUrl = 'http://' + location.host + '/webclient/?tourUrl=' +
                                encodeURIComponent('http://' + location.host + item.DownloadUrl);
                        } else if (item.ContentType === 0) { //TODO: Find out why type can be "all"
                            item.ContentType = 7;
                        } else if (item.ContentType === 2) {
                            item.webclientUrl = 'http://' + location.host + '/webclient/?wtml=' +
                                encodeURIComponent('http://' + location.host + item.DownloadUrl);
                        }
                        item.ContentTypeName = types.contentValues.getName(item.ContentType);
                        if (item.AssociatedFiles && item.AssociatedFiles.length) {
                            $.each(item.AssociatedFiles, function(i, file) {
                                var fileLink = uiHelper.getDownloadUrl(file.Name, file.ID);
                                if (fileLink.DownloadUrl) {
                                    file.DownloadUrl = fileLink.DownloadUrl;
                                } else if (fileLink.LinkUrl) {
                                    file.LinkUrl = fileLink.LinkUrl;
                                }
                            });
                        }
                        //deserialize extended data from public tours
                        if (item.Citation && item.Citation.indexOf('json://') === 0) {
                            var json = item.Citation.split('json://')[1];
                            try {
                                item.extData = JSON.parse(json);
                                item.Citation = item.extData.organization;
                                item.Tags = item.extData.tags;
                            } catch (er) {
                                console.log('missing ext data', item);
                            }

                        }
                    }
                    if (item.DistributedBy && item.DistributedBy.indexOf('<') === 0) {
                        item.DistributedBy = $(item.DistributedBy).text();
                    }


                });
                return collection;
            } catch (er) {
                console.log('datahelper: error in collection - ', er, collection);
                return [];
            }
        }

        var getFriendlyTypes = function(array) {
            var friendly = [];
            var pushFriendly = function(v, f, i) {
                friendly.push({ val: v, name: f, index:i });
            }
            $.each(array, function(i, s) {
                switch (s) {
                    
                    case 'Tours':
                        pushFriendly(s, 'Tour', i);
                        break;
                    case 'Wtml':
                        pushFriendly(s, 'Collection', i);
                        break;
                    case 'Excel':
                        pushFriendly(s, 'Spreadsheet', i);
                        break;
                    case 'Doc':
                        pushFriendly(s, 'Word Doc', i);
                        break;
                    case 'Ppt':
                        pushFriendly(s, 'PowerPoint', i);
                        break;
                    case 'Link':
                        pushFriendly(s,s,i);
                        break;
                    case 'Generic':
                        pushFriendly(s,s,i);
                        break;
                    case 'Wwtl':
                        pushFriendly(s, '3d Model',i);
                        break;
                    case 'Video':
                        pushFriendly(s,s,i);
                        break;

                }
            });
            friendly = sortAndExtend(friendly);
            friendly.splice(0, 0, { val: 'All', name: 'All', index: 0 });
            return friendly;
        };
        
        var convertCamel = function(array) {
            var converted = [];
            $.each(array, function (i, s) {
                var fname = s.indexOf('WWT') !== -1 ? s.replace('WWT', ' WWT ') : s.replace(/([A-Z]+)/g, "$1").replace(/([A-Z][a-z])/g, " $1");
                converted.push({
                    val: s,
                    name: $.trim(fname),
                    index:i
                });
            });
            return sortAndExtend(converted);

        }

        var sortAndExtend = function(array) {
            var sorted = array.sort(function (a, b) {
                if (a.name > b.name) {
                    return 1;
                }
                if (a.name < b.name) {
                    return -1;
                }
                return 0;
            });


            var special = function () {
                this.array = sorted;
                return this.array;
            };

            special.prototype = Array.prototype;

            var getTypeOrName = function(array, index, getName) {
                if (index) {
                    var result;
                    $.each(array, function (i, item) {
                        if (item.index === index) {
                            result = getName?item.name:item.val;
                        }
                    });
                    return result;
                }
                return '';
            }

            special.prototype.getName = function (index) {
                return getTypeOrName(this, index, true);
            }
            special.prototype.getTypeName = function (index) {
                return getTypeOrName(this, index);
            }
            return special();
        }

        var handleError = function(data, status, headers, config, deferred) {
            console.log({ data: data, status: status, headers: headers, config: config });
            deferred.resolve({ error: true, data: data, status: status, headers: headers, config: config });
        };
        //#endregion

        sessionStart = new Date();

        return api;
    }
]);