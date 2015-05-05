/**
* hoverIntent r6 // 2011.02.26 // jQuery 1.5.1+
* <http://cherne.net/brian/resources/jquery.hoverIntent.html>
* 
* @param  f  onMouseOver function || An object with configuration options
* @param  g  onMouseOut function  || Nothing (use configuration options object)
* @author    Brian Cherne brian(at)cherne(dot)net
*/
(function ($) { $.fn.hoverIntent = function (f, g) { var cfg = { sensitivity: 7, interval: 100, timeout: 0 }; cfg = $.extend(cfg, g ? { over: f, out: g } : f); var cX, cY, pX, pY; var track = function (ev) { cX = ev.pageX; cY = ev.pageY }; var compare = function (ev, ob) { ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t); if ((Math.abs(pX - cX) + Math.abs(pY - cY)) < cfg.sensitivity) { $(ob).unbind("mousemove", track); ob.hoverIntent_s = 1; return cfg.over.apply(ob, [ev]) } else { pX = cX; pY = cY; ob.hoverIntent_t = setTimeout(function () { compare(ev, ob) }, cfg.interval) } }; var delay = function (ev, ob) { ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t); ob.hoverIntent_s = 0; return cfg.out.apply(ob, [ev]) }; var handleHover = function (e) { var ev = jQuery.extend({}, e); var ob = this; if (ob.hoverIntent_t) { ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t) } if (e.type == "mouseenter") { pX = ev.pageX; pY = ev.pageY; $(ob).bind("mousemove", track); if (ob.hoverIntent_s != 1) { ob.hoverIntent_t = setTimeout(function () { compare(ev, ob) }, cfg.interval) } } else { $(ob).unbind("mousemove", track); if (ob.hoverIntent_s == 1) { ob.hoverIntent_t = setTimeout(function () { delay(ev, ob) }, cfg.timeout) } } }; return this.bind('mouseenter', handleHover).bind('mouseleave', handleHover) } })(jQuery);


///verticalslider.js

var sliderNavLink;
var sliderPane;

function initSlider(sliderPaneClass, navLinkClass) {

    sliderNavLink = $(navLinkClass);
    sliderPane = $(sliderPaneClass);

    sliderPane.css('top', sliderNavLink.css('top'));
    sliderPane.css('left', sliderNavLink.css('left'));
    sliderPane.css('visibility', 'visible');

    var paneWidth = sliderPane.innerWidth() + "px";
    var paneHeight = sliderPane.height() + "px";

    setSlider(sliderPane);
    hideSlider(sliderNavLink);
    sliderNavLink.css('visibility', 'visible');

    sliderPane.hoverIntent(function () {
        $(this).animate({ width: paneWidth, height: paneHeight });
    }, function () {
        $(this).animate({ width: sliderNavLink.width(), height: sliderNavLink.height() }, 500, hideSlider(sliderNavLink));
    });

    sliderNavLink.hoverIntent(function () {
        $('.scroll-container').show();
        sliderPane.animate({ width: paneWidth, height: paneHeight });
    }, function () { }

    );
}

function hideSlider(navLink) {
    $('.scroll-container').hide();
}

