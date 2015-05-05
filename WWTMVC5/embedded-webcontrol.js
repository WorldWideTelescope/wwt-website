
(function () {//avoid possible ns collision with wwt
	var api = {
		importImage: function (imgUrl) {
			iframe.contentWindow.postMessage('import::' + imgUrl, hostUrl);
		},
		showCrosshairs: function (show) {
			runtimeSetting('crosshairs', show);
		},
		showEcliptic: function (show) {
			runtimeSetting('ecliptic', show);
		},
		showEclipticGrid: function (show) {
			runtimeSetting('eclipticGrid', show);
		},
		showAltAzGrid: function (show) {
			runtimeSetting('altAzGrid', show);
		},
		showGalacticGrid: function (show) {
			runtimeSetting('galacticGrid', show);
		},
		showEquatorialGrid: function (show) {
			runtimeSetting('equatorialGrid', show);
		},
		showFigures: function (show) {
			runtimeSetting('figures', show);
		},
		showBoundaries: function (show) {
			runtimeSetting('boundaries', show);
		},
		showPictures: function (show) {
			runtimeSetting('pictures', show);
		},
		galacticPlaneMode: function (show) {
		    runtimeSetting('galacticPlaneMode', show);
		},
		getCoords: function (callback) {
		    coordCallback = callback;
		    iframe.contentWindow.postMessage('getCoords', hostUrl);
		},
        onWtmlChange: function(callback) {
            wtmlCallback = callback;
        }
	}; 
		
	var hostUrl,
		div,
		containerWidth,
		aspect,
		iframe,
		fullscreen = false,
		    coordCallback,
		    wtmlCallback;

	var runtimeSetting = function(setting, value) {
		iframe.contentWindow.postMessage('set::' + setting + '=' + value, hostUrl);
	}

	var init = function() {
	    window.addEventListener("message", function (event) {
	        if (event.origin === hostUrl) {
	            if (event.data.coords && coordCallback) {
	                coordCallback(event.data.coords);
	            }
	            if (event.data.wtmlEvent && typeof wtmlCallback == 'function') {
	                wtmlCallback(event.data.wtmlEvent);
	            }
	        }
	    });
	        div = document.querySelector('div#wwtControl');
		hostUrl = div.getAttribute('data-host') ? div.getAttribute('data-host') : 'http://worldwidetelescope.org';
		containerWidth = div.offsetWidth;
		aspect = .5625;
		var options = {
			hostWindow: 'http://' + location.hostname
		};
		if (div.getAttribute('data-wwt-tour-location')) {
			options.tour = div.getAttribute('data-wwt-tour-location');
		}  if (div.getAttribute('data-wwt-galactic-mode')) {
			options.galacticMode = div.getAttribute('data-wwt-galactic-mode');
		}  if (div.getAttribute('data-wwt-import-image')) {
			options.importImage = div.getAttribute('data-wwt-import-image');
		}  if (div.getAttribute('data-tour-location')) {
			options.tour = div.getAttribute('data-tour-location');
		}  if (div.getAttribute('data-galactic-mode')) {
			options.galacticMode = div.getAttribute('data-galactic-mode');
		}
		if (div.getAttribute('data-import-image')) {
			options.importImage = div.getAttribute('data-import-image');
		}
		if (div.getAttribute('data-wwt-settings')) {
			options.settings = div.getAttribute('data-wwt-settings');
		} else if (div.getAttribute('data-settings')) {
			options.settings = div.getAttribute('data-settings');
		}
		if (div.getAttribute('data-wtml')) {
			options.wtml = div.getAttribute('data-wtml');
		}
		if (div.getAttribute('data-display')) {
			options.display = div.getAttribute('data-display');
		}
		var aspectString = null;
		if (div.getAttribute('data-wwt-aspect-ratio')) {
			aspectString = div.getAttribute('data-wwt-aspect-ratio');
		} else if (div.getAttribute('data-aspect-ratio')) {
			aspectString = div.getAttribute('data-aspect-ratio');
		}
		if (aspectString) {
			var w_h = aspectString.split(':');
			if (w_h.length === 2 && !isNaN(parseInt(w_h[0])) && !isNaN(parseInt(w_h[1]))) {
				aspect = parseInt(w_h[1]) / parseInt(w_h[0]);
			} else {
				aspect = parseFloat(aspectString);
			}
			if (isNaN(aspect) || aspect > 1.5 || aspect < .3) {
				aspect = .6667;
			}
		}
		div.style.position = 'relative';
		var fsButton = document.createElement('a');
		fsButton.setAttribute('style', 'position:absolute;bottom:0;right:0;height:33px;width:40px;display:block;background:rgba(234,234,234,.01)');
		fsButton.onmouseenter = function() {
			iframe.contentWindow.postMessage('fsenter', hostUrl);
		};
		fsButton.onmouseleave = function() {
			iframe.contentWindow.postMessage('fsleave', hostUrl);
		};
		fsButton.onclick = fullScreenMode;

	    
		div.appendChild(fsButton);
		iframe = document.createElement('iframe');
		iframe.setAttribute('src', hostUrl + '/WebControl.aspx?v=5.0.23#' + JSON.stringify(options));
		iframe.setAttribute('style', 'border:none;background-color:transparent;');
		iframe.setAttribute('frameborder', 'no');
		iframe.setAttribute('scrolling', 'no');
		iframe.style.height = (containerWidth * aspect) + 'px';
		iframe.style.width = '100%';
		div.appendChild(iframe);
	};

	var requestFullScreen = function (element) {
		if (element.requestFullscreen) {
			element.requestFullscreen();
		} else if (element.msRequestFullscreen) {
			element.msRequestFullscreen();
		} else if (element.mozRequestFullScreen) {
			element.mozRequestFullScreen();
		} else if (element.webkitRequestFullscreen) {
			element.webkitRequestFullscreen();
		}
		div.style.zIndex = 99999;
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
		div.style.zIndex = 0;
	};

	var fullScreenMode = function () {
		if (!fullscreen) {
			requestFullScreen(document.body);
			div.style.width = '100%';
			div.style.height = '100%';
			iframe.style.height = '100%';
			iframe.style.width = '100%';
			div.style.position = 'fixed';
			div.style.top = 0;
			div.style.left = 0;
			fullscreen = true;
			
		} else {
			exitFullScreen(document.body);
			div.style.width = containerWidth;
			div.style.height = (containerWidth * aspect) + 'px';
			iframe.style.height = (containerWidth * aspect) + 'px';
			iframe.style.width = containerWidth + 'px';
			div.style.position = 'relative';
			fullscreen = false;
		}
	};

	init();
    if (!window.wwt) {
        window.wwt = api;
    } 
        window.ewwt = api;
    
})();
