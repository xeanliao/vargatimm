define([
	'jquery',
	'metisMenu',
	'alertify',
	'print/storage',
	'handlebars',
	'templates/print/footer',
	'templates/print/cover',
	'templates/print/campaign',
	'templates/print/campaignSummary',
	'templates/print/submap',
	'templates/print/submapDetail',
	'templates/print/submapDetailBody',
	'templates/print/dmap',
	'templates/print/tableOfContents',
	'templates/print/optionDialog'
], function ($, metisMenu, alertify, Storage, Handlebars, footerTemplate, coverTemplate, campaignTemplate, campaignSummaryTemplate, submapTemplate, submapDetailTemplate, submapDetailBodyTemplate, dmapTemplate, tableOfContentsTemplate, optionDialogTemplate) {

	function Print(campaignId, options) {
		this.campaignId = campaignId;
		$.extend(this.options, options);
		this.storage = new Storage(this.options.baseUrl, this.campaignId);
	};
	$.extend(Print.prototype, {
		options: {
			googleKey: "AIzaSyAKdtxTHEA7_knbSLYAw8iQDrKQ70_K7uI",
			targetMethod: '',
			distributorMap: [],
			gTUDotsRadii: 5,
			PenetrationColors: [20, 40, 60, 80]
		},
		progress: {
			total: 0,
			current: 0
		},
		showImageProgress: function(){
			this.progress.total = 0;
			this.progress.current = 0;
		},
		penetrationColorGroup: ['Blue', 'Green', 'Yellow', 'Orange', 'Red'],
		formatePenetrationColor: function(values){
			/**
			 * template exmaple:
			 * <div class="tips">COLOR LEGEND</div>
			 * <div>
			 *     <i class="blue"></i>
			 *     <label>Blue(0%-20%)</label>
			 *     <i class="green"></i>
			 *     <label>Green(20%-40%)</label>
			 *     <i class="yellow"></i>
			 *     <label>Yellow(40%-60%)</label>
			 *     <i class="orange"></i>
			 *     <label>Orange(60%-80%)</label>
			 *     <i class="red"></i>
			 *     <label>Red(80%-100%)</label>
			 * </div>
			 */
			var colors = [0].concat(values),
				html = '<div class="tips">COLOR LEGEND</div><div>',
				min = 0;
				colors.push(100);
			for (var i = 0; i < colors.length - 1; i++) {
				if (min >= colors[i + 1]) {
					continue;
				}
				
				html += '<i class="' + this.penetrationColorGroup[i].toLowerCase() + '"></i>';
				html += '<label>' + this.penetrationColorGroup[i] + '(' + colors[i] + '%-' + colors[i + 1] + '%)</label>';
				min = colors[i + 1];
			}
			return html + '</div>';
		},
		init: function () {
			var self = this;

			this.storage.loadCampaign()
				.done($.proxy(self.showOptions, self))
				.fail(function () {
					console.log('load campaign failed.');
					alertify.error("oops! we can not found this campaign.");
				});
			$(document).on('print.loadMapImage', $.proxy(this.loadMapImage, this));
			$(document).on('click', '.btnRetry', $.proxy(this.retry, this));
			$(document).on('retry.click', '.page', $.proxy(this.retry, this));
		},
		showOptions: function () {
			var self = this;
			this.storage.loadCampaign().done(function (val) {
				var allDmaps = [];
				$.each(val.SubMaps, function () {
					$.each(this.DMaps, function () {
						allDmaps.push({
							Id: this.Id,
							Name: this.Name
						});
					});
				});
				self.buildOptionDialog(allDmaps);
				if($('.page-container').hasClass('dmapPrint')){
					$('.hideForDistribute').hide();
				}
				if($('.page-container').hasClass('campaignPrint')){
					$('.hideForCampaign').hide();
				}
				$("#btnPrint").removeClass('disabled');
				$(document).one('closed.fndtn.reveal', '#optionDialog', function () {
					self.initPage();
				});
				$('#optionDialog').foundation('reveal', 'open');
			});
		},
		initPage: function () {
			console.log("onCampaignLoaded");
			var self = this;
			this.storage.loadCampaign().done(function (val) {
				self.campaign = val;
				self.campaign.targetMethod = self.options.targetMethod;
				//var footer = footerTemplate(val);
				Handlebars.registerPartial('footer', footerTemplate);

				var pageContainer = $('.page-container');
				pageContainer.addClass('A4');
				pageContainer.empty();
				/**
				 * build cover page
				 */
				if (!self.options.suppressCover) {
					var cover = coverTemplate(val);
					$('<div id="cover" class="page cover" role="cover">').html(cover).appendTo(pageContainer);
				}
				/**
				 * build campaign page
				 */
				if (!self.options.suppressCampaign) {
					var cmapign = campaignTemplate(val);
					var campginDom = $('<div id="campaign" class="page campaign" role="campaign">')
						.html(cmapign).appendTo(pageContainer);
					if(self.options.mapType){
						campginDom.data('MapOptions', {mapType: self.options.mapType});
					}else{
						campginDom.data('MapOptions', {mapType: 'ROADMAP'});
					}
						
					if(self.options.showPenetrationColors){
						campginDom.find('.map-container').attr('color', true);
						campginDom.find('.color-legend').html(self.formatePenetrationColor(self.options.PenetrationColors));
					}					
				}
				if (!self.options.suppressCampaign && !self.options.suppressCampaignSummary) {
					var campaignSummary = campaignSummaryTemplate(val);
					$('<div class="page campaign-summary" role="campaign-summary">').html(campaignSummary).appendTo(pageContainer);
				}
				/**
				 * build submap
				 */
				$.each(val.SubMaps, function () {
					var submapValue = {
						campaign: val,
						submap: this
					};
					if (!self.options.suppressSubMap) {
						var submap = submapTemplate(submapValue);
						var submapDom = $('<div id="submap-' + this.Id + '" class="page submap" role="submap">')
							.html(submap)
							.appendTo(pageContainer)
							.data('submapId', this.Id);

						if(self.options.mapType){
							submapDom.data('MapOptions', {mapType: self.options.mapType});
						}else{
							submapDom.data('MapOptions', {mapType: 'HYBRID'});
						}

						if(self.options.showPenetrationColors){
							submapDom.find('.map-container').attr('color', true);
							submapDom.find('.color-legend').html(self.formatePenetrationColor(self.options.PenetrationColors));
						}
					}

					if (!self.options.suppressSubMap && !self.options.suppressSubMapCountDetail) {
						var submapDetail = submapDetailTemplate(submapValue);
						$('<div id="submap-detail-' + this.Id + '" class="page submap-detail" role="submapDetail">')
							.html(submapDetail)
							.appendTo(pageContainer)
							.data('submapId', this.Id);

						self.storage.loadSubMapDetail(this.Id)
							.done($.proxy(self.onLoadSubMapDetail, self));
					}
				});
				// for render option dialog
				var allDmaps = [];
				/**
				 * build dmap
				 */
				$.each(val.SubMaps, function () {
					var subMap = this;
					$.each(this.DMaps, function () {
						allDmaps.push({
							Id: this.Id,
							Name: this.Name
						});

						if (!self.options.suppressDMap && (self.options.distributorMap.length == 0 || $.inArray(this.Id + '', self.options.distributorMap) == -1)) {
							var dmapValue = $.extend({}, val, {
								submap: subMap,
								dmap: this
							});
							var dmap = dmapTemplate(dmapValue);
							var dmapContainer = $('<div id="dmap-' + this.Id + '" class="page dmap" role="dmap">').html(dmap)
								.appendTo(pageContainer)
								.data('submapId', subMap.Id)
								.data('dmapId', this.Id);
							if(self.options.mapType){
								dmapContainer.data('MapOptions', {mapType: self.options.mapType});
							}else{
								dmapContainer.data('MapOptions', {mapType: 'HYBRID'});
							}

							/**
							 * fix map title
							 */
							if ((dmapValue.DisplayName + ' - ' + this.name).length > 70) {
								dmapContainer.find('.mapTitle span').css({
									"font-size": "12pt"
								});
							}
						}
					});
				});

				/**
				 * build table of contents
				 */
				var tableOfContents = tableOfContentsTemplate(val);
				$(".table-of-contents").html(tableOfContents);
				$(".table-of-contents").metisMenu();
				alertify.success("Map images loading, this may take a couple of minutes.");
				$(document).trigger("print.loadMapImage");
			});
		},
		buildOptionDialog: function (allDmaps) {
			console.log('begin build option dialog');
			var btnOption = $(".btnOptionDialog");
			var dialogVal = $.extend({}, this.options, {
				DMaps: allDmaps
			});
			var optionDialog = optionDialogTemplate(dialogVal);
			$('#optionDialog').html(optionDialog);
			btnOption.removeClass('disabled');			
		},
		loadMapImage: function () {
			console.log("begin load map images");
			this.showImageProgress();
			var self = this;
			if (!self.options.suppressCampaign) {
				var campaignPrams = $.extend({}, self.options, {
					type: "Campaign"
				}),
				campaignOption = $('#campaign').data('MapOptions');
				self.progress.total++;
				self.onLoadCampaignImage($.extend({}, campaignPrams, campaignOption));
			}
			$.each(this.campaign.SubMaps, function () {
				console.log("each load submap", this.Id);
				//begin load submap image
				var submapParams = {
					submapId: this.Id,
					type: "SubMap"
				},
				submapOption = $('#submap-' + this.Id).data('MapOptions');
				if (!self.options.suppressSubMap) {
					self.progress.total++;
					self.onLoadSubMapImage($.extend({}, self.options, submapParams, submapOption), this.Name);
				}
				//begin load dmap image
				$.each(this.DMaps, function () {
					var dmapParams = {
						dmapId: this.Id,
						type: "DMap"
					},
					dmapOption = $('#dmap-' + this.Id).data('MapOptions');
					if (!self.options.suppressDMap && (self.options.distributorMap.length == 0 || $.inArray(this.Id + '', self.options.distributorMap) == -1)) {
						self.progress.total++;
						self.onLoadDMapImage($.extend({}, self.options, submapParams, dmapParams, dmapOption), this.Name);
					}
				});
			});
		},
		onLoadCampaignImage: function (options) {
			var self = this;
			$.ajax({
				url: options.mapUrl,
				method: "POST",
				async: true,
				data: options,
				success: function (result) {
					console.log('load campaign image success');
					alertify.success("campaign map image load success!");
					if (result && result.success && result.campaignId) {
						console.log(result.campaignId);
						var container = $('#campaign .map-container');
						//container.wrap('<a href="#" data-reveal-id="mapEditor"></a>');
						var tiles = options.imgUrl + result.tiles;
						var geometry = '<img src="' + options.imgUrl + result.geometry + '" />'
						container.removeClass("loading hexdots-loader").html(geometry);
						container.css({
							'background-image': 'url(' + tiles + ')',
							'background-repeat': 'no-repeat',
							'background-size': '100% 100%',
							'background-position': '0 0'
						});
						alertify.success('Campaign map loaded!');
					} else {
						$('#campaign .map-container').html('<div class="btnRetry"><i class="fa fa-refresh fa-5x"></i></div>');
						alertify.error("load campaign image failed. please try again later!");
					}
				},
				error: function (e) {
					alertify.error("load campaign image failed. please try again later!");

				}
			});
		},
		onLoadSubMapImage: function (options, name) {
			var self = this;
			$.ajax({
				url: options.mapUrl,
				method: "POST",
				async: true,
				data: options,
				success: function (result) {
					console.log('load submap image success');

					if (result && result.success && result.submapId) {
						console.log(result.submapId);

						var container = $('#submap-' + result.submapId + ' .map-container');
						//container.wrap('<a href="#" data-reveal-id="mapEditor"></a>');
						var tiles = options.imgUrl + result.tiles;
						var geometry = '<img src="' + options.imgUrl + result.geometry + '" />'
						container.removeClass("loading hexdots-loader").html(geometry);
						container.css({
							'background-image': 'url(' + tiles + ')',
							'background-repeat': 'no-repeat',
							'background-size': '100% auto',
							'background-position': '0 0'
						});
						alertify.success('Sub Map ' + name + ' loaded!');
					} else {
						$('#submap-' + options.submapId + ' .map-container').html('<div class="btnRetry"><i class="fa fa-refresh fa-5x"></i></div>');
						alertify.error('Sub map ' + name + ' map image load failed. please try again later!');
					}
				},
				error: function (e) {
					console.log(e);
					alertify.error('Sub map ' + name + ' map image load failed. please try again later!');
					$('#submap-' + options.SubmapId + ' .map-container').html('<div class="btnRetry"><i class="fa fa-refresh fa-5x"></i></div>');
				}
			});
		},
		onLoadDMapImage: function (options, name) {
			$.ajax({
				url: options.mapUrl,
				method: "POST",
				async: true,
				data: options,
				success: function (result) {
					console.log('load dmap image success');
					if (result && result.success && result.dmapId) {
						console.log(result.dmapId);
						var container = $('#dmap-' + result.dmapId + ' .map-container');
						var tiles = options.imgUrl + result.tiles;
						var geometry = '<img src="' + options.imgUrl + result.geometry + '" />'
						container.removeClass("loading hexdots-loader").html(geometry);
						container.css({
							'background-image': 'url(' + tiles + ')',
							'background-repeat': 'no-repeat',
							'background-size': '100% auto',
							'background-position': '0 0'
						});
						alertify.success('Distribute map ' + name + ' loaded!');
					} else {
						$('#dmap-' + options.dmapId + ' .map-container').html('<div class="btnRetry"><i class="fa fa-refresh fa-5x"></i></div>');
						alertify.error('Distribute map ' + name + ' map image load failed. please try again later!');
					}
				},
				error: function (e) {
					console.log(e);
					$('#dmap-' + options.dmapId + ' .map-container').html('<div class="btnRetry"><i class="fa fa-refresh fa-5x"></i></div>');
					alertify.error('Distribute map ' + name + ' map image load failed. please try again later!');
				}
			});
		},
		onLoadSubMapDetail: function (result) {
			if (result && result.submapId) {
				var content = submapDetailBodyTemplate(result.record);
				var tableBody = $('#submap-detail-' + result.submapId + ' .detail-body');
				tableBody.html(content);
			}
		},
		retry: function (evt) {
			var page = $(evt.target).closest('.page'),
				mapOptions = page.data('MapOptions');
			page.find('.map-container').html('<div class="loading hexdots-loader"></div>');
			if (page.hasClass('campaign')) {
				var campaignPrams = $.extend({}, this.options, mapOptions, {
					type: "Campaign"
				});
				this.onLoadCampaignImage(campaignPrams);
			} else if (page.hasClass('submap')) {
				var submapParams = {
					submapId: page.data('submapId'),
					type: "SubMap"
				};
				this.onLoadSubMapImage($.extend({}, this.options, mapOptions, submapParams), $('.title', page).eq(0).text());
			} else if (page.hasClass('dmap')) {
				var dmapParams = {
					submapId: page.data('submapId'),
					dmapId: page.data('dmapId'),
					type: "DMap"
				};
				this.onLoadDMapImage($.extend({}, this.options, mapOptions, dmapParams), $('.title', page).eq(0).text());
			}
			return false;
		},
		editMap: function (event) {
			var self = this;
			var page = $(event.currentTarget).closest(".page");
			var editor = $('#mapEditor');
			var mapContainer = page.find('.map-container');
			if (mapContainer.find('.loading').size() > 0) {
				return;
			}
			editor.width(mapContainer.width());
			editor.height(mapContainer.height());

			var oldOption = page.data('option');
			oldOption = oldOption || {};

			if (page.hasClass('campaign')) {
				var option = $.extend({}, self.options, oldOption, {
					campaignId: self.campaignId,
					type: 'Cmapaign'
				});
				editor.data('option', option);
				editor.data('page', page);
				self.storage.getCampaignBundary().done(function (location) {
					editor.data('locations', location);
					editor.foundation('reveal', 'open');
				});
			} else if (page.hasClass('submap')) {
				var submapId = page.data('submapId');
				console.log('submapId', submapId);
				var option = $.extend({}, self.options, oldOption, {
					campaignId: self.campaignId,
					submapId: submapId,
					type: 'SubMap'
				});
				editor.data('option', option);
				editor.data('page', page);
				self.storage.getSubMapBundary(submapId).done(function (location) {
					editor.data('locations', location);
					editor.foundation('reveal', 'open');
				});

			} else if (page.hasClass('dmap')) {
				var submapId = page.data('submapId');
				var dmapId = page.data('dmapId');
				console.log('submapId', submapId);
				console.log('dmapId', dmapId);
				var option = $.extend({}, self.options, oldOption, {
					campaignId: self.campaignId,
					submapId: submapId,
					dmapId: dmapId,
					type: 'DMap'
				});
				editor.data('option', option);
				editor.data('page', page);
				self.storage.getDMapBundary(submapId, dmapId).done(function (location) {
					editor.data('locations', location);
					editor.foundation('reveal', 'open');
				});
			}
		}
	});
	return Print;
});