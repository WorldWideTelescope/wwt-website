wwt.Spectroscopy = (function() {
	var api = {
		init:init
	};
	var ctl,
		surveys,
		slider, // full slider container $('.spectrum-slider')
		sliderbar, // the slidable bar slider.find('.slider-bar');
		sliderMover, // the wwt.move() module/behavior instance
		canvasOverlay, // the previews over the canvas
		defaultTour,
		defaultBand,
		defaultSurvey, //'Digitized Sky Survey (Color)'
		canvas,
		/*** track the index (within the surveys collection) of the currently used imagery ***/
		bgInd = -1,
		sliderCurLeft,
		fgOpacity,
		bgOpacity,
		tourMode;
			


	//#region initialization
	function init(opts) {
		surveys = opts.surveys;
		defaultTour = opts.defaultTour;
		defaultSurvey = 'Digitized Sky Survey (Color)';
		delete opts.surveys;
		opts.onresize = resizeHandler;
		opts.hideUI = true;
		opts.ontourcomplete = interactionMode;
		wwt.WebControl.init(opts);
		ctl = wwt.WebControl.ctl();
		initElements();
		bindEvents();
		cacheBackgrounds();
	}

	var initElements = function() {
		slider = $('.spectrum-slider');
		sliderbar = slider.find('.slider-bar');
		canvasOverlay = $('.canvas-overlay').show();
		canvas = $('#WWTCanvas');
		fgOpacity = $('#fgOpacity');
		bgOpacity = $('#bgOpacity');
		sliderMover = new wwt.Move({
			el: sliderbar,
			bounds: {
				x: [0, slider.width()],
				y: [0, 0]
			},
			onmove: sliderMove
		});
		
		ctl.add_ready(cacheBackgrounds);
		var modal = $('#imageryDetailModal');

		$.each(surveys, function (i, item) {
			var pnl = modal.find('.panel-default').first().clone(true);
			pnl.find('.smdl-spectrum-title')
				.attr('href', '#smdl' + item.ref)
				.text(item.name);
			pnl.find('.smdl-spectrum-id')
				.attr('id', 'smdl' + item.ref)
				.find('.smdl-spectrum-creditlink')
				.attr('href', item.creditLink)
				.text(item.creditLink);
			pnl.find('.smdl-spectrum-credit').text(item.credit);
			pnl.find('.smdl-desc-div')
				.toggle(item.description.length > 0)
				.find('p').text(item.description);
			modal.find('.modal-body').append(pnl);
		});
		modal.find('.panel-default').first().remove();
	};
	var bindEvents = function () {
		canvasOverlay.find('a').on('click', startMode);
		$('#btnSpectroscopyOverview').on('click', function() {
			toggleSpectrumChrome(false);
			tourMode = true;
			wwt.WebControl.playTour();
			
		});
	};

	var toggleSpectrumChrome = function(show) {
		$('#spectrumInfo, .spectrum-slider').removeClass('hide').toggle(show);
		$('.tour-trouble').removeClass('hide').toggle(!show);
		wwt.triggerResize();
	};

	var cacheBackgrounds = function () {

		ctl.add_collectionLoaded(function () {
			ctl.setForegroundImageByName(defaultSurvey);
			$.each(surveys, function (i, item) {
				setTimeout(function () {
					ctl.setBackgroundImageByName(item.name);
				}, i * 100);
				setTimeout(function () {
					ctl.setBackgroundImageByName(defaultSurvey);

				}, i * 111);
			});
		});
		ctl.loadImageCollection('https://wwtweb.blob.core.windows.net/wtml/WMAP.xml');
	};
	//#endregion

	//#region slider functions
	var moveTimer;
	var setOpacity = function() {
		var wid = slider.width();
		var w = wid / (surveys.length - 1);
		var left = sliderCurLeft = this.css.left;

		var bgIndex = Math.floor(left / w);
		var fgIndex = Math.ceil(left / w);
		var fgTrans = (left % w) / w;
		var opacity = (fgTrans * 100).toFixed(1);
	   
		$('#opacitySpan').text(opacity);

		fgOpacity.width((opacity * 2) + 10);
		bgOpacity.width(((100 - opacity) * 2) + 10);
		if (bgInd != bgIndex) {
			ctl.setForegroundImageByName(surveys[fgIndex].name);
			ctl.setBackgroundImageByName(surveys[bgIndex].name);
			$('#surveyBg').text(surveys[bgIndex].name);
			$('#surveyFg').text(surveys[fgIndex].name);
			bgInd = bgIndex;
		}
		ctl.setForegroundOpacity(opacity);
	}
	var sliderMove = function () {
		clearTimeout(moveTimer);
		var left = this.css.left
		moveTimer = setTimeout(function() {
			setOpacity.call({ css: { left: left } });
		}, 400);
	};

	var setupSlider = function (oldWidth, newWidth) {
		if (!surveys) {
			return;
		}
		var pct = false;
		if (oldWidth && newWidth) {

			pct = sliderCurLeft / oldWidth;
			sliderbar.remove();
			sliderbar = $('<a class="slider-bar btn btn-small btn-info">&nbsp;</a>');
			slider.append(sliderbar);
			sliderMover = null;
			slider.width(newWidth);
		}
		sliderMover = new wwt.Move({
			el: sliderbar,
			bounds: {
				x: [0, $('#WWTCanvas').width()],
				y: [0, 0]
			},
			onmove: sliderMove
		});
		var bandContainer = slider.find('.band-container');
		slider.find('.band-marker, .band').remove();
		slider.removeClass('hide').show();
		var w = slider.width() / (surveys.length - 1);
		var h = slider.height();
		$.each(surveys, function (i, item) {
			var marker = $('<div class=band-marker></div>');
			var left = i == 0 ? 0 : (i - 1) * w,
				wid = i == 0 ? 0 : w;
			marker.css({
				left: left,
				width: wid
			});
			if (wid == 0) marker.hide();
			slider.append(marker);
			var colorBit = Math.round(255 / surveys.length) * (i + 1);
			var trig = wwt.getAngle({ x: 0, y: h }, { x: w, y: 0 }, true);
			var flyout = $('<span></span>');
			
			if (item.description.length) {
				flyout.append($('<p></p>').text(item.description));
			}
			var band = $('<a class=band href=javascript:void(0)></a>').css({
				width: trig.dist * 2,
				left: ((i - 1) * w),
				background: 'rgba(' + colorBit + ',' + colorBit + ',' + colorBit + ',.7)',
				borderBottom: 'solid 1px rgb(' + colorBit + ',' + colorBit + ',' + colorBit + ')',
				borderTop: 'solid 1px rgb(' + colorBit + ',' + colorBit + ',' + colorBit + ')',
				transform: 'rotate(' + (360 - trig.deg) + 'deg)'
			}).attr({
				'data-title': item.name,
				'data-html': true,
				'data-placement': 'top',
				'data-content': '<div style=text-align:left>' + flyout.html() + '</div>',
				'data-animation': true,
				'data-trigger': 'hover',
				'data-container': 'body'
			});

			bandContainer.append(band);
			band.on('mouseenter', function () {
				band.popover('show');
			}).on('click', function () {

				moveSlider(i === 0 ? 0 : left + w);
			});
			if (!oldWidth && item.name === defaultSurvey) {
				defaultBand = band;
				band.click();
			}
		});
		if (pct) {
			moveSlider(newWidth * pct);
		}
	};

	

	var moveSlider = function (leftCoord) {
		sliderCurLeft = leftCoord;
		sliderbar.css('left', sliderCurLeft);
		sliderMove.call({ css: { left: sliderCurLeft } });
		

	};

	var startMode = function () {
		canvasOverlay.hide();
		tourMode = $(this).attr('data-mode') === 'tour';
		if (tourMode) {
			toggleSpectrumChrome(false);
			wwt.WebControl.playTour();
		} else {
			interactionMode();

		}
		
		
		wwt.triggerResize();
	};

	var resizeHandler = function (args) {
		if (tourMode) {
			return;
		}
		if (args.fullscreen) {
			$('.player-controls').append(slider);
			setupSlider(slider.width(), $('body').width() - 88);
			
		} else {
			$('.wwt-webcontrol-wrapper').after(slider);
			setupSlider(slider.width(), canvas.width());
		}
		
	};

	var interactionMode = function () {
		tourMode = false;
		wwt.WebControl.interact();
		toggleSpectrumChrome(true); 
		setupSlider();
	   
		wwt.triggerResize();
	};

   

	//#endregion
	return api;

})();