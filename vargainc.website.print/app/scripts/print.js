define([
	"jquery",
	'URI',
	'foundation',
	'alertify',
	'handlebars',
	'handlebars-helper-intl',
	'print/campaign',
	'print/distribute',
	'print/optionDialog',
	'print/mapOption',
	'print/map',
	'print/pdf',
	'jquery.scrollTo'
], function ($, URI, foundation, alertify, Handlebars, HandlebarsIntl, Campaign, Distribute, OptionDialog, MapOption, Map, Pdf) {
	$(function () {
		$('.main-section').css('min-height', $(window).height());
		$(document).foundation();
		var options = {
			//Local dev
			baseUrl: "http://local.timm.vargainc.com/api/",
			mapUrl: "http://local.timm.vargainc.com/map/",
			imgUrl: 'http://local.timm.vargainc.com/mapimg/',

			// baseUrl: "http://98.189.6.210:9000/timm201507/api/",
			// mapUrl: "http://54.67.112.52/map/",
			// imgUrl: 'http://54.67.112.52/mapimg/'

			//Timm dev
			// baseUrl: "http://98.189.6.210:9000/timm201507/api/",
			// mapUrl: "http://local.timm.vargainc.com/map/",
			// imgUrl: 'http://local.timm.vargainc.com/mapimg/',

			// Timm Ubuntu
			// baseUrl: "http://98.189.6.210:9000/timm201507/api/",
			// mapUrl: "http://98.189.6.213/map/",
			// imgUrl: 'http://98.189.6.213/mapimg/',

			// EC2
			// baseUrl: "http://52.8.230.211/201601/api/",
			// mapUrl: "http://54.67.112.52/map/",
			// imgUrl: 'http://ec2-54-67-112-52.us-west-1.compute.amazonaws.com/mapimg/'
		};
		var campaignOptions = {
				suppressCover: false,

				suppressCampaign: false,
				suppressCampaignSummary: false,
				suppressNDAInCampaign: false,

				suppressSubMap: false,
				suppressSubMapCountDetail: false,
				suppressNDAInSubMap: false,

				suppressDMap: true,

				showPenetrationColors: true,
				suppressLocations: false,
				suppressRadii: false,
			},
			dmapOptions = {
				suppressCover: true,

				suppressCampaign: true,
				suppressCampaignSummary: true,
				suppressNDAInCampaign: true,

				suppressSubMap: true,
				suppressSubMapCountDetail: true,
				suppressNDAInSubMap: true,

				suppressDMap: false,
				suppressGTU: true,
				suppressNDAInDMap: true,

				customSubMapPenetrationColors: false,
				suppressLocations: true,
				suppressRadii: true,
				mapType: 'ROADMAP', //ROADMAP, SATELLITE, HYBRID, TERRAIN
				zoom: 0
			},
			reportOptions = {
				suppressCover: false,

				suppressCampaign: false,
				suppressCampaignSummary: false,
				suppressNDAInCampaign: false,

				suppressSubMap: false,
				suppressSubMapCountDetail: false,
				suppressNDAInSubMap: false,

				suppressDMap: false,
				suppressGTU: false,

				showPenetrationColors: true,
				suppressLocations: false,
				suppressRadii: false,
				mapType: 'HYBRID', //ROADMAP, SATELLITE, HYBRID, TERRAIN
			};
		HandlebarsIntl.registerWith(Handlebars);
		Handlebars.registerHelper("math", function (lvalue, operator, rvalue, options) {
			lvalue = parseFloat(lvalue);
			rvalue = parseFloat(rvalue);

			return {
				"+": lvalue + rvalue,
				"-": lvalue - rvalue,
				"*": lvalue * rvalue,
				"/": lvalue / rvalue,
				"%": lvalue % rvalue
			}[operator];
		});

		var uriResult = new URI(location.href).search(true);
		console.log(uriResult);
		var campaignId = parseInt(uriResult.campaign);

		if (isNaN(campaignId)) {
			console.log("parse campaignId failed.");
			alertify.error("Oops! the campaign can not found.");
			return;
		}
		options.campaignId = campaignId;

		console.log("campaignId", campaignId);
		var printOptions = {},
			print;
		switch (uriResult.type) {
		case 'campaign':
			$('.page-container').addClass('campaignPrint');
			printOptions = $.extend({}, options, campaignOptions);
			print = new Campaign(campaignId, printOptions);
			break;
		case 'dmap':
			$('.page-container').addClass('dmapPrint');
			printOptions = $.extend({}, options, dmapOptions);
			print = new Distribute(campaignId, printOptions);
			break;
		case 'report':
			$('.page-container').addClass('reportPrint');
			printOptions = $.extend({}, options, reportOptions);
			print = new Campaign(campaignId, printOptions);
			break;
		default:
			printOptions = options;
			print = new Campaign(campaignId, printOptions);
			break;
		}

		print.init();

		var map = new Map();
		map.App = print;

		$(document).on('opened.fndtn.reveal', '#mapEditor', function () {
			console.log("map editor opend");
			map.init();
		});
		$(document).on('closed.fndtn.reveal', '#mapEditor', function () {
			console.log("map editor closed");
			map.close();
		});

		$(document).on('opened.fndtn.reveal', '#optionDialog', function () {
			$(this).data('option', print.options);
			OptionDialog.onOpen();
		});
		$(document).on('closed.fndtn.reveal', '#optionDialog', function () {
			OptionDialog.onClose();
		});
		$(document).on('click', ".btnApplyOption", function () {
			$(document).off('closed.fndtn.reveal', '#optionDialog');
			var option = OptionDialog.onApply();
			print.options = $.extend(print.options, option);
			console.log(option);
			print.initPage();
			$("#optionDialog").foundation('reveal', 'close');
		});
		$(document).on('click', ".btnCloseOption", function () {
			$("#optionDialog").foundation('reveal', 'close');
		});
		$(".btnOptionDialog").on("click", function () {
			$("#optionDialog").foundation('reveal', 'open');
		});

		MapOption.init(print.options);

		var pdf = new Pdf(print.options);

		pdf.init();

		$(document).on('click', '.metismenu a', function(){
			var target = $(this).attr('role'),
				targetEle = $(target);
			if(targetEle.size() > 0){
				$(window).scrollTo(targetEle);			
			}else{
				return false;
			}
		});

	});
});