wwt.ImportImage = (function() {
	var api = {
		init: init
	};
	var ctl, wc, url;
	function init(resLoc) {
		wc = wwt.WebControl;
		wc.init({
			defaultSurvey: 'Digitized Sky Survey (Color)',
			resLoc: resLoc
		});
		ctl = wc.ctl();
		bindEvents();
		if (location.hash) {
			$('#txtImportImage').val(location.hash.replace(/#/g, ''));
			$('#btnImportImage').click();
		}
		initElements();
	}

	var bindEvents = function() {
		$('#btnEditImageCoordinates').on('click', function() {
			$('#AVMDataModal').modal();
			initElements();

		});
		$('#btnImportImage').on('click', loadExternalImage);
		$('#txtImportImage').on('keypress', function(e) {
			if (e.keyCode === 13) {
				e.stopPropagation();
				e.preventDefault();
				$('#btnImportImage').click();
			}
		});

		$('#btnAstrometryWCS, #retryAstrometry').on('click', function() {
			modalState.untouched = false,
				modalState.lookingInAstrometry = true,
				modalState.astrometryFail = false,
				modalState.astrometrySuccess = false,
				modalState.coordsSent = false;
			
		
			wwt.WCSService.submitImage(url, astrometryStatus);
			initElements();
		});

		$('#btnSaveAVMData').on('click', importWCSData);
		$('#txtImportImage').on('change', function() {
			location.href = '#' + $(this).val();
		});
		$('a.input-group-addon').on('click', function() {
			location.href = '#' + $('#txtImportImage').val();
		});
		
	};

	var modalState = {
		untouched:true,
		notFoundInControl: false,
		lookingInAstrometry: false,
		astrometryFail: false,
		astrometrySuccess: false,
		coordsSent:false
	}

	function initElements() {
		$('#wcNotfound').toggle(!modalState.lookingInAstrometry && modalState.notFoundInControl);
		$('#astrometryFail').toggle(!modalState.lookingInAstrometry && modalState.astrometryFail);
		$('#astrometrySuccess').toggle(!modalState.lookingInAstrometry && modalState.astrometrySuccess);
		$('#notFoundInfo').toggle(!modalState.lookingInAstrometry &&(modalState.untouched || modalState.notFoundInControl));
		$('#astrometryStatus').toggle(modalState.lookingInAstrometry);
		if (!modalState.lookingInAstrometry) {
			$('#astrometryStatusText').text('');
		}
		$('#WCSForm').toggle(!modalState.lookingInAstrometry);
		$('#AVMDataModal .modal-footer').toggle(!modalState.lookingInAstrometry);
	}

	/* (astrometry) statusTypes = {
		connecting: 'Connecting',
		connected: 'Connect Success',
		connectFail: 'Connection Failed',
		uploading: 'Uploading Image',
		uploadSuccess: 'Upload Success',
		uploadFail: 'Upload Failed',
		statusCheck: 'Checking Status',
		statusCheckFail: 'Status Check Failed',
		jobFound: 'Job Found',
		jobStatusCheck: 'Checking Job Status',
		jobFail: 'Could Not Resolve Image',
		jobStatus: 'Job Status',
		jobSuccess: 'Job Succeeded',
		calibrationFail: 'Calibration Results Failed'
	}*/
	var astrometryStatus = function (data) {
		if ($('#astrometryStatusText').text().indexOf(data.message) == 0) {
			$('#astrometryStatusText').text($('#astrometryStatusText').text() + ' .');
		} else {
			$('#astrometryStatusText').text(data.message);
		}
		if (data.calibration) {
			/*calibration.ra = result.ra; // in degrees devide 15 for hours
			calibration.dec = result.dec; // in degrees
			calibration.rotation = result.orientation;
			calibration.scale = result.pixscale;
			calibration.parity = result.parity;
			calibration.radius = result.radius;*/
			$('#txtRA').val(data.calibration.ra);
			$('#txtDec').val(data.calibration.dec);
			$('#txtScale').val(data.calibration.scale / 3600);
			$('#txtRotation').val(data.calibration.rotation);
			$('#chkReverse').prop('checked', data.calibration.parity != 1);
			modalState.lookingInAstrometry = false,
			modalState.astrometryFail = false,
			modalState.astrometrySuccess = true,
			modalState.coordsSent = false;

			initElements();
		}
		if (data.status.toLowerCase().indexOf('fail') != -1) {
			modalState.lookingInAstrometry = false,
			modalState.astrometryFail = true,
			modalState.astrometrySuccess = false,
			modalState.coordsSent = false;
			initElements();
		}
	};

	var loadExternalImage = function() {
		$('#divProgress').removeClass('hide').show();
		$('#divOpenInWWT').hide();
		url = $('#txtImportImage').val();
		wc.loadExternalImage(null, url, function(result) {
			$('#divProgress').hide();
			//$('#btnEditImageCoordinates').click();
			wwt.triggerResize();
			if (!result.success) {
				modalState.notFoundInControl = true;
				initElements();
				$('#AVMDataModal').modal();
				//$('#txtRA').val(202.45355674088898);
				//$('#txtDec').val(47.20018130592933);
				//$('#txtScale').val(0.3413275776344843 / 3600);//
				//$('#txtRotation').val(122.97953942448784);
				$('#txtRA').val(parseFloat(result.place.get_RA()) * 15);
				$('#txtDec').val(result.place.get_dec());
				$('#txtScale').val(result.imageSet.get_baseTileDegrees());
				$('#txtRotation').val(result.imageSet.get_rotation());
				$('#txtThumb').val(result.wtml.find('ThumbnailUrl').text());
				$('#txtCredits').val(result.wtml.find('Credits').text());
				$('#txtCreditsUrl').val(result.wtml.find('CreditsUrl').text());
			}
		});
		
	};

	

	var importWCSData = function() {
		var inputs = $('#AVMDataModal .modal-body *[data-param]');
		var qs = '';
		inputs.each(function (i, item) {
			var input = $(this);
			qs += '&';
			if (input.attr('type') === 'checkbox') {
				if (input.prop('checked')) {
					qs += 'reverseparity=true';
				}
			} else {
				if (input.data('param') === 'rotation') {
					qs += 'rotation=' + (parseFloat(input.val()));
				} else {
					qs += input.data('param') + '=' + encodeURIComponent(input.val());
				}

			}
		});

		wc.loadExternalImage(qs, url);
		modalState.coordsSent = true;
		initElements();
	};
	return api;
})();