function setSlider($scrollpane) {//$scrollpane is the div to be scrolled

    //set options for handle image - amend this to true or false as required
    var handleImage = true;

    //change the main div to overflow-hidden as we can use the slider now
    $scrollpane.css('overflow', 'hidden');

    //if it's not already there, wrap an extra div around the scrollpane so we can use the mousewheel later
    if ($scrollpane.parent('.scroll-container').length == 0) $scrollpane.wrap('<\div class="scroll-container"> /');
    //and again, if it's not there, wrap a div around the contents of the scrollpane to allow the scrolling
    if ($scrollpane.find('.scroll-content').length == 0) $scrollpane.children().wrapAll('<\div class="scroll-content"> /');

    //compare the height of the scroll content to the scroll pane to see if we need a scrollbar
    var difference = $scrollpane.find('.scroll-content').height() - $scrollpane.height(); //eg it's 200px longer 
    $scrollpane.data('difference', difference);

    if (difference <= 0 && $scrollpane.find('.slider-wrap').length > 0)//scrollbar exists but is no longer required
    {
        $scrollpane.find('.slider-wrap').remove(); //remove the scrollbar
        $scrollpane.find('.scroll-content').css({ top: 0 }); //and reset the top position
    }

    if (difference > 0)//if the scrollbar is needed, set it up...
    {
        var proportion = difference / $scrollpane.find('.scroll-content').height(); //eg 200px/500px

        var handleHeight = Math.round((1 - proportion) * $scrollpane.height()); //set the proportional height - round it to make sure everything adds up correctly later on
        handleHeight -= handleHeight % 2;

        //if the slider has already been set up and this function is called again, we may need to set the position of the slider handle
        var contentposition = $scrollpane.find('.scroll-content').position();
        var sliderInitial = 100 * (1 - Math.abs(contentposition.top) / difference);

        if ($scrollpane.find('.slider-wrap').length == 0)//if the slider-wrap doesn't exist, insert it and set the initial value
        {
            $scrollpane.append('<\div class="slider-wrap"><\div class="slider-vertical"><\/div><\/div>'); //append the necessary divs so they're only there if needed
        }

        $scrollpane.find('.slider-wrap').height($scrollpane.height()); //set the height of the slider bar to that of the scroll pane
        sliderInitial = 100;

        //set up the slider 
        $scrollpane.find('.slider-vertical').slider({
            orientation: 'vertical',
            min: 0,
            max: 100,
            range: 'min',
            value: sliderInitial,
            slide: function (event, ui) {
                var topValue = -((100 - ui.value) * difference / 100);
                $scrollpane.find('.scroll-content').css({ top: topValue }); //move the top up (negative value) by the percentage the slider has been moved times the difference in height
                $('ui-slider-range').height(ui.value + '%'); //set the height of the range element
            },
            change: function (event, ui) {
                var topValue = -((100 - ui.value) * ($scrollpane.find('.scroll-content').height() - $scrollpane.height()) / 100); //recalculate the difference on change
                $scrollpane.find('.scroll-content').css({ top: topValue }); //move the top up (negative value) by the percentage the slider has been moved times the difference in height
                $('ui-slider-range').height(ui.value + '%');
            }
        });

        //set the handle height and bottom margin so the middle of the handle is in line with the slider
        $scrollpane.find(".ui-slider-handle").css({ height: handleHeight, 'margin-bottom': -0.5 * handleHeight });
        var origSliderHeight = $scrollpane.height(); //read the original slider height
        var sliderHeight = origSliderHeight - handleHeight; //the height through which the handle can move needs to be the original height minus the handle height
        var sliderMargin = (origSliderHeight - sliderHeight) * 0.5; //so the slider needs to have both top and bottom margins equal to half the difference
        $scrollpane.find(".ui-slider").css({ height: sliderHeight, 'margin-top': sliderMargin }); //set the slider height and margins
        $scrollpane.find(".ui-slider-range").css({ bottom: -sliderMargin }); //position the slider-range div at the top of the slider container

        //if required create elements to hold the images for the scrollbar handle
        if (handleImage) {
            $(".ui-slider-handle").append('<img class="scrollbar-top" src="../images/scrollbar-handle-top.png"/>');
            $(".ui-slider-handle").append('<img class="scrollbar-bottom" src="../images/scrollbar-handle-bottom.png"/>');
            $(".ui-slider-handle").append('<img class="scrollbar-grip" src="../images/scrollbar-handle-grip.png"/>');
        }
    } //end if

    //code for clicks on the scrollbar outside the slider
    $(".ui-slider").click(function (event) {//stop any clicks on the slider propagating through to the code below
        event.stopPropagation();
    });

    $(".slider-wrap").click(function (event) {//clicks on the wrap outside the slider range
        var offsetTop = $(this).offset().top; //read the offset of the scroll pane
        var clickValue = (event.pageY - offsetTop) * 100 / $(this).height(); //find the click point, subtract the offset, and calculate percentage of the slider clicked
        $(this).find(".slider-vertical").slider("value", 100 - clickValue); //set the new value of the slider
    });


    //additional code for mousewheel
    if ($.fn.mousewheel) {

        $scrollpane.parent().unmousewheel(); //remove any previously attached mousewheel events
        $scrollpane.parent().mousewheel(function (event, delta) {

            var speed = Math.round(5000 / $scrollpane.data('difference'));
            if (speed < 1) speed = 1;
            if (speed > 100) speed = 100;

            var sliderVal = $(this).find(".slider-vertical").slider("value"); //read current value of the slider

            sliderVal += (delta * speed); //increment the current value

            $(this).find(".slider-vertical").slider("value", sliderVal); //and set the new value of the slider

            event.preventDefault(); //stop any default behaviour
        });

    }

}





