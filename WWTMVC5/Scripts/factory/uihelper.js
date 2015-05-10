wwtng.factory('UIHelper', ['$rootScope', function($rootScope) {
    var api = {
        fixLinks: fixLinks,
        getFileSizeString: getFileSizeString,
        getDownloadUrl:getDownloadUrl
    };

    function getFileSizeString(bytes) {
        var thresh = 1000;
        if (bytes < thresh) return bytes + ' B';
        var units = ['KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
        var u = -1;
        do {
            bytes /= thresh;
            ++u;
        } while (bytes >= thresh);
        return bytes.toFixed(1) + ' ' + units[u];
    }

    function fixLinks(activeNav) {
        $('.bs-sidenav li').removeClass('active');
        $('#' + activeNav).addClass('active');
        $('#communityLink a').attr('href', '#/');
        $('#profileLink a, #username').attr('href', '#/MyProfile');
        wwt.triggerResize();
    }
    function getDownloadUrl(fileName, guid, type) {
        var obj = {};
        if ($.trim(fileName).indexOf('http') === 0) {
            obj.LinkUrl = fileName;
        } else if (fileName) {
            var fileSplit = fileName.split('.');
            if (fileSplit.length === 1) {
                console.log(fileSplit);
            }
            var ext = fileSplit.length > 1 ? fileSplit[fileSplit.length - 1] : null;
            if (ext === null && type) {
                switch (type) {
                    case 1:
                        ext = 'wtt';
                        break;
                    case 2:
                        ext = 'wtml';
                        break;
                    case 3:
                        ext = 'xlsx';
                        break;
                    case 4:
                        ext = 'docx';
                        break;
                    case 5:
                        ext = 'pptx';
                        break;
                    case 8:
                        ext = 'wwtl';
                        break;
                }
            }
            var file;
            if (fileSplit.length > 2) {
                fileSplit.pop();
                file = fileSplit.join('_');
            } else {
                file = fileSplit[0];
            }
            obj.DownloadUrl = '/file/Download/' + guid + '/' + file;
            if (ext) {
                obj.DownloadUrl += '/' + ext;
            }
        } else {
            obj.DownloadUrl = '/file/Download/' + guid + '/File';
        }
        return obj;
    }
    return api;
}])