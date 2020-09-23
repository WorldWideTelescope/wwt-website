

wwt.WebControl = (function () {

	//#region global variables
	var ctl, // wwt html5 control (external)

		/** controls **/
		canvas, // $('#WWTCanvas')
		container, //$('#WorldWideTelescopeControlHost')
		btnFS, //fullscreen button
		btnPlay, //play button
		btnOpenWWT,
		btnFGInfo,
		tweenTimer,
		uiControls, //container.find('.player-controls')
		crossFader,
		parentDiv,
		zoomIn,
		zoomOut,

		/*** state flags ***/
		playing,
		fullscreen,
		tourLoaded = false,
		isChangingFs = false,
		loadTimer,
		defaultTour,
		defaultSurvey = 'Digitized Sky Survey (Color)',

		msie = navigator.userAgent.indexOf('Trident') !== -1 || navigator.userAgent.indexOf('MSIE') !== -1,
		debug = false,
		resLoc, // '<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>'
		remote = false,
		resizeCallback,
		initOptions;
	//#endregion

	//#region initialization
	function init(opts) {
		if (opts) {
			initOptions = opts;
			resLoc = opts.resLoc;
			defaultTour = opts.defaultTour;
			
			if (opts.debug) {
				debug = true;
			}
			if (opts.remote) {
				remote = opts.remote;
				
				
			}
			if (!opts.hideUI) {
				opts.hideUI = false;
			}
		}
		if (window.Type && Type.canCast) {
		    window.ss = { canCast: Type.canCast };
		}
		initElements();
		bindEvents();
		resize();
		resizeCallback = opts.onresize;
		if (opts.remote) {
			window.addEventListener("message", function (event) {
				if (event.origin === remote.hostWindow) {
					if (event.data === 'fsenter') {
						showBottomBar();
						btnFS.addClass('hover');
					}
					if (event.data === 'fsleave') {
						btnFS.removeClass('hover');
					}
					if (event.data === 'istouch') {
						wwt.isTouch = true;
					}
					if (event.data.indexOf('import::') === 0) {
						loadExternalImage(null, event.data.replace(/import::/g,''));
					}
					if (event.data.indexOf('set::') === 0) {
						var pair = event.data.replace(/set::/g, '').split('=');
						var setting = pair[0], value = pair[1] === 'true';
						setSetting(setting, value);

					}
				    if (event.data === 'getCoords') {
				        top.postMessage({
				            coords: {
				                ra: ctl.getRA(),
				                dec: ctl.getDec(),
				                fov: ctl.get_fov()
				            }
				    }, remote.hostWindow);
				    }
				}
			}, false);
			showBottomBar(false);
			
		}
	}

	var setSetting = function(setting, value) {
		if (setting === 'crosshairs') {
			ctl.settings.set_showCrosshairs(value);
		}
		if (setting === 'ecliptic') {
			ctl.settings.set_showEcliptic(value);
		}
		if (setting === 'figures') {
			ctl.settings.set_showConstellationFigures(value);
		}
		if (setting === 'boundaries') {
			ctl.settings.set_showConstellationBoundries(value);
		}
		if (setting === 'galacticPlaneMode') {
			ctl.settings.set_galacticMode(value);
		}
		if (setting === 'eclipticGrid') {
			ctl.settings.set_showEclipticGrid(value);
			ctl.settings.set_showEclipticGridText(value);
		}
		if (setting === 'altAzGrid') {
			ctl.settings.set_showAltAzGrid(value);
			ctl.settings.set_showAltAzGridText(value);
		}
		if (setting === 'galacticGrid') {
			ctl.settings.set_showGalacticGrid(value);
			ctl.settings.set_showGalacticGridText(value);
		}
		if (setting === 'equatorialGrid') {
			ctl.settings.set_showGrid(value);
			ctl.settings.set_showEquatorialGridText(value);
		}
		if (setting === 'pictures') {
			ctl.settings.set_showConstellationPictures(value);
		}
	}

	var reinit = function () {
		setDefaultUI();
		ctl.add_ready(function() {});
		ctl.loadImageCollection(wwt.resLoc + "/Content/WTML/WMAP.xml");
		//ctl.stopTour();
		ctl.setForegroundImageByName(defaultSurvey);
		ctl.endInit();
	};

	var setDefaultUI = function () {
		if (initOptions.webGl) {
			ctl = wwtlib.WWTControl.initControlParam("WWTCanvas", true);
		} else {
			ctl = wwtlib.WWTControl.initControlParam("WWTCanvas", true);
		}
		ctl.settings.set_showConstellationBoundries(false);
		if (remote && remote.galacticMode) {
			ctl.add_collectionLoaded(function() {
				ctl.setForegroundImageByName('Glimpse 360');
				ctl.settings.set_galacticMode(true);
				initXFader();
				uiControls.find('.btn').first().remove();
				uiControls.find('.btn').first().remove();
			});
			ctl.loadImageCollection("//worldwidetelescope.org/data/glimpse360.wtml");
		} else if (remote && remote.importImage) {
			loadExternalImage(null, remote.importImage);
		}
		else if (remote && remote.wtml) {
			//var lastPlaceIndex = wwtlib.WWTControl.imageSets.length;
			ctl.add_collectionLoaded(function () {
				var pl = wwtlib.WWTControl.imageSets[wwtlib.WWTControl.imageSets.length-1];
				if (ss.canCast(pl, wwtlib.Place)) {
					wwtlib.WWTControl.singleton.gotoTarget(pl, false, false, false);
				}
				if (ss.canCast(pl, wwtlib.Imageset)) {
					wwtlib.WWTControl.singleton.renderContext.set_backgroundImageset(pl);
				}
			});
			ctl.loadImageCollection(remote.wtml);
		} else {
			ctl.gotoRaDecZoom(17.7 * 15, -28, 60, true);
		}
		if (remote && remote.settings) {
			var settings = remote.settings.split(',');
			$.each(settings, function () {
				var settingSplit = this.split('=');
				var setting = settingSplit[0];
				var value = settingSplit[1] == 'true';
				setSetting(setting, value);
			});
		}
	};

	var initElements = function () {
		if ('ontouchstart' in window || window.navigator.msPointerEnabled) {
			wwt.isTouch = true;
		}
		parentDiv = $('<div class="wwt-webcontrol-wrapper"></div>');
		container = $('#WorldWideTelescopeControlHost')
			.attr({
				'data-html': true,
				'data-placement': 'bottom',
				'data-content': '',
				'data-animation': true
			})
		.css('position', 'relative')
		.append('<div id="WWTCanvas"></div>')
		.after(parentDiv);
		parentDiv.append(container);

		setDefaultUI();
		
		canvas = container.find('#WWTCanvas');
		
		uiControls = $('<div class="player-controls">' +
			'<a id="wwtLogo" title="WorldWide Telescope Home Page" href="http://worldwidetelescope.org" class=pull-left target="wwt">' +
			'<img src="//wwtweb.blob.core.windows.net/images/lens-logo-sm.png" alt="WorldWideTelescope Logo" style=max-height:32px /></a> ' +
			'<a class="btn play btn-sm"><i class="fa fa-play"></i></a> ' +
			'<a href="javascript:void(0)" class="btn fs btn-sm" title="View Full Screen"><i class="fa fa-arrows-alt"></i></a></div>');

		

		if (!remote) {
			uiControls.find('a.pull-left').remove();
		}

		container.append(uiControls);
		btnFS = uiControls.find('.btn.fs').toggle(!initOptions.hideUI);
		btnPlay = uiControls.find('.btn.play').toggle(!initOptions.hideUI);
		if (!defaultTour) {
			btnPlay.remove();
		}
		zoomIn = $('<a class="btn btn-sm zoom" id="zoomIn" title="Zoom In"><i class="fa fa-search-plus"></i></a>').toggle(!initOptions.hideUI);
		zoomOut = $('<a class="btn btn-sm zoom" id="zoomOut" title="Zoom Out"><i class="fa fa-search-minus"></i></a>').toggle(!initOptions.hideUI);
		container.append(zoomIn);
		container.append(zoomOut);
		ctl.endInit();
	};

	var bindEvents = function() {
		btnFS.on('click', function() {
			if (fullscreen) {
				if (msie) {
					fullScreenMode();
				} else {
					exitFullScreen();
				}
			} else {
				fullScreenMode();
			}
		});
		$(window).on('resize', resize);
		
		zoomIn.on('click', function () {
			ctl.zoom(.66666666666667);
		});
		zoomOut.on('click', function () {
			ctl.zoom(1.5);
		});
		ctl.add_tourReady(function(a, b) {
			clearTimeout(loadTimer);
			tourLoaded = true;
		});
		ctl.add_tourEnded(tourComplete);
		$(document).on('fullscreenchange msfullscreenchange mozfullscreenchange webkitfullscreenchange', fullScreenChange);

		btnPlay.on('click', playTour);
		container.on('wheel scroll mousewheel', zoomCanvas);

		
		
		if (debug) {
			var pinchDiv = $('<div class=pincher></div>').css('background-color','rgba(0,0,0,.01)');
			canvas.append(pinchDiv);
			wwt.pincher.init(pinchDiv, function (pct) {
				ctl.zoom(pct);
				//$('textarea').val('zoom: ' + pct + '\n' + $('textarea').val());
			});
			zoomIn.show();
			zoomOut.show();
			btnFS.show();
			
		}
		container.on('mouseenter mousemove', showBottomBar);
	};

	var initXFader = function () {
		if (crossFader) {
			return;
		}
		crossFader = $('<div class="x-fader-wrapper">' +
					'<div class="cross-fader">' +
					'<a class="slider-bar" title="Cross-fade the foreground image...">&nbsp;</a>' +
					'</div></div>');
		btnOpenWWT = $('<a class="btn" href="javascript:void(0)" title="Open in WorldWide Telescope">' +
		'<i class="fa fa-external-link"></i></a>');
		btnFGInfo = $('<a class=btn href="javascript:void(0)" title="Imagery Info/Credits">' +
		'<i class="fa fa-info-circle"></i></a>').on('click', function() {
			bootbox.dialog({
				message: $(this).data('Description'),
				title:$(this).data('Title')
			});
		});
		btnFS.before(crossFader);
		crossFader.before(btnOpenWWT);
		btnOpenWWT.before(btnFGInfo);
		var bar = crossFader.find('a.slider-bar');
		var xf = new wwt.Move({
			el: bar,
			bounds: {
				x: [-100, 0],
				y: [0, 0]
			},
			onstart: function() {
				bar.addClass('moving');
			},
			onmove: function () {
				ctl.setForegroundOpacity(this.css.left);
			},
			oncomplete: function() {
				bar.removeClass('moving');
			}
		});
	};


	//#endregion

	//#region scaling/resizing functions

	var requestFullScreen = function(element) {
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

	var exitFullScreen = function() {
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
		} /*else {
			fullScreenMode();
		}*/
		if (resizeCallback) {
			setTimeout(function() {
				resizeCallback({ fullscreen: false });
			}, 888);

		}
	};

	var fullScreenMode = function() {
		isChangingFs = true;
		setTimeout(function () { isChangingFs = false; }, 888);
		if (remote) {
				
			top.postMessage('fullscreen', remote.hostWindow);
			return;
		}
		if (!fullscreen) {
			
			requestFullScreen(document.body);
			
			$('body').css({ overflow: 'hidden',marginTop:0 }).append(container);

			container.css({
				//height: $(window).height(),
				//width: $(window).width(),
				left: 0,
				right: 0,
				bottom: -1,
				top: 0,
				margin: 0,
				position:'fixed'
		});
			container.addClass('fullscreen');
			$('.navbar, .footer').hide();

			btnFS.attr('title', 'View normal size')
				.find('i')
				.removeClass('fa-arrows-alt')
				.addClass('fa-compress');
			fullscreen = true;
			resize();
		} else {
			exitFullScreen(document.body);
			parentDiv.append(container);
			$('body').css({ overflow: 'hidden', marginTop: 44 });
			container.css({
				position: 'relative',
				height: 'auto',
				width: 'auto',
				margin: '8px 0 16px'
			});
			container.removeClass('fullscreen');
			btnFS.attr('title', 'View full screen')
				.find('i')
				.removeClass('fa-compress')
				.addClass('fa-arrows-alt');
			$('.navbar, .footer').show();
			fullscreen = false;
			
			resize();
		}
		ctl.set_showCaptions(fullscreen);
	};
	
	var resize = function() {
		if (fullscreen) {
			/*container.css({
				height: $(window).height(),
				width: $(window).width()
			});*/
			container.css({
				//height: $(window).height(),
				//width: $(window).width(),
				left: 0,
				right: 0,
				bottom: -1,
				top: 0,
				margin: 0,
				position: 'fixed'
			});
			
		}
		if (resizeCallback) {
			setTimeout(function () {
				resizeCallback({ fullscreen: fullscreen });
			}, 888);
		}
		canvas.width(container.width());
		canvas.height(fullscreen || remote ? (container.height()) : container.width() * .667);
		canvas.find('canvas').height(canvas.height()).width(canvas.width());
		if (!remote) {
			wwt.triggerResize();
		}
	};

	// handler for when user changes this state
	var fullScreenChange = function() {
		if (!isChangingFs) {
			fullScreenMode();
			isChangingFs = true;
			setTimeout(function() { isChangingFs = false; }, 2500);
		}
	};

	//#endregion

	//#region toolbars
	var hideBottomBar = function () {
		if (wwt.isTouch) {
			return;
		}
		uiControls.removeClass('slide-up').addClass('slide-down');
		$('.zoom').addClass('fade-out').removeClass('fade-in');

	};

	var showBottomBar = function (arg) {

		uiControls.removeClass('slide-down').addClass('slide-up');
		$('.zoom').addClass('fade-in').removeClass('fade-out');
		if (wwt.isTouch) {
			return;
		}
		clearTimeout(tweenTimer);
		if (typeof arg === 'boolean' && arg === false) {
			return;
		}
		
		tweenTimer = setTimeout(hideBottomBar, 3333);
	};
	//#endregion

	//#region utility functions
	var playTour = function (tour) {

		if (!tour || typeof tour !== 'string') {
			tour = defaultTour;
		}
		btnPlay.removeClass('hide').show();
		$('#spectrumInfo').hide();
		var attempts = 1;
		var tryLoadTour = function () {
			attempts++;
			
			var tourPath = tour.indexOf('http') != -1 ? tour :
				location.host.indexOf('thewebkid') != -1 || location.host.indexOf('localhost') != -1 ?
				'http://thewebkid.com' + tour : resLoc + tour;
			ctl.loadTour(tourPath);
			if (!tourLoaded) {
				
				if (attempts > 5) {
					clearTimeout(loadTimer);
					return;
				}
				loadTimer = setTimeout(tryLoadTour, 1234);
			}
		};
		if (playing) {
			wwtlib.WWTControl.singleton.pauseCurrentTour();
			//wwtlib.WWTControl.singleton.stopCurrentTour();
			//ctl.clearAnnotations();
			//ctl.gotoRaDecZoom(0, 0, 60, true);
			//ctl.setForegroundImageByName(defaultSurvey);
			//tourComplete();
			btnPlay.find('i').removeClass('fa-stop').addClass('fa-play').show();
			playing = false;
		} else {
			if (tourLoaded) {
				ctl.playTour();
			} else {
				tryLoadTour();
			}
			btnPlay.find('i').removeClass('fa-play').addClass('fa-stop').show();
			playing = true;
		}


		ctl.add_slideChanged(function(sender, text) {
			clearTimeout(loadTimer);
			tourLoaded = true;
			var txt = text.get_caption();
			//console.log(txt);
			container.attr('data-content', txt);
			if (txt == '' || fullscreen) {
				container.popover('hide');
			} else {
				container.popover('show');
			}
		});
		ctl.set_showCaptions(fullscreen ? true : false);
		btnFS.show();
		zoomIn.toggle(!playing);
		zoomOut.toggle(!playing);
		wwt.triggerResize();
	};

	var interact = function () {
		btnPlay.hide();
		btnFS.show();
		uiControls.show();
		zoomIn.show();
		zoomOut.show();
	};

	var loadExternalImage = function (manualData, imageUrl, callback) {
		var url = imageUrl;
		var encodedUrl = '//worldwidetelescope.org/WWTWeb/TileImage.aspx?imageurl=' + encodeURIComponent(url);
		if (manualData && typeof manualData === "string") {
			encodedUrl += manualData;
		}
		
		var wtmlLoaded = function (xml) {
			var wtml = $(xml);
			var place = parseWtml(wtml)[0];
			var imageSet = place.get_studyImageset();
			if (place.get_RA() == 0 && place.get_dec() === 0 && imageSet.get_rotation() == 0) {
				if (callback) {
					callback({
						success: false, place: place, imageSet: imageSet, wtml: wtml
					});
				}
				return;
			} else {
				wwtlib.WWTControl.singleton.renderContext.set_foregroundImageset(place.get_studyImageset());
				wwtlib.WWTControl.singleton.gotoTarget(place, false, false, true);
				if (!crossFader) {
					initXFader();
				}
				btnFGInfo.hide();
				
				btnOpenWWT.show().attr('href', encodedUrl);
				
				showBottomBar(false);
				if (callback) {
					callback({ success: true });
				}
			}
		};

		function getImgWtml() {
			$.ajax({
				url: encodedUrl,
				crossDomain: true,
				dataType: 'xml'
			}).done(wtmlLoaded).fail(ajaxError);
		}

		var errCount = 0;

		function ajaxError(xhr, status, er) {

			if (er === 'Internal Server Error') {
				errCount++;
				if (errCount < 10) {
					getImgWtml();
				}
			} else {
				try {
					console.log(er, status, xhr);
					wtmlLoaded($.parseXML(xhr.responseText.replace(/& /g, '&amp; ')));
				} catch (er) {
					errCount++;
					if (errCount < 10) {
						getImgWtml();
					} else {
						return;
					}
				}
			}
		}

		getImgWtml();
	};

	var parseWtml = function(wtml) {

		ctl = wwt.WebControl.ctl();
		var places = [];
		var constellationInstance;

		function findConstellation(ra, dec) {
			if (!constellationInstance) {
				constellationInstance = new wwtlib.Constellations();
			}
			return constellationInstance.findConstellationForPoint(ra, dec);
		}

		var imageSetTypes = [];

		function getImageSetType(sType) {
			if (!imageSetTypes.length) {
				$.each(wwtlib.ImageSetType, function(k, v) {
					if (!isNaN(v)) {
						imageSetTypes[v] = k.toLowerCase();
					}
				});
			}
			return imageSetTypes.indexOf(sType.toLowerCase()) == -1 ? 2 : imageSetTypes.indexOf(sType.toLowerCase());

		}

		var isId = 100;
		var createPlace = function(node) {
			var place = null, fgi, constellation;
			var createImageset = function() {
				isId++;
				return wwtlib.Imageset.create(
					fgi.attr('Name'),
					fgi.attr('Url'),
					getImageSetType(fgi.attr('DataSetType')),
					fgi.attr('BandPass'),
					wwtlib.ProjectionType[fgi.attr('Projection').toLowerCase()],
					isId, //imagesetid
					parseInt(fgi.attr('BaseTileLevel')),
					parseInt(fgi.attr('TileLevels')),
					null, //tilesize
					parseFloat(fgi.attr('BaseDegreesPerTile')),
					fgi.attr('FileType'),
					fgi.attr('BottomsUp') === 'True',
					'', //quadTreeTileMap 
					parseFloat(fgi.attr('CenterX')),
					parseFloat(fgi.attr('CenterY')),
					parseFloat(fgi.attr('Rotation')),
					true, //sparse
					fgi.find('ThumbnailUrl').text(), //thumbnailUrl,
					false, //defaultSet,
					false, //elevationModel
					parseFloat(fgi.attr('WidthFactor')), //widthFactor,
					parseFloat(fgi.attr('OffsetX')),
					parseFloat(fgi.attr('OffsetY')),
					fgi.find('Credits').text(),
					fgi.find('CreditsUrl').text(),
					'', '',
					0, //meanRadius
					null);
			};
			if (node.tagName === 'ImageSet') {
				fgi = $(node);
				place = wwtlib.Place.create(fgi.attr('Name'), 0, 0, 'Sky', null, fgi ? getImageSetType(fgi.attr('DataSetType')) : 2, 360);
				place.set_studyImageset(createImageset());
			} else if (node.tagName === 'Place') {
				var plNode = $(node);
				var ra = parseFloat(plNode.attr('RA')), dec = parseFloat(plNode.attr('Dec'));
				constellation = findConstellation(ra, dec);

				fgi = plNode.find('ImageSet').length ? plNode.find('ImageSet') : null;
				place = wwtlib.Place.create(
					plNode.attr('Name'),
					dec,
					ra,
					plNode.attr('DataSetType'),
					constellation,
					fgi ? getImageSetType(fgi.attr('DataSetType')) : 2, //type
					parseFloat(plNode.find('ZoomLevel')) //zoomfactor
				);
				if (fgi != null) {
					place.set_studyImageset(createImageset());
				}

			}
			return place;
		};


		wtml.find('Folder').children().each(function(i, childnode) {
			var wwtPlace = createPlace(childnode);
			if (wwtPlace) {
				places.push(wwtPlace);
			}

		});
		return places;
	};

	var setForegroundImage = function (args) {
		//console.log('setFGImg' + args);
		ctl.setForegroundImageByName(args.imgName);
		ctl.gotoRaDecZoom(args.RA, args.Dec, args.Scale);
		
		if (!crossFader) {
			initXFader();
		}
		if (args.Description) {
			btnFGInfo.show().data({
				Description: args.Description,
				Title: args.Title
			});
		} else {
			btnFGInfo.hide();
		}
		if (args.Link) {
			btnOpenWWT.attr('href', args.Link);
		} else {
			btnOpenWWT.hide();
		}
		showBottomBar(false);
	};

	var tourComplete = function () {
		if (fullscreen) {
			if (msie) {
				fullScreenMode();
			} else {
				exitFullScreen();
			}
			setTimeout(tourComplete, 111);
			return;
		}
		
		btnPlay.find('i').removeClass('fa-stop').addClass('fa-play');
		playing = false;
		
		zoomIn.show();
		zoomOut.show();
		reinit();
		
		wwt.triggerResize();
		if (initOptions.ontourcomplete) {
			initOptions.ontourcomplete();
		}

	};

	//#endregion

	var zoomCanvas = function(event) {

		var wheelDelta = (event.originalEvent.deltaY);
		if (wheelDelta) {
			event.stopImmediatePropagation();
			event.preventDefault();
			ctl.zoom(wheelDelta > 0 ? 1.25 : .8);
		}

	};

	var getCoords = function() {
		return {
			ra: ctl.getRA(),
			dec: ctl.getDec(),
			fov: ctl.get_fov()
		}
	};

	return {
		init: init,
		getCoords: getCoords,
		ctl: function() {
			return ctl;
		},
		resize:resize,
		playTour: playTour,
		reinit: reinit,
		interact: interact,
		setForegroundImage: setForegroundImage,
		loadExternalImage: loadExternalImage,
        parseWtml:parseWtml
	};

})();

