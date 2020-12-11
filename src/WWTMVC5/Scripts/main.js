var wwt = {
	triggerResize: function() {
		setTimeout(function() {
			$(window).trigger('contentchange');
		}, 100);
	},

	accordianClick: function (e) {
		if ($(document.body).width() > 991 && $(e.target).data('toggle') === 'collapse') {
			wwt.scriptHashChange = true;

			location.href = $(e.target).attr('href');

			for (var i = 0; i < 500; i += 50) {
				setTimeout(wwt.triggerResize, i);
			}

			setTimeout(function() {
				wwt.scriptHashChange = false;
				wwt.triggerResize();
			}, 500);
		}
	},

	failedSigninAttempts: 0
};

if (top === self) {
	wwt.viewMaster = (function() {
		var api = {
			init: init,
			loaded: loaded,
			fullScreenImage: fullScreenImage,
			signIn: signIn
		};

		var isLoaded = false;
		var layoutTimer;
		var isSmall = false;

		function init() {
			try {
					initLiveId();
			} catch (ex) {
			}

			bindEvents();
			var rememberSetting = wwt.user && wwt.user.get('rememberMe');
			wwt.autoSignin = rememberSetting && rememberSetting === true;
			resize();

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

			if ('ontouchstart' in document.documentElement) {
				wwt.isTouch = true;
			}
		};

		function bindEvents() {
			$(window).on('resize contentchange', resize);
			$(window).on('hashchange', hashChange);
			$('img.img-border:not([data-nofs])').on('click', fullScreenImage).attr('title', 'click to view full size');
			$('#accordion').on('click', wwt.accordianClick);
		}

		function initLiveId() {
			var signedIn = $('#signinContainer').attr('loggedIn') === 'true';

			if (!signedIn && getQSValue('code') != null) {
				var returnUrl = location.href.split('?')[0];
				console.log(returnUrl);
				return;
			}

			var rememberSetting = wwt.user.get('rememberMe');
			var autoSignin = wwt.autoSignin = rememberSetting && rememberSetting === true;

			if (wwt.currentResolution === 'md' || wwt.currentResolution === 'lg') {
				$('#signinContainer label').slideUp(function() {
					$('.sign-in').show();
				});
			}

			$('#signinContainer #signin').on('click', signIn);

			if (autoSignin && !signedIn) {
				signIn();
			}
		}

		function signIn() {
			var signedIn = $('#signinContainer').attr('loggedIn') === 'true';
			if (signedIn || location.host.indexOf('localhost') > -1) {
					return;
			}

			wwt.signingIn = true;
			wwt.user.set('rememberMe', true);
			if (wwt.user.get('authCodeRedirect')) {
					cleanCookies();
			}

			wwt.user.set('authCodeRedirect', true);
			var wlUrl = 'https://login.live.com/oauth20_authorize.srf?client_id=' +
						encodeURIComponent(_liveClientId) +
						'&scope=wl.offline_access%20wl.emails&response_type=code&redirect_uri=' +
						encodeURIComponent(_liveClientRedirectUrl) +
						'&display=popup';
			location.href = wlUrl;
		}

		var cleanCookies = function() {
			var hosts = [
				'.wwtstaging.azurewebsites.net',
				'wwtstaging.azurewebsites.net',
				'.worldwidetelescope.org',
				'worldwidetelescope.org',
				'worldwidetelescope.org'
			];

			document.cookie = 'wl_auth=; expires=Thu, 01-Jan-1970 00:00:01 GMT;';
			for (var i = 0; i < hosts.length; i++) {
					document.cookie = 'wl_auth=; expires=Thu, 01-Jan-1970 00:00:01 GMT;domain=' + hosts[i] + ';path=/';
			}
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

		var resize = function() {
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

		var determineFooterLayout = function() {
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

		var fsImg;

		function fullScreenImage(e) {
			e.stopPropagation();

			var el = $(this);
			var coords = null;

			try {
				fsImg.remove();
			} catch (er) {
			}

			fsImg = $('<img class=img-fullscreen />')
				.attr('src', $(this).attr('src'))
				.hide();

			$('body').on('click', function (e) {
				var target = $(e.target);

				if (target.hasClass('img-fullscreen') && target.attr('hasMoved')) {
					e.stopPropagation();
					return;
				}

				if (fsImg.prop('vis')) {
					$(fsImg).fadeOut(function () {
						fsImg.prop('vis', false);
					});
				}
			}).append(fsImg);

			fsImg.fadeIn(function () {
				fsImg.prop('vis', true);
			});

			var loaded = function () {
				if (fsImg.width() === 0)
					return;

				var move = new wwt.Move({ el: fsImg, onmove: function () { fsImg.attr('hasMoved', true); } });

				if (el.data('coords') && (fsImg.width() > $(window).width() || el.height() > $(window).height())) {
					coords = el.data('coords').split(',');
				}

				var offsetTop = 75;
				var left = coords ? 0 - coords[1] : Math.max(($(window).width() - fsImg.width()) / 2, 3);
				var top = coords ? (offsetTop - coords[0]) + $(document).scrollTop() : $(document).scrollTop() + offsetTop;

				fsImg.css({
					left: left,
					top: top
				});
			}

			fsImg.on('load', loaded);
			loaded();
		};

		return api;
	})();

	$(window).on('load', function() {
		wwt.viewMaster.loaded();
	});

	$(wwt.viewMaster.init);
}

function findPosX(obj) {
	var curleft = 0;

	if (obj.offsetParent) {
		while (1) {
			curleft += obj.offsetLeft;
			if (!obj.offsetParent)
				break;
			obj = obj.offsetParent;
		}
	} else if (obj.x) {
		curleft += obj.x;
	}

	return curleft;
}

function findPosY(obj) {
	var curtop = 0;

	if (obj.offsetParent) {
		while (1) {
			curtop += obj.offsetTop;
			if (!obj.offsetParent)
				break;
			obj = obj.offsetParent;
		}
	} else if (obj.y) {
		curtop += obj.y;
	}

	return curtop;
}

function findPos(obj) {
	var curleft = 0;
	var curtop = 0;

	if (obj.offsetParent) {
		while (obj.offsetParent) {
			curleft += obj.offsetLeft - obj.scrollLeft;
			curtop += obj.offsetTop - obj.scrollTop;
			var position = '';

			if (obj.style && obj.style.position)
				position = obj.style.position.toLowerCase();

			if (position == 'absolute' || position == 'relative')
				break;

			while (obj.parentNode != obj.offsetParent) {
				obj = obj.parentNode;
				curleft -= obj.scrollLeft;
				curtop -= obj.scrollTop;
			}

			obj = obj.offsetParent;
		}
	} else {
		if (obj.x)
			curleft += obj.x;
		if (obj.y)
			curtop += obj.y;
	}

	return { left: curleft, top: curtop };
}

function showPlayer() {}

function hidePlayer() {}

function clickEl(id, isTours) {
	if (isTours) {
		var div = $('#divHiddenButtons');
		var el = $('#' + div.find('a[id*="' + id + '"]').attr('id'));
		eval(el.attr('href').split('javascript:')[1]);
	} else {
		eval($(id).attr('href').split('javascript:')[1]);
	}
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
