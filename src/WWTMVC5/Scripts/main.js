var wwt = {
	triggerResize: function() {
		setTimeout(function() {
			$(window).trigger('contentchange');
		}, 100);
	}
};

if (top === self) {
	wwt.viewMaster = (function() {
		var api = {
			init: init,
			loaded: loaded,
			signIn: signIn
		};

    // OAuth stuff. There used to be more "remember me" functionality, but it
    // was rickety and this is UI is super rarely used now.

		function initLiveId() {
      // This attribute will be set by the server if it's happy with our login cookies:
			var signedIn = $('#signinContainer').attr('loggedIn') === 'true';
			$('#signinContainer #signin').on('click', signIn);
		}

		function signIn() {
			var signedIn = $('#signinContainer').attr('loggedIn') === 'true';
			if (signedIn) {
					return;
			}

			var wlUrl = 'https://login.live.com/oauth20_authorize.srf?client_id=' +
						encodeURIComponent(_liveClientId) +
						'&scope=wl.offline_access%20wl.emails&response_type=code&redirect_uri=' +
						encodeURIComponent(_liveClientRedirectUrl) +
						'&display=popup';
			location.href = wlUrl;
		}

		// Old UI stuff:

		var isLoaded = false;
		var layoutTimer;
		var isSmall = false;

		function init() {
      initLiveId();
			bindEvents();
			resize();

			if (wwt.currentResolution === 'md' || wwt.currentResolution === 'lg') {
				$('#signinContainer label').slideUp(function() {
					$('.sign-in').show();
				});
			}

			if (!isLoaded) {
				layoutTimer = setInterval(function() {
					wwt.triggerResize();
				}, 100);
			}

			setTimeout(resize, 500);

			$('img.img-border[data-nofs]').css('cursor', 'default');
			$('a[data-toggle=tooltip]').tooltip({ container: 'body', trigger: 'hover' });
			$('label[data-toggle=tooltip]').tooltip({ trigger: 'hover' });
			$('input[data-toggle=tooltip],textarea[data-toggle=tooltip]').tooltip({ trigger: 'focus' });

			hashChange();

			var ua = navigator.userAgent;

			if (ua.indexOf('MSIE 9') !== -1 || ua.indexOf('MSIE 8') !== -1) {
				$('.navbar').css('filter', '');
				$('.navbar-inverse .navbar-brand, .navbar-nav>li>a').css({
					filter: '',
					padding: '21px 15px 21px 31px'
				});
				if (!wwt.user.get('downlevelIgnore')) {
					bootbox.alert('It looks like you are using an older version of Internet Explorer. WorldWide Telescope has been optimized for the latest browser technologies. Please upgrade your browser.<br/><label><input type=checkbox checked=checked onclick=wwt.user.set("downlevelIgnore",!this.checked) /> Keep reminding me.</label>');
				}
			}
		};

    function bindEvents() {
			$(window).on('resize contentchange', resize);
			$(window).on('hashchange', hashChange);
		}

		function hashChange() {
			if (wwt.scriptHashChange) {
				return;
			}

			try {
				var curHash = location.hash.replace(/##/g, '#');
				var hashLink = $('a[href=' + curHash + ']');

				if (hashLink.length) {
					hashLink.click();
					location.href = curHash;
					setTimeout(function() {
						//hashLink.click();
						location.href = curHash;
						var st = $(window).scrollTop();  //your current y position on the page
						$(window).scrollTop(st -(60+ $('.navbar-fixed-top').height()));
					}, 500);
				}

				wwt.triggerResize();
			} catch (er) {
			}
		}

		function loaded() {
			isLoaded = true;
			clearInterval(layoutTimer);
		}

		function resize() {
			determineFooterLayout();

			if ($('.large-video-player.autoresize').length) {
				var videoaspect = 9 / 16;
				var w = Math.max(310, $('.large-video-player.autoresize').width());
				$('.large-video-player iframe').css({
					height: w * videoaspect,
					width: w
				});
			};

			var prevRes = wwt.currentResolution;

			if ($(window).width() >= 1200) {
				wwt.currentResolution = 'lg';
			} else if ($(window).width() >= 992) {
				wwt.currentResolution = 'md';
			} else if ($(window).width() >= 768) {
				wwt.currentResolution = 'sm';
			} else {
				wwt.currentResolution = 'xs';
			}

			if (wwt.currentResolution !== prevRes) {
				$(window).trigger('resolutionchange');
			}
		}

		function determineFooterLayout() {
			var mainH = $('#divMain').height(),
				footerH = $('.footer').height() + 20,
				navH = $('.navbar').height();

			var isScrollable = mainH + footerH + navH > $(window).height();

			$('.footer').toggleClass('navbar-fixed-bottom', !isScrollable);
			$('body').css('overflow', isScrollable ? 'auto' : 'hidden');

			if (!isScrollable) {
				$(window).scrollTop(0);
			}

			var winW = $(window).width();

			if (winW < 562 && !isSmall) {
				isSmall = true;
				$('.navbar-inverse .navbar-header').addClass('mobile');
			} else if (winW >= 562 && isSmall) {
				isSmall = false;
				$('.navbar-inverse .navbar-header').removeClass('mobile');
			}
		};

		return api;
	})();

	$(window).on('load', function() {
		wwt.viewMaster.loaded();
	});

	$(wwt.viewMaster.init);
}

function getQSValue(name) {
	name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	var regexS = "[\\?&]" + name + "=([^&#]*)";
	var regex = new RegExp(regexS);
	var results = regex.exec(window.location.href);

	if (results == null)
		return null;
	else
		return results[1];
}
