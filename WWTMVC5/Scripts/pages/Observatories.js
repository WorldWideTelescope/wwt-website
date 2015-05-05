wwt.Observatories = (function() {
    var api = {
        init: init
    };

    var resLoc, interactMode, ctl, canvasOverlay, canvas;

    function init(opts) {
        resLoc = opts.resLoc;
        //opts.onresize = resizeHandler;
        opts.hideUI = true;
        wwt.WebControl.init(opts);
        ctl = wwt.WebControl.ctl();
        initElements();
        bindEvents();
        loadWtml(function (xml) {
            //console.log(x.getResponseHeader("Content-Type"));
            //console.log($(xml).find('Folder'));
            var places = $(xml).find('Place');
            var thumbTemplate = $('<div class="col-xs-3 col-md-2"><a href="javascript:void(0)" class="thumbnail"><img src=""/><i class="fa fa-info-circle"></i></a></div>');
            places.each(function (i, pl) {
                var place = $(pl);
                var descText = '', desc;
                desc = place.find('Description').text().split('\n');
                $.each(desc, function (i, item) {
                    if (item != undefined) {
                        descText += '<p>' + item + '</p>';
                    }
                });
                descText += '<hr><h4>Credits</h4>' +
                    '<p><a href="' + place.find('CreditsUrl').text() + '" target=_blank >' +
                    place.find('Credits').text() + '</p>';
                var tmp = thumbTemplate.clone();
                tmp.find('img').attr({
                    src: place.find('ThumbnailUrl').text(),
                    alt: place.attr('Name'),
                    'data-toggle':'tooltip', 
                    'data-placement': 'top',
                    'data-container':'body',
                    title: place.find('Description').attr('Title')
                });
                tmp.find('a')
                    .data('foreground-image', place.attr('Name'))
                    .on('click', function() {
                    wwt.WebControl.setForegroundImage({
                        imgName: $(this).data('foreground-image'),
                        RA: parseFloat(place.attr('RA')) * 15,
                        Dec: parseFloat(place.attr('Dec')),
                        Scale: parseFloat(place.find('ImageSet').attr('BaseDegreesPerTile')),
                        Description: descText,
                        Title:place.find('Description').attr('Title')
                    });
                    });
                //tmp.find('img').tooltip();
                tmp.find('i').attr({
                    'data-toggle': 'tooltip',
                    'data-placement': 'top',
                    title: 'Image Information'
                })
                .on('click', function(e) {
                    bootbox.dialog({
                        message: descText,
                        title: place.find('Description').attr('Title')
                    });

                    e.preventDefault();
                    e.stopPropagation();
                }).tooltip();
                $('#divInteractive .row').append(tmp);
                if (i<6)
                $('.player-controls .btn').first().before(tmp.clone(true).find('a'));
            });
            $('.thumbnail img').tooltip();
            
        });
        
       
    }

    var initElements = function() {
        canvasOverlay = $('.canvas-overlay').show();
        canvas = $('#WWTCanvas');
        $('.obvs-div img[data-nofs]').css({ margin: '0 16px 0 0' });
    };
    var bindEvents = function() {
        canvasOverlay.find('a').on('click', startMode);
        $('a.start-mode').on('click', startMode);
        $('#btnSpectroscopyOverview').on('click', function() {
            toggleInteractionChrome(false);
            interactMode = true;
            wwt.WebControl.playTour();
        });

        $('a.thumbnail').on('click', function() {
            $('a.thumbnail').removeClass('active');
            $(this).addClass('active');
            $('.obvs-div').hide();
            $('#divGA' + $(this).data('toggle-key')).show();
            wwt.triggerResize();
        });

    };

    var toggleInteractionChrome = function(show) {
        $('#divInteractive').removeClass('hide').toggle(show);
        $('.wwt-webcontrol-wrapper').toggle(show);
        $('#divOverview').toggle(!show);
        wwt.triggerResize();
    };

    var loadWtml = function(callback) {
        var wtmlPath = "/Content/WTML/GreatObservatories.xml";
        ctl.add_collectionLoaded(function() {
            $.ajax({
                url: wtmlPath,
                crossDomain: false,
                dataType: 'xml',
                cache:false,
                success: callback,
                error: function(a,b,c) {
                    console.log({ a: a, b: b, c: c });
                }
            });
        });
        ctl.loadImageCollection(wtmlPath);
    };
    //#endregion


    var startMode = function() {
        interactMode = $(this).attr('data-mode') === 'interact';
        if (interactMode) {
            interactionMode();
        } else {
            $('a.thumbnail').first().click();
            $('.obvs-div').hide();
            $('#divOverview').removeClass('hide').show();
            $('#divGASpectrum').show();
            toggleInteractionChrome(false);
        }
        
        canvasOverlay.hide();

        wwt.triggerResize();
    };

    var interactionMode = function() {
        interactMode = true;
        wwt.WebControl.interact();
        toggleInteractionChrome(true);

        wwt.triggerResize();
    };

    return api;

})();