wwt.TourPlayer = (function () {
    //#region global variables / api
    var api = {
        init: init
    };

    var ctl,
        overlay,
        canvas;
    //#endregion

    //#region initialization
    function init(opts) {
        $('#WorldWideTelescopeControlHost').append('<div class="canvas-overlay"><a href="javascript:void(0)" class="row" data-mode="tour"><i class="fa fa-play"></i></a></div>');
        var controlOpts = {
            resLoc: opts.resLoc,
            defaultTour: opts.tour,
            hideUI: true
        };
        if (opts.remote)
            controlOpts.remote = opts.remote;
        wwt.WebControl.init(controlOpts);
        ctl = wwt.WebControl.ctl();
        initElements();
        bindEvents();
        
    }

    var initElements = function () {
        overlay = $('.canvas-overlay');
        canvas = $('#WWTCanvas');
        var icon = overlay.find('i');
        overlay.show()
            .css({
                top: 0,
                left: 0,
                right: 0,
                bottom: 0,
                height: canvas.height(),
                width:canvas.width()
            });

        overlay.find('a').css({
            position: 'absolute',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            paddingLeft: (overlay.width() / 2) - (icon.width() / 2),
            paddingTop: (overlay.height() / 2) - (icon.height() / 2),
            margin: 0
        }).attr('title', 'Play tour...');
    };

    var bindEvents = function () {
        overlay.find('a').on('click', function() {
            overlay.hide();
            wwt.WebControl.playTour();
        });
    };
    //#endregion

    //#region event handlers
    
    //#endregion

    return api;

})();