var progressBar = {
    
    html: '<div class="progress progress-striped active"><div class="progress-bar progress-bar-info"  role="progressbar" style=width:100%></div></div>',
    init: function() {
        $('div.rightContentContainer').last().append(progressBar.html);
        
    },
    destroy: function() {
        $('div.rightContentContainer div.progress').remove();
    }
};

$(document).ready(function () {

    progressBar.init();
    var str = getQSValue('str');
    if (str != '') {
        str = unescape(str);
        var targetEl = $("*:contains('" + str + "')");
        if (targetEl.length == 0) {
            str = str.split(' ').join(String.fromCharCode(160));
            targetEl = $("*:contains('" + str + "')");
            str = str.split(String.fromCharCode(160)).join(' ');
        }
        while (str.length > 11 && targetEl.length == 0) {
            str = str.substr(0, str.length - 1);
            targetEl = $("*:contains('" + str + "')");
            targetEl = targetEl.last();
        }
        targetEl = targetEl.last();
        if (targetEl.length > 0) {
            var anchorName = null;
            var counter = 0;
            while (anchorName == null && counter < 99) {
                counter++;
                while (anchorName == null && targetEl.prev().length > 0) {
                    if (targetEl.find('a[name]').length > 0) {
                        anchorName = targetEl.find('a[name]').last().attr('name');
                        break;
                    }
                    targetEl = targetEl.prev();
                }
                if (anchorName == null)
                    anchorName = targetEl.prevAll('a[name]').first().attr('name');
                targetEl = targetEl.parent();
            }
            if (anchorName != null)
                location.href = '#' + anchorName;
        }
    }
    $("#sdkFrame").load(function () {

        $('div.rightContentContainer').last().append(pager.tempContainer);
        $('div.rightContentContainer').last().append(pager.sdkDiv);

        pager.slicePages();
        loadTOC(); 
        progressBar.destroy();
        wwt.triggerResize();
        $(window).trigger('sdkready');
    });
    $('div.pager-div input').keypress(function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            searchDocs();
        }
    });


});

function searchDocs() { 
    var redir = "/authoring/search.aspx?q=" + $('div.pager-div input').val();
    location.href = redir;
}

var sdkLinkMapper = [
    /*['WorldWideTelescopeDataFilesReference.html',		'Developer.aspx?Page=DataFilesReference'],
    ['WorldWideTelescopeDataToolsGuide.html',			'Developer.aspx?Page=DataToolsGuide'],
    ['WorldWideTelescopeProjectionReference.html',		'Developer.aspx?Page=ProjectionReference'],
    ['WorldWideTelescopeWebControlScriptReference.html','Developer.aspx?Page=WebControlScriptReference'],
    ['worldwidetelescopeplanetarium.html',				'Developer.aspx?Page=WWTPlanetarium'],
    ['worldwidetelescopelocalizationtool.html',			'Developer.aspx?Page=LocalizationTool'],
    ['worldwidetelescopeuserguide.html',				'../help/SupportHelp.aspx?Page=UserGuide'],
    ['wtml/',											'../docs/wtml/']*/
];

