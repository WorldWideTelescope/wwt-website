



(function () {
    var hostUrl = 'http://worldwidetelescope.org';
    var div = document.querySelector('div[data-wwt-tour-location]');
    var wid = div.offsetWidth;
    var tourLocation = div.getAttribute('data-wwt-tour-location');
    var iframe = document.createElement('iframe');
    iframe.setAttribute('src', hostUrl + '/HTML5TourPlayer.aspx#' + tourLocation + '&&http://' + location.hostname);
    iframe.style.border = 'none';
    iframe.style.backgroundColor = 'transparent';
    iframe.setAttribute('frameborder', 'no');
    iframe.setAttribute('scrolling', 'no');
    iframe.style.height = (wid * .6667) + 'px';
    iframe.style.width = '100%';
    div.appendChild(iframe);
    window.addEventListener("message", receiveMessage, false);
    function receiveMessage(event) {
        if (event.origin === hostUrl) {
            if (event.data === 'fullscreen') {
                fullScreenMode();
                //test.fs();
            }
        }

    };

    var fullscreen = false;
    //var isChangingFs = false;

    var requestFullScreen = function (element) {
        if (element.requestFullscreen) {
            element.requestFullscreen();
        } else if (element.msRequestFullscreen) {
            element.msRequestFullscreen();
        } else if (element.mozRequestFullScreen) {
            element.mozRequestFullScreen();
        } else if (element.webkitRequestFullscreen) {
            element.webkitRequestFullscreen();
        } else {
            console.log("Fullscreen API is not supported");
        }
    };

    var exitFullScreen = function () {
        var previousFullScreen = document.fullScreenElement || document.mozFullScreenElement || document.webkitFullscreenElement;
        if (previousFullScreen) {

            if (previousFullScreen.cancelFullScreen) {
                previousFullScreen.cancelFullScreen();
            } else if (document.mozCancelFullScreen) {
                document.mozCancelFullScreen();
            } else if (document.webkitCancelFullScreen) {
                document.webkitCancelFullScreen();
            }
        } else if (document.msExitFullscreen) {
            document.msExitFullscreen();
        } 
    };

    var fullScreenMode = function () {
        //isChangingFs = true;
        //setTimeout(function () { isChangingFs = false; }, 888);
        $('#btnFS').click();
        if (!fullscreen) {

            requestFullScreen(document.body);
            div.style.width = '100%';
            div.style.height = '100%';
            iframe.style.height = '100%';
            iframe.style.width = '100%';
            div.style.position = 'absolute';
            div.style.top = 0;
            div.style.left = 0;
            fullscreen = true;
            
        } else {
            exitFullScreen(document.body);
            div.style.width = wid;
            div.style.height = (wid * .6667) + 'px';
            iframe.style.height = (wid * .6667) + 'px';
            iframe.style.width = wid + 'px';
            div.style.position = 'static';
            
            fullscreen = false;
            
        }
        
    };
})();