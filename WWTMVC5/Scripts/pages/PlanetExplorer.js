wwt.PlanetExplorer = (function() {
    var api = { init: init }
    var resLoc, ctl, root, planets;

    //var planetNames = ['Mercury','Venus','Earth','Mars','Jupiter','Saturn','Uranus','Neptune','Pluto'];

    function init(opts) {
        resLoc = opts.resLoc;
        wwt.WebControl.init(opts);
        ctl = wwt.WebControl.ctl();
        initElements();
        bindEvents();
        root = ctl.createFolder();
        
        var loadSSView = function () {
            var imageSets = wwtlib.WWTControl.imageSets;
            $.each(imageSets, function() {
                if (this.get_dataSetType() === 4) {
                    ctl.setBackgroundImageByName(this.get_name());
                    ctl.gotoRaDecZoom(22, 90, 20);
                    ctl.settings.set_solarSystemLighting(false);
                }
            });
            var folders = root.get_children();

            var solarSystemFolder = folders[1];
            solarSystemFolder.childLoadCallback(function () {
                var thumbTemplate = $('<div class="col-xs-3 col-md-2"><a href="javascript:void(0)" class="thumbnail"><img /><i class="fa fa-info-circle"></i><label></label></a></div>');
                planets = solarSystemFolder.get_children().splice(2, 9);
                $.each(planets, function (i, planet) {
                    var thumb = thumbTemplate.clone();
                    thumb
                        .find('a')
                        .data('place', this)
                        .on('click',thumbClick)
                        .find('img').attr({
                            src: this.get_thumbnailUrl(),
                            alt:this.get_name()
                        });
                    thumb.find('label').text(this.get_name());
                    thumb.find('i').attr({
                        'data-toggle': 'tooltip',
                        'data-placement': 'top',
                        title: 'Image Information'
                    })
                    .on('click', function(e) {
                        bootbox.dialog({
                            message: $('#mdl' + planet.get_name()).html(),
                            title: planet.get_name()
                        });
                        e.stopPropagation();
                    });
                    $('#planetThumbs').append(thumb);
                    var fsThumb = thumb.clone(true).find('a');
                    fsThumb.find('label').remove();
                    $('.player-controls .btn').first().before(fsThumb);
                });
                wwt.triggerResize();
            });
        };

        root.loadFromUrl('http://www.worldwidetelescope.org/wwtweb/catalog.aspx?W=ExploreRoot', function(){
            setTimeout(loadSSView,333);
        });
        
    }

    function initElements() {
        $('#planetInfo').children().each(function() {
            var modal = $(this);
            if (modal.attr('imgUrl') !== '') {
                var image = $('<img/>').attr({
                    src: modal.attr('imgUrl'),
                    title: modal.attr('hover'),
                    alt: modal.attr('hover')
                }).addClass('img-responsive');
                var credits = $('<div><small><em class=text-muted><strong>Image Credit:&nbsp;<strong><span></span></em></small></div>')
                    .css('margin-bottom', 8);
                credits.find('span').text(modal.attr('credits'));
                modal.find('h2').after(image);
                image.after(credits);
            }
            modal.find('h2').hide();
        });
    }

    function bindEvents() { }

    function thumbClick() {
        //wwtlib.WWTControl.singleton.renderContext.set_foregroundImageset(
        wwtlib.WWTControl.singleton.gotoTarget($(this).data('place'), false, false, true);
    }

    return api;
})()
