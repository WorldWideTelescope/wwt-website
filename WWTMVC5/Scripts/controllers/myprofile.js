wwtng.controller('MyProfile', [
    '$scope',
    'dataproxy',
    '$timeout',
    '$routeParams',
    'UIHelper',
    'FileUploader',
    function ($scope, dataproxy, $timeout, $routeParams, uiHelper, fileUploader) {

        function init() {

            if ($('#signin').length && !$('#signin').prop('authenticated')) {
                location.href = '#/';
                return;
            }
            $scope.options = {
                activeTab: 'uploads'
            };
            uiHelper.fixLinks('profileLink');
            dataproxy.getAllTypes().then(function (types) {
            $scope.types = types;
            dataproxy.getMyProfile(0)
                .then(setProfile);
            });
        }

        $scope.uploader = new fileUploader({
            url: "/Entity/AddThumbnail/User",
            autoUpload: true,
            onSuccessItem: success,
            onErrorItem: log,
            alias: 'thumbnail'
        });

        function success(xhr, response) {
            $timeout(function() {
                $scope.editProfile.profileImageId = response.ThumbnailID;
            });
        }

        function log() {
            console.log({ uploader: arguments });
        }

    

        function setProfile(profile) {
            if (typeof profile == 'string' && profile.indexOf('error') === 0) {
                location.href = '#/';
            }
            $scope.profile = profile;
            //string affiliation, string aboutMe, bool isSubscribed, Guid? profileImageId, string profileName
            $scope.editProfile = {};
            $scope.editProfile.profileName = profile.ProfileName;
            $scope.editProfile.aboutMe = profile.AboutProfile;
            $scope.editProfile.affiliation = profile.Affiliation,
            $scope.editProfile.profileId = profile.ProfileId;
            $scope.editProfile.isSubscribed = profile.IsSubscribed;
            dataproxy.getUserEntities('Content', profile.ProfileId).then(function (response) {
                $scope.profile.uploads = response.entities && response.entities.length ? response.entities : null;
                wwt.triggerResize();
            });
            dataproxy.getUserEntities('Community', profile.ProfileId).then(function (response) {
                $scope.profile.communities = response.entities && response.entities.length ? response.entities : null;
                wwt.triggerResize();
            });
            getRequests();
        }

        var getRequests = function() {
            dataproxy.getUserRequests().then(function (response) {
                $scope.profile.requests = response.PermissionItemList && response.PermissionItemList.length ? response.PermissionItemList : null;
                wwt.triggerResize();
            });
        }

        $scope.tabChange = function(tab) {
            $scope.options.activeTab = tab;
            wwt.triggerResize();
        }

        $scope.saveProfile = function() {
            dataproxy.saveProfile($scope.editProfile).then(function(response) {
                console.log(response);
                setProfile(response);
            });
        }
        $scope.rememberMe = wwt.user.get('rememberMe');
        $scope.rememberMeChange = function() {
            wwt.user.set('rememberMe', $scope.rememberMe);
        }
        $scope.logout = function() {
            wwt.user.set('rememberMe', false);
            location.href = '/Logout';
        }

        $scope.approveRequest = function(r) {
            //args: long entityId, long requestorId, UserRole userRole, bool approve
            var row = $('tr[requestHash="' + r.$$hashKey + '"]');
            var roles = ['', 'Reader', 'Contributor', 'Moderator'];
            var role = roles[parseInt(row.find('select').val())];
            var requestArgs = {
                entityId: r.CommunityId,
                requestorId: r.Id,
                userRole: role,
                approve: true
            }
            dataproxy.requestResponse(requestArgs).then(function (response) {
                console.log({ update: response });
                getRequests();
            });
        }
        $scope.denyRequest = function (r) {
            //args: long entityId, long requestorId, UserRole userRole, bool approve
            var roles = ['','Reader','Contributor','Moderator'];
            var requestArgs = {
                entityId: r.CommunityId,
                requestorId: r.Id,
                userRole: roles[r.Role],
                approve: false
            }
            dataproxy.requestResponse(requestArgs).then(function (response) {
                console.log({ update: response });
                getRequests();
            });
        }

        init();
}]);