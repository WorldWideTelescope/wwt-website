
wwt.Tween = function (initArgs) {
		//#region api
		var api = {
			isTweening: false,
			SetNext: function (args) {
				setNext(args);
				return this;
			},
			Start: function () {
				start();
			},
			Reset: function (startOver) {
				if (this.isTweening)
					reset(startOver);
			},
			Pause: function () {
				stopFlag = true;
			},
			Resume: function () {
				start();
			}
		};
		//#endregion

		//#region private globals
		var startArgs,
			stopFlag = false,
			startTime,
			endTime,
			animTime,
			css = null, // css object that contains keys/values to tween
			gradients = null, //array of gradients to tween
			gradientObj = {},/*{
					template: [the template with placeholders for tween values]
					startData:(constant during tween) [start values],
					workingData:(variable during tween) [working values],
					endData: (constant during tween)[end values]
				}*/
			transTempl = 'rotate(deg)',
			el = null,
			timer = 0, // time window in ms for tween to run within
			timerId, // for setInterval fn
			trans = 0, // transition id (circular, elastic, linear, etc.)
			delay = 10, // delay amt for setInterval
			easing = 3; // 0=none, 1=in, 2=out, 3=in/out

		var frames = 0/*, totalFrames = 0, totalTime = 0*/;
		//#endregion

		//#region initialization
		var init = function(args) {
			startArgs = new wwt.TweenArgs(args).args;
			el = this.el || args.el;
			timer = args.timer || 400;
			trans = args.trans || 4;
			easing = args.ease || 2;
			gradients = args.gradients || null;
			
		};

		var initCssState = function (args) {
			args = args || startArgs;
			css = args.css;
			
			if (gradients) {
				gradientObj = gradient.createAnimation(gradients[0], gradients[1]);
				css.backgroundImage = [];
			}
			for (var key in css) {
				var v = css[key];
				if ($.isArray(v) && v.length == 3) v = v[1];
				var vals;
				if (key.toLowerCase().indexOf('color') != -1 &&
					typeof (v) == 'string' &&
					(v.indexOf('#') == 0 || v.indexOf('rgb') == 0)) {

					var elColor = el.css(key.indexOf('border') == 0 ? 'border-top-color' : key);
					var curRgb = elColor == '' || elColor == undefined ? [0, 0, 0] : wwt.getRGBArray(elColor);
					var newRgb = wwt.getRGBArray(v);
					vals = [curRgb, newRgb, curRgb, 'color'];
				}
				else if (key == 'backgroundImage') {
					vals = [gradientObj.startData, $.tmpl(gradientObj.template, gradientObj.endData)[0].data];
					
				} else if (key == 'transform') {
					var r = parseInt(el.prop('rotate')) || 0;
					vals = [r, v, r];
					el.prop('rotate', v);
				} else {
					var ky = (key == 'border-width' || key == 'borderWidth') ? 'borderTopWidth' :
						key == 'border-radius' || key == 'borderRadius' ? 'border-top-right-radius' : key;
					vals = [parseFloat(el.css(ky)), v];
					vals[2] = vals[0]; // current step value
				}
				css[key] = vals;
			}
		};
		//#endregion

		//#region api functions
		var setNext = function (args) {
			if (args.css) {
				startArgs.css = args.css;
				startArgs.trans = trans = args.trans || trans;
				startArgs.ease = easing = args.ease || easing;
			} else {
				startArgs.css = args;
				startArgs.trans = trans;
				startArgs.ease = easing;
				args = { css: startArgs.css };
			}
			if (args.gradients) {
				startArgs.gradients = gradients = args.gradients;
			} else gradients = startArgs.gradients = null;
			if (args.timer) {
				startArgs.timer = timer = args.timer;
			}
			initCssState(args);
		};
		var start = function () {
			frames = 0;
			if (api.isTweening) reset(true);
			startTime = new Date().valueOf();
			endTime = startTime + timer;
			animTime = timer;
			stopFlag = false;
			api.isTweening = true;
			timerId = setInterval(tweenStep, delay);
			//console.log('start');
		};
	   
		var reset = function (startOver) {
			clearInterval(timerId);
			initCssState(startArgs);
			start();
			if (!startOver) pause();
		};
		//#endregion

		//#region tween calc
		var tweenStep = function () {
			//console.log(frames);
			frames++;
			stopFlag = new Date().valueOf() >= endTime;

			var setCss = {};
			var keyIndex = stopFlag ? 1 : 2;

			for (var key in css) {
				var vals = css[key];
				if (vals[3] == 'color') {
					vals[2] = 'rgb(' +
						Math.round(calcStepAmt(vals[0][0], vals[1][0])) + ', ' +
						Math.round(calcStepAmt(vals[0][1], vals[1][1])) + ', ' +
						Math.round(calcStepAmt(vals[0][2], vals[1][2])) + ')';
				}
				else if (key == 'backgroundImage') {
					$.each(gradientObj.workingData, function (i) {
						var v = calcStepAmt(gradientObj.startData[i], gradientObj.endData[i]);
						var k0 = i.charAt(0);
						gradientObj.workingData[i] = k0 == 'r' || k0 == 'g' || k0 == 'b' || k0 == 'd' ? Math.round(v) : v;
					});
					vals[2] = $.tmpl(gradientObj.template, gradientObj.workingData)[0].data;
					
				} else if (key == 'transform') {
					
					vals[2] = transTempl.replace(/deg/,Math.round(calcStepAmt(vals[0], vals[1])) + 'deg');
					
				} else {
					vals[2] = calcStepAmt(vals[0], vals[1]);
				}
				setCss[key] = vals[keyIndex];
			}
			el.css(setCss);
			if (stopFlag) {
				clearInterval(timerId);
				api.isTweening = false;
				el.trigger('tweenComplete');
				if (startArgs.onComplete) {
					startArgs.onComplete.call();
				}
				initCssState();
				clearInterval(timerId);
				api.isTweening = false;
				/*var fps = (frames / (timer / 1000)).toFixed(1);
				totalFrames += frames;
				totalTime += timer;
				var afps = (totalFrames / (totalTime / 1000)).toFixed(1);
				*/
				return;
			}

		};

		// Robert Penner Easing Equations
		var calcStepAmt = function (v1, v2) {
			var b = v1;
			var c = v2 - v1;
			var t = new Date().valueOf() - startTime;
			var d = timer;
			var s;
			
			switch (trans) {
				case 0:
					// transitions.linear
					return c * t / d + b;
				case 1:
					// transitions.back
					s = 1.70158;
					switch (easing) {
						case 1:
							return c * (t /= d) * t * ((s + 1) * t - s) + b;
						case 2:
							return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
						default:
							if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
							return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
					}
				case 2:
					// transitions.bounce
					var bounceOut = function (t1, b1, c1, d1) {
						if ((t1 /= d1) < (1 / 2.75)) {
							return c1 * (7.5625 * t1 * t1) + b1;
						} else if (t1 < (2 / 2.75)) {
							return c1 * (7.5625 * (t1 -= (1.5 / 2.75)) * t1 + .75) + b1;
						} else if (t1 < (2.5 / 2.75)) {
							return c1 * (7.5625 * (t1 -= (2.25 / 2.75)) * t1 + .9375) + b1;
						} else {
							return c1 * (7.5625 * (t1 -= (2.625 / 2.75)) * t1 + .984375) + b1;
						}
					};
					var bounceIn = function (t1, b1, c1, d1) {
						return c1 - (bounceOut(d1 - t1, 0, c1, d1)) + b1;
					};
					switch (easing) {
						case 1:
							return bounceIn(t, b, c, d);
						case 2:
							return bounceOut(t, b, c, d);
						default:
							if (t < d / 2) return (bounceIn(t * 2, 0, c, d)) * .5 + b;
							else return bounceOut(t * 2 - d, 0, c, d) * .5 + c * .5 + b;
					}
				case 3:
					// transitions.circular
					switch (easing) {
						case 1:
							return -c * (Math.sqrt(1 - (t /= d) * t) - 1) + b;
						case 2:
							return c * Math.sqrt(1 - (t = t / d - 1) * t) + b;
						default:
							if ((t /= d / 2) < 1) return -c / 2 * (Math.sqrt(1 - t * t) - 1) + b;
							return c / 2 * (Math.sqrt(1 - (t -= 2) * t) + 1) + b;
					}
				case 4:
					// transitions.cubic
					switch (easing) {
						case 1:
							return c * (t /= d) * t * t + b;
						case 2:
							return c * ((t = t / d - 1) * t * t + 1) + b;
						default:
							if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
							return c / 2 * ((t -= 2) * t * t + 2) + b;
					}
				case 5:
					// transitions.elastic
					var p = p = d * .3, a = c;
					s = (d * .3) / 4;
					switch (easing) {
						case 1:
							if (t == 0) return b;
							if ((t /= d) == 1) return b + c;
							else s = p / (2 * Math.PI) * Math.asin(c / a);
							return -(a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
						case 2:
							if (t == 0) return b;
							if ((t /= d) == 1) return b + c;
							else s = p / (2 * Math.PI) * Math.asin(c / a);
							return (a * Math.pow(2, -10 * t) * Math.sin((t * d - s) * (2 * Math.PI) / p) + c + b);
						default:
							if (t == 0) return b;
							if ((t /= d / 2) == 2) return b + c;
							p = d * (.3 * 1.5);
							if (!a || a < Math.abs(c)) {
								a = c;
								s = p / 4;
							} else s = p / (2 * Math.PI) * Math.asin(c / a);
							if (t < 1) return -.5 * (a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
							return a * Math.pow(2, -10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
					}
				case 6:
					// transitions.exponential
					switch (easing) {
						case 1:
							return (t == 0) ? b : c * Math.pow(2, 10 * (t / d - 1)) + b;
						case 2:
							return (t == d) ? b + c : c * (-Math.pow(2, -10 * t / d) + 1) + b;
						default:
							if (t == 0) return b;
							if (t == d) return b + c;
							if ((t /= d / 2) < 1) return c / 2 * Math.pow(2, 10 * (t - 1)) + b;
							return c / 2 * (-Math.pow(2, -10 * --t) + 2) + b;
					}
				case 7:
					// transitions.quadratic
					switch (easing) {
						case 1:
							return c * (t /= d) * t + b;
						case 2:
							return -c * (t /= d) * (t - 2) + b;
						default:
							if ((t /= d / 2) < 1) return c / 2 * t * t + b;
							return -c / 2 * ((--t) * (t - 2) - 1) + b;
					}
				case 8:
					// transitions.quartic
					switch (easing) {
						case 1:
							return c * (t /= d) * t * t * t + b;
						case 2:
							return -c * ((t = t / d - 1) * t * t * t - 1) + b;
						default:
							if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
							return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
					}
				case 9:
					// transitions.quintic
					switch (easing) {
						case 1:
							return c * (t /= d) * t * t * t * t + b;
						case 2:
							return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
						default:
							if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
							return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
					}
				case 10:
					// transitions.sine
					switch (easing) {
						case 1:
							return -c * Math.cos(t / d * (Math.PI / 2)) + c + b;
						case 2:
							return c * Math.sin(t / d * (Math.PI / 2)) + b;
						default:
							return -c / 2 * (Math.cos(Math.PI * t / d) - 1) + b;
					}
			}
			return 0;
		};
		//#endregion
		init(initArgs);
		return api;
	};
	
	wwt.TweenArgs = function (args) {
		this.args = args;
	};