var pager = {
    //searchTarget:null,
    frame: null,
    tempContainer: $('<div id="tempContainer"></div>'),
    sdkDiv: $('<div class="sdk-content"></div>'),
    pageCount: 0,
    curPage: 0,
    divs: [],
    anchors: [],
    /*control: $('<div class=pager-div><a title="First Page" class=first-page href="javascript:pager.gotoPage(0)"><< </a>&nbsp;&nbsp;<a title="Previous Page" class=prev-page href="javascript:pager.prev()">< </a><div class=page-num-links></div><a class=next-page title="Next Page" href="javascript:pager.next()"> ></a>&nbsp;&nbsp;<a href="javascript:pager.gotoPage(pager.pageCount-1)" class=last-page title="Last Page">  >></a><div class=right><input class=search value="' + getQSValue('q') + '"/> <a href="javascript:searchDocs()" class=search>Search</a>&nbsp;&nbsp;&nbsp;<a href=javascript:pager.printSDK() class=print><img src="../images/print16.png" />&nbsp;Printable view</a></div></div>'),*/
    control: $('<div class="pager-div"><div class=btn-group><a title="First Page" class="first-page btn btn-info" href="javascript:pager.gotoPage(0)"><i class="fa fa-angle-double-left"></i></a>&nbsp;&nbsp;<a title="Previous Page" class="prev-page btn btn-info" href="javascript:pager.prev()"><i class="fa fa-angle-left"></i><div class=page-num-links></div><a class="next-page btn btn-info" title="Next Page" href="javascript:pager.next()"><i class="fa fa-angle-right"></i></a>&nbsp;&nbsp;<a href="javascript:pager.gotoPage(pager.pageCount-1)" class="last-page btn btn-info" title="Last Page"><i class="fa fa-angle-double-right"></i></a></div><div class=right><input class=search value="' + getQSValue('q') + '"/> &nbsp;&nbsp;&nbsp;<div class=print><i class="fa fa-print"></i>&nbsp;Printable View<br><a href=javascript:pager.printSDK()>This page</a> &nbsp; <a href=javascript:pager.printAll()>All</a></div></div></div>'),
    init: function (opts) {
        pager.options = opts;
    },
    slicePages: function () {
        pager.tempContainer.append(pager.control);
        pager.frame = $('#sdkFrame').contents();
        /*if ($.browser.msie) {
            pager.frame.find('body').contents().filter(function () {
                if (this.nodeType != 1) {
                    console.log('node type: ' + this.nodeType);
                    if (this.nodeType == 8) {
                        $(this).remove();
                        console.log('removed node');
                    }
                }
            });
        }*/

        pager.frame.find('img').first().css('display', 'none');
        pager.makeDiv(pager.frame.find('h1').first().text(), 0);
        seps = pager.frame.find('hr');
        seps.each(function (index, hr) {
            var Heading = $(hr).next();
            if (Heading.prop('tagName') != undefined) {
                while (Heading.prop('tagName').toLowerCase().indexOf('h') != 0) {
                    if (Heading.children().length > 0)
                        Heading = Heading.children().first();
                    else
                        Heading = Heading.next();
                }
                pager.makeDiv(Heading.text(), index + 1);
                //console.log((index + 2) + ': ' + Heading.text());
            }

        });
        // replace image paths
        pager.frame.find('img[src*=uiimages]').each(function (i, img) {
            img = $(img);
            if (img.attr('src').indexOf('/Docs/') == -1)
                img.attr('src', "/Docs/" + img.attr('src'));

        });

        pager.frame.find('img[src*=Images]').each(function (i, img) {
            img = $(img);
            if (img.attr('src').indexOf('/Docs/') == -1)
                img.attr('src', "/Docs/" + img.attr('src'));

        });
        var htmlString = new String();
        htmlString = pager.frame.find('body').html();

        var pagesHtmlArray = htmlString.split('<hr>');
        if (pagesHtmlArray.length == 1)
            pagesHtmlArray = htmlString.split('<HR>');

        pager.sdkDiv.html('');
        pager.sdkDiv.append(pager.control);
        pager.pageCount = pager.divs.length;
        for (var i = 0; i < pager.divs.length; i++) {
            pager.divs[i].html(pagesHtmlArray[i]);
            pager.sdkDiv.append(pager.divs[i]);
            pager.divs[i].find('a[name]').each(function (index, anchor) {
                var a = $(anchor);
                pager.anchors[a.attr('name')] = i;

                if (a.html().length > 0) {
                    a.after(a.html());
                    a.html('');
                }
            });
            var pageLink = $('<a></a>').text(i + 1);
            pager.control.children('div').first().append(pageLink);
            pageLink.attr({
                'href': 'javascript:pager.gotoPage(' + i + ')',
                'class': i < 5 ? 'pager btn btn-info' : 'hidden',
                'page': i,
                'title': pager.divs[i].attr('tooltip')
            });
        }
        var btnContainer = pager.control.find('.btn-group');
        btnContainer.append(btnContainer.find('.next-page, .last-page'));

        var clonedPager = $(pager.control.clone());
        clonedPager.find('div.right').first().css('display', 'none');
        clonedPager.find('a.print').first().css('display', 'none');
        clonedPager.css({ 'top': 22, 'margin-top': 10 });
        pager.sdkDiv.append(clonedPager);
        $('div.pager-div a.first-page').attr('title', pager.divs[0].attr('tooltip'));
        $('div.pager-div a.last-page').attr('title', pager.divs[pager.pageCount - 1].attr('tooltip'));
        if (location.hash.indexOf('#') != -1) {
            var name = location.hash.split('#')[1];
            pager.gotoPage(parseInt(pager.anchors[name]), name);
        }
        else {
            pager.gotoPage(0);
        }
        $('div.pager-div').last().css({
            margin: '30px 0 88px',
            top: 0
        });
    },
    makeDiv: function (id, pageNum) {
        var div = $('<div></div>');
        pager.tempContainer.append(div);
        div.attr({ 'id': id.replace(/ /g, '') + pageNum, 'pageNum': pageNum, 'class': 'sdk-page', 'tooltip': id });
        pager.divs[pageNum] = div;
    },
    gotoPage: function (page, anchorName) {
        setTimeout("pager.unbindPageLinks(" + pager.curPage + ")", 1);
        pager.divs[pager.curPage].css('display', 'none');
        pager.curPage = page;
        pager.divs[pager.curPage].css('display', 'block');
        var textLinks = $('div.pager-div').children('a');
        pager.setPagerControlsEnabled(textLinks.first(), page != 0);
        pager.setPagerControlsEnabled(textLinks.first().next(), page != 0);
        pager.setPagerControlsEnabled(textLinks.last().prev(), page != pager.pageCount - 1);
        pager.setPagerControlsEnabled(textLinks.last().prev().prev(), page != pager.pageCount - 1);
        
        var pagerOffset = 0;
        var startPage = page - 4;
        if (startPage < 0) {
            pagerOffset = Math.abs(startPage);
            startPage = 0;
        }
        var endPage = page + 4 + pagerOffset;
        if (endPage > pager.pageCount - 1) {
            startPage = Math.max(0, startPage - (endPage - pager.pageCount) - 1);
            endPage = pager.pageCount - 1;
        }

        $('div.pager-div a[page]').hide();
        for (var i = startPage; i <= endPage; i++) {
            $('div.pager-div a[page=' + i + ']')
                .addClass('pager btn btn-info')
                .removeClass('active')
                .removeClass('hidden')
                .show();
            //console.log(i);
        }
        
        $('div.pager-div a[page=' + page + ']').addClass('active');

        $('div.pager-div a.next-page').attr('title', page + 1 != pager.pageCount ? pager.divs[page + 1].attr('tooltip') : '');
        $('div.pager-div a.prev-page').attr('title', page != 0 ? pager.divs[page - 1].attr('tooltip') : '');
        $('table.WT1').each(function(i, tbl) {
            if ($(tbl).find('tr').length > 1) {
                $(tbl).addClass('table');
            }
        });
        pager.divs[pager.curPage].find('img').each(function (index, image) {
            var img = $(image);
            if (img.attr('crawled') == '1') return;
            img.attr('crawled', '1');
            var maxW = $('.sdk-page').width() * .85;
            var iw = img.innerWidth();
            if (iw > maxW) {
                var ih = img.innerHeight();
                var ratio = ih / iw;
                img.css({
                    'width': maxW, 'height': maxW * ratio,
                    'border': 'solid 1px blue', 'cursor': 'pointer'
                });
                img.attr({ 'origW': iw, 'origH': ih, 'title': 'Click to view full-size image...', 'alt': 'Click to view full-size image...' });
                img.click(wwt.viewMaster.fullScreenImage
                    /*function (event) {
                    var image = $(event.target);
                    var h = parseInt(image.attr('origH'));
                    var w = parseInt(image.attr('origW'));
                    var win = window.open('/authoring/ImgViewer.htm?h=' + h +
                        '&w=' + w + '&src='+ image.attr('src'), 'imgWin',
                        'height=' + h + ',width=' + w + ',status=no,toolbar=no,menubar=no,location=no,scrollbars=no'
                    );
                    win.resizeTo(w, h);
                    win.focus();
                }*/);
            }
        });

        if (anchorName != null)
            location.href = '#' + anchorName;
        $('div.content-main').css('height', Math.max(pager.sdkDiv.height() + 55, 400));
        setTimeout(pager.bindPageLinks, 2);
        document.body.scrollTop = 0;
        wwt.triggerResize();
    },
    next: function () {
        var page = Math.min(pager.curPage + 1, pager.pageCount - 1);
        pager.gotoPage(page);
    },
    prev: function () {
        var page = Math.max(pager.curPage - 1, 0);
        pager.gotoPage(page);
    },
    clickAnchor: function (event) {
        var anchor = $(event.target);
        if (anchor.prop('tagName').toLowerCase() != 'a')
            anchor = anchor.parent('a').first();
        var name = anchor.attr('href').split('#')[1];
        if (isNaN(parseInt(pager.anchors[name]))) {
            console.log('Err! ' + name);
            return;
        }
        
        pager.gotoPage(parseInt(pager.anchors[name]), name);
        event.stopPropagation();
    },
    bindPageLinks: function () {
        pager.divs[pager.curPage].find("a[href*='#']").each(function (i, anchor) {
            anchor = $(anchor);
            var loc = location.href.split('#')[0];
            var link = anchor.attr('href').split('#');
            if (link[0] == '' || link[0] == loc) {
                anchor.bind('click', pager.clickAnchor);
            }

        });
        pager.divs[pager.curPage].find("a[href*='.html']").each(function (i, anchor) {
            anchor = $(anchor);
            for (var i = 0; i < sdkLinkMapper.length; i++) {
                anchor.attr('href', anchor.attr('href').replace(new RegExp(sdkLinkMapper[i][0], 'i'), sdkLinkMapper[i][1]));
            }
        });
    },
    unbindPageLinks: function (page) {
        pager.divs[page].find("a[href*='#']").each(function (i, anchor) {
            var loc = location.href.split('#')[0];
            var link = $(anchor).attr('href').split('#');
            if (link[0] == '' || link[0] == loc) {
                $(anchor).unbind('click');
            }
        });
    },
    setPagerControlsEnabled: function (link, enabled) {
        link.attr('disabled', enabled ? '' : 'disabled');
        if (enabled)
            link.removeClass('disabled');
        else
            link.addClass('disabled');
        if (link.attr('hasDisabledListener') != '1') {
            link.attr('hasDisabledListener', '1');
            link.click(function (event) {
                if (link.attr('disabled') == 'disabled')
                    event.stopPropagation();
            });
        }
    },
    printSDK: function () {
        var win = window.open('/content/print.htm', 'printWin', 'height=600,width=800,status=no,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes');
    },
    printAll: function () {
        var win = window.open($('iframe').attr('src'), 'printWin', 'height=600,width=800,status=no,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes');
    },
    setPrintContent: function () {
        return pager.divs[pager.curPage].html();
    },
    search: function (searchText) {
        var results = "top 10 results for : <strong>" + this.stripExtraChars(searchText) + "</strong>";
        var found = false;

        //create array to store hit count for search weight
        var hitCount = new Array();
        for (var i = 0; i < pager.divs.length; i++) {
            hitCount[i] = new SearchHits(i, 0, pager.divs[i].attr('id').substr(0, pager.divs[i].attr('id').lastIndexOf(i)));
        }

        // do a separate search for each word provided
        var words = new Array();
        var firstQuote = searchText.indexOf('"');
        if (firstQuote >= 0) {
            var lastQuote = searchText.lastIndexOf('"');
            if (lastQuote != firstQuote)
                words[0] = searchText.substr(firstQuote + 1, lastQuote - firstQuote - 1);
            else
                words[0] = searchText.substr(firstQuote + 1);
        } else {
            words = searchText.split(' ');
        }

        for (var word = 0; word < words.length; word++) {
            for (var i = 0; i < pager.divs.length; i++) {
                var patt = new RegExp(words[word], "gi");
                var find = pager.divs[i].html().match(patt);

                if (find != null) {
                    hitCount[i].count += find.length;
                } else { // do this so only results with all words are returned
                    hitCount[i].count = 0;
                }
            }
        }

        for (var x = 0; x < hitCount.length; x++) {
            if (hitCount[x].count > 0) found = true;
        }

        //process the results
        if (!found) {
            results += "<br />match not found";
        } else {
            hitCount.sortByProp('count');

            for (var i = hitCount.length - 1; i > hitCount.length - 11; i--) {
                if (hitCount[i].count > 0) {
                    //results += "<br />" + hitCount[i].anchor + " found " + hitCount[i].count + " hits on page: " + hitCount[i].page;
                    results += "<br /><a href='javascript:pager.gotoPage(" + hitCount[i].page + ");' >" + hitCount[i].anchor + "</a>";
                }
            }
        }

        return results;
    },

    stripExtraChars: function (text) {

        var stripped = text.replace(/[^a-zA-Z 0-9]+/g,'');

        return stripped;

    }
};
    function loadTOC() {

        //copy toc from html doc to scrollable toc div
        $(".toc").appendTo('div.tocMenu');

        //bind the pager method to each anchor
        $("div.tocMenu a").each(function (idx, a) {
            anchor = $(a);
            anchor.bind('click', pager.clickAnchor);
        });

        initSlider('div.tocMenu', '.tocLink');

    };

    function SearchHits(page, count, anchor) {
        this.page = page;
        this.count = count;
        this.anchor = anchor;
    }

//    SearchHits.prototype.toString = function () {
//        return this.count.toString();
//    }

    Array.prototype.sortByProp = function (p) {
        return this.sort(function (a, b) {
            return (a[p] > b[p]) ? 1 : (a[p] < b[p]) ? -1 : 0;
        });
    } 




