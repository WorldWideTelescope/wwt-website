wwtng.factory('UIHelper', ['$rootScope', function($rootScope) {
    var api = {
        fixLinks: fixLinks,
        getFileSizeString: getFileSizeString,
        getDownloadUrl: getDownloadUrl,
        positiveIntParamExists: positiveIntParamExists,
        imageHelper:imageHelper
    };

    //fresh thumbnails sometimes need a couple of seconds to load
    function imageHelper(image) {
        var retry = 0;
        //let angular render view first
        setTimeout(function () {
            image = $(image).first();
            image.attr('src', image.attr('src') + '?retry=0');
            image.on('error', function () {
                retry++;
                console.log(retry);
                if (retry < 20) {
                    image.attr('src', image.attr('src').split('?')[0] + "?retry=" + retry);
                }
            });

        }, 100);
        
    }

    $rootScope.contentRoot = $('.community-content').attr('content-location');

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
    function positiveIntParamExists(param, routeParams, scope) {
        var exists = routeParams && routeParams[param] && parseInt(routeParams[param]) > 0;
        if (exists) {
            scope[param] = routeParams[param];
        }
        return exists;
    };
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
            var ext = null;
            if (type) {
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
            var fileSplit = fileName.split('.');
            if (!ext){
                if (fileSplit.length === 1) {
                    console.log(fileSplit);
                }
                if (fileSplit.length > 1 && fileSplit[fileSplit.length - 1].length <= 5) {
                    ext = fileSplit[fileSplit.length - 1];
                    fileSplit.pop();
                }
            }
            var file;
            
            if (fileSplit.length > 1) {
                if (ext && fileSplit[fileSplit.length - 1] === ext) {
                    fileSplit.pop();
                }
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