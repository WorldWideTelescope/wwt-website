
var wwt = {
	triggerResize: function() {
		setTimeout(function() { $(window).trigger('contentchange'); }, 100);
	},
	accordianClick: function (e) {
		//return;
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
	failedSigninAttempts:0//,
	//resLoc:location.href.indexOf('worldwidetelescope.org') != -1 ? 'http://cdn.worldwidetelescope.org' : ''
};
if (top === self) {
	wwt.viewMaster = (function() {
		var api = {
			init: init,
			loaded: loaded,
			fullScreenImage: fullScreenImage,
            signIn:signIn
		};

		var isLoaded = false;
		var layoutTimer;

		var isSmall = false;
	    
		
		function init() {
		    
		    initLiveId();
		    
			wwt.resLoc = $('body').attr('resLoc');
		    //console.log('init');
		    bindEvents();
			
			resize();
			if (!isLoaded) {
				layoutTimer = setInterval(function() {
					wwt.triggerResize();
					//console.log('polling loaded');
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
			if ('ontouchstart' in document.documentElement){
				wwt.isTouch = true;
				
			}
			
			wwt.wcLink = '/webclient';
			
		};

	    function bindEvents() {
	        $(window).on('resize contentchange', resize);
	        $(window).on('hashchange', hashChange);
	        $('img.img-border:not([data-nofs])').on('click', fullScreenImage).attr('title', 'click to view full size');
	        $('#accordion').on('click', wwt.accordianClick);
	        $('#chkRemember').on('change', function() {
	            wwt.user.set('rememberMe', $(this).prop('checked'));
	        });
	    }

	    //function logWL(info) {
	    //    console.log(this,arguments);
	    //    if (info.indexOf('WL.login') ===0 && info.indexOf('denied') !== -1) {
	    //        //$('#signin').html("Login failed").title('Retry Login');
	    //    }
	    //}

	    function initLiveId() {
	        var autoSignin = false;
	        var signedIn = $('#signinContainer input').length !== 1;
	        if (wwt.user.get('rememberMe') === false) {
	            $('#chkRemember').prop('checked', false);
	        }
	        else if (wwt.user.get('rememberMe') === true) {
	            $('#chkRemember').prop('checked', true);
	            autoSignin = true;
	        } else {
	            wwt.user.set('rememberMe', false);
	        }
	        if (wwt.currentResolution === 'md' || wwt.currentResolution === 'lg') {
	            $('#signinContainer label').slideUp(function() {
	                $('.sign-in').show();
	            });
	        }
	        if (!signedIn) {
	            WL.init({
	                client_id: _liveClientId,
	                redirect_uri: 'http://' + location.host + '/Community',
	                response_type: "token",
                    logging:true
	            });
	        }
	        $('#signinContainer').on('mouseenter', function () {
	            if (wwt.currentResolution === 'md' || wwt.currentResolution === 'lg') {
	                $(this).find('label').slideDown();
	            }
	        }).on('mouseleave', function () {
	            if (wwt.currentResolution === 'md' || wwt.currentResolution === 'lg') {
	                $(this).find('label').slideUp();
	            }
	        }).find('#signin').on('click', function () {
	            signinScope = $('#chkRemember').prop('checked') ? 'wl.signin' : 'wl.basic';
	            signIn();
	        });

	        
            if (autoSignin && !signedIn) {
	            signIn();
	        }
	    }

	    function signIn() {
	        wwt.signingIn = true;
	        WL.login({
	            scope: ['wl.signin', 'wl.emails']//, "wl.offline_access"
	        }).then(function (session) {
	            if (!session.error) {
	                $('#signinContainer label').html('&nbsp;');
	                WL.api({
	                    path: "me",
	                    method: "GET"
	                }).then(
                        function (response) {
                            console.log(response);
                            tryServerSignin(response);
                        },
                        loginFail
                    );
	            }
	        },
            loginFail);
	        if ($('#signin').attr('title').indexOf('(Sign') === -1) {
	            $('#signin').html('Signing in...').attr('title', 'Please wait while we sign you in to WorldWide Telescope');
	        }
	    }

        var tryServerSignin = function(response) {
            $.get("/LiveId/Authenticate").success(function (data) {
                console.log(arguments);
                $('#profileMenuItem, #profileLink').removeClass('hide');
                $(window).trigger('login');
                wwt.signingIn = false;
                if (data.Status === 'Connected') {
                    $('#signin').off('click').html(data.Session.User).on('click', function(e) {
                        e.stopImmediatePropagation();
                        e.preventDefault();
                        location.href = '/Community/Profile';
                    }).attr('title', '(Signed in) View your WWT Profile').prop('authenticated', true);
                } else if (data.LogoutUrl) {
                    
                    WL.logout();
                    location.href = data.LogoutUrl;
                } else {
                    loginFail(data);
                }
            }).error(loginFail);
        }

	    var loginFail = function(responseFailed) {
            wwt.signingIn = false;
            wwt.failedSigninAttempts++;
            console.log('loginfail, attempts:' + wwt.failedSigninAttempts, responseFailed);
            if (wwt.failedSigninAttempts > 1) {
                $('#signin').html("Login failed");
                $(window).trigger('loginfail');
                WL.logout();
            } else {
                // Cleanse wl_auth cookie
                var hosts = ['http://www.worldwidetelescope.org', 'www.worldwidetelescope.org', '.worldwidetelescope.org', 'worldwidetelescope.org'];
                document.cookie = 'wl_auth=; expires=Thu, 01-Jan-1970 00:00:01 GMT;';
                for (var i = 0; i < hosts.length; i++) {
                    document.cookie = 'wl_auth=; expires=Thu, 01-Jan-1970 00:00:01 GMT;domain=' + hosts[i] + ';path=/';
                }
                tryServerSignin();
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
			//console.log('loaded');
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

			var isScrollable = mainH + footerH + navH > $(window).height(); // || $(document).height() > $(window).height();
			
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
			} catch (er ){
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
				if (fsImg.width() === 0)return;
				var move = new wwt.Move({ el: fsImg, onmove: function () { fsImg.attr('hasMoved', true); } });
				if (el.data('coords') && (fsImg.width() > $(window).width() || el.height() > $(window).height())) {
					coords = el.data('coords').split(',');
				}
				//alert(fsImg.width() + ' ' + $(window).width());
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
	if (obj.offsetParent)
		while (1) {
			curleft += obj.offsetLeft;
			if (!obj.offsetParent)
				break;
			obj = obj.offsetParent;
		}
	else if (obj.x)
		curleft += obj.x;
	return curleft;
}

function findPosY(obj) {
	var curtop = 0;
	if (obj.offsetParent)
		while (1) {
			curtop += obj.offsetTop;
			if (!obj.offsetParent)
				break;
			obj = obj.offsetParent;
		}
	else if (obj.y)
		curtop += obj.y;
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
	}
	else {
		if (obj.x)
			curleft += obj.x;
		if (obj.y)
			curtop += obj.y;
	}
	return { left: curleft, top: curtop };
}

function showPlayer() {
	//		if (document.getElementById('ctl00_ctl00_ContentPlaceHolder1_MainContent_slPlayer') != null)
	//			document.getElementById('ctl00_ctl00_ContentPlaceHolder1_MainContent_slPlayer').style.visibility = 'visible';
	//		else if (document.getElementById('divPlayerContainer') != null)
	//			document.getElementById('divPlayerContainer').style.visibility = 'visible';
}

function hidePlayer() {
	//		if (document.getElementById('ctl00_ctl00_ContentPlaceHolder1_MainContent_slPlayer') != null)
	//			document.getElementById('ctl00_ctl00_ContentPlaceHolder1_MainContent_slPlayer').style.visibility = 'hidden';
	//		else if (document.getElementById('divPlayerContainer') != null)
	//			document.getElementById('divPlayerContainer').style.visibility = 'hidden';
}

function clickEl(id, isTours) {
	if (isTours) {
		var div = $('#divHiddenButtons');
		var el = $('#' + div.find('a[id*="' + id + '"]').attr('id'));
		eval(el.attr('href').split('javascript:')[1]);
	}
	else {
		eval($(id).attr('href').split('javascript:')[1]);
	}
}

function getQSValue(name) {
	name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	var regexS = "[\\?&]" + name + "=([^&#]*)";
	var regex = new RegExp(regexS);
	var results = regex.exec(window.location.href);
	if (results == null)
		return "";
	else
		return results[1];
}

