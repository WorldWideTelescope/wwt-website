

wwt.home = (function () {
    var api = {
        init: init,
        replayVideo: replayVideo
    };



    var videoContainer,
        youtube,
        youtubeTed,
        videoaspect = 9 / 16,
        newsToggleLink,
        hideNews = false,
        showVideo = true,
        twitterLoadedTimer,
        replayVideoContainer,
        replayVideoButton;

    function init() {
        initElements();
        bindEvents();
        resize(true);
    }

    function initElements() {
        videoContainer = $('#videoContainer');
        newsToggleLink = $('a#hideNews');
        replayVideoContainer = $('#replayVideo');
        replayVideoButton = replayVideoContainer.find('.btn');
        showVideo = !(wwt.user.get('showAASVideo') === false);
        $('#chkShowVideo').prop('checked', showVideo);
        replayVideoContainer.toggle(!showVideo).removeClass('hide');
        videoContainer.toggle(showVideo).removeClass('hide');
        if (!wwt.user.get('hideNews')) {
            toggleNews();
        }
        var i;
        twitterLoadedTimer = setInterval(function () {
            i++;
            if (i > 30 || !$('a.twitter-timeline').length) {
                clearInterval(twitterLoadedTimer);
                wwt.triggerResize();
            }
        }, 100);
        $('.carousel-indicators li').tooltip();
    };

    var bindEvents = function () {
        $('#chkShowVideo').on('change', function() {
            wwt.user.set('showAASVideo', $(this).prop('checked'));
        });
        $('#closeHeroVideo').on('click', fadeOutVideo);
        replayVideoButton.on('click', replayVideo);

        $(window).on('resize contentchange', resize);
        newsToggleLink.on('click', toggleNews);

    };

    var toggleNews = function () {
        newsToggleLink.find('i').
            toggleClass('fa-minus-circle').
            toggleClass('fa-plus-circle');
        newsToggleLink.find('span').toggleClass('hide');

        $('#newsDiv').toggleClass('hide');
        hideNews = $('#newsDiv').hasClass('hide');
        wwt.user.set('hideNews', hideNews);

        wwt.triggerResize();
    };

    var fadeOutVideo = function () {
        try {
            youtube.stopVideo();
        } catch (er) {
        }
        wwt.user.set('seenVideo', true);
        //wwt.exitFullScreen();
        videoContainer.fadeOut(400, wwt.triggerResize);
        replayVideoContainer.show();
        $('#carousel').carousel(0);
    };


    var isReplay = false;
    function replayVideo() {
        videoContainer.fadeIn(function() {
            setTimeout(function () {
                youtube.loadVideoById('d36Ix0uQ1hg');
                youtube.playVideo();
            }, 100);
        });
        isReplay = true;
        //try {
        //    youtube.playVideo();
        //} catch (er) {
        //}
        setTimeout(function() { isReplay = false; }, 1000);

        replayVideoContainer.hide();
        wwt.triggerResize();
    }

    var isSmall = false;

    function resize() {
        
        var winW = $(window).width();
        //if (winW < 676 && !isSmall) {

        //    isSmall = true;
        //    $('#heroDiv').after($('.hero-action'));
        //    $('.hero-nav').hide();
        //    $('#heroDiv .hero-backdrop').css({
        //        minWidth: '96%',
        //        left: '12%',
        //        top: '5%'
        //    });
        //}
        //else if (winW >= 676 && isSmall) {

        //    isSmall = false;
        //    $('#heroDiv').append($('.hero-action'));
        //    $('.hero-nav').show();
        //    $('#heroDiv .hero-backdrop').css({
        //        minWidth: 267,
        //        left: '17%',
        //        top: '11%'
        //    });
        //}
        
        $('.home-features img.img-responsive').css({
            marginLeft: (winW < 750 && winW > 582) ? (winW - 580) / 2 : 0
        });
        //console.log(winW);
        try {
            var w = Math.max(310, $('.large-video-player').width());

            youtube.setSize(w, w * videoaspect);
            w = $('#newsDiv').width();
            youtubeTed.setSize(w, w * videoaspect);
        } catch (er) {
            //console.log(er);
            if (youtube) {
                console.log(youtube);
            }
        }
    };


    window.onYouTubeIframeAPIReady = function () {
        var w = Math.max(310, $('.large-video-player').width());
        youtube = new YT.Player('youtube', {
            height: w * videoaspect,
            width: w,
            videoId: 'd36Ix0uQ1hg',//'vz1EsbdqN7A',
            events: {
                'onReady': function (event) {
                    //if (!wwt.user.get('seenVideo')) {
                    //	event.target.playVideo();

                    //}
                },
                'onStateChange': function (event) {
                    //console.log({ event: event });
                    if (event.data === YT.PlayerState.ENDED && !isReplay) {
                        fadeOutVideo();
                    }
                }
            }
        });

        w = $('#newsDiv').width();
        youtubeTed = new YT.Player('youtubeTed', {
            height: w * videoaspect,
            width: w,
            videoId: 'AZk0IQ9pFOU',//'xEqietDMp-U',
            events: {
                'onReady': function (event) {
                    //console.log(event);

                },
                'onStateChange': function (event) {

                }
            }
        });

    };
    var yts = document.createElement('script');

    yts.src = "https://www.youtube.com/iframe_api";
    var firstScriptTag = document.getElementsByTagName('script')[0];
    firstScriptTag.parentNode.insertBefore(yts, firstScriptTag);

    return api;
})()