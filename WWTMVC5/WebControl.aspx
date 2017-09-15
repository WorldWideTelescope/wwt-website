<%@ Page Language="C#" %>

<!DOCTYPE html>
<html>
<head runat="server">
	<script runat="server">
	public string DebugQs = "";

	protected void Page_Load(object sender, EventArgs e)
	{
		if (Request.QueryString["debug"] != null)
		{
			DebugQs = "?debug=true";
		}
	}

</script>
	<title>WorldWide Telescope Embedded Control</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<link href="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/Content/CSS/wwt.min.css?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>" rel="stylesheet" />
	<link href="//maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" rel="stylesheet">
	<script src="//code.jquery.com/jquery-2.1.4.min.js"></script>
    <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js"></script>
	<script src="//cdnjs.cloudflare.com/ajax/libs/bootbox.js/4.4.0/bootbox.min.js"></script>
	<script src="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/Scripts/main.js?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>"></script>
	<script src="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/Scripts/controls/util.js?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>"></script>
    <script src="/html5sdk/1.0.0/wwtsdk.min.js"></script>
	<script src="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/Scripts/controls/WebControl.js?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>"></script>
	<script src="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/Scripts/controls/move.js?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>"></script>
	<script src="<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>/Scripts/controls/TourPlayer.js?v=<%= ConfigurationManager.AppSettings["ResourcesVersion"] %>"></script>
	<script>
		var ctl;
		$(window).on('load', function () {
			var options = $.parseJSON(location.hash.replace(/#/, ''));
			if (options.tour) {
				wwt.TourPlayer.init({ 
					resLoc: '<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>',
					tour: options.tour,
					remote: options
				});
			} else {
				wwt.WebControl.init({
					resLoc: '<%= ConfigurationManager.AppSettings["ResourcesLocation"] %>',
					remote: options
				});
			}
		    ctl = wwt.WebControl.ctl();
		     
			if (options.wtml) {
				
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
						$.each(wwtlib.ImageSetType, function (k, v) {
							if (!isNaN(v)) {
								imageSetTypes[v] = k.toLowerCase();
							}
						});
					}
					return imageSetTypes.indexOf(sType.toLowerCase()) === -1 ? 2 : imageSetTypes.indexOf(sType.toLowerCase());

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
				$.ajax({
					url: options.wtml
				}).done(function() {
					var wtml = $($.parseXML(arguments[0]));
					
					wtml.find('Folder').children().each(function(i, childnode) {
						var wwtPlace = createPlace(childnode);
						if (wwtPlace) {
							places.push(wwtPlace);
						}
						
					});
					bindData();
				});

					var getImageset = function(name) {
						var imgSet = null;
						$.each(places, function(i, place) {
							if (place.get_name() == name && place.get_studyImageset() != null) {
								imgSet = place.get_studyImageset();
							}
						});
						return imgSet;
					};

					var setBg = function(name) {
						var imgSet = getImageset(name);
						if (imgSet) {
							wwtlib.WWTControl.singleton.renderContext.set_backgroundImageset(imgSet);
						} else {
							ctl.setBackgroundImageByName(name);
						}
						top.postMessage({
						    wtmlEvent: {
						        setBackground: true,
						        imagery:name
						    }
						}, options.hostWindow);
					};
					var setFg = function(name) {
						var imgSet = getImageset(name);
						if (imgSet) {
							wwtlib.WWTControl.singleton.renderContext.set_foregroundImageset(imgSet);
						} else {
							ctl.setForegroundImageByName(name);
						}
						top.postMessage({
						    wtmlEvent: {
						        setForeground: true,
						        imagery: name
						    }
						}, options.hostWindow);
					};
					var xFade = options.display && options.display.toLowerCase() == 'cross fade';
					var dropdown = options.display && options.display.toLowerCase() == 'dropdown';
				var bindData = function () {
					
					if (xFade || dropdown) {
						var s1 = $('<select></select>');
						$.each(places, function() {
							var option = $('<option></option>').text(this.get_name()).val(this.get_name());
							s1.append(option);
						});
						
						var bgLbl = $('<label></label>').text(xFade ? 'Bg' : 'View:');
						
						$('#wwtLogo').after(bgLbl);
						bgLbl.after(s1);
						s1.val($(s1.find('option')[xFade ? 1 : 0] ).text());
						s1.on('change', function () {
							if (xFade) {
								setBg(s1.val());
							} else {
								setFg(s1.val());
							}
						});
						setTimeout(function() {
							if (xFade) {
								setBg(places[1].get_name());
							} else {
								setFg(places[0].get_name());
							}
						}, 100);
						if (xFade) {
							var fgLbl = $('<label>Fg</label>');
							var xFader = $('<div class="x-fader-wrapper"><div class="cross-fader"><a class="slider-bar" title="Cross-fade the foreground / background" style="position: absolute;">&nbsp;</a></div></div>');

							var s2 = s1.clone();
							s1.after(xFader);

							var bar = xFader.find('a.slider-bar');
							var xf = new wwt.Move({
								el: bar,
								bounds: {
									x: [-100, 0],
									y: [0, 0]
								},
								onstart: function() {
									bar.addClass('moving');
								},
								onmove: function() {
									ctl.setForegroundOpacity(this.css.left);
								},
								oncomplete: function() {
									bar.removeClass('moving');
								}
							});

							xFader.after(fgLbl);
							fgLbl.after(s2);
							
							s2.val(s2.find('option').first().text());
							
							s2.on('change', function () {
								setFg(s2.val());

							});
							
							setTimeout(function () {
								setFg(places[0].get_name());
								ctl.setForegroundOpacity(100);
							}, 200);
						}
					}
				};
			}
		});
	</script>
	<style>

		 html, body.remote-player {
			 height: 100%;
			 width: 100%;
			 margin: 0;
			 padding: 0;
			 overflow: hidden;
		 }
		.wwt-webcontrol-wrapper, #WorldWideTelescopeControlHost {
			margin: 0;
			height: 100%;
			width: 100%;
		}
		#WorldWideTelescopeControlHost .player-controls select {
			border-radius: 3px;
		}
		select, option {
			margin-top: 3px;
			width: 205px;
			background-color: transparent;
			background-image:linear-gradient(to bottom, rgba(123, 123, 123, 0.3) 0, rgba(0, 0, 0, 0.3) 100%);
			color: rgba(255, 255, 255, .75);
		}
		option {
			background-color: rgba(0, 0, 0, .75);
		}
		label {
			margin-top: 3px;
			font-weight: 100;
			display: inline-block;
			margin: 0 2px;
		}
		#WorldWideTelescopeControlHost .player-controls div.x-fader-wrapper {
			margin:0 22px
		}
		body.remote-player #WorldWideTelescopeControlHost .player-controls {
			background-image:linear-gradient(to bottom, rgba(123, 123, 123, 0.6) 0, rgba(0, 0, 0, 0.6) 100%);
		}
	</style>
</head>
<body class="remote-player">
	
	<div id="WorldWideTelescopeControlHost"></div>
</body>
</html>
