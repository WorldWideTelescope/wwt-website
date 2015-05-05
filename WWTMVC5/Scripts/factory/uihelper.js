wwtng.factory('UIHelper', ['$rootScope', function($rootScope) {
    var api = {
        fixLinks: fixLinks
    };

    function fixLinks(activeNav) {
        $('.bs-sidenav li').removeClass('active');
        $('#' + activeNav).addClass('active');
        $('#communityLink a').attr('href', '#/');
        $('#profileLink a, #username').attr('href', '#/MyProfile');
        wwt.triggerResize();
    }

    return api;
